<script setup lang="ts">
const props = withDefaults(defineProps<{
  isIncomplete?: boolean
}>(), {
  isIncomplete: false
})
</script>

<template>
  <div v-if="isIncomplete">
    <Panel class="h-full" :pt="{ header: '!pb-0' }">
      <template #header>
        <h3 class="text-xl">Finish account creation</h3>
      </template>
      <div class="flex flex-col">
        <Divider />
        <div class="flex flex-col items-center gap-5">
          <span class="text-muted-color">Follow genres and platforms to personalize your recommendations.</span>
          <RouterLink to="/me">
            <Button variant="outlined">Go to profile</Button>
          </RouterLink>
        </div>
      </div>
    </Panel>
  </div>
</template>
