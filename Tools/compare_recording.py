#!/usr/bin/env python3
"""Compares a device recording with screenshots taken while it was recording.

Usage: compare_recording.py <folder with results.json, recording.mp4 and recording-shot-*.png>

For each screenshot, the best-matching movie frame within WINDOW of its time (the times are known only to a few
frames) should look like the screen; the recording excludes the on-screen UI. Similarity is the normalized
cross-correlation of small greyscale images. A correct recording matches its screenshot clearly better than that
frame flipped vertically or rotated 180 degrees; a frame from another moment is shown for reference. Needs ffmpeg,
numpy and Pillow.
"""
import json
import subprocess
import sys
import tempfile
from pathlib import Path

import numpy as np
from PIL import Image

SIZE = (90, 195)  # portrait thumbnails; landscape images are compared at the transposed size
WINDOW = 0.6  # seconds searched on each side of a screenshot's time


def frame_at(movie: Path, seconds: float, out: Path) -> Image.Image:
    subprocess.run(["ffmpeg", "-v", "error", "-y", "-ss", f"{max(seconds, 0):.3f}", "-i", str(movie),
                    "-frames:v", "1", str(out)], check=True)
    return Image.open(out)


def frames_around(movie: Path, seconds: float, folder: Path) -> list[Image.Image]:
    """Frames every 0.05 s within WINDOW of a time: the screenshot's time is only known to a few frames."""
    start = max(seconds - WINDOW, 0)
    folder.mkdir()
    subprocess.run(["ffmpeg", "-v", "error", "-y", "-ss", f"{start:.3f}", "-t", f"{2 * WINDOW:.3f}", "-i",
                    str(movie), "-vf", "fps=20", str(folder / "f%03d.png")], check=True)
    return [Image.open(path) for path in sorted(folder.glob("f*.png"))]


def grey(image: Image.Image, size: tuple[int, int]) -> np.ndarray:
    values = np.asarray(image.convert("L").resize(size, Image.BILINEAR), dtype=np.float64)
    values -= values.mean()
    return values / (np.linalg.norm(values) or 1.0)


def similarity(a: np.ndarray, b: np.ndarray) -> float:
    return float((a * b).sum())


def main() -> int:
    folder = Path(sys.argv[1])
    report = json.loads((folder / "results.json").read_text())
    movie = folder / report.get("video", "recording.mp4")
    duration = float(subprocess.run(["ffprobe", "-v", "error", "-show_entries", "format=duration", "-of",
                                     "csv=p=0", str(movie)], capture_output=True, text=True, check=True).stdout)
    ok = True
    with tempfile.TemporaryDirectory() as temp:
        for index, (shot, seconds) in enumerate(zip(report["recordingShots"], report["recordingShotSeconds"])):
            screen = Image.open(folder / shot)
            size = SIZE if screen.height >= screen.width else SIZE[::-1]
            screen_grey = grey(screen, size)
            candidates = frames_around(movie, seconds, Path(temp) / f"around{index}")
            match, frame = max(((similarity(screen_grey, grey(image, size)), image) for image in candidates),
                               key=lambda item: item[0])
            other = frame_at(movie, (seconds + duration / 2) % duration, Path(temp) / f"other{index}.png")
            controls = {
                "flipped": similarity(screen_grey, grey(frame.transpose(Image.FLIP_TOP_BOTTOM), size)),
                "rotated 180": similarity(screen_grey, grey(frame.rotate(180), size)),
                "other moment": similarity(screen_grey, grey(other, size)),
            }
            # A still phone sees nearly the same scene at another moment, so that control is shown but not judged.
            passed = match > 0.5 and all(match > controls[name] + 0.1 for name in ("flipped", "rotated 180"))
            ok &= passed
            detail = ", ".join(f"{name} {value:.2f}" for name, value in controls.items())
            print(f"{'PASS' if passed else 'FAIL'} {shot} at {seconds:.2f}s: best frame within {WINDOW}s "
                  f"match {match:.2f} ({detail})")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
