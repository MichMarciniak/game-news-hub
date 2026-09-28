<script setup lang="ts">
import { OiArrowDown, OiArrowUp } from '@openvue/openicons-vue';
const ICON_SIZE = '1.5rem'

const isDescending = defineModel<boolean | undefined>({ required: true })
</script>

<template>
  <Button
    class="h-[2.7rem]! aspect-square p-0!"
    variant="outlined"
    :aria-label="isDescending ? 'Descending' : 'Ascending'"
    @click="isDescending = !isDescending"
  >
    <OiArrowDown v-if="isDescending" title="descending" :size="ICON_SIZE" />
    <OiArrowUp v-else title="ascending" :size="ICON_SIZE" />
  </Button>
</template>