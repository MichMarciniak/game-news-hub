
<script lang="ts" setup>
import { useGetGameGameId } from '@/api/generated';
import { computed } from 'vue';
import GenreCard from '../genres/GenreCard.vue';
import PlatformGroupCard from '../platforms/PlatformGroupCard.vue';
import { useRoute, useRouter } from 'vue-router';
import GameGrid from './GameGrid.vue';
import GameCard from './GameCard.vue';

const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><defs><pattern id="stripes" width="20" height="20" patternTransform="rotate(45)" patternUnits="userSpaceOnUse"><line x1="0" y1="0" x2="0" y2="20" stroke="%23cccccc" stroke-width="2"/></pattern></defs><rect width="100%" height="100%" fill="url(%23stripes)"/></svg>';

const route = useRoute()
const router = useRouter()

const gameId = computed(() => String(route.params.id))

const { data: gameResponse } = useGetGameGameId(gameId)
const game = computed(() => (gameResponse.value))

function goToGame() {
  router.push({ name: 'game-detail', params: { id: game.value?.parentGameId } })
  console.log(game.value?.parentGameId)
}

</script>

<template>
  <div class="max-w-6xl w-full mx-auto flex flex-col gap-4">
    <!-- Image, title, follow -->
    <div class="flex not-sm:flex-col">
      <div class="flex justify-center shrink-0">
        <Image :src="TRANSPARENT_IMAGE" class="shrink-0 mr-2"/>
      </div>
      <div class="flex flex-col gap-2">
        <h2 class="text-2xl font-medium">{{ game?.title }}</h2>
        <span class="text-muted-color-emphasis text-xl">release date placeholder</span>
        <span class="text-muted-color">{{ game?.summary }}</span>
        <span class="text-muted-color">{{ game?.type }}</span>
        <span v-if="game?.parentGameId" @click="goToGame" class="cursor-pointer self-start">Parent game name</span>

      </div>
    </div>

    <!-- platforms, genres, childs -->
    <div class="flex flex-col gap-5">
      <div class="flex gap-3 flex-wrap">
        <GenreCard v-for="g in game?.genres" :key="g.id" :name="g.name"/>
      </div>
      <div class="flex gap-3 flex-wrap">
        <PlatformGroupCard v-for="p in game?.platformGroups" :key="p.id" :name="p.name!" :platforms="p.platforms!" />
      </div>
    </div>

    <div v-if="game?.addons?.length != 0" class="flex flex-col">
      <h2 class="text-xl">Addons</h2>
      <div class="px-1 py-3">
        <GameGrid class="max-w-[80%]">
          <GameCard v-for="g in game?.addons" :game="g" :key="g.id" />
        </GameGrid>

      </div>
    </div>

  </div>
</template>