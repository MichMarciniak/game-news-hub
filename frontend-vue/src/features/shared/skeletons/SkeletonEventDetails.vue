<script setup lang="ts">
import GameGrid from '@/features/games/GameGrid.vue';
import SkeletonGameCard from '@/features/shared/skeletons/SkeletonGameCard.vue';

</script>


<template>

  <div class="flex flex-col gap-3">
    <div class="flex justify-between items-center">
      <Skeleton width="50%" height="1.5rem" />
      <Skeleton width="20%" height="1.5rem" />
    </div>
    <Skeleton height="5rem" />
  </div>
  
  <Skeleton height="1.5rem mb-0"/>
  <div class="p-2 mt-0">
    <GameGrid>
      <SkeletonGameCard v-for="i in 12" :key="i" />
    </GameGrid>

  </div>

</template>