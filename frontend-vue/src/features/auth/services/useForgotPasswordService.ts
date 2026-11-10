import { usePostAuthForgotPassword } from '@/api/generated'
import { resetPasswordRequestSchema } from '@/features/auth/schemas'
import type { EmailDto } from '@/api/generated/models'
import { useToast } from 'openvue'

export function useForgotPasswordService() {
  const { mutate: sendForgotPassword, isPending: isForgotPending } = usePostAuthForgotPassword()
  const toast = useToast()

  const forgotPassword = async (rawValues: unknown) => {
    const values = resetPasswordRequestSchema.parse(rawValues) as EmailDto

    sendForgotPassword(
      { data: values },
      {
        onSuccess: () => {
          toast.add({ severity: 'success', summary: 'Email sent', detail: 'If account exists, you will receive instructions', life: 3000 })
        }
      }
    )
  }

  return { forgotPassword, isForgotPending }
}
