# Checklist de Aceite - Desafio ZDZCode 2026

## Contexto

Checklist consolidado com base no BluePrint, contrato de API e implementacao atual do repositorio.

## Critérios eliminatórios

- [x] Nao usa EF Core InMemory
  - Evidencia: `UseSqlite` em `src/backend/ZDZCode.Api/Program.cs`.
- [x] CORS restritivo (sem AllowAnyOrigin)
  - Evidencia: politica `WithOrigins(allowedOrigins)` em `src/backend/ZDZCode.Api/Program.cs`.
- [x] Sem refresh forcado para sincronizacao de estado
  - Evidencia: atualizacao local com `map`/`filter` nas paginas de categorias e produtos.
- [x] Contrato de endpoints respeitado
  - Evidencia: controllers `CategoriasController` e `ProdutosController`.
- [x] Bloqueio de exclusao de categoria com produtos vinculados
  - Evidencia: retorno `Conflict` em `src/backend/ZDZCode.Api/Controllers/CategoriasController.cs`.
- [x] Nome com minimo de 5 caracteres no back e no front
  - Evidencia back: `[MinLength(5)]` nos contratos.
  - Evidencia front: `isCreateValid` e `isEditValid` em categorias/produtos.

## Contrato funcional

- [x] CRUD de categorias completo
- [x] CRUD de produtos completo
- [x] GET de produtos com categoria vinculada
- [x] Select de categoria carregado via API em produtos
- [x] Coluna de acoes no grid (editar/excluir)
- [x] Confirmacao antes de excluir
- [x] Tratamento visual para erro de integridade (409)

## Evidências de documentação

- [x] Setup local documentado
- [x] Endpoints e payloads de referencia documentados
- [x] Arquivo HTTP de testes atualizado

## Risco residual

- [ ] Validacao formal por roteiro manual fim-a-fim em ambiente limpo (cold start)
  - Observacao: recomendado para fechamento final de entrega.

## Conclusão

Status geral: **Aderente aos criterios principais do desafio**, com risco residual baixo concentrado na validacao manual fim-a-fim.
