import type { RouteRecordRaw } from "vue-router";

export const authRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'login',
    component: () => import('@/features/auth/LoginView.vue'),
    
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
  },
  {
    path: '/forgot-password',
    name: 'forgot-password',
    component: () => import('@/features/auth/ForgotPassword.vue')
  },
  {
    path: '/reset-password',
    name: 'reset-password',
    component: () => import('@/features/auth/PasswordReset.vue')
  }
  
]