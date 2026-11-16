import { useGetEvents, useGetRecommendationEvents } from '@/api/generated'
import type { EventListItemDto, RecommendationDto, RecommendationResponse } from '@/api/generated/models'
import { isInRange } from '@/utils/date'
import { computed, ref, type Ref, watch } from 'vue'
import { keepPreviousData } from '@tanstack/vue-query'

export function useCalendarService(
  apiType: 'events' | 'recommendations' = 'events',
  limit?: Ref<number>
) {
  const rangeStart = ref<Date>()
  const rangeEnd = ref<Date>()

  const selectedDate = ref<Date>(new Date())
  const selectedDateEvents = ref<(EventListItemDto | RecommendationDto)[]>([])

  const apiHook = apiType === 'recommendations' ? useGetRecommendationEvents : useGetEvents

  const queryParams = computed(() => ({
    from: rangeStart.value?.toISOString(),
    to: rangeEnd.value?.toISOString(),
    limit: limit?.value
  }))

  const { data: rawEventsData, isPending, refetch } = apiHook(
    queryParams,
    {
      query: { placeholderData: keepPreviousData }
    }
  )

  // Force refetch when limit changes
  if (limit) {
    watch(limit, () => {
      refetch()
    })
  }

  // Extract just the events array for recommendations
  const eventsData = computed<(EventListItemDto | RecommendationDto)[]>(() => {
    const data = rawEventsData.value
    if (!data) return []
    if (apiType === 'recommendations') {
      const resp = data as RecommendationResponse
      return resp.events ?? []
    }
    return data as EventListItemDto[]
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