#!/usr/bin/env bash
# Builds and runs the phone table's host tests on a Mac, then checks that the shipped libUnityHoloKit.a has every
# phone id in the source (a stale library would pass the host tests).
# Usage: Tests/Profile/Tools~/run_native_tests.sh [dir with HoloKitProfile.m/.h, default Sources~/holokit]
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
SRC="${1:-$ROOT/Sources~/holokit}"
OUT="${TMPDIR:-/tmp}/holokit-profile-tests"
clang -fobjc-arc -Wall -Wextra -Werror -Wno-unused-parameter -Duname=FakeUname \
  -I "$SRC" "$SRC/HoloKitProfile.m" "$ROOT/Tests/Profile/Native~/phone_profile_tests.m" \
  -framework Foundation -o "$OUT"
"$OUT"
if [ $# -eq 0 ]; then
  LIB_STRINGS="$(strings -a "$ROOT/Runtime/Plugins/iOS/libUnityHoloKit.a")"
  for id in $(grep -oE '@"iP(hone|ad)[0-9]+,[0-9]+"' "$SRC/HoloKitProfile.m" | tr -d '@"' | sort -u); do
    grep -qx "$id" <<< "$LIB_STRINGS" || { echo "libUnityHoloKit.a is stale: no $id. Rebuild it from Sources~."; exit 1; }
  done
  echo "libUnityHoloKit.a has every phone id in HoloKitProfile.m"
fi
