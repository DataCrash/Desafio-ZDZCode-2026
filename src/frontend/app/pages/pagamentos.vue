<script setup lang="ts">
type Order = { id: number; total: number; status: string };
type Payment = {
  id: number;
  orderId: number;
  method: string;
  status: string;
  value: number;
  transactionReference?: string | null;
  createdAt: string;
};

const config = useRuntimeConfig();
const apiBase = config.public.apiBase as string;

const orders = ref<Order[]>([]);
const payments = ref<Payment[]>([]);
const errorMessage = ref("");
const loading = ref(false);

const createForm = reactive({
  orderId: null as number | null,
  method: "pix",
  status: "approved",
  value: 0,
  transactionReference: "",
});

const canCreate = computed(
  () => Number(createForm.orderId) > 0 && createForm.value > 0,
);

async function loadData() {
  loading.value = true;
  errorMessage.value = "";
  try {
    const [orderResult, paymentResult] = await Promise.all([
      $fetch<Order[]>(`${apiBase}/api/pedidos`),
      $fetch<Payment[]>(`${apiBase}/api/pagamentos`),
    ]);

    orders.value = orderResult;
    payments.value = paymentResult;
  } catch {
    errorMessage.value = "Falha ao carregar pagamentos.";
  } finally {
    loading.value = false;
  }
}

function onOrderChange() {
  const order = orders.value.find((item) => item.id === createForm.orderId);
  createForm.value = order ? Number(order.total) : 0;
}

async function createPayment() {
  if (!canCreate.value) return;

  errorMessage.value = "";
  try {
    const created = await $fetch<Payment>(`${apiBase}/api/pagamentos`, {
      method: "POST",
      body: {
        orderId: Number(createForm.orderId),
        method: createForm.method.trim(),
        status: createForm.status.trim(),
        value: Number(createForm.value),
        transactionReference: createForm.transactionReference.trim() || null,
      },
    });

    payments.value = [created, ...payments.value];
    createForm.orderId = null;
    createForm.value = 0;
    createForm.transactionReference = "";
  } catch (error: any) {
    errorMessage.value = error?.data?.message || "Falha ao criar pagamento.";
  }
}

onMounted(loadData);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Pagamentos</h1>
      <p>Registro de pagamentos vinculados aos pedidos.</p>
    </header>

    <div class="neo-card form-card">
      <h2>Novo pagamento</h2>
      <div class="form-grid payment-grid">
        <select
          v-model.number="createForm.orderId"
          class="neo-input"
          @change="onOrderChange"
        >
          <option :value="null">Selecione pedido</option>
          <option v-for="order in orders" :key="order.id" :value="order.id">
            Pedido #{{ order.id }} - total {{ Number(order.total).toFixed(2) }}
          </option>
        </select>

        <input
          v-model="createForm.method"
          class="neo-input"
          placeholder="Metodo"
        />
        <input
          v-model="createForm.status"
          class="neo-input"
          placeholder="Status"
        />

        <input
          v-model.number="createForm.value"
          class="neo-input"
          type="number"
          min="0.01"
          step="0.01"
          placeholder="Valor"
        />

        <input
          v-model="createForm.transactionReference"
          class="neo-input"
          placeholder="Referencia da transacao"
        />

        <button
          class="neo-button primary"
          :disabled="!canCreate"
          @click="createPayment"
        >
          Registrar pagamento
        </button>
      </div>
    </div>

    <p v-if="errorMessage" class="neo-alert danger">{{ errorMessage }}</p>
    <p v-if="loading" class="loading-note">Carregando pagamentos...</p>

    <div class="neo-card table-card" v-if="!loading">
      <div v-if="payments.length > 0" class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Pedido</th>
              <th>Metodo</th>
              <th>Status</th>
              <th>Valor</th>
              <th>Referencia</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="payment in payments" :key="payment.id">
              <td>{{ payment.id }}</td>
              <td>#{{ payment.orderId }}</td>
              <td>{{ payment.method }}</td>
              <td>{{ payment.status }}</td>
              <td>{{ Number(payment.value).toFixed(2) }}</td>
              <td>{{ payment.transactionReference || "-" }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else class="empty-state">
        <strong>Nenhum pagamento registrado</strong>
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

.payment-grid {
  grid-template-columns: repeat(6, minmax(0, 1fr));
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

@media (max-width: 1200px) {
  .payment-grid {
    grid-template-columns: 1fr;
  }
}
</style>
