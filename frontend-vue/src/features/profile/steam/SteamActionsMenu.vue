<script setup lang="ts">
import SteamIcon from './SteamIcon.vue';


defineProps<{
  steamId?: string;
}>();

defineEmits<{
  (e: 'importWishlist'): void
  (e: 'importOwned'): void
}>();

</script>

<template>
  <div class="flex flex-col gap-2 ">
    <h2 class="text-semibold text-2xl">Steam</h2>
    <Button
      @click="$emit('importWishlist')"
      class="w-fit"
    >
      Import Wishlist
    </Button>
    <Button
      @click="$emit('importOwned')"
      class="w-fit"
    >
      Import Owned Games
    </Button>
  </div>
</template>