<script lang="ts" setup>
import { isDark, toggleDarkMode } from '@/composables/theme';
import Profile from '@/features/auth/Profile.vue';
import ThemeSwitcher from '@/features/themes/ThemeSwitcher.vue';
import { OiAngleDown, OiAngleRight, OiAsterisk, OiMoon, OiSun } from '@openvue/openicons-vue';
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute()
const isEventsExpanded = ref(false)
const isClosing = ref(false)

const eventsChildren = [
  { route: '/events', label: 'Calendar' },
  { route: '/events/recommended', label: 'Recommended' },
  { route: '/events/search', label: 'Search' },
]

const menuItems = computed(() => [
  { label: 'Events', isEventsToggle: true },
  ...(isEventsExpanded.value || isClosing.value ? [{ isEventsGroup: true }] : []),
  { route: '/games', label: 'Games' },
])

const isEventsActive = computed(() => route.path.startsWith('/events'))

function toggleEvents() {
  if (isEventsExpanded.value) {
    isClosing.value = true
    isEventsExpanded.value = false
    setTimeout(() => { isClosing.value = false }, 150)
  } else {
    isEventsExpanded.value = true
  }
}
</script>

<template>
  <header class="top-0 left-0 w-full flex">
    <Menubar :model="menuItems" class="w-full" breakpoint="632px">
      <template #start>
        <RouterLink 
          to="/" 
          class="flex items-center gap-2 p-1"
          v-slot="{isActive}"
        >
          <OiAsterisk size="32" :class="{'text-primary': isActive}" />
        </RouterLink>
      </template>

      <template #item="{item, props}">
        <RouterLink
          v-if="item.route"
          :to="item.route"
          v-slot="{href, navigate, isActive}"
          custom
        >
          <a 
            :href="href"
            v-bind="props.action"
            @click="navigate"
            :class="{ 'text-primary font-semibold': isActive}"
          >
            <span>{{ item.label }}</span>
          </a>
        </RouterLink>

        <a 
          v-else-if="item.isEventsToggle" 
          @click.stop="toggleEvents"
          v-bind="props.action" 
          class="cursor-pointer flex items-center gap-1"
          :class="{ 'text-primary font-semibold': isEventsActive }"
        >
          <span>{{ item.label }}</span>
          <OiAngleRight v-if="!isEventsExpanded" size="20" />
          <OiAngleDown v-else size="20" />
        </a>

        <div
          v-else-if="item.isEventsGroup"
          class="event-group"
          :class="{ 'event-group-closing': isClosing }"
        >
          <RouterLink
            v-for="child in eventsChildren"
            :key="child.route"
            :to="child.route"
            v-slot="{href, navigate, isActive}"
            custom
          >
            <a
              :href="href"
              @click="navigate"
              class="event-child-link"
              :class="{ 'event-child-active': isActive }"
            >
              {{ child.label }}
            </a>
          </RouterLink>
        </div>
      </template>

      <template #end>
        <div class="flex items-center gap-3">
            <ThemeSwitcher />
          <Button @click="() => toggleDarkMode()" class="p-1" severity="contrast">
            <OiSun v-if="isDark" size="1.5rem" />
            <OiMoon v-else size="1.5rem" />
          </Button>
        
          <Profile />
        </div>
      </template>
    </Menubar>
  </header>
</template>

<style scoped>
.event-group {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem;
  border-radius: 0.5rem;
  background-color: color-mix(in srgb, var(--p-primary-color) 10%, transparent);
  animation: event-group-in 0.18s ease-out;
}

.event-group-closing {
  animation: event-group-out 0.15s ease-in forwards;
}

.event-child-link {
  padding: 0.25rem 0.6rem;
  border-radius: 0.375rem;
  white-space: nowrap;
}

.event-child-active {
  background-color: color-mix(in srgb, var(--p-primary-color) 22%, transparent);
  font-weight: 600;
}

@keyframes event-group-in {
  from { opacity: 0; transform: translateX(-6px); }
  to   { opacity: 1; transform: translateX(0); }
}

@keyframes event-group-out {
  from { opacity: 1; }
  to   { opacity: 0; }
}
</style>