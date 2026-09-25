<script setup lang="ts">
import PasswordChange from '@/features/auth/PasswordChange.vue';
import Settings from '@/features/profile/Settings.vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';


const authStore = useAuthStore();
const {user, isAuthenticated } = storeToRefs(authStore)



</script>


<template>
  <div>
    <h2 class="text-2xl font-semibold">{{ user?.username }}</h2>
    <Settings />
    <PasswordChange />

  </div>
 
</template>