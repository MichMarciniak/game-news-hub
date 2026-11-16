import { useGetEvents, useGetRecommendationEvents } from '@/api/generated'
import type { EventListItemDto, RecommendationDto, RecommendationResponse } from '@/api/generated/models'
import { isInRange } from '@/utils/date'
import { computed, ref } from 'vue'
import { keepPreviousData } from '@tanstack/vue-query'

export function useCalendarService(apiType: 'events' | 'recommendations' = 'events') {
  const rangeStart = ref<Date>()
  const rangeEnd = ref<Date>()

  const selectedDate = ref<Date>(new Date())
  const selectedDateEvents = ref<(EventListItemDto | RecommendationDto)[]>([])

  const apiHook = apiType === 'recommendations' ? useGetRecommendationEvents : useGetEvents

  const { data: rawEventsData, isPending } = apiHook(
    computed(() => ({
      from: rangeStart.value?.toISOString(),
      to: rangeEnd.value?.toISOString()
    })),
    {
      query: { placeholderData: keepPreviousData }
    }
  )

  // Extract just the events array for recommendations
  const eventsData = computed(() => {
    if (!rawEventsData.value) return []
    if (apiType === 'recommendations') {
      return (rawEventsData.value as RecommendationResponse).events ?? []
    }
    return rawEventsData.value ?? []
  })

  function handleRangeChange(start: Date, end: Date) {
    rangeStart.value = start
    rangeEnd.value = end
  }

  function handleDateSelect(date: Date) {
    selectedDate.value = date
    updateSelectedDateEvents()
  }

  function updateSelectedDateEvents() {
    if (!eventsData.value) return []
    selectedDateEvents.value = eventsData.value.filter(
      (e: EventListItemDto | RecommendationDto) => isInRange(selectedDate.value, e.startTime as string, e.endTime as string | undefined)
    )
  }

  return {
    rangeStart,
    rangeEnd,
    selectedDate,
    selectedDateEvents,
    eventsData,
    isPending,
    handleRangeChange,
    handleDateSelect,
    updateSelectedDateEvents
  }
}
