<script lang="ts" setup>
import { usePostConfirmEmail } from '@/api/generated';
import type { EmailConfirmDto } from '@/api/generated/models';
import AuthPageWrapper from '@/features/auth/AuthPageWrapper.vue';
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';


const route = useRoute()

const userId = computed(() => route.query.userId as string | number)
const token = computed(() => route.query.token as string)

const body = ref({
  title: '',
  detail: '',
  redirect: ''
})
const {mutate: confirm} = usePostConfirmEmail()

function onView() {
  const data = {
    userId: userId.value,
    token: token.value
  }
  confirm(
    {data: data as EmailConfirmDto},
    {
      onSuccess: () => {
        body.value.title = 'Email confirmed'
        body.value.detail = 'You can login now.'
        body.value.redirect = '/login'
      },
      onError: () => {
        body.value.title = "Something went wrong"
        body.value.detail = 'Go to resend email.'
        body.value.redirect = '/resend-confirmation'
      }
    })
}

onMounted(() => {
  onView()
})


</script>

<template>
  <AuthPageWrapper >
    <template #title>{{ body.title }}</template>

    <template #body>
      <div class="flex flex-col gap-1">
        <p>{{ body.detail }}</p>
        <Button as-child v-slot="slotProps" variant="link" fluid>
          <RouterLink :to="body.redirect" :class="slotProps.class">Go</RouterLink>
        </Button>

      </div>

    </template>
  </AuthPageWrapper>
  
</template>