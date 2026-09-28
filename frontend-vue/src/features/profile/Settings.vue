<script setup lang="ts">
import { usePatchUsersMeSettings } from '@/api/generated'
import { useAuthStore } from '@/stores/auth'
import { storeToRefs } from 'pinia'

const authStore = useAuthStore()
const { user } = storeToRefs(authStore)

const {mutate: patchSettings} = usePatchUsersMeSettings();

const settingsConf = [
  { key: 'showFutureRecommendations', label: 'Show predicted recommendations' }
] as const

type SettingKey = typeof settingsConf[number]['key']

function updateSetting(key: SettingKey, value: boolean) {
  if (!user.value) return
  if (!user.value.settings) return
  user.value.settings[key] = value
  // TODO: wywołanie akcji / mutacji na backend
  const data = {
    [key]: value
  }
  patchSettings({data}, {
    onError: () => {
      user.value!.settings![key] = !value
    }
  })
}
</script>

<template>
  <div class="flex flex-col gap-1">
    <div 
      v-for="setting in settingsConf" 
      :key="setting.key"
      class="flex gap-1 items-center" 
    >
      <ToggleSwitch 
        :inputId="setting.key"   
        :model-value="user?.settings?.[setting.key] ?? false"
        @update:model-value="val => updateSetting(setting.key, val)"
      />
      <label :for="setting.key">{{ setting.label }}</label>
    </div>
  </div>
</template>