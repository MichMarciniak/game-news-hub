<script setup lang="ts">
import { useGetRecommendationEvents } from '@/api/generated';
import { addDays } from '@/utils/date';
import { ref, computed } from 'vue';
import EventFullDateItem from './EventFullDateItem.vue';
import type { RecommendationResponse } from '@/api/generated/models';
import type { EventListItemDto } from '@/api/generated/models';


const query = ref({
  from: new Date().toISOString(),
  to: addDays(new Date(), 7).toISOString()
})

const {data: responseData, isPending} = useGetRecommendationEvents(query)

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

</script>


<template>
  <Panel class="h-full" :pt="{header: '!pb-0'}">
    <template #header>
      <h3 class="text-xl">Recommended this week</h3>
    </template>
    <div class="flex flex-col">

      <Divider />
      <div class="flex flex-col gap-5 items-center">

        <div v-if="isPending || events.length === 0">
          <span class="text-muted-color">No recommended events.</span>
        </div>
        
        <div v-else>
          <EventFullDateItem v-for="e in events" :key="e.id" :event="e" />
        </div>
        <RouterLink to="/events/recommended">
          <Button variant="outlined">
            Go to calendar
          </Button>

        </RouterLink>
      </div>
    </div>

  </Panel>

</template>
