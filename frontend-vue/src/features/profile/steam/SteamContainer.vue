<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import SteamActionsMenu from './SteamActionsMenu.vue';
import SteamConnectButton from './SteamConnectButton.vue';
import SteamImportModal from './SteamImportModal.vue';
import { useSteamService } from '@/features/profile/services/useSteamService';
import { useAuthStore } from '@/stores/auth';
import { useToast } from 'openvue'

const route = useRoute()
const router = useRouter()
const hasSteamQuery = computed(() => 
  route.query.steam !== undefined || route.query.code !== undefined
)

const authStore = useAuthStore()
const toast = useToast()
const { isSteamConnected, steamUrl, isPending, handleSteamConnect, importSteamWishlist, importSteamGames, batchFollowGames, importedGames, showImportModal, sortedGames, sortBy, sortDesc, isImportingWishlist, isImportingGames, isBatchFollowing } = useSteamService()

// Auto-refresh user data when coming back from Steam callback with query params
onMounted(() => {
  if (!hasSteamQuery.value) return

  const steamResult = route.query.steam as string | undefined

  if (steamResult === 'connected') {
    authStore.initialize()
    return
  }

  if (steamResult === 'error') {
    const errorCode = route.query.code as string | undefined
    const errorMessage = route.query.message as string | undefined
    toast.add({
      severity: 'error',
      summary: 'Steam connection failed',
      detail: errorMessage ?? errorCode ?? 'Failed to connect Steam account',
      life: 5000
    })
    router.replace({ query: { steam: undefined, code: undefined, message: undefined } })
    return
  }

  if (steamUrl.value) {
    handleSteamConnect()
  }
})

const setSort = (key: 'playtime' | 'name' | 'lastPlayed') => {
  if (sortBy.value === key) {
    sortDesc.value = !sortDesc.value
  } else {
    sortBy.value = key
    sortDesc.value = true
  }
}

const handleBatchFollow = () => {
  const ids = sortedGames.value.map(g => g.id).filter((id): id is number => id != null)
  if (ids.length > 0) {
    batchFollowGames(ids)
  }
}
</script>

<template>
  <div class="rounded-border border-2 p-2 border-surface w-fit">
    <h2 class="font-semibold text-2xl mb-2">Steam</h2>

    <SteamActionsMenu v-if="isSteamConnected" @import-wishlist="importSteamWishlist" @import-owned="importSteamGames" />

    <SteamImportModal
      v-if="showImportModal"
      :visible="showImportModal"
      :games="sortedGames"
      :is-importing="isBatchFollowing"
      @update:visible="showImportModal = $event"
      @batch-follow="batchFollowGames"
    />

    <SteamConnectButton v-else-if="!isSteamConnected" @connect="handleSteamConnect" />
  </div>
</template>
