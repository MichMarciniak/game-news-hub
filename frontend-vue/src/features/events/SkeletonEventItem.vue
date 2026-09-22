<script setup lang="ts">
</script>

<template>
  <!-- Desktop -->
  <div class="hidden md:grid grid-cols-[1rem_1fr_11rem_11rem_2.7rem] items-center gap-4 rounded-lg px-4 py-2">
    <Skeleton shape="circle" size="0.625rem" />
    <Skeleton width="60%" height="1rem" />
    <Skeleton width="7rem" height="0.875rem" />
    <Skeleton width="7rem" height="0.875rem" />
    <Skeleton shape="circle" size="1.5rem" class="justify-self-end" />
  </div>

  <!-- Mobile -->
  <div class="md:hidden flex items-start justify-between gap-3 px-0 py-3">
    <div class="min-w-0 flex-1">
      <div class="flex items-center gap-2 mb-2">
        <Skeleton shape="circle" size="0.625rem" />
        <Skeleton width="70%" height="1rem" />
      </div>
      <Skeleton width="9rem" height="0.875rem" />
    </div>
    <Skeleton shape="circle" size="1.5rem" />
  </div>
</template>