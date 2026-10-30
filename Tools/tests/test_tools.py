"""Tests for Tools/verify_ios_export.py, Tools/run_all_tests.sh and Tools/run_device_tests.py.

Run: python3 -m unittest discover -s Tools/tests
"""
from __future__ import annotations

import os
import plistlib
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from unittest import mock

from PIL import Image

TOOLS = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(TOOLS))
import run_device_tests  # noqa: E402
import verify_ios_export  # noqa: E402

BUNDLE = "org.example.mofa"
RED, BLUE = (220, 30, 30), (30, 30, 220)


def icon(path: Path, color, size=1024):
    path.parent.mkdir(parents=True, exist_ok=True)
    Image.new("RGB", (size, size), color).save(path)


def config_block(settings: dict[str, str]) -> str:
    lines = "".join(f'\t\t\t\t{key} = "{value}";\n' for key, value in settings.items())
    return f"\t\tA = {{\n\t\t\tisa = XCBuildConfiguration;\n\t\t\tbuildSettings = {{\n{lines}\t\t\t}};\n\t\t}};\n"


class Fixture:
    """A minimal Unity product and its Xcode export that pass every check."""

    def __init__(self, root: Path, watch=True, name="MOFA"):
        self.project, self.export = root / "project", root / "export"
        (self.project / "ProjectSettings").mkdir(parents=True)
        (self.project / "ProjectSettings/ProjectSettings.asset").write_text(
            f"PlayerSettings:\n  productName: {name}\n  applicationIdentifier:\n    Android: x\n    iPhone: {BUNDLE}\n")
        (self.project / "Assets/Constants").mkdir(parents=True)
        (self.project / "Assets/Constants/HoloKitAppConfig.asset").write_text(
            f"  WatchCompanionEnabled: {1 if watch else 0}\n")
        icon(self.project / "Assets/Branding/AppIcon/1024.png", RED)
        icon(self.export / "Unity-iPhone/Images.xcassets/AppIcon.appiconset/Icon-Store-1024.png", RED)
        icon(self.export / "Unity-iPhone/Images.xcassets/AppIcon.appiconset/Icon-iPhone-120.png", RED, 120)
        self.main = {"PRODUCT_NAME": "ProductName", "PRODUCT_BUNDLE_IDENTIFIER": BUNDLE}
        self.watch = {
            "PRODUCT_NAME": "HoloKitWatchApp",
            "PRODUCT_BUNDLE_IDENTIFIER": BUNDLE + ".watch-app",
            "INFOPLIST_KEY_WKCompanionAppBundleIdentifier": BUNDLE,
            "INFOPLIST_KEY_CFBundleDisplayName": name,
            "INFOPLIST_KEY_NSHealthShareUsageDescription": "Your workout data will be displayed.",
            "INFOPLIST_KEY_NSHealthUpdateUsageDescription": f"Workouts tracked by {name} will be saved.",
        }
        self.has_watch_target = watch
        self.watch_privacy_in_resources = watch
        if watch:
            icon(self.export / "WatchApp/Assets.xcassets/AppIcon.appiconset/1024.png", RED)
            self.write_dir("WatchApp/PrivacyInfo.xcprivacy").write_text("<plist/>")
        with open(self.write_dir("Info.plist"), "wb") as handle:
            plistlib.dump({
                "NSBonjourServices": ["_netcode-mpc._tcp", "_netcode-mpc._udp"],
                "NSLocalNetworkUsageDescription": "Nearby devices",
                "NSCameraUsageDescription": "AR",
            }, handle)

    def write_dir(self, relative: str) -> Path:
        path = self.export / relative
        path.parent.mkdir(parents=True, exist_ok=True)
        return path

    def verify(self) -> list[str]:
        blocks = config_block(self.main) + (config_block(self.watch) if self.has_watch_target else "")
        if self.watch_privacy_in_resources:
            blocks += "\t\tA1 /* PrivacyInfo.xcprivacy in Resources */ = {isa = PBXBuildFile; fileRef = A2; };\n"
        self.write_dir("Unity-iPhone.xcodeproj/project.pbxproj").write_text(
            "// !$*UTF8*$!\n{\n\tobjects = {\n" + blocks + "\t};\n}\n")
        return verify_ios_export.verify(self.project, self.export)


class VerifyIosExportTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.root = Path(self.tmp.name)

    def tearDown(self):
        self.tmp.cleanup()

    def test_matching_export_passes(self):
        self.assertEqual(Fixture(self.root).verify(), [])

    def test_health_text_naming_holokit_in_another_product_fails(self):
        fixture = Fixture(self.root)
        fixture.watch["INFOPLIST_KEY_NSHealthUpdateUsageDescription"] = "Workouts tracked by HoloKit app."
        self.assertTrue(any("names HoloKit" in f for f in fixture.verify()))

    def test_holokit_product_may_name_holokit(self):
        fixture = Fixture(self.root, name="HoloKit")
        fixture.watch["INFOPLIST_KEY_NSHealthUpdateUsageDescription"] = "Workouts tracked by HoloKit app."
        self.assertEqual(fixture.verify(), [])

    def test_app_icon_other_than_product_icon_fails(self):
        fixture = Fixture(self.root)
        icon(fixture.export / "Unity-iPhone/Images.xcassets/AppIcon.appiconset/Icon-Store-1024.png", BLUE)
        self.assertIn("Exported app icon Icon-Store-1024.png is not the product icon", fixture.verify())

    def test_stale_watch_icon_fails(self):
        fixture = Fixture(self.root)
        icon(fixture.export / "WatchApp/Assets.xcassets/AppIcon.appiconset/1024.png", BLUE)
        self.assertIn("Exported Watch icon is not the product icon", fixture.verify())

    def test_watch_without_privacy_manifest_fails(self):
        fixture = Fixture(self.root)
        (fixture.export / "WatchApp/PrivacyInfo.xcprivacy").unlink()
        self.assertIn("Exported Watch app has no PrivacyInfo.xcprivacy", fixture.verify())

    def test_watch_privacy_manifest_outside_resources_fails(self):
        fixture = Fixture(self.root)
        fixture.watch_privacy_in_resources = False
        self.assertIn("PrivacyInfo.xcprivacy is not in a Resources build phase", fixture.verify())

    def test_watch_bundle_id_must_derive_from_app(self):
        fixture = Fixture(self.root)
        fixture.watch["PRODUCT_BUNDLE_IDENTIFIER"] = "com.holoi.holokit.holokit-app.watch-app"
        self.assertTrue(any("PRODUCT_BUNDLE_IDENTIFIER" in f for f in fixture.verify()))

    def test_watch_target_in_product_without_watch_fails(self):
        fixture = Fixture(self.root, watch=False)
        fixture.has_watch_target = True
        self.assertIn("Watch is disabled for this product but the export has a Watch target", fixture.verify())

    def test_missing_bonjour_service_fails(self):
        fixture = Fixture(self.root)
        with open(fixture.export / "Info.plist", "wb") as handle:
            plistlib.dump({"NSBonjourServices": ["_netcode-mpc._tcp"], "NSLocalNetworkUsageDescription": "x",
                           "NSCameraUsageDescription": "x"}, handle)
        self.assertTrue(any("_netcode-mpc._udp" in f for f in fixture.verify()))

    def test_exception_in_export_log_is_reported(self):
        log = self.root / "export.log"
        log.write_text("Build Finished, Result: Success.\nArgumentException: An item with the same key has already been added.\n")
        self.assertEqual(len(verify_ios_export.log_exceptions(log)), 1)
        log.write_text("Build Finished, Result: Success.\n")
        self.assertEqual(verify_ios_export.log_exceptions(log), [])


class AssetSnapshotTests(unittest.TestCase):
    def test_reports_changed_assets_and_ignores_xr_settings_and_non_assets(self):
        with tempfile.TemporaryDirectory() as tmp:
            repo = Path(tmp) / "repo"
            (repo / "Products/MOFA/Assets/XR").mkdir(parents=True)
            (repo / "Packages/game").mkdir(parents=True)
            (repo / "Packages/game/health.mat").write_text("_Health: 1\n")
            (repo / "Tools").mkdir()
            (repo / "Tools/run.sh").write_text("echo 1\n")
            (repo / "Products/MOFA/Assets/XR/settings.asset").write_text("a\n")
            git = lambda *args: subprocess.run(["git", "-C", str(repo), *args], check=True, capture_output=True)
            git("init", "-q"); git("add", "."); git("-c", "user.email=t@t", "-c", "user.name=t", "commit", "-qm", "init")
            env = dict(os.environ, ASSET_SNAPSHOT_REPO=str(repo))
            script = str(TOOLS / "asset_snapshot.sh")
            snapshot = Path(tmp) / "before.txt"
            subprocess.run([script, "save", str(snapshot)], env=env, check=True)
            (repo / "Products/MOFA/Assets/XR/settings.asset").write_text("b\n")
            self.assertEqual(subprocess.run([script, "check", str(snapshot)], env=env).returncode, 0)
            (repo / "Tools/run.sh").write_text("echo 2\n")
            self.assertEqual(subprocess.run([script, "check", str(snapshot)], env=env).returncode, 0, "scripts are not assets")
            (repo / "Packages/game/health.mat").write_text("_Health: 0.98\n")
            result = subprocess.run([script, "check", str(snapshot)], env=env, capture_output=True, text=True)
            self.assertEqual(result.returncode, 1)
            self.assertEqual(result.stdout.strip(), "Packages/game/health.mat")


