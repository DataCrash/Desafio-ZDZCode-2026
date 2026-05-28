# Politica de Versionamento

## Principios

- Forward-only: proibido reescrever historico (sem reset/rebase destrutivo), salvo ordem explicita.
- Nunca commitar codigo quebrado.
- Commits curtos, atomicos e semanticos.

## GitFlow adotado

- Branch de desenvolvimento principal: develop
- Branch de comparacao/release: master
- feature/\*: nasce de develop e volta para develop via PR
- bugfix/\*: nasce de develop e volta para develop via PR
- release/x.y.z: nasce de develop, PR para master, depois PR de master para develop
- hotfix/\*: nasce de master, PR para master, depois PR de master para develop

## Sincronizacao

- Manter sincronizacao frequente com remoto.
- Tratar sincronizacao como parte da conclusao de tarefa.
