#!/usr/bin/env python3
"""Read-only GUID and package audit; Unity loading remains the authoritative check."""
import argparse
import collections
import json
from pathlib import Path
import re
import subprocess
import tempfile

GUID = re.compile(r"guid:\s*([0-9a-f]{32})")
META_GUID = re.compile(r"^guid:\s*([0-9a-f]{32})", re.M)
REFERENCE_EXTENSIONS = {".unity", ".prefab", ".asset", ".mat", ".controller", ".overrideController", ".asmdef", ".asmref", ".vfx", ".shadergraph", ".shadersubgraph"}


def git(*args):
    return subprocess.check_output(["git", *args]).decode()


def audit(baseline):
    files = [Path(p) for p in git("ls-files", "-z", "Assets", "Packages", "ProjectSettings").split("\0") if p]
    files += [Path(p) for p in git("ls-files", "--others", "--exclude-standard", "-z", "Assets", "Packages").split("\0") if p]
    index = collections.defaultdict(list)
    # Cache entries help resolve registry packages; Unity's actual resolver must confirm them.
    metas = [p for p in files if p.suffix == ".meta"]
    metas += list(Path("Library/PackageCache").rglob("*.meta"))
    for p in metas:
        if not p.exists() or any(part.endswith("~") for part in p.parts):
            continue
        match = META_GUID.search(p.read_text(errors="replace"))
        if match and p.with_suffix("").exists():
            index[match[1]].append(str(p.with_suffix("")))
    missing = collections.defaultdict(list)
    orphan_scripts = []
    for p in files:
        if not p.exists() or any(part.endswith("~") for part in p.parts):
            continue
        if p.suffix == ".cs" and p.parts[0] == "Packages":
            parents = [p.parent, *p.parents[1:]]
            if not any(list(parent.glob("*.asmdef")) or list(parent.glob("*.asmref")) for parent in parents if parent != Path(".")):
                orphan_scripts.append(str(p))
        if p.suffix not in REFERENCE_EXTENSIONS:
            continue
        data = p.read_text(errors="replace")
        refs = set(GUID.findall(data))
        refs.update(re.findall(r"GUID:([0-9a-f]{32})", data))
        for guid in refs:
            # Unity's built-in resources use a zero-prefix reserved GUID.
            if guid not in index and not guid.startswith("0000000000000000"):
                missing[guid].append(str(p))
    baseline_paths = collections.defaultdict(list)
    if missing:
        with tempfile.NamedTemporaryFile(mode="w", suffix=".txt") as patterns:
            patterns.write("\n".join("guid: " + guid for guid in missing))
            patterns.flush()
            result = subprocess.run(["git", "grep", "-F", "-f", patterns.name, baseline, "--", "*.meta"], capture_output=True, text=True)
            if result.returncode not in (0, 1):
                raise RuntimeError(result.stderr)
            for line in result.stdout.splitlines():
                match = GUID.search(line)
                if match:
                    baseline_paths[match[1]].append(line.split(":", 2)[1].removesuffix(".meta"))
    packages = {}
    for p in sorted(Path("Packages").glob("*/package.json")):
        data = json.loads(p.read_text(encoding="utf-8-sig"))
        packages[data["name"]] = {"path": str(p.parent), "dependencies": data.get("dependencies", {})}
    return {
        "baseline": git("rev-parse", baseline).strip(),
        "head": git("rev-parse", "HEAD").strip(),
        "limitations": "Static GUID audit, includes package cache. Does not prove serialized fileID or runtime validity; omissions may predate the upgrade.",
        "missing_guids": [{"guid": g, "references": refs, "baseline_paths": baseline_paths[g]} for g, refs in sorted(missing.items())],
        "duplicate_project_guids": {g: [p for p in paths if not p.startswith("Library/")] for g, paths in index.items() if len([p for p in paths if not p.startswith("Library/")]) > 1},
        "package_scripts_without_assembly": orphan_scripts,
        "packages": packages,
    }


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--baseline", default="a00a971")
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    report = audit(args.baseline)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + "\n")
    print(json.dumps({"report": str(args.output), "missing_guids": len(report["missing_guids"]), "duplicate_project_guids": len(report["duplicate_project_guids"]), "package_scripts_without_assembly": len(report["package_scripts_without_assembly"])}))
