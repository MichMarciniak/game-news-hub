import { createRouter, createWebHistory } from 'vue-router'
import { authRoutes } from './auth.routes'
import { defaultRoutes } from './default.routes'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    ...authRoutes,
    ...defaultRoutes
  ],
})

export default router
