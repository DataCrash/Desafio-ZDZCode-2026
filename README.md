# ZDZCode

Implementação full stack do desafio técnico ZDZCode 2026.

## Stack atual

- Backend: ASP.NET Core 10 + EF Core + SQLite
- Frontend: Nuxt 4 + Vue 3
- Board/Operação: GitHub Project + scripts em `scripts/agents` e `scripts/github`

## Pré-requisitos

- .NET SDK 10+
- Node.js 20+
- npm 10+
- GitHub CLI (`gh`) e `jq` para operações de board (opcional para rodar app)

## Estrutura de código

- `src/backend/ZDZCode.Api`: API REST de categorias e produtos
- `src/frontend/app`: interface Nuxt com páginas `/categorias` e `/produtos`
- `scripts/agents`: agentes operacionais (Flow Guard, Board Ops, Delivery Prep)

## Backend - executar localmente

```bash
cd src/backend/ZDZCode.Api
dotnet restore
dotnet run
```

API disponível por padrão em `http://localhost:5286`.

## Frontend - executar localmente

```bash
cd src/frontend/app
npm install
npm run dev
```

Frontend disponível por padrão em `http://localhost:3000`.

Configuração de base da API em `src/frontend/app/nuxt.config.ts`:

- `runtimeConfig.public.apiBase`: `http://localhost:5286`

## Endpoints implementados

### Categorias

- `GET /api/categorias`
- `POST /api/categorias`
- `PUT /api/categorias/{id}`
- `DELETE /api/categorias/{id}`

### Produtos

- `GET /api/produtos`
- `POST /api/produtos`
- `PUT /api/produtos/{id}`
- `DELETE /api/produtos/{id}`

## Payloads de referência

### Criar categoria

```json
{
  "name": "Bebidas Frias",
  "description": "Itens gelados"
}
```

### Criar produto

```json
{
  "name": "Refrigerante Cola 2L",
  "description": "Garrafa pet",
  "price": 12.5,
  "categoryId": 1
}
```

## Regras de domínio já aplicadas

- Nome com mínimo de 5 caracteres (frontend e backend)
- Bloqueio de exclusão de categoria com produtos vinculados (retorno de conflito)
- Atualização reativa da UI após `PUT`/`DELETE` sem refresh forçado

## Verificações rápidas

```bash
# Backend
dotnet build src/backend/ZDZCode.Api/ZDZCode.Api.csproj -nologo

# Frontend
cd src/frontend/app && npm run build
```

## Operação de board (opcional)

```bash
GH_OWNER="DataCrash" GH_REPO="Desafio-ZDZCode-2026" BOARD_TITLE="Board Desafio ZDZCode 2026" ./scripts/agents/board_ops.sh
```

Sincronização de status por label/estado:

```bash
GH_OWNER="DataCrash" GH_REPO="Desafio-ZDZCode-2026" BOARD_TITLE="Board Desafio ZDZCode 2026" ./scripts/github/sync_board_status.sh
```

## Evidências de aceite

- Checklist consolidado do PDF: `docs/quality/pdf-acceptance-checklist.md`
- Resumo executivo final: `docs/quality/final-executive-summary.md`
- Pacote final de submissão: `docs/delivery/submission-package.md`
- Variações de mensagem de submissão: `docs/delivery/submission-message-variants.md`
- Variações de mensagem de submissão (EN): `docs/delivery/submission-message-variants.en.md`
