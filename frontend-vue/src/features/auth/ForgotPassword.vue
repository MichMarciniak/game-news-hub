<script setup lang="ts">
import { usePostRequestPasswordReset } from '@/api/generated';
import type { EmailDto } from '@/api/generated/models';
import AuthView from '@/views/AuthView.vue';
import { type FormSubmitEvent } from '@openvue/forms';
import { zodResolver } from '@openvue/forms/resolvers/zod';
import { useToast } from 'openvue';
import { ref } from 'vue';
import z from 'zod';

const initialValues = ref({
  email: ''
})

const toast = useToast()

const resolver = zodResolver(
  z.object({
    email: z.string().min(1, { message: 'Email is required.' }).email({ message: 'Invalid email.' }),
  })
);

const {mutate: sendReset, isPending} = usePostRequestPasswordReset()

function submit({valid, values}: FormSubmitEvent) {
  if (!valid) return
  sendReset(
    {data: values as EmailDto},
    {
      onSuccess: () => {
        toast.add({severity:'success', summary:'Email send', detail:'Email has been sent if it exists', life: 3000})

      }
    }
  )
  
}


</script>


<template>
  <AuthView>
    <template #title>Reset Password</template>
    <template #body>
      <p>hello there</p>
      <Form 
        :initial-values="initialValues"
        :resolver="resolver"
        @submit="submit"
        v-slot="$form"
        class="flex flex-col gap-4"
      >
        <div>
          <InputText fluid
            name="email"
            type="text" 
            placeholder="your@email.com" 
            class="mt-5"/>
          <Message v-if="$form.email?.invalid" severity="error" size="small" variant="simple">
              {{ $form.email.error?.message }}
          </Message>
        </div>

        <Button type="submit" fluid label="Send" :disabled="isPending"/>

      </Form>
    </template>
  </AuthView>
</template>