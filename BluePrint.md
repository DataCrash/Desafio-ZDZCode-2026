# BluePrint - Desafio Tecnico ZDZCode 2026

## 1. Objetivo

Implementar uma solucao full stack aderente ao desafio, com foco em:

- dominio consistente;
- API confiavel;
- frontend reativo;
- frontend com identidade visual consistente e documentada;
- rastreabilidade documental;
- baixo risco de desclassificacao.

## 2. Escopo do Sistema (Aprovado)

O sistema vai contemplar o dominio de catalogo e vendas com as entidades e relacionamentos aprovados na modelagem.

Fonte oficial do modelo aprovado:

- `docs/operations/domain-model-approved.md`

## 3. Fontes Oficiais

- [Vaga](https://odoo.zdzcode.com.br/pt/jobs/desenvolvedor-a-full-stack-100-remoto-8)
- [PDF do desafio](https://odoo.zdzcode.com.br/web/content/1799/desafio-de-código-2026.pdf?download=true)
- `.copilot/memory/notes/ZDZ_VAGA_ANALISE.md`
- `.copilot/memory/notes/ZDZ_DESAFIO_PDF_ANALISE.md`

## 4. Criterios Criticos (Eliminatorios)

- Nao usar EF Core InMemory.
- Nao usar AllowAnyOrigin no CORS.
- Nao usar refresh forcado para sincronizacao de estado.
- Respeitar contrato dos endpoints do desafio.
- Bloquear exclusao de categoria com produtos vinculados.
- Validar nome com minimo de 5 caracteres no backend e frontend.

## 5. Entidades e Relacionamentos Aprovados

### 5.1 Entidades

1. CATEGORIA
2. PRODUTO
3. CLIENTE
4. ENDERECO_ENTREGA
5. PEDIDO
6. ITEM_PEDIDO
7. PAGAMENTO
8. MOVIMENTACAO_ESTOQUE
9. TAG
10. PRODUTO_TAG

### 5.2 Relacionamentos

- 1:0..1 CLIENTE -> ENDERECO_ENTREGA
- 1:1 PEDIDO -> PAGAMENTO
- 1:N CATEGORIA -> PRODUTO
- 1:N CLIENTE -> PEDIDO
- 1:N PEDIDO -> ITEM_PEDIDO
- 1:N PRODUTO -> ITEM_PEDIDO
- 1:N PRODUTO -> MOVIMENTACAO_ESTOQUE
- N:M PRODUTO <-> TAG (via PRODUTO_TAG)

### 5.3 Regras de Integridade Prioritarias

- Nao permitir exclusao de CATEGORIA com PRODUTO vinculado.
- Nao permitir ITEM_PEDIDO sem PEDIDO e sem PRODUTO validos.
- Nao permitir PAGAMENTO sem PEDIDO valido (1:1).
- Nao permitir PRODUTO_TAG duplicado para o mesmo par produto/tag.
- Nao permitir estoque negativo em operacoes de saida.

## 6. Contrato Minimo de API (Compatibilidade com o Desafio)

### Categorias

- GET /api/categorias
- POST /api/categorias
- PUT /api/categorias/{id}
- DELETE /api/categorias/{id}

### Produtos

- GET /api/produtos
- POST /api/produtos
- PUT /api/produtos/{id}
- DELETE /api/produtos/{id}

## 7. Contrato Minimo de Frontend (Compatibilidade com o Desafio)

- Rotas principais:
  - /categorias
  - /produtos
- UI/UX guiada por identidade visual documentada.
- Esboco de referencia obrigatorio antes da expansao de telas.
- Componentes e telas com comportamento uniforme documentado.
- Grid com coluna Acoes.
- Editar e excluir por linha.
- Confirmacao obrigatoria antes de DELETE.
- Botao salvar desabilitado quando nome invalido.
- Atualizacao reativa sem refresh da pagina.
- Tratamento visual para erro de integridade (409).
- Select de categoria carregado via API em produtos.

## 8. Planejamento de Execucao Detalhado

Fonte oficial detalhada de execucao:

- `docs/operations/execution-plan-complementar.md`

### Fase 0 - Modelagem e Governanca (Concluida)

Objetivo:

- Consolidar o modelo aprovado e as regras de processo.

Entregas:

- Modelo de entidades e relacionamentos aprovado.
- Regra de aprovacao antes de implementacao registrada.

Criterio de pronto:

- Documentacao permanente sem dependencia de pasta temporaria.

### Fase 0.1 - Identidade Visual e Esbocos de UX

Objetivo:

- Definir a base visual do frontend e registrar o comportamento esperado dos componentes e telas.

Atividades tecnicas:

- Consolidar identidade visual oficial.
- Produzir esbocos das telas prioritarias.
- Documentar componentes-base, estados de interface e regras de comportamento.
- Uniformizar a experiencia entre catalogo e fluxos transacionais.

Saidas esperadas:

- Guia visual permanente.
- Esbocos versionados.
- Padrao reutilizavel para implementacao frontend.

Criterio de pronto:

- Nenhuma nova tela frontend segue sem referencia visual e comportamental documentada.

### Fase 1 - Fundacao Backend

Objetivo:

- Preparar base de dominio e persistencia para todas as entidades aprovadas.

Atividades tecnicas:

- Criar/ajustar entidades de dominio.
- Configurar mapeamentos, FKs e indices.
- Configurar migrations iniciais da fase complementar.
- Revisar CORS e configuracoes de ambiente.

Saidas esperadas:

- Compilacao limpa do backend.
- Migration consistente aplicada localmente.

Criterio de pronto:

- Estrutura de dados integra e versionada.

### Fase 2 - API de Catalogo e Cadastros

Objetivo:

- Implementar API completa para CATEGORIA, PRODUTO, CLIENTE e TAG.

Atividades tecnicas:

- CRUDs com contratos request/response.
- Validacoes de dominio e retorno HTTP consistente.
- Paginacao/filtros simples quando necessario.

Saidas esperadas:

- Endpoints testados via `.http`.
- Erros de validacao padronizados.

Criterio de pronto:

- Casos principais e de erro cobertos nos recursos base.

### Fase 3 - API Transacional

Objetivo:

- Implementar fluxo de PEDIDO, ITEM_PEDIDO, PAGAMENTO e MOVIMENTACAO_ESTOQUE.

Atividades tecnicas:

- Criar endpoint de abertura e fechamento de pedido.
- Calcular subtotal, desconto e total no backend.
- Registrar movimentacao de estoque por operacao.
- Implementar regras de consistencia transacional.

Saidas esperadas:

- Fluxo completo pedido -> pagamento funcionando.
- Estoque atualizado de forma auditavel.

Criterio de pronto:

- Integridade transacional validada em cenarios de sucesso/erro.

### Fase 4 - Frontend de Catalogo

Objetivo:

- Garantir UX completa e reativa para CATEGORIA e PRODUTO.

Atividades tecnicas:

- Refinar telas existentes.
- Padronizar mensagens de erro e estados de carregamento.
- Garantir acessibilidade basica e responsividade.

Saidas esperadas:

- CRUDs de catalogo estaveis no frontend.

Criterio de pronto:

- Sem refresh forcado; feedback claro ao usuario.

### Fase 5 - Frontend Transacional

Objetivo:

- Entregar jornadas de CLIENTE, PEDIDO e PAGAMENTO.

Atividades tecnicas:

- Tela de clientes (cadastro/listagem/edicao).
- Tela de pedidos com itens e totalizacao.
- Tela de pagamento e status.
- Integracao com atualizacao de estoque e indicadores basicos.

Saidas esperadas:

- Fluxo operacional completo no frontend.

Criterio de pronto:

- Jornada ponta a ponta concluida sem ajustes manuais.

### Fase 6 - Qualidade, Evidencias e Entrega

Objetivo:

- Fechar qualidade tecnica e documentacao para submissao.

Atividades tecnicas:

- Rodar lint/build/testes disponiveis.
- Atualizar README, checklist e overview.
- Validar ambiente limpo (cold start).

Saidas esperadas:

- Pacote de entrega coeso e auditavel.

Criterio de pronto:

- Checklist final sem pendencias criticas.

## 9. Checklist Macro de Pronto

### Modelagem

- [x] Entidades e relacionamentos aprovados e documentados.
- [x] Sem referencia a artefatos temporarios no BluePrint.

### Backend

- [ ] Entidades e mapeamentos implementados conforme modelo aprovado.
- [ ] Migrations aplicadas com sucesso.
- [ ] Validacoes e integridade implementadas.
- [ ] Endpoints principais testados.

### Frontend

- [ ] Identidade visual documentada e versionada.
- [ ] Esbocos de referencia produzidos para telas prioritarias.
- [ ] Guia de comportamento de componentes registrado.
- [ ] CRUD de catalogo estavel.
- [ ] Jornadas transacionais implementadas.
- [ ] Tratamento de erro e estados de UI padronizados.

### Qualidade e Entrega

- [ ] Build/lint/testes sem falhas bloqueadoras.
- [ ] Documentacao final consistente.
- [ ] Validacao de ambiente limpo concluida.

## 10. Modo de Execucao

- Commits curtos, atomicos e semanticos.
- Fluxo forward-only (sem rewrite de historico).
- Implementacao faseada com validacao por etapa.
- Sem desvio de escopo sem aprovacao explicita.
