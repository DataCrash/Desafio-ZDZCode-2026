# Análise do PDF do Desafio Técnico ZDZCode

## Fonte

- [PDF Desafio Técnico 2026](https://odoo.zdzcode.com.br/web/content/1799/desafio-de-código-2026.pdf?download=true)

## Contexto do Documento

- Epic: Modernização e Escalabilidade da Plataforma de Inventário Comercial
- Feature: Módulo de Gestão de Catálogo (Produtos e Categorias)
- Abordagem: desafio full stack ponta a ponta com API .NET + Nuxt 3

## Métricas e Criticidade

- API CRUD com resposta inferior a 200ms por requisição
- Integridade referencial rigorosa (evitar órfãos)
- Front-end reativo sem refresh forçado de página

## User Story Back-end (Pontos Obrigatórios)

- Persistência com banco relacional permanente (SQL Server, PostgreSQL ou MySQL)
- Proibido EF InMemory ou simulação estática
- Modelagem:
  - Categoria(Id, Nome, Descrição)
  - Produto(Id, Nome, Descrição, Preço, CategoriaId)
  - Relação 1:N com FK explícita
- Endpoints exigidos:
  - GET /api/categorias
  - POST /api/categorias
  - PUT /api/categorias/{id}
  - DELETE /api/categorias/{id}
  - GET /api/produtos (com .Include() da categoria)
  - POST /api/produtos
  - PUT /api/produtos/{id}
  - DELETE /api/produtos/{id}
- Regra de negócio crítica:
  - Bloquear exclusão de categoria com produtos vinculados
  - Retornar 409 Conflict ou 400 Bad Request
  - Mensagem textual esperada:
    "Não é possível excluir uma categoria que possua produtos vinculados."
- Validação:
  - Nome nulo ou com menos de 5 caracteres deve retornar 400 com sumário de erros
- CORS:
  - Permitir apenas URL/porta oficial do frontend Nuxt 3
  - Proibido AllowAnyOrigin()

## User Story Front-end (Pontos Obrigatórios)

- Composition API (ref/reactive)
- Consumo API com Axios ou cliente nativo do Nuxt
- Grid de categorias e produtos com coluna final "Ações"
  - Botões por linha: Editar e Excluir
- Edição via modal/dialog ou inline, com PUT
- Botão Salvar desabilitado quando Nome tiver menos de 5 caracteres
- Confirmação obrigatória para DELETE
- Atualização reativa local após PUT/DELETE, sem reload
- Tratamento visual de erro de integridade referencial com mensagem amigável
- Select de categoria no formulário de produto alimentado assincronamente por GET /api/categorias

## Tasks Técnicas Declaradas no PDF

- Task 01: inicializar Web API .NET e modelos + appsettings
- Task 02: DbContext + relação 1:N + migrations
- Task 03: CRUD + validações + regra de exclusão
- Task 04: inicializar Nuxt 3 + rotas /categorias e /produtos
- Task 05: formulário + grid consumindo API
- Task 06: editar/excluir via modal/confirmação
- Task 07: README com setup, migrations e payloads de teste

## Riscos de Desclassificação

- Uso de EF InMemory
- Uso de AllowAnyOrigin em CORS
- Falhar em bloquear exclusão de categoria vinculada
- Usar refresh de página para sincronizar estado
- Ignorar validação de Nome >= 5 caracteres
- Entregar rotas fora do contrato exigido

## Implicações para Planejamento

- O desafio não é genérico: há contrato técnico explícito e mensurável
- Escopo ideal deve priorizar aderência estrita aos ACs
- Qualidade de execução importa mais que complexidade extra
- O planejamento deve iniciar por modelagem + contrato API + UX reativa sem refresh
