import type { RouteRecordRaw } from "vue-router";

export const authRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/features/auth/LoginPage.vue'),
    
  },
  {
    path: '/register',
    name: 'register',
    component: () => import('@/features/auth/RegisterPage.vue')
  },
  {
    path: '/confirm-email',
    name: 'confirm-email',
    component: () => import('@/features/auth/ConfirmEmailPage.vue')
  },
  {
    path: '/confirm-email-sent',
    name: 'confirm-email-sent',
    component: () => import('@/features/auth/ConfirmEmailSentPage.vue')
  },
  {
    path: '/forgot-password',
    name: 'forgot-password',
    component: () => import('@/features/auth/ForgotPasswordPage.vue')
  },
  {
    path: '/reset-password',
    name: 'reset-password',
    component: () => import('@/features/auth/PasswordResetPage.vue')
  }
  
]