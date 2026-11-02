"""Checks which audio a HoloKit recording plays and keeps when it is played or exported, on this Mac.

A recording carries three audio tracks (Tools/check_recording_audio.py): the game and microphone mix, the game, and the
microphone, grouped as alternates with the mix as the default. Players and exporters must use the mix alone: never the
sum of all three (the game twice over) and never an editing track.

The check writes a movie laid out as the recorder lays it out (Tools/recording_tracks.swift: mix 440 Hz, game 660 Hz,
microphone 880 Hz), passes it through Apple's export presets (avconvert: AVAssetExportSession, which iOS apps use to
export and share video) and through ffmpeg's default stream choice (common on servers), and tells by frequency which
tracks each output kept. Given a device recording, it also checks that the recording's tracks carry the same flags as
that movie (enabled, alternate group).

Usage: check_recording_playback.py [DEVICE_RECORDING.mp4]
"""
from __future__ import annotations

import struct
import subprocess
import sys
import tempfile
from pathlib import Path

import numpy as np

TOOLS = Path(__file__).resolve().parent
TONES = {"mix": 440.0, "game": 660.0, "microphone": 880.0}
PRESETS = ("PresetHighestQuality", "PresetMediumQuality", "Preset1280x720", "PresetAppleM4A", "PresetPassthrough")
# A tone within this many dB of the loudest one is in the track.
PRESENT_DB = 20.0


def track_flags(path: Path) -> list[tuple[str, bool, int]]:
    """Each track's handler ("vide", "soun"), enabled flag and alternate group, read from the MP4 boxes."""
    data = path.read_bytes()

    def boxes(start: int, end: int):
        while start + 8 <= end:
            size, kind = struct.unpack(">I4s", data[start:start + 8])
            header = 8
            if size == 1:
                size, header = struct.unpack(">Q", data[start + 8:start + 16])[0], 16
            elif size == 0:
                size = end - start
            yield kind.decode("latin-1"), start + header, start + size
            start += size

    tracks = []
    for kind, body, end in boxes(0, len(data)):
        if kind != "moov":
            continue
        for trak, trak_body, trak_end in boxes(body, end):
            if trak != "trak":
                continue
            enabled, group, handler = False, 0, "?"
            for child, child_body, child_end in boxes(trak_body, trak_end):
                if child == "tkhd":
                    version, flags = data[child_body], int.from_bytes(data[child_body + 1:child_body + 4], "big")
                    enabled = bool(flags & 1)
                    # version 0: 4-byte times and duration; version 1: 8-byte ones.
                    group_at = child_body + (4 + 8 + 4 + 4 + 4 + 8 + 2 if version == 0 else 4 + 16 + 4 + 4 + 8 + 8 + 2)
                    group = struct.unpack(">H", data[group_at:group_at + 2])[0]
                elif child == "mdia":
                    for grandchild, grand_body, _ in boxes(child_body, child_end):
                        if grandchild == "hdlr":
                            handler = data[grand_body + 8:grand_body + 12].decode("latin-1")
            tracks.append((handler, enabled, group))
    return tracks


def audio_count(path: Path) -> int:
    result = subprocess.run(["ffprobe", "-v", "error", "-select_streams", "a", "-show_entries", "stream=index",
                             "-of", "csv=p=0", str(path)], capture_output=True, text=True, check=True)
    return len(result.stdout.split())


def tones_in(path: Path, audio_index: int) -> list[str]:
    """The test tones (by track name) one audio stream carries."""
    raw = subprocess.run(["ffmpeg", "-v", "error", "-i", str(path), "-map", f"0:a:{audio_index}", "-ac", "1",
                          "-ar", "48000", "-f", "f32le", "-"], capture_output=True, check=True).stdout
    samples = np.frombuffer(raw, dtype="<f4")
    spectrum = np.abs(np.fft.rfft(samples * np.hanning(len(samples))))
    frequencies = np.fft.rfftfreq(len(samples), 1 / 48000)
    power = {name: spectrum[np.abs(frequencies - tone) < 5].max() for name, tone in TONES.items()}
    loudest = max(power.values())
    return [name for name, value in power.items() if 20 * np.log10(value / loudest + 1e-12) > -PRESENT_DB]


def outputs(movie: Path, directory: Path) -> dict[str, Path]:
    """The movie passed through each export path."""
    made = {}
    for preset in PRESETS:
        out = directory / f"{preset}.{'m4a' if preset == 'PresetAppleM4A' else 'mp4'}"
        subprocess.run(["avconvert", "--preset", preset, "--source", str(movie), "--output", str(out), "--replace"],
                       capture_output=True, check=True)
        made[f"avconvert {preset}"] = out
    out = directory / "ffmpeg-default.mp4"
    subprocess.run(["ffmpeg", "-v", "error", "-y", "-i", str(movie), "-c", "copy", str(out)], check=True)
    made["ffmpeg default streams"] = out
    return made


def problems(device_recording: Path | None) -> tuple[list[str], list[str]]:
    """(lines of findings, problems)."""
    lines, issues = [], []
    with tempfile.TemporaryDirectory() as temporary:
        directory = Path(temporary)
        movie = directory / "layout.mp4"
        subprocess.run(["xcrun", "swift", str(TOOLS / "recording_tracks.swift"), str(movie)],
                       capture_output=True, check=True)
        layout = track_flags(movie)
        lines.append(f"recorder layout (handler, enabled, alternate group): {layout}")
        for name, out in outputs(movie, directory).items():
            count = audio_count(out)
            kept = [tones_in(out, index) for index in range(count)]
            lines.append(f"{name}: {count} audio track(s) {kept}")
            if name.endswith("PresetPassthrough") and kept == [["mix"], ["game"], ["microphone"]]:
                flags = [flag for flag in track_flags(out) if flag[0] == "soun"]
                if flags != [flag for flag in layout if flag[0] == "soun"]:
                    issues.append(f"{name} kept the tracks but changed their flags: {flags}")
            elif kept != [["mix"]]:
                issues.append(f"{name} keeps {kept}, want the mix alone")
        if device_recording is not None:
            recorded = track_flags(device_recording)
            lines.append(f"{device_recording.name} (handler, enabled, alternate group): {recorded}")
            want = [(handler, enabled, group > 0) for handler, enabled, group in layout if handler == "soun"]
            got = [(handler, enabled, group > 0) for handler, enabled, group in recorded if handler == "soun"]
            if got != want:
                issues.append(f"{device_recording.name} audio flags {got} differ from the recorder layout {want}")
    return lines, issues


def main() -> int:
    device_recording = Path(sys.argv[1]) if len(sys.argv) > 1 else None
    lines, issues = problems(device_recording)
    print("\n".join(lines))
    print("PASS" if not issues else "FAIL" + "".join(f"\n  {issue}" for issue in issues))
    return 1 if issues else 0


if __name__ == "__main__":
    sys.exit(main())
