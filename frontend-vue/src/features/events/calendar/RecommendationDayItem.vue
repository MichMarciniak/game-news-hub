<script setup lang="ts">
import type { RecommendationDto } from '@/api/generated/models';
import { formatTime } from '@/utils/date';
import { OiChartBar, OiClock } from '@openvue/openicons-vue';
import { Badge } from 'openvue';
import { useRouter } from 'vue-router';

const props = defineProps<{
  event: RecommendationDto
}>();

const router = useRouter();

function goToEvent() {
  router.push({ name: 'event-detail', params: { id: props.event.id } });
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
    followed_genre: 'Genre',
    followed_platform: 'Platform',
    followed_platform_group: 'Platform group',
    followed_series: 'Series',
    followed_game: 'Game',
    high_momentum: 'Momentum',
    popular: 'Popular',
  };
  return labels[code] || code.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
}
</script>

<template>
  <div class="px-2 py-2 mx-2 my-2 cursor-pointer hover:bg-highlight transition-colors duration-100 ease-in-out rounded-md" @click="goToEvent">
    <div class="flex items-start justify-between gap-2">
      <div class="flex-1 min-w-0">
        <div class="flex items-center gap-1.5 mb-1">
          <span class="text-sm font-medium truncate">{{ props.event.name }}</span>
          <Badge v-if="props.event.score" :value="Math.round(Number(props.event.score.finalScore || 0))" :severity="getScoreColor(props.event.score.finalScore)" class="px-1.5 py-0.5 text-xs" />
        </div>
        <p class="text-xs text-muted-color flex items-center gap-1">
          <OiClock size="0.875rem" />
          <span>{{ formatTime(props.event.startTime) }}</span>
          <span v-if="props.event.endTime"> – {{ formatTime(props.event.endTime) }}</span>
        </p>
      </div>
    </div>

    <!-- Labels -->
    <div v-if="props.event.score?.labels?.length" class="flex flex-wrap gap-1 mt-1.5">
      <Badge v-for="(label, idx) in props.event.score.labels" :key="label.code ?? idx" :label="getLabelText(label.code ?? '') + (label.count ? ' ×' + label.count : '')" severity="contrast" class="text-[10px] px-1.5 py-0.5" />
    </div>
  </div>
</template>