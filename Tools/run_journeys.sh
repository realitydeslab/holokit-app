#!/usr/bin/env bash
# Runs every end-to-end journey (a simulated person in XR Simulation), one after another, and prints a summary:
# single-Editor journeys in their product, three-Editor journeys through run_multiplayer_test.sh.
#
#   Tools/run_journeys.sh [--out DIR]
#
# About an hour. Exits non-zero unless every journey passes. See Documentation/Testing.md.
set -uo pipefail
REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY="${UNITY_CLI:-$HOME/.unity/bin/unity}"
EDITOR="${UNITY_EDITOR:-/Applications/Unity/6000.3.27f1/Unity.app}"
OUT="$REPO/output/journeys-$(date +%Y%m%d-%H%M%S)"
[[ "${1:-}" == "--out" ]] && OUT="${2:?}"
mkdir -p "$OUT"

SINGLE=(  # product:filter
  "QRealm:QuantumRealmJourneyTests"
  "MOFA:MofaTrainingJourneyTests|MofaPopupTests"
  "Typed:TypedTheFlockDrawTests|TypedTheSculptureDrawTests|TornadoFloorRayTests"
)
MULTI=(  # product:class
  "MOFA:MofaDuelJourneyTests" "MOFA:MofaHuntingJourneyTests"
  "Typed:TypedTheFlockJourneyTests" "Typed:TypedTheRainJourneyTests" "Typed:TypedTheTornadoJourneyTests"
  "Typed:TypedTheHairJourneyTests" "Typed:TypedTheScannerJourneyTests" "Typed:TypedTheFingerRibbonJourneyTests"
  "Typed:TypedTheSculptureJourneyTests"
)

FAILED=0
for entry in "${SINGLE[@]}"; do
  product="${entry%%:*}"; filter="${entry#*:}"; dir="$OUT/$product-single"; mkdir -p "$dir"
  HOLOKIT_PLAYTHROUGH_OUTPUT="$dir" "$UNITY" test "$REPO/Products/$product" --mode EditMode --filter "$filter" \
    --editor-path "$EDITOR" --output "$dir/r.xml" --timeout 3000 --no-banner -- -buildTarget iOS -logFile "$dir/r.log" \
    > "$dir/r.cli.txt" 2>&1
  code=$?
  counts="$(grep -o 'passed="[0-9]*" failed="[0-9]*"' "$dir/r.xml" 2>/dev/null | head -1)"
  printf '%-8s %-60s exit %-3s %s\n' "$product" "$filter" "$code" "${counts:-no report}" | tee -a "$OUT/summary.txt"
  [[ "$code" == 0 ]] || FAILED=1
done
for entry in "${MULTI[@]}"; do
  product="${entry%%:*}"; class="${entry#*:}"
  if "$REPO/Tools/run_multiplayer_test.sh" "$product" "$class" --out "$OUT/$class" > "$OUT/$class.txt" 2>&1; then result=passed
  else result="FAILED: $(grep -m1 -v '^Checkpoints' "$OUT/$class.txt")"; FAILED=1; fi
  printf '%-8s %-60s %s\n' "$product" "$class" "$result" | tee -a "$OUT/summary.txt"
done
echo "Traces, captures and logs: $OUT"
exit $FAILED
