import { usePostAuthRegister } from '@/api/generated'
import { registerSchema } from '@/features/auth/schemas'
import type { RegisterDto, ProblemDetails } from '@/api/generated/models'
import { useToast } from 'openvue'
import { ref } from 'vue'
import { AxiosError } from 'axios'

export function useRegisterService() {
  const { mutateAsync: registerAsync, isPending: isRegisterPending } = usePostAuthRegister()
  const registerError = ref<string | null>(null)
  const toast = useToast()

  const registerUser = async (rawValues: unknown) => {
    registerError.value = null
    const values = registerSchema.parse(rawValues) as RegisterDto

    try {
      await registerAsync({ data: values })
      toast.add({ severity: 'success', summary: 'Account created', detail: 'Check email to confirm registration', life: 3000 })
      return { success: true, email: values.email }
    } catch (error: unknown) {
      const err = error as AxiosError<ProblemDetails>
      registerError.value = err.response?.data?.title || 'Registration error'
      return { success: false, error: registerError.value }
    }
  }

  return { registerUser, isRegisterPending, registerError }
}
