#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT_DIR"

fail() {
  echo "[FAIL] $1"
  exit 1
}

pass() {
  echo "[OK] $1"
}

current_branch="$(git branch --show-current)"
if [[ "$current_branch" =~ ^(develop|master|feature/.+|bugfix/.+|release/.+|hotfix/.+)$ ]]; then
  pass "Branch dentro do padrão GitFlow: $current_branch"
else
  fail "Branch fora do padrão GitFlow: $current_branch"
fi

last_subject="$(git log -1 --pretty=%s)"
# Bloqueia explicitamente padrões comuns de mojibake para evitar recorrência.
if grep -Eq 'ï¿¿|�|\xEF\xBF\xBD' <<<"$last_subject"; then
  fail "Mensagem de commit com mojibake detectado: $last_subject"
fi

# Esperado: <prefixo com emoji> tipo(escopo opcional): descrição
if [[ "$last_subject" =~ ^([^[:space:]]+)[[:space:]]+([a-z]+(\([a-zA-Z0-9._/-]+\))?:[[:space:]].+) ]]; then
  emoji_token="${BASH_REMATCH[1]}"
  if [[ "$emoji_token" =~ [^[:alnum:]] ]]; then
    pass "Mensagem de commit no padrão com emoji"
  else
    fail "Mensagem de commit fora do padrão com emoji: $last_subject"
  fi
else
  fail "Mensagem de commit fora do padrão com emoji: $last_subject"
fi

pr_template="$ROOT_DIR/.github/PULL_REQUEST_TEMPLATE.md"
[[ -f "$pr_template" ]] || fail "Template de PR ausente"

first_line="$(head -n 1 "$pr_template")"
[[ "$first_line" =~ ^#[[:space:]]+ ]] || fail "Template de PR sem H1 na primeira linha (MD041)"

for section in "## Objetivo da Mudança" "## Critérios de Aceite Cobertos" "## Evidências" "## Riscos" "## Checklist" "## Issue Vinculada"; do
  grep -Fq "$section" "$pr_template" || fail "Seção ausente no template de PR: $section"
done
pass "Template de PR validado"

if [[ -f "$ROOT_DIR/.markdownlint.jsonc" ]]; then
  pass "Configuração markdownlint presente"
else
  fail "Arquivo .markdownlint.jsonc ausente"
fi

echo "Flow Guard concluído com sucesso."
