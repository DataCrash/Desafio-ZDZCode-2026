<script setup lang="ts">
type Address = {
  street: string;
  number: string;
  district: string;
  city: string;
  state: string;
  zipCode: string;
  complement?: string | null;
};

type Customer = {
  id: number;
  name: string;
  email: string;
  phone?: string | null;
  isActive: boolean;
  deliveryAddress?: Address | null;
};

const config = useRuntimeConfig();
const apiBase = config.public.apiBase as string;

const customers = ref<Customer[]>([]);
const loading = ref(false);
const errorMessage = ref("");

const createForm = reactive({
  name: "",
  email: "",
  phone: "",
  isActive: true,
  deliveryAddress: {
    street: "",
    number: "",
    district: "",
    city: "",
    state: "",
    zipCode: "",
    complement: "",
  },
});

const canCreate = computed(
  () =>
    createForm.name.trim().length >= 3 && createForm.email.trim().length >= 5,
);

function clearForm() {
  createForm.name = "";
  createForm.email = "";
  createForm.phone = "";
  createForm.isActive = true;
  createForm.deliveryAddress.street = "";
  createForm.deliveryAddress.number = "";
  createForm.deliveryAddress.district = "";
  createForm.deliveryAddress.city = "";
  createForm.deliveryAddress.state = "";
  createForm.deliveryAddress.zipCode = "";
  createForm.deliveryAddress.complement = "";
}

async function loadCustomers() {
  loading.value = true;
  errorMessage.value = "";
  try {
    customers.value = await $fetch<Customer[]>(`${apiBase}/api/clientes`);
  } catch {
    errorMessage.value = "Falha ao carregar clientes.";
  } finally {
    loading.value = false;
  }
}

async function createCustomer() {
  if (!canCreate.value) return;

  errorMessage.value = "";
  try {
    const created = await $fetch<Customer>(`${apiBase}/api/clientes`, {
      method: "POST",
      body: {
        name: createForm.name.trim(),
        email: createForm.email.trim(),
        phone: createForm.phone.trim() || null,
        isActive: createForm.isActive,
        deliveryAddress:
          createForm.deliveryAddress.street.trim().length > 0
            ? {
                street: createForm.deliveryAddress.street.trim(),
                number: createForm.deliveryAddress.number.trim() || "S/N",
                district: createForm.deliveryAddress.district.trim(),
                city: createForm.deliveryAddress.city.trim(),
                state: createForm.deliveryAddress.state.trim(),
                zipCode: createForm.deliveryAddress.zipCode.trim(),
                complement:
                  createForm.deliveryAddress.complement.trim() || null,
              }
            : null,
      },
    });

    customers.value = [...customers.value, created];
    clearForm();
  } catch (error: any) {
    errorMessage.value = error?.data?.message || "Falha ao criar cliente.";
  }
}

onMounted(loadCustomers);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Clientes</h1>
      <p>Cadastro de clientes com endereco de entrega opcional.</p>
    </header>

    <div class="neo-card form-card">
      <h2>Novo cliente</h2>
      <div class="form-grid two-col">
        <input v-model="createForm.name" class="neo-input" placeholder="Nome" />
        <input
          v-model="createForm.email"
          class="neo-input"
          placeholder="Email"
        />
        <input
          v-model="createForm.phone"
          class="neo-input"
          placeholder="Telefone"
        />
        <label class="toggle-wrap">
          <input v-model="createForm.isActive" type="checkbox" />
          <span>Ativo</span>
        </label>

        <input
          v-model="createForm.deliveryAddress.street"
          class="neo-input"
          placeholder="Rua"
        />
        <input
          v-model="createForm.deliveryAddress.number"
          class="neo-input"
          placeholder="Numero"
        />
        <input
          v-model="createForm.deliveryAddress.district"
          class="neo-input"
          placeholder="Bairro"
        />
        <input
          v-model="createForm.deliveryAddress.city"
          class="neo-input"
          placeholder="Cidade"
        />
        <input
          v-model="createForm.deliveryAddress.state"
          class="neo-input"
          placeholder="UF"
        />
        <input
          v-model="createForm.deliveryAddress.zipCode"
          class="neo-input"
          placeholder="CEP"
        />
        <input
          v-model="createForm.deliveryAddress.complement"
          class="neo-input"
          placeholder="Complemento"
        />

        <button
          class="neo-button primary"
          :disabled="!canCreate"
          @click="createCustomer"
        >
          Salvar cliente
        </button>
      </div>
    </div>

    <p v-if="errorMessage" class="neo-alert danger">{{ errorMessage }}</p>
    <p v-if="loading" class="loading-note">Carregando clientes...</p>

    <div v-if="!loading" class="neo-card table-card">
      <div v-if="customers.length > 0" class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Nome</th>
              <th>Email</th>
              <th>Telefone</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in customers" :key="item.id">
              <td>{{ item.id }}</td>
              <td>{{ item.name }}</td>
              <td>{{ item.email }}</td>
              <td>{{ item.phone || "-" }}</td>
              <td>{{ item.isActive ? "Ativo" : "Inativo" }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-else class="empty-state">
        <strong>Nenhum cliente cadastrado</strong>
        <p>Cadastre o primeiro cliente para habilitar pedidos.</p>
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

.form-card h2 {
  margin: 0 0 0.78rem;
  font-size: 1rem;
}

.form-grid {
  display: grid;
  gap: 0.62rem;
}

.form-grid.two-col {
  grid-template-columns: repeat(2, minmax(0, 1fr));
}

.toggle-wrap {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  color: var(--text-soft);
  font-weight: 600;
}

.neo-input {
  width: 100%;
  border: 1px solid var(--line);
  border-radius: 11px;
  background: color-mix(in srgb, var(--surface) 92%, transparent);
  color: var(--text);
  font: inherit;
  padding: 0.56rem 0.68rem;
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

.empty-state {
  display: grid;
  gap: 0.25rem;
  text-align: center;
  padding: 1.6rem 0.5rem;
  color: var(--text-soft);
}

@media (max-width: 900px) {
  .form-grid.two-col {
    grid-template-columns: 1fr;
  }
}
</style>
