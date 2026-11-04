import { usePostAuthResendEmail } from '@/api/generated'
import { useToast } from 'openvue'
import { computed, ref } from 'vue'

export function useResendEmailService() {
  const { mutate: resendEmail } = usePostAuthResendEmail()
  const toast = useToast()
  const COOLDOWN_SECONDS = 60
  const secondsLeft = ref(0)
  let intervalId: number | undefined

  const isResendDisabled = computed(() => secondsLeft.value > 0)

  const startCooldown = () => {
    secondsLeft.value = COOLDOWN_SECONDS
    intervalId = setInterval(() => {
      secondsLeft.value -= 1
      if (secondsLeft.value <= 0) {
        clearInterval(intervalId)
        intervalId = undefined
      }
    }, 1000)
  }

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

  return { resendConfirmationEmail, isResendDisabled, secondsLeft }
}
