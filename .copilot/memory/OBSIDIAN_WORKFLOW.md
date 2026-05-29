# Workflow com Obsidian

Objetivo: usar Obsidian como cockpit operacional da memoria, com execucao sem deriva.

## Fonte de Verdade

- Vault: .copilot/memory
- Documento inicial: MOC_HOME.md
- Nao duplicar conteudo em outra pasta.

## Fluxo Diario

1. Abrir MOC_HOME.md
2. Preencher notes/ACTIVE_TASK.md com pedido e escopo fechado
3. Passar no runbooks/EXECUTION_CHECKLIST.md antes de editar
4. Confirmar nomenclatura em patterns/CANONICAL_GLOSSARY.md
5. Ao concluir, registrar decisao/contexto quando necessario

## Fluxo de Decisao

1. Nova decisao relevante: criar nota usando templates/obsidian/DECISION_NOTE_TEMPLATE.md
2. Linkar a decisao no maps/MOC_DECISIONS.md
3. Atualizar impactados (contexto, padroes, runbooks)

## Regras de Controle

- Modo estrito sempre ativo: context/STRICT_MODE.md
- Qualquer proposta de automacao segue: AUTOMATION_BLUEPRINT.md
- Sem aprovacao explicita, nao criar agentes/skills/tools novos
