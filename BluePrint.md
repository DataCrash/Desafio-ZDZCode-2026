# BluePrint - Desafio Técnico ZDZCode 2026

## Objetivo

Entregar uma solução full stack aderente ao PDF do desafio, com foco em execução pragmática, código de produção e risco mínimo de desclassificação.

## Escopo Fechado

- Domínio: catálogo de inventário (Produtos e Categorias).
- Back-end: API .NET com EF Core e banco relacional real.
- Front-end: Nuxt 3 com grid reativo e operações CRUD.
- Documentação: README reproduzível com setup, migrations e payloads.

## Fora de Escopo

- Funcionalidades não exigidas no PDF.
- Otimizações prematuras sem impacto no aceite.
- Arquitetura excessivamente complexa para o desafio.

## Fontes Oficiais

- [Vaga](https://odoo.zdzcode.com.br/pt/jobs/desenvolvedor-a-full-stack-100-remoto-8)
- [PDF do desafio](https://odoo.zdzcode.com.br/web/content/1799/desafio-de-código-2026.pdf?download=true)
- [Análise da vaga](.copilot/memory/notes/ZDZ_VAGA_ANALISE.md)
- [Análise do PDF](.copilot/memory/notes/ZDZ_DESAFIO_PDF_ANALISE.md)

## Critérios Críticos (Eliminatórios)

- Não usar EF Core InMemory.
- Não usar AllowAnyOrigin no CORS.
- Não usar refresh forçado da página para sincronizar estado.
- Respeitar contrato de endpoints exigido.
- Bloquear exclusão de categoria com produtos vinculados.
- Validar nome com mínimo de 5 caracteres no back e no front.

## Contrato Mínimo de Dados

- Categoria:
  - Id
  - Nome
  - Descrição
- Produto:
  - Id
  - Nome
  - Descrição
  - Preço
  - CategoriaId
- Relacionamento:
  - Categoria 1:N Produto

## Contrato Mínimo de API

- Categorias:
  - GET /api/categorias
  - POST /api/categorias
  - PUT /api/categorias/{id}
  - DELETE /api/categorias/{id}
- Produtos:
  - GET /api/produtos (com categoria vinculada)
  - POST /api/produtos
  - PUT /api/produtos/{id}
  - DELETE /api/produtos/{id}

## Regra de Integridade Referencial

Ao excluir categoria com produtos vinculados, retornar 409 ou 400 com mensagem:

"Não é possível excluir uma categoria que possua produtos vinculados."

## Contrato Mínimo de Front-end

- Páginas:
  - /categorias
  - /produtos
- Grid com coluna final "Ações".
- Botões por linha: Editar e Excluir.
- Edição via modal/dialog ou inline.
- Confirmação obrigatória antes de DELETE.
- Botão Salvar desabilitado enquanto Nome < 5.
- Atualização reativa local após PUT/DELETE sem reload.
- Tratamento visual de erro de integridade referencial.
- Select de categoria carregado via GET /api/categorias.

## Fases de Implementação

### Fase 0 - Alinhamento

- Congelar escopo e contrato técnico do PDF.
- Definir critério de pronto por fase.

### Fase 1 - Arquitetura Base

- Estruturar backend (.NET + EF Core + DbContext + entities).
- Estruturar frontend (Nuxt 3 + rotas + layout base).
- Definir payloads de entrada e saída.

### Fase 2 - Backend

- Implementar modelos e migrations.
- Implementar CRUD de categorias e produtos.
- Implementar validações de Nome >= 5.
- Implementar bloqueio de exclusão com vínculo.
- Configurar CORS restritivo para origem do front.

### Fase 3 - Frontend

- Implementar grids de categorias e produtos.
- Implementar cadastro, edição e exclusão.
- Implementar fluxo de confirmação e erros.
- Garantir sincronização reativa sem reload.

### Fase 4 - Qualidade e Evidências

- Revisar aderência aos ACs do PDF.
- Revisar performance básica local das operações.
- Organizar evidências visuais de funcionamento.

### Fase 5 - Entrega

- Finalizar README com setup completo.
- Revisar repositório, commits e clareza técnica.
- Preparar mensagem de submissão objetiva.

## Checklist de Pronto

- [ ] Banco relacional real configurado e migrations aplicadas.
- [ ] Todos os endpoints exigidos implementados e testados.
- [ ] Validação Nome >= 5 funcionando no back e no front.
- [ ] Regra de exclusão com vínculo funcionando com resposta correta.
- [ ] CORS restritivo configurado sem AllowAnyOrigin.
- [ ] Grid com Ações, editar, excluir e confirmação.
- [ ] UI reativa sem refresh após PUT/DELETE.
- [ ] Select de categoria em produtos com carga assíncrona.
- [ ] README reproduzível validado em ambiente limpo.

## Riscos e Mitigações

- Risco: desvio de escopo.
  - Mitigação: seguir escopo fechado e task brief por etapa.
- Risco: falha em critério eliminatório.
  - Mitigação: validar critérios críticos ao fim de cada fase.
- Risco: complexidade excessiva.
  - Mitigação: priorizar aderência ao contrato antes de extras.

## Modo de Execução

- Commits curtos, atômicos e semânticos.
- Sem alterar nomes canônicos sem ordem explícita.
- Sem tarefas fora de escopo.
- Resumo curto por padrão, detalhe sob demanda.
