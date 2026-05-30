# Changelog Visual do Frontend (Neo Clean)

Este documento resume a evolucao visual do frontend durante a entrega do desafio.

## Objetivo

Registrar, de forma cronologica e auditavel, as mudancas de UI/UX aplicadas no frontend.

## Linha do tempo

1. `7353167` - feat(frontend): aplica tema Neo Clean com dark/light e prototipos

- Novo visual Neo Clean nas telas de categorias e produtos.
- Tema automatico por `prefers-color-scheme`.
- Botao discreto para alternancia de tema.
- Prototipos e previews adicionados em `docs/prototipos/neo-clean/`.

2. `138eee8` - feat(frontend): refinamento visual e acessibilidade do tema Neo Clean

- Melhorias de foco visivel para navegacao e botoes.
- Ajustes de hover e microinteracoes para conforto visual.
- Polimento de densidade e legibilidade de tabelas.
- Respeito a `prefers-reduced-motion`.

3. `820dbef` - chore(frontend): padroniza microcopy de erros no CRUD

- Mensagens de erro harmonizadas para consistencia de linguagem na UI.

4. `939a661` - chore(frontend): incorpora ajuste pendente em produtos

- Incorporacao de ajuste complementar na tela de produtos.

## Artefatos relacionados

- Guia de prototipacao: `docs/prototipos/neo-clean/README.md`
- Mockups SVG: `docs/prototipos/neo-clean/mockups/`
- Previews reais PNG: `docs/prototipos/neo-clean/previews/`
- Layout global: `src/frontend/app/app/app.vue`
- Categorias: `src/frontend/app/app/pages/categorias.vue`
- Produtos: `src/frontend/app/app/pages/produtos.vue`
