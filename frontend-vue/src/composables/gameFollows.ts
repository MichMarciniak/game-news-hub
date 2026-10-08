import { getGetUsersMeFollowedGamesQueryKey, useDeleteGamesIdFollow, useGetUsersMeFollowedGames, usePostGamesIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedGames() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  // w gamesearch jest skeleton or gameCard - przez to jest refetch za każdym searchem - dlatego infinity
  const {data: gamesData } = useGetUsersMeFollowedGames({query: {enabled, staleTime: Infinity}})

  const followedGameIds = computed(() => new Set(gamesData.value?.map(g => String(g.id)) ?? []))

  const followGameMutation = usePostGamesIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedGamesQueryKey() })
      }
    }
  })

  const unfollowGameMutation = useDeleteGamesIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedGamesQueryKey() })
      }
    }
  })

  const toggleFollowGame = (gameId: string | number) => {
    const idStr = String(gameId)
    if (followedGameIds.value.has(idStr)) {
      unfollowGameMutation.mutate({id: idStr})
    } else {
      followGameMutation.mutate({id: idStr})
    }
  }


  return {followedGameIds, toggleFollowGame}
}