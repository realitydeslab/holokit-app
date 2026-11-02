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
import re
import shutil
import subprocess
import sys
import tempfile
import time
from pathlib import Path

import check_recording_audio
import check_recording_orientation

REPO = Path(__file__).resolve().parents[1]
UNITY = str(Path.home() / ".unity/bin/unity")
EDITOR = "/Applications/Unity/6000.3.27f1/Unity.app"
TEAM = "KR9H35SQQ9"
STALLED_POLLS = 3

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
    ("MOFA", "mofa-single", {"MOFA_SINGLE_TEST": "1"}, "MofaSinglePlayerTests", 400),
    # Settings > Record Microphone off: the recording carries the game track alone.
    ("MOFA", "mofa-single-no-mic", {"MOFA_SINGLE_TEST": "1", "MOFA_SINGLE_RECORD_MICROPHONE": "0"},
     "MofaSinglePlayerTests", 400),
    # MR recordings the right way up (Tools/check_recording_orientation.py). The phone must stay still and see a
    # detailed scene, best in a HoloKit headset standing upright; a plain ceiling or desk fails with "cannot tell".
    ("MOFA", "mofa-stereo-recording", {"MOFA_STEREO_RECORDING_TEST": "1"}, "MofaStereoRecordingTests", 300),
    # Needs a person wearing the paired Apple Watch to swing during a 90 s window.
    ("MOFA", "mofa-single-real-watch", {"MOFA_SINGLE_TEST": "1", "MOFA_SINGLE_REAL_WATCH": "1"}, "MofaSinglePlayerTests", 600),
    ("Typed", "typed-scenes", {"HOLOKIT_SCENE_TEST": "1"}, "HoloKitSceneTests", 700),
    ("Typed", "typed-finger-ribbon", {"FINGER_RIBBON_INTERACTION_TEST": "1"}, "FingerRibbonInteractionTests", 300),
    ("Typed", "typed-sculpture", {"SCULPTURE_INTERACTION_TEST": "1"}, "SculptureInteractionTests", 300),
    ("HoloKit", "holokit-scenes", {"HOLOKIT_SCENE_TEST": "1"}, "HoloKitSceneTests", 1200),
    # The gallery idles at 60 fps and scrolls through every room without hitches. No person needed.
    ("HoloKit", "holokit-gallery-frame-rate", {"HOLOKIT_GALLERY_PERF": "1"}, "HoloKitGalleryPerf", 300),
]

# (product, name, role variable, result folder, timeout seconds)
TWO_PHONE_TESTS = [
    ("MOFA", "mofa-duel-two-phone", "MOFA_DUEL_TWO_PHONE_ROLE", "MofaDuelTwoPhoneTests", 400),
    ("MOFA", "mofa-hunting-two-phone", "MOFA_HUNTING_TWO_PHONE_ROLE", "MofaHuntingTwoPhoneTests", 400),
    # The host starts Training before it shows its QR code; a spectator joining afterwards must still see the AI avatar.
    ("MOFA", "mofa-training-late-spectator", "MOFA_TRAINING_LATE_SPECTATOR_ROLE", "MofaTrainingLateSpectatorTests", 400),
    # The app's Network Manager in an empty toy scene: connect, round trips, poses, leave; five times. No person needed.
    ("HoloKit", "holokit-toy-transport", "HOLOKIT_TOY_TRANSPORT_ROLE", "ToyTransportTests", 420),
    ("MOFA", "mofa-toy-transport", "HOLOKIT_TOY_TRANSPORT_ROLE", "ToyTransportTests", 420),
    ("QRealm", "qrealm-toy-transport", "HOLOKIT_TOY_TRANSPORT_ROLE", "ToyTransportTests", 420),
    ("Typed", "typed-toy-transport", "HOLOKIT_TOY_TRANSPORT_ROLE", "ToyTransportTests", 420),
    # The host rejects the client's first connection request; the client must try again and join. No camera needed.
    ("HoloKit", "holokit-reconnect", "HOLOKIT_RECONNECT_ROLE", "ReconnectTests", 300),
    # Every reality: the host shows its QR code, each client joins (players and spectators in turn), scans it and taps
    # the check button. The clients' cameras must see the host's screen.
    ("HoloKit", "holokit-multiplayer-scenes", "HOLOKIT_MULTIPLAYER_SCENES_ROLE", "MultiplayerSceneTests", 3600),
    # The client scans the host's QR code three times as a spectator; after each alignment it tracks the code again,
    # so where it sees the code can be compared with where the host says it is (Tools/analyze_qr_alignment.py).
    ("HoloKit", "holokit-qr-alignment", "HOLOKIT_QR_ALIGNMENT_ROLE", "QRAlignmentTests", 600),
]

