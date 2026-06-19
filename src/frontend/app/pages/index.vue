<template>
  <section class="screen">
    <p class="idle-hint">Selecione uma opção no menu.</p>
  </section>
</template>

<style scoped>
.screen {
  min-height: clamp(300px, 62vh, 680px);
  display: grid;
  place-items: center;
}

.idle-hint {
  margin: 0;
  color: var(--text-soft);
  font-weight: 600;
  font-size: 0.96rem;
  letter-spacing: 0.01em;
}
</style>
