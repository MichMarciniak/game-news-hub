<script setup lang="ts">
import { RouterView } from 'vue-router'
import Footer from './components/Footer.vue';
import { useAuthStore } from './stores/auth.ts';
import { storeToRefs } from 'pinia';
import { OiList } from '@openvue/openicons-vue';
import AdminSidebar from './components/AdminSidebar.vue';
import { useTemplateRef } from 'vue';

const authStore = useAuthStore()

const {isAdmin} = storeToRefs(authStore)

const adminPanelRef = useTemplateRef('adminPanelRef')

function handleOpen() {
  adminPanelRef.value.openPanel()
}

</script>

<template>
  <Header >
    <template v-if="isAdmin">
      <Button variant="text" class="p-0 m-0" @click="handleOpen">
        <OiList size="1.5rem"/>
      </Button>
    </template>
  </Header>

  <main class="flex-1 flex justify-center py-4 sm:px-4 not-sm:px-1">
    <Toast />
    <AdminSidebar v-if="isAdmin" ref="adminPanelRef"/>
    <RouterView />
  </main>
  <Footer />
</template>

