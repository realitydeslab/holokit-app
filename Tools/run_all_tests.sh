#!/usr/bin/env bash
# Runs every automated check for the four apps, one product at a time.
#
#   Tools/run_all_tests.sh [--products "HoloKit MOFA Typed QRealm"] [--skip-playthrough] [--export] [--multiplayer] [--out DIR]
#
# First the tool tests (Tools/tests). Per product: the EditMode suite, the Editor scene playthrough with XR Simulation, and
# with --export a Unity iOS export checked by Tools/verify_ios_export.py, and with --multiplayer the
# three-Editor tests in MULTIPLAYER_TESTS (Tools/run_multiplayer_test.sh).
# Products run sequentially: parallel Editors collide on the local Netcode port 7777 and
# recompile the shared packages at the same time. Close Unity Editors on these projects first.
# Exits non-zero if anything fails. Needs the Unity CLI and Unity 6000.3.24f1.
set -uo pipefail

REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY="${UNITY_CLI:-$HOME/.unity/bin/unity}"
EDITOR="${UNITY_EDITOR:-/Applications/Unity/6000.3.24f1/Unity.app}"
PRODUCTS="HoloKit MOFA Typed QRealm"
PLAYTHROUGH=1
EXPORT=0
MULTIPLAYER=0
# "<product>:<test class>" pairs run by --multiplayer.
MULTIPLAYER_TESTS="MOFA:CatalogMultiplayerSyncTests Typed:CatalogMultiplayerSyncTests QRealm:CatalogMultiplayerSyncTests MOFA:MofaHuntingMultiplayerTests MOFA:MofaDuelMultiplayerTests QRealm:QuantumRealmMultiplayerTests"
OUT="$REPO/output/test-run-$(date +%Y%m%d-%H%M%S)"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --products) PRODUCTS="$2"; shift 2 ;;
    --skip-playthrough) PLAYTHROUGH=0; shift ;;
    --export) EXPORT=1; shift ;;
    --multiplayer) MULTIPLAYER=1; shift ;;
    --out) OUT="$2"; shift 2 ;;
    -h|--help) sed -n '2,11p' "$0"; exit 0 ;;
    *) echo "Unknown option: $1" >&2; exit 2 ;;
  esac
done

project_dir() { [[ "$1" == HoloKit ]] && echo "$REPO" || echo "$REPO/Products/$1"; }
export_dir() { [[ "$1" == HoloKit ]] && echo "$REPO/Builds/Upgrade/HoloKit-four-products" || echo "$REPO/Products/$1/Builds/iOS"; }

mkdir -p "$OUT"
"$REPO/Tools/asset_snapshot.sh" save "$OUT/assets-before.txt"
SUMMARY="$OUT/summary.txt"
: > "$SUMMARY"
FAILED=0

record() {  # name, status, detail
  printf '%-10s %-24s %s\n' "$2" "$1" "$3" | tee -a "$SUMMARY"
  [[ "$2" == PASS ]] || FAILED=1
}

totals() {  # NUnit XML -> "passed/total, failed N"
  python3 - "$1" <<'EOF'
import re, sys
try:
    run = re.search(r'<test-run [^>]*', open(sys.argv[1]).read()).group(0)
    get = lambda k: int(re.search(k + r'="(\d+)"', run).group(1))
    print(f"{get('passed')}/{get('total')} passed, {get('failed')} failed, {get('skipped')} skipped")
except Exception as error:
    print(f"no report ({error})")
EOF
}

run_tests() {  # product, label, filter-or-empty
  local product="$1" label="$2" filter="$3" xml="$OUT/$1-$2.xml" args=()
  [[ -n "$filter" ]] && args=(--filter "$filter")
  HOLOKIT_PLAYTHROUGH_OUTPUT="$OUT/$product-playthrough" \
    "$UNITY" test "$(project_dir "$product")" --mode EditMode ${args[@]+"${args[@]}"} --editor-path "$EDITOR" \
    --output "$xml" --timeout 3600 --no-banner -- -buildTarget iOS -logFile "$OUT/$product-$label.log" \
    > "$OUT/$product-$label.cli.txt" 2>&1
  local code=$?
  if [[ $code -eq 0 ]]; then record "$product $label" PASS "$(totals "$xml")"
  else record "$product $label" FAIL "exit $code; $(totals "$xml"); see $OUT/$product-$label.log"; fi
}

# Tools/tests runs this script with a stub Unity; HOLOKIT_SKIP_TOOL_TESTS stops the recursion.
if [[ -n "${HOLOKIT_SKIP_TOOL_TESTS:-}" ]]; then
  :
elif python3 -m unittest discover -s "$REPO/Tools/tests" > "$OUT/tools-tests.txt" 2>&1; then
  record "tools" PASS "$(tail -n 3 "$OUT/tools-tests.txt" | head -n 1)"
else
  record "tools" FAIL "see $OUT/tools-tests.txt"
fi

for product in $PRODUCTS; do
  echo "== $product ($(project_dir "$product"))"
  run_tests "$product" editmode ""
  [[ $PLAYTHROUGH -eq 1 ]] && run_tests "$product" playthrough "EditorScenePlaythroughTests"
  if [[ $EXPORT -eq 1 ]]; then
    "$UNITY" build "$(project_dir "$product")" --profile iOS --output-path "$(export_dir "$product")" \
      --editor-path "$EDITOR" --no-tail -l "$OUT/$product-export.log" > "$OUT/$product-export.cli.txt" 2>&1
    code=$?
    if [[ $code -ne 0 ]]; then record "$product export" FAIL "exit $code; see $OUT/$product-export.log"
    elif python3 "$REPO/Tools/verify_ios_export.py" --project "$(project_dir "$product")" \
        --export "$(export_dir "$product")" --unity-log "$OUT/$product-export.log" > "$OUT/$product-verify.txt" 2>&1
    then record "$product export" PASS "$(export_dir "$product")"
    else record "$product export" FAIL "$(tail -n +2 "$OUT/$product-verify.txt" | tr '\n' ' ')"; fi
  fi
done

if [[ $MULTIPLAYER -eq 1 ]]; then
  for pair in $MULTIPLAYER_TESTS; do
    product="${pair%%:*}"; test="${pair#*:}"
    if "$REPO/Tools/run_multiplayer_test.sh" "$product" "$test" --out "$OUT/multiplayer-$product-$test" > "$OUT/multiplayer-$product-$test.txt" 2>&1
    then record "$product $test" PASS "host, player and spectator passed"
    else record "$product $test" FAIL "$(grep -v Passed "$OUT/multiplayer-$product-$test/summary.txt" 2>/dev/null | head -n 1)"; fi
  done
fi

if changed="$("$REPO/Tools/asset_snapshot.sh" check "$OUT/assets-before.txt")"; then
  record "assets unchanged" PASS "tests wrote no tracked files"
else
  record "assets unchanged" FAIL "tests modified: $(echo "$changed" | tr '\n' ' ')"
fi

echo
echo "Summary ($SUMMARY):"
cat "$SUMMARY"
if git -C "$REPO" status --short -- Products/*/Assets/XR | grep -q .; then
  echo "Note: Unity rewrote XR Simulation settings under Products/*/Assets/XR; review before committing."
fi
exit $FAILED
