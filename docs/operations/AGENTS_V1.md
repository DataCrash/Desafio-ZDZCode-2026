# Agentes V1 - Operação

Este documento descreve a implementação inicial dos agentes operacionais do projeto.

## Objetivo

- Reduzir retrabalho em tarefas repetitivas de board e governança.
- Aplicar regras de fluxo de desenvolvimento com validações rápidas.
- Melhorar previsibilidade antes de iniciar ciclos de implementação.

## Agentes Implementados

## 1) Board Ops

Responsável principal: Tech Lead / Product Ops

Script:

- `scripts/agents/board_ops.sh`

Função:

- Executa sincronização de labels, milestones, épico e backlog.
- Gera resumo final com total de issues e board encontrado.

Uso:

```bash
GH_OWNER="DataCrash" \
GH_REPO="Desafio-ZDZCode-2026" \
BOARD_TITLE="Board Desafio ZDZCode 2026" \
./scripts/agents/board_ops.sh
```

## 2) Flow Guard

Responsável principal: Tech Lead

Script:

- `scripts/agents/flow_guard.sh`

Função:

- Valida branch no padrão GitFlow.
- Valida formato da última mensagem de commit com emoji.
- Valida estrutura mínima do template de PR.
- Valida presença do arquivo de lint de Markdown.

Uso:

```bash
./scripts/agents/flow_guard.sh
```

## 3) Delivery Prep

Responsável principal: QA Lead / Tech Lead

Script:

- `scripts/agents/delivery_prep.sh`

Função:

- Compara total de issues do repositório com total de itens no board.
- Detecta inconsistência de cards fora do board.
- Emite status de prontidão para ciclo de desenvolvimento.

Uso:

```bash
GH_OWNER="DataCrash" \
GH_REPO="Desafio-ZDZCode-2026" \
BOARD_TITLE="Board Desafio ZDZCode 2026" \
./scripts/agents/delivery_prep.sh
```

## Sequência Recomendada

1. Rodar `Flow Guard` antes de abrir PR.
2. Rodar `Board Ops` no início do ciclo (ou quando mudar backlog).
3. Rodar `Delivery Prep` antes de planejamento de execução.

## Limites da V1

- Não altera automaticamente status de cards por coluna.
- Não preenche campos customizados do board por label.
- Não substitui revisão humana de prioridade e escopo.

## Próxima Iteração

- Sincronizar labels -> campos customizados do board.
- Criar rotina de movimentação de status por eventos de PR.
- Adicionar relatório diário de progresso por fase.
