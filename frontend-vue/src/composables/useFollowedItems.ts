import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";

export interface FollowComposableConfig {
  useGetFollowed: (options?: any) => any;
  usePostFollow: (options: any) => any;
  useUnfollow: (options: any) => any;
  getQueryKey: () => readonly unknown[];
  staleTime?: number;
}

export function createFollowComposable(config: FollowComposableConfig) {
  return function useFollowed() {
    const queryClient = useQueryClient()
    const auth = useAuthStore();
    const enabled = computed(() => auth.isAuthenticated)

    const { data: itemsData } = config.useGetFollowed({
      query: { enabled, staleTime: config.staleTime }
    })

    const followedIds = computed(() => new Set(itemsData.value?.map((i: any) => String(i.id)) ?? []))

    const followMutation = config.usePostFollow({
      mutation: {
        onSuccess: () => {
          queryClient.invalidateQueries({ queryKey: config.getQueryKey() })
        }
      }
    })

    const unfollowMutation = config.useUnfollow({
      mutation: {
        onSuccess: () => {
          queryClient.invalidateQueries({ queryKey: config.getQueryKey() })
        }
      }
    })

    const toggleFollow = (itemId: string | number) => {
      const idStr = String(itemId)
      if (followedIds.value.has(idStr)) {
        unfollowMutation.mutate({ id: idStr })
      } else {
        followMutation.mutate({ id: idStr })
      }
    }

    return { followedIds, toggleFollow }
  }
}
