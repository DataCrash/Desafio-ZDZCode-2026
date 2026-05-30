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
  try {
    const [categoryResult, productResult] = await Promise.all([
      $fetch<Category[]>(`${apiBase}/api/categorias`),
      $fetch<Product[]>(`${apiBase}/api/produtos`),
    ]);

    categories.value = categoryResult;
    products.value = productResult;
  } catch {
    errorMessage.value = "Failed to load products.";
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
  } catch {
    errorMessage.value = "Failed to create product.";
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
}

async function saveEdit(id: number) {
  if (!isEditValid.value) return;

  errorMessage.value = "";
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
  } catch {
    errorMessage.value = "Failed to update product.";
  }
}

async function removeProduct(id: number) {
  if (!confirm("Are you sure you want to delete this product?")) return;

  errorMessage.value = "";
  try {
    await $fetch(`${apiBase}/api/produtos/${id}`, { method: "DELETE" });
    products.value = products.value.filter((item) => item.id !== id);
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;
    const backendMessage = error?.data?.message;

    if (statusCode === 409) {
      errorMessage.value =
        "Operation blocked by referential integrity rule.";
      return;
    }

    errorMessage.value = backendMessage || "Failed to delete product.";
  }
}

onMounted(loadData);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Products</h1>
      <p>Manage products and category links.</p>
    </header>

    <div class="neo-card form-card">
      <h2>Create product</h2>
      <div class="form-grid">
        <input
          v-model="createForm.name"
          class="neo-input"
          placeholder="Name (min 5)"
        />
        <input
          v-model="createForm.description"
          class="neo-input"
          placeholder="Description"
        />
        <input
          v-model.number="createForm.price"
          class="neo-input"
          type="number"
          min="0.01"
          step="0.01"
          placeholder="Price"
        />
        <select v-model.number="createForm.categoryId" class="neo-input">
          <option :value="null">Select category</option>
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
        >
          Save
        </button>
      </div>
    </div>

    <p v-if="errorMessage" class="neo-alert danger">{{ errorMessage }}</p>
    <p v-if="loading" class="loading-note">Loading products...</p>

    <div class="neo-card table-card" v-if="!loading">
      <div class="table-wrap" v-if="products.length > 0">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Description</th>
              <th>Price</th>
              <th>Category</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in products" :key="item.id">
              <td class="id-col">{{ item.id }}</td>
              <td v-if="editingId !== item.id">{{ item.name }}</td>
              <td v-else><input v-model="editForm.name" class="neo-input" /></td>
              <td v-if="editingId !== item.id">{{ item.description || "-" }}</td>
              <td v-else>
                <input v-model="editForm.description" class="neo-input" />
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
                />
              </td>
              <td v-if="editingId !== item.id">
                {{ item.category?.name || "-" }}
              </td>
              <td v-else>
                <select v-model.number="editForm.categoryId" class="neo-input">
                  <option :value="null">Select category</option>
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
                  <button class="neo-button ghost" @click="startEdit(item)">
                    Edit
                  </button>
                  <button
                    class="neo-button ghost danger"
                    @click="removeProduct(item.id)"
                  >
                    Delete
                  </button>
                </template>
                <template v-else>
                  <button
                    class="neo-button primary"
                    :disabled="!isEditValid"
                    @click="saveEdit(item.id)"
                  >
                    Save
                  </button>
                  <button class="neo-button ghost" @click="cancelEdit">
                    Cancel
                  </button>
                </template>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-else class="empty-state">
        <strong>No products yet</strong>
        <p>Create your first product and link it to a category.</p>
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
}

.form-card h2 {
  margin: 0 0 0.78rem;
  font-size: 1rem;
}

.form-grid {
  display: grid;
  gap: 0.62rem;
  grid-template-columns: 1.15fr 1fr 130px 1fr auto;
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
}

.neo-button {
  border-radius: 999px;
  border: 1px solid transparent;
  padding: 0.52rem 0.86rem;
  font: inherit;
  font-weight: 700;
  cursor: pointer;
}

.neo-button:disabled {
  cursor: not-allowed;
  opacity: 0.55;
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
  min-width: 840px;
}

th,
td {
  border-bottom: 1px solid var(--line);
  padding: 0.68rem 0.45rem;
  text-align: left;
}

th {
  color: var(--text-soft);
  font-weight: 700;
  font-size: 0.8rem;
}

.id-col {
  color: var(--text-soft);
  font-weight: 700;
}

.price-col {
  font-variant-numeric: tabular-nums;
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

@media (max-width: 1140px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
