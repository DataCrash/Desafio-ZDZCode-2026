# Planejamento de Execucao Detalhado - Fase Complementar

## Objetivo

Executar a implementacao completa do modelo aprovado de entidades e relacionamentos em etapas pequenas, validaveis e com baixo risco operacional.

## Premissas

- Fluxo de aprovacao: modelagem aprovada antes da implementacao.
- Politica de historico: forward-only.
- Commits atomicos por unidade de entrega.
- Validacao tecnica ao fim de cada fase.
- Frontend deve obedecer a uma identidade visual documentada antes da expansao das telas.
- Toda nova tela deve seguir um esboco aprovado, um padrao de comportamento e um guia visual permanente.

## Fase 0.1 - Fundacao de UX e Identidade Visual

### Objetivo

Definir a base visual e comportamental que vai guiar todos os componentes e telas do frontend.

### Escopo tecnico

- Consolidar a identidade visual oficial do produto.
- Definir tokens visuais: cores, tipografia, espacamentos, raios, sombras e estados.
- Definir principios de UX para formularios, tabelas, acoes, feedback, loading e erro.
- Definir o comportamento padrao dos componentes reutilizaveis.
- Registrar a navegacao base entre as telas principais e auxiliares.

### Artefatos obrigatorios

- Documento de identidade visual.
- Esbocos de baixa/media fidelidade das telas prioritarias.
- Guia de comportamento de componentes e estados.
- Prototipo visual de referencia em documentacao permanente.

### Criterios de pronto

- Identidade visual documentada e rastreavel.
- Esboco aprovado para catalogo e fluxo transacional.
- Regras de comportamento uniformes para componentes e telas.

### Riscos e mitigacao

- Risco: frontend evoluir com inconsistencia visual entre telas.
- Mitigacao: bloquear implementacao de novas paginas sem artefato visual e comportamento documentado.

## Fase 1 - Fundacao Backend

### Objetivo

Estruturar dominio, persistencia e integridade para suportar o modelo aprovado.

### Escopo tecnico

- Criar/ajustar entidades:
  - Category, Product, Customer, DeliveryAddress, Order, OrderItem, Payment, StockMovement, Tag, ProductTag.
- Atualizar DbContext com DbSets e Fluent API.
- Definir FKs, UQs e indices necessarios.
- Gerar migration da fase complementar.

### Criterios de pronto

- `dotnet build` sem erro.
- Migration gerada e aplicada localmente.
- Estrutura do banco refletindo o modelo aprovado.

### Riscos e mitigacao

- Risco: quebra de mapeamento.
- Mitigacao: implementar por entidade e validar migration incremental.

## Fase 2 - API de Catalogo e Cadastros

### Objetivo

Disponibilizar CRUD robusto para recursos-base do sistema.

### Escopo tecnico

- Endpoints para:
  - categorias
  - produtos
  - clientes
  - tags
- Contratos DTO de request/response por recurso.
- Validacoes de regra de negocio.
- Padronizacao de erros HTTP para validacao e conflito.

### Criterios de pronto

- Arquivo `.http` cobrindo CRUD dos 4 recursos.
- Cenarios de erro validados (400/404/409).
- Integracao produto-categoria funcionando.

### Riscos e mitigacao

- Risco: divergencia de contrato entre front e back.
- Mitigacao: versionar DTOs e validar payloads com exemplos no README.

## Fase 3 - API Transacional

### Objetivo

Implementar processos de pedido, pagamento e estoque com consistencia.

### Escopo tecnico

- Endpoints para:
  - pedidos
  - itens de pedido
  - pagamentos
  - movimentacoes de estoque
  - vinculo produto-tag (N:M)
- Regras transacionais:
  - fechamento de pedido com totalizacao;
  - atualizacao de estoque em saidas;
  - bloqueio de inconsistencias de pagamento.

### Criterios de pronto

- Fluxo pedido -> item -> pagamento validado ponta a ponta no backend.
- Estoque atualizado corretamente conforme regras.
- Logs suficientes para troubleshooting.

### Riscos e mitigacao

- Risco: inconsistencias em operacoes concorrentes.
- Mitigacao: transacoes no EF Core e validacoes de pre-condicao.

## Fase 4 - Frontend de Catalogo

### Objetivo

Entregar UX estavel para entidades base (categoria/produto).

### Escopo tecnico

- Aplicar a identidade visual aprovada no app shell e nas telas de catalogo.
- Revisar telas atuais de categorias/produtos.
- Padronizar estados: loading, empty, error, success.
- Padronizar componentes-base: page header, form section, data table, inline actions, badges e alerts.
- Reforcar validacoes no formulario.
- Melhorar feedback de conflito de integridade.
- Garantir consistencia textual, espacial e comportamental com o guia visual.

### Criterios de pronto

- CRUD de catalogo funcional sem refresh forcado.
- Erros de validacao e conflito com mensagens claras.
- Componentes reutilizados com aparencia e comportamento uniformes.
- Build do frontend sem erro.

### Riscos e mitigacao

- Risco: regressao visual em refatoracoes.
- Mitigacao: checklist visual por tela e tema.

## Fase 5 - Frontend Transacional

### Objetivo

Implementar jornadas completas para cliente, pedido e pagamento.

### Escopo tecnico

- Aplicar o mesmo sistema visual e comportamental nas telas transacionais.
- Tela de clientes (CRUD).
- Tela de pedidos:
  - cabecalho do pedido;
  - adicao/edicao de itens;
  - totalizacao visual alinhada ao backend.
- Tela de pagamento e status.
- Tela basica de movimentacao de estoque e historico.
- Vinculacao de tags a produtos.
- Garantir consistencia entre telas simples, tabelas, formularios longos e estados vazios.

### Criterios de pronto

- Jornada completa de venda validada na UI.
- Consistencia entre dados exibidos e dados persistidos.
- Navegacao funcional entre recursos relacionados.
- Linguagem visual uniforme com o catalogo e com o app shell.

### Riscos e mitigacao

- Risco: acoplamento excessivo de estado.
- Mitigacao: composables por dominio e estados isolados por pagina.

## Fase 6 - Qualidade, Evidencias e Entrega

### Objetivo

Concluir validacoes tecnicas e documentais para entrega segura.

### Escopo tecnico

- Rodar typecheck/build/testes disponiveis.
- Atualizar README, BluePrint e checklist de aceite.
- Validar ambiente limpo (cold start) com roteiro reproduzivel.
- Consolidar evidencias em docs e overview.

### Criterios de pronto

- Sem erros bloqueadores de build/typecheck.
- Documentacao coerente com o sistema implementado.
- Evidencias de execucao e aceite registradas.

### Riscos e mitigacao

- Risco: divergencia entre codigo e documentacao final.
- Mitigacao: revisao cruzada final por arquivo de evidencias.

## Ordem Recomendada de Implementacao por PR

1. PR-01: Fundacao UX, identidade visual, esbocos e documentacao comportamental.
2. PR-02: Fundacao backend (entidades, mapeamento, migration).
3. PR-03: API catalogo e cadastros.
4. PR-04: API transacional.
5. PR-05: Frontend catalogo (ajustes e robustez).
6. PR-06: Frontend transacional.
7. PR-07: Qualidade, docs e fechamento.

## Definicao de Pronto (DoD)

Uma fase so pode ser encerrada quando:

- codigo compila e roda localmente;
- validacoes criticas estao cobrindo sucesso/erro;
- documentacao da fase esta atualizada;
- impacto em fases seguintes esta identificado.
