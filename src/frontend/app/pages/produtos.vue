<script setup lang="ts">
type Category = {
  id: number;
  name: string;
  description: string | null;
};

type Product = {
  id: number;
  name: string;
  description: string | null;
  price: number;
  categoryId: number;
  category?: Category | null;
};

type ProductPayload = {
  name: string;
  description: string | null;
  price: number;
  categoryId: number | null;
};

const config = useRuntimeConfig();
const apiBase = config.public.apiBase as string;

const products = ref<Product[]>([]);
const categories = ref<Category[]>([]);
const loading = ref(false);
const errorMessage = ref("");
const errorType = ref<"validation" | "conflict" | "server" | null>(null);

const createForm = reactive<ProductPayload>({
  name: "",
  description: null,
  price: 0,
  categoryId: null,
});

const editingId = ref<number | null>(null);
const editForm = reactive<ProductPayload>({
  name: "",
  description: null,
  price: 0,
  categoryId: null,
});

const isCreateValid = computed(
  () => createForm.name.trim().length >= 5 && Number(createForm.categoryId) > 0,
);
const isEditValid = computed(
  () => editForm.name.trim().length >= 5 && Number(editForm.categoryId) > 0,
);

async function loadData() {
  loading.value = true;
  errorMessage.value = "";
  errorType.value = null;
  try {
    const [categoryResult, productResult] = await Promise.all([
      $fetch<Category[]>(`${apiBase}/api/categorias`),
      $fetch<Product[]>(`${apiBase}/api/produtos`),
    ]);

    categories.value = categoryResult;
    products.value = productResult;
  } catch (error: any) {
    errorMessage.value = "Nao foi possivel carregar produtos.";
    errorType.value = "server";
  } finally {
    loading.value = false;
  }
}

function mapCategory(categoryId: number) {
  return categories.value.find((item) => item.id === categoryId) || null;
}

async function createProduct() {
  if (!isCreateValid.value) return;

  errorMessage.value = "";
  errorType.value = null;
  try {
    const created = await $fetch<Product>(`${apiBase}/api/produtos`, {
      method: "POST",
      body: {
        name: createForm.name.trim(),
        description: createForm.description?.trim() || null,
        price: createForm.price,
        categoryId: Number(createForm.categoryId),
      },
    });

    products.value = [
      ...products.value,
      {
        ...created,
        category: mapCategory(created.categoryId),
      },
    ];

    createForm.name = "";
    createForm.description = null;
    createForm.price = 0;
    createForm.categoryId = null;
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;
    errorMessage.value = error?.data?.message || "Falha ao criar produto.";
    errorType.value = statusCode === 409 ? "conflict" : "server";
  }
}

function startEdit(item: Product) {
  editingId.value = item.id;
  editForm.name = item.name;
  editForm.description = item.description;
  editForm.price = item.price;
  editForm.categoryId = item.categoryId;
}

function cancelEdit() {
  editingId.value = null;
  editForm.name = "";
  editForm.description = null;
  editForm.price = 0;
  editForm.categoryId = null;
  errorMessage.value = "";
  errorType.value = null;
}

async function saveEdit(id: number) {
  if (!isEditValid.value) return;

  errorMessage.value = "";
  errorType.value = null;
  try {
    const updated = await $fetch<Product>(`${apiBase}/api/produtos/${id}`, {
      method: "PUT",
      body: {
        name: editForm.name.trim(),
        description: editForm.description?.trim() || null,
        price: editForm.price,
        categoryId: Number(editForm.categoryId),
      },
    });

    products.value = products.value.map((item) =>
      item.id === id
        ? {
            ...updated,
            category: mapCategory(updated.categoryId),
          }
        : item,
    );

    cancelEdit();
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;
    errorMessage.value = error?.data?.message || "Falha ao atualizar produto.";
    errorType.value = statusCode === 409 ? "conflict" : "server";
  }
}

async function removeProduct(id: number) {
  if (!confirm("Tem certeza que deseja excluir este produto?")) return;

  errorMessage.value = "";
  errorType.value = null;
  try {
    await $fetch(`${apiBase}/api/produtos/${id}`, { method: "DELETE" });
    products.value = products.value.filter((item) => item.id !== id);
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;

    if (statusCode === 409) {
      errorMessage.value =
        "Nao eh possivel excluir um produto com pedidos vinculados.";
      errorType.value = "conflict";
      return;
    }

    errorMessage.value = error?.data?.message || "Falha ao excluir produto.";
    errorType.value = "server";
  }
}

