<script setup lang="ts">
type Category = {
  id: number;
  name: string;
  description: string | null;
};

type CategoryPayload = {
  name: string;
  description: string | null;
};

const config = useRuntimeConfig();
const apiBase = config.public.apiBase as string;

const categories = ref<Category[]>([]);
const loading = ref(false);
const errorMessage = ref("");
const errorType = ref<"validation" | "conflict" | "server" | null>(null);

const createForm = reactive<CategoryPayload>({
  name: "",
  description: null,
});

const editingId = ref<number | null>(null);
const editForm = reactive<CategoryPayload>({
  name: "",
  description: null,
});

const isCreateValid = computed(() => createForm.name.trim().length >= 5);
const isEditValid = computed(() => editForm.name.trim().length >= 5);
const hasCreateError = computed(
  () => createForm.name.trim().length > 0 && !isCreateValid.value,
);

async function loadCategories() {
  loading.value = true;
  errorMessage.value = "";
  errorType.value = null;
  try {
    categories.value = await $fetch<Category[]>(`${apiBase}/api/categorias`);
  } catch (error: any) {
    errorMessage.value = "Nao foi possivel carregar categorias.";
    errorType.value = "server";
  } finally {
    loading.value = false;
  }
}

async function createCategory() {
  if (!isCreateValid.value) return;

  errorMessage.value = "";
  errorType.value = null;
  try {
    const created = await $fetch<Category>(`${apiBase}/api/categorias`, {
      method: "POST",
      body: {
        name: createForm.name.trim(),
        description: createForm.description?.trim() || null,
      },
    });

    categories.value = [...categories.value, created];
    createForm.name = "";
    createForm.description = null;
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;
    errorMessage.value = error?.data?.message || "Falha ao criar categoria.";
    errorType.value = statusCode === 409 ? "conflict" : "server";
  }
}

function startEdit(category: Category) {
  editingId.value = category.id;
  editForm.name = category.name;
  editForm.description = category.description;
}

function cancelEdit() {
  editingId.value = null;
  editForm.name = "";
  editForm.description = null;
  errorMessage.value = "";
  errorType.value = null;
}

async function saveEdit(id: number) {
  if (!isEditValid.value) return;

  errorMessage.value = "";
  errorType.value = null;
  try {
    const updated = await $fetch<Category>(`${apiBase}/api/categorias/${id}`, {
      method: "PUT",
      body: {
        name: editForm.name.trim(),
        description: editForm.description?.trim() || null,
      },
    });

    categories.value = categories.value.map((item) =>
      item.id === id ? updated : item,
    );
    cancelEdit();
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;
    errorMessage.value =
      error?.data?.message || "Falha ao atualizar categoria.";
    errorType.value = statusCode === 409 ? "conflict" : "server";
  }
}

async function removeCategory(id: number) {
  if (!confirm("Tem certeza que deseja excluir esta categoria?")) return;

  errorMessage.value = "";
  errorType.value = null;
  try {
    await $fetch(`${apiBase}/api/categorias/${id}`, { method: "DELETE" });
    categories.value = categories.value.filter((item) => item.id !== id);
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;

    if (statusCode === 409) {
      errorMessage.value =
        "Nao eh possivel excluir uma categoria com produtos vinculados.";
      errorType.value = "conflict";
      return;
    }

    errorMessage.value = error?.data?.message || "Falha ao excluir categoria.";
    errorType.value = "server";
  }
}

