<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <Header />

  <main class="flex-1 flex justify-center p-4">
    <Toast />
  <RouterView />
  </main>
</template>

