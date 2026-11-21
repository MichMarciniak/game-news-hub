import { useGetUsersMeWishlist, usePostUsersMeWishlistGamesGameId, useDeleteUsersMeWishlistGamesGameId, getGetUsersMeWishlistQueryKey } from '@/api/generated'
import { useQueryClient } from '@tanstack/vue-query'
import { computed } from 'vue'

export function useWishlistService() {
  const queryClient = useQueryClient()

  const { data: wishlist, isPending } = useGetUsersMeWishlist()

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

  const wishlistIds = computed(() => new Set(wishlist.value?.map(g => String(g.id)) ?? []))

  const isInWishlist = (gameId: string | number) => wishlistIds.value.has(String(gameId))

  return {
    wishlist,
    isPending,
    addToWishlist,
    removeFromWishlist,
    isInWishlist
  }
}
