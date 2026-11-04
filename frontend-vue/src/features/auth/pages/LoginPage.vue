<script setup lang="ts">
import { ref } from 'vue'
import { useToast } from 'openvue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { useAuthService } from '@/features/auth/services/auth.service'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import LoginForm from '../components/LoginForm.vue'

const route = useRoute()
const router = useRouter()
const toast = useToast()

const { login, loginError } = useAuthService()
const errorMessage = ref('')

const onSubmit = async (values: unknown) => {
  errorMessage.value = ''
  const result = await login(values)
  if (!result?.success) {
    errorMessage.value = result.error ?? 'Login error'
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
