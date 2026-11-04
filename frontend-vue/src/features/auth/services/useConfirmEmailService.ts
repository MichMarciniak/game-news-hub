import { usePostAuthConfirmEmail } from '@/api/generated'
import type { EmailConfirmDto, ProblemDetails } from '@/api/generated/models'
import { useToast } from 'openvue'
import { AxiosError } from 'axios'

export function useConfirmEmailService() {
  const { mutate: confirmEmail } = usePostAuthConfirmEmail()
  const toast = useToast()

  const confirmUserEmail = (values: { userId: string | number; token: string }) => {
    const data: EmailConfirmDto = { userId: values.userId, token: values.token }

    confirmEmail(
      { data },
      {
        onSuccess: () => {
          toast.add({ severity: 'success', summary: 'Email confirmed', detail: 'You can now log in', life: 3000 })
        },
        onError: () => {
          toast.add({ severity: 'error', summary: 'Error', detail: 'Failed to confirm email', life: 3000 })
        }
      }
    )
  }

  return { confirmUserEmail }
}
