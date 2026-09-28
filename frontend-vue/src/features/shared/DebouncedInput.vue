<script setup lang="ts">
import { refDebounced, watchDebounced } from '@vueuse/core';
import { ref, toRef, watch } from 'vue';


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

watchDebounced(
  localValue,
  (val) => {model.value = val},
  {debounce: toRef(props, 'delay')}
)

watch(model, (val) => {
  if (val !== localValue.value) localValue.value = val
})

function flush() {
  model.value = localValue.value
}

</script>

<template>
  <InputText
    fluid
    v-model="localValue"
    :placeholder="placeholder"
    @keyup.enter="flush"
  ></InputText>
</template>