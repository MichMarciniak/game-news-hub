<script setup lang="ts">
import { useCalendarService } from '@/features/events/services/useCalendarService';
import MonthCalendar from '@/features/events/calendar/MonthCalendar.vue';
import EventDayList from '@/features/events/calendar/EventDayList.vue';
import SkeletonCalendar from '@/features/shared/skeletons/SkeletonCalendar.vue';

const {
  selectedDate,
  selectedDateEvents,
  eventsData,
  isPending,
  handleRangeChange,
  handleDateSelect
} = useCalendarService('recommendations');
</script>

<template>
  <div class="w-full max-w-7xl flex flex-col lg:flex-row gap-6">

    <SkeletonCalendar v-if="isPending" />
    <MonthCalendar v-else
      class="lg:w-[80%]"
      :events="eventsData as any[]"
      :selected-date="selectedDate"
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
