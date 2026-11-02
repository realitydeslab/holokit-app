"""Checks the audio of a HoloKit recording.

A recording carries three audio tracks: the game and the microphone mixed (the only default track, which Photos and
social apps play), the game alone and the microphone alone (for editing). Each must have sound in it.

Usage: check_recording_audio.py RECORDING.mp4 [...]
"""
from __future__ import annotations

import json
import re
import subprocess
import sys
from pathlib import Path

TRACKS = ("game + microphone", "game", "microphone")
# RMS below this is digital silence: a track that was written but never fed.
SILENCE_DB = -70.0


def audio_streams(path: Path) -> list[dict]:
    """The file's audio streams in track order, with their dispositions."""
    result = subprocess.run(["ffprobe", "-v", "error", "-select_streams", "a", "-show_entries",
                             "stream=index,codec_name,sample_rate,channels:stream_disposition=default",
                             "-of", "json", str(path)], capture_output=True, text=True, check=True)
    return json.loads(result.stdout).get("streams", [])


def rms_db(path: Path, audio_index: int) -> float:
    """Overall RMS level of one audio stream in dBFS (-inf for digital silence)."""
    result = subprocess.run(["ffmpeg", "-hide_banner", "-nostats", "-i", str(path), "-map", f"0:a:{audio_index}",
                             "-af", "astats=measure_overall=RMS_level:measure_perchannel=none", "-f", "null", "-"],
                            capture_output=True, text=True)
    match = re.search(r"RMS level dB: (\S+)", result.stderr)
    if match is None:
        raise RuntimeError(f"ffmpeg gave no RMS level for audio stream {audio_index} of {path}: {result.stderr[-300:]}")
    return float(match.group(1))  # float() reads "-inf"


def problems(path: Path) -> list[str]:
    """What is wrong with the recording's audio; empty when it has the three tracks, each with sound."""
    streams = audio_streams(path)
    if len(streams) != len(TRACKS):
        return [f"{len(streams)} audio track(s), want {len(TRACKS)} ({', '.join(TRACKS)}): "
                "the microphone was not recorded (Record Microphone off or microphone permission not granted?)"]
    issues = []
    defaults = [stream.get("disposition", {}).get("default", 0) for stream in streams]
    if defaults != [1, 0, 0]:
        issues.append(f"default flags {defaults}, want [1, 0, 0]: only the mix may play by default")
    for index, name in enumerate(TRACKS):
        level = rms_db(path, index)
        if level < SILENCE_DB:
            issues.append(f"track {index + 1} ({name}) is silent ({level:.1f} dB RMS)")
    return issues


def main() -> int:
    failed = False
    for argument in sys.argv[1:]:
        found = problems(Path(argument))
        failed |= bool(found)
        print(f"{'FAIL' if found else 'PASS'} {argument}" + "".join(f"\n  {issue}" for issue in found))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
