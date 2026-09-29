<script setup lang="ts">
import { usePostResendEmail } from '@/api/generated';
import AuthPageWrapper from '@/features/auth/AuthPageWrapper.vue';
import { useToast } from 'openvue';
import { ref } from 'vue';
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute()
const userEmail = computed(() => atob(route.query.email as string))
const toast = useToast()

const {mutate: resendConfirmation} = usePostResendEmail();

const COOLDOWN_SECONDS = 60
const secondsLeft = ref(0)
let intervalId: number | undefined;

const isDisabled = computed(() => secondsLeft.value > 0)

function startCooldown() {
  secondsLeft.value = COOLDOWN_SECONDS
  intervalId = setInterval(() => {
    secondsLeft.value -= 1
    if (secondsLeft.value <= 0) {
      clearInterval(intervalId)
      intervalId = undefined
    }
  }, 1000)
}

function resend() {
  resendConfirmation(
    {data: {email: userEmail.value}},
    {
      onSuccess: () => {
        toast.add({ severity: 'success', summary: 'Email sent', life: 3000 })
        startCooldown()
      },
      onError: () => {
        toast.add({ severity: 'error', summary: 'Error', detail: 'There was an error when sending email', life: 3000 })
      }
    }
  );
}

</script>

<template>
  <AuthPageWrapper>
    <template #title>Confirm Your Email</template>

    <template #body>
      <p>An email has been sent to
         <span class="font-semibold! ">{{ userEmail }}</span>
      </p>
      <Button @click="resend" severity="secondary" fluid :disabled="isDisabled" class="mt-10">
        <span v-if="!isDisabled">Resend email</span>
        <span v-else>{{ secondsLeft }}</span>
      </Button>
      
    </template>
  </AuthPageWrapper>

</template>