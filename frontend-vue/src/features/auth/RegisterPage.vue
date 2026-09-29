<script lang="ts" setup>
import { ref } from 'vue';
import { Form, type FormSubmitEvent } from '@openvue/forms';
import { zodResolver } from '@openvue/forms/resolvers/zod';
import InputText from 'openvue/inputtext';
import Password from 'openvue/password';
import Button from 'openvue/button';
import Message from 'openvue/message';
import z from 'zod';
import { usePostRegister } from '@/api/generated';
import AuthPageWrapper from '@/features/auth/AuthPageWrapper.vue';
import { OiSpinner } from '@openvue/openicons-vue';
import { useRouter } from 'vue-router';
import type { ProblemDetails, RegisterDto } from '@/api/generated/models';
import type { AxiosError } from 'axios';
import PasswordWithHints from './PasswordWithHints.vue';
import { registerResolver } from './schemas.ts';

const router = useRouter()

const initialValues = ref({
  username: 'asd',
  email: 'asd@asd.we',
  password: 'asdasdasdasd'
});

const resolver = registerResolver

const {mutate: register, isPending, isError} = usePostRegister()
const errorMessage = ref('asd')

const onFormSubmit = async ({ valid, values }: FormSubmitEvent) => {
  if (!valid) return;

  register(
    {data: values as RegisterDto},
  {
    onSuccess: () => {
      router.push({
        path: '/confirm-email-sent',
        query: {email: btoa(values.email)}
      })
    },
    onError: (error) => {
      const resError = error as AxiosError<ProblemDetails>
      errorMessage.value = resError.response?.data.title as string
    }
  })
  
};
</script>

<template>
  <AuthPageWrapper>
    <template #title>Register</template>

    <template #body>
      <Form 
        :initialValues="initialValues" 
        :resolver="resolver" 
        @submit="onFormSubmit" 
        v-slot="$form" 
        class="flex flex-col gap-4"
      >
        <!-- Pole Username -->
        <div class="flex flex-col gap-1">
          <FloatLabel variant="on">
            <InputText name="username" id="username" type="text" placeholder="yourusername" fluid />
            <label for="username">Username</label>
          </FloatLabel>
          <Message v-if="$form.username?.invalid" severity="error" size="small" variant="simple">
            {{ $form.username.error?.message }}
          </Message>
        </div>

        <!-- Pole Email -->
        <div class="flex flex-col gap-1">
          <FloatLabel variant="on">
            <InputText name="email" id="email" type="text" placeholder="your@email.com" fluid />
            <label for="email">Email</label>
          </FloatLabel>
          <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
            {{ $form.email.error?.message }}
          </Message>
        </div>

        <!-- Pole Hasło z szablonem wymagań -->
        <div class="flex flex-col gap-1">
          <FloatLabel variant="on">
            <PasswordWithHints name="password"/>
            <label for="password">Password</label>
          </FloatLabel>
          <Message v-if="$form.password?.invalid" severity="error" size="small" variant="simple">
            {{ $form.password.error?.message }}
          </Message>
        </div>

        <div class="flex flex-col">
          <Transition name="fade">
            <span v-if="isError" class="text-red-500">{{ errorMessage }}</span>
          </Transition>
        </div>

        <Button type="submit" label="Register" severity="primary" class="mt-2" fluid :disabled="isPending" >
          <span v-if="isPending"><OiSpinner size="1.5rem" class="animate-spin"/></span>
        </Button>
      </Form>

      <div class="text-center text-sm mt-4">
        Already have an account?
        <RouterLink to="/login" class="font-semibold text-primary hover:underline ml-1">
          Login
        </RouterLink>
      </div>

    </template>
  </AuthPageWrapper>
</template>

<style lang="css" scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>