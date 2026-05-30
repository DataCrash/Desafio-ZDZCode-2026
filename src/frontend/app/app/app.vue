<script setup lang="ts">
type ThemeMode = "light" | "dark";

const THEME_KEY = "zdz-theme";

const theme = ref<ThemeMode>("dark");
const hasUserPreference = ref(false);

function detectSystemTheme(): ThemeMode {
  if (globalThis.window === undefined) return "dark";
  return globalThis.matchMedia("(prefers-color-scheme: dark)").matches
    ? "dark"
    : "light";
}

function toggleTheme() {
  theme.value = theme.value === "dark" ? "light" : "dark";
  hasUserPreference.value = true;
  localStorage.setItem(THEME_KEY, theme.value);
}

onMounted(() => {
  const stored = globalThis.localStorage.getItem(THEME_KEY);
  if (stored === "dark" || stored === "light") {
    theme.value = stored;
    hasUserPreference.value = true;
  } else {
    theme.value = detectSystemTheme();
  }

  const media = globalThis.matchMedia("(prefers-color-scheme: dark)");
  const onChange = (event: MediaQueryListEvent) => {
    if (hasUserPreference.value) return;
    theme.value = event.matches ? "dark" : "light";
  };

  if (media.addEventListener) {
    media.addEventListener("change", onChange);
    onBeforeUnmount(() => media.removeEventListener("change", onChange));
    return;
  }

  media.addListener(onChange);
  onBeforeUnmount(() => media.removeListener(onChange));
});
</script>

<template>
  <div class="app-shell" :data-theme="theme">
    <NuxtRouteAnnouncer />
    <header class="topbar">
      <div class="brand-wrap">
        <strong>ZDZCode Inventory</strong>
        <span class="brand-badge">Neo Clean</span>
      </div>

      <nav class="topbar-nav">
        <NuxtLink to="/categorias">Categories</NuxtLink>
        <NuxtLink to="/produtos">Products</NuxtLink>
      </nav>

      <button
        class="theme-toggle"
        type="button"
        :aria-label="
          theme === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro'
        "
        :title="theme === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro'"
        @click="toggleTheme"
      >
        {{ theme === "dark" ? "☀️" : "🌙" }}
      </button>
    </header>

    <main class="page-container">
      <NuxtPage />
    </main>
  </div>
</template>

<style>
@import url("https://fonts.googleapis.com/css2?family=Manrope:wght@400;500;600;700&family=Sora:wght@600;700&display=swap");

* {
  box-sizing: border-box;
}

html,
body,
#__nuxt {
  min-height: 100%;
}

body {
  margin: 0;
  font-family: "Manrope", "Segoe UI", sans-serif;
}

.app-shell {
  --bg-0: #f4f7fb;
  --bg-1: #ffffff;
  --bg-2: #eef4ff;
  --surface: rgba(255, 255, 255, 0.84);
  --surface-strong: #ffffff;
  --text: #111827;
  --text-soft: #4b5563;
  --line: rgba(15, 23, 42, 0.12);
  --accent: #0ea5a4;
  --accent-strong: #0f766e;
  --danger: #dc2626;
  --shadow: 0 14px 34px rgba(14, 18, 34, 0.08);
  --focus-ring: rgba(14, 165, 164, 0.35);
  --row-hover: rgba(14, 165, 164, 0.08);

  min-height: 100vh;
  color: var(--text);
  background:
    radial-gradient(
      circle at 18% -10%,
      rgba(14, 165, 164, 0.2),
      transparent 35%
    ),
    radial-gradient(
      circle at 88% 6%,
      rgba(15, 118, 110, 0.12),
      transparent 35%
    ),
    linear-gradient(180deg, var(--bg-0) 0%, var(--bg-2) 70%, var(--bg-1) 100%);
}

.app-shell[data-theme="dark"] {
  --bg-0: #0b1220;
  --bg-1: #111a2b;
  --bg-2: #0f1726;
  --surface: rgba(17, 26, 43, 0.85);
  --surface-strong: #131e33;
  --text: #e5e7eb;
  --text-soft: #94a3b8;
  --line: rgba(148, 163, 184, 0.2);
  --accent: #2dd4bf;
  --accent-strong: #14b8a6;
  --danger: #f87171;
  --shadow: 0 16px 36px rgba(3, 7, 18, 0.35);
  --focus-ring: rgba(45, 212, 191, 0.35);
  --row-hover: rgba(45, 212, 191, 0.1);
}

.topbar {
  position: sticky;
  top: 0;
  z-index: 40;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.9rem;
  padding: 0.8rem 1.1rem;
  border-bottom: 1px solid var(--line);
  backdrop-filter: blur(10px);
  background: color-mix(in srgb, var(--surface) 84%, transparent);
}

.brand-wrap {
  display: inline-flex;
  align-items: center;
  gap: 0.65rem;
}

.brand-wrap strong {
  font-family: "Sora", "Manrope", sans-serif;
  font-size: 0.98rem;
  letter-spacing: 0.01em;
}

.brand-badge {
  border: 1px solid var(--line);
  border-radius: 999px;
  padding: 0.18rem 0.52rem;
  font-size: 0.68rem;
  color: var(--text-soft);
}

.topbar-nav {
  display: flex;
  gap: 0.4rem;
}

.topbar-nav a {
  border-radius: 999px;
  padding: 0.36rem 0.65rem;
  color: var(--text-soft);
  text-decoration: none;
  font-weight: 700;
  font-size: 0.86rem;
  transition:
    background-color 120ms ease,
    color 120ms ease,
    transform 120ms ease;
}

.topbar-nav a:hover {
  background: color-mix(in srgb, var(--surface-strong) 74%, transparent);
  transform: translateY(-1px);
}

.topbar-nav a.router-link-active {
  color: var(--accent);
  background: color-mix(in srgb, var(--accent) 16%, transparent);
}

.theme-toggle {
  border: 1px solid var(--line);
  border-radius: 999px;
  background: color-mix(in srgb, var(--surface-strong) 80%, transparent);
  color: var(--text);
  font-family: inherit;
  font-size: 0.78rem;
  font-weight: 700;
  line-height: 1;
  padding: 0.48rem 0.72rem;
  cursor: pointer;
  transition:
    border-color 130ms ease,
    color 130ms ease,
    transform 130ms ease,
    box-shadow 130ms ease;
}

.theme-toggle:hover {
  border-color: color-mix(in srgb, var(--accent) 45%, var(--line));
  color: var(--accent);
  transform: translateY(-1px);
}

.theme-toggle:focus-visible,
.topbar-nav a:focus-visible {
  outline: none;
  box-shadow: 0 0 0 3px var(--focus-ring);
}

.page-container {
  width: min(1220px, 95vw);
  margin: 1.05rem auto 2rem;
  animation: page-enter 240ms ease;
}

@keyframes page-enter {
  from {
    opacity: 0;
    transform: translateY(4px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@media (prefers-reduced-motion: reduce) {
  .page-container,
  .theme-toggle,
  .topbar-nav a {
    animation: none;
    transition: none;
  }
}

@media (max-width: 880px) {
  .topbar {
    flex-wrap: wrap;
  }

  .topbar-nav {
    order: 3;
    width: 100%;
  }

  .topbar-nav a {
    flex: 1;
    text-align: center;
  }
}
</style>
