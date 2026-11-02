"""Tests for Tools/check_recording_orientation.py on small movies made with ffmpeg.

Run: python3 -m unittest discover -s Tools/tests
"""
from __future__ import annotations

import json
import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from unittest import mock

TOOLS = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(TOOLS))
import check_recording_orientation  # noqa: E402
import run_device_tests  # noqa: E402

# A landscape scene with no symmetry: the Game of Life from a fixed seed, in large cells so scaling keeps it.
SCENE = "life=s=320x148:r=30:ratio=0.5:seed=7:life_color=white:death_color=black,scale=640x296:flags=neighbor"
PLAIN = "color=c=gray:s=640x296:r=30"
TURNED = "hflip,vflip"


@unittest.skipUnless(shutil.which("ffmpeg"), "needs ffmpeg")
class RecordingOrientationTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.folder = Path(self.directory.name)

    def tearDown(self):
        self.directory.cleanup()

    def movie(self, name: str, source: str = SCENE, turn: str | None = None) -> None:
        command = ["ffmpeg", "-v", "error", "-y", "-f", "lavfi", "-i", source, "-t", "2"]
        if turn:
            command += ["-vf", turn]
        subprocess.run(command + ["-pix_fmt", "yuv420p", str(self.folder / name)], check=True)

    def screenshot(self, source: str = SCENE, turn: str | None = None) -> None:
        """Mono's screen at 1 s: the scene's frame then, as a PNG."""
        command = ["ffmpeg", "-v", "error", "-y", "-f", "lavfi", "-i", source, "-ss", "1", "-frames:v", "1"]
        if turn:
            command += ["-vf", turn]
        subprocess.run(command + [str(self.folder / "mono-landscape.png")], check=True)

    def report(self, stereo: bool = True, stereo_up: float = 0.95) -> None:
        def clip(name: str, mode: str, up: float = 0.95) -> dict:
            return {"name": name, "renderMode": mode, "screenOrientation": "LandscapeLeft",
                    "deviceOrientation": "LandscapeLeft", "interfaceOrientation": "LandscapeLeft",
                    "width": 2556, "height": 1179, "video": f"{name}.mp4", "screenshot": f"{name}.png",
                    "screenshotSeconds": 1.0, "upDot": up}
        clips = [clip("mono-landscape", "Mono")] + ([clip("stereo", "Stereo", stereo_up)] if stereo else [])
        (self.folder / "results.json").write_text(json.dumps({"complete": True, "clips": clips}))

    def run_layout(self, stereo_turn: str | None = None, mono_turn: str | None = None, source: str = SCENE,
                   stereo_up: float = 0.95) -> list[str]:
        self.movie("mono-landscape.mp4", source, mono_turn)
        self.screenshot(source)
        self.movie("stereo.mp4", source, stereo_turn)
        self.report(stereo_up=stereo_up)
        return check_recording_orientation.problems(self.folder)[1]

    def test_upright_stereo_recording_passes(self):
        self.assertEqual(self.run_layout(), [])

    def test_stereo_recording_turned_upside_down_fails(self):
        found = self.run_layout(stereo_turn=TURNED)
        self.assertEqual(len(found), 1)
        self.assertIn("Stereo movie vs Mono movie: matches turned 180", found[0])

    def test_stereo_recording_flipped_fails(self):
        found = self.run_layout(stereo_turn="vflip")
        self.assertEqual(len(found), 1)
        self.assertIn("matches flipped upside down", found[0])

    def test_mono_recording_that_differs_from_its_screen_fails(self):
        found = self.run_layout(stereo_turn=TURNED, mono_turn=TURNED)
        self.assertIn("Mono movie vs Mono screen: matches turned 180", found[0])

    def test_stereo_camera_upside_down_against_gravity_fails(self):
        found = self.run_layout(stereo_up=-0.97)
        self.assertEqual(found, ["stereo: the movie is upside down against gravity (up -0.97)"])

    def test_phone_lying_flat_leaves_gravity_out(self):
        self.assertEqual(self.run_layout(stereo_up=0.02), [])

    def test_plain_scene_cannot_tell(self):
        found = self.run_layout(source=PLAIN)
        self.assertTrue(found)
        self.assertTrue(all("cannot tell" in issue for issue in found))

    def test_missing_stereo_recording_fails(self):
        self.movie("mono-landscape.mp4")
        self.screenshot()
        self.report(stereo=False)
        self.assertEqual(check_recording_orientation.problems(self.folder)[1], ["stereo: no recording"])

    def test_device_run_fails_when_its_stereo_recording_is_upside_down(self):
        self.run_layout(stereo_turn=TURNED)
        passed, detail = run_device_tests.check_orientation(self.folder, True, "complete")
        self.assertFalse(passed)
        self.assertIn("turned 180", detail)

    def test_scene_symmetric_left_to_right_passes(self):
        # A picture whose right half mirrors its left: as is and mirrored tie; only up and down matter.
        found = self.run_layout(source=SCENE + ",crop=320:296:0:0,split[l][r];[r]hflip[m];[l][m]hstack")
        self.assertEqual(found, [])

    def test_movie_without_a_frame_at_the_screenshot_time_is_a_problem_not_a_crash(self):
        self.run_layout()
        subprocess.run(["ffmpeg", "-v", "error", "-y", "-f", "lavfi", "-i", SCENE, "-t", "0.3", "-pix_fmt", "yuv420p",
                        str(self.folder / "stereo.mp4")], check=True)
        passed, detail = run_device_tests.check_orientation(self.folder, True, "complete")
        self.assertFalse(passed)
        self.assertIn("stereo.mp4", detail)

    def test_truncated_movie_is_a_problem_not_a_crash(self):
        self.run_layout()
        movie = self.folder / "stereo.mp4"
        movie.write_bytes(movie.read_bytes()[:2000])
        passed, detail = run_device_tests.check_orientation(self.folder, True, "complete")
        self.assertFalse(passed)
        self.assertIn("could not be read", detail)

    def test_device_run_fails_without_ffmpeg_to_check_it(self):
        self.run_layout()
        with mock.patch.object(run_device_tests.shutil, "which", return_value=None):
            passed, detail = run_device_tests.check_orientation(self.folder, True, "complete")
        self.assertFalse(passed)
        self.assertIn("no ffmpeg", detail)

    def test_device_run_without_a_stereo_recording_keeps_its_verdict(self):
        self.assertEqual(run_device_tests.check_orientation(self.folder, True, "complete"), (True, "complete"))


if __name__ == "__main__":
    unittest.main()
