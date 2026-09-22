<script setup lang="ts">
import { refDebounced, watchDebounced } from '@vueuse/core';
import { ref, watch } from 'vue';


const props = withDefaults(
  defineProps<{
    delay?: number
    placeholder: string
  }>(),
  {
    delay: 400,
    placeholder: 'Search...'
  }
)

const model = defineModel<string>({default: ''})

const localValue = ref(model.value)

const debouncedValue = refDebounced(localValue, props.delay)

watchDebounced(debouncedValue, (val) => {
  model.value = val
})

watch(
  () => model.value,
  (newVal) => {
    if (newVal !== localValue.value) {
      localValue.value = newVal
    }
  }
)

</script>

<template>
  <InputText
    fluid
    v-model="localValue"
    :placeholder="placeholder"
  ></InputText>
</template>