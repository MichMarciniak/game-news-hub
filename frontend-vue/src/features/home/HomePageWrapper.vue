<script setup lang="ts">
import LoggedInPage from '@/features/home/LoggedInPage.vue';
import NotLoggedInPage from '@/features/home/NotLoggedInPage.vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';

const authStore = useAuthStore()
const {isAuthenticated} = storeToRefs(authStore)

</script>


<template>
  <LoggedInPage v-if="isAuthenticated"></LoggedInPage>
  <NotLoggedInPage v-else></NotLoggedInPage>

</template>
