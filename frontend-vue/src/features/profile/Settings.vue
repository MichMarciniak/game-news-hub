<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';
import { computed } from 'vue';


const authStore = useAuthStore()
const {user} = storeToRefs(authStore)


const settingsConf = [
  {key:'showFutureRecommendations', label: 'Show predicted recommendations'}
] as const

// what?
const settingModels = Object.fromEntries(
  settingsConf.map(({ key }) => [
    key,
    computed({
      get: () => user.value?.settings?.[key] ?? false,
      set: (value: boolean) => {
        if (!user.value) return
        user.value.settings[key] = value
        // TODO: mutation
      }
    })
  ])
)


</script>

<template>
  <div class="flex flex-col gap-1">
    <div
      v-for="setting in settingsConf" 
      :key="setting.key"
      class="flex gap-1 items-center" 
    >
      <ToggleSwitch 
        :name="setting.key"   
        v-model="settingModels[setting.key].value"
      />
      <label :for="setting.key">{{ setting.label }}</label>
    </div>
  </div>
</template>