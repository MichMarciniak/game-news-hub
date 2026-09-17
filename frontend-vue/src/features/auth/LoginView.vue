<script lang="ts" setup>
import { ref } from 'vue';
import { Form, type FormSubmitEvent } from '@openvue/forms';
import { zodResolver } from '@openvue/forms/resolvers/zod';
import InputText from 'openvue/inputtext';
import Password from 'openvue/password';
import Button from 'openvue/button';
import Message from 'openvue/message';
import { useAuthStore } from '@/stores/auth';
import z from 'zod';
import { useToast } from 'openvue';

const authStore = useAuthStore();

// const toast = useToast()

const initialValues = ref({
  email: '',
  password: ''
});

// W formularzu logowania zazwyczaj wystarczy email i password
const resolver = ref(zodResolver(
  z.object({
    email: z.string().min(1, { message: 'Email is required.' }).email({ message: 'Invalid email.' }),
    password: z.string().min(6, { message: 'Minimum 8 characters' })
  })
));

const onFormSubmit = async ({ valid, values }: FormSubmitEvent) => {
  if (!valid) return;

  try {
    await authStore.login(values);
    // toast.add({ severity: 'success', summary: 'Success', detail: 'Logged in', life: 3000 });
  } catch (error) {
    // toast.add({ severity: 'error', summary: 'Error', detail: 'Invalid username or password', life: 3000 });
  }
};
</script>

<template>
  <!-- <div class=""> -->
    <div class="grow max-w-md mx-auto my-auto p-6 border ">
      <h2 class="text-2xl font-bold mb-6">Login</h2>
  
      <Form 
        :initialValues="initialValues" 
        :resolver="resolver" 
        @submit="onFormSubmit" 
        v-slot="$form" 
        class="flex flex-col gap-4"
      >
        <!-- Pole Email -->
        <div class="flex flex-col gap-1">
          <label for="email">Email</label>
          <InputText name="email" type="text" placeholder="your@email.com" fluid />
          <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
            {{ $form.email.error?.message }}
          </Message>
        </div>
  
        <!-- Pole Hasło z szablonym wymagań -->
        <div class="flex flex-col gap-1">
          <label for="password">Password</label>
          <Password
            name="password"
            toggleMask
            fluid
            variant="filled"
            :feedback="true"
          >
            <!-- Szablon nagłówka podpowiedzi -->
            <template #header>
              <div class="font-semibold text-xs mb-2">Strength</div>
            </template>
  
            <!-- Szablon stopki wyświetlający warunki -->
            <template #footer>
              <div class="mt-2 pt-2 border-t">
                <p class="text-xs font-semibold mb-1">Password requirements:</p>
                <ul class="text-xs list-disc pl-4 space-y-1">
                  <li>Minimum 8 characters</li>
                  <li>At least one uppercase</li>
                  <li>At least one number</li>
                </ul>
              </div>
            </template>
          </Password>
          <Message v-if="$form.password?.invalid" severity="error" size="small" variant="simple">
            {{ $form.password.error?.message }}
          </Message>
        </div>
  
        <div class="flex items-center justify-between mb-1">
          <RouterLink to="/forgot-password" class="text-xs text-primary hover:underline">
            Forgot password?
          </RouterLink>
        </div>
        <Button type="submit" label="Login" severity="primary" class="mt-2" fluid />
      </Form>

      <div class="text-center text-sm mt-4">
        Don't have an  account yet?
        <RouterLink to="/register" class="font-semibold text-primary hover:underline ml-1">
          Create one
        </RouterLink>
      </div>
      
    </div>

  <!-- </div> -->
</template>