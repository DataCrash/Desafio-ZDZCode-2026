# Prototipacao Frontend - Modelo A (Neo Clean)

Este pacote registra o conceito visual aprovado para o frontend do desafio.

## Objetivo

Apresentar um visual moderno, limpo e confortavel, com suporte a tema claro/escuro.

Este documento tambem funciona como base inicial da identidade visual do frontend e deve orientar a uniformizacao das telas, componentes e comportamentos.

## Regras de tema

- Tema inicial: segue `prefers-color-scheme` do sistema hospedeiro.
- Alternancia manual: botao discreto no topo (Light/Dark).
- Persistencia: preferencia local salva no navegador.

## Paleta principal

- Dark
  - Background: `#0B1220`
  - Surface: `#111A2B`
  - Text: `#E5E7EB`
  - Accent: `#2DD4BF`
- Light
  - Background: `#F4F7FB`
  - Surface: `#FFFFFF`
  - Text: `#111827`
  - Accent: `#0EA5A4`

## Tipografia

- Titulos: Sora
- Corpo: Manrope

## Comportamento padrao dos componentes

- Header de pagina com titulo, subtitulo curto e area de acoes.
- Formularios com grupos visuais claros, labels consistentes e feedback imediato de erro.
- Tabelas com acoes previsiveis por linha, hierarquia visual dos dados e estados vazios amigaveis.
- Confirmacoes destrutivas sempre explicitas antes de exclusao.
- Estados de loading discretos, mas visiveis.
- Estados de erro com mensagem acionavel e sem ambiguidade tecnica para o usuario final.

## Esbocos e referencia visual

- Os mockups e previews desta pasta sao a base de referencia para consolidar o visual.
- Antes da criacao de novas telas, deve existir um esboco correspondente ou uma extensao coerente desta linguagem visual.
- Novas paginas precisam herdar estrutura, componentes e comportamento do mesmo sistema visual.

## Mockups

- Categorias (Dark): [mockups/categorias-dark.svg](mockups/categorias-dark.svg)
- Categorias (Light): [mockups/categorias-light.svg](mockups/categorias-light.svg)
- Produtos (Dark): [mockups/produtos-dark.svg](mockups/produtos-dark.svg)
- Produtos (Light): [mockups/produtos-light.svg](mockups/produtos-light.svg)

## Previews reais implementados

- Categorias (Dark): [previews/categorias-dark-real.png](previews/categorias-dark-real.png)
- Categorias (Light): [previews/categorias-light-real.png](previews/categorias-light-real.png)
- Produtos (Dark): [previews/produtos-dark-real.png](previews/produtos-dark-real.png)
- Produtos (Light): [previews/produtos-light-real.png](previews/produtos-light-real.png)

Observacao: os previews de produtos podem exibir estado de carregamento quando a API local nao esta ativa. O objetivo aqui e evidenciar layout e tema.

## Mapeamento para implementacao

- Layout e tema global: `src/frontend/app/app.vue`
- Tela de categorias: `src/frontend/app/pages/categorias.vue`
- Tela de produtos: `src/frontend/app/pages/produtos.vue`

## Estados previstos

- Lista vazia com mensagem amigavel
- Loading discreto por tela
- Alerta visual para erros de integridade/operacao
- Acoes de linha em botoes capsula (Edit/Delete)
- Validacao visual consistente em campos obrigatorios e formatos invalidos
- Confirmacao visual consistente para acoes destrutivas
