<script setup lang="ts">
import Settings from '@/features/profile/Settings.vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';


const authStore = useAuthStore();
const {user, isAuthenticated } = storeToRefs(authStore)



</script>


<template>
  <h2 class="text-2xl font-semibold">{{ user?.username }}</h2>
  <Settings />
 
</template>