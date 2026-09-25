<script setup lang="ts">
import { ref } from 'vue';

defineProps<{
  title: string
}>()

const open = ref(false)

function toggleOpen() {
  open.value = !open.value
}
</script>

<template>
  <div class="flex flex-col my-2">
    <div class="flex items-center gap-2 cursor-pointer" @click="toggleOpen">
      <h2 class="text-xl">{{ title }}</h2>
    </div>
    <Divider />
    <div class="expand-wrapper" :class="{ 'is-open': open }">
      <div class="expand-inner flex gap-2 flex-wrap">
        <slot />
      </div>
    </div>
  </div>
</template>

<style scoped>
.expand-wrapper {
  display: grid;
  grid-template-rows: 0fr;
  transition: grid-template-rows 0.3s ease;
}

.expand-wrapper.is-open {
  grid-template-rows: 1fr;
}

.expand-inner {
  overflow: hidden;
  min-height: 0;
}
</style>