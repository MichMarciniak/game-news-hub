import { getGetUsersMeFollowedPlatformGroupsQueryKey, useDeletePlatformsGroupsIdFollow, useGetUsersMeFollowedPlatformGroups, usePostPlatformsGroupsIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedPlatforms() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: platformsData } = useGetUsersMeFollowedPlatformGroups({query: {enabled}})

  const followedPlatformIds = computed(() => new Set(platformsData.value?.map(g => String(g.id)) ?? []))

  const followPlatformMutation = usePostPlatformsGroupsIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedPlatformGroupsQueryKey() })
      }
    }
  })

  const unfollowPlatformMutation = useDeletePlatformsGroupsIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedPlatformGroupsQueryKey() })
      }
    }
  })

  const toggleFollowPlatform = (platformId: string | number) => {
    const idStr = String(platformId)
    if (followedPlatformIds.value.has(idStr)) {
      unfollowPlatformMutation.mutate({id: idStr})
    } else {
      followPlatformMutation.mutate({id: idStr})
    }
  }


  return {followedPlatformIds, toggleFollowPlatform}
}