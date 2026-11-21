<script setup lang="ts">
import SteamIcon from './SteamIcon.vue';
import Button from 'openvue/button'

const emit = defineEmits<{
  (e: 'import-wishlist'): void
  (e: 'import-owned'): void
}>()
</script>

<template>
  <div class="flex flex-col gap-2">
    <Button
      @click="emit('import-wishlist')"
      class="w-fit"
    >
      Import Wishlist
    </Button>
    <Button
      @click="emit('import-owned')"
      class="w-fit"
    >
      Import Owned Games
    </Button>
  </div>
</template>
