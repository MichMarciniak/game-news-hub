<script setup lang="ts">
import { ref } from 'vue';
import { useGoogleService } from '../services/useGoogleService';
import Button from 'openvue/button';

const { isGoogleConnected, linkGoogleAccount, isLinking, linkError, handleGoogleLink } = useGoogleService();
const showUnlinkConfirm = ref(false);

function unlinkGoogle() {
  showUnlinkConfirm.value = true;
}

async function confirmUnlink() {
  showUnlinkConfirm.value = false;
  // TODO: Implement unlink API call
  console.log('Unlink Google account')
}
</script>

<template>
  <div class="flex flex-col gap-2">
    <div class="flex items-center justify-between gap-4 p-2 rounded-border border border-surface">
      <div class="flex items-center gap-3">
        <img src="@/assets/google/google_icon.svg" alt="Google" class="w-6 h-6" />
        <span class="font-medium">Google connected</span>
      </div>
      <Button label="Unlink" variant="outline" @click="unlinkGoogle" class="whitespace-nowrap" />
    </div>

    <div v-if="showUnlinkConfirm" class="flex gap-2">
      <p class="text-sm text-muted-color">Are you sure you want to unlink your Google account?</p>
      <Button label="Yes, unlink" variant="destructive" @click="confirmUnlink" />
      <Button label="Cancel" variant="outline" @click="showUnlinkConfirm = false" />
    </div>

    <div v-if="linkError" class="text-sm text-red-500">{{ linkError }}</div>
  </div>
</template>