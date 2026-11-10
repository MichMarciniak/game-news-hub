<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { useLoginService } from '@/features/auth/services/useLoginService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import LoginForm from '../components/LoginForm.vue'

const route = useRoute()
const router = useRouter()

const { login, loginError } = useLoginService()
const errorMessage = ref('')

const onSubmit = async (values: unknown) => {
  errorMessage.value = ''
  const redirectPath = route.query.redirectPath as string | undefined
  const result = await login(values, redirectPath)
  if (!result?.success) {
    errorMessage.value = result.error ?? 'Login error'
  } else if (result.redirectTo) {
    await router.push(result.redirectTo)
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
