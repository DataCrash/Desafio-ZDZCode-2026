# Blueprint de Automacao (Agentes, Skills e Tools)

Regra de governanca: nada novo e criado sem proposta objetiva + aprovacao explicita.

## Quando propor um novo Agente

Propor apenas se houver repeticao de fluxo multi-etapa com risco alto de erro manual.

Criterio objetivo (todos devem ser verdadeiros):

- O fluxo aparece em pelo menos 3 tarefas por semana.
- O fluxo exige 5 ou mais passos.
- Erros nesse fluxo causam retrabalho relevante.

## Quando propor uma nova Skill

Propor apenas se houver conhecimento estavel que precisa ser aplicado de forma consistente.

Criterio objetivo:

- Existe regra fixa que nao deve variar por tarefa.
- Essa regra impacta qualidade, custo ou seguranca.
- A regra ja foi repetida manualmente pelo menos 3 vezes.

## Quando propor uma nova Tool

Propor apenas se faltar capacidade real para executar uma etapa critica.

Criterio objetivo:

- A capacidade nao existe nas tools atuais.
- Sem essa capacidade, a entrega fica parcial ou lenta.
- Ha ganho claro de tempo ou reducao de falha.

## Formato de Proposta Obrigatorio

- Problema concreto
- Solucao proposta
- Criterio irrefutavel de ganho
- Risco de nao implementar
- Escopo minimo da implementacao
- Plano em 3 passos

## Gate de Aprovacao

- Etapa 1: proposta curta
- Etapa 2: sua aprovacao
- Etapa 3: implementacao controlada
