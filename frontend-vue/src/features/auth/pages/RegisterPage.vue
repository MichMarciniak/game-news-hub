<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthService } from '@/features/auth/services/auth.service'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import RegisterForm from '../components/RegisterForm.vue'
import { RouterLink } from 'vue-router'

const router = useRouter()

const { registerUser, isRegisterPending, registerError } = useAuthService()
const errorMessage = ref('')

const onSubmit = async (values: unknown) => {
  errorMessage.value = ''
  await registerUser(values)
  if (registerError.value) {
    errorMessage.value = registerError.value
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
