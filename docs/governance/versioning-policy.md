# Política de Versionamento

## Princípios

- Forward-only: proibido reescrever histórico (sem reset/rebase destrutivo), salvo ordem explícita.
- Nunca commitar código quebrado.
- Commits curtos, atômicos e semânticos.

## Codificação de Texto

- Todo texto do projeto deve ser codificado em UTF-8 (sem BOM).
- Inclui: código-fonte, documentação, arquivos de configuração e mensagens de commit.
- Quando o terminal não garantir UTF-8 corretamente, usar mensagem de commit em ASCII para evitar corrupção no histórico.

## GitFlow adotado

- Branch de desenvolvimento principal: develop
- Branch de comparação/release: master
- feature/\*: nasce de develop e volta para develop via PR
- bugfix/\*: nasce de develop e volta para develop via PR
- release/x.y.z: nasce de develop, PR para master, depois PR de master para develop
- hotfix/\*: nasce de master, PR para master, depois PR de master para develop

## Sincronização

- Manter sincronização frequente com remoto.
- Tratar sincronização como parte da conclusão de tarefa.
