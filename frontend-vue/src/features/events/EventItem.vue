<script setup lang="ts">
import type { EventListItemDto } from '@/api/generated/models'; // dopasuj nazwę typu
import { useFollowedEvents } from '@/composables/eventFollows';
import { formatEventDate } from '@/utils/date';
import { computed } from 'vue';
import FollowHeart from '../shared/FollowHeart.vue';
import { useRouter } from 'vue-router';
import EventStatusBadge, { type EventStatus } from '@/features/events/EventStatusBadge.vue';

const HEART_SIZE = '1.5rem'

const props = defineProps<{
  event: EventListItemDto
}>()

const router = useRouter()

const {followedEventIds, toggleFollowEvent } = useFollowedEvents()

const isFollowed = computed(() => followedEventIds.value.has(String(props.event.id)))

function goToEvent() {
  router.push({ name: 'event-detail', params: { id: props.event.id } })
}

</script>

<template>


  <!-- Desktop -->
  <div class="hidden md:grid grid-cols-[1rem_1fr_11rem_11rem_2.7rem] items-center gap-4 rounded-lg px-4 py-2
            transition-colors hover:bg-highlight cursor-pointer"
            @click="goToEvent">
      <EventStatusBadge 
        :start-time="event.startTime as string" 
        :end-time="event.endTime" 
      />

    <span class="truncate font-medium!">
      
      {{ event.name }}
    </span>
    <span>{{ formatEventDate(event.startTime) }}</span>
    <span>{{ event.endTime ? formatEventDate(event.endTime) : '—' }}</span>
    <FollowHeart
      :is-followed="isFollowed"
      :size="HEART_SIZE"
      @click.stop="toggleFollowEvent(event.id as string)"
    />  
  </div>
   
  <!-- Mobile -->
  <div class="md:hidden flex items-start justify-between gap-3 px-0 py-3" @click="goToEvent">
    <div class="min-w-0">
      <p class="font-medium flex items-center gap-2">
        <EventStatusBadge 
            :start-time="event.startTime as string" 
            :end-time="event.endTime" 
          />
        {{ event.name }}
      </p>
      <p class="text-sm text-muted-color">
        {{ formatEventDate(event.startTime) }}<template v-if="event.endTime"> – {{ formatEventDate(event.endTime) }}</template>
      </p>
    </div>
  
  
    <FollowHeart
      :is-followed="isFollowed"
      :size="HEART_SIZE"
      @click.stop="toggleFollowEvent(event.id as string)"
    />
  </div>


</template>