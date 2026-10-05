import { getGetUsersMeFollowedGenresQueryKey, useDeleteGenresIdFollow, useGetUsersMeFollowedGenres, usePostGenresIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedGenres() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: genresData } = useGetUsersMeFollowedGenres({query: {enabled}})

  const followedGenreIds = computed(() => new Set(genresData.value?.map(g => String(g.id)) ?? []))

  const followGenreMutation = usePostGenresIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedGenresQueryKey() })
      }
    }
  })

  const unfollowGenreMutation = useDeleteGenresIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedGenresQueryKey() })
      }
    }
  })

  const toggleFollowGenre = (genreId: string | number) => {
    const idStr = String(genreId)
    if (followedGenreIds.value.has(idStr)) {
      unfollowGenreMutation.mutate({id: idStr})
    } else {
      followGenreMutation.mutate({id: idStr})
    }
  }


  return {followedGenreIds, toggleFollowGenre}
}