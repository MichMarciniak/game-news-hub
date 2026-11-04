import { useAuthStore } from '@/stores/auth'
import { loginSchema } from '@/features/auth/schemas'
import type { LoginDto } from '@/api/generated/models'
import { useToast } from 'openvue'
import { ref } from 'vue'

export function useLoginService() {
  const authStore = useAuthStore()
  const toast = useToast()
  const loginError = ref<string | null>(null)

  const login = async (rawValues: unknown, redirectPath?: string) => {
    loginError.value = null
    const values = loginSchema.parse(rawValues) as LoginDto

    try {
      await authStore.login(values)
      toast.add({ severity: 'success', summary: 'Logged in', detail: 'Welcome back', life: 3000 })
      return { success: true, redirectTo: redirectPath || '/' }
    } catch {
      loginError.value = 'Invalid email or password'
      return { success: false, error: loginError.value }
    }
  }

  return { login, loginError }
}
