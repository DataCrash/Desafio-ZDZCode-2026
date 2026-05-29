# TEMP GITHUB SETUP

Preencha primeiro a seção rápida abaixo. Se preferir, depois refine na versão completa.

## Preenchimento Rápido

Nome do repositório:

- Desafio-ZDZCode-2026

Descrição curta:

- Desafio técnico ZDZCode 2026: API .NET + Nuxt 3 para gestão de catálogo (produtos e categorias) com EF Core e banco relacional real.

Visibilidade:

- Público

Nome do board:

- Board Desafio ZDZCode 2026

Objetivo executivo:

- Entregar uma solução full stack aderente ao desafio ZDZCode, demonstrando execução de produção com C#/.NET, EF Core, SQL e Nuxt 3.

Problema que resolve:

- Centraliza e automatiza o cadastro e manutenção de produtos e categorias em uma plataforma web com integridade referencial e operação reativa.
- Reduz inconsistências de dados e elimina retrabalho manual no fluxo de atualização do catálogo.

Ator principal:

- Operador(a) de estoque / Gestor(a) de catálogo.

Escopo MVP:

- CRUD completo de categorias e produtos com API .NET + EF Core.
- Front-end Nuxt 3 com grids, ações de editar/excluir, confirmação de deleção e atualização reativa sem refresh.
- Regra de integridade referencial para impedir exclusão de categoria com produtos vinculados.
- README com setup, migrations e payloads de teste.

Fora de escopo:

- Autenticação/autorização.
- Relatórios e dashboards avançados.
- Integrações externas (ERP, faturamento, webhook, mensageria).
- Multi-tenant e controle de perfis.
- Deploy em cloud e observabilidade avançada.

Topics extras:

- rest-api
- inventory-management
- tech-challenge
- zdzcode

Branch protection em develop:

- Mínima (recomendado para velocidade no desafio).

Branch protection em master:

- Rígida (recomendado para estabilidade da entrega final).

Manter milestones propostas:

- Sim.

## O Que Você Precisa Preencher

### Obrigatório para eu criar tudo depois

- Nome final do repositório
- Descrição curta final
- Visibilidade do repositório
- Nome final do board
- Objetivo executivo em 1 frase
- Problema que o sistema resolve
- Ator principal
- Escopo MVP
- Fora de escopo
- Preferência de branch protection
- Confirmação sobre manter ou renomear milestones

### Opcional

- Topics extras
- Ajustes em labels
- Campos extras do board
- Renomear épico principal

---

## Versão Completa

## 1. Identidade do Repositório

### Nome do repositório

- Nome final:
  - Desafio-ZDZCode-2026

### Alternativas avaliadas

- job-assessment-zdzcode-2026
- zdzcode-tech-challenge-2026
- inventory-catalog-zdzcode-challenge
- (PREENCHER OUTRA OPÇÃO)

### Descrição curta do repositório

- Descrição final:
  - Desafio técnico ZDZCode 2026: API .NET + Nuxt 3 para gestão de catálogo (produtos e categorias) com EF Core e banco relacional real.

### Visibilidade

- Público / Privado:
  - Público

### Topics

- job-assessment
- portfolio
- dotnet
- csharp
- entity-framework
- sql-server
- nuxt3
- vue3
- vuetify
- fullstack
- (PREENCHER TOPICS EXTRAS)

## 2. Metadados do Projeto

### Objetivo executivo

- Entregar uma solução full stack aderente ao desafio ZDZCode, demonstrando execução de produção com C#/.NET, EF Core, SQL e Nuxt 3.

### Problema que o sistema resolve

- Centraliza e automatiza o cadastro e manutenção de produtos e categorias em uma plataforma web com integridade referencial e operação reativa.
- Reduz inconsistências de dados e elimina retrabalho manual no fluxo de atualização do catálogo.

### Público/ator principal

- Operador(a) de estoque / Gestor(a) de catálogo.

### Escopo fechado do MVP

