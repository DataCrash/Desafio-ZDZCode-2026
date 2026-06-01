# Workflow com Obsidian

Objetivo: usar Obsidian como cockpit operacional da memória, com execução sem deriva.

## Fonte de Verdade

- Vault: .copilot/memory
- Documento inicial: MOC_HOME.md
- Não duplicar conteúdo em outra pasta.

## Fluxo Diário

1. Abrir MOC_HOME.md
2. Preencher notes/ACTIVE_TASK.md com pedido e escopo fechado
3. Passar no runbooks/EXECUTION_CHECKLIST.md antes de editar
4. Confirmar nomenclatura em patterns/CANONICAL_GLOSSARY.md
5. Ao concluir, registrar decisão/contexto quando necessário

## Fluxo de Decisão

1. Nova decisão relevante: criar nota usando templates/obsidian/DECISION_NOTE_TEMPLATE.md
2. Linkar a decisão no maps/MOC_DECISIONS.md
3. Atualizar impactados (contexto, padrões, runbooks)

## Regras de Controle

- Modo estrito sempre ativo: context/STRICT_MODE.md
- Qualquer proposta de automação segue: AUTOMATION_BLUEPRINT.md
- Sem aprovação explícita, não criar agentes/skills/tools novos
