<script lang="ts" setup>
import { ref } from 'vue';
import { Form, type FormSubmitEvent } from '@openvue/forms';
import Password from 'openvue/password';
import Button from 'openvue/button';
import Message from 'openvue/message';
import { useAuthStore } from '@/stores/auth';
import { useToast } from 'openvue';
import { useRoute, useRouter } from 'vue-router';
import type { LoginDto } from '@/api/generated/models';
import AuthPageWrapper from '@/features/auth/AuthPageWrapper.vue';
import { loginResolver } from './schemas';

const authStore = useAuthStore();
const router = useRouter()
const route = useRoute()

const toast = useToast()

const initialValues = ref({
  email: 'test@test.asd',
  password: 'Siema1234'
});

const errorMessage = ref('')

const resolver = loginResolver

const onFormSubmit = async ({ valid, values }: FormSubmitEvent) => {
  if (!valid) return;
  console.log("CLICKED")
  console.log(values)
  try {
    await authStore.login(values as LoginDto);
    toast.add({ severity: 'success', summary: 'Success', detail: 'Logged in', life: 3000 });

    await router.push((route.query.redirectPath as string) || '/')
  } catch {
    errorMessage.value = 'Invalid username or password'
    // toast.add({ severity: 'error', summary: 'Error', detail: 'Invalid username or password', life: 3000 });
  }
};
</script>

<template>
  <AuthPageWrapper>
    <template #title>Login</template>
    <template #body>
      <Form 
        :initialValues="initialValues" 
        :resolver="resolver" 
        @submit="onFormSubmit" 
        v-slot="$form" 
        class="flex flex-col gap-4"
      >

        <div class="flex flex-col gap-1">
          <FloatLabel variant="on">
            <InputText name="email" id="email" type="text" placeholder="your@email.com" fluid />
            <label for="email">Email</label>

          </FloatLabel>
          <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
            {{ $form.email.error?.message }}
          </Message>
        </div>
  

        <div class="flex flex-col gap-1">
          <FloatLabel variant="on" >
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
          <Message v-if="$form.password?.invalid" severity="error" size="small" variant="simple">
            {{ $form.password.error?.message }}
          </Message>
        </div>
  
        <div class="flex items-center justify-between mb-1">
          <RouterLink to="/forgot-password" class="text-xs text-primary hover:underline">
            Forgot password?
          </RouterLink>
        </div>
        <div class="flex flex-col">
          <Transition name="fade">
            <span v-if="errorMessage" class="text-red-500 text-sm">
              {{ errorMessage }}
            </span>
          </Transition>
        </div>
        <Button type="submit" label="Login" severity="primary" class="mt-2" fluid />
      </Form>

      <div class="text-center text-sm mt-4">
        Don't have an  account yet?
        <RouterLink to="/register" class="font-semibold text-primary hover:underline ml-1">
          Create one
        </RouterLink>
      </div>
      
    </template>
  </AuthPageWrapper>
</template>