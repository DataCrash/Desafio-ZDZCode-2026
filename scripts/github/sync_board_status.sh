#!/usr/bin/env bash
set -euo pipefail

: "${GH_OWNER:?Defina GH_OWNER}"
: "${GH_REPO:?Defina GH_REPO}"
BOARD_TITLE="${BOARD_TITLE:-Board Desafio ZDZCode 2026}"

GH_BIN="${GH_BIN:-$(command -v gh || true)}"
if [[ -z "$GH_BIN" && -x "/c/Program Files/GitHub CLI/gh.exe" ]]; then
  GH_BIN="/c/Program Files/GitHub CLI/gh.exe"
fi

JQ_BIN="${JQ_BIN:-$(command -v jq || true)}"
if [[ -z "$JQ_BIN" && -x "/c/Users/$USERNAME/AppData/Local/Microsoft/WinGet/Links/jq.exe" ]]; then
  JQ_BIN="/c/Users/$USERNAME/AppData/Local/Microsoft/WinGet/Links/jq.exe"
fi

[[ -n "$GH_BIN" ]] || { echo "Erro: gh nao encontrado"; exit 1; }
[[ -n "$JQ_BIN" ]] || { echo "Erro: jq nao encontrado"; exit 1; }

project_json="$("$GH_BIN" project list --owner "$GH_OWNER" --format json | "$JQ_BIN" -c --arg t "$BOARD_TITLE" '.projects[] | select(.title == $t)')"
if [[ -z "$project_json" ]]; then
  echo "Erro: board nao encontrado: $BOARD_TITLE"
  exit 1
fi

project_number="$("$JQ_BIN" -r '.number' <<<"$project_json")"
project_id="$("$JQ_BIN" -r '.id' <<<"$project_json")"

fields_json="$("$GH_BIN" project field-list "$project_number" --owner "$GH_OWNER" --format json)"
status_field_id="$("$JQ_BIN" -r '.fields[] | select(.name == "Status") | .id' <<<"$fields_json" | head -n1)"

if [[ -z "$status_field_id" ]]; then
  echo "Aviso: campo Status nao encontrado no board."
  exit 0
fi

todo_option_id="$("$JQ_BIN" -r '.fields[] | select(.name == "Status") | .options[] | select(.name == "Todo") | .id' <<<"$fields_json" | head -n1)"
in_progress_option_id="$("$JQ_BIN" -r '.fields[] | select(.name == "Status") | .options[] | select(.name == "In Progress") | .id' <<<"$fields_json" | head -n1)"
done_option_id="$("$JQ_BIN" -r '.fields[] | select(.name == "Status") | .options[] | select(.name == "Done") | .id' <<<"$fields_json" | head -n1)"

[[ -n "$todo_option_id" ]] || { echo "Erro: opcao 'Todo' ausente no campo Status"; exit 1; }
[[ -n "$in_progress_option_id" ]] || { echo "Erro: opcao 'In Progress' ausente no campo Status"; exit 1; }
[[ -n "$done_option_id" ]] || { echo "Erro: opcao 'Done' ausente no campo Status"; exit 1; }

items_json="$("$GH_BIN" project item-list "$project_number" --owner "$GH_OWNER" --limit 500 --format json)"
item_count="$("$JQ_BIN" -r '.items | length' <<<"$items_json")"

if [[ "$item_count" == "0" ]]; then
  echo "Aviso: nenhum item encontrado no board para sincronizar status."
  exit 0
fi

updated=0
while IFS=$'\t' read -r item_id issue_state labels; do
  [[ -n "$item_id" ]] || continue

  target_option_id="$todo_option_id"
  target_name="Todo"

  if [[ "$issue_state" == "CLOSED" ]]; then
    target_option_id="$done_option_id"
    target_name="Done"
  elif grep -q 'status:in-progress' <<<"$labels"; then
    target_option_id="$in_progress_option_id"
    target_name="In Progress"
  fi

  "$GH_BIN" project item-edit \
    --id "$item_id" \
    --project-id "$project_id" \
    --field-id "$status_field_id" \
    --single-select-option-id "$target_option_id" >/dev/null

  updated=$((updated + 1))
  echo "  - item $item_id => $target_name"
done < <("$JQ_BIN" -r '.items[] | [ .id, (.content.state // "OPEN"), ((.labels // []) | join(",")) ] | @tsv' <<<"$items_json")

echo "Status sincronizado para $updated itens no board #$project_number."
