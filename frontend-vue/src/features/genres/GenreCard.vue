<script setup lang="ts">

defineProps<{
  name: string
}>()

</script>

<template>
  <div class="border-surface border-2 rounded-lg py-1 px-2">
    <span>{{ name }}</span>
  </div>
</template>