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
import AuthView from '@/views/AuthView.vue';
import { OiSpinner } from '@openvue/openicons-vue';
import { useRouter } from 'vue-router';
import type { ProblemDetails, RegisterDto } from '@/api/generated/models';
import type { AxiosError } from 'axios';

const router = useRouter()

const initialValues = ref({
  username: 'asd',
  email: 'asd@asd.we',
  password: 'asdasdasdasd'
});

const resolver = ref(zodResolver(
  z.object({
    username: z.string().min(3, { message: 'Minimum 3 characters' }),
    email: z.string().min(1, { message: 'Email is required.' }).email({ message: 'Invalid email.' }),
    password: z.string().min(8, { message: 'Minimum 8 characters' })
  })
));

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
  <AuthView>
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
          <label for="username">Username</label>
          <InputText name="username" type="text" placeholder="yourusername" fluid />
          <Message v-if="$form.username?.invalid" severity="error" size="small" variant="simple">
            {{ $form.username.error?.message }}
          </Message>
        </div>

        <!-- Pole Email -->
        <div class="flex flex-col gap-1">
          <label for="email">Email</label>
          <InputText name="email" type="text" placeholder="your@email.com" fluid />
          <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
            {{ $form.email.error?.message }}
          </Message>
        </div>

        <!-- Pole Hasło z szablonem wymagań -->
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
  </AuthView>
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