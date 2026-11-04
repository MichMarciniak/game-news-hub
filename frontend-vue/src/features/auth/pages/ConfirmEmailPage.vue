<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useConfirmEmailService } from '@/features/auth/services/useConfirmEmailService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import { useRoute, RouterLink } from 'vue-router'
import Button from 'openvue/button'

const route = useRoute()
const userId = route.query.userId as string | number
const token = route.query.token as string

const { confirmUserEmail } = useConfirmEmailService()
const status = ref<'checking' | 'success' | 'error'>('checking')

onMounted(async () => {
  try {
    confirmUserEmail({ userId, token })
    status.value = 'success'
  } catch {
    status.value = 'error'
  }
})
</script>

<template>
  <AuthPageWrapper>
    <template #title>Confirm email</template>

    <template #body>
      <div v-if="status === 'checking'" class="flex flex-col gap-4 text-center">
        <p>Checking your email...</p>
        <p class="text-sm text-muted">This may take a moment.</p>
      </div>

      <div v-else-if="status === 'success'" class="flex flex-col gap-4 text-center">
        <p class="text-success">Email confirmed!</p>
        <p class="text-sm text-muted">You can now log in.</p>
        <Button label="Go to login" severity="primary" as-child @click="() => {}">
          <RouterLink to="/login" class="cursor-pointer">Go to login</RouterLink>
        </Button>
      </div>

      <div v-else class="flex flex-col gap-4 text-center">
        <p class="text-error">Confirmation failed.</p>
        <Button label="Go to login" severity="contrast" as-child @click="() => {}">
          <RouterLink to="/login" class="cursor-pointer">Go to login</RouterLink>
        </Button>
      </div>
    </template>
  </AuthPageWrapper>
</template>
