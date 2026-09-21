<script setup lang="ts">
import type { GameListItemDto } from '@/api/generated/models';

const props = defineProps<{
  game: GameListItemDto
}>()

const TRANSPARENT_IMAGE = 'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"></svg>'
// const TRANSPARENT_IMAGE =
//   'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="264" height="352"><rect width="100%" height="100%" fill="white"/></svg>'
</script>

<template>
  <div class="relative">
    <!-- <img :src="props.game.coverUrl as string" alt="asd" /> -->
     <Image :src="TRANSPARENT_IMAGE"/>
    <div class="absolute bottom-0 right-0 left-0">
      <span class="block text-center font-medium line-clamp-2">{{ props.game.title }}</span>

    </div>
  </div>
</template>