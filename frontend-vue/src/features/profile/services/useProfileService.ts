import { usePatchUsersMeSettings } from '@/api/generated'
import { useAuthStore } from '@/stores/auth'
import { storeToRefs } from 'pinia'

export function useProfileService() {
  const authStore = useAuthStore()
  const { user } = storeToRefs(authStore)

  const { mutate: patchSettings } = usePatchUsersMeSettings()

  const settingsConf = [
    { key: 'showFutureRecommendations', label: 'Show predicted recommendations' }
  ] as const

  type SettingKey = typeof settingsConf[number]['key']

  const updateSetting = (key: SettingKey, value: boolean) => {
    if (!user.value) return
    if (!user.value.settings) return

    const previousValue = user.value.settings?.[key]
    user.value.settings[key] = value

    patchSettings({ data: { [key]: value } }, {
      onError: () => {
        if (user.value?.settings) {
          user.value.settings[key] = previousValue
        }
      }
    })
  }

  const logout = () => {
    authStore.logout()
  }

  const logoutAll = () => {
    authStore.logoutAll()
  }

  return {
    user,
    isAuthenticated: authStore.isAuthenticated,
    settingsConf,
    updateSetting,
    logout,
    logoutAll
  }
}
