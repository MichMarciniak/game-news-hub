import type { RouteRecordRaw } from 'vue-router'


export const defaultRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: () => import('@/features/home/HomePageWrapper.vue'),
  },
  {
    path: '/me',
    name: 'me',
    component: () => import('@/features/profile/pages/ProfilePage.vue'),
    meta: {requiresAuth: true}
  },
  {
    path: '/games',
    name: 'games',
    component: () => import('@/features/games/GameSearchPage.vue'),
  },
  {
    path: '/games/:id',
    name: 'game-detail',
    component: () => import('@/features/games/GameDetailsPage.vue'),
    props: true,
  },
  {
    path: '/events',
    name: 'events',
    component: () => import('@/features/events/pages/EventCalendarPage.vue'),
  },
  {
    path: '/events/search',
    name: 'events-search',
    component: () => import('@/features/events/pages/EventSearchPage.vue')
  },
  {
    path: '/events/:id',
    name: 'event-detail',
    component: () => import('@/features/events/pages/EventDetailsPage.vue')
  },
  {
    path: '/events/recommended',
    name: 'events-recommended',
    component: () => import('@/features/events/pages/RecommendedCalendarPage.vue'),
    meta: {requiresAuth: true}
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/NotFoundPage.vue')
  }
]