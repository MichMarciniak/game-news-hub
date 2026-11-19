<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthRedirect } from '@/composables/useAuthRedirect'
import { useToast } from 'openvue'

const route = useRoute()
const router = useRouter()
const { consumeRedirectPath } = useAuthRedirect()
const toast = useToast()

onMounted(() => {
  const googleResult = route.query.google as string | undefined
  if (!googleResult) {
    router.push({ name: 'login' })
    return
  }

  const redirectPath = consumeRedirectPath()

  if (googleResult === 'connected') {
    router.push(redirectPath)
    return
  }

  if (googleResult === 'linked') {
    toast.add({ severity: 'success', summary: 'Google connected', detail: 'Your Google account has been linked', life: 3000 })
    router.push({ name: 'profile' })
    return
  }

  if (googleResult === 'account_exists') {
    const message = route.query.message as string | undefined
    toast.add({ severity: 'warn', summary: 'Account exists', detail: message ?? 'An account with this email already exists. Log in first to link your Google account.', life: 5000 })
    router.push({ name: 'login' })
    return
  }

  // Error cases
  const errorCode = route.query.code as string | undefined
  const errorMessage = route.query.message as string | undefined
  
  let detail = errorMessage ?? 'An error occurred during Google authentication'
  
  // User-friendly messages for specific error codes
  if (errorCode) {
    const friendlyMessages: Record<string, string> = {
      'Google.AuthFailed': 'Authentication with Google failed. Please try again.',
      'Google.InvalidState': 'Invalid login state. Please try again.',
      'Google.InvalidFlow': 'Invalid login flow. Please try again.',
      'Google.EmailMissing': 'Google did not provide an email address.',
      'Google.EmailNotVerified': 'Your Google email is not verified.',
      'Google.InvalidId': 'Google did not provide a valid ID.',
      'Google.SignInFailed': 'Failed to sign in with Google account.',
      'Google.SignupFailed': 'Failed to create account. Please try again.',
      'Google.LinkFailed': 'Failed to link Google account.',
      'Google.RoleAssignmentFailed': 'Account created but role assignment failed.',
      'Google.AccountAlreadyLinked': 'This Google account is already linked to another account.',
      'Google.EmailMismatch': 'Google account email does not match your account email.',
      'Google.ServerError': 'Google authentication service is temporarily unavailable.',
    }
    detail = friendlyMessages[errorCode] ?? detail
  }

  toast.add({ severity: 'error', summary: 'Google authentication failed', detail, life: 5000 })
  
  // If there was a saved redirect, still go there after showing error
  if (redirectPath !== '/') {
    router.push(redirectPath)
  } else {
    router.push({ name: 'login' })
  }
})
</script>

<template>
  <div class="min-h-screen flex items-center justify-center">
    <div class="animate-spin rounded-full h-8 w-8 border-2 border-primary border-t-transparent"></div>
    <span class="sr-only">Processing authentication...</span>
  </div>
</template>