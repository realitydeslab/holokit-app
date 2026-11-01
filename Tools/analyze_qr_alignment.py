#!/usr/bin/env python3
"""Measures how well a spectator lined up with the host after scanning its QR code.

Usage: analyze_qr_alignment.py RUN_DIR

RUN_DIR is a device runner output folder of the holokit-qr-alignment test: the host's folder has host-qr.csv (by the
network clock, where the host believes its code is) and results.json with its screen geometry; the client's folder has,
per round, pairs-N.csv (the image-pose pairs the alignment was solved from), retrack-N.csv (where the client saw the
code after aligning) and the solved theta and translation in results.json.

For each round it prints:
  - the host screen's tilt (90 degrees = lying face up, 0 = upright);
  - theta as the app solves it and the yaw of the world-frame rotation between the host's and the client's view of
    the code (client rotation times host rotation inverse), which is what the alignment should rotate by;
  - how far that world-frame rotation is from a pure yaw (it must be one: both worlds are gravity aligned), which
    checks the host's model of where its code is;
  - the alignment error: where the client sees the code after aligning against where the host says it is (yaw and
    centimetres), and the same error predicted had the world-frame yaw been used instead.
Exits 1 when an aligned round is off by more than 3 degrees of yaw.
"""
from __future__ import annotations

import argparse
import csv
import json
import math
import statistics
import sys
from dataclasses import dataclass
from pathlib import Path

Vector = tuple[float, float, float]
Matrix = tuple[Vector, Vector, Vector]  # rows
PENALTY = 10.0  # OptimizationPenaltyConstant in HoloKitAppMultiplayerManager_SpatialAnchorSharing_Client.cs
YAW_TOLERANCE_DEG = 3.0


@dataclass(frozen=True)
class Pose:
    position: Vector
    rotation: Matrix


def rotation(qx: float, qy: float, qz: float, qw: float) -> Matrix:
    """Unity's Matrix4x4.Rotate for a quaternion."""
    return ((1 - 2 * (qy * qy + qz * qz), 2 * (qx * qy - qz * qw), 2 * (qx * qz + qy * qw)),
            (2 * (qx * qy + qz * qw), 1 - 2 * (qx * qx + qz * qz), 2 * (qy * qz - qx * qw)),
            (2 * (qx * qz - qy * qw), 2 * (qy * qz + qx * qw), 1 - 2 * (qx * qx + qy * qy)))


def multiply(a: Matrix, b: Matrix) -> Matrix:
    return tuple(tuple(sum(a[i][k] * b[k][j] for k in range(3)) for j in range(3)) for i in range(3))


def transpose(a: Matrix) -> Matrix:
    return tuple(tuple(a[j][i] for j in range(3)) for i in range(3))


def apply(a: Matrix, v: Vector) -> Vector:
    return tuple(sum(a[i][k] * v[k] for k in range(3)) for i in range(3))


def subtract(a: Vector, b: Vector) -> Vector:
    return (a[0] - b[0], a[1] - b[1], a[2] - b[2])


def yaw_matrix(degrees: float) -> Matrix:
    """Quaternion.AngleAxis(degrees, Vector3.up) as a matrix."""
    c, s = math.cos(math.radians(degrees)), math.sin(math.radians(degrees))
    return ((c, 0.0, s), (0.0, 1.0, 0.0), (-s, 0.0, c))


def yaw_of(m: Matrix) -> float:
    """The yaw (degrees, about world up) closest to a rotation."""
    return math.degrees(math.atan2(m[0][2] - m[2][0], m[0][0] + m[2][2]))


def tilt_of(m: Matrix) -> float:
    """How far (degrees) a rotation moves world up: 0 for a pure yaw."""
    return math.degrees(math.acos(max(-1.0, min(1.0, m[1][1]))))


def wrap(degrees: float) -> float:
    return (degrees + 180.0) % 360.0 - 180.0


def mean(vectors: list[Vector]) -> Vector:
    return tuple(statistics.fmean(v[i] for v in vectors) for i in range(3))


def read_poses(path: Path, prefix: str = "") -> list[tuple[float | None, Pose]]:
    rows = []
    with open(path, newline="") as handle:
        for row in csv.DictReader(handle):
            get = lambda name: float(row[prefix + name])
            time = float(row["serverTime"]) if "serverTime" in row else None
            rows.append((time, Pose((get("x"), get("y"), get("z")), rotation(get("qx"), get("qy"), get("qz"), get("qw")))))
    return rows


def read_pairs(path: Path) -> list[tuple[Pose, Pose]]:
    hosts = [pose for _, pose in read_poses(path, "host")]
    clients = [pose for _, pose in read_poses(path, "client")]
    return list(zip(hosts, clients))


def legacy_theta(pairs: list[tuple[Pose, Pose]]) -> float:
    """CalculateClientSyncResult's theta: positions about their centres plus the rotation term."""
    host_centre = mean([h.position for h, _ in pairs])
    client_centre = mean([c.position for _, c in pairs])
    a = b = 0.0
    for host, client in pairs:
        p, q = subtract(host.position, host_centre), subtract(client.position, client_centre)
        r = multiply(transpose(host.rotation), client.rotation)
        a += p[0] * q[0] + p[2] * q[2] + PENALTY * (r[0][0] + r[2][2])
        b += -p[0] * q[2] + p[2] * q[0] + PENALTY * (-r[2][0] + r[0][2])
    return math.degrees(math.atan2(b, a))


