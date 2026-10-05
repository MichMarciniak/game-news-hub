<script setup lang="ts">
import { useGetGenres, useGetPlatformsGroups } from '@/api/generated';
import PasswordChange from '@/features/auth/PasswordChange.vue';
import ExpandableFlex from '@/features/profile/ExpandableFlex.vue';
import GenreFollowCard from '@/features/profile/GenreFollowCard.vue';
import PlatformFollowCard from '@/features/profile/PlatformFollowCard.vue';
import Settings from '@/features/profile/Settings.vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';
import { useRouter } from 'vue-router';

const router = useRouter()

const {logout, logoutAll} = useAuthStore()
const authStore = useAuthStore();
const {user, isAuthenticated } = storeToRefs(authStore)

const {data: platforms} = useGetPlatformsGroups()
const {data: genres} = useGetGenres()

function onLogout(fn: Function) {
  fn()
  router.push({name: 'home'})
}

</script>


<template>
  <div class="max-w-7xl w-full mt-5 mx-auto grid grid-cols-1 sm:grid-cols-2">
    <div class="flex flex-col gap-2">
      <h2 class="text-2xl font-semibold">{{ user?.username }}</h2>
      <Settings />
      <PasswordChange />
      <div class="flex gap-2">
        <Button label="logout" @click="onLogout(logout)" ></Button>
        <Button label="logoutAll" @click="onLogout(logoutAll)" ></Button>
      </div>

    </div>

    <div>
      <ExpandableFlex title="Platforms">
        <PlatformFollowCard v-for="p in platforms" :key="p.id" 
          :id="p.id"
          :name="p.name"
        />
      </ExpandableFlex>
      <ExpandableFlex title="Genres">
        <GenreFollowCard v-for="g in genres" :key="g.id"
          :id="g.id"
          :name="g.name"
        />
      </ExpandableFlex>

    </div>
  </div>
 
</template>