<template>
  <div class="layout">
    <NuxtRouteAnnouncer />
    <header class="topbar">
      <h1>ZDZCode Inventory</h1>
      <nav>
        <NuxtLink to="/categorias">Categories</NuxtLink>
        <NuxtLink to="/produtos">Products</NuxtLink>
      </nav>
    </header>

    <main>
      <NuxtPage />
    </main>
  </div>
</template>

<style scoped>
.layout {
  min-height: 100vh;
  background: linear-gradient(180deg, #f8fafc 0%, #eef2ff 100%);
  color: #0f172a;
}

.topbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  padding: 1rem 1.2rem;
  border-bottom: 1px solid #cbd5e1;
  background: #ffffff;
}

.topbar h1 {
  margin: 0;
  font-size: 1.2rem;
}

nav {
  display: flex;
  gap: 1rem;
}

a {
  color: #1d4ed8;
  text-decoration: none;
  font-weight: 600;
}

main {
  padding: 1rem;
}
</style>
