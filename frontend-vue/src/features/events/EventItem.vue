<script setup lang="ts">
import type { EventListItemDto } from '@/api/generated/models'; // dopasuj nazwę typu
import { useFollowedEvents } from '@/composables/eventFollows';
import { formatEventDate } from '@/utils/date';
import { computed } from 'vue';
import FollowHeart from '../shared/FollowHeart.vue';

const HEART_SIZE = '1.5rem'

const props = defineProps<{
  event: EventListItemDto
}>()

const {followedEventIds, toggleFollowEvent } = useFollowedEvents()

const isFollowed = computed(() => followedEventIds.value.has(String(props.event.id)))

</script>

<template>
  <!-- Desktop -->
  <div class="hidden md:grid grid-cols-[1fr_11rem_11rem_2.7rem] items-center gap-4 border-b px-4 py-2">
    <span class="truncate font-medium">{{ event.name }}</span>
    <span>{{ formatEventDate(event.startTime) }}</span>
    <span>{{ event.endTime ? formatEventDate(event.endTime) : '—' }}</span>
    <FollowHeart
      :is-followed="isFollowed"
      :size="HEART_SIZE"
      @click="toggleFollowEvent(event.id as string)"
    />  
  </div>

  <!-- Mobile -->
  <div class="md:hidden flex items-start justify-between gap-3 px-2 py-3 border-b">
    <div class="min-w-0">
      <p class="truncate font-medium">{{ event.name }}</p>
      <p class="text-sm text-muted-color">
        {{ formatEventDate(event.startTime) }}<template v-if="event.endTime"> – {{ formatEventDate(event.endTime) }}</template>
      </p>
    </div>
    <FollowHeart
      :is-followed="isFollowed"
      :size="HEART_SIZE"
      @click="toggleFollowEvent(event.id as string)"
    />
  </div>
</template>