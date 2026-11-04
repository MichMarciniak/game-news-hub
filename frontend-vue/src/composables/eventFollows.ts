import { createFollowComposable } from "./useFollowedItems";
import { useGetUsersMeFollowedEvents, usePostEventsIdFollow, useDeleteEventsIdFollow, getGetUsersMeFollowedEventsQueryKey } from "@/api/generated";

const useFollowed = createFollowComposable({
  useGetFollowed: () => useGetUsersMeFollowedEvents(),
  usePostFollow: (options) => usePostEventsIdFollow(options),
  useUnfollow: (options) => useDeleteEventsIdFollow(options),
  getQueryKey: getGetUsersMeFollowedEventsQueryKey,
  staleTime: Infinity
});

export function useFollowedEvents() {
  const { followedIds, toggleFollow } = useFollowed();
  return { followedEventIds: followedIds, toggleFollowEvent: toggleFollow };
}
