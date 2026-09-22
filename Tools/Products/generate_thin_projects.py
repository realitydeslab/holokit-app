"""Create isolated Unity products from the package source tree, without editing siblings.

Run from this repository root. Existing generated projects are refused so manual
product branding and settings are never overwritten.
"""

import json
import argparse
import re
import shutil
import tempfile
import uuid
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
PACKAGES = ROOT / "Packages"
FULL_MANIFEST = json.loads((PACKAGES / "manifest.json").read_text())
PACKAGE_DOCS = {p.parent.name: json.loads(p.read_text()) for p in PACKAGES.glob("*/package.json")}
META = re.compile(r"^guid: ([0-9a-f]{32})", re.M)
SCENE = re.compile(r"    path: (.+\.unity)\n    guid: ([0-9a-f]{32})")
REF = re.compile(r"guid: ([0-9a-f]{32})")
SCENES = SCENE.findall((ROOT / "ProjectSettings/EditorBuildSettings.asset").read_text())
PRODUCTS = {
    "MOFA": ("MOFA", "org.realitydeslab.mofa", ("MOFATheTraining", "MOFATheDuel", "MOFATheHunting", "MOFATheDucks", "MOFATheGhost"), True),
    "Typed": ("Typed", "org.realitydeslab.typed", ("TypedTheScanner", "TypedTheFlock", "TypedTheFingerRibbon", "TypedTheRain", "TypedTheHair", "TypedTheSculpture", "TypedTheTornado"), False),
    "QRealm": ("QRealm", "org.realitydeslab.qrealm", ("QuantumRealm",), False),
}


def meta_guid(path):
    return META.search(Path(str(path) + ".meta").read_text())[1]


def new_guid(path):
    meta = Path(str(path) + ".meta")
    old = meta_guid(path)
    new = uuid.uuid4().hex
    meta.write_text(meta.read_text().replace("guid: " + old, "guid: " + new, 1))
    return old, new


def copy_asset(src, dst, fresh_guid=False):
    dst.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(src, dst)
    shutil.copy2(Path(str(src) + ".meta"), Path(str(dst) + ".meta"))
    return new_guid(dst) if fresh_guid else (meta_guid(src), meta_guid(dst))


def asset_index():
    result = {}
    for root in (ROOT / "Assets", PACKAGES):
        for meta in root.rglob("*.meta"):
            if "Library" in meta.parts or meta.name == "packages-lock.json.meta":
                continue
            match = META.search(meta.read_text(errors="ignore"))
            if match:
                result[match[1]] = Path(str(meta)[:-5])
    return result


def local_closure(game_packages):
    selected = set()
    pending = list(game_packages) + ["com.holoi.library.holokit-app-lib", "com.holoi.library.shared-assets"]
    while pending:
        package = pending.pop()
        if package in selected:
            continue
        assert package in PACKAGE_DOCS, "Missing local package: " + package
        selected.add(package)
        pending.extend(dep for dep in PACKAGE_DOCS[package].get("dependencies", {}) if dep in PACKAGE_DOCS)
    return selected


def copy_project_settings(destination, product, bundle, scene_names):
    shutil.copytree(ROOT / "ProjectSettings", destination / "ProjectSettings",
                    ignore=shutil.ignore_patterns("NatMLHub.asset", "NatShare.asset"))
    (destination / "ProjectSettings/ProjectVersion.txt").write_text((ROOT / "ProjectSettings/ProjectVersion.txt").read_text())
    settings = destination / "ProjectSettings/ProjectSettings.asset"
    content = settings.read_text()
    content = re.sub(r"^  productName: .+$", "  productName: " + product, content, count=1, flags=re.M)
    content = re.sub(r"^(    iPhone: )com\.holoi\.holokit\.holokit-app$", r"\g<1>" + bundle, content, count=1, flags=re.M)
    assert "  productName: " + product in content and "    iPhone: " + bundle in content
    settings.write_text(content)
    entries = [(path, guid) for path, guid in SCENES if Path(path).stem in ("Start", "NoLiDAR", *scene_names)]
    assert len(entries) == len(scene_names) + 2, entries
    build = destination / "ProjectSettings/EditorBuildSettings.asset"
    text = build.read_text()
    start, end = text.index("  m_Scenes:\n"), text.index("  m_configObjects:", text.index("  m_Scenes:\n"))
    scene_list = "  m_Scenes:\n" + "".join(f"  - enabled: 1\n    path: {path}\n    guid: {guid}\n" for path, guid in entries)
    text = text[:start] + scene_list + text[end:]
    text = re.sub(r"^    com\.unity\.addressableassets:.*\n", "", text, flags=re.M)
    build.write_text(text)