onMounted(loadData);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Produtos</h1>
      <p>Gerencie produtos e suas categorias.</p>
    </header>

    <div class="neo-card form-card">
      <h2>Criar novo produto</h2>
      <div class="form-grid">
        <div class="form-field">
          <input
            v-model="createForm.name"
            class="neo-input"
            :class="{
              'input-error':
                createForm.name.trim().length > 0 &&
                createForm.name.trim().length < 5,
            }"
            placeholder="Nome (minimo 5 caracteres)"
            aria-label="Nome do produto"
          />
          <span
            v-if="
              createForm.name.trim().length > 0 &&
              createForm.name.trim().length < 5
            "
            class="field-hint error"
          >
            Minimo 5 caracteres
          </span>
        </div>
        <input
          v-model="createForm.description"
          class="neo-input"
          placeholder="Descricao (opcional)"
          aria-label="Descricao do produto"
        />
        <input
          v-model.number="createForm.price"
          class="neo-input"
          type="number"
          min="0.01"
          step="0.01"
          placeholder="Preco"
          aria-label="Preco do produto"
        />
        <select v-model.number="createForm.categoryId" class="neo-input">
          <option :value="null">Selecione categoria</option>
          <option
            v-for="category in categories"
            :key="category.id"
            :value="category.id"
          >
            {{ category.name }}
          </option>
        </select>
        <button
          class="neo-button primary"
          :disabled="!isCreateValid"
          @click="createProduct"
          aria-label="Salvar novo produto"
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
      <p>Carregando produtos...</p>
    </div>

    <div class="neo-card table-card" v-if="!loading">
      <div class="table-wrap" v-if="products.length > 0">
        <table role="grid">
          <thead>
            <tr>
              <th scope="col">ID</th>
              <th scope="col">Nome</th>
              <th scope="col">Descricao</th>
              <th scope="col">Preco</th>
              <th scope="col">Categoria</th>
              <th scope="col">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in products" :key="item.id">
              <td class="id-col">{{ item.id }}</td>
              <td v-if="editingId !== item.id">{{ item.name }}</td>
              <td v-else>
                <input
                  v-model="editForm.name"
                  class="neo-input"
                  aria-label="Editar nome do produto"
                />
              </td>
              <td v-if="editingId !== item.id">
                {{ item.description || "-" }}
              </td>
              <td v-else>
                <input
                  v-model="editForm.description"
                  class="neo-input"
                  aria-label="Editar descricao do produto"
                />
              </td>
              <td v-if="editingId !== item.id" class="price-col">
                {{ Number(item.price).toFixed(2) }}
              </td>
              <td v-else>
                <input
                  v-model.number="editForm.price"
                  class="neo-input"
                  type="number"
                  min="0.01"
                  step="0.01"
                  aria-label="Editar preco do produto"
                />
              </td>
              <td v-if="editingId !== item.id">
                {{ item.category?.name || "-" }}
              </td>
              <td v-else>
                <select
                  v-model.number="editForm.categoryId"
                  class="neo-input"
                  aria-label="Editar categoria do produto"
                >
                  <option :value="null">Selecione categoria</option>
                  <option
                    v-for="category in categories"
                    :key="category.id"
                    :value="category.id"
                  >
                    {{ category.name }}
                  </option>
                </select>
              </td>
              <td class="actions">
                <template v-if="editingId !== item.id">
                  <button
                    class="neo-button ghost"
                    @click="startEdit(item)"
                    aria-label="Editar produto"
                  >
                    Editar
                  </button>
                  <button
                    class="neo-button ghost danger"
                    @click="removeProduct(item.id)"
                    aria-label="Excluir produto"
                  >
                    Excluir
                  </button>
                </template>
                <template v-else>
                  <button
                    class="neo-button primary"
                    :disabled="!isEditValid"
                    @click="saveEdit(item.id)"
                    aria-label="Salvar edicao"
                  >
                    Salvar
                  </button>
                  <button
                    class="neo-button ghost"
                    @click="cancelEdit"
                    aria-label="Cancelar edicao"
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
        <div class="empty-icon"></div>
        <strong>Nenhum produto ainda</strong>
        <p>Crie seu primeiro produto e o vincule a uma categoria.</p>
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
  grid-template-columns: 1.5fr 1fr 100px 1fr auto;
}

.form-field {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
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

.neo-input.input-error {
  border-color: var(--danger);
}

.field-hint {
  font-size: 0.75rem;
  padding-left: 0.5rem;
}

.field-hint.error {
  color: var(--danger);
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
  padding: 0.75rem 1rem;
  border: 1px solid transparent;
  margin: 0;
}

.neo-alert.danger {
  border-color: color-mix(in srgb, var(--danger) 35%, var(--line));
  color: var(--danger);
  background: color-mix(in srgb, var(--danger) 12%, transparent);
}

.neo-alert.warning {
  border-color: color-mix(in srgb, var(--warning) 35%, var(--line));
  color: var(--warning);
  background: color-mix(in srgb, var(--warning) 12%, transparent);
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
  -webkit-overflow-scrolling: touch;
}

table {
  width: 100%;
  border-collapse: collapse;
  min-width: 700px;
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
  font-size: 0.875rem;
}

.price-col {
  font-variant-numeric: tabular-nums;
  font-weight: 700;
}

.actions {
  display: flex;
  gap: 0.4rem;
  align-items: center;
  flex-wrap: wrap;
}

.empty-state {
  text-align: center;
  padding: 2rem 1rem;
  color: var(--text-soft);
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

.empty-state p {
  margin: 0.35rem 0 0;
}

@media (max-width: 1024px) {
  .form-grid {
    grid-template-columns: 1fr 1fr auto;
  }

  table {
    min-width: 700px;
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

  table {
    min-width: 600px;
  }
}
</style>
