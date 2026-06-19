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

- `src/backend/ZDZCode.Api`: API REST de categorias, produtos, clientes, tags e pedidos
- `src/frontend/app`: interface Nuxt com páginas `/categorias` e `/produtos`
- `scripts/agents`: agentes operacionais (Flow Guard, Board Ops, Delivery Prep)

## Modelo de dominio aprovado (permanente)

O modelo oficial de entidades e relacionamentos contemplado pelo sistema esta documentado em:

- `docs/operations/domain-model-approved.md`

Planejamento detalhado de implementacao da fase complementar:

- `docs/operations/execution-plan-complementar.md`

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

### Clientes

- `GET /api/clientes`
- `GET /api/clientes/{id}`
- `POST /api/clientes`
- `PUT /api/clientes/{id}`
- `DELETE /api/clientes/{id}`

### Tags

- `GET /api/tags`
- `POST /api/tags`
- `PUT /api/tags/{id}`
- `DELETE /api/tags/{id}`
- `POST /api/tags/{tagId}/produtos/{productId}`
- `DELETE /api/tags/{tagId}/produtos/{productId}`

### Pedidos

- `GET /api/pedidos`
- `GET /api/pedidos/{id}`
- `POST /api/pedidos`
- `PUT /api/pedidos/{id}`
- `DELETE /api/pedidos/{id}`
- `POST /api/pedidos/{id}/itens`
- `PUT /api/pedidos/{id}/itens/{itemId}`
- `DELETE /api/pedidos/{id}/itens/{itemId}`

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

### Criar cliente

```json
{
  "name": "Cliente Exemplo",
  "email": "cliente@example.com",
  "phone": "11999999999",
  "isActive": true,
  "deliveryAddress": {
    "street": "Rua das Flores",
    "number": "100",
    "district": "Centro",
    "city": "Sao Paulo",
    "state": "SP",
    "zipCode": "01000-000",
    "complement": "Apto 12"
  }
}
```

### Criar pedido

```json
{
  "customerId": 1,
  "status": "draft",
  "discountTotal": 0,
  "note": "Pedido inicial"
}
```

### Adicionar item no pedido

```json
{
  "productId": 1,
  "quantity": 2
}
```

## Regras de domínio já aplicadas

- Nome com mínimo de 5 caracteres (frontend e backend)
- Bloqueio de exclusão de categoria com produtos vinculados (retorno de conflito)
- Atualização reativa da UI após `PUT`/`DELETE` sem refresh forçado
- Unicidade de e-mail para clientes (retorno de conflito)
- Relação N:M entre produto e tag com bloqueio de vínculo duplicado
- Totalização de pedidos com itens e desconto no backend

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
- Fechamento final (1 pagina): `docs/delivery/final-delivery-one-page.md`
- Pacote final de submissão: `docs/delivery/submission-package.md`
- Variações de mensagem de submissão: `docs/delivery/submission-message-variants.md`
- Variações de mensagem de submissão (EN): `docs/delivery/submission-message-variants.en.md`
- Changelog visual do frontend: `docs/delivery/frontend-visual-changelog.md`
- Submissão consolidada na raiz: `Submissão.md`
- Overview final com evidências visuais: `Overview-Desafio/Overview.md`
- Capturas de evidência (board/cards/sistema/API): `Overview-Desafio/prints/`
- Prototipação e previews do frontend: `docs/prototipos/neo-clean/`
