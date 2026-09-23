<script setup lang="ts">
import { OiAngleLeft, OiAngleRight } from '@openvue/openicons-vue';

const ICON_SIZE = '2rem'

defineProps<{
  title: string,
  currentView: string,
  viewOptions: {label: string, value: string}[],
  today: boolean
}>()

const emit = defineEmits<{
  prev: [],
  next: [],
  today: [],
  'update:currentView': [value: string]
}>()


</script>

<template>
  <div class="flex justify-between mb-2">
    <!-- prev,next today -->
    <div class="flex gap-2 items-center">
      <div class="flex items-center">
        <Button class="p-0 rounded-r-none" @click="emit('prev')" >
          <OiAngleLeft :size="ICON_SIZE" />
        </Button>
        <Button class="p-0 rounded-l-none" @click="emit('next')">
          <OiAngleRight :size="ICON_SIZE"/>
        </Button>
      </div>
      <Button class="px-2 py-0" @click="emit('today')" :disabled="today">
        <div class="h-8 flex items-center">
          <span class="">Today</span>
        </div>
      </Button>
      
    </div>

    <!-- Title -->
    <div>
      <h2 class="text-2xl">
        {{ title }}
      </h2>
    </div>

     <!-- week,month view -->
    <div>
      <Button
        v-for="(option, index) in viewOptions"
        :key="option.value"
        class="px-3 py-0"
        :class="[
          index === 0 ? 'rounded-r-none' : '',
          index === viewOptions.length - 1 ? 'rounded-l-none' : '',
          index !== 0 && index !== viewOptions.length - 1 ? 'rounded-none' : '',
          index !== 0 ? '-ml-px' : '',
        ]"
        :variant="option.value === currentView ? 'outlined' : 'solid'"
        :disabled="option.value === currentView"
        @click="emit('update:currentView', option.value)"
      >
        <div class="h-8 flex items-center">
          <span>{{ option.label }}</span>
        </div>
      </Button>
    </div>

  </div>

</template>