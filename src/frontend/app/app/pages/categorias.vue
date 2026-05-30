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

async function loadCategories() {
  loading.value = true;
  errorMessage.value = "";
  try {
    categories.value = await $fetch<Category[]>(`${apiBase}/api/categorias`);
  } catch {
    errorMessage.value = "Failed to load categories.";
  } finally {
    loading.value = false;
  }
}

async function createCategory() {
  if (!isCreateValid.value) return;

  errorMessage.value = "";
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
  } catch {
    errorMessage.value = "Failed to create category.";
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
}

async function saveEdit(id: number) {
  if (!isEditValid.value) return;

  errorMessage.value = "";
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
  } catch {
    errorMessage.value = "Failed to update category.";
  }
}

async function removeCategory(id: number) {
  if (!confirm("Are you sure you want to delete this category?")) return;

  errorMessage.value = "";
  try {
    await $fetch(`${apiBase}/api/categorias/${id}`, { method: "DELETE" });
    categories.value = categories.value.filter((item) => item.id !== id);
  } catch (error: any) {
    const statusCode = error?.statusCode || error?.response?.status;
    const backendMessage = error?.data?.message;
    if (statusCode === 409) {
      errorMessage.value =
        "Nao e possivel excluir uma categoria com produtos vinculados.";
      return;
    }

    errorMessage.value = backendMessage || "Failed to delete category.";
  }
}

onMounted(loadCategories);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Categories</h1>
      <p>Manage category records used by products.</p>
    </header>

    <div class="panel form-panel">
      <h2>Create category</h2>
      <div class="form-grid">
        <input v-model="createForm.name" placeholder="Name (min 5)" />
        <input v-model="createForm.description" placeholder="Description" />
        <button :disabled="!isCreateValid" @click="createCategory">Save</button>
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
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in categories" :key="item.id">
            <td>{{ item.id }}</td>
            <td v-if="editingId !== item.id">{{ item.name }}</td>
            <td v-else>
              <input v-model="editForm.name" />
            </td>
            <td v-if="editingId !== item.id">{{ item.description }}</td>
            <td v-else>
              <input v-model="editForm.description" />
            </td>
            <td>
              <template v-if="editingId !== item.id">
                <button @click="startEdit(item)">Edit</button>
                <button class="danger" @click="removeCategory(item.id)">
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
  grid-template-columns: 1fr 1fr auto;
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

@media (max-width: 900px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
