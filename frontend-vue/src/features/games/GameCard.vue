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

// const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"></svg>'
const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><defs><pattern id="stripes" width="20" height="20" patternTransform="rotate(45)" patternUnits="userSpaceOnUse"><line x1="0" y1="0" x2="0" y2="20" stroke="%23cccccc" stroke-width="2"/></pattern></defs><rect width="100%" height="100%" fill="url(%23stripes)"/></svg>';
// const TRANSPARENT_IMAGE =
//   'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><rect width="100%" height="100%" fill="white"/></svg>'

const {followedGameIds, toggleFollowGame} = useFollowedGames()

const isFollowed = computed(() => followedGameIds.value.has(String(props.game.id)))

function goToGame() {
  router.push( {name: 'game-detail', params: { id: props.game.id } } )
}

</script>

<template>
  <div class="relative cursor-pointer transition-all duration-150 hover:ring-2 hover:ring-primary rounded-lg overflow-hidden" @click="goToGame">
    <img v-if="props.game.coverUrl" :src="props.game.coverUrl" alt="asd" /> 
    
    <Image v-else :src="TRANSPARENT_IMAGE"/>
    <FollowHeart 
      :is-followed="isFollowed"
      :size="'1.8rem'"
      @click.stop="toggleFollowGame(game.id as string)"
      class="absolute top-1 right-1"   
    />
    <div class="absolute bottom-0 right-0 left-0 gradient pt-4">
      <span class="block text-center font-medium line-clamp-2">{{ props.game.title }}</span>

    </div>
  </div>
</template>

<style lang="css" scoped>
.gradient {
  background-image: linear-gradient(
    to top,
    var(--p-primary-contrast-color),
    /* var(--p-highlight-color), */
    transparent
  );
}
</style>