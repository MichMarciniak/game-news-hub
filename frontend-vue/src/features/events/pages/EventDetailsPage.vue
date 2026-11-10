<script setup lang="ts">
import { computed } from 'vue'
import { useEventService } from '@/features/events/services/useEventService'
import { useRoute } from 'vue-router'
import { OiHeart, OiHeartFill, OiLink } from '@openvue/openicons-vue'
import EventItemSimple from '@/features/events/components/EventItemSimple.vue'
import EventStatusBadge from '@/features/events/components/EventStatusBadge.vue'
import FollowHeart from '@/features/shared/FollowHeart.vue'
import GameInfiniteGrid from '@/features/shared/GameInfiniteGrid.vue'
import type { GameListItemDto } from '@/api/generated/models/gameListItemDto.ts'

const route = useRoute()

const eventId = String(route.params.id)

const {
  event,
  isPending,
  isFollowed,
  toggleFollowEvent,
  otherSeriesEvents,
  defaultValue,
  formatEventDate
} = useEventService(eventId)

const formattedStartTime = computed(() => formatEventDate(event.value?.startTime ?? ''))
const formattedEndTime = computed(() => formatEventDate(event.value?.endTime ?? ''))
</script>

<template>
  <div class="max-w-4xl w-full mx-auto flex flex-col gap-8">
    <template v-if="isPending">
      <!-- <SkeletonEventDetails /> -->
    </template>


    <template v-else-if="event">
      <!-- Header, follow, description -->
      <div class="flex flex-col gap-4">
        <div class="flex items-start justify-between gap-4">
          <!-- Badge + title -->
          <div class="flex items-center gap-3 min-w-0">
            <EventStatusBadge
              :start-time="event.startTime ?? ''" 
              :end-time="event.endTime ?? ''" 
            />
            <h1 class="text-2xl font-semibold">{{ event.name }}</h1>
          </div>

          <!-- Follow btn -->
          <Button
            :severity="isFollowed ? 'secondary' : 'primary'"
            :outlined="isFollowed"
            @click="toggleFollowEvent(event.id as string)"
            class="not-sm:hidden"
          >
            <template v-if="isFollowed">
              <OiHeartFill v-if="isFollowed" size="1.2rem"/>
              <span>Following</span>
            </template>
            <template v-else>
              <OiHeart size="1.2rem"/>
              <span>Follow</span>
            </template>
          </Button>

          <!-- Follow btn mobile -->
          <FollowHeart 
            class="sm:hidden"
            :size='"2rem"'
            :is-followed="isFollowed"
            @click="toggleFollowEvent(event.id as string)"
          />
        </div>

        <!-- Time, follow count, stream url -->
        <div class="flex items-center gap-4 text-sm text-muted-color">
          <span>
            {{ formattedStartTime }}
            <template v-if="event.endTime"> – {{ formattedEndTime }}</template>
          </span>
          <span class="flex items-center gap-1">
            <i class="pi pi-users" />
            {{ event.followerCount }} followers
          </span>
          <a v-if="event.liveStreamUrl" :href="event.liveStreamUrl" rel="noopener noreferrer" target="_blank" class="flex items-center ">
            <OiLink size="1.5rem"/>
            {{ event.liveStreamUrl }}
          </a>
        </div>

      </div>

      <!-- Description -->
      <p v-if="event.description" class="text-muted-color whitespace-pre-line">
        {{ event.description }}
      </p>

      <!-- Games -->
      <Tabs :value="defaultValue" v-if="event.games?.length || otherSeriesEvents.length">
        <TabList>
          <Tab value="0" v-if="event.games?.length">Games ({{ event.games?.length }})</Tab>
          <Tab value="1" v-if="otherSeriesEvents.length">Events ({{ otherSeriesEvents.length }})</Tab>
        </TabList>
        <TabPanels class="p-0">
          <TabPanel value="0" class="pt-2">
            <GameInfiniteGrid
              :games="event.games as GameListItemDto[]"
            />
          </TabPanel>
          <TabPanel value="1" class="p-0">
            <div class="flex flex-col gap-4 overflow-y-auto pb-2">
              <EventItemSimple
                v-for="seriesEvent in otherSeriesEvents as { id: string | number; name: string }[]"
                :key="seriesEvent.id"
                :event="seriesEvent"
              />
            </div>
          </TabPanel>
        </TabPanels>
      </Tabs>

    </template>
  </div>
</template>
