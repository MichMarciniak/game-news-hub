<script setup lang="ts">
import type { EventListItemDto, RecommendationDto } from '@/api/generated/models';
import { monthDayFormatter } from '@/utils/date';
import EventDayItem from './EventDayItem.vue';
import RecommendationDayItem from './RecommendationDayItem.vue';
import { OiTimes } from '@openvue/openicons-vue';
import EventFullDateItem from '@/features/events/list/EventFullDateItem.vue';

const props = defineProps<{
  events: (EventListItemDto | RecommendationDto)[],
  date: Date,
  isRecommendations?: boolean
}>()
</script>

<template>

  <Panel class="flex flex-col" :pt="{ content: { class: '!p-0' }, header: {class: '!pb-0'} }">
    <template #header>
      <h2 class="text-2xl">{{ monthDayFormatter.format(props.date) }}</h2>
    </template>
    <div class="flex flex-col items-center">
      <Divider class="max-w-[90%]"/>
      <div class=" w-full mb-4">
        <template v-if="events && events.length">
          <RecommendationDayItem v-if="props.isRecommendations" v-for="e in events" :key="e.id" :event="e" />
          <EventFullDateItem v-else v-for="e in events" :key="e.id" :event="e" />
        </template>
        <div v-else class="flex flex-col mx-auto items-center text-muted-color">
          <OiTimes size="2rem"/>
          <span class="text-xl">No events that day.</span>
        </div>
      </div>

    </div>
  </Panel>

</template>