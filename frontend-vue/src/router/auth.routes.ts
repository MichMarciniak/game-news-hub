import type { RouteRecordRaw } from "vue-router";

export const authRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/features/auth/pages/LoginPage.vue')
  },
  {
    path: '/register',
    name: 'register',
    component: () => import('@/features/auth/pages/RegisterPage.vue')
  },
  {
    path: '/confirm-email',
    name: 'confirm-email',
    component: () => import('@/features/auth/pages/ConfirmEmailPage.vue')
  },
  {
    path: '/confirm-email-sent',
    name: 'confirm-email-sent',
    component: () => import('@/features/auth/pages/ConfirmEmailSentPage.vue')
  },
  {
    path: '/forgot-password',
    name: 'forgot-password',
    component: () => import('@/features/auth/pages/ForgotPasswordPage.vue')
  },
  {
    path: '/reset-password',
    name: 'reset-password',
    component: () => import('@/features/auth/pages/PasswordResetPage.vue')
  }
]