import { useGetUsersMeWishlist, usePostUsersMeWishlistGamesGameId, useDeleteUsersMeWishlistGamesGameId, getGetUsersMeWishlistQueryKey } from '@/api/generated'
import { useQueryClient } from '@tanstack/vue-query'
import { computed } from 'vue'

export function useWishlist() {
  const queryClient = useQueryClient()

  const { data: wishlistData } = useGetUsersMeWishlist({
    query: { staleTime: Infinity }
  })

  const wishlistIds = computed(() => new Set(wishlistData.value?.map((g: any) => String(g.id)) ?? []))

  const { mutate: addToWishlist } = usePostUsersMeWishlistGamesGameId({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: getGetUsersMeWishlistQueryKey() })
      }
    }
  })

  const { mutate: removeFromWishlist } = useDeleteUsersMeWishlistGamesGameId({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: getGetUsersMeWishlistQueryKey() })
      }
    }
  })

  const toggleWishlist = (gameId: string | number) => {
    const idStr = String(gameId)
    if (wishlistIds.value.has(idStr)) {
      removeFromWishlist({ gameId: idStr })
    } else {
      addToWishlist({ gameId: idStr })
    }
  }

  const isWishlisted = (gameId: string | number) => wishlistIds.value.has(String(gameId))

  return {
    wishlistIds,
    toggleWishlist,
    isWishlisted
  }
}
