<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import SteamActionsMenu from './SteamActionsMenu.vue';
import SteamConnectButton from './SteamConnectButton.vue';
import { useGetSteamConnect } from '@/api/generated';

const authStore = useAuthStore()
const steamId = computed(() => authStore.user?.steamId ?? null)


const {data: steamUrl} = useGetSteamConnect()


function handleSteamConnect() {
  if (!steamUrl.value) return
  window.location.href = steamUrl.value
}

function handleImportWishlist() {

}

function handleImportOwnedGames() {

}

</script>


<template>
  <div class="rounded-border border-2 p-2 border-surface w-fit">
    <h2 class="font-semibold text-2xl"></h2>

    <SteamActionsMenu v-if="steamId">
  
    </SteamActionsMenu>
    <SteamConnectButton v-else @connect="handleSteamConnect">
  
    </SteamConnectButton>
  </div>
</template>