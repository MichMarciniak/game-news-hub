<script setup lang="ts">
import { useGetEvent } from '@/api/generated';
import { addDays } from '@/utils/date';
import { ref } from 'vue';
import EventDayItem from '../events/calendar/EventDayItem.vue';

const query = ref({
  from: new Date().toISOString(),
  to: addDays(new Date(), 7).toISOString()
})

const {data: events, isPending} = useGetEvent(query)

</script>

<template>

  <Panel class="h-full" :pt="{ header: {class: '!pb-0'} }">
    <template #header>
      <h3 class="text-xl">Upcoming this week</h3>
    </template>
    <div class="flex flex-col">
      <Divider />
      <div class="flex flex-col items-center gap-5">

        <div v-if="isPending || events?.length == 0">
          <span class="text-muted-color">No upcoming events.</span>
        </div>
        
        <div v-else>
          <EventDayItem v-for="e in events" :key="e.id" :event="e" />
        </div>
        
        <RouterLink to="/events">
          <Button variant="outlined">
            Go to calendar
          </Button>
        </RouterLink>
      </div>
    </div>
    
  </Panel>

</template>