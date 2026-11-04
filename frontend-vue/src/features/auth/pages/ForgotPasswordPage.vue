<template>
  <form @submit.prevent="forgotPassword(email)" class="flex flex-col gap-4">
    <FloatLabel variant="on">
      <InputText
        name="email"
        id="email"
        v-model="email"
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
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useForgotPasswordService } from '@/features/auth/services/useForgotPasswordService'
import AuthPageWrapper from '@/features/auth/components/AuthPageWrapper.vue'
import { RouterLink } from 'vue-router'
import InputText from 'openvue/inputtext'
import Button from 'openvue/button'
import FloatLabel from 'openvue/floatlabel'

const email = ref('')
const { forgotPassword, isForgotPending } = useForgotPasswordService()
</script>
