<script setup lang="ts">
import { useFollowedGenres } from '@/composables/genreFollows';
import { useFollowedPlatforms } from '@/composables/platformFollows';
import { useAuthStore } from '@/stores/auth';
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import RecommendedList from '../events/list/RecommendedList.vue';
import EventWeekList from '../events/list/EventWeekList.vue';

const router = useRouter()

const authStore = useAuthStore()
const {followedPlatformIds} = useFollowedPlatforms()
const {followedGenreIds} = useFollowedGenres()

const notFollowedPlatformsOrGenres = computed(() => followedGenreIds.value.size === 0 || followedPlatformIds.value.size === 0)

const gridCols = computed(() =>
  notFollowedPlatformsOrGenres.value ? 'lg:grid-cols-3' : 'sm:grid-cols-2'
)

</script>

<template>
  <div class="flex flex-col max-w-7xl w-full">

    <h2 class="text-2xl">Hello {{ authStore.user?.username }}</h2>
    <div class="grid grid-cols-1 gap-2 mt-2 flex-1"
          :class="gridCols">
      <!-- Recommended + followed? -->
      <RecommendedList/>
      
      <!-- upcoming (default) -->
      <EventWeekList />
      
      <!-- Follow platforms / genres -->
      <div v-if="notFollowedPlatformsOrGenres">
        <Panel class="h-full" :pt="{header: '!pb-0'}">
          <template #header>
            <h3 class="text-xl">Finish account creation</h3>
          </template>
          <div class="flex flex-col">
            <Divider />
            <div class="flex flex-col items-center gap-5">
  
              <span class="text-muted-color">Follow genres and platforms to personalize your recommendations.</span>
              <RouterLink to="/me">
                <Button variant="outlined">Go to profile</Button>
              </RouterLink>
            </div>
          </div>
        </Panel>
      </div>
      
    </div>
    
  </div>
</template>