class RunAllTestsScriptTests(unittest.TestCase):
    def run_script(self, *args: str, fail: str = "") -> tuple[int, str, list[str]]:
        with tempfile.TemporaryDirectory() as tmp:
            log = Path(tmp) / "calls.log"
            env = dict(os.environ, UNITY_CLI=str(TOOLS / "tests/fake_unity.sh"), FAKE_LOG=str(log), FAKE_FAIL=fail,
                       HOLOKIT_SKIP_TOOL_TESTS="1")
            result = subprocess.run([str(TOOLS / "run_all_tests.sh"), "--out", tmp, *args],
                                    env=env, capture_output=True, text=True, timeout=60)
            summary = (Path(tmp) / "summary.txt").read_text()
            calls = log.read_text().splitlines() if log.exists() else []
        return result.returncode, summary, calls

    def test_passing_run_exits_zero_and_runs_products_in_order(self):
        code, summary, calls = self.run_script("--products", "MOFA Typed")
        self.assertEqual(code, 0, summary)
        self.assertEqual(summary.count("PASS"), 4 + 1)  # 4 product steps + "assets unchanged"
        self.assertEqual([c.split()[1].split("/")[-1] for c in calls], ["MOFA", "MOFA", "Typed", "Typed"])
        self.assertIn("--filter EditorScenePlaythroughTests", calls[1])

    def test_failing_playthrough_fails_the_run(self):
        code, summary, _ = self.run_script("--products", "QRealm", fail="EditorScenePlaythroughTests")
        self.assertEqual(code, 1)
        self.assertRegex(summary, r"FAIL\s+QRealm playthrough\s+exit 8")

    def test_skip_playthrough_runs_only_editmode(self):
        code, _, calls = self.run_script("--products", "HoloKit", "--skip-playthrough")
        self.assertEqual(code, 0)
        self.assertEqual(len(calls), 1)
        self.assertNotIn("--filter", calls[0])


if __name__ == "__main__":
    unittest.main()


class DeviceSigningKeyTests(unittest.TestCase):
    """Automatic signing on a Mac without a signed-in Xcode account needs the App Store Connect key."""

    def test_key_config_gives_the_authentication_flags(self):
        with tempfile.TemporaryDirectory() as tmp:
            config = Path(tmp) / "config.json"
            config.write_text('{"key_path": "~/keys/AuthKey_X.p8", "key_id": "X", "issuer_id": "I"}')
            args = run_device_tests.signing_key_args(config)
        self.assertEqual(args, ["-authenticationKeyPath", str(Path.home() / "keys/AuthKey_X.p8"),
                                "-authenticationKeyID", "X", "-authenticationKeyIssuerID", "I"])

    def test_no_key_config_gives_no_flags(self):
        self.assertEqual(run_device_tests.signing_key_args(Path("/nonexistent/config.json")), [])


class DeviceReportPollingTests(unittest.TestCase):
    """The device runner must take only this run's report, whether or not the test saved progress."""

    def poll(self, reports, previous):
        with mock.patch.object(run_device_tests.time, "sleep"), \
                mock.patch.object(run_device_tests, "read_report", side_effect=reports):
            return run_device_tests.poll("device", "bundle", "Folder", 60, previous)

    def test_report_written_only_at_the_end_is_accepted(self):
        old = {"complete": True, "run": 1}
        self.assertEqual(self.poll([old, old, {"complete": True, "run": 2}], old), {"complete": True, "run": 2})

    def test_report_saved_during_the_run_is_followed_to_completion(self):
        report = self.poll([None, {"complete": False}, {"complete": True, "x": 1}], None)
        self.assertEqual(report, {"complete": True, "x": 1})

    def test_unchanged_report_from_an_earlier_run_is_not_this_runs(self):
        old = {"complete": True, "run": 1}
        with mock.patch.object(run_device_tests.time, "time", side_effect=[0, 1, 2, 100]):
            self.assertIsNone(self.poll([old, old, old], old))

    def test_test_that_stopped_on_an_error_is_reported_without_waiting_for_the_timeout(self):
        failed = {"complete": False, "errors": ["Open palm gesture was not recognized."]}
        # The clock never reaches the deadline, and reading past the given reports raises StopIteration.
        with mock.patch.object(run_device_tests.time, "time", return_value=0):
            self.assertEqual(self.poll([{"complete": False}, failed, failed, failed, failed], None), failed)
