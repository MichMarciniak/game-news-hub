<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuthService } from '@/features/auth/services/auth.service'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'

const { confirmUserEmail } = useAuthService()

onMounted(() => {
  confirmUserEmail()
})
</script>

<template>
  <AuthPageWrapper>
    <template #title>Confirm email</template>

    <template #body>
      <div class="flex flex-col gap-4 text-center">
        <p>Checking your email...</p>
        <p class="text-sm text-muted">This may take a moment.</p>
      </div>
    </template>
  </AuthPageWrapper>
</template>
