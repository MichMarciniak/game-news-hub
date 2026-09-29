<script setup lang="ts">
import { ref } from 'vue';
import PasswordWithHints from './PasswordWithHints.vue';
import { zodResolver } from '@openvue/forms/resolvers/zod';
import z from 'zod';
import { usePostChangePassword } from '@/api/generated';
import type { FormSubmitEvent } from '@openvue/forms';
import type { ChangePasswordDto, ProblemDetails } from '@/api/generated/models';
import { useToast } from 'openvue';
import type { AxiosError } from 'axios';
import { changePasswordResolver } from './schemas.ts';

const initialValues = ref({
  oldPassword: '',
  newPassword: ''
})

const toast = useToast()

const visible = ref(false)
const errorMessage = ref('')

const resolver = changePasswordResolver

const {mutate: changePassword, isError} = usePostChangePassword()

const onFormSubmit = async ({valid, values}: FormSubmitEvent) => {
  if (!valid) return

  const data = {
    oldPassword: values.oldPassword,
    newPassword: values.newPassword
  } as ChangePasswordDto

  changePassword({data}, {
    onSuccess: () => {
      toast.add({severity: 'success', summary: 'Password changed'})
      visible.value = false
    },
    onError: (error) => {
      const err = error as AxiosError<ProblemDetails>
      errorMessage.value = err.response?.data.title as string
    }
  })

}

</script>

<template>
  <div>
    <Drawer v-model:visible="visible" position="full" header="Change password" >
      <Form 
        v-slot="$form"
        :resolver="resolver"
        :initial-values="initialValues"
        @submit="onFormSubmit"
        class="mx-auto mt-[5rem] max-w-md flex flex-col gap-5"
      >
        <div>
          <FloatLabel variant="on">
            <Password toggle-mask :feedback="false" name="oldPassword" id="oldPassword" fluid type="password"/>
            <label for="oldPassword">Current password</label>
          </FloatLabel>
        </div>
        <div>
          <FloatLabel variant="on">
            <PasswordWithHints name="newPassword"/>
            <label for="newPassword">New password</label>
          </FloatLabel>
          <Message v-if="$form.newPassword?.invalid" severity="error" size="small" variant="simple">
            {{ $form.newPassword.error.message }}
          </Message>
        </div>
        <div class="flex flex-col">
          <Transition name="fade">
            <span v-if="isError" class="text-red-500">{{ errorMessage }}</span>
          </Transition>
        </div>
        <div class="flex justify-between">
          <Button label="Cancel" variant="outlined" @click="visible = false" />
          <Button type="submit" label="Change"/>
        </div>
      </Form>
    </Drawer>
    <Button @click="visible = true" label="Change password"></Button>
  </div>
</template>