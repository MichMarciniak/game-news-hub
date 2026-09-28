<script setup lang="ts">
import type { EventListItemDto } from '@/api/generated/models';
import { addDays, monthDayFormatter } from '@/utils/date';
import { OiBan, OiTimes } from '@openvue/openicons-vue';
import EventWeekItem from './EventWeekItem.vue';
import { computed, ref } from 'vue';
import { useGetEvent } from '@/api/generated/index.ts';

const query = ref({
  // from: new Date().toISOString(),
  // to: addDays(new Date(), 7).toISOString()
});

const { data: responseData, isPending } = useGetEvent(query);

// Bezpieczne wyciągnięcie tablicy (dostosuj .data jeśli API zwraca obiekt bezpośrednio)
const eventsData = computed(() => {
  return Array.isArray(responseData.value) 
    ? responseData.value 
    : responseData.value?.data ?? [];
});

const options = ref([
  { label: "Date", value: 'date' },
  { label: "Popularity", value: 'popularity' }
]);
const sort = ref(options.value[0]);

const events = computed(() => {
  if (!eventsData.value || eventsData.value.length === 0) return [];

  const isDateSort = sort.value?.value === 'date';

  return [...eventsData.value].sort((a, b) => {
    if (isDateSort) {
      return new Date(a.startTime!).getTime() - new Date(b.startTime!).getTime();
    }
    return Number(b.followerCount ?? 0) - Number(a.followerCount ?? 0);
  });
});

</script>

<template>
  <Panel class="flex flex-col" :pt="{ content: { class: '!p-0' }, header: { class: '!pb-0' } }">
    <template #header>
      <div class="flex items-center justify-between w-full">
        <h3 class="text-xl flex-1 shrink-0">Upcoming this week</h3>
        <SelectButton v-model="sort" :options="options" option-label="label" data-key="value" />
      </div>
    </template>
    <div class="flex flex-col items-center">
      <Divider class="max-w-[90%]" />
      <div class="w-full mb-4">
        <!-- 1. Ładowanie -->
        <template v-if="isPending">
          <Skeleton class="h-12 w-full my-2" />
        </template>
        
        <!-- 2. Są wydarzenia -->
        <template v-else-if="events.length > 0">
          <EventWeekItem v-for="e in events" :key="e.id" :event="e" />
        </template>
        
        <!-- 3. Brak wydarzeń -->
        <div v-else class="flex flex-col mx-auto items-center text-muted-color">
          <OiTimes size="2rem" />
          <span class="text-xl">No events this week</span>
        </div>
      </div>
    </div>
    <RouterLink to="/events" class="flex justify-center p-2">
      <Button variant="outlined">View calendar</Button>
    </RouterLink>
  </Panel>
</template>