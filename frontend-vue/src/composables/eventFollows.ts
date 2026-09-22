import { getGetUsersMeEventsFollowedQueryKey, useDeleteEventEventIdFollow, useGetUsersMeEventsFollowed, usePostEventEventIdFollow, usePostGameGameIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedEvents() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: eventsData} = useGetUsersMeEventsFollowed({query:{enabled}})
  
  const followedEventIds = computed(() => new Set(eventsData.value?.map(e => String(e.id)) ?? []))


  const followEventMutation = usePostEventEventIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeEventsFollowedQueryKey() })
      }
    }
  })

  const unfollowEventMutation = useDeleteEventEventIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeEventsFollowedQueryKey() })
      }
    }
  })

  const toggleFollowEvent = (eventId: string | number) => {
    const idStr = String(eventId)
    if (followedEventIds.value.has(idStr)) {
      unfollowEventMutation.mutate({eventId: idStr})
    } else {
      followEventMutation.mutate({eventId: idStr})
    }
  }


  return {followedEventIds, toggleFollowEvent}
}