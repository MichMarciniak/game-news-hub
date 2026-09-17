<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <Header />

  <main class="flex-1 flex items-center justify-center p-4">
    <RouterView />
  </main>
</template>

