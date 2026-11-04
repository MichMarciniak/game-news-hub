<script setup lang="ts">
import { ref } from 'vue'
import { useGetGenres, useGetPlatformsGroups } from '@/api/generated'
import { useProfileService } from '@/features/profile/services/useProfileService'
import { useWishlistService } from '@/features/profile/services/useWishlistService'
import { useUserFollowedService } from '@/features/profile/services/useUserFollowedService'
import PasswordChange from '@/features/auth/pages/PasswordChangePage.vue'
import GenreFollowCard from '@/features/genres/GenreFollowCard.vue'
import PlatformFollowCard from '@/features/platforms/PlatformFollowCard.vue'
import Settings from '@/features/profile/components/Settings.vue'
import { useRouter } from 'vue-router'
import SteamContainer from '@/features/profile/steam/SteamContainer.vue'
import GameGrid from '@/features/games/components/GameGrid.vue'
import GameCard from '@/features/games/components/GameCard.vue'
import GameCardWishlist from '@/features/games/components/GameCardWishlist.vue'
import EventItem from '@/features/events/components/EventItem.vue'
import Button from 'openvue/button'

const router = useRouter()

const {
  user,
  isAuthenticated,
  settingsConf,
  updateSetting,
  logout,
  logoutAll
} = useProfileService()

const { wishlist, isPending: wishlistPending } = useWishlistService()
const { followedGames, gamesPending, followedEvents, eventsPending } = useUserFollowedService()

const {data: platforms} = useGetPlatformsGroups()
const {data: genres} = useGetGenres()

const activeTab = ref('settings')

function onLogout(fn: Function) {
  fn()
  router.push({name: 'home'})
}
</script>

<template>
  <div class="max-w-7xl w-full mt-5 mx-auto grid grid-cols-[25%_75%] gap-6">
    <!-- Left column: profile and actions -->
    <div class="flex flex-col gap-4">
      <div class="flex flex-col items-start gap-2">
        <h1 class="text-2xl font-semibold">{{ user?.username }}</h1>
        <div class="flex gap-2">
          <Button label="Logout" @click="onLogout(logout)" />
          <Button label="Logout all" @click="onLogout(logoutAll)" />
        </div>
      </div>

      <div class="mt-4">
        <SteamContainer />
      </div>
    </div>

    <!-- Right column: tabs -->
    <div class="flex flex-col gap-6">
      <Tabs :value="activeTab" class="mt-2">
        <TabList class="flex flex-wrap gap-2 mb-6">
          <Tab value="settings">Settings</Tab>
          <Tab value="followed-games">Followed games</Tab>
          <Tab value="followed-events">Followed events</Tab>
          <Tab value="wishlist">Wishlist</Tab>
          <Tab value="genres">Genres</Tab>
          <Tab value="platforms">Platforms</Tab>
        </TabList>

        <TabPanels class="p-0">
          <!-- Settings -->
          <TabPanel value="settings">
            <Settings />
            <PasswordChange />
          </TabPanel>

          <!-- Followed games -->
          <TabPanel value="followed-games">
            <h2 class="text-xl font-semibold mb-4">Followed games</h2>
            <div v-if="gamesPending" class="flex flex-col gap-2">
              <div class="h-4 bg-surface-200 rounded w-1/3 mb-2"></div>
              <div class="h-6 bg-surface-200 rounded w-full mb-4"></div>
            </div>
            <GameGrid v-else-if="followedGames" :games="followedGames">
            <GameCard v-for="game in followedGames" :key="game.id" :game="game" />
          </GameGrid>
            <div v-else class="text-muted-color">No followed games.</div>
          </TabPanel>

          <!-- Followed events -->
          <TabPanel value="followed-events">
            <h2 class="text-xl font-semibold mb-4">Followed events</h2>
            <div v-if="eventsPending" class="flex flex-col gap-2">
              <div class="h-4 bg-surface-200 rounded w-1/3 mb-2"></div>
              <div class="h-6 bg-surface-200 rounded w-full mb-4"></div>
            </div>
            <div v-else-if="followedEvents" class="flex flex-col gap-4">
              <EventItem v-for="e in followedEvents" :key="e.id" :event="e" />
            </div>
            <div v-else class="text-muted-color">No followed events.</div>
          </TabPanel>

          <!-- Wishlist -->
          <TabPanel value="wishlist">
            <h2 class="text-xl font-semibold mb-4">Wishlist</h2>
            <div v-if="wishlistPending" class="flex flex-col gap-2">
              <div class="h-4 bg-surface-200 rounded w-1/3 mb-2"></div>
              <div class="h-6 bg-surface-200 rounded w-full mb-4"></div>
            </div>
            <GameGrid v-else-if="wishlist" :games="wishlist">
            <GameCardWishlist v-for="game in wishlist" :key="game.id" :game="game" />
          </GameGrid>
            <div v-else class="text-muted-color">No games on wishlist.</div>
          </TabPanel>

          <!-- Genres -->
          <TabPanel value="genres">
            <h2 class="text-xl font-semibold mb-4">Genres</h2>
            <div class="flex flex-row flex-wrap gap-4">
              <GenreFollowCard v-for="g in (genres ?? [])" :key="String(g.id)"
                :id="String(g.id)"
                :name="String(g.name ?? '')"
              />
            </div>
          </TabPanel>

          <!-- Platforms -->
          <TabPanel value="platforms">
            <h2 class="text-xl font-semibold mb-4">Platforms</h2>
            <div class="flex flex-row flex-wrap gap-4">
              <PlatformFollowCard v-for="p in (platforms ?? [])" :key="String(p.id)"
                :id="String(p.id)"
                :name="String(p.name ?? '')"
              />
            </div>
          </TabPanel>
        </TabPanels>
      </Tabs>
    </div>
  </div>
</template>
