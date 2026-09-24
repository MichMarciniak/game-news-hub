import type { RouteRecordRaw } from 'vue-router'
import GameList from '@/features/games/GameList.vue'
import HomeView from '@/views/HomeView.vue'
import ProfileView from '@/views/ProfileView.vue'

export const defaultRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/me',
    name: 'me',
    component: ProfileView
  },
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
  {
    path: '/events/search',
    name: 'events-search',
    component: () => import('@/features/events/EventList.vue')
  },
  {
    path: '/events/:id',
    name: 'event-detail',
    component: () => import('@/features/events/EventDetails.vue')
  },
  {
    path: '/events/recommended',
    name: 'events-recommended',
    component: () => import('@/features/events/RecommendedCalendar.vue')
  }
]