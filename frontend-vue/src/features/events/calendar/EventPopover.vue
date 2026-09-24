<script setup lang="ts">
import { type EventListItemDto } from '@/api/generated/models';
import { Popover } from 'openvue';
import { computed, ref } from 'vue';

const props = defineProps<{
  event: EventListItemDto | null
}>()

const popRef = ref<InstanceType<typeof Popover> | null>(null)
const selectedEvent = computed(() => props.event)

// śledzimy, dla którego eventu popover jest aktualnie otwarty
const activeEventId = ref<string | null>(null)

function open(jsEvent: Event, target: HTMLElement, eventId: string) {
  const isOpenOnSameEvent = activeEventId.value === eventId

  if (isOpenOnSameEvent) {
    hide()
    return
  }

  // jeśli otwarty na innym evencie, zamknij i pokaż na nowym
  popRef.value?.hide()
  activeEventId.value = eventId
  // nextTick nie jest tu potrzebny — show() samo repozycjonuje się do nowego targetu
  popRef.value?.show(jsEvent, target)
}

function hide() {
  popRef.value?.hide()
  activeEventId.value = null
}

defineExpose({ open, hide })
</script>

<template>
  <Popover ref="popRef">
    <div v-if="selectedEvent">
      <span>{{ selectedEvent.name }}</span>
    </div>
  </Popover>
</template>