onMounted(loadCategories);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Categorias</h1>
      <p>Gerencie os registros de categorias usados pelos produtos.</p>
    </header>

    <div class="neo-card form-card">
      <h2>Criar nova categoria</h2>
      <div class="form-grid">
        <div class="form-field">
          <input
            v-model="createForm.name"
            class="neo-input"
            :class="{ 'input-error': hasCreateError }"
            placeholder="Nome (mínimo 5 caracteres)"
            aria-label="Nome da categoria"
            aria-invalid="false"
          />
          <span v-if="hasCreateError" class="field-hint error">
            Mínimo 5 caracteres
          </span>
        </div>
        <input
          v-model="createForm.description"
          class="neo-input"
          placeholder="Descrição (opcional)"
          aria-label="Descrição da categoria"
        />
        <button
          class="neo-button primary"
          :disabled="!isCreateValid"
          @click="createCategory"
          aria-label="Salvar nova categoria"
        >
          Salvar
        </button>
      </div>
    </div>

    <div
      v-if="errorMessage"
      :class="['neo-alert', errorType === 'conflict' ? 'warning' : 'danger']"
      role="alert"
    >
      <strong>{{ errorType === "conflict" ? "Aviso" : "Erro" }}:</strong>
      {{ errorMessage }}
    </div>

    <div v-if="loading" class="loading-state" role="status" aria-live="polite">
      <div class="spinner"></div>
      <p>Carregando categorias...</p>
    </div>

    <div class="neo-card table-card" v-if="!loading">
      <div class="table-wrap" v-if="categories.length > 0">
        <table role="grid">
          <thead>
            <tr>
              <th scope="col">ID</th>
              <th scope="col">Nome</th>
              <th scope="col">Descrição</th>
              <th scope="col">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in categories" :key="item.id">
              <td class="id-col">{{ item.id }}</td>
              <td v-if="editingId !== item.id">{{ item.name }}</td>
              <td v-else>
                <input v-model="editForm.name" class="neo-input" />
              </td>
              <td v-if="editingId !== item.id">
                {{ item.description || "-" }}
              </td>
              <td v-else>
                <input v-model="editForm.description" class="neo-input" />
              </td>
              <td class="actions">
                <template v-if="editingId !== item.id">
                  <button
                    class="neo-button ghost"
                    @click="startEdit(item)"
                    aria-label="Editar categoria"
                  >
                    Editar
                  </button>
                  <button
                    class="neo-button ghost danger"
                    @click="removeCategory(item.id)"
                    aria-label="Excluir categoria"
                  >
                    Excluir
                  </button>
                </template>
                <template v-else>
                  <button
                    class="neo-button primary"
                    :disabled="!isEditValid"
                    @click="saveEdit(item.id)"
                    aria-label="Salvar edição"
                  >
                    Salvar
                  </button>
                  <button
                    class="neo-button ghost"
                    @click="cancelEdit"
                    aria-label="Cancelar edição"
                  >
                    Cancelar
                  </button>
                </template>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-else class="empty-state">
        <div class="empty-icon">📦</div>
        <strong>Nenhuma categoria ainda</strong>
        <p>Crie sua primeira categoria para começar a vincular produtos.</p>
      </div>
    </div>
  </section>
</template>

<style scoped>
.screen {
  display: grid;
  gap: 1rem;
}

.screen-header h1 {
  margin: 0;
  font-family: "Sora", "Manrope", sans-serif;
  letter-spacing: 0.01em;
}

.screen-header p {
  margin: 0.3rem 0 0;
  color: var(--text-soft);
}

.neo-card {
  border: 1px solid var(--line);
  border-radius: 16px;
  background: color-mix(in srgb, var(--surface-strong) 86%, transparent);
  box-shadow: var(--shadow);
  padding: 1rem;
  transition:
    border-color 140ms ease,
    transform 140ms ease;
}

.neo-card:hover {
  border-color: color-mix(in srgb, var(--accent) 28%, var(--line));
}

.form-card h2 {
  margin: 0 0 0.78rem;
  font-size: 1rem;
}

.form-grid {
  display: grid;
  gap: 0.62rem;
  grid-template-columns: 1.2fr 1fr auto;
}

.neo-input {
  width: 100%;
  border: 1px solid var(--line);
  border-radius: 11px;
  background: color-mix(in srgb, var(--surface) 92%, transparent);
  color: var(--text);
  font: inherit;
  padding: 0.56rem 0.68rem;
  outline: none;
  transition: border-color 120ms ease;
}

