<script setup lang="ts">
import { useGameService } from '@/features/games/services/useGameService'
import { computed } from 'vue'
import GenreCard from '@/features/genres/GenreCard.vue'
import PlatformGroupCard from '@/features/platforms/PlatformGroupCard.vue'
import { useRoute, useRouter } from 'vue-router'
import GameGrid from '@/features/games/components/GameGrid.vue'
import GameCard from '@/features/games/components/GameCard.vue'
import FollowHeart from '@/features/shared/FollowHeart.vue'
import Button from 'openvue/button'
import { OiHeart, OiHeartFill } from '@openvue/openicons-vue'
import Divider from 'openvue/divider'
import Image from 'openvue/image'

const route = useRoute()
const router = useRouter()

const gameId = String(route.params.id)

const { game, isPending, isFollowed, toggleFollowGame, formatGameType } = useGameService(gameId)

const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><defs><pattern id="stripes" width="20" height="20" patternTransform="rotate(45)" patternUnits="userSpaceOnUse"><line x1="0" y1="0" x2="0" y2="20" stroke="%23cccccc" stroke-width="2"/></pattern></defs><rect width="100%" height="100%" fill="url(%23stripes)"/></svg>'

function goToGame() {
  router.push({ name: 'game-detail', params: { id: game.value?.parentGame?.id } })
}
</script>

<template>
  <div v-if="isPending" class="max-w-6xl w-full mx-auto flex flex-col gap-4">
    <div class="h-4 bg-surface-200 rounded w-1/3 mb-2"></div>
    <div class="h-6 bg-surface-200 rounded w-full mb-4"></div>
  </div>

  <div v-else-if="game" class="max-w-6xl w-full mx-auto flex flex-col gap-4">
    <!-- Image, title, follow -->
    <div class="flex not-sm:flex-col gap-4">
      <div class="flex justify-center shrink-0">
        <Image :src="game.coverUrl ?? TRANSPARENT_IMAGE" class="shrink-0 mr-2 w-48 h-64 object-cover rounded-lg"/>
      </div>
      <div class="flex flex-col gap-2 flex-1">
        <div class="flex items-start justify-between">
          <h2 class="text-2xl font-medium">{{ game.title }}</h2>
          <Button
            :severity="isFollowed ? 'secondary' : 'primary'"
            :outlined="isFollowed"
            @click="toggleFollowGame(game.id as string)"
            class="flex-shrink-0"
          >
            <template v-if="isFollowed">
              <OiHeartFill size="1.2rem"/>
              <span>Following</span>
            </template>
            <template v-else>
              <OiHeart size="1.2rem"/>
              <span>Follow</span>
            </template>
          </Button>
        </div>
        <div class="flex gap-5">
          <span class="text-muted-color">{{ formatGameType(game.type) }}</span>
          <Divider v-if="game.parentGame" layout="vertical"/>
          <span v-if="game.parentGame" @click="goToGame" class="cursor-pointer self-start">{{ game.parentGame.title }}</span>
        </div>
        <span class="text-muted-color-emphasis text-xl">{{ game.releaseDate }}</span>
        <span class="text-muted-color">{{ game.summary }}</span>
      </div>
    </div>

    <!-- platforms, genres, childs -->
    <div class="flex flex-col gap-5">
      <div v-if="game.genres" class="flex gap-3 flex-wrap">
        <GenreCard v-for="g in game.genres ?? []" :key="g.id" :name="g.name ?? ''"/>
      </div>
      <div v-if="game.platformGroups" class="flex gap-3 flex-wrap">
        <PlatformGroupCard v-for="p in game.platformGroups ?? []" :key="String(p.id)" :name="p.name ?? ''" :platforms="p.platforms ?? []" />
      </div>
    </div>

    <div v-if="game.addons?.length != 0" class="flex flex-col">
      <h2 class="text-xl">Addons</h2>
      <div class="px-1 py-3">
        <GameGrid class="max-w-[80%]">
          <GameCard v-for="g in game.addons" :game="g" :key="g.id" />
        </GameGrid>
      </div>
    </div>
  </div>
</template>
