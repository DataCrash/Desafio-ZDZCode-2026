#!/usr/bin/env bash
set -euo pipefail

: "${GH_OWNER:?Defina GH_OWNER}"
: "${GH_REPO:?Defina GH_REPO}"
BOARD_TITLE="${BOARD_TITLE:-Board Desafio ZDZCode 2026}"

ROOT_DIR="$(cd "$(dirname "$0")/../.." && pwd)"
BOOTSTRAP_SCRIPT="$ROOT_DIR/scripts/github/bootstrap_board.sh"

if [[ ! -x "$BOOTSTRAP_SCRIPT" ]]; then
  echo "Erro: script não executável ou inexistente: $BOOTSTRAP_SCRIPT"
  exit 1
fi

echo "==> Board Ops: sincronizando board e backlog"
GH_OWNER="$GH_OWNER" GH_REPO="$GH_REPO" BOARD_TITLE="$BOARD_TITLE" "$BOOTSTRAP_SCRIPT"

echo "==> Board Ops: resumo"
GH_BIN="${GH_BIN:-$(command -v gh || true)}"
if [[ -z "$GH_BIN" && -x "/c/Program Files/GitHub CLI/gh.exe" ]]; then
  GH_BIN="/c/Program Files/GitHub CLI/gh.exe"
fi
if [[ -z "$GH_BIN" ]]; then
  echo "Aviso: gh indisponível para resumo final."
  exit 0
fi

"$GH_BIN" issue list --repo "$GH_OWNER/$GH_REPO" --limit 300 --json number | jq -r '"Issues no repositório: " + (length|tostring)'
"$GH_BIN" project list --owner "$GH_OWNER" --format json | jq -r --arg t "$BOARD_TITLE" '.projects[] | select(.title == $t) | "Board: #" + (.number|tostring) + " - " + .title'
