#!/usr/bin/env bash
set -euo pipefail

# Uso:
# GH_OWNER="seu-usuario" GH_REPO="Desafio-ZDZCode-2026" BOARD_TITLE="Board Desafio ZDZCode 2026" ./scripts/github/bootstrap_board.sh

ROOT_DIR="$(cd "$(dirname "$0")/../.." && pwd)"
LABELS_FILE="$ROOT_DIR/.github/project/labels.json"
MILESTONES_FILE="$ROOT_DIR/.github/project/milestones.json"
BACKLOG_FILE="$ROOT_DIR/.github/project/backlog.json"

: "${GH_OWNER:?Defina GH_OWNER}"
: "${GH_REPO:?Defina GH_REPO}"
BOARD_TITLE="${BOARD_TITLE:-Board Desafio ZDZCode 2026}"
REPO="$GH_OWNER/$GH_REPO"

GH_BIN="${GH_BIN:-$(command -v gh || true)}"
if [[ -z "$GH_BIN" && -x "/c/Program Files/GitHub CLI/gh.exe" ]]; then
  GH_BIN="/c/Program Files/GitHub CLI/gh.exe"
fi

JQ_BIN="${JQ_BIN:-$(command -v jq || true)}"
if [[ -z "$JQ_BIN" && -x "/c/Users/$USERNAME/AppData/Local/Microsoft/WinGet/Links/jq.exe" ]]; then
  JQ_BIN="/c/Users/$USERNAME/AppData/Local/Microsoft/WinGet/Links/jq.exe"
fi

if [[ -z "$GH_BIN" ]]; then
  echo "Erro: GitHub CLI (gh) não encontrado."
  exit 1
fi

if [[ -z "$JQ_BIN" ]]; then
  echo "Erro: jq não encontrado."
  exit 1
fi

gh() {
  "$GH_BIN" "$@"
}

jq() {
  "$JQ_BIN" "$@"
}

gh auth status >/dev/null

echo "==> Validando acesso ao repositório $REPO"
gh repo view "$REPO" >/dev/null

echo "==> Criando/atualizando labels"
jq -c '.[]' "$LABELS_FILE" | while IFS= read -r label; do
  name="$(jq -r '.name' <<<"$label")"
  color="$(jq -r '.color' <<<"$label")"
  desc="$(jq -r '.description' <<<"$label")"
  gh label create "$name" --repo "$REPO" --color "$color" --description "$desc" --force >/dev/null
  echo "  - $name"
done

get_milestone_number() {
  local title="$1"
  gh api "repos/$REPO/milestones?state=all&per_page=100" | jq -r --arg t "$title" '.[] | select(.title == $t) | .number' | head -n1
}

echo "==> Criando milestones"
jq -c '.[]' "$MILESTONES_FILE" | while IFS= read -r ms; do
  title="$(jq -r '.title' <<<"$ms")"
  description="$(jq -r '.description' <<<"$ms")"
  state="$(jq -r '.state' <<<"$ms")"
  number="$(get_milestone_number "$title")"

  if [[ -z "$number" ]]; then
    gh api "repos/$REPO/milestones" \
      -f title="$title" \
      -f description="$description" \
      -f state="$state" >/dev/null
    echo "  - criada: $title"
  else
    gh api "repos/$REPO/milestones/$number" \
      -X PATCH \
      -f title="$title" \
      -f description="$description" \
      -f state="$state" >/dev/null
    echo "  - atualizada: $title"
  fi
done

issue_number_by_title() {
  local title="$1"
  gh issue list --repo "$REPO" --state all --limit 200 --json number,title \
    | jq -r --arg t "$title" '.[] | select(.title == $t) | .number' | head -n1
}

