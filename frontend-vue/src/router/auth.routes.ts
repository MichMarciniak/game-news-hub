import type { RouteRecordRaw } from "vue-router";

export const authRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/features/auth/LoginView.vue')
  },
  {
    path: '/register',
    name: 'register',
    component: () => import('@/features/auth/RegisterView.vue')
  },
  {
    path: '/confirm-email',
    name: 'confirm-email',
    component: () => import('@/features/auth/ConfirmEmail.vue')
  },
  {
    path: '/confirm-email-sent',
    name: 'confirm-email-sent',
    component: () => import('@/features/auth/ConfirmEmailSent.vue')
  }
  
]