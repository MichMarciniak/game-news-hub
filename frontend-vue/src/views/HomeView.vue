<script setup lang="ts">
import { useGetTest } from '@/api/generated';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';

const {logout, logoutAll} = useAuthStore()
const authStore = useAuthStore()

const {user, isAuthenticated} = storeToRefs(authStore)

const { refetch} = useGetTest({
  query:{
    enabled: false
  }
})

function test() {
  refetch()
}

</script>

<template>
<div class="flex flex-col mx-auto gap-2">
  <Button label="logout" @click="logout" ></Button>
  <Button label="logoutAll" @click="logoutAll" ></Button>
  <Button label="test auth" @click="test"></Button>
  <span>{{ user?.username }}</span>
  <span>{{ isAuthenticated }}</span>

</div>
</template>
