
<script setup lang="ts">
import type { EventListItemDto } from '@/api/generated/models';
import { monthDayFormatter } from '@/utils/date';
import EventDayItem from './EventDayItem.vue';

const props = defineProps<{
  events: EventListItemDto[],
  date: Date
}>()

</script>

<template>

  <Panel class="flex flex-col" :pt="{ content: { class: '!p-0' }, header: {class: '!pb-0'} }">
    <template #header>
      <h2 class="text-2xl">{{ monthDayFormatter.format(props.date) }}</h2>
    </template>
    <div class="flex flex-col items-center">
      <Divider class="max-w-[90%]"/>
      <div class=" w-full">
        <EventDayItem v-for="e in events" :key="e.id" :event="e"/>
      </div>

    </div>
  </Panel>

</template>