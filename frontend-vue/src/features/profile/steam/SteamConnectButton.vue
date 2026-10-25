<script setup lang="ts">
import steamImage from "@/assets/steam/sits_large_border.png"

defineEmits<{
  (e: 'connect'): void;
}>();

</script>

<template>
  <Button 
    aria-label="Connect to steam"
    @click="$emit('connect')"
    variant="text"
    class="w-fit h-fit p-0"
  >
    <img :src="steamImage" alt="" class=""/>
  </Button>

</template>