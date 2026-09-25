<script setup lang="ts">
import { type EventListItemDto } from '@/api/generated/models';
import { useFollowedEvents } from '@/composables/eventFollows';
import FollowHeart from '@/features/shared/FollowHeart.vue';
import { formatTime } from '@/utils/date';
import { OiArrowRight, OiClock, OiHeartFill, OiTimes } from '@openvue/openicons-vue';
import { Popover } from 'openvue';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';

const props = defineProps<{
  event: EventListItemDto | null
}>()

const router = useRouter()

const popRef = ref<InstanceType<typeof Popover> | null>(null)
const selectedEvent = computed(() => props.event)

// śledzimy, dla którego eventu popover jest aktualnie otwarty
const activeEventId = ref<string | null>(null)

const {followedEventIds, toggleFollowEvent} = useFollowedEvents()
const isFollowed = computed(() => !!activeEventId.value && followedEventIds.value.has(String(activeEventId.value)))


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

function goToEvent() {
  router.push({ name: 'event-detail', params: { id: activeEventId.value } })
}
</script>

<template>
  <Popover ref="popRef">
    <div v-if="selectedEvent" class="flex flex-col gap-4">
      <!-- Header -->
      <div class="flex justify-between gap-4">
        <span class="text-semibold! max-w-70 text-center">{{ selectedEvent.name }}</span>
        <div class="flex items-start">
          <Button class="p-0 m-0 hover:bg-transparent hover:text-muted-color text-color" variant="text" @click="hide">
            <OiTimes size="1.5rem" title="close"/>
          </Button>
        </div>
      </div>

      <!-- content -->
      <div class="text-muted-color flex justify-between gap-5">
        <!-- Time -->
        <div class="flex items-center gap-1">
          <OiClock size="1rem" title="time"/>
          <div>
            <span class="text-muted-color">{{ formatTime(event?.startTime) }}</span>
            <span v-if="event?.endTime" class="text-muted-color"> - {{ formatTime(event?.endTime) }}</span>
          </div>

        </div>
        <!-- Follows -->
        <div class="flex items-center gap-1">
          <OiHeartFill size="1rem" title="follows"/>
          <span>{{ event?.followerCount }}</span>
        </div>
      </div>

      <!-- nav -->
      <div class="flex items-center justify-between">
        <FollowHeart 
          :is-followed="isFollowed"
          @click="toggleFollowEvent(activeEventId as string)" 
        />
        <Button variant="text" class="p-0 m-0" @click="goToEvent" aria-label="go-to-event">
          <OiArrowRight size="1.5rem"/>
        </Button>
      </div>
    </div>
    <div>

    </div>
  </Popover>
</template>