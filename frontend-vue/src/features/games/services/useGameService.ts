import { useGetGamesId } from '@/api/generated'
import { useFollowedGames } from '@/composables/gameFollows'
import { computed } from 'vue'

export function useGameService(gameId: string | number | undefined) {
  const { data: gameResponse, isPending } = useGetGamesId(computed(() => String(gameId)))
  const game = computed(() => gameResponse.value)

  const { followedGameIds, toggleFollowGame } = useFollowedGames()
  const isFollowed = computed(() => !!game.value && followedGameIds.value.has(String(game.value.id)))

  const formatGameType = (type?: string): string => {
    if (!type) return ''
    return type.replace(/([a-z])([A-Z])/g, '$1 $2')
  }

  return {
    game,
    isPending,
    isFollowed,
    toggleFollowGame,
    formatGameType
  }
}
