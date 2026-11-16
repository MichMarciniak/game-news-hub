<script setup lang="ts">
import type { RecommendationDto } from '@/api/generated/models';
import { useFollowedEvents } from '@/composables/eventFollows';
import FollowHeart from '@/features/shared/FollowHeart.vue';
import { formatTime } from '@/utils/date';
import { OiArrowRight, OiClock, OiHeartFill, OiTimes, OiInfo, OiChartBar } from '@openvue/openicons-vue';
import { Popover, Accordion, AccordionPanel, AccordionHeader, AccordionContent, Badge, ProgressBar } from 'openvue';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';

const props = defineProps<{
  event: RecommendationDto | null
}>();

const router = useRouter();

const popRef = ref<InstanceType<typeof Popover> | null>(null);
const selectedEvent = computed(() => props.event);
const activeEventId = ref<string | null>(null);

const { followedEventIds, toggleFollowEvent } = useFollowedEvents();
const isFollowed = computed(() => !!activeEventId.value && followedEventIds.value.has(String(activeEventId.value)));

const expandedPanel = ref<string | null>(null);

function open(jsEvent: Event, target: HTMLElement, eventId: string) {
  const isOpenOnSameEvent = activeEventId.value === eventId;

  if (isOpenOnSameEvent) {
    hide();
    return;
  }

  popRef.value?.hide();
  activeEventId.value = eventId;
  popRef.value?.show(jsEvent, target);
}

function hide() {
  popRef.value?.hide();
  activeEventId.value = null;
  expandedPanel.value = null;
}

defineExpose({ open, hide });

function goToEvent() {
  router.push({ name: 'event-detail', params: { id: activeEventId.value } });
}

function getScoreColor(score: number | string | undefined): string {
  const numScore = typeof score === 'string' ? parseFloat(score) : (score ?? 0);
  if (numScore >= 80) return 'green';
  if (numScore >= 50) return 'yellow';
  if (numScore >= 20) return 'orange';
  return 'red';
}

function getLabelText(code: string): string {
  const labels: Record<string, string> = {
    followed_genre: 'Followed genre',
    followed_platform: 'Followed platform',
    followed_platform_group: 'Followed platform group',
    followed_series: 'Followed series',
    followed_game: 'Followed game',
    high_momentum: 'High momentum',
    popular: 'Popular event',
  };
  return labels[code] || code.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
}

function getDetailLabel(key: string): string {
  const labels: Record<string, string> = {
    personalScore: 'Personal match',
    momentumScore: 'Momentum',
    gameFacetContribution: 'Game relevance',
    genreFacetContribution: 'Genre match',
    platformGroupFacetContribution: 'Platform match',
    seriesFacetContribution: 'Series match',
  };
  return labels[key] || key;
}
</script>

<template>
  <Popover ref="popRef">
    <div v-if="selectedEvent" class="flex flex-col gap-4 min-w-[320px] max-w-[400px]">
      <!-- Header -->
      <div class="flex justify-between gap-4">
        <span class="text-semibold! max-w-70 text-center">{{ selectedEvent.name }}</span>
        <div class="flex items-center gap-2">
          <Badge :value="selectedEvent.isUpcoming ? 'Upcoming' : 'Ended'" :severity="selectedEvent.isUpcoming ? 'success' : 'secondary'" class="text-xs" />
          <Button class="p-0 m-0 hover:bg-transparent hover:text-muted-color text-color" variant="text" @click="hide">
            <OiTimes size="1.5rem" title="close" />
          </Button>
        </div>
      </div>

      <!-- Score badge row -->
      <div v-if="selectedEvent.score" class="flex items-center gap-3 flex-wrap">
        <div class="flex items-center gap-2">
          <OiChartBar size="1.25rem" class="text-primary" />
          <span class="text-sm font-medium">Score:</span>
          <Badge :value="Math.round(Number(selectedEvent.score.finalScore || 0))" :severity="getScoreColor(selectedEvent.score.finalScore)" class="px-2 py-0.5" />
        </div>
        <div v-if="selectedEvent.score.popularityScore !== undefined" class="flex items-center gap-1 text-sm text-muted-color">
          <span>Popularity:</span>
          <Badge :value="Math.round(Number(selectedEvent.score.popularityScore))" severity="info" class="px-2 py-0.5" />
        </div>
      </div>

      <!-- Labels chips -->
      <div v-if="selectedEvent.score?.labels?.length" class="flex flex-wrap gap-1.5">
        <Badge v-for="(label, idx) in selectedEvent.score.labels" :key="label.code ?? idx" :label="getLabelText(label.code ?? '') + (label.count ? ' ×' + label.count : '')" severity="contrast" class="text-xs" />
      </div>

      <!-- Basic info -->
      <div class="text-muted-color flex justify-between gap-5 border-t pt-3">
        <div class="flex items-center gap-1">
          <OiClock size="1rem" />
          <div>
            <span>{{ formatTime(selectedEvent?.startTime) }}</span>
            <span v-if="selectedEvent?.endTime" class="text-muted-color"> - {{ formatTime(selectedEvent?.endTime) }}</span>
          </div>
        </div>
        <div class="flex items-center gap-1">
          <OiHeartFill size="1rem" />
          <span>{{ selectedEvent?.followerCount }}</span>
        </div>
      </div>

      <!-- Expandable "Why this recommendation?" -->
      <div v-if="selectedEvent.score?.details" class="border-t pt-3">
        <Accordion v-model="expandedPanel" multiple>
          <AccordionPanel value="details">
            <AccordionHeader class="flex items-center gap-2 cursor-pointer p-2 hover:bg-highlight rounded-md">
              <OiInfo size="1rem" class="text-primary" />
              <span class="text-sm font-medium">Why this recommendation?</span>
            </AccordionHeader>
            <AccordionContent class="p-2 space-y-3 text-sm">
              <div v-for="(value, key, idx) in selectedEvent.score.details" :key="key ?? idx" class="flex flex-col gap-1">
                <div class="flex justify-between text-muted-color">
                  <span>{{ getDetailLabel(key ?? '') }}</span>
                  <span class="font-medium">{{ Math.round(Number(value)) }}</span>
                </div>
                <ProgressBar :value="Math.min(100, Math.max(0, Math.round(Number(value))))" :showValue="false" class="h-1.5" />
              </div>
              <div v-if="selectedEvent.score.labels?.length" class="border-t pt-2">
                <p class="text-xs text-muted-color mb-1">Matching factors:</p>
                <div class="flex flex-wrap gap-1">
                  <Badge v-for="(label, idx) in selectedEvent.score.labels" :key="label.code ?? idx" :label="getLabelText(label.code ?? '') + (label.count ? ' ×' + label.count : '')" severity="secondary" class="text-xs" />
                </div>
              </div>
            </AccordionContent>
          </AccordionPanel>
        </Accordion>
      </div>

      <!-- Actions -->
      <div class="flex items-center justify-between border-t pt-3">
        <FollowHeart :is-followed="isFollowed" @click="toggleFollowEvent(activeEventId as string)" />
        <Button variant="text" class="p-0 m-0" @click="goToEvent" aria-label="go-to-event">
          <OiArrowRight size="1.5rem" />
        </Button>
      </div>
    </div>
  </Popover>
</template>