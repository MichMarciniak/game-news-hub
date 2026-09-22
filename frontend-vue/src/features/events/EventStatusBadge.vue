<script setup lang="ts">
import { computed } from 'vue'

export type EventStatus = 'ended' | 'ongoing' | 'upcoming'

const props = defineProps<{
  startTime: string | Date
  endTime?: string | Date | null
}>()

const eventStatus = computed<EventStatus>(() => {
  const now = new Date()
  const start = new Date(props.startTime)
  const end = props.endTime ? new Date(props.endTime) : null

  if (end ? end < now : start < now) return 'ended'
  if (start <= now) return 'ongoing'
  return 'upcoming'
})

const statusConfig: Record<EventStatus, { color: string; label: string }> = {
  ended: { color: 'bg-surface-400', label: 'Finished' },
  ongoing: { color: 'bg-green-500', label: 'Ongoing' },
  upcoming: { color: 'bg-blue-500', label: 'Upcoming' },
}
</script>

<template>
  <div class="flex items-center gap-1.5 shrink-0">
    <span
      v-tooltip.top="statusConfig[eventStatus].label"
      class="inline-block size-2.5 rounded-full shrink-0"
      :class="statusConfig[eventStatus].color"
    />
  </div>
</template>