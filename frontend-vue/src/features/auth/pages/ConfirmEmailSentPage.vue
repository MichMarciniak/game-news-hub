<script setup lang="ts">
import { useAuthService } from '@/features/auth/services/auth.service'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'

const { resendConfirmationEmail, isResendDisabled, userEmail, secondsLeft } = useAuthService()
</script>

<template>
  <AuthPageWrapper>
    <template #title>Confirm email</template>

    <template #body>
      <p>
        An email has been sent to
        <span class="font-semibold!">{{ userEmail }}</span>
      </p>

      <Button
        @click="resendConfirmationEmail"
        :disabled="isResendDisabled"
        class="px-4 py-2 rounded mt-6 disabled:opacity-50 w-full"
      >
        <span v-if="!isResendDisabled">Resend email</span>
        <span v-else>{{ secondsLeft }}s</span>
      </Button>
    </template>
  </AuthPageWrapper>
</template>
