<script setup lang="ts">
import { useGoogleService } from '../services/useGoogleService';

const { handleGoogleLogin } = useGoogleService();

defineProps<{
  onConnect?: () => void;
}>();

const emit = defineEmits<{
  connect: [];
}>();

function handleConnect() {
  emit('connect');
  handleGoogleLogin();
}
</script>

<template>
  <div class="flex flex-col gap-2">
    <p class="text-sm text-muted-color">Connect your Google account for easier sign-in</p>
    <button
      @click="handleConnect"
      class="flex items-center justify-center gap-2 px-4 py-2 rounded-border border-2 border-surface bg-white dark:bg-surface-800 hover:bg-surface-100 dark:hover:bg-surface-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors w-full"
      type="button"
    >
      <img src="@/assets/google/google_icon.svg" alt="Google" class="w-5 h-5" />
      <span class="font-medium">Connect Google</span>
    </button>
  </div>
</template>