- CRUD completo de categorias e produtos com API .NET + EF Core.
- Front-end Nuxt 3 com grids, ações de editar/excluir, confirmação de deleção e atualização reativa sem refresh.
- Regra de integridade referencial para impedir exclusão de categoria com produtos vinculados.
- README com setup, migrations e payloads de teste.

### Fora de escopo

- Autenticação/autorização.
- Relatórios e dashboards avançados.
- Integrações externas (ERP, faturamento, webhook, mensageria).
- Multi-tenant e controle de perfis.
- Deploy em cloud e observabilidade avançada.

## 3. Estratégia de Versionamento

### Branches principais

- develop
- master

### Fluxo

- feature/\* nasce de develop e volta para develop
- bugfix/\* nasce de develop e volta para develop
- release/\* nasce de develop
- hotfix/\* nasce de master

### Convenção de commit

- Formato:
  - emoji tipo(escopo opcional): descrição curta

### Exemplos aprovados

- ✨ feat(api): criar CRUD de categorias
- 🐛 fix(front): corrigir confirmação de exclusão
- ♻️ refactor(core): simplificar validação de nome
- 📝 docs(readme): documentar setup local
- ✅ test(api): cobrir regra de integridade referencial
- 🔧 chore(repo): ajustar configuração do projeto

## 4. Estrutura do GitHub Project

### Nome do board

- Board Desafio ZDZCode 2026

### Tipo de projeto

- GitHub Project moderno:
  - Sim

### Colunas/status

- Backlog
- Ready
- In Progress
- Review
- Blocked
- Done

### Campos customizados

- Área
- Tipo
- Prioridade
- Fase
- Crítico
- Evidência
- (PREENCHER OUTROS, SE NECESSÁRIO)

## 5. Labels

### Área

- area:backend
- area:frontend
- area:db
- area:docs
- area:qa
- area:devops

### Tipo

- type:feature
- type:bug
- type:chore
- type:docs
- type:test

### Prioridade

- priority:P0
- priority:P1
- priority:P2

### Criticidade do desafio

- critical:eliminatorio
- critical:importante
- critical:diferencial

### Fase

- phase:0-alignment
- phase:1-architecture
- phase:2-backend
- phase:3-frontend
- phase:4-quality
- phase:5-delivery

## 6. Milestones

### Milestone 1

- Nome:
  - Fase 0 - Alinhamento e Escopo
- Objetivo:
  - Congelar escopo, contrato e critérios de pronto.

### Milestone 2

- Nome:
  - Fase 1 - Arquitetura Base
- Objetivo:
  - Definir fundação técnica backend/frontend.

### Milestone 3

- Nome:
  - Fase 2 - Backend
- Objetivo:
  - Entregar API, validações, migrations e CORS.

### Milestone 4

- Nome:
  - Fase 3 - Frontend
- Objetivo:
  - Entregar grid reativo, CRUD e UX exigida.

### Milestone 5

- Nome:
  - Fase 4 - Qualidade e Evidências
- Objetivo:
  - Validar ACs, README e rastreabilidade.

### Milestone 6

- Nome:
  - Fase 5 - Entrega Final
- Objetivo:
  - Empacotar submissão com narrativa forte.

## 7. Backlog Inicial

### Épico principal

- Nome:
  - Entrega do Desafio Técnico ZDZCode 2026
- Descrição:
  - Planejar, implementar e validar uma solução full stack de gestão de catálogo (categorias e produtos), aderente aos critérios do PDF e pronta para submissão.

### Lista de issues iniciais

- Issue 01:
  - Título: Definir escopo fechado do desafio
  - Área: docs
  - Tipo: feature
  - Prioridade: P0
  - Fase: 0
- Issue 02:
  - Título: Estruturar solução backend .NET
  - Área: backend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 1
- Issue 03:
  - Título: Modelar entidades Categoria e Produto
  - Área: db
  - Tipo: feature
  - Prioridade: P0
  - Fase: 2
