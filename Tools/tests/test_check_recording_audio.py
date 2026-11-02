"""Tests for Tools/check_recording_audio.py on small movies made with ffmpeg.

Run: python3 -m unittest discover -s Tools/tests
"""
from __future__ import annotations

import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(TOOLS))
import check_recording_audio  # noqa: E402

TONE = "sine=frequency=440:duration=1:sample_rate=24000"
SILENCE = "anullsrc=channel_layout=stereo:sample_rate=24000:duration=1"


@unittest.skipUnless(shutil.which("ffmpeg") and shutil.which("ffprobe"), "needs ffmpeg and ffprobe")
class RecordingAudioTests(unittest.TestCase):
    def movie(self, sources: list[str], defaults: list[int]) -> Path:
        """A one-second movie with a black video track and one audio track per source, in order."""
        path = Path(self.directory.name) / f"movie-{len(list(Path(self.directory.name).iterdir()))}.mp4"
        command = ["ffmpeg", "-v", "error", "-y", "-f", "lavfi", "-i", "color=c=black:s=16x16:d=1"]
        for source in sources:
            command += ["-f", "lavfi", "-i", source]
        command += ["-map", "0:v"]
        for index in range(len(sources)):
            command += ["-map", f"{index + 1}:a"]
        for index, default in enumerate(defaults):
            command += [f"-disposition:a:{index}", "default" if default else "0"]
        command += ["-c:v", "libx264", "-c:a", "aac", "-shortest", str(path)]
        subprocess.run(command, check=True)
        return path

    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()

    def tearDown(self):
        self.directory.cleanup()

    def test_mix_game_and_microphone_tracks_pass(self):
        movie = self.movie([TONE, TONE, TONE], [1, 0, 0])
        self.assertEqual(check_recording_audio.problems(movie), [])

    def test_game_track_alone_fails_naming_the_microphone(self):
        found = check_recording_audio.problems(self.movie([TONE], [1]))
        self.assertEqual(len(found), 1)
        self.assertIn("microphone was not recorded", found[0])

    def test_silent_microphone_track_fails(self):
        found = check_recording_audio.problems(self.movie([TONE, TONE, SILENCE], [1, 0, 0]))
        self.assertEqual(len(found), 1)
        self.assertIn("track 3 (microphone) is silent", found[0])

    def test_every_track_playing_by_default_fails(self):
        found = check_recording_audio.problems(self.movie([TONE, TONE, TONE], [1, 1, 1]))
        self.assertEqual(len(found), 1)
        self.assertIn("only the mix may play by default", found[0])

    def test_device_run_fails_when_its_recording_lacks_the_microphone(self):
        import run_device_tests
        folder = Path(self.directory.name) / "mofa-single" / "298D3A"
        folder.mkdir(parents=True)
        self.movie([TONE], [1]).rename(folder / "recording.mp4")
        passed, detail = run_device_tests.check_recordings(folder.parent, True, "complete")
        self.assertFalse(passed)
        self.assertIn("recording.mp4: 1 audio track(s)", detail)

    def test_device_run_without_a_recording_keeps_its_verdict(self):
        import run_device_tests
        self.assertEqual(run_device_tests.check_recordings(Path(self.directory.name), True, "complete"), (True, "complete"))


if __name__ == "__main__":
    unittest.main()
