#!/usr/bin/env python3
"""Check an exported iOS Xcode project against its Unity product settings.

Usage: verify_ios_export.py --project <unity project> --export <xcode export dir> [--unity-log <export log>]

Checks the app bundle id, local-network and camera plist keys, the app icon,
and the Apple Watch target (present only when the product enables it, with the
product's bundle id, name, companion id, health text and icon). With --unity-log
it also fails on exceptions in the export log: Unity reports a successful build even
when a post-process step throws. Exits 1 on any failure.
"""
from __future__ import annotations

import argparse
import plistlib
import re
import sys
from pathlib import Path

from PIL import Image, ImageChops

WATCH_TARGET = "HoloKitWatchApp"
BONJOUR = {"_netcode-mpc._tcp", "_netcode-mpc._udp"}


def read_product(project: Path) -> dict:
    settings = (project / "ProjectSettings/ProjectSettings.asset").read_text()
    bundle = re.search(r"applicationIdentifier:\n(?:\s+\S+: \S*\n)*?\s+iPhone: (\S+)", settings)
    name = re.search(r"^\s+productName: (.+)$", settings, re.M)
    config = (project / "Assets/Constants/HoloKitAppConfig.asset").read_text()
    watch = re.search(r"WatchCompanionEnabled: (\d)", config)
    if not (bundle and name and watch):
        raise SystemExit(f"Cannot read product settings from {project}")
    return {
        "bundle_id": bundle.group(1),
        "name": name.group(1).strip(),
        "watch": watch.group(1) == "1",
        "icon": project / "Assets/Branding/AppIcon/1024.png",
    }


def build_configurations(pbxproj: str) -> list[dict[str, str]]:
    """Return the buildSettings of every XCBuildConfiguration as dicts."""
    configs = []
    for block in re.finditer(r"isa = XCBuildConfiguration;\s*buildSettings = \{(.*?)\n\t\t\t\};", pbxproj, re.S):
        values = {}
        for key, value in re.findall(r"^\s*([A-Za-z0-9_\[\]=*\"]+) = (.*?);\s*$", block.group(1), re.M):
            values[key.strip('"')] = value.strip().strip('"')
        configs.append(values)
    return configs


def same_pixels(a: Path, b: Path) -> bool:
    """Same picture after Unity/Xcode re-encoding: compare 64x64 thumbnails by mean difference."""
    left = Image.open(a).convert("RGB").resize((64, 64), Image.LANCZOS)
    right = Image.open(b).convert("RGB").resize((64, 64), Image.LANCZOS)
    histogram = ImageChops.difference(left, right).histogram()
    total = sum(value * (index % 256) for index, value in enumerate(histogram))
    return total / (64 * 64 * 3) < 6


def log_exceptions(log: Path) -> list[str]:
    """Exceptions thrown during the export, e.g. by [PostProcessBuild] steps."""
    found = re.findall(r"^(\w+(?:\.\w+)*Exception: .*)$", log.read_text(errors="replace"), re.M)
    return [f"Export log exception: {line[:200]}" for line in dict.fromkeys(found)]


def verify(project: Path, export: Path) -> list[str]:
    product = read_product(project)
    failures: list[str] = []
    pbxproj = (export / "Unity-iPhone.xcodeproj/project.pbxproj").read_text()
    configs = build_configurations(pbxproj)

    if not any(c.get("PRODUCT_BUNDLE_IDENTIFIER") == product["bundle_id"] for c in configs):
        failures.append(f"No Xcode configuration uses bundle id {product['bundle_id']}")

    with open(export / "Info.plist", "rb") as handle:
        info = plistlib.load(handle)
    missing = BONJOUR - set(info.get("NSBonjourServices", []))
    if missing:
        failures.append(f"Info.plist NSBonjourServices lacks {sorted(missing)}")
    for key in ("NSLocalNetworkUsageDescription", "NSCameraUsageDescription"):
        if not info.get(key):
            failures.append(f"Info.plist has no {key}")

    icons = sorted((export / "Unity-iPhone/Images.xcassets/AppIcon.appiconset").glob("*.png"),
                   key=lambda p: Image.open(p).size[0], reverse=True)
    if not icons:
        failures.append("Exported app has no AppIcon images")
    elif not same_pixels(product["icon"], icons[0]):
        failures.append(f"Exported app icon {icons[0].name} is not the product icon")

    watch = [c for c in configs if c.get("PRODUCT_NAME") == WATCH_TARGET]
    if not product["watch"]:
        if watch:
            failures.append("Watch is disabled for this product but the export has a Watch target")
        return failures
    if not watch:
        return failures + ["Watch is enabled but the export has no Watch target"]
    expected = {
        "PRODUCT_BUNDLE_IDENTIFIER": product["bundle_id"] + ".watch-app",
        "INFOPLIST_KEY_WKCompanionAppBundleIdentifier": product["bundle_id"],
        "INFOPLIST_KEY_CFBundleDisplayName": product["name"],
    }
    for config in watch:
        for key, value in expected.items():
            if config.get(key) != value:
                failures.append(f"Watch {key} is {config.get(key)!r}, expected {value!r}")
        for key in ("INFOPLIST_KEY_NSHealthShareUsageDescription", "INFOPLIST_KEY_NSHealthUpdateUsageDescription"):
            text = config.get(key, "")
            if not text:
                failures.append(f"Watch has no {key}")
            elif product["name"] != "HoloKit" and "HoloKit" in text:
                failures.append(f"Watch {key} names HoloKit in {product['name']}: {text!r}")
    watch_icon = export / "WatchApp/Assets.xcassets/AppIcon.appiconset/1024.png"
    if not watch_icon.exists():
        failures.append("Exported Watch app has no 1024 icon")
    elif not same_pixels(product["icon"], watch_icon):
        failures.append("Exported Watch icon is not the product icon")
    return sorted(set(failures))


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--project", type=Path, required=True)
    parser.add_argument("--export", type=Path, required=True)
    parser.add_argument("--unity-log", type=Path)
    args = parser.parse_args()
    failures = verify(args.project.resolve(), args.export.resolve())
    if args.unity_log:
        failures += log_exceptions(args.unity_log)
    label = f"{args.project.name or 'project'} -> {args.export}"
    if failures:
        print(f"FAIL {label}")
        for failure in failures:
            print("  - " + failure)
        return 1
    print(f"PASS {label}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
