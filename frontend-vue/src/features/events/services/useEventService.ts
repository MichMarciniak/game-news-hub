import { useFollowedEvents } from '@/composables/eventFollows'
import { useGetEventsId } from '@/api/generated'
import { formatEventDate } from '@/utils/date'
import { computed } from 'vue'

export function useEventService(eventId: string | number | undefined) {
  const { data: event, isPending } = useGetEventsId(computed(() => String(eventId)))
  const { followedEventIds, toggleFollowEvent } = useFollowedEvents()

  const isFollowed = computed(() => !!event.value && followedEventIds.value.has(String(event.value.id)))

  // inne eventy z tej samej serii, minus obecny event
  const otherSeriesEvents = computed(() =>
    event.value?.relatedEvents?.filter((e) => e.id !== event.value?.id) ?? []
  )

  const defaultValue = computed(() =>
    event.value?.games?.length ? "0" : otherSeriesEvents.value.length ? "1" : "0"
  )

  return {
    event,
    isPending,
    isFollowed,
    toggleFollowEvent,
    otherSeriesEvents,
    defaultValue,
    formatEventDate
  }
}
