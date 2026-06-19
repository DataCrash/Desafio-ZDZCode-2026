<script setup lang="ts">
type Customer = { id: number; name: string };
type Product = {
  id: number;
  name: string;
  price: number;
  stockCurrent: number;
};
type OrderItem = {
  id: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
};
type Order = {
  id: number;
  customerId: number;
  status: string;
  subtotal: number;
  discountTotal: number;
  total: number;
  note?: string | null;
  items: OrderItem[];
};

const config = useRuntimeConfig();
const apiBase = config.public.apiBase as string;

const customers = ref<Customer[]>([]);
const products = ref<Product[]>([]);
const orders = ref<Order[]>([]);
const selectedOrderId = ref<number | null>(null);
const errorMessage = ref("");
const loading = ref(false);

const createOrderForm = reactive({
  customerId: null as number | null,
  status: "draft",
  discountTotal: 0,
  note: "",
});

const addItemForm = reactive({
  productId: null as number | null,
  quantity: 1,
});

const canCreateOrder = computed(() => Number(createOrderForm.customerId) > 0);
const canAddItem = computed(
  () =>
    Number(selectedOrderId.value) > 0 &&
    Number(addItemForm.productId) > 0 &&
    addItemForm.quantity > 0,
);

const selectedOrder = computed(
  () =>
    orders.value.find((order) => order.id === selectedOrderId.value) || null,
);

async function loadData() {
  loading.value = true;
  errorMessage.value = "";
  try {
    const [customerResult, productResult, orderResult] = await Promise.all([
      $fetch<Customer[]>(`${apiBase}/api/clientes`),
      $fetch<Product[]>(`${apiBase}/api/produtos`),
      $fetch<Order[]>(`${apiBase}/api/pedidos`),
    ]);

    customers.value = customerResult;
    products.value = productResult;
    orders.value = orderResult;
    if (!selectedOrderId.value && orderResult.length > 0) {
      selectedOrderId.value = orderResult[0].id;
    }
  } catch {
    errorMessage.value = "Falha ao carregar dados de pedidos.";
  } finally {
    loading.value = false;
  }
}

function customerName(customerId: number) {
  return (
    customers.value.find((item) => item.id === customerId)?.name ||
    `#${customerId}`
  );
}

function productName(productId: number) {
  return (
    products.value.find((item) => item.id === productId)?.name ||
    `#${productId}`
  );
}

async function createOrder() {
  if (!canCreateOrder.value) return;

  errorMessage.value = "";
  try {
    const created = await $fetch<Order>(`${apiBase}/api/pedidos`, {
      method: "POST",
      body: {
        customerId: Number(createOrderForm.customerId),
        status: createOrderForm.status.trim() || "draft",
        discountTotal: Number(createOrderForm.discountTotal) || 0,
        note: createOrderForm.note.trim() || null,
      },
    });

    orders.value = [created, ...orders.value];
    selectedOrderId.value = created.id;
  } catch (error: any) {
    errorMessage.value = error?.data?.message || "Falha ao criar pedido.";
  }
}

async function addItem() {
  if (!canAddItem.value || !selectedOrder.value) return;

  errorMessage.value = "";
  try {
    const updated = await $fetch<Order>(
      `${apiBase}/api/pedidos/${selectedOrder.value.id}/itens`,
      {
        method: "POST",
        body: {
          productId: Number(addItemForm.productId),
          quantity: Number(addItemForm.quantity),
        },
      },
    );

    orders.value = orders.value.map((item) =>
      item.id === updated.id ? updated : item,
    );
    addItemForm.productId = null;
    addItemForm.quantity = 1;
  } catch (error: any) {
    errorMessage.value =
      error?.data?.message || "Falha ao adicionar item no pedido.";
  }
}

onMounted(loadData);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Pedidos</h1>
      <p>Criacao de pedidos e adicao de itens com totalizacao pelo backend.</p>
    </header>

    <div class="neo-card form-card">
      <h2>Novo pedido</h2>
      <div class="form-grid order-form-grid">
        <select v-model.number="createOrderForm.customerId" class="neo-input">
          <option :value="null">Selecione cliente</option>
          <option
            v-for="customer in customers"
            :key="customer.id"
            :value="customer.id"
          >
            {{ customer.name }}
          </option>
        </select>

        <input
          v-model="createOrderForm.status"
          class="neo-input"
          placeholder="Status"
        />

        <input
          v-model.number="createOrderForm.discountTotal"
          class="neo-input"
          type="number"
          step="0.01"
          min="0"
          placeholder="Desconto"
        />

        <input
          v-model="createOrderForm.note"
          class="neo-input"
          placeholder="Observacao"
        />

        <button
          class="neo-button primary"
          :disabled="!canCreateOrder"
          @click="createOrder"
        >
          Criar pedido
        </button>
      </div>
    </div>

    <p v-if="errorMessage" class="neo-alert danger">{{ errorMessage }}</p>
    <p v-if="loading" class="loading-note">Carregando pedidos...</p>

    <div class="neo-card table-card" v-if="!loading">
      <h2>Pedidos</h2>
      <div v-if="orders.length > 0" class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Cliente</th>
              <th>Status</th>
              <th>Subtotal</th>
              <th>Desconto</th>
              <th>Total</th>
              <th>Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in orders" :key="order.id">
              <td>{{ order.id }}</td>
              <td>{{ customerName(order.customerId) }}</td>
              <td>{{ order.status }}</td>
              <td>{{ Number(order.subtotal).toFixed(2) }}</td>
              <td>{{ Number(order.discountTotal).toFixed(2) }}</td>
              <td>{{ Number(order.total).toFixed(2) }}</td>
              <td>
                <button
                  class="neo-button ghost"
                  @click="selectedOrderId = order.id"
                >
                  Selecionar
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-else class="empty-state">
        <strong>Nenhum pedido cadastrado</strong>
      </div>
    </div>

    <div class="neo-card form-card" v-if="selectedOrder">
      <h2>Itens do pedido #{{ selectedOrder.id }}</h2>
      <div class="form-grid order-form-grid">
        <select v-model.number="addItemForm.productId" class="neo-input">
          <option :value="null">Selecione produto</option>
          <option
            v-for="product in products"
            :key="product.id"
            :value="product.id"
          >
            {{ product.name }} (estoque: {{ product.stockCurrent }})
          </option>
        </select>
        <input
          v-model.number="addItemForm.quantity"
          class="neo-input"
          type="number"
          min="1"
          step="1"
        />
        <button
          class="neo-button primary"
          :disabled="!canAddItem"
          @click="addItem"
        >
          Adicionar item
        </button>
      </div>

      <div class="table-wrap" v-if="selectedOrder.items.length > 0">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Produto</th>
              <th>Qtd</th>
              <th>Unitario</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in selectedOrder.items" :key="item.id">
              <td>{{ item.id }}</td>
              <td>{{ productName(item.productId) }}</td>
              <td>{{ item.quantity }}</td>
              <td>{{ Number(item.unitPrice).toFixed(2) }}</td>
              <td>{{ Number(item.lineTotal).toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
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

.order-form-grid {
  grid-template-columns: repeat(5, minmax(0, 1fr));
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

.neo-button.ghost {
  border-color: var(--line);
  background: transparent;
  color: var(--text-soft);
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

@media (max-width: 1100px) {
  .order-form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
