import { useGetSteamAuthConnect } from '@/api/generated'
import { useAuthStore } from '@/stores/auth'
import { computed } from 'vue'

export function useSteamService() {
  const authStore = useAuthStore()
  const steamId = computed(() => authStore.user?.steamId ?? null)

  const { data: steamUrl, isPending } = useGetSteamAuthConnect({
    query: { enabled: true }
  })

  const handleSteamConnect = () => {
    if (!steamUrl.value) return
    window.location.href = steamUrl.value
  }

  return {
    steamId,
    steamUrl,
    isPending,
    handleSteamConnect
  }
}
