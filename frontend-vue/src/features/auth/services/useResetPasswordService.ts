import { usePostAuthResetPassword } from '@/api/generated'
import { resetPasswordSchema } from '@/features/auth/schemas'
import type { ResetPasswordDto, ProblemDetails } from '@/api/generated/models'
import { useToast } from 'openvue'
import { ref } from 'vue'
import { AxiosError } from 'axios'

export function useResetPasswordService() {
  const { mutateAsync: resetPasswordAsync, isError: isResetError } = usePostAuthResetPassword()
  const resetError = ref<string | null>(null)
  const toast = useToast()

  const resetUserPassword = async (values: { userId: string | number; token: string; password: string }) => {
    resetError.value = null
    const parsed = resetPasswordSchema.parse({ password: values.password }) as { password: string }

    const data: ResetPasswordDto = {
      userId: values.userId,
      token: values.token,
      password: parsed.password
    }

    try {
      await resetPasswordAsync({ data })
      toast.add({ severity: 'success', summary: 'Password changed', detail: 'Log in with your new password', life: 3000 })
      return { success: true }
    } catch (error: unknown) {
      const err = error as AxiosError<ProblemDetails>
      resetError.value = err.response?.data?.title || 'Password reset error'
      return { success: false, error: resetError.value }
    }
  }

  return { resetUserPassword, resetError, isResetError }
}
