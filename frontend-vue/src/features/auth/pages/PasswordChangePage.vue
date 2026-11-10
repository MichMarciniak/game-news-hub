<script setup lang="ts">
import { ref } from 'vue'
import { useChangePasswordService } from '@/features/auth/services/useChangePasswordService'
import Password from 'openvue/password'
import Button from 'openvue/button'
import FloatLabel from 'openvue/floatlabel'

const visible = ref(false)
const oldPassword = ref('')
const newPassword = ref('')

const { changeUserPassword, changePasswordError } = useChangePasswordService()

const handleSubmit = () => {
  changeUserPassword({ oldPassword: oldPassword.value, newPassword: newPassword.value })
}
</script>

<template>
  <div>
    <Drawer v-model:visible="visible" position="full" header="Change password">
      <form
        @submit.prevent="handleSubmit"
        class="mx-auto mt-[5rem] max-w-md flex flex-col gap-5"
      >
        <FloatLabel variant="on">
          <Password
            name="oldPassword"
            inputId="oldPassword"
            toggleMask
            fluid
            variant="filled"
            :feedback="false"
          />
          <label for="oldPassword">Current password</label>
        </FloatLabel>

        <FloatLabel variant="on">
          <Password
            name="newPassword"
            inputId="newPassword"
            toggleMask
            fluid
            variant="filled"
            :feedback="false"
          />
          <label for="newPassword">New password</label>
        </FloatLabel>

        <div v-if="changePasswordError" class="text-red-500 text-sm">
          {{ changePasswordError }}
        </div>
        <div class="flex justify-between">
          <Button type="button" label="Cancel" variant="outlined" @click="visible = false" />
          <Button type="submit" label="Change" severity="primary" />
        </div>
      </form>
    </Drawer>
    <Button label="Change password" severity="primary" @click="visible = true" />
  </div>
</template>
