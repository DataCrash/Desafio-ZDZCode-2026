#!/usr/bin/env bash
set -euo pipefail

: "${GH_OWNER:?Defina GH_OWNER}"
: "${GH_REPO:?Defina GH_REPO}"
BOARD_TITLE="${BOARD_TITLE:-Board Desafio ZDZCode 2026}"

GH_BIN="${GH_BIN:-$(command -v gh || true)}"
if [[ -z "$GH_BIN" && -x "/c/Program Files/GitHub CLI/gh.exe" ]]; then
  GH_BIN="/c/Program Files/GitHub CLI/gh.exe"
fi
[[ -n "$GH_BIN" ]] || { echo "Erro: gh não encontrado"; exit 1; }

echo "==> Delivery Prep"
issues_total="$($GH_BIN issue list --repo "$GH_OWNER/$GH_REPO" --limit 300 --json number | jq 'length')"
echo "Issues no repositório: $issues_total"

board_number="$($GH_BIN project list --owner "$GH_OWNER" --format json | jq -r --arg t "$BOARD_TITLE" '.projects[] | select(.title == $t) | .number' | head -n1)"
if [[ -z "$board_number" ]]; then
  echo "Erro: board não encontrado: $BOARD_TITLE"
  exit 1
fi

items_total="$($GH_BIN project item-list "$board_number" --owner "$GH_OWNER" --format json | jq '.items | length')"
echo "Itens no board #$board_number: $items_total"

if [[ "$items_total" -lt "$issues_total" ]]; then
  echo "Aviso: há issues fora do board. Execute Board Ops novamente."
else
  echo "OK: board sincronizado com issues do repositório."
fi

echo "Entrega pronta para ciclo de desenvolvimento e PR."
