<script setup lang="ts">
import { useCalendarService } from '@/features/events/services/useCalendarService';
import { ref, computed } from 'vue';
import MonthCalendar from '@/features/events/calendar/MonthCalendar.vue';
import EventDayList from '@/features/events/calendar/EventDayList.vue';
import SkeletonCalendar from '@/features/shared/skeletons/SkeletonCalendar.vue';
import Select from 'openvue/select';

const limit = ref(5)

const {
  selectedDate,
  selectedDateEvents,
  eventsData,
  isPending,
  handleRangeChange,
  handleDateSelect
} = useCalendarService('recommendations', limit);

const limitOptions = [
  { label: '2', value: 2 },
  { label: '5', value: 5 },
  { label: '10', value: 10 },
  { label: '20', value: 20 }
]

function onLimitChange(value: number) {
  limit.value = value
}
</script>

<template>
  <div class="w-full max-w-7xl flex flex-col lg:flex-row gap-6">
    <div class="lg:w-[80%]">
      <SkeletonCalendar v-if="isPending" />
      <MonthCalendar v-else
        :events="eventsData as any[]"
        :selected-date="selectedDate"
        :is-recommendations="true"
        @range-change="handleRangeChange"
        @date-select="handleDateSelect"

      />
      <div class="pt-4 self-start  w-full lg:w-48">
        <Select v-model="limit" :options="limitOptions" option-label="label" option-value="value" placeholder="Limit" class="w-full" @update:model-value="onLimitChange" />
      </div>

    </div>

    <EventDayList
      class="w-full lg:w-[20%]"
      :events="selectedDateEvents" 
      :date="selectedDate"
      :is-recommendations="true"
    />
  </div>
</template>