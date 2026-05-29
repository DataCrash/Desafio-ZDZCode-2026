# Convenção de Commits

Formato sugerido:

[emoji] [tipo](escopo opcional): descrição curta

Exemplos:

- ✨ feat(auth): cria fluxo de login com refresh token
- 🐛 fix(api): corrige serializacao de datas em UTC
- ♻️ refactor(core): simplifica validação de entrada
- ✅ test(user): adiciona testes de criacao de conta
- 📝 docs(readme): documenta setup inicial
- 🔧 chore(ci): ajusta cache do pipeline

## Regras

- Emoji é obrigatório e deve ser coerente com o tipo da mudança.
- Mensagem objetiva, no imperativo e em linha única.
- Um commit = uma intenção.
- Evitar commits mistos (feature + refactor + fix no mesmo commit).
- Proibido commitar código quebrado.
