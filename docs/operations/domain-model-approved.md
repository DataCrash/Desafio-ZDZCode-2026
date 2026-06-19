# Modelo Aprovado - Entidades e Relacionamentos

Status: aprovado em 2026-06-18.

Este documento e a fonte oficial e permanente do modelo de dados que o sistema vai contemplar.

## 1. Entidades e Propriedades

### 1.1 CATEGORIA

| Campo     | Tipo   | Regra        |
| --------- | ------ | ------------ |
| id        | int    | PK           |
| nome      | string | obrigatorio  |
| descricao | string | opcional     |
| ativo     | bool   | default true |

### 1.2 PRODUTO

| Campo         | Tipo    | Regra           |
| ------------- | ------- | --------------- |
| id            | int     | PK              |
| categoria_id  | int     | FK CATEGORIA.id |
| nome          | string  | obrigatorio     |
| descricao     | string  | opcional        |
| sku           | string  | unico, opcional |
| preco         | decimal | obrigatorio     |
| estoque_atual | int     | >= 0            |
| ativo         | bool    | default true    |

### 1.3 CLIENTE

| Campo     | Tipo     | Regra              |
| --------- | -------- | ------------------ |
| id        | int      | PK                 |
| nome      | string   | obrigatorio        |
| email     | string   | unico, obrigatorio |
| telefone  | string   | opcional           |
| ativo     | bool     | default true       |
| criado_em | datetime | obrigatorio        |

### 1.4 ENDERECO_ENTREGA

| Campo       | Tipo   | Regra             |
| ----------- | ------ | ----------------- |
| id          | int    | PK                |
| cliente_id  | int    | FK CLIENTE.id, UK |
| logradouro  | string | obrigatorio       |
| numero      | string | obrigatorio       |
| bairro      | string | opcional          |
| cidade      | string | obrigatorio       |
| uf          | string | obrigatorio       |
| cep         | string | obrigatorio       |
| complemento | string | opcional          |

### 1.5 PEDIDO

| Campo          | Tipo     | Regra         |
| -------------- | -------- | ------------- |
| id             | int      | PK            |
| cliente_id     | int      | FK CLIENTE.id |
| status         | string   | obrigatorio   |
| subtotal       | decimal  | obrigatorio   |
| desconto_total | decimal  | obrigatorio   |
| total          | decimal  | obrigatorio   |
| criado_em      | datetime | obrigatorio   |
| atualizado_em  | datetime | opcional      |
| observacao     | string   | opcional      |

### 1.6 ITEM_PEDIDO

| Campo          | Tipo    | Regra         |
| -------------- | ------- | ------------- |
| id             | int     | PK            |
| pedido_id      | int     | FK PEDIDO.id  |
| produto_id     | int     | FK PRODUTO.id |
| quantidade     | int     | > 0           |
| preco_unitario | decimal | obrigatorio   |
| total_linha    | decimal | obrigatorio   |

### 1.7 PAGAMENTO

| Campo                | Tipo     | Regra            |
| -------------------- | -------- | ---------------- |
| id                   | int      | PK               |
| pedido_id            | int      | FK PEDIDO.id, UK |
| metodo               | string   | obrigatorio      |
| status               | string   | obrigatorio      |
| valor                | decimal  | obrigatorio      |
| referencia_transacao | string   | opcional         |
| pago_em              | datetime | opcional         |

### 1.8 MOVIMENTACAO_ESTOQUE

| Campo      | Tipo     | Regra         |
| ---------- | -------- | ------------- |
| id         | int      | PK            |
| produto_id | int      | FK PRODUTO.id |
| tipo       | string   | IN/OUT        |
| quantidade | int      | > 0           |
| motivo     | string   | opcional      |
| criado_em  | datetime | obrigatorio   |

### 1.9 TAG

| Campo | Tipo   | Regra              |
| ----- | ------ | ------------------ |
| id    | int    | PK                 |
| nome  | string | unico, obrigatorio |

### 1.10 PRODUTO_TAG

| Campo        | Tipo     | Regra             |
| ------------ | -------- | ----------------- |
| produto_id   | int      | PK, FK PRODUTO.id |
| tag_id       | int      | PK, FK TAG.id     |
| vinculado_em | datetime | obrigatorio       |

## 2. Relacionamentos

| Tipo   | Relacao                         |
| ------ | ------------------------------- |
| 1:0..1 | CLIENTE -> ENDERECO_ENTREGA     |
| 1:1    | PEDIDO -> PAGAMENTO             |
| 1:N    | CATEGORIA -> PRODUTO            |
| 1:N    | CLIENTE -> PEDIDO               |
| 1:N    | PEDIDO -> ITEM_PEDIDO           |
| 1:N    | PRODUTO -> ITEM_PEDIDO          |
| 1:N    | PRODUTO -> MOVIMENTACAO_ESTOQUE |
| N:M    | PRODUTO <-> TAG via PRODUTO_TAG |

## 3. Regras Chave para Implementacao

- Excluir categoria com produto vinculado deve retornar erro de integridade.
- Nao permitir estoque negativo em saidas.
- Pedido calcula valores no backend (subtotal, descontos, total).
- Pagamento deve manter 1:1 com pedido.
- Relacao N:M de produto/tag deve impedir duplicidade.
