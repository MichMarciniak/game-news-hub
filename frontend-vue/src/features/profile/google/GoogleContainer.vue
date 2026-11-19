<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import GoogleActionsMenu from './GoogleActionsMenu.vue';
import GoogleConnectButton from './GoogleConnectButton.vue';
import { useGoogleService } from '../services/useGoogleService';

const route = useRoute()
const hasGoogleQuery = computed(() => 
  route.query.google !== undefined || route.query.code !== undefined
)

const { googleId, googleLoginUrl, handleGoogleLogin } = useGoogleService()

// Auto-refresh user data when coming back from Google callback with query params
onMounted(() => {
  if (hasGoogleQuery.value) {
    // The auth store will auto-refresh on next navigation or we can manually trigger
    // For now, just log - the user data will be refreshed by the auth store
    console.log('Returned from Google OAuth')
  }
})
</script>

<template>
  <div class="rounded-border border-2 p-2 border-surface w-fit">
    <h2 class="font-semibold text-2xl">Google</h2>

    <GoogleActionsMenu v-if="googleId" />

    <GoogleConnectButton v-else @connect="handleGoogleLogin" />
  </div>
</template>