import { usePostAuthChangePassword } from '@/api/generated'
import { changePasswordSchema } from '@/features/auth/schemas'
import type { ChangePasswordDto, ProblemDetails } from '@/api/generated/models'
import { useToast } from 'openvue'
import { ref } from 'vue'
import { AxiosError } from 'axios'

export function useChangePasswordService() {
  const { mutate: changePassword, isError: isChangeError } = usePostAuthChangePassword()
  const changePasswordError = ref<string | null>(null)
  const toast = useToast()

  const changeUserPassword = async (rawValues: unknown) => {
    changePasswordError.value = null
    const values = changePasswordSchema.parse(rawValues) as ChangePasswordDto

    changePassword({ data: values }, {
      onSuccess: () => {
        toast.add({ severity: 'success', summary: 'Password changed' })
      },
      onError: (error: unknown) => {
        const err = error as AxiosError<ProblemDetails>
        changePasswordError.value = err.response?.data?.title || 'Password change error'
      }
    })
  }

  return { changeUserPassword, changePasswordError, isChangeError }
}
