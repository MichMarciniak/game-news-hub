<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import GoogleActionsMenu from './GoogleActionsMenu.vue';
import GoogleConnectButton from './GoogleConnectButton.vue';
import { useGoogleService } from '../services/useGoogleService';
import { useAuthStore } from '@/stores/auth';

const route = useRoute()
const hasGoogleQuery = computed(() => 
  route.query.google !== undefined || route.query.code !== undefined
)

const authStore = useAuthStore()
const { isGoogleConnected, handleGoogleLogin } = useGoogleService()

// Auto-refresh user data when coming back from Google callback with query params
onMounted(() => {
  if (hasGoogleQuery.value) {
    console.log('Returned from Google OAuth')
    authStore.initialize()
  }
})
</script>

<template>
  <div class="rounded-border border-2 p-2 border-surface w-fit">
    <h2 class="font-semibold text-2xl">Google</h2>

    <GoogleActionsMenu v-if="isGoogleConnected" />

    <GoogleConnectButton v-else @connect="handleGoogleLogin" />
  </div>
</template>