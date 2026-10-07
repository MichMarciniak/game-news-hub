import { getGetUsersMeFollowedEventsQueryKey, useDeleteEventsIdFollow, useGetUsersMeFollowedEvents, usePostEventsIdFollow } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { useQueryClient } from "@tanstack/vue-query";
import { computed } from "vue";


export function useFollowedEvents() {
  const queryClient = useQueryClient()
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: eventsData} = useGetUsersMeFollowedEvents({query:{enabled, staleTime: Infinity}})
  
  const followedEventIds = computed(() => new Set(eventsData.value?.map(e => String(e.id)) ?? []))


  const followEventMutation = usePostEventsIdFollow({
    mutation:{
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedEventsQueryKey() })
      }
    }
  })

  const unfollowEventMutation = useDeleteEventsIdFollow({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({queryKey: getGetUsersMeFollowedEventsQueryKey() })
      }
    }
  })

  const toggleFollowEvent = (eventId: string | number) => {
    const idStr = String(eventId)
    if (followedEventIds.value.has(idStr)) {
      unfollowEventMutation.mutate({id: idStr})
    } else {
      followEventMutation.mutate({id: idStr})
    }
  }


  return {followedEventIds, toggleFollowEvent}
}