#!/bin/bash
# Stub Unity CLI for tool tests. Appends its arguments to $FAKE_LOG. For "test" it writes an
# NUnit summary to --output and fails (exit 8) when $FAKE_FAIL is part of the --filter value.
echo "$@" >> "${FAKE_LOG:-/dev/null}"
out=""; filter=""
while [[ $# -gt 0 ]]; do case "$1" in --output) out="$2"; shift 2 ;; --filter) filter="$2"; shift 2 ;; *) shift ;; esac; done
if [[ -n "$out" ]]; then
  if [[ -n "${FAKE_FAIL:-}" && "$filter" == *"$FAKE_FAIL"* ]]; then
    echo '<test-run total="2" passed="1" failed="1" skipped="0">' > "$out"; exit 8
  fi
  echo '<test-run total="3" passed="3" failed="0" skipped="0">' > "$out"
fi
exit 0
