import { getSteamImportGames, getSteamImportWishlist, useGetSteamAuthConnect, usePostGamesBatchFollow, getGetUsersMeFollowedGamesQueryKey } from '@/api/generated'
import { useAuthStore } from '@/stores/auth'
import { useMutation } from '@tanstack/vue-query'
import { useQueryClient } from '@tanstack/vue-query'
import { computed, ref } from 'vue'
import { useToast } from 'openvue'
import type { SteamGameDto } from '@/api/generated/models'

export function useSteamService() {
  const authStore = useAuthStore()
  const queryClient = useQueryClient()
  const toast = useToast()
  const isSteamConnected = computed(() => authStore.isSteamConnected)

  const { data: steamUrl, isPending } = useGetSteamAuthConnect({
    query: { enabled: true }
  })

  const handleSteamConnect = () => {
    if (!steamUrl.value) return
    window.location.href = steamUrl.value
  }

  const importedGames = ref<SteamGameDto[]>([])
  const showImportModal = ref(false)
  const sortBy = ref<'playtime' | 'name' | 'lastPlayed'>('playtime')
  const sortDesc = ref(true)

  const sortedGames = computed(() => {
    const games = [...importedGames.value]
    return games.sort((a, b) => {
      let aVal: any, bVal: any
      if (sortBy.value === 'playtime') {
        aVal = Number(a.playtimeForeverMinutes) ?? 0
        bVal = Number(b.playtimeForeverMinutes) ?? 0
      } else if (sortBy.value === 'name') {
        aVal = (a.title ?? '').toLowerCase()
        bVal = (b.title ?? '').toLowerCase()
      } else {
        aVal = new Date(a.lastPlayed ?? '').getTime()
        bVal = new Date(b.lastPlayed ?? '').getTime()
      }
      if (sortDesc.value) return bVal - aVal > 0 ? 1 : bVal - aVal < 0 ? -1 : 0
      return aVal - bVal > 0 ? 1 : aVal - bVal < 0 ? -1 : 0
    })
  })

  const importWishlistMutation = useMutation({
    mutationFn: () => getSteamImportWishlist(),
    onSuccess: (data) => {
      const count = data?.importedCount ?? 0
      const total = data?.allCount ?? 0
      toast.add({
        severity: 'success',
        summary: 'Wishlist imported',
        detail: `Imported ${count} of ${total} games from Steam wishlist`,
        life: 5000
      })
    },
    onError: () => {
      toast.add({
        severity: 'error',
        summary: 'Wishlist import failed',
        detail: 'Failed to import wishlist from Steam',
        life: 5000
      })
    }
  })

  const importGamesMutation = useMutation({
    mutationFn: () => getSteamImportGames(),
    onSuccess: (data) => {
      importedGames.value = data?.games ?? []
      showImportModal.value = importedGames.value.length > 0
      const count = data?.found ?? 0
      const requested = data?.requested ?? 0
      toast.add({
        severity: 'success',
        summary: 'Games imported',
        detail: `Found ${count} of ${requested} games from Steam`,
        life: 5000
      })
    },
    onError: () => {
      toast.add({
        severity: 'error',
        summary: 'Games import failed',
        detail: 'Failed to import games from Steam',
        life: 5000
      })
    }
  })

  const batchFollowMutation = usePostGamesBatchFollow({
    mutation: {
      onSuccess: () => {
        toast.add({
          severity: 'success',
          summary: 'Following games',
          detail: 'Games have been added to followed',
          life: 5000
        })
        queryClient.invalidateQueries({ queryKey: getGetUsersMeFollowedGamesQueryKey() })
      },
      onError: () => {
        toast.add({
          severity: 'error',
          summary: 'Follow failed',
          detail: 'Failed to add games to followed',
          life: 5000
        })
      }
    }
  })

  const importSteamWishlist = () => {
    importWishlistMutation.mutate()
  }

  const importSteamGames = () => {
    importGamesMutation.mutate()
  }

  const batchFollowGames = (gameIds: (number | string)[]) => {
    batchFollowMutation.mutate({ data: { ids: gameIds } })
  }

  return {
    isSteamConnected,
    steamUrl,
    isPending,
    handleSteamConnect,
    importSteamWishlist,
    importSteamGames,
    batchFollowGames,
    importedGames,
    showImportModal,
    sortedGames,
    sortBy,
    sortDesc,
    isImportingWishlist: computed(() => importWishlistMutation.isPending.value),
    isImportingGames: computed(() => importGamesMutation.isPending.value),
    isBatchFollowing: computed(() => batchFollowMutation.isPending.value)
  }
}