create_or_update_issue() {
  local title="$1"
  local body="$2"
  local labels_csv="$3"
  local milestone_title="$4"

  local milestone_number
  milestone_number="$(get_milestone_number "$milestone_title")"

  if [[ -z "$milestone_number" ]]; then
    echo "Erro: milestone não encontrada: $milestone_title"
    exit 1
  fi

  local existing_number
  existing_number="$(issue_number_by_title "$title")"

  if [[ -z "$existing_number" ]]; then
    gh issue create \
      --repo "$REPO" \
      --title "$title" \
      --body "$body" \
      --label "$labels_csv" \
      --milestone "$milestone_title" >/dev/null
    echo "  - criada: $title"
  else
    gh issue edit "$existing_number" \
      --repo "$REPO" \
      --title "$title" \
      --body "$body" \
      --milestone "$milestone_title" >/dev/null

    # Sincroniza labels: remove todas e reaplica as do seed
    current_labels="$(gh issue view "$existing_number" --repo "$REPO" --json labels | jq -r '.labels[].name' || true)"
    if [[ -n "$current_labels" ]]; then
      while IFS= read -r lbl; do
        [[ -n "$lbl" ]] && gh issue edit "$existing_number" --repo "$REPO" --remove-label "$lbl" >/dev/null || true
      done <<<"$current_labels"
    fi

    IFS=',' read -ra labels_arr <<<"$labels_csv"
    for lbl in "${labels_arr[@]}"; do
      gh issue edit "$existing_number" --repo "$REPO" --add-label "$lbl" >/dev/null
    done

    echo "  - atualizada: $title"
  fi
}

echo "==> Criando/atualizando épico"
epic_title="$(jq -r '.epic.title' "$BACKLOG_FILE")"
epic_body="$(jq -r '.epic.body' "$BACKLOG_FILE")"
epic_milestone="$(jq -r '.epic.milestone' "$BACKLOG_FILE")"
epic_labels="$(jq -r '.epic.labels | join(",")' "$BACKLOG_FILE")"
create_or_update_issue "$epic_title" "$epic_body" "$epic_labels" "$epic_milestone"

echo "==> Criando/atualizando backlog inicial"
jq -c '.issues[]' "$BACKLOG_FILE" | while IFS= read -r item; do
  title="$(jq -r '.title' <<<"$item")"
  body="$(jq -r '.body' <<<"$item")"
  labels="$(jq -r '.labels | join(",")' <<<"$item")"
  milestone="$(jq -r '.milestone' <<<"$item")"
  create_or_update_issue "$title" "$body" "$labels" "$milestone"
done

# Etapa opcional: GitHub Project (Board)
if gh project list --owner "$GH_OWNER" >/dev/null 2>&1; then
  echo "==> Garantindo existência do board (GitHub Project)"
  project_number="$(gh project list --owner "$GH_OWNER" --format json | jq -r --arg t "$BOARD_TITLE" '.projects[] | select(.title == $t) | .number' | head -n1)"

  if [[ -z "$project_number" ]]; then
    gh project create --owner "$GH_OWNER" --title "$BOARD_TITLE" >/dev/null
    project_number="$(gh project list --owner "$GH_OWNER" --format json | jq -r --arg t "$BOARD_TITLE" '.projects[] | select(.title == $t) | .number' | head -n1)"
    echo "  - board criado: $BOARD_TITLE (#$project_number)"
  else
    echo "  - board já existe: $BOARD_TITLE (#$project_number)"
  fi

  echo "==> Vinculando issues ao board"
  gh issue list --repo "$REPO" --limit 300 --json url,title | jq -r '.[] | .url' | while IFS= read -r issue_url; do
    gh project item-add "$project_number" --owner "$GH_OWNER" --url "$issue_url" >/dev/null || true
  done
  echo "  - issues vinculadas"
else
  echo "==> Aviso: comando 'gh project' indisponível nesta instalação do GH CLI."
  echo "   As issues foram criadas/atualizadas; o board pode ser montado manualmente em segundos."
fi

echo
echo "Concluído: labels, milestones, épico e backlog inicial prontos no repositório $REPO."
