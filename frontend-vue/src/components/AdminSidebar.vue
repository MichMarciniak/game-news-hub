<script setup lang="ts">
import { OiTimes } from '@openvue/openicons-vue';
import { ref } from 'vue';

const navItems = [
  {label: 'Reassign event series', to: '/admin/event-assign'},
  {label: 'Reassign platforms', to: '/admin/platform-assign'}
]


const open = ref(false)

function openPanel() {
  open.value = true
}

function closePanel() {
  open.value = false
}

defineExpose({
  openPanel
})

</script>


<template>
  <Transition mode="out-in" name="slide">
    <div v-if="open" class=" border-surface border-r-2 absolute left-0 top-0 bottom-0 bg-primary-contrast z-1">
      <div class="flex items-center gap-[2rem] p-2">
        <h2 class="text-xl">Admin Panel</h2>
        <Button 
          variant="outlined" 
          @click="closePanel"
          class="p-1 rounded-xl border-0"
  
        >
          <OiTimes size="1.5rem" />
        </Button>
      </div>
      <ul class="flex flex-col m-2 gap-3 mt-5">
        <li
          v-for="item in navItems" 
          :key="item.to"
          class=" hover:bg-highlight transition-all duration-100 ease-in-out rounded-lg border-surface border-l-3 hover:border-l-5 box-border"
        >
          <RouterLink 
            :to="item.to" 
            class="flex grow px-2 py-1"
            v-slot="{isActive}"
          >
            <span :class="{'text-primary': isActive}" class=" transition-colors duration-150 ease-in-out">{{ item.label }}</span>
          </RouterLink>

        </li>

  
      </ul>
    </div>

  </Transition>
</template>

<style scoped>
.slide-enter-active,
.slide-leave-active {
  transition: all 0.25s ease;
}

.slide-enter-from,
.slide-leave-to {
  transform: translateX(-100%);
}
</style>