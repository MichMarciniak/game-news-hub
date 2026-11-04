<script setup lang="ts">
import { Form, type FormSubmitEvent } from '@openvue/forms'
import InputText from 'openvue/inputtext'
import Password from 'openvue/password'
import Message from 'openvue/message'
import Button from 'openvue/button'
import { loginResolver } from '@/features/auth/schemas'

const props = withDefaults(defineProps<{
  initialValues?: Record<string, unknown>
  loading?: boolean
}>(), {
  initialValues: () => ({ email: '', password: '' }),
  loading: false
})

const emit = defineEmits<{
  (e: 'submit', values: unknown): void
}>()

const resolver = loginResolver

const onSubmit = ({ valid, values }: FormSubmitEvent) => {
  if (!valid) return
  emit('submit', values)
}
</script>

<template>
  <Form
    :initial-values="initialValues"
    :resolver="resolver"
    @submit="onSubmit"
    v-slot="$form"
    class="flex flex-col gap-4"
  >
    <div class="flex flex-col gap-1">
      <FloatLabel variant="on">
        <InputText name="email" id="email" type="text" placeholder="twoj@email.com" fluid />
        <label for="email">Email</label>
      </FloatLabel>
      <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
        {{ $form.email.error?.message }}
      </Message>
    </div>

    <div class="flex flex-col gap-1">
      <FloatLabel variant="on">
        <Password name="password" inputId="password" toggleMask fluid variant="filled" :feedback="false" />
        <label for="password">Password</label>
      </FloatLabel>
      <Message v-if="$form.password?.invalid" severity="error" size="small" variant="simple">
        {{ $form.password.error?.message }}
      </Message>
    </div>

    <Button type="submit" label="Log in" severity="primary" class="mt-2" fluid :disabled="loading" />
  </Form>
</template>
