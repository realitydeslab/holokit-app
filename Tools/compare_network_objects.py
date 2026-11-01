#!/usr/bin/env python3
"""Compares where each client of a multiplayer run shows the network objects against where the host has them.

Usage: compare_network_objects.py RUN_DIR [--tolerance METRES]

RUN_DIR is a device runner output folder: one folder per device with results.json and the objects-<scene>.csv files
the multiplayer scene test writes (network time, object id, name, world position, about five times a second). For
every scene and client, every object the host has must also be on the client, and where the client shows it must be
where the host had it at the same network time (the host's samples are interpolated). Prints one line per scene and
client and one per object that differs; exits 1 when any object is missing or further off than the tolerance.
"""
from __future__ import annotations

import argparse
import bisect
import csv
import json
import math
import statistics
import sys
from dataclasses import dataclass
from pathlib import Path

Samples = list[tuple[float, float, float, float]]  # (network time, x, y, z), by time


@dataclass(frozen=True)
class ObjectDifference:
    object_id: int
    name: str
    median: float | None  # metres between client and host; None when the object is missing on the client
    worst: float | None
    compared: int  # client samples that fell inside the host's sampling window


def load(path: Path) -> dict[int, tuple[str, Samples]]:
    objects: dict[int, tuple[str, Samples]] = {}
    with open(path, newline="") as handle:
        for row in csv.DictReader(handle):
            name, samples = objects.setdefault(int(row["objectId"]), (row["name"], []))
            samples.append((float(row["serverTime"]), float(row["x"]), float(row["y"]), float(row["z"])))
    for _, samples in objects.values():
        samples.sort()
    return objects


def position_at(samples: Samples, time: float) -> tuple[float, float, float] | None:
    """The sampled position at this time, interpolated; None outside the samples' span."""
    times = [sample[0] for sample in samples]
    if not samples or time < times[0] or time > times[-1]:
        return None
    i = bisect.bisect_left(times, time)
    after = samples[i]
    if after[0] == time or i == 0:
        return after[1:]
    before = samples[i - 1]
    f = (time - before[0]) / (after[0] - before[0])
    return tuple(before[k] + f * (after[k] - before[k]) for k in (1, 2, 3))


def compare(host: dict[int, tuple[str, Samples]], client: dict[int, tuple[str, Samples]]) -> list[ObjectDifference]:
    differences = []
    for object_id, (name, host_samples) in sorted(host.items()):
        if object_id not in client:
            differences.append(ObjectDifference(object_id, name, None, None, 0))
            continue
        errors = []
        for time, *position in client[object_id][1]:
            expected = position_at(host_samples, time)
            if expected is not None:
                errors.append(math.dist(expected, position))
        median = statistics.median(errors) if errors else 0.0
        differences.append(ObjectDifference(object_id, name, median, max(errors, default=0.0), len(errors)))
    return differences


def compare_run(run: Path, tolerance: float) -> tuple[list[str], bool]:
    devices = {}
    for report in sorted(run.rglob("results.json")):
        devices[report.parent] = json.loads(report.read_text())
    hosts = [folder for folder, report in devices.items() if report.get("role") == "host"]
    if len(hosts) != 1:
        return [f"expected one host report under {run}, found {len(hosts)}"], False
    host = hosts[0]
    lines, ok = [], True
    for scene in [entry["scene"] for entry in devices[host].get("scenes", [])]:
        host_csv = host / f"objects-{scene}.csv"
        if not host_csv.exists():
            lines.append(f"{scene}: no host samples")
            ok = False
            continue
        host_objects = load(host_csv)
        for client in (folder for folder in devices if folder != host):
            client_csv = client / f"objects-{scene}.csv"
            if not client_csv.exists():
                lines.append(f"{scene} {client.name}: no client samples")
                ok = False
                continue
            differences = compare(host_objects, load(client_csv))
            missing = [d for d in differences if d.median is None]
            off = [d for d in differences if d.median is not None and d.median > tolerance]
            unmatched = [d for d in differences if d.median is not None and d.compared == 0]
            worst = max((d for d in differences if d.median is not None), key=lambda d: d.median, default=None)
            lines.append(f"{scene} {client.name}: {len(differences)} objects, {len(missing)} missing, {len(off)} off by "
                         f"more than {tolerance} m, {len(unmatched)} not sampled at the same time" +
                         (f"; largest median {worst.median:.3f} m ({worst.name})" if worst else ""))
            for d in missing:
                lines.append(f"    missing on the client: {d.name} (id {d.object_id})")
            for d in off:
                lines.append(f"    off: {d.name} (id {d.object_id}) median {d.median:.3f} m, worst {d.worst:.3f} m")
            ok &= not missing and not off
    return lines, ok


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("run", type=Path)
    parser.add_argument("--tolerance", type=float, default=0.25, help="metres (default 0.25)")
    args = parser.parse_args()
    lines, ok = compare_run(args.run, args.tolerance)
    print("\n".join(lines))
    print("ALL MATCH" if ok else "SOME DIFFER")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
