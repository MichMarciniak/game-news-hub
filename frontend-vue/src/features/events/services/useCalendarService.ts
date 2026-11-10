import { useGetEvents, useGetRecommendationEvents } from '@/api/generated'
import type { EventListItemDto } from '@/api/generated/models/eventListItemDto.ts'
import { isInRange } from '@/utils/date'
import { computed, ref } from 'vue'
import { keepPreviousData } from '@tanstack/vue-query'

export function useCalendarService(apiType: 'events' | 'recommendations' = 'events') {
  const rangeStart = ref<Date>()
  const rangeEnd = ref<Date>()

  const selectedDate = ref<Date>(new Date())
  const selectedDateEvents = ref<EventListItemDto[]>([])

  const apiHook = apiType === 'recommendations' ? useGetRecommendationEvents : useGetEvents

  const { data: eventsData, isPending } = apiHook(
    computed(() => ({
      from: rangeStart.value?.toISOString(),
      to: rangeEnd.value?.toISOString()
    })),
    {
      query: { placeholderData: keepPreviousData }
    }
  )

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
    const items = apiType === 'recommendations'
      ? (eventsData.value as any).events ?? []
      : (eventsData.value ?? [])
    selectedDateEvents.value = items.filter(
      (e: EventListItemDto) => isInRange(selectedDate.value, e.startTime as string, e.endTime as string | undefined)
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
