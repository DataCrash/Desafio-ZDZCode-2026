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
const errorType = ref<"conflict" | "server" | null>(null);
const isCreateOpen = ref(false);

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
    errorMessage.value =
      error?.data?.message || "Não foi possível carregar categorias.";
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
    isCreateOpen.value = false;
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
  errorMessage.value = "";
  errorType.value = null;
  try {
    await $fetch(`${apiBase}/api/categorias/${id}`, { method: "DELETE" });
    categories.value = categories.value.filter((item) => item.id !== id);
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;

    if (statusCode === 409) {
      errorMessage.value =
        "Não é possível excluir uma categoria com produtos vinculados.";
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
      <div class="card-head">
        <h2>
          <button
            v-if="!isCreateOpen"
            class="title-trigger"
            type="button"
            @click="isCreateOpen = true"
          >
            Criar nova categoria
          </button>
          <span v-else>Criar nova categoria</span>
        </h2>
        <button
          class="collapse-toggle"
          type="button"
          :aria-label="
            isCreateOpen ? 'Recolher formulário' : 'Expandir formulário'
          "
          @click="isCreateOpen = !isCreateOpen"
        >
          <span aria-hidden="true">{{ isCreateOpen ? "▴" : "▾" }}</span>
        </button>
      </div>

      <div v-show="isCreateOpen" class="form-grid">
        <div class="form-field required-field">
          <input
            v-model="createForm.name"
            class="neo-input"
            :class="{ 'input-error': hasCreateError }"
            placeholder="Nome da categoria *"
            aria-label="Nome da categoria (obrigatório)"
          />
          <span class="field-meta required">Obrigatório</span>
          <span v-if="hasCreateError" class="field-hint error">
            Mínimo de 5 caracteres
          </span>
        </div>

        <div class="form-field">
          <input
            v-model="createForm.description"
            class="neo-input"
            placeholder="Descrição (opcional)"
            aria-label="Descrição da categoria"
          />
          <span class="field-meta optional">Opcional</span>
        </div>

        <button
          class="neo-button primary"
          :disabled="!isCreateValid"
          aria-label="Salvar categoria"
          @click="createCategory"
        >
          <span aria-hidden="true">💾</span>
          <span class="sr-only">Salvar categoria</span>
        </button>
      </div>
    </div>

    <output
      v-if="errorMessage"
      :class="['neo-alert', errorType === 'conflict' ? 'warning' : 'danger']"
      aria-live="polite"
    >
      <strong>{{ errorType === "conflict" ? "Aviso" : "Erro" }}:</strong>
      {{ errorMessage }}
    </output>

    <output v-if="loading" class="loading-state" aria-live="polite">
      <div class="spinner"></div>
      <p>Carregando categorias...</p>
    </output>

    <div class="neo-card table-card" v-if="!loading">
      <div class="table-wrap" v-if="categories.length > 0">
        <table>
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
                <input
                  v-model="editForm.name"
                  class="neo-input required-field"
                />
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
                    aria-label="Editar categoria"
                    @click="startEdit(item)"
                  >
                    <span aria-hidden="true">✏️</span>
                    <span class="sr-only">Editar categoria</span>
                  </button>
                  <button
                    class="neo-button ghost danger"
                    aria-label="Excluir categoria"
                    @click="removeCategory(item.id)"
                  >
                    <span aria-hidden="true">🗑️</span>
                    <span class="sr-only">Excluir categoria</span>
                  </button>
                </template>
                <template v-else>
                  <button
                    class="neo-button primary"
                    :disabled="!isEditValid"
                    aria-label="Salvar edição"
                    @click="saveEdit(item.id)"
                  >
                    <span aria-hidden="true">💾</span>
                    <span class="sr-only">Salvar edição</span>
                  </button>
                  <button
                    class="neo-button ghost"
                    aria-label="Cancelar edição"
                    @click="cancelEdit"
                  >
                    <span aria-hidden="true">✖️</span>
                    <span class="sr-only">Cancelar edição</span>
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
}

.card-head {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.78rem;
}

.card-head h2 {
  margin: 0;
  font-size: 1rem;
}

.title-trigger {
  border: 0;
  background: transparent;
  color: var(--text);
  padding: 0;
  font: inherit;
  font-weight: 700;
  cursor: pointer;
}

.collapse-toggle {
  border: 1px solid var(--line);
  border-radius: 999px;
  background: transparent;
  color: var(--text-soft);
  width: 32px;
  height: 32px;
  cursor: pointer;
}

.form-grid {
  display: grid;
  gap: 0.62rem;
  grid-template-columns: 1.2fr 1fr auto;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.required-field .neo-input,
.neo-input.required-field {
  border-left: 3px solid var(--accent);
}

.field-meta {
  font-size: 0.72rem;
  padding-left: 0.2rem;
}

.field-meta.required {
  color: var(--accent-strong);
}

.field-meta.optional {
  color: var(--text-soft);
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
}

.neo-input:focus {
  border-color: color-mix(in srgb, var(--accent) 68%, var(--line));
  box-shadow: 0 0 0 3px var(--focus-ring);
}

.neo-input.input-error {
  border-color: var(--danger);
}

.field-hint.error {
  color: var(--danger);
  font-size: 0.75rem;
}

.neo-button {
  border-radius: 999px;
  border: 1px solid transparent;
  padding: 0.52rem 0.86rem;
  font: inherit;
  font-weight: 700;
  cursor: pointer;
}

.neo-button.primary {
  background: var(--accent);
  color: #05201d;
}

.neo-button.ghost {
  border-color: var(--line);
  background: transparent;
  color: var(--text-soft);
}

.neo-button.ghost.danger {
  color: var(--danger);
}

.neo-button:disabled {
  cursor: not-allowed;
  opacity: 0.55;
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

.neo-alert.warning {
  border-color: color-mix(in srgb, #d97706 35%, var(--line));
  color: #5c3a00;
  background: color-mix(in srgb, #d97706 20%, var(--surface));
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

.id-col {
  color: var(--text-soft);
  font-weight: 700;
}

.actions {
  display: flex;
  gap: 0.4rem;
}

.empty-state {
  text-align: center;
  padding: 1.4rem 0.8rem;
}

.empty-state p {
  color: var(--text-soft);
  margin: 0.35rem 0 0;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}

@media (max-width: 920px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
