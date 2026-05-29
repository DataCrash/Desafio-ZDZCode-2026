# Blueprint de Automação (Agentes, Skills e Tools)

Regra de governança: nada novo é criado sem proposta objetiva + aprovação explícita.

## Quando propor um novo Agente

Propor apenas se houver repetição de fluxo multi-etapa com risco alto de erro manual.

Critério objetivo (todos devem ser verdadeiros):

- O fluxo aparece em pelo menos 3 tarefas por semana.
- O fluxo exige 5 ou mais passos.
- Erros nesse fluxo causam retrabalho relevante.

## Quando propor uma nova Skill

Propor apenas se houver conhecimento estável que precisa ser aplicado de forma consistente.

Critério objetivo:

- Existe regra fixa que não deve variar por tarefa.
- Essa regra impacta qualidade, custo ou segurança.
- A regra já foi repetida manualmente pelo menos 3 vezes.

## Quando propor uma nova Tool

Propor apenas se faltar capacidade real para executar uma etapa crítica.

Critério objetivo:

- A capacidade não existe nas tools atuais.
- Sem essa capacidade, a entrega fica parcial ou lenta.
- Há ganho claro de tempo ou redução de falha.

## Formato de Proposta Obrigatorio

- Problema concreto
- Solucao proposta
- Critério irrefutável de ganho
- Risco de não implementar
- Escopo mínimo da implementação
- Plano em 3 passos

## Gate de Aprovação

- Etapa 1: proposta curta
- Etapa 2: sua aprovação
- Etapa 3: implementação controlada
