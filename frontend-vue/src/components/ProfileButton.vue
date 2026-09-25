<script lang="ts" setup>
import { useAuthStore } from '@/stores/auth';
import { OiUser } from '@openvue/openicons-vue';
import { storeToRefs } from 'pinia';
import { computed } from 'vue';

const authStore = useAuthStore()
const {isAuthenticated} = storeToRefs(authStore)


const url = computed(() => isAuthenticated.value ? '/me' : '/login')

</script>


<template>
  <Button label="Account" v-slot="slotProps" as-child severity="primary p-0" rounded variant="link">
    <RouterLink :to="url" aria-label="Account" :class="slotProps.class">
      <OiUser size="32" />
    </RouterLink>
  </Button>
</template>