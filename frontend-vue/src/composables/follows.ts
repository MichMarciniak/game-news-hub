import { useGetUsersMeEventsFollowed, useGetUsersMeGamesFollowed, useGetUsersMeGenresFollowed } from "@/api/generated";
import { useAuthStore } from "@/stores/auth";
import { defineStore } from "pinia";
import { computed, ref } from "vue";


export function useFollowedSets() {
  const auth = useAuthStore();
  const enabled = computed(() => auth.isAuthenticated)

  const {data: gamesData } = useGetUsersMeGamesFollowed({query: {enabled}})
  const {data: eventsData} = useGetUsersMeEventsFollowed({query:{enabled}})
  const {data: genresData} = useGetUsersMeGenresFollowed({query:{enabled}})
  const {data: platformsData} = useGetUsersMeGenresFollowed({query:{enabled}})

  

  const followedGameIds = computed(() => new Set(gamesData.value?.data.map(g => String(g.id)) ?? []))
  const followedEventIds = computed(() => new Set(eventsData.value?.data.map(e => String(e.id)) ?? []))
  const followedGenreIds = computed(() => new Set(genresData.value?.data.map(e => String(e.id)) ?? []))
  const followedPlatformIds = computed(() => new Set(platformsData.value?.data.map(e => String(e.id)) ?? []))

  return {followedGameIds, followedEventIds, followedGenreIds, followedPlatformIds}
}