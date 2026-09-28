#!/usr/bin/env bash
# Records or checks the state of Git-tracked files, to catch tests that write to assets (for example a
# script calling SetFloat on a shared material while the Editor plays the game).
#
#   Tools/asset_snapshot.sh save  <file>   # before the tests
#   Tools/asset_snapshot.sh check <file>   # after; prints changed paths and exits 1 if any
#
# XR Simulation settings under */Assets/XR/ are excluded: Unity rewrites them whenever it plays.
set -euo pipefail
REPO="${ASSET_SNAPSHOT_REPO:-$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)}"
snapshot() {
  git -C "$REPO" status --porcelain=v1 --untracked-files=no | while IFS= read -r line; do
    path="${line:3}"; path="${path#\"}"; path="${path%\"}"
    [[ "$path" == */Assets/XR/* ]] && continue
    printf '%s %s\n' "$(git -C "$REPO" hash-object -- "$path" 2>/dev/null || echo deleted)" "$path"
  done | sort
}
case "${1:-}" in
  save) snapshot > "${2:?snapshot file}" ;;
  check)
    changed="$(diff <(sort "${2:?snapshot file}") <(snapshot) | sed -n 's/^[<>] [^ ]* //p' | sort -u || true)"
    if [[ -n "$changed" ]]; then echo "$changed"; exit 1; fi ;;
  *) echo "usage: $0 save|check <file>" >&2; exit 2 ;;
esac
