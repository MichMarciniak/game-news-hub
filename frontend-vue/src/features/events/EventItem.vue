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

const eventStatus = computed<'ended' | 'ongoing' | 'upcoming'>(() => {
  const now = new Date()
  const start = new Date(props.event.startTime as string)
  const end = props.event.endTime ? new Date(props.event.endTime) : null

  if (end ? end < now : start < now) return 'ended'
  if (start <= now) return 'ongoing'
  return 'upcoming'
})

const statusConfig = {
  ended: {color: 'bg-surface-400', label: 'Finished'},
  ongoing: {color: 'bg-green-500', label: 'Ongoing'},
  upcoming: {color: 'bg-blue-500', label: 'Upcoming'}
} as const

</script>

<template>
  <!-- Desktop -->
  <div class="hidden md:grid grid-cols-[1rem_1fr_11rem_11rem_2.7rem] items-center gap-4 rounded-lg px-4 py-2
            transition-colors hover:bg-highlight">
    <span
      v-tooltip.top="statusConfig[eventStatus].label"
      class="inline-block size-2.5 rounded-full shrink-0"
      :class="statusConfig[eventStatus].color"
    />
    <span class="truncate font-medium!">
      {{ event.name }}
    </span>
    <span>{{ formatEventDate(event.startTime) }}</span>
    <span>{{ event.endTime ? formatEventDate(event.endTime) : '—' }}</span>
    <FollowHeart
      :is-followed="isFollowed"
      :size="HEART_SIZE"
      @click="toggleFollowEvent(event.id as string)"
    />  
  </div>

  <!-- Mobile -->
  <div class="md:hidden flex items-start justify-between gap-3 px-0 py-3">

      <div class="min-w-0">
        <p class="truncate font-medium flex items-center gap-2">
          <span
            v-tooltip.top="statusConfig[eventStatus].label"
            class="inline-block size-2.5 rounded-full shrink-0"
            :class="statusConfig[eventStatus].color"
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
      @click="toggleFollowEvent(event.id as string)"
    />
  </div>
</template>