import { createRouter, createWebHistory } from 'vue-router'
import { authRoutes } from './auth.routes'
import { defaultRoutes } from './default.routes'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    ...authRoutes,
    ...defaultRoutes
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (!authStore) {
    return false
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({
      name: 'login',
      query: {redirectPath: to.fullPath}
    })
  }
  else next()
})

export default router
