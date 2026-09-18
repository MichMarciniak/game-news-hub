<script lang="ts" setup>
import { useAuthStore } from '@/stores/auth';
import { OiUser } from '@openvue/openicons-vue';
import { computed } from 'vue';

const {isAuthenticated} = useAuthStore();


const url = computed(() => isAuthenticated ? '/me' : '/login')

</script>


<template>
  <Button label="Account" severity="primary p-0" rounded variant="link">
    <RouterLink :to="url">
      <OiUser size="32" />
    </RouterLink>
  </Button>
</template>