<script setup lang="ts">
import { useAuthService } from '@/features/auth/services/auth.service'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import { RouterLink } from 'vue-router'
import InputText from 'openvue/inputtext'
import Button from 'openvue/button'
import FloatLabel from 'openvue/floatlabel'

const { forgotPassword, isForgotPending } = useAuthService()
</script>

<template>
  <AuthPageWrapper>
    <template #title>Reset password</template>

    <template #body>
      <p>Enter your email to receive a reset link.</p>

      <form @submit.prevent="forgotPassword" class="flex flex-col gap-4">
        <FloatLabel variant="on">
          <InputText
            name="email"
            id="email"
            type="email"
            placeholder="your@email.com"
            required
            fluid
          />
          <label for="email">Email</label>
        </FloatLabel>

        <Button
          type="submit"
          label="Send link"
          severity="primary"
          class="mt-2"
          fluid
          :disabled="isForgotPending"
        />
      </form>

      <div class="text-center text-sm mt-4">
        <RouterLink to="/login" class="font-semibold text-primary hover:underline ml-1">
          Return to login
        </RouterLink>
      </div>
    </template>
  </AuthPageWrapper>
</template>
