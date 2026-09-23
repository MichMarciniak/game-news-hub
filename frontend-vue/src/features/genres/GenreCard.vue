<script setup lang="ts">

defineProps<{
  name: string
}>()

</script>

<template>
  <div class="border rounded-lg py-1 px-2">
    <span>{{ name }}</span>
  </div>
</template>