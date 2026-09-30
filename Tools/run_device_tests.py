#!/usr/bin/env python3
"""Build, install and run the development-build device tests on connected iPhones, unattended.

  Tools/run_device_tests.py --device B-UDID [--second-device I-UDID] [--products MOFA Typed] [--no-build]

Each test is started with its environment variable through `devicectl`; the script polls the test's
results.json in the app's data container until that run reports complete (or times out), then copies
the whole result folder (report and screenshots) to output/device-<timestamp>/. Two-phone tests run
only with --second-device. Exits non-zero if any build, install or test fails.

Phones must stay unlocked or have no passcode; the tests keep the screen awake while they run.
"""
from __future__ import annotations

import argparse
import json
import subprocess
import sys
import tempfile
import time
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
UNITY = str(Path.home() / ".unity/bin/unity")
EDITOR = "/Applications/Unity/6000.3.24f1/Unity.app"
TEAM = "KR9H35SQQ9"

PRODUCTS = {
    "HoloKit": {"project": REPO, "export": REPO / "Builds/Upgrade/HoloKit-four-products",
                "bundle": "com.holoi.holokit.holokit-app", "app": "HoloKit.app"},
    "MOFA": {"project": REPO / "Products/MOFA", "export": REPO / "Products/MOFA/Builds/iOS",
             "bundle": "org.realitydeslab.mofa", "app": "MOFA.app"},
    "Typed": {"project": REPO / "Products/Typed", "export": REPO / "Products/Typed/Builds/iOS",
              "bundle": "org.realitydeslab.typed", "app": "Typed.app"},
    "QRealm": {"project": REPO / "Products/QRealm", "export": REPO / "Products/QRealm/Builds/iOS",
               "bundle": "org.realitydeslab.qrealm", "app": "QRealm.app"},
}

# (product, name, environment, result folder, timeout seconds)
SINGLE_PHONE_TESTS = [
    ("QRealm", "qrealm-scenes", {"HOLOKIT_SCENE_TEST": "1"}, "HoloKitSceneTests", 300),
    ("QRealm", "qrealm-interaction", {"QREALM_INTERACTION_TEST": "1"}, "QRealmInteractionTests", 300),
    ("MOFA", "mofa-scenes", {"HOLOKIT_SCENE_TEST": "1"}, "HoloKitSceneTests", 600),
    ("MOFA", "mofa-hunting-spawn", {"MOFA_HUNTING_SPAWN_TEST": "1"}, "MofaHuntingSpawnTests", 300),
    ("MOFA", "mofa-combat", {"MOFA_COMBAT_TEST": "1"}, "MofaCombatTests", 400),
    ("MOFA", "mofa-combat-stereo", {"MOFA_COMBAT_TEST": "1", "MOFA_COMBAT_STEREO_TEST": "1"}, "MofaCombatTests", 400),
    ("Typed", "typed-scenes", {"HOLOKIT_SCENE_TEST": "1"}, "HoloKitSceneTests", 700),
    ("Typed", "typed-finger-ribbon", {"FINGER_RIBBON_INTERACTION_TEST": "1"}, "FingerRibbonInteractionTests", 300),
    ("Typed", "typed-sculpture", {"SCULPTURE_INTERACTION_TEST": "1"}, "SculptureInteractionTests", 300),
    ("HoloKit", "holokit-scenes", {"HOLOKIT_SCENE_TEST": "1"}, "HoloKitSceneTests", 1200),
]

# (product, name, role variable, result folder, timeout seconds)
TWO_PHONE_TESTS = [
    ("MOFA", "mofa-duel-two-phone", "MOFA_DUEL_TWO_PHONE_ROLE", "MofaDuelTwoPhoneTests", 400),
    ("MOFA", "mofa-hunting-two-phone", "MOFA_HUNTING_TWO_PHONE_ROLE", "MofaHuntingTwoPhoneTests", 400),
]


def log(out: Path, message: str) -> None:
    line = time.strftime("%H:%M:%S ") + message
    print(line, flush=True)
    with open(out / "progress.log", "a") as handle:
        handle.write(line + "\n")


def run(command: list[str], logfile: Path, timeout: int) -> int:
    with open(logfile, "w") as handle:
        try:
            return subprocess.run(command, stdout=handle, stderr=subprocess.STDOUT, timeout=timeout).returncode
        except subprocess.TimeoutExpired:
            handle.write(f"\nTIMEOUT after {timeout}s\n")
            return 124


def build(product: str, out: Path) -> Path | None:
    spec = PRODUCTS[product]
    code = run([UNITY, "build", str(spec["project"]), "--profile", "iOS", "--output-path", str(spec["export"]),
                "--editor-path", EDITOR, "--no-tail", "-l", str(out / f"{product}-export.log")],
               out / f"{product}-export.cli.txt", 3600)
    if code != 0:
        log(out, f"FAIL build {product}: Unity export exit {code}")
        return None
    derived = out / f"xcode-{product}"
    code = run(["xcodebuild", "-project", str(spec["export"] / "Unity-iPhone.xcodeproj"), "-scheme", "Unity-iPhone",
                "-configuration", "ReleaseForRunning", "-destination", "generic/platform=iOS",
                "-derivedDataPath", str(derived), "CODE_SIGN_STYLE=Automatic", f"DEVELOPMENT_TEAM={TEAM}",
                "-allowProvisioningUpdates", "-quiet"], out / f"{product}-xcode.log", 5400)
    app = derived / "Build/Products/ReleaseForRunning-iphoneos" / spec["app"]
    if code != 0 or not app.exists():
        log(out, f"FAIL build {product}: xcodebuild exit {code}")
        return None
    log(out, f"PASS build {product}")
    return app


