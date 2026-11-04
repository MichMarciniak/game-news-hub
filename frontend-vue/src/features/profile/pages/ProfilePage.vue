<script setup lang="ts">
import { useGetGenres, useGetPlatformsGroups } from '@/api/generated';
import { useProfileService } from '@/features/profile/services/useProfileService';
import PasswordChange from '@/features/auth/pages/PasswordChangePage.vue';
import ExpandableFlex from '@/features/profile/components/ExpandableFlex.vue';
import GenreFollowCard from '@/features/genres/GenreFollowCard.vue';
import PlatformFollowCard from '@/features/platforms/PlatformFollowCard.vue';
import Settings from '@/features/profile/components/Settings.vue';
import { useRouter } from 'vue-router';
import SteamContainer from '@/features/profile/steam/SteamContainer.vue';

const router = useRouter()

const {
  user,
  isAuthenticated,
  settingsConf,
  updateSetting,
  logout,
  logoutAll
} = useProfileService()

const {data: platforms} = useGetPlatformsGroups()
const {data: genres} = useGetGenres()

function onLogout(fn: Function) {
  fn()
  router.push({name: 'home'})
}
</script>

<template>
  <div class="max-w-7xl w-full mt-5 mx-auto grid grid-cols-1 sm:grid-cols-2">
    <div class="flex flex-col gap-5">
      <h2 class="text-2xl font-semibold">{{ user?.username }}</h2>
      <Settings />
      <PasswordChange />
      <div class="flex gap-2">
        <Button label="Logout" @click="onLogout(logout)" ></Button>
        <Button label="Logout all" @click="onLogout(logoutAll)" ></Button>
      </div>
      <SteamContainer />

    </div>

    <div>
      <ExpandableFlex title="Platforms">
        <PlatformFollowCard v-for="p in (platforms ?? [])" :key="String(p.id)" 
          :id="String(p.id)"
          :name="String(p.name)"
        />
      </ExpandableFlex>
      <ExpandableFlex title="Genres">
        <GenreFollowCard v-for="g in (genres ?? [])" :key="String(g.id)"
          :id="String(g.id)"
          :name="String(g.name)"
        />
      </ExpandableFlex>

    </div>
  </div>
  
</template>
