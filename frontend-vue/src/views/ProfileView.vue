<script setup lang="ts">
import { useGetGenreGenres, useGetPlatformGroups } from '@/api/generated';
import PasswordChange from '@/features/auth/PasswordChange.vue';
import ExpandableFlex from '@/features/profile/ExpandableFlex.vue';
import GenreFollowCard from '@/features/profile/GenreFollowCard.vue';
import PlatformFollowCard from '@/features/profile/PlatformFollowCard.vue';
import Settings from '@/features/profile/Settings.vue';
import { useAuthStore } from '@/stores/auth';
import { storeToRefs } from 'pinia';


const authStore = useAuthStore();
const {user, isAuthenticated } = storeToRefs(authStore)

const {data: platforms} = useGetPlatformGroups()
const {data: genres} = useGetGenreGenres()

</script>


<template>
  <div class="max-w-7xl w-full mt-5 mx-auto">
    <h2 class="text-2xl font-semibold">{{ user?.username }}</h2>
    <Settings />
    <PasswordChange />
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
 
</template>