<script setup lang="ts">
import { useProfileService } from '@/features/profile/services/useProfileService'
import ToggleSwitch from 'openvue/toggleswitch'

const { user, updateSetting, settingsConf } = useProfileService()
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