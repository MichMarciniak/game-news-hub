<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import SteamActionsMenu from './SteamActionsMenu.vue';
import SteamConnectButton from './SteamConnectButton.vue';
import { useSteamService } from '@/features/profile/services/useSteamService';

const route = useRoute()
const hasSteamQuery = computed(() => 
  route.query.steam !== undefined || route.query.code !== undefined
)

const { steamId, steamUrl, isPending, handleSteamConnect } = useSteamService()

// Auto-redirect when coming back from Steam callback with query params
onMounted(() => {
  if (hasSteamQuery.value && steamUrl.value) {
    handleSteamConnect()
  }
})
</script>

<template>
  <div class="rounded-border border-2 p-2 border-surface w-fit">
    <h2 class="font-semibold text-2xl">Steam</h2>

    <SteamActionsMenu v-if="steamId" />

    <SteamConnectButton v-else @connect="handleSteamConnect" />
  </div>
</template>