def own_app_icons(destination, index):
    if (destination / "Assets/Branding").exists():
        raise RuntimeError("Refusing to replace existing product branding: " + str(destination))
    settings = destination / "ProjectSettings/ProjectSettings.asset"
    content = settings.read_text()
    sources = {guid: path for guid in REF.findall(content) if (path := index.get(guid)) and
               "Assets/Images/AppIcon/" in str(path)}
    assert sources, "No iOS icon source assets were found."
    for guid, source in sources.items():
        target = destination / "Assets/Branding/AppIcon" / source.name
        _, replacement = copy_asset(source, target, True)
        content = content.replace("guid: " + guid, "guid: " + replacement)
    settings.write_text(content)


def copy_project_assets(destination, product, include_watch):
    source = ROOT / "Assets"
    target = destination / "Assets"
    for name in ("URP Assets", "XR", "TextMesh Pro", "Settings"):
        if not (source / name).exists():
            raise FileNotFoundError(f"Required Unity project assets are missing: {source / name}")
        shutil.copytree(source / name, target / name)
        if (source / (name + ".meta")).exists():
            shutil.copy2(source / (name + ".meta"), target / (name + ".meta"))
    for name in ("DefaultNetworkPrefabs.asset", "DefaultVolumeProfile.asset", "UniversalRenderPipelineGlobalSettings.asset"):
        copy_asset(source / name, target / name)
    (target / "Constants").mkdir()
    (target / "Resources").mkdir()
    mapping = {}
    for name in ("HoloKitAppConfig.asset", "AvailableRealityList.asset", "AvailableMetaAvatarCollectionList.asset", "AvailableMetaObjectCollectionList.asset"):
        old, new = copy_asset(source / "Constants" / name, target / "Constants" / name, True)
        mapping[old] = new
    config = target / "Constants/HoloKitAppConfig.asset"
    config.write_text(re.sub(r"^  WatchCompanionEnabled: [01]$", "  WatchCompanionEnabled: " + ("1" if include_watch else "0"), config.read_text(), flags=re.M))
    old, new = copy_asset(source / "Resources/HoloKitAppGlobalSettings.asset", target / "Resources/HoloKitAppGlobalSettings.asset", True)
    settings = target / "Resources/HoloKitAppGlobalSettings.asset"
    content = settings.read_text()
    for original, replacement in mapping.items():
        content = content.replace("guid: " + original, "guid: " + replacement)
    settings.write_text(content)
    return mapping


def own_reality_catalog(destination, scene_names, index):
    catalog = destination / "Assets/Constants/AvailableRealityList.asset"
    contents = catalog.read_text()
    refs = REF.findall(contents)
    assert len(refs) >= 13
    kept = []
    for guid in refs:
        source = index.get(guid)
        if source is None or source.suffix != ".asset" or source.parent == ROOT / "Assets/Constants":
            continue
        data = source.read_text(errors="ignore")
        scene = next((name for name in scene_names if "m_SceneName: " + name + "\n" in data), None)
        if scene is None:
            continue
        reality = destination / "Assets/Constants/Realities" / (scene + ".asset")
        _, reality_guid = copy_asset(source, reality, True)
        thumb = re.search(r"^  ThumbnailPrefab: \{fileID: (\d+), guid: ([0-9a-f]{32}), type: (\d+)\}", data, re.M)
        if thumb and thumb[2] in index:
            cover_source = index[thumb[2]]
            cover = destination / "Assets/Covers" / (scene + ".prefab")
            _, cover_guid = copy_asset(cover_source, cover, True)
            reality.write_text(data.replace("guid: " + thumb[2], "guid: " + cover_guid))
        kept.append((guid, reality_guid))
    assert len(kept) == len(scene_names), f"Catalog has {len(kept)} entries, expected {scene_names}"
    keep_map = dict(kept)
    lines = []
    for line in contents.splitlines(keepends=True):
        match = REF.search(line)
        if line.strip().startswith("- {fileID:") and match:
            if match[1] not in keep_map:
                continue
            line = line.replace("guid: " + match[1], "guid: " + keep_map[match[1]])
        lines.append(line)
    catalog.write_text("".join(lines))


