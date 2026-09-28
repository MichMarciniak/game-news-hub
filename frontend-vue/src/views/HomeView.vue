<script setup lang="ts">
import LoggedIn from '@/features/home/LoggedIn.vue';
import NotLoggedIn from '@/features/home/NotLoggedIn.vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';

const authStore = useAuthStore()
const {isAuthenticated} = storeToRefs(authStore)

</script>


<template>
  <LoggedIn v-if="isAuthenticated"></LoggedIn>
  <NotLoggedIn v-else></NotLoggedIn>

</template>