- Issue 04:
  - Título: Configurar EF Core com banco relacional real
  - Área: backend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 2
- Issue 05:
  - Título: Implementar endpoints de categorias
  - Área: backend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 2
- Issue 06:
  - Título: Implementar endpoints de produtos com Include de categoria
  - Área: backend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 2
- Issue 07:
  - Título: Implementar regra de bloqueio na exclusão de categorias vinculadas
  - Área: backend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 2
- Issue 08:
  - Título: Configurar CORS restritivo para frontend Nuxt
  - Área: backend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 2
- Issue 09:
  - Título: Inicializar aplicação Nuxt 3
  - Área: frontend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 1
- Issue 10:
  - Título: Criar página de categorias com grid e ações
  - Área: frontend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 3
- Issue 11:
  - Título: Criar página de produtos com grid e select de categorias
  - Área: frontend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 3
- Issue 12:
  - Título: Implementar fluxo de edição e exclusão sem refresh
  - Área: frontend
  - Tipo: feature
  - Prioridade: P0
  - Fase: 3
- Issue 13:
  - Título: Implementar validação visual de nome mínimo
  - Área: frontend
  - Tipo: feature
  - Prioridade: P1
  - Fase: 3
- Issue 14:
  - Título: Implementar tratamento visual de erro de integridade
  - Área: frontend
  - Tipo: feature
  - Prioridade: P1
  - Fase: 3
- Issue 15:
  - Título: Documentar setup, migrations e payloads no README
  - Área: docs
  - Tipo: docs
  - Prioridade: P0
  - Fase: 4
- Issue 16:
  - Título: Validar critérios de aceite do PDF
  - Área: qa
  - Tipo: test
  - Prioridade: P0
  - Fase: 4

## 8. Template de Issue

### Template padrão de issue

- Título:
  - (AREA) (FASE) Título objetivo da entrega
- Contexto:
  - Qual parte do desafio este item cobre e qual dor resolve.
- Objetivo:
  - Resultado técnico esperado ao concluir o item.
- Critérios de aceite:
  - Lista verificável do que precisa funcionar para considerar pronto.
- Fora de escopo:
  - Itens explicitamente não cobertos por este card.
- Evidência esperada:
  - Endpoints testados, screenshot, logs ou GIF curto do fluxo.

## 9. Template de Pull Request

### Campos desejados

- Objetivo da mudança
- Critérios de aceite cobertos
- Evidências
- Riscos
- Checklist
- Issue vinculada

## 10. Branch Protection

### develop

- Exigir PR:
  - Sim
- Exigir status checks:
  - Não (mínima)
- Permitir force push:
  - Não
- Permitir delete:
  - Não

### master

- Exigir PR:
  - Sim
- Exigir status checks:
  - Sim (rígida)
- Permitir force push:
  - Não
- Permitir delete:
  - Não

## 11. Regras Operacionais do Board

- WIP máximo:
  - 1 card por vez
- Todo card deve citar:
  - issue
  - fase
  - área
  - critério do PDF impactado
- Todo PR deve conter:
  - evidência objetiva
  - issue vinculada
  - validação executada

## 12. Informações que você precisa preencher

- nome final do repositório
- descrição curta final
- visibilidade pública ou privada
- nome final do board
- objetivo executivo em 1 frase
- problema que o sistema resolve
- ator principal
- escopo MVP em linguagem final
- fora de escopo final
- topics extras, se houver
- se quer branch protection mínima ou mais rígida
- se quer milestones exatamente como proposto ou renomeadas

## 13. Decisão Estratégica Atual

- Conta principal do GitHub:
  - Sim
- Organization separada:
  - Não, por enquanto
- Motivo:
  - Melhor para vincular reputação e portfólio diretamente ao seu perfil.

## 14. Próximo Passo Após Preenchimento

- Criar repositório
- Criar GitHub Project
- Criar labels
- Criar milestones
- Criar issues
- Conectar board
- Organizar backlog inicial
- Iniciar execução por GitFlow
