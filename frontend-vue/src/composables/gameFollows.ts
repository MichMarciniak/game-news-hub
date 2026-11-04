import { createFollowComposable } from "./useFollowedItems";
import { useGetUsersMeFollowedGames, usePostGamesIdFollow, useDeleteGamesIdFollow, getGetUsersMeFollowedGamesQueryKey } from "@/api/generated";

const useFollowed = createFollowComposable({
  useGetFollowed: () => useGetUsersMeFollowedGames(),
  usePostFollow: (options) => usePostGamesIdFollow(options),
  useUnfollow: (options) => useDeleteGamesIdFollow(options),
  getQueryKey: getGetUsersMeFollowedGamesQueryKey,
  staleTime: Infinity
});

export function useFollowedGames() {
  const { followedIds, toggleFollow } = useFollowed();
  return { followedGameIds: followedIds, toggleFollowGame: toggleFollow };
}
