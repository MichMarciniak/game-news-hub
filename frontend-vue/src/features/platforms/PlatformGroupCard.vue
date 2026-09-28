<script setup lang="ts">
import type { PlatformDto } from '@/api/generated/models';
import { ref } from 'vue';


defineProps<{
  name: string,
  platforms: PlatformDto[]
}>()

const open = ref(false)
const target = ref(null)

</script>
<template>
  <div ref="target">
    <button 
      @pointerover="open=true"  
      @pointerleave="open = false"
      @focusin="open = true"
      @focusout="open = false"
      class=" border-surface border-2 rounded-lg py-1 px-2"
    >
      <span>{{ name }}</span>
  </button>
    <div :class="{'hidden': !open}" 
      class="flex flex-col gap-1 absolute bg-emphasis p-2 rounded-lg">
      <span v-for="p in platforms" :key="p.id">{{ p.name }}</span>
    </div>
  </div>


</template>