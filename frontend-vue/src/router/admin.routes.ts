import type { RouteRecordRaw } from "vue-router";

export const adminRoutes: RouteRecordRaw[] = [
  {
    path: '/admin',
    name: 'admin',
    component: () => import('@/views/AdminView.vue'),
    meta: {requiresAdmin: true}
  },
  {
    path: '/admin/event-assign',
    name: 'events-assign',
    component: () => import('@/features/admin/ReassignEvents.vue')
  },
  {
    path: '/admin/platform-assign',
    name: 'platform-assign',
    component: () => import('@/features/admin/ReassignPlatforms.vue')
  }
]