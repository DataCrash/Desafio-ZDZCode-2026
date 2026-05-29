# Setup Operacional do Board

Este guia deixa o ambiente pronto para registrar backlog, cards e começar o desenvolvimento.

## Pré-requisitos

- GitHub CLI (`gh`) instalado
- `jq` instalado
- Login no GitHub CLI feito (`gh auth login`)
- Repositório remoto já criado no GitHub

## Artefatos já preparados no repositório

- Template de issue: `.github/ISSUE_TEMPLATE/card.yml`
- Template de PR: `.github/PULL_REQUEST_TEMPLATE.md`
- Seed de labels: `.github/project/labels.json`
- Seed de milestones: `.github/project/milestones.json`
- Seed de backlog: `.github/project/backlog.json`
- Script de bootstrap: `scripts/github/bootstrap_board.sh`

## Execução em 1 comando

```bash
GH_OWNER="SEU_USUARIO_OU_ORG" \
GH_REPO="Desafio-ZDZCode-2026" \
BOARD_TITLE="Board Desafio ZDZCode 2026" \
./scripts/github/bootstrap_board.sh
```

## O que o script faz

1. Cria/atualiza labels padronizadas
2. Cria/atualiza milestones por fase
3. Cria/atualiza épico principal
4. Cria/atualiza 16 issues do backlog inicial
5. Tenta criar e preencher o GitHub Project (board), quando `gh project` estiver disponível

## Colunas recomendadas no board

- Backlog
- Ready
- In Progress
- Review
- Blocked
- Done

## Campos recomendados no board

- Área
- Tipo
- Prioridade
- Fase
- Crítico
- Evidência

## Regra operacional

- WIP máximo: 1 card por vez
- Todo card deve citar critério do PDF impactado
- Todo PR deve vincular issue e anexar evidência de validação

## Fluxo diário sugerido

1. Puxar card `Ready` para `In Progress`
2. Implementar com branch `feature/*` ou `bugfix/*` a partir de `develop`
3. Abrir PR para `develop` com checklist completo
4. Após merge, mover card para `Done`
