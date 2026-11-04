<script setup lang="ts">
import { ref } from 'vue'
import { useAuthService } from '@/features/auth/services/auth.service'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import { RouterLink } from 'vue-router'
import Password from 'openvue/password'
import Button from 'openvue/button'
import FloatLabel from 'openvue/floatlabel'

const password = ref('')

const { resetUserPassword, resetError } = useAuthService()
</script>

<template>
  <AuthPageWrapper>
    <template #title>New password</template>

    <template #body>
      <p>Enter your new password.</p>

      <form @submit.prevent="resetUserPassword({ password })" class="flex flex-col gap-4">
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
