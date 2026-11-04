<script setup lang="ts">
import { useResendEmailService } from '@/features/auth/services/useResendEmailService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import { useRoute, RouterLink } from 'vue-router'
import Button from 'openvue/button'

const route = useRoute()
const userEmail = route.query.email as string | undefined
const decodedEmail = userEmail ? atob(userEmail) : ''

const { resendConfirmationEmail, isResendDisabled, secondsLeft } = useResendEmailService()
</script>

<template>
  <AuthPageWrapper>
    <template #title>Confirm email</template>

    <template #body>
      <p>
        An email has been sent to
        <span class="font-semibold!">{{ decodedEmail }}</span>
      </p>

      <Button
        @click="() => resendConfirmationEmail(decodedEmail)"
        :disabled="isResendDisabled"
        class="px-4 py-2 rounded mt-6 disabled:opacity-50 w-full"
      >
        <span v-if="!isResendDisabled">Resend email</span>
        <span v-else>{{ secondsLeft }}s</span>
      </Button>
    </template>
  </AuthPageWrapper>
</template>
