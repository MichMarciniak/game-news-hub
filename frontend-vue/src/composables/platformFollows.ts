import { getGetUsersMePlatformsFollowedQueryKey, useDeletePlatformGroupsGroupIdFollow, useGetUsersMePlatformsFollowed, usePostPlatformGroupsGroupIdFollow,  } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedPlatforms() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: platformsData } = useGetUsersMePlatformsFollowed({query: {enabled}})

  const followedPlatformIds = computed(() => new Set(platformsData.value?.map(g => String(g.id)) ?? []))

  const followPlatformMutation = usePostPlatformGroupsGroupIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMePlatformsFollowedQueryKey() })
      }
    }
  })

  const unfollowPlatformMutation = useDeletePlatformGroupsGroupIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMePlatformsFollowedQueryKey() })
      }
    }
  })

  const toggleFollowPlatform = (platformId: string | number) => {
    const idStr = String(platformId)
    if (followedPlatformIds.value.has(idStr)) {
      unfollowPlatformMutation.mutate({groupId: idStr})
    } else {
      followPlatformMutation.mutate({groupId: idStr})
    }
  }


  return {followedPlatformIds, toggleFollowPlatform}
}