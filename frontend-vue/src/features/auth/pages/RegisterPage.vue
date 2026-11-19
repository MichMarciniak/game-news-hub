<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useRegisterService } from '@/features/auth/services/useRegisterService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import RegisterForm from '../components/RegisterForm.vue'

const router = useRouter()

const { registerUser, isRegisterPending, registerError } = useRegisterService()
const errorMessage = ref('')

const handleGoogleLogin = () => {
  window.location.href = '/api/auth/google/login/redirect'
}

const onSubmit = async (values: unknown) => {
  errorMessage.value = ''
  const result = await registerUser(values)
  if (result?.success && result.email) {
    await router.push({
      path: '/confirm-email-sent',
      query: { email: btoa(result.email) }
    })
  } else if (result?.error) {
    errorMessage.value = result.error
  }
}
</script>

<template>
  <AuthPageWrapper>
    <template #title>Register</template>

    <template #body>
      <RegisterForm
        @submit="onSubmit"
        :loading="isRegisterPending"
      />

      <div v-if="errorMessage" class="text-red-500 text-sm mt-2">
        {{ errorMessage }}
      </div>

      <div class="relative mb-4">
        <div class="absolute inset-0 flex items-center">
          <div class="w-full border-t border-surface"></div>
        </div>
        <div class="relative flex justify-center text-sm">
          <span class="px-2 bg-white dark:bg-surface-900 text-muted-color">Or continue with</span>
        </div>
      </div>

<button
        @click="handleGoogleLogin"
        class="flex items-center justify-center gap-2 w-full px-4 py-2 rounded-border border-2 border-surface bg-white dark:bg-surface-800 hover:bg-surface-100 dark:hover:bg-surface-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        type="button"
      >
        <svg class="w-5 h-5" viewBox="0 0 24 24">
          <path
            fill="currentColor"
            d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
          />
          <path
            fill="currentColor"
            d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
          />
          <path
            fill="currentColor"
            d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
          />
          <path
            fill="currentColor"
            d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
          />
        </svg>
        <span class="font-medium">Continue with Google</span>
      </button>

      <div class="text-center text-sm mt-4">
        Already have an account?
        <RouterLink to="/login" class="font-semibold text-primary hover:underline ml-1">
          Log in
        </RouterLink>
      </div>
    </template>
  </AuthPageWrapper>
</template>
