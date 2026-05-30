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
        "Operacao bloqueada por regra de integridade referencial.";
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

    <div class="panel form-panel">
      <h2>Create product</h2>
      <div class="form-grid">
        <input v-model="createForm.name" placeholder="Name (min 5)" />
        <input v-model="createForm.description" placeholder="Description" />
        <input
          v-model.number="createForm.price"
          type="number"
          min="0.01"
          step="0.01"
          placeholder="Price"
        />
        <select v-model.number="createForm.categoryId">
          <option :value="null">Select category</option>
          <option
            v-for="category in categories"
            :key="category.id"
            :value="category.id"
          >
            {{ category.name }}
          </option>
        </select>
        <button :disabled="!isCreateValid" @click="createProduct">Save</button>
      </div>
    </div>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="loading">Loading...</p>

    <div class="panel table-panel" v-if="!loading">
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
            <td>{{ item.id }}</td>
            <td v-if="editingId !== item.id">{{ item.name }}</td>
            <td v-else><input v-model="editForm.name" /></td>
            <td v-if="editingId !== item.id">{{ item.description }}</td>
            <td v-else><input v-model="editForm.description" /></td>
            <td v-if="editingId !== item.id">
              {{ Number(item.price).toFixed(2) }}
            </td>
            <td v-else>
              <input
                v-model.number="editForm.price"
                type="number"
                min="0.01"
                step="0.01"
              />
            </td>
            <td v-if="editingId !== item.id">
              {{ item.category?.name || "-" }}
            </td>
            <td v-else>
              <select v-model.number="editForm.categoryId">
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
            <td>
              <template v-if="editingId !== item.id">
                <button @click="startEdit(item)">Edit</button>
                <button class="danger" @click="removeProduct(item.id)">
                  Delete
                </button>
              </template>
              <template v-else>
                <button :disabled="!isEditValid" @click="saveEdit(item.id)">
                  Save
                </button>
                <button @click="cancelEdit">Cancel</button>
              </template>
            </td>
          </tr>
        </tbody>
      </table>
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
}

.panel {
  border: 1px solid #cbd5e1;
  border-radius: 12px;
  padding: 1rem;
  background: #ffffff;
}

.form-grid {
  display: grid;
  gap: 0.5rem;
  grid-template-columns: 1fr 1fr 140px 1fr auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  border-bottom: 1px solid #e2e8f0;
  padding: 0.5rem;
  text-align: left;
}

button {
  margin-right: 0.4rem;
}

.danger {
  color: #b91c1c;
}

.error {
  color: #b91c1c;
}

@media (max-width: 1100px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
