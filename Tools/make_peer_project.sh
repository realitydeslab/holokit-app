#!/usr/bin/env bash
# Creates Products/<Product>-peer<N>: a second Unity project that shares Assets, Packages and
# ProjectSettings with Products/<Product> through symlinks but has its own Library, so another
# Editor can run the same product at the same time (Unity locks a project to one Editor).
#
#   Tools/make_peer_project.sh MOFA 1
#
# The peer sits beside the product, so the manifest's relative file:../../../Packages paths resolve
# to the same packages. Peers are local build state and are ignored by Git.
set -euo pipefail
REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PRODUCT="${1:?product name, e.g. MOFA}"
INDEX="${2:-1}"
SOURCE="$REPO/Products/$PRODUCT"
PEER="$REPO/Products/$PRODUCT-peer$INDEX"
[[ -d "$SOURCE/Assets" ]] || { echo "No product at $SOURCE" >&2; exit 2; }
mkdir -p "$PEER"
for entry in Assets Packages ProjectSettings WatchApp; do
  [[ -e "$SOURCE/$entry" ]] || continue
  if [[ -L "$PEER/$entry" ]]; then continue; fi
  [[ -e "$PEER/$entry" ]] && { echo "$PEER/$entry exists and is not a symlink" >&2; exit 1; }
  ln -s "../$PRODUCT/$entry" "$PEER/$entry"
done
echo "$PEER"