def world_theta(pairs: list[tuple[Pose, Pose]]) -> float:
    """The least-squares yaw of the world-frame rotations taking the host's view of the code to the client's."""
    a = b = 0.0
    for host, client in pairs:
        d = multiply(client.rotation, transpose(host.rotation))
        a += d[0][0] + d[2][2]
        b += d[0][2] - d[2][0]
    return math.degrees(math.atan2(b, a))


def to_aligned(point: Vector, theta: float, translate: Vector) -> Vector:
    """ARSession.setWorldOrigin with ResetOrigin(translate, AngleAxis(theta, up)): the new origin sits at that pose
    in the old world, so an old point x is now R^-1 (x - t)."""
    return apply(transpose(yaw_matrix(theta)), subtract(point, translate))


def from_aligned(point: Vector, theta: float, translate: Vector) -> Vector:
    moved = apply(yaw_matrix(theta), point)
    return (moved[0] + translate[0], moved[1] + translate[1], moved[2] + translate[2])


def nearest(host_log: list[tuple[float, Pose]], time: float) -> Pose:
    return min(host_log, key=lambda entry: abs(entry[0] - time))[1]


@dataclass(frozen=True)
class Error:
    yaw: float  # degrees
    tilt: float  # degrees
    centimetres: float


def error(seen: list[tuple[float, Pose]], host_log: list[tuple[float, Pose]], theta_change: float = 0.0,
          change=None) -> Error | None:
    """Where the client sees the code against where the host says it is. change re-maps the client's points (to
    predict another alignment); theta_change is the yaw it adds."""
    yaws, tilts, distances = [], [], []
    for time, pose in seen:
        host = nearest(host_log, time)
        position = change(pose.position) if change else pose.position
        rotation_ = multiply(transpose(yaw_matrix(theta_change)), pose.rotation)
        d = multiply(rotation_, transpose(host.rotation))
        yaws.append(yaw_of(d))
        tilts.append(tilt_of(d))
        distances.append(100 * math.dist(position, host.position))
    if not yaws:
        return None
    return Error(statistics.median(yaws), statistics.median(tilts), statistics.median(distances))


def analyze(run: Path) -> tuple[list[str], bool]:
    reports = {path.parent: json.loads(path.read_text()) for path in sorted(run.rglob("results.json"))}
    hosts = [folder for folder, report in reports.items() if report.get("role") == "host"]
    if len(hosts) != 1 or not (hosts[0] / "host-qr.csv").exists():
        return [f"expected one host with host-qr.csv under {run}"], False
    host_log = [(time, pose) for time, pose in read_poses(hosts[0] / "host-qr.csv", "qr")]
    geometry = reports[hosts[0]].get("host") or {}
    lines = [f"host {reports[hosts[0]].get('device')}: " + ", ".join(f"{k}={v}" for k, v in geometry.items())]
    ok = True
    for client in (folder for folder in reports if folder != hosts[0]):
        report = reports[client]
        for round_ in report.get("rounds", []):
            n = round_["round"]
            pairs_path, seen_path = client / f"pairs-{n}.csv", client / f"retrack-{n}.csv"
            if not round_.get("synced") or not pairs_path.exists():
                lines.append(f"{client.name} round {n}: not aligned")
                ok = False
                continue
            pairs = read_pairs(pairs_path)
            theta, translate = round_["thetaDeg"], tuple(round_["translate"][k] for k in "xyz")
            host_tilt = statistics.median(math.degrees(math.asin(max(-1.0, min(1.0, h.rotation[1][1]))))
                                          for h, _ in pairs)
            pair_tilt = statistics.median(tilt_of(multiply(c.rotation, transpose(h.rotation))) for h, c in pairs)
            world = world_theta(pairs)
            lines.append(f"{client.name} round {n}: host screen tilt {host_tilt:.0f} deg; app theta {theta:.1f} "
                         f"(recomputed {legacy_theta(pairs):.1f}), world-frame yaw {world:.1f}; host model off "
                         f"pure yaw by {pair_tilt:.1f} deg")
            seen = read_poses(seen_path) if seen_path.exists() else []
            measured = error(seen, host_log)
            if measured is None:
                lines.append("    no re-tracked code after aligning")
                continue
            # The same observations under the world-frame yaw: back to the pre-alignment world, then re-aligned.
            world_translate = subtract(mean([c.position for _, c in pairs]),
                                       apply(yaw_matrix(world), mean([h.position for h, _ in pairs])))
            predicted = error(seen, host_log, theta_change=world - theta, change=lambda x: to_aligned(
                from_aligned(x, theta, translate), world, world_translate))
            lines.append(f"    after aligning: yaw off {measured.yaw:+.1f} deg, tilt {measured.tilt:.1f} deg, "
                         f"{measured.centimetres:.1f} cm ({len(seen)} views); with the world-frame yaw: "
                         f"{predicted.yaw:+.1f} deg, {predicted.centimetres:.1f} cm")
            ok &= abs(wrap(measured.yaw)) <= YAW_TOLERANCE_DEG
    return lines, ok


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("run", type=Path)
    lines, ok = analyze(parser.parse_args().run)
    print("\n".join(lines))
    print("ALIGNED" if ok else "MISALIGNED")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
