import { getGetUsersMeGenresFollowedQueryKey, useDeleteGenreGenreIdFollow, useGetUsersMeGenresFollowed, usePostGenreGenreIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedGenres() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: genresData } = useGetUsersMeGenresFollowed({query: {enabled}})

  const followedGenreIds = computed(() => new Set(genresData.value?.map(g => String(g.id)) ?? []))

  const followGenreMutation = usePostGenreGenreIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeGenresFollowedQueryKey() })
      }
    }
  })

  const unfollowGenreMutation = useDeleteGenreGenreIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeGenresFollowedQueryKey() })
      }
    }
  })

  const toggleFollowGenre = (genreId: string | number) => {
    const idStr = String(genreId)
    if (followedGenreIds.value.has(idStr)) {
      unfollowGenreMutation.mutate({genreId: idStr})
    } else {
      followGenreMutation.mutate({genreId: idStr})
    }
  }


  return {followedGenreIds, toggleFollowGenre}
}