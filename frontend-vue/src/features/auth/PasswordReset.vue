<script setup lang="ts">
import { usePostResetPassword } from '@/api/generated';
import type { ProblemDetails, ResetPasswordDto } from '@/api/generated/models';
import AuthView from '@/views/AuthView.vue';
import type { FormSubmitEvent } from '@openvue/forms';
import { zodResolver } from '@openvue/forms/resolvers/zod';
import { AxiosError } from 'axios';
import { useToast } from 'openvue';
import { computed, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import z from 'zod';

const initialValues = ref({
  password: ''
})

const route = useRoute()
const router = useRouter()
const toast = useToast()

const userId = computed(() => route.query.userId as string | number)
const token = computed(() => route.query.token as string)

const resolver = zodResolver(
  z.object({
    password: z.string().min(8, { message: 'Minimum 8 characters' }),
  })
)
const errorMessage = ref('')

const {mutate: resetPassword, isError} = usePostResetPassword()

const onFormSubmit = async ({ valid, values }: FormSubmitEvent) => {
  if (!valid) return
  const data = {
    userId: userId.value,
    token: token.value,
    password: values.password as string
  }
  console.log(data as ResetPasswordDto)
  console.log(data.token)

  resetPassword(
    {data: data as ResetPasswordDto},
  {
    onSuccess: () => {
      toast.add({severity:'success',summary:'Password changed', detail:'Login to your account with your new password' ,life: 3000})
      router.push({path: '/login'})
    },
    onError: (error) => {
      const err = error as AxiosError<ProblemDetails>
      errorMessage.value = err.response?.data.title as string
    }
  })


}

</script>


<template>
  <AuthView>
    <template #title>Reset Password</template>
    <template #body>
      <p>Enter your new password</p>
      <Form
        v-slot="$form"
        :resolver="resolver"
        @submit="onFormSubmit"
        :initialValues="initialValues"
      >
        <div class="flex flex-col gap-1 mt-3">
          <label for="password" class="text-sm">Password</label>
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
        <Button type="submit" label="Confirm" fluid class="mt-2"/>

      </Form>
    </template>
  </AuthView>
</template>