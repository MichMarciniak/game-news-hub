<script setup lang="ts">
import type { EventListItemDto } from '@/api/generated/models';
import { formatTime } from '@/utils/date';
import { useRouter } from 'vue-router';

const router = useRouter()

const props = defineProps<{
  event: EventListItemDto
}>()

function goToEvent() {
  router.push({ name: 'event-detail', params: { id: props.event.id } })
}
</script>


<template>
  <div class="px-2 py-1 mx-2 my-3 cursor-pointer hover:bg-highlight transition-colors duration-100 ease-in-out rounded-md" 
    @click="goToEvent" 
  >
    <span>{{ props.event.name }}</span>
    <p class="text-sm text-muted-color">
      {{ formatTime(props.event.startTime) }}
      <template v-if="event.endTime"> 
        – {{ formatTime(props.event.endTime) }}
      </template>
    </p>

  </div>
</template>