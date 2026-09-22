<script setup lang="ts">
import { OiHeart, OiHeartFill } from '@openvue/openicons-vue';


withDefaults(
  defineProps<{
    isFollowed: boolean
    size?: number | string
  }>(),
  {
    size: '1.5rem',
  }
)

defineEmits<{
  (e: 'click', event: MouseEvent): void
}>()
</script>

<template>
  <Button
    variant="text"
    class="p-0 m-0 hover:bg-transparent self-center shrink-0"
    @click="$emit('click', $event)"
  >
    <Transition mode="out-in" name="heart-pop">
      <OiHeartFill v-if="isFollowed" :key="'filled'" :size="size" />
      <OiHeart v-else :key="'empty'" :size="size" />
    </Transition>
  </Button>
</template>

<style scoped>
.heart-pop-enter-active,
.heart-pop-leave-active {
  transition: all 0.15s ease-in-out;
}

.heart-pop-enter-from,
.heart-pop-leave-to {
  opacity: 0;
  transform: scale(0.6);
}
</style>