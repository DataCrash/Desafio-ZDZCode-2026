# Convencao de Commits

Formato sugerido:

<emoji> <tipo>(escopo opcional): descricao curta

Exemplos:

- ✨ feat(auth): cria fluxo de login com refresh token
- 🐛 fix(api): corrige serializacao de datas em UTC
- ♻️ refactor(core): simplifica validacao de entrada
- ✅ test(user): adiciona testes de criacao de conta
- 📝 docs(readme): documenta setup inicial
- 🔧 chore(ci): ajusta cache do pipeline

## Regras

- Mensagem objetiva, no imperativo e em linha unica.
- Um commit = uma intencao.
- Evitar commits mistos (feature + refactor + fix no mesmo commit).
- Proibido commitar codigo quebrado.
