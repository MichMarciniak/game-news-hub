import { useGetUsersMeWishlist, usePostUsersMeWishlistGamesGameId, useDeleteUsersMeWishlistGamesGameId } from '@/api/generated'
import { computed } from 'vue'

export function useWishlistService() {
  const { data: wishlist, isPending } = useGetUsersMeWishlist()

  const { mutate: addToWishlist } = usePostUsersMeWishlistGamesGameId()
  const { mutate: removeFromWishlist } = useDeleteUsersMeWishlistGamesGameId()

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
