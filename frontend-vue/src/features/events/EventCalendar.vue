<script setup lang="ts">
import { useGetEvent } from '@/api/generated/index.ts';
import MonthCalendar from '../shared/MonthCalendar.vue';
import type { EventListItemDto } from '@/api/generated/models/eventListItemDto.ts';
import { computed, ref } from 'vue';

const rangeStart = ref<Date>()
const rangeEnd = ref<Date>()

function handleRangeChange(start: Date, end: Date) {
  rangeStart.value = start
  rangeEnd.value = end
}

const {data: eventsData, isPending} = useGetEvent(
  computed(() => ({
    from: rangeStart.value?.toISOString(),
    to: rangeEnd.value?.toISOString()
  }))
)

</script>

<template>
  <div class="w-full max-w-7xl">

    <MonthCalendar v-if="eventsData" :events="eventsData as EventListItemDto[]" @range-change="handleRangeChange"/>
  </div>
</template>