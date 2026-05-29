# Glossario Canonico

Este arquivo define os nomes oficiais do dominio. Nao renomear sem ordem explicita.

## Entidades

- ZDZCode: nome canonico do repositorio/workspace atual.
- Obsidian vault: a pasta .copilot/memory aberta no Obsidian como fonte de verdade operacional.
- MOC_HOME: dashboard central da memoria operacional.
- Tarefa Ativa: nota operacional corrente em notes/ACTIVE_TASK.md.
- Inbox: nota de captura rapida em notes/INBOX.md.

## Variaveis e Campos Criticos

- develop: branch canonica de desenvolvimento.
- master: branch canonica de release/comparacao.
- forward-only: politica canonica de historico sem reescrita destrutiva.
- escopo fechado: conjunto explicito e verificavel do que foi pedido em uma tarefa.
- nomes canonicos: nomes oficiais que nao podem ser alterados sem ordem explicita.

## Modulos e Servicos

- .copilot: modulo de memoria isolada do sistema.
- .copilot/memory: raiz canonica da memoria operacional.
- context: modulo de contexto e regras de execucao.
- decisions: modulo de registro de decisoes.
- patterns: modulo de padroes e nomenclatura.
- runbooks: modulo de procedimentos operacionais.
- templates: modulo de modelos reutilizaveis.
- maps: modulo de mapas de conteudo do vault.
- notes: modulo de notas operacionais.

## Eventos e Contratos

- STRICT_MODE: contrato operacional que impede deriva de escopo e renomeacoes indevidas.
- EXECUTION_CHECKLIST: contrato minimo de verificacao antes de editar e antes de entregar.
- TASK_BRIEF_TEMPLATE: contrato base para formalizar pedido, escopo, proibicoes e criterio de pronto.
- AUTOMATION_BLUEPRINT: contrato de governanca para propor agentes, skills e tools antes de criar.

## Regra

- Se o nome nao estiver definido aqui, o nome atual no codigo e o canonico temporario.
- Se surgir novo nome recorrente, registrar aqui antes de renomear ou generalizar.
