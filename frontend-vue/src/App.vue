<script setup lang="ts">
import { RouterView } from 'vue-router'
import Footer from './components/Footer.vue';
</script>

<template>
  <Header />

  <main class="flex-1 flex justify-center py-4 sm:px-4 not-sm:px-1">
    <Toast />
    <RouterView />
  </main>
  <Footer />
</template>

