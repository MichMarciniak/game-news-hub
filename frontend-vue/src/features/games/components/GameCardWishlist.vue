<script setup lang="ts">
import type { GameListItemDto } from '@/api/generated/models';
import { useWishlist } from '@/composables/useWishlist'
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { OiStar, OiStarFill } from '@openvue/openicons-vue'
import Button from 'openvue/button'

const props = defineProps<{
  game: GameListItemDto
}>()

const router = useRouter()

const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><defs><pattern id="stripes" width="20" height="20" patternTransform="rotate(45)" patternUnits="userSpaceOnUse"><line x1="0" y1="0" x2="0" y2="20" stroke="%23cccccc" stroke-width="2"/></pattern></defs><rect width="100%" height="100%" fill="url(%23stripes)"/></svg>';

const { wishlistIds, toggleWishlist } = useWishlist()

const isWishlisted = computed(() => wishlistIds.value.has(String(props.game.id)))

function goToGame() {
  router.push( {name: 'game-detail', params: { id: props.game.id } } )
}

</script>

<template>
  <div class="relative cursor-pointer transition-all duration-150 hover:ring-2 hover:ring-primary rounded-lg overflow-hidden" @click="goToGame">
    <img v-if="props.game.coverUrl" :src="props.game.coverUrl" alt="asd" />

    <Image v-else :src="TRANSPARENT_IMAGE"/>
    <Button
      :severity="isWishlisted ? 'secondary' : 'outline'"
      :outlined="!isWishlisted"
      @click.stop="toggleWishlist(props.game.id as string)"
      class="absolute top-1 right-1 p-1 text-xs"
      size="small"
    >
      <template v-if="isWishlisted">
        <OiStarFill size="1.2rem" class="text-yellow-500"/>
      </template>
      <template v-else>
        <OiStar size="1.2rem"/>
      </template>
    </Button>
    <div class="absolute bottom-0 right-0 left-0 gradient pt-4">
      <span class="block text-center font-medium line-clamp-2">{{ props.game.title }}</span>
    </div>
  </div>
</template>
