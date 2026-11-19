<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { useLoginService } from '@/features/auth/services/useLoginService'
import { useAuthRedirect } from '@/composables/useAuthRedirect'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import LoginForm from '../components/LoginForm.vue'

const route = useRoute()
const router = useRouter()

const { login, loginError } = useLoginService()
const { saveRedirectPath, consumeRedirectPath } = useAuthRedirect()
const errorMessage = ref('')

onMounted(() => {
  const redirectPath = route.query.redirectPath as string | undefined
  saveRedirectPath(redirectPath)
})

const handleGoogleLogin = () => {
  const redirectPath = route.query.redirectPath as string | undefined
  saveRedirectPath(redirectPath)
  window.location.href = '/api/auth/google/login/redirect'
}

const onSubmit = async (values: unknown) => {
  errorMessage.value = ''
  const result = await login(values)
  if (!result?.success) {
    errorMessage.value = result.error ?? 'Login error'
  } else {
    const redirectTo = consumeRedirectPath()
    await router.push(redirectTo)
  }
}
</script>

<template>
  <AuthPageWrapper>
    <template #title>Login</template>

    <template #body>
      <LoginForm @submit="onSubmit" />

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
        <img src="@/assets/google/google_icon.svg" alt="Google" class="w-5 h-5" />
        <span class="font-medium">Continue with Google</span>
      </button>

      <div class="flex items-center justify-between mb-1">
        <RouterLink to="/forgot-password" class="text-xs text-primary hover:underline">
          Forgot password?
        </RouterLink>
      </div>

      <div class="text-center text-sm mt-4">
        Don't have an account?
        <RouterLink to="/register" class="font-semibold text-primary hover:underline ml-1">
          Create account
        </RouterLink>
      </div>
    </template>
  </AuthPageWrapper>
</template>
