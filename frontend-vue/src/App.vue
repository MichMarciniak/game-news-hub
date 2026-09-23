<script setup lang="ts">
import { RouterView } from 'vue-router'
import Footer from './components/Footer.vue';
</script>

<template>
  <Header />

  <main class="flex-1 flex justify-center p-4">
    <Toast />
  <RouterView />
  </main>
  <Footer />
</template>