.neo-input:focus {
  border-color: color-mix(in srgb, var(--accent) 68%, var(--line));
  box-shadow: 0 0 0 3px var(--focus-ring);
}

.neo-button {
  border-radius: 999px;
  border: 1px solid transparent;
  padding: 0.52rem 0.86rem;
  font: inherit;
  font-weight: 700;
  cursor: pointer;
  transition:
    transform 120ms ease,
    border-color 120ms ease,
    color 120ms ease,
    background-color 120ms ease;
}

.neo-button:disabled {
  cursor: not-allowed;
  opacity: 0.55;
}

.neo-button.primary {
  background: var(--accent);
  color: #05201d;
}

.neo-button:not(:disabled):hover {
  transform: translateY(-1px);
}

.neo-button:focus-visible {
  outline: none;
  box-shadow: 0 0 0 3px var(--focus-ring);
}

.neo-button.ghost {
  border-color: var(--line);
  background: transparent;
  color: var(--text-soft);
}

.neo-button.ghost:hover {
  color: var(--text);
}

.neo-button.ghost.danger {
  color: var(--danger);
}

.neo-alert {
  border-radius: 12px;
  padding: 0.68rem 0.75rem;
  border: 1px solid transparent;
}

.neo-alert.danger {
  border-color: color-mix(in srgb, var(--danger) 35%, var(--line));
  color: var(--danger);
  background: color-mix(in srgb, var(--danger) 12%, transparent);
}

.loading-note {
  margin: 0;
  color: var(--text-soft);
}

.table-wrap {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  min-width: 620px;
}

th,
td {
  border-bottom: 1px solid var(--line);
  padding: 0.72rem 0.5rem;
  text-align: left;
}

th {
  color: var(--text-soft);
  font-weight: 700;
  font-size: 0.8rem;
  letter-spacing: 0.01em;
}

tbody tr {
  transition: background-color 120ms ease;
}

tbody tr:hover {
  background: var(--row-hover);
}

.id-col {
  color: var(--text-soft);
  font-weight: 700;
}

.actions {
  display: flex;
  gap: 0.4rem;
  align-items: center;
}

.empty-state {
  text-align: center;
  padding: 1.4rem 0.8rem;
}

.empty-state p {
  color: var(--text-soft);
  margin: 0.35rem 0 0;
}

.form-field {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.field-hint {
  font-size: 0.75rem;
  padding-left: 0.5rem;
}

.field-hint.error {
  color: var(--danger);
}

.neo-input.input-error {
  border-color: var(--danger);
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 2rem 1rem;
  color: var(--text-soft);
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid var(--line);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.neo-alert.warning {
  border-color: color-mix(in srgb, var(--warning) 35%, var(--line));
  color: var(--warning);
  background: color-mix(in srgb, var(--warning) 12%, transparent);
}

.empty-icon {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

.empty-state strong {
  display: block;
  margin-top: 0.5rem;
  color: var(--text);
}

.form-grid {
  grid-template-columns: 1.5fr 1fr auto;
}

.table-wrap {
  -webkit-overflow-scrolling: touch;
}

table {
  min-width: 600px;
}

.id-col {
  font-size: 0.875rem;
}

.actions {
  flex-wrap: wrap;
}

.empty-state {
  padding: 2rem 1rem;
}

@media (max-width: 1024px) {
  .form-grid {
    grid-template-columns: 1fr 1fr auto;
  }
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }

  .form-grid button {
    width: 100%;
  }

  th,
  td {
    padding: 0.6rem 0.4rem;
    font-size: 0.875rem;
  }

  .neo-button {
    padding: 0.4rem 0.6rem;
    font-size: 0.875rem;
  }

  .actions {
    gap: 0.2rem;
  }
}
</style>
