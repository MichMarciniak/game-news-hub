<!-- EventItemSimple.vue -->
<script setup lang="ts">
import { computed } from 'vue'
import { useFollowedEvents } from '@/composables/eventFollows'
import FollowHeart from '../shared/FollowHeart.vue'

const props = defineProps<{
  event: { id: string | number; name: string }
}>()

const { followedEventIds, toggleFollowEvent } = useFollowedEvents()

const isFollowed = computed(() => followedEventIds.value.has(String(props.event.id)))
</script>

<template>
  <div class="flex items-center justify-between gap-3 rounded-lg px-3 py-2 hover:bg-highlight transition-colors">
    <span class="font-medium">{{ event.name }}</span>
    <FollowHeart
      :is-followed="isFollowed"
      size="1.25rem"
      @click.stop="toggleFollowEvent(event.id as string)"
    />
  </div>
</template>