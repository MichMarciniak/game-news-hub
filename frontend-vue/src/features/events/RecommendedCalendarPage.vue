<script setup lang="ts">
import { useGetRecommendationsEvents } from '@/api/generated/index.ts';
import MonthCalendar from '@/features/events/calendar/MonthCalendar.vue';
import type { EventListItemDto } from '@/api/generated/models/eventListItemDto.ts';
import { computed, ref } from 'vue';
import EventDayList from './calendar/EventDayList.vue';
import { isInRange } from '@/utils/date.ts';
import { keepPreviousData } from '@tanstack/vue-query';
import SkeletonCalendar from '../shared/skeletons/SkeletonCalendar.vue';

const rangeStart = ref<Date>()
const rangeEnd = ref<Date>()

const selectedDate = ref<Date>(new Date())
const selectedDateEvents = ref<EventListItemDto[]>([])

const {data: eventsData, isPending} = useGetRecommendationsEvents(
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
  selectedDateEvents.value = eventsData.value.filter(
    e => isInRange(selectedDate.value, e.startTime as string, e.endTime as string | undefined)
  )
}


</script>

<template>
  <div class="w-full max-w-7xl flex flex-col lg:flex-row gap-6">

    <SkeletonCalendar v-if="isPending" />
    <MonthCalendar v-else
      class="lg:w-[80%]"
      :events="eventsData as EventListItemDto[]" 
      @range-change="handleRangeChange" 
      @date-select="handleDateSelect"
    />
    <EventDayList
      class="w-full lg:w-[20%]"
      :events="selectedDateEvents" 
      :date="selectedDate" 
    />
  </div>
</template>