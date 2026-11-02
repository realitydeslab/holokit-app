"""Checks that HoloKit recordings play the right way up, from the device test DeviceStereoRecordingTests.

The phone stays still while it records twice in landscape left: in Stereo (MR), opened from portrait as the app opens
it, then in Mono, with a screenshot taken mid-recording. Mono's screen shows the view it records, so the Mono movie's
frame at the screenshot's time must match the screenshot as it is, not turned or flipped; the Stereo movie's middle
frame must then match the Mono one the same way. Each comparison scores the frame against the reference as is, turned
180 degrees and flipped either way. Only up and down are judged: a scene that mirrors itself left to right is fine. When
the phone stands upright (in a headset), the recorded camera's up (as the device reports it) must also be the world's
up; that checks the camera rig, not the pixels, which the comparisons check.

Usage: check_recording_orientation.py RESULT_FOLDER
"""
from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

import numpy as np

# Frames are compared at this width (the height follows the aspect), on the middle of the picture: Mono's screen has
# buttons along its edges that the recording does not.
WIDTH = 192
MIDDLE = 0.6
# The right way must beat the upside-down ways by this much correlation, or the scene is too plain to tell.
MARGIN = 0.15
UPSIDE_DOWN = ("turned 180", "flipped upside down")
# The recorded camera's up against gravity beyond which the phone stands upright enough to tell up from down.
UPRIGHT = 0.5
TRANSFORMS = {
    "as is": lambda image: image,
    "turned 180": lambda image: image[::-1, ::-1],
    "flipped upside down": lambda image: image[::-1, :],
    "mirrored": lambda image: image[:, ::-1],
}


def gray(path: Path, width: int, height: int, seconds: float | None = None) -> np.ndarray:
    """A picture or a movie's frame (at `seconds`) as a width x height grayscale array, top row first."""
    seek = ["-ss", f"{seconds:.3f}"] if seconds is not None else []
    result = subprocess.run(["ffmpeg", "-v", "error", *seek, "-i", str(path), "-frames:v", "1", "-vf",
                             f"scale={width}:{height}", "-f", "rawvideo", "-pix_fmt", "gray", "-"], capture_output=True)
    if result.returncode != 0 or len(result.stdout) != width * height:
        at = f" at {seconds:.2f} s" if seconds is not None else ""
        raise ValueError(f"{path.name} could not be read{at}: {result.stderr.decode(errors='replace').strip()[-200:]}")
    return np.frombuffer(result.stdout, dtype=np.uint8).reshape(height, width).astype(np.float64)


def middle(image: np.ndarray) -> np.ndarray:
    height, width = image.shape
    top, left = int(height * (1 - MIDDLE) / 2), int(width * (1 - MIDDLE) / 2)
    return image[top:height - top, left:width - left]


def correlation(a: np.ndarray, b: np.ndarray) -> float:
    a, b = a - a.mean(), b - b.mean()
    denominator = np.sqrt((a * a).sum() * (b * b).sum())
    return float((a * b).sum() / denominator) if denominator > 0 else 0.0


def scores(frame: np.ndarray, reference: np.ndarray) -> dict[str, float]:
    """How well the frame matches the reference in each orientation."""
    return {name: correlation(middle(turn(frame)), middle(reference)) for name, turn in TRANSFORMS.items()}


def judge(label: str, result: dict[str, float]) -> str | None:
    """None when the frame matches its reference as is, clearly; else the problem."""
    upright = result["as is"]
    worst = max(UPSIDE_DOWN, key=lambda name: result[name])
    if result[worst] > upright:
        return f"{label}: matches {worst} ({result[worst]:.2f}), as is {upright:.2f}"
    if upright - result[worst] < MARGIN:
        return f"{label}: cannot tell (as is {upright:.2f}, {worst} {result[worst]:.2f}); the camera sees too little"
    if result["mirrored"] > upright + MARGIN:
        return f"{label}: matches mirrored ({result['mirrored']:.2f}), as is {upright:.2f}"
    return None


def problems(folder: Path) -> tuple[list[str], list[str]]:
    """(lines of findings, problems)."""
    report = json.loads((folder / "results.json").read_text())
    clips = {clip["name"]: clip for clip in report.get("clips", [])}
    lines: list[str] = []
    issues: list[str] = []
    for name in ("mono-landscape", "stereo"):
        clip = clips.get(name)
        if not clip or not clip.get("video") or not (folder / clip["video"]).exists():
            issues.append(f"{name}: no recording")
    if issues:
        return lines, issues
    mono, stereo = clips["mono-landscape"], clips["stereo"]
    for clip in (stereo, mono):
        up = clip.get("upDot", 0.0)
        lines.append(f"{clip['name']}: {clip['renderMode']} {clip['screenOrientation']} {clip['width']}x{clip['height']}, "
                     f"device {clip['deviceOrientation']}, interface {clip.get('recordingInterfaceOrientation') or clip['interfaceOrientation']}, "
                     f"up against gravity {up:.2f}")
        if up <= -UPRIGHT:
            issues.append(f"{clip['name']}: the movie is upside down against gravity (up {up:.2f})")
    width = WIDTH
    height = round(WIDTH * mono["height"] / mono["width"])
    at = mono["screenshotSeconds"]
    mono_frame = gray(folder / mono["video"], width, height, at)
    screenshot = gray(folder / mono["screenshot"], width, height)
    stereo_frame = gray(folder / stereo["video"], width, height, stereo["screenshotSeconds"])
    for label, frame, reference in (("Mono movie vs Mono screen", mono_frame, screenshot),
                                    ("Stereo movie vs Mono movie", stereo_frame, mono_frame)):
        result = scores(frame, reference)
        lines.append(f"{label}: " + ", ".join(f"{name} {value:.2f}" for name, value in result.items()))
        issue = judge(label, result)
        if issue:
            issues.append(issue)
    return lines, issues


def main() -> int:
    lines, issues = problems(Path(sys.argv[1]))
    print("\n".join(lines))
    print("PASS" if not issues else "FAIL" + "".join(f"\n  {issue}" for issue in issues))
    return 1 if issues else 0


if __name__ == "__main__":
    sys.exit(main())
