import { useAuthStore } from '@/stores/auth'
import {
  usePostAuthRegister,
  usePostAuthForgotPassword,
  usePostAuthResetPassword,
  usePostAuthConfirmEmail,
  usePostAuthResendEmail,
  usePostAuthChangePassword
} from '@/api/generated'
import type {
  LoginDto,
  RegisterDto,
  EmailDto,
  ResetPasswordDto,
  EmailConfirmDto,
  ChangePasswordDto,
  ProblemDetails
} from '@/api/generated/models'
import {
  loginSchema,
  registerSchema,
  resetPasswordRequestSchema,
  resetPasswordSchema,
  changePasswordSchema
} from '@/features/auth/schemas'
import { useToast } from 'openvue'
import { useRouter, useRoute } from 'vue-router'
import { computed, ref } from 'vue'
import { AxiosError } from 'axios'

export function useAuthService() {
  const authStore = useAuthStore()
  const router = useRouter()
  const route = useRoute()
  const toast = useToast()

  // ---------- Login ----------
  const loginError = ref<string | null>(null)

  const login = async (rawValues: unknown) => {
    loginError.value = null
    const values = loginSchema.parse(rawValues) as LoginDto

    try {
      await authStore.login(values)
      toast.add({ severity: 'success', summary: 'Logged in', detail: 'Welcome back', life: 3000 })
      const redirect = route.query.redirectPath as string | undefined
      await router.push(redirect || '/')
      return { success: true }
    } catch {
      loginError.value = 'Invalid email or password'
      return { success: false, error: loginError.value }
    }
  }

  // ---------- Register ----------
  const { mutate: register, isPending: isRegisterPending, isError: isRegisterError } = usePostAuthRegister()
  const registerError = ref<string | null>(null)

  const registerUser = async (rawValues: unknown) => {
    registerError.value = null
    const values = registerSchema.parse(rawValues) as RegisterDto

    register(
      { data: values },
      {
        onSuccess: () => {
          toast.add({ severity: 'success', summary: 'Account created', detail: 'Check email to confirm registration', life: 3000 })
          router.push({ path: '/confirm-email-sent', query: { email: btoa(values.email) } })
        },
        onError: (error: unknown) => {
          const err = error as AxiosError<ProblemDetails>
          registerError.value = err.response?.data?.title || 'Błąd rejestracji'
        }
      }
    )
  }

  // ---------- Forgot password ----------
  const { mutate: sendForgotPassword, isPending: isForgotPending } = usePostAuthForgotPassword()

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

  // ---------- Reset password ----------
  const { mutate: resetPassword, isError: isResetError } = usePostAuthResetPassword()
  const resetError = ref<string | null>(null)

  const resetUserPassword = async (rawValues: unknown) => {
    resetError.value = null
    const values = resetPasswordSchema.parse(rawValues) as { password: string }

    const userId = route.query.userId as string | number
    const token = route.query.token as string

    const data: ResetPasswordDto = {
      userId,
      token,
      password: values.password
    }

    resetPassword(
      { data },
      {
        onSuccess: () => {
          toast.add({ severity: 'success', summary: 'Password changed', detail: 'Log in with your new password', life: 3000 })
          router.push({ path: '/login' })
        },
        onError: (error: unknown) => {
          const err = error as AxiosError<ProblemDetails>
          resetError.value = err.response?.data?.title || 'Błąd resetowania hasła'
        }
      }
    )
  }

  // ---------- Confirm email ----------
  const { mutate: confirmEmail } = usePostAuthConfirmEmail()

  const confirmUserEmail = () => {
    const userId = route.query.userId as string | number
    const token = route.query.token as string

    const data: EmailConfirmDto = { userId, token }

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

  // ---------- Resend email ----------
  const { mutate: resendEmail } = usePostAuthResendEmail()
  const COOLDOWN_SECONDS = 60
  const secondsLeft = ref(0)
  let intervalId: number | undefined

  const isResendDisabled = computed(() => secondsLeft.value > 0)
  const userEmail = computed(() => {
    const email = route.query.email as string | undefined
    return email ? atob(email) : ''
  })

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

  const resendConfirmationEmail = () => {
    resendEmail(
      { data: { email: userEmail.value } },
      {
        onSuccess: () => {
          toast.add({ severity: 'success', summary: 'E-mail wysłany', life: 3000 })
          startCooldown()
        },
        onError: () => {
          toast.add({ severity: 'error', summary: 'Błąd', detail: 'Błąd podczas wysyłania e-maila', life: 3000 })
        }
      }
    )
  }

  // ---------- Change password ----------
  const { mutate: changePassword, isError: isChangeError } = usePostAuthChangePassword()
  const changePasswordError = ref<string | null>(null)

  const changeUserPassword = async (rawValues: unknown) => {
    changePasswordError.value = null
    const values = changePasswordSchema.parse(rawValues) as ChangePasswordDto

    changePassword({ data: values }, {
      onSuccess: () => {
        toast.add({ severity: 'success', summary: 'Password changed' })
      },
      onError: (error: unknown) => {
        const err = error as AxiosError<ProblemDetails>
        changePasswordError.value = err.response?.data?.title || 'Błąd zmiany hasła'
      }
    })
  }

  return {
    login,
    loginError,
    registerUser,
    isRegisterPending,
    isRegisterError,
    registerError,
    forgotPassword,
    isForgotPending,
    resetUserPassword,
    resetError,
    isResetError,
    confirmUserEmail,
    resendConfirmationEmail,
    isResendDisabled,
    userEmail,
    secondsLeft,
    changeUserPassword,
    changePasswordError,
    isChangeError
  }
}
