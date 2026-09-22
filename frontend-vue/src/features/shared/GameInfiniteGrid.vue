<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useIntersectionObserver } from '@vueuse/core'
import GameCard from '@/features/games/GameCard.vue'
import GameGrid from '@/features/games/GameGrid.vue'
import type { GameListItemDto } from '@/api/generated/models';

const props = withDefaults(defineProps<{
  games: GameListItemDto[]
  pageSize?: number
}>(), {
  pageSize: 24,
})

const visibleCount = ref(props.pageSize)

// reset przy zmianie listy (np. inny event/tab)
watch(() => props.games, () => {
  visibleCount.value = props.pageSize
})

const visibleGames = computed(() => props.games.slice(0, visibleCount.value))
const hasMore = computed(() => visibleCount.value < props.games.length)

function loadMore() {
  visibleCount.value += props.pageSize
}

const sentinel = ref<HTMLElement | null>(null)

useIntersectionObserver(sentinel, ([entry]) => {
  if (entry?.isIntersecting && hasMore.value) {
    loadMore()
  }
})
</script>

<template>
  <div class="flex flex-col gap-4">
    <GameGrid>
      <GameCard v-for="game in visibleGames" :key="game.id" :game="game" />
    </GameGrid>

    <div v-if="hasMore" ref="sentinel" class="h-4" />
  </div>
</template>