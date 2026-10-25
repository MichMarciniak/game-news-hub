import { createRouter, createWebHistory } from 'vue-router'
import { authRoutes } from './auth.routes'
import { defaultRoutes } from './default.routes'
import { useAuthStore } from '@/stores/auth'
import { adminRoutes } from './admin.routes'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    ...authRoutes,
    ...defaultRoutes,
    ...adminRoutes
  ],
})

router.beforeEach(async (to, from) => {
  const authStore = useAuthStore()

  if (!authStore) {
    return false
  }

  // Ensure cookie-based session is restored before auth checks on hard refresh.
  await authStore.initialize()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return ({
      name: 'login',
      query: {redirectPath: to.fullPath}
    })
  }

  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    return ({name: 'not-found'})
  }
})

export default router
