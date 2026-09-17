import type { RouteRecordRaw } from 'vue-router'
import GameList from '@/features/games/GameList.vue'

export const defaultRoutes: RouteRecordRaw[] = [
  {
    path: '/games',
    name: 'games',
    component: GameList,
  },
  {
    path: '/games/:id',
    name: 'game-detail',
    component: () => import('@/features/games/GameDetails.vue'),
    props: true,
  },
  {
    path: '/events',
    name: 'events',
    component: () => import('@/features/events/EventCalendar.vue'),
  },
]