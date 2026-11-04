import { createFollowComposable } from "./useFollowedItems";
import { useGetUsersMeFollowedGenres, usePostGenresIdFollow, useDeleteGenresIdFollow, getGetUsersMeFollowedGenresQueryKey } from "@/api/generated";

const useFollowed = createFollowComposable({
  useGetFollowed: () => useGetUsersMeFollowedGenres(),
  usePostFollow: (options) => usePostGenresIdFollow(options),
  useUnfollow: (options) => useDeleteGenresIdFollow(options),
  getQueryKey: getGetUsersMeFollowedGenresQueryKey
});

export function useFollowedGenres() {
  const { followedIds, toggleFollow } = useFollowed();
  return { followedGenreIds: followedIds, toggleFollowGenre: toggleFollow };
}
