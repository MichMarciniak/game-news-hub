import { getGetUsersMeGamesFollowedQueryKey, useDeleteGameGameIdFollow, useGetUsersMeGamesFollowed, usePostGameGameIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedGames() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: gamesData } = useGetUsersMeGamesFollowed({query: {enabled}})

  const followedGameIds = computed(() => new Set(gamesData.value?.map(g => String(g.id)) ?? []))

  const followGameMutation = usePostGameGameIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeGamesFollowedQueryKey() })
      }
    }
  })

  const unfollowGameMutation = useDeleteGameGameIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeGamesFollowedQueryKey() })
      }
    }
  })

  const toggleFollowGame = (gameId: string | number) => {
    const idStr = String(gameId)
    if (followedGameIds.value.has(idStr)) {
      unfollowGameMutation.mutate({gameId: idStr})
    } else {
      followGameMutation.mutate({gameId: idStr})
    }
  }


  return {followedGameIds, toggleFollowGame}
}