def install(device: str, app: Path, out: Path, name: str) -> bool:
    code = run(["xcrun", "devicectl", "device", "install", "app", "--device", device, str(app)],
               out / f"install-{name}-{device[-6:]}.log", 600)
    log(out, f"{'PASS' if code == 0 else 'FAIL'} install {name} on {device[-6:]}")
    return code == 0


def launch(device: str, bundle: str, environment: dict[str, str], logfile: Path) -> bool:
    for attempt in range(3):
        code = run(["xcrun", "devicectl", "device", "process", "launch", "--device", device, "--terminate-existing",
                    "--environment-variables", json.dumps(environment), bundle], logfile, 120)
        if code == 0:
            return True
        time.sleep(15)
    return False


def fetch(device: str, bundle: str, source: str, destination: Path) -> bool:
    return subprocess.run(["xcrun", "devicectl", "device", "copy", "from", "--device", device,
                           "--domain-type", "appDataContainer", "--domain-identifier", bundle,
                           "--source", source, "--destination", str(destination)],
                          capture_output=True, timeout=180).returncode == 0


def read_report(device: str, bundle: str, folder: str) -> dict | None:
    with tempfile.TemporaryDirectory() as tmp:
        target = Path(tmp) / "results.json"
        if not fetch(device, bundle, f"Documents/{folder}/results.json", target):
            return None
        try:
            return json.loads(target.read_text())
        except json.JSONDecodeError:
            return None


def poll(device: str, bundle: str, folder: str, timeout: int, previous: dict | None) -> dict | None:
    """Waits for this run's report: complete and different from the one left by an earlier run.

    Some tests write results.json only when they finish, so an incomplete report is not required."""
    started, report, deadline = False, None, time.time() + timeout
    while time.time() < deadline:
        time.sleep(15)
        current = read_report(device, bundle, folder)
        if current is None or current == previous:
            continue
        report, started = current, True
        if report.get("complete"):
            return report
    return report if started else None


def verdict(report: dict | None) -> tuple[bool, str]:
    if report is None:
        return False, "no report from this run (app did not start the test?)"
    errors = report.get("errors") or []
    scenes = report.get("scenes") or []
    scene_errors = sum(len(s.get("errors") or []) for s in scenes) + len(report.get("startupErrors") or [])
    if not report.get("complete"):
        return False, f"timed out; {len(errors)} error(s)"
    if errors or scene_errors:
        first = (errors or [e for s in scenes for e in (s.get("errors") or [])] or report.get("startupErrors"))[0]
        return False, f"{len(errors) + scene_errors} error(s): {first.splitlines()[0][:160]}"
    return True, f"{len(scenes)} scene case(s)" if scenes else "complete"


def run_test(devices: list[str], product: str, name: str, environments: list[dict[str, str]], folder: str,
             timeout: int, out: Path) -> bool:
    bundle = PRODUCTS[product]["bundle"]
    previous = {device: read_report(device, bundle, folder) for device in devices}
    for device, environment in zip(devices, environments):
        if not launch(device, bundle, environment, out / f"{name}-launch-{device[-6:]}.log"):
            log(out, f"FAIL {name}: could not launch on {device[-6:]} (locked or disconnected?)")
            return False
        time.sleep(5)
    ok = True
    for device in devices:
        report = poll(device, bundle, folder, timeout, previous[device])
        destination = out / name / device[-6:]
        destination.mkdir(parents=True, exist_ok=True)
        fetch(device, bundle, f"Documents/{folder}", destination)
        passed, detail = verdict(report)
        ok &= passed
        log(out, f"{'PASS' if passed else 'FAIL'} {name} on {device[-6:]}: {detail}")
    return ok


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--device", required=True)
    parser.add_argument("--second-device")
    parser.add_argument("--products", nargs="+", default=["QRealm", "MOFA", "Typed", "HoloKit"])
    parser.add_argument("--tests", nargs="+", help="only these test names")
    parser.add_argument("--no-build", action="store_true", help="use the apps already installed")
    parser.add_argument("--out", type=Path, default=REPO / f"output/device-{time.strftime('%Y%m%d-%H%M%S')}")
    args = parser.parse_args()
    out = args.out
    out.mkdir(parents=True, exist_ok=True)
    devices = [args.device] + ([args.second_device] if args.second_device else [])
    ok = True

    for product in args.products:
        if not args.no_build:
            app = build(product, out)
            if app is None:
                ok = False
                continue
            for device in devices:
                ok &= install(device, app, out, product)
        for test_product, name, environment, folder, timeout in SINGLE_PHONE_TESTS:
            if test_product == product and (not args.tests or name in args.tests):
                ok &= run_test([args.device], product, name, [environment], folder, timeout, out)
        if args.second_device:
            for test_product, name, variable, folder, timeout in TWO_PHONE_TESTS:
                if test_product == product and (not args.tests or name in args.tests):
                    ok &= run_test(devices, product, name, [{variable: "host"}, {variable: "client"}], folder, timeout, out)
    log(out, "ALL PASSED" if ok else "SOME FAILED")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
