import { createFollowComposable } from "./useFollowedItems";
import { useGetUsersMeFollowedPlatformGroups, usePostPlatformsGroupsIdFollow, useDeletePlatformsGroupsIdFollow, getGetUsersMeFollowedPlatformGroupsQueryKey } from "@/api/generated";

const useFollowed = createFollowComposable({
  useGetFollowed: () => useGetUsersMeFollowedPlatformGroups(),
  usePostFollow: (options) => usePostPlatformsGroupsIdFollow(options),
  useUnfollow: (options) => useDeletePlatformsGroupsIdFollow(options),
  getQueryKey: getGetUsersMeFollowedPlatformGroupsQueryKey
});

export function useFollowedPlatforms() {
  const { followedIds, toggleFollow } = useFollowed();
  return { followedPlatformIds: followedIds, toggleFollowPlatform: toggleFollow };
}