# Role variable -> the variable that tells the host how many clients to wait for. Only these tests take more devices.
CLIENT_COUNT_VARIABLES = {"HOLOKIT_TOY_TRANSPORT_ROLE": "HOLOKIT_TOY_TRANSPORT_CLIENTS",
                          "HOLOKIT_MULTIPLAYER_SCENES_ROLE": "HOLOKIT_MULTIPLAYER_SCENES_CLIENTS",
                          "MOFA_HUNTING_TWO_PHONE_ROLE": "MOFA_HUNTING_TWO_PHONE_CLIENTS",
                          "HOLOKIT_QR_ALIGNMENT_ROLE": "HOLOKIT_QR_ALIGNMENT_CLIENTS"}


def host_and_clients(devices: list[str], more: list[str], variable: str) -> tuple[list[str], list[dict[str, str]]]:
    """The devices of a host-and-clients test and their environments: extra devices join as clients, numbered from 1,
    when the test counts its clients; otherwise it keeps the first two."""
    if variable not in CLIENT_COUNT_VARIABLES or not more:
        return devices, [{variable: "host"}, {variable: "client"}]
    group = devices + more
    host = {variable: "host", CLIENT_COUNT_VARIABLES[variable]: str(len(group) - 1)}
    return group, [host] + [{variable: "client", "HOLOKIT_CLIENT_INDEX": str(i)} for i in range(1, len(group))]


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


def signing_key_args(config: Path = Path.home() / ".appstoreconnect/config.json") -> list[str]:
    """xcodebuild flags that let automatic signing fetch profiles with the App Store Connect API key.

    Xcode on the build Mac has no signed-in account, so without them -allowProvisioningUpdates fails with
    "No Accounts" whenever a profile is missing or out of date. Empty when the key's config is absent."""
    if not config.exists():
        return []
    settings = json.loads(config.read_text())
    return ["-authenticationKeyPath", str(Path(settings["key_path"]).expanduser()),
            "-authenticationKeyID", settings["key_id"], "-authenticationKeyIssuerID", settings["issuer_id"]]


def build(product: str, out: Path) -> Path | None:
    spec = PRODUCTS[product]
    # Unity appends to an existing Xcode project, which keeps what the older export linked and declared: after the move
    # to the Network framework transport, the removed Multipeer plugin still failed the link. Export afresh every time.
    if spec["export"].exists():
        shutil.rmtree(spec["export"])
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
                "-allowProvisioningUpdates", *signing_key_args(), "-quiet"], out / f"{product}-xcode.log", 5400)
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
    # A locked phone refuses every launch ("Locked"), and iOS refuses a freshly installed app for a few minutes while
    # it verifies it ("Security"); keep trying for 10 minutes in those cases, 3 tries otherwise.
    attempt = 0
    while True:
        code = run(["xcrun", "devicectl", "device", "process", "launch", "--device", device, "--terminate-existing",
                    "--environment-variables", json.dumps(environment), bundle], logfile, 120)
        if code == 0:
            return True
        attempt += 1
        waiting = re.search(r"for reason: (Locked|Security)", logfile.read_text(errors="replace"))
        if attempt >= (40 if waiting else 3):
            return False
        time.sleep(15)


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

    Some tests write results.json only when they finish, so an incomplete report is not required.
    A test that stopped on an error without marking itself complete is taken as finished once its
    report has errors and has not changed for STALLED_POLLS polls."""
    started, report, stalled, deadline = False, None, 0, time.time() + timeout
    while time.time() < deadline:
        time.sleep(15)
        current = read_report(device, bundle, folder)
        if current is None or current == previous:
            continue
        stalled = stalled + 1 if current == report else 0
        report, started = current, True
        if report.get("complete") or (report.get("errors") and stalled >= STALLED_POLLS):
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


def expects_microphone(environment: dict[str, str]) -> bool:
    """Whether a test's recordings must carry the microphone: always, unless the test turns Record Microphone off."""
    return environment.get("MOFA_SINGLE_RECORD_MICROPHONE") != "0"


def check_recordings(folder: Path, passed: bool, detail: str, microphone: bool = True) -> tuple[bool, str]:
    """Adds the audio check of every recording a test left: mix, game and microphone tracks, or the game alone when the
    test turned Record Microphone off; each with sound. The expectation comes from the test, not from the device."""
    for movie in sorted(folder.rglob("recording.mp4")):
        if shutil.which("ffprobe") is None:
            return passed, f"{detail}; {movie.name} not checked (no ffprobe)"
        issues = check_recording_audio.problems(movie, microphone)
        if issues:
            passed, detail = False, f"{detail}; {movie.name}: {issues[0]}"
    return passed, detail


