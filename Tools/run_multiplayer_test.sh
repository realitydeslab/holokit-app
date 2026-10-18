#!/usr/bin/env bash
# Runs a multiplayer Editor test in three Editors at once: host in Products/<Product>, player in
# Products/<Product>-peer1, spectator in Products/<Product>-peer2 (created with make_peer_project.sh).
#
#   Tools/run_multiplayer_test.sh MOFA MofaHuntingMultiplayerTests [--out DIR]
#
# Each Editor runs the same explicit test with HOLOKIT_MP_ROLE set; roles exchange checkpoints
# through files. The Editor transport is UnityTransport on 127.0.0.1:7777, so this checks game
# networking (joining, spawning, RPCs, synced state, spectator view), not Multipeer Connectivity
# or QR alignment. Exits non-zero unless all three roles pass.
set -uo pipefail
REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY="${UNITY_CLI:-$HOME/.unity/bin/unity}"
EDITOR="${UNITY_EDITOR:-/Applications/Unity/6000.3.26f1/Unity.app}"
PRODUCT="${1:?product, e.g. MOFA}"
FILTER="${2:?test filter, e.g. MofaHuntingMultiplayerTests}"
OUT="$REPO/output/multiplayer-$PRODUCT-$(date +%Y%m%d-%H%M%S)"
[[ "${3:-}" == "--out" ]] && OUT="${4:?}"
mkdir -p "$OUT/exchange"
"$REPO/Tools/asset_snapshot.sh" save "$OUT/assets-before.txt"

for index in 1 2; do
  [[ -d "$REPO/Products/$PRODUCT-peer$index" ]] || "$REPO/Tools/make_peer_project.sh" "$PRODUCT" "$index" > /dev/null
done

run_role() {  # role, project
  HOLOKIT_MP_ROLE="$1" HOLOKIT_MP_DIR="$OUT/exchange" HOLOKIT_PLAYTHROUGH_OUTPUT="$OUT/$1" \
    "$UNITY" test "$2" --mode EditMode --filter "$FILTER" --editor-path "$EDITOR" \
    --output "$OUT/$1.xml" --timeout 1800 --no-banner -- -buildTarget iOS -logFile "$OUT/$1.log" \
    > "$OUT/$1.cli.txt" 2>&1
  echo $? > "$OUT/$1.exit"
}

run_role host "$REPO/Products/$PRODUCT" &
run_role player "$REPO/Products/$PRODUCT-peer1" &
run_role spectator "$REPO/Products/$PRODUCT-peer2" &
wait

FAILED=0
for role in host player spectator; do
  code="$(cat "$OUT/$role.exit")"
  message="$(python3 - "$OUT/$role.xml" <<'PY'
import re, sys
try:
    x = open(sys.argv[1]).read()
    case = re.search(r'<test-case [^>]*result="(\w+)"[^>]*>(.*?)</test-case>', x, re.S)
    note = re.search(r'<message><!\[CDATA\[(.*?)\]\]>', case.group(2), re.S) if case else None
    print((case.group(1) if case else "no test ran") + (": " + note.group(1).strip().splitlines()[0] if note else ""))
except Exception as error:
    print(f"no report ({error})")
PY
)"
  printf '%-10s exit %-3s %s\n' "$role" "$code" "$message" | tee -a "$OUT/summary.txt"
  [[ "$code" == 0 && "$message" == Passed* ]] || FAILED=1
done
if ! changed="$("$REPO/Tools/asset_snapshot.sh" check "$OUT/assets-before.txt")"; then
  echo "assets     FAIL tests modified: $(echo "$changed" | tr '\n' ' ')" | tee -a "$OUT/summary.txt"
  FAILED=1
fi
echo "Checkpoints: $(ls "$OUT/exchange" | tr '\n' ' ')"
exit $FAILED
