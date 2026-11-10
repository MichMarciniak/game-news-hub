<script setup lang="ts">
import { ref } from 'vue'
import { useResetPasswordService } from '@/features/auth/services/useResetPasswordService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import Password from 'openvue/password'
import Button from 'openvue/button'
import FloatLabel from 'openvue/floatlabel'

const route = useRoute()
const router = useRouter()

const password = ref('')
const userId = route.query.userId as string | number
const token = route.query.token as string

const { resetUserPassword, resetError } = useResetPasswordService()

const handleSubmit = async () => {
  const result = await resetUserPassword({ userId, token, password: password.value })
  if (result?.success) {
    await router.push({ path: '/login' })
  }
}
</script>

<template>
  <AuthPageWrapper>
    <template #title>New password</template>

    <template #body>
      <p>Enter your new password.</p>

      <form @submit.prevent="handleSubmit" class="flex flex-col gap-4">
        <FloatLabel variant="on">
          <Password
            name="password"
            inputId="password"
            toggleMask
            fluid
            variant="filled"
            :feedback="false"
          />
          <label for="password">Password</label>
        </FloatLabel>

        <div v-if="resetError" class="text-red-500 text-sm">
          {{ resetError }}
        </div>

        <Button type="submit" label="Change password" severity="primary" class="mt-2" fluid />
      </form>

      <div class="text-center text-sm mt-4">
        <RouterLink to="/login" class="font-semibold text-primary hover:underline ml-1">
          Return to login
        </RouterLink>
      </div>
    </template>
  </AuthPageWrapper>
</template>
