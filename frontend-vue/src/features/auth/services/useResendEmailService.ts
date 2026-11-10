import { usePostAuthResendEmail } from '@/api/generated'
import { useToast } from 'openvue'
import { useCooldown } from '@/composables/useCooldown'

export function useResendEmailService() {
  const { mutate: resendEmail } = usePostAuthResendEmail()
  const toast = useToast()
  const { secondsLeft, isDisabled, start: startCooldown } = useCooldown(60)

  const resendConfirmationEmail = (email: string) => {
    resendEmail(
      { data: { email } },
      {
        onSuccess: () => {
          toast.add({ severity: 'success', summary: 'Email sent', life: 3000 })
          startCooldown()
        },
        onError: () => {
          toast.add({ severity: 'error', summary: 'Error', detail: 'Error sending email', life: 3000 })
        }
      }
    )
  }

  return { resendConfirmationEmail, isResendDisabled: isDisabled, secondsLeft }
}