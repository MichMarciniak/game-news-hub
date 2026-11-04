<script setup lang="ts">
import { useFollowedGenres } from '@/composables/genreFollows';
import { OiHeartFill } from '@openvue/openicons-vue';
import { computed } from 'vue';

const props = defineProps<{
  id: string | number,
  name: string
}>()

const {toggleFollowGenre, followedGenreIds} = useFollowedGenres()

const isFollowed = computed(() => followedGenreIds.value.has(String(props.id)))

</script>


<template>
  <div 
    class="flex cursor-pointer transition duration-150 ease-in-out"
    :class="{'scale-90': isFollowed}"
    @click="() => toggleFollowGenre(id)"
    >
    <Button class="relative px-3 py-1 overflow-visible "
      :variant="isFollowed ? 'outlined' : 'solid'"
      size="small"  
    >
      <span >{{ name }}</span>
      <Transition mode="out-in" name="fade">
        <OiHeartFill 
          v-if="isFollowed" 
          class="absolute -right-2.5 -top-2" size="1.8rem" 
        />
      </Transition>
    </Button>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.15s ease-in-out;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>