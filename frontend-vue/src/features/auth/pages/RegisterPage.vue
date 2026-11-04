<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useRegisterService } from '@/features/auth/services/useRegisterService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import RegisterForm from '../components/RegisterForm.vue'

const router = useRouter()

const { registerUser, isRegisterPending, registerError } = useRegisterService()
const errorMessage = ref('')

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

      <div class="text-center text-sm mt-4">
        Already have an account?
        <RouterLink to="/login" class="font-semibold text-primary hover:underline ml-1">
          Log in
        </RouterLink>
      </div>
    </template>
  </AuthPageWrapper>
</template>
