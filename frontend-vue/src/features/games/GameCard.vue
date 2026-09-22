<script setup lang="ts">
import type { GameListItemDto } from '@/api/generated/models';
import FollowHeart from '../shared/FollowHeart.vue';
import { useFollowedGames } from '@/composables/gameFollows';
import { computed } from 'vue';
import { useRouter } from 'vue-router';

const props = defineProps<{
  game: GameListItemDto
}>()

const router = useRouter()

const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"></svg>'
// const TRANSPARENT_IMAGE =
//   'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><rect width="100%" height="100%" fill="white"/></svg>'

const {followedGameIds, toggleFollowGame} = useFollowedGames()

const isFollowed = computed(() => followedGameIds.value.has(String(props.game.id)))

function goToGame() {
  router.push( {name: 'game-detail', params: { id: props.game.id } } )
}

</script>

<template>
  <div class="relative cursor-pointer transition-all duration-150 hover:ring-2 hover:ring-primary rounded-lg" @click="goToGame">
    <!-- <img :src="props.game.coverUrl as string" alt="asd" /> -->
    <Image :src="TRANSPARENT_IMAGE"/>
    <FollowHeart 
      :is-followed="isFollowed"
      @click.stop="toggleFollowGame(game.id as string)"
      class="absolute top-1 right-1"   
    />
    <div class="absolute bottom-0 right-0 left-0">
      <span class="block text-center font-medium line-clamp-2">{{ props.game.title }}</span>

    </div>
  </div>
</template>