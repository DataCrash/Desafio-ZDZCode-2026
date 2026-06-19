<script setup lang="ts">
type Product = {
  id: number;
  name: string;
  sku?: string | null;
  stockCurrent: number;
  isActive: boolean;
};

type Movement = {
  id: number;
  productId: number;
  movementType: string;
  quantity: number;
  reason?: string | null;
  createdAt: string;
};

const config = useRuntimeConfig();
const apiBase = config.public.apiBase as string;

const products = ref<Product[]>([]);
const movements = ref<Movement[]>([]);
const loading = ref(false);
const errorMessage = ref("");
const isCreateOpen = ref(false);

const form = reactive({
  productId: null as number | null,
  movementType: "entrada",
  quantity: 1,
  reason: "",
});

const canCreate = computed(
  () => Number(form.productId) > 0 && form.quantity > 0,
);

async function loadData() {
  loading.value = true;
  errorMessage.value = "";
  try {
    const [productResult, movementResult] = await Promise.all([
      $fetch<Product[]>(`${apiBase}/api/produtos`),
      $fetch<Movement[]>(`${apiBase}/api/estoque/movimentacoes`),
    ]);

    products.value = productResult;
    movements.value = movementResult;
  } catch {
    errorMessage.value = "Falha ao carregar dados de estoque.";
  } finally {
    loading.value = false;
  }
}

function productName(productId: number) {
  return (
    products.value.find((item) => item.id === productId)?.name ||
    `#${productId}`
  );
}

async function createMovement() {
  if (!canCreate.value) return;

  errorMessage.value = "";
  try {
    const created = await $fetch<Movement>(
      `${apiBase}/api/estoque/movimentacoes`,
      {
        method: "POST",
        body: {
          productId: Number(form.productId),
          movementType: form.movementType,
          quantity: Number(form.quantity),
          reason: form.reason.trim() || null,
        },
      },
    );

    movements.value = [created, ...movements.value];
    await loadData();
  } catch (error: any) {
    errorMessage.value =
      error?.data?.message || "Falha ao registrar movimentacao.";
  }
}

onMounted(loadData);
</script>

<template>
  <section class="screen">
    <header class="screen-header">
      <h1>Estoque</h1>
      <p>Movimentações de entrada e saída com bloqueio de estoque negativo.</p>
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
            Nova movimentação
          </button>
          <span v-else>Nova movimentação</span>
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
      <div v-show="isCreateOpen" class="form-grid stock-grid">
        <select
          v-model.number="form.productId"
          class="neo-input required-field"
        >
          <option :value="null">Selecione produto</option>
          <option
            v-for="product in products"
            :key="product.id"
            :value="product.id"
          >
            {{ product.name }} (atual: {{ product.stockCurrent }})
          </option>
        </select>

        <select v-model="form.movementType" class="neo-input required-field">
          <option value="entrada">Entrada</option>
          <option value="saida">Saída</option>
        </select>

        <input
          v-model.number="form.quantity"
          class="neo-input required-field"
          type="number"
          min="1"
          step="1"
          placeholder="Quantidade *"
        />

        <input
          v-model="form.reason"
          class="neo-input"
          placeholder="Motivo (opcional)"
        />

        <button
          class="neo-button primary"
          :disabled="!canCreate"
          aria-label="Registrar movimentação"
          @click="createMovement"
        >
          <span aria-hidden="true">💾</span>
          <span class="sr-only">Registrar movimentação</span>
        </button>
      </div>
    </div>

    <p v-if="errorMessage" class="neo-alert danger">{{ errorMessage }}</p>
    <p v-if="loading" class="loading-note">Carregando estoque...</p>

    <div class="neo-card table-card" v-if="!loading">
      <h2>Saldos atuais</h2>
      <div class="table-wrap" v-if="products.length > 0">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Produto</th>
              <th>SKU</th>
              <th>Estoque</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in products" :key="product.id">
              <td>{{ product.id }}</td>
              <td>{{ product.name }}</td>
              <td>{{ product.sku || "-" }}</td>
              <td>{{ product.stockCurrent }}</td>
              <td>{{ product.isActive ? "Ativo" : "Inativo" }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <h2>Histórico de movimentações</h2>
      <div class="table-wrap" v-if="movements.length > 0">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Produto</th>
              <th>Tipo</th>
              <th>Quantidade</th>
              <th>Motivo</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="movement in movements" :key="movement.id">
              <td>{{ movement.id }}</td>
              <td>{{ productName(movement.productId) }}</td>
              <td>{{ movement.movementType }}</td>
              <td>{{ movement.quantity }}</td>
              <td>{{ movement.reason || "-" }}</td>
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
}

.stock-grid {
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

.required-field {
  border-left: 3px solid var(--accent);
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
  margin-bottom: 0.8rem;
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

@media (max-width: 1100px) {
  .stock-grid {
    grid-template-columns: 1fr;
  }
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
</style>