def check_orientation(folder: Path, passed: bool, detail: str) -> tuple[bool, str]:
    """Adds the orientation check of a stereo recording test's movies (Tools/check_recording_orientation.py)."""
    for report in sorted(folder.rglob("results.json")):
        if not (report.parent / "stereo.mp4").exists():
            continue
        # The device test asserts nothing about orientation itself: without this check it has not passed.
        if shutil.which("ffmpeg") is None:
            return False, f"{detail}; orientation not checked (no ffmpeg)"
        try:
            _, issues = check_recording_orientation.problems(report.parent)
        except (subprocess.CalledProcessError, ValueError, KeyError, OSError) as error:
            issues = [f"orientation not checked: {error}"]
        if issues:
            passed, detail = False, f"{detail}; {issues[0]}"
    return passed, detail


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
    for device, environment in zip(devices, environments):
        report = poll(device, bundle, folder, timeout, previous[device])
        destination = out / name / device[-6:]
        destination.mkdir(parents=True, exist_ok=True)
        fetch(device, bundle, f"Documents/{folder}", destination)
        passed, detail = verdict(report)
        passed, detail = check_recordings(destination, passed, detail, expects_microphone(environment))
        passed, detail = check_orientation(destination, passed, detail)
        ok &= passed
        log(out, f"{'PASS' if passed else 'FAIL'} {name} on {device[-6:]}: {detail}")
    return ok


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--device", required=True)
    parser.add_argument("--second-device")
    parser.add_argument("--more-devices", nargs="+", default=[],
                        help="extra clients for the tests whose host counts clients (the toy transport test)")
    parser.add_argument("--client-env", nargs="+", default=[], metavar="KEY=VALUE",
                        help="extra environment for every client of a host-and-clients test, e.g. "
                             "HOLOKIT_CLIENT_INDEX=2 to join as a spectator")
    parser.add_argument("--products", nargs="+", default=["QRealm", "MOFA", "Typed", "HoloKit"])
    parser.add_argument("--tests", nargs="+", help="only these test names")
    parser.add_argument("--no-build", action="store_true", help="use the apps already installed")
    parser.add_argument("--in-app", choices=sorted(PRODUCTS),
                        help="run every selected test inside this one app (the HoloKit app has every reality), "
                             "so a phone grants camera access once")
    parser.add_argument("--out", type=Path, default=REPO / f"output/device-{time.strftime('%Y%m%d-%H%M%S')}")
    args = parser.parse_args()
    out = args.out
    out.mkdir(parents=True, exist_ok=True)
    devices = [args.device] + ([args.second_device] if args.second_device else [])
    more = args.more_devices if args.second_device else []
    client_env = dict(pair.split("=", 1) for pair in args.client_env)
    ok = True

    if args.in_app:
        # Tests find their reality by scene name in the app's catalog, so they run in any app that has it. The
        # in-app product's scene test covers its whole catalog, replacing the other products' scene tests.
        if not args.no_build:
            app = build(args.in_app, out)
            if app is None:
                log(out, "SOME FAILED")
                return 1
            for device in devices + more:
                ok &= install(device, app, out, args.in_app)
        for test_product, name, environment, folder, timeout in SINGLE_PHONE_TESTS:
            selected = test_product in args.products and (not args.tests or name in args.tests)
            if selected and (not name.endswith("-scenes") or test_product == args.in_app):
                ok &= run_test([args.device], args.in_app, name, [environment], folder, timeout, out)
        if args.second_device:
            for test_product, name, variable, folder, timeout in TWO_PHONE_TESTS:
                if test_product in args.products and (not args.tests or name in args.tests):
                    group, environments = host_and_clients(devices, more, variable)
                    environments = environments[:1] + [{**env, **client_env} for env in environments[1:]]
                    ok &= run_test(group, args.in_app, name, environments, folder, timeout, out)
        log(out, "ALL PASSED" if ok else "SOME FAILED")
        return 0 if ok else 1

    for product in args.products:
        if not args.no_build:
            app = build(product, out)
            if app is None:
                ok = False
                continue
            for device in devices + more:
                ok &= install(device, app, out, product)
        for test_product, name, environment, folder, timeout in SINGLE_PHONE_TESTS:
            if test_product == product and (not args.tests or name in args.tests):
                ok &= run_test([args.device], product, name, [environment], folder, timeout, out)
        if args.second_device:
            for test_product, name, variable, folder, timeout in TWO_PHONE_TESTS:
                if test_product == product and (not args.tests or name in args.tests):
                    group, environments = host_and_clients(devices, more, variable)
                    environments = environments[:1] + [{**env, **client_env} for env in environments[1:]]
                    ok &= run_test(group, product, name, environments, folder, timeout, out)
    log(out, "ALL PASSED" if ok else "SOME FAILED")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