def write_manifest(destination, selected):
    root = FULL_MANIFEST
    external = {k: v for k, v in root["dependencies"].items() if k not in PACKAGE_DOCS and
                k not in {"com.coplaydev.unity-mcp", "com.unity.ai.assistant", "com.holoi.missing-references-finder",
                          "com.unity.device-simulator.devices", "com.veriorpies.parrelsync", "com.unity.ide.visualstudio"}}
    # Unity resolves file: dependencies relative to the manifest's Packages/ directory.
    local = {k: "file:../../../Packages/" + k for k in sorted(selected)}
    manifest = {"scopedRegistries": root["scopedRegistries"], "dependencies": dict(sorted({**external, **local}.items())),
                "testables": ["com.holoi.library.holokit-app-lib", *sorted(k for k in selected if k.startswith("org.realitydeslab.") and (".the-" in k or k == "org.realitydeslab.quantum-realm"))]}
    (destination / "Packages").mkdir()
    (destination / "Packages/manifest.json").write_text(json.dumps(manifest, indent=2) + "\n")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--holokit-root", action="store_true", help="Make the full HoloKit catalog and covers product-owned once")
    parser.add_argument("--icons-only", action="store_true", help="Give existing products separate editable app icon assets")
    args = parser.parse_args()
    index = asset_index()
    if args.icons_only:
        for destination in (ROOT, *(ROOT / "Products" / label for label in PRODUCTS)):
            own_app_icons(destination, index)
            print("Owned app icons:", destination)
        return
    if args.holokit_root:
        if (ROOT / "Assets/Constants/Realities").exists() or (ROOT / "Assets/Covers").exists():
            raise SystemExit("Refusing to replace existing HoloKit product realities or covers.")
        all_scenes = tuple(Path(path).stem for path, _ in SCENES if Path(path).stem not in ("Start", "NoLiDAR"))
        own_reality_catalog(ROOT, all_scenes, index)
        print(f"HoloKit: {len(all_scenes)} product-owned realities and covers")
        return
    for label, (product, bundle, scene_names, watch) in PRODUCTS.items():
        destination = ROOT / "Products" / label
        if destination.exists():
            raise SystemExit(f"Refusing to overwrite existing product: {destination}")
        games = {Path(path).parts[1] for path, _ in SCENES if Path(path).stem in scene_names}
        selected = local_closure(games)
        with tempfile.TemporaryDirectory(prefix="." + label + "-", dir=destination.parent) as staging:
            staging = Path(staging)
            (staging / "Assets").mkdir()
            copy_project_assets(staging, product, watch)
            own_reality_catalog(staging, scene_names, index)
            copy_project_settings(staging, product, bundle, scene_names)
            own_app_icons(staging, index)
            write_manifest(staging, selected)
            if watch:
                shutil.copytree(ROOT / "WatchApp", staging / "WatchApp", ignore=shutil.ignore_patterns("xcuserdata", ".DS_Store", "TestHoloKitWatchApp.xcodeproj"))
            staging.rename(destination)
        print(f"{label}: {len(scene_names)} scenes, {len(selected)} local packages, watch={watch}")


if __name__ == "__main__":
    main()
