<script setup lang="ts">
import { useGetRecommendationEvents } from '@/api/generated';
import { addDays } from '@/utils/date';
import { ref, computed, watch } from 'vue';
import EventFullDateItem from './EventFullDateItem.vue';
import type { RecommendationResponse } from '@/api/generated/models';
import type { EventListItemDto } from '@/api/generated/models';
import Select from 'openvue/select';

const limit = ref(5)

const query = computed(() => ({
  from: new Date().toISOString(),
  to: addDays(new Date(), 7).toISOString(),
  limit: limit.value
}))

const {data: responseData, isPending, refetch} = useGetRecommendationEvents(query)

const events = computed<EventListItemDto[]>(() => {
  const items = responseData.value?.events ?? []
  return items.map(e => ({
    id: e.id,
    name: e.name,
    followerCount: e.followerCount,
    startTime: e.startTime,
    endTime: e.endTime
  }))
})

const limitOptions = [
  { label: '2', value: 2 },
  { label: '5', value: 5 },
  { label: '10', value: 10 },
  { label: '20', value: 20 }
]

function onLimitChange(event: { value: number }) {
  limit.value = event.value
  refetch()
}
</script>

<template>
  <Panel class="flex flex-col gap-4">
    <template #header>
      <div class="flex items-center justify-between w-full">
        <h3 class="text-xl flex-1 shrink-0">Recommended events</h3>
        <div class="flex items-center gap-2">
          <span class="text-sm text-muted-color">Max recommendations:</span>
          <Select v-model="limit" :options="limitOptions" option-label="label" option-value="value" @change="onLimitChange" />
        </div>
      </div>
    </template>

    <div class="flex flex-col gap-4">
      <EventFullDateItem v-for="e in events" :key="e.id" :event="e" />
      <div v-if="isPending" class="text-center text-muted-color">Loading...</div>
      <div v-else-if="events.length === 0" class="text-center text-muted-color">No recommended events.</div>
    </div>
  </Panel>
</template>