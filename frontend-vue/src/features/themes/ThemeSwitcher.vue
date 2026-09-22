<script setup lang="ts">
import { activePreset, applyPreset } from '@/composables/theme';
import { presets } from './theme';



const presetEntries = Object.entries(presets) as [keyof typeof presets, typeof presets[keyof typeof presets]][];
</script>

<template>
  <button
    v-for="[name, def] in presetEntries"
    :key="name"
    class="color-dot cursor-pointer"
    :class="{ active: activePreset === name }"
    :style="{ backgroundColor: def.color }"
    :title="name"
    :aria-label="`Theme ${name}`"
    @click="applyPreset(name)"
  >

</button>

  
</template>

<style lang="css" scoped>
.color-dot {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  border: 2px solid transparent;
  cursor: pointer;
  padding: 0;
  transition: transform 0.15s ease, border-color 0.15s ease;
}
.color-dot:hover {
  transform: scale(1.1);
}
.color-dot.active {
  border-color: var(--p-text-color, #000);
  box-shadow: 0 0 0 2px var(--p-content-background, #fff);
}
</style>