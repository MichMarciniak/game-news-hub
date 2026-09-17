import { createRouter, createWebHistory } from 'vue-router'
import GameDetails from '@/features/games/GameDetails.vue'
import GameList from '@/features/games/GameList.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/games',
      name: 'games',
      component: GameList //pobiera się od razu z wejściem na stronę
    },
    {
      path: '/games/:id',
      name: 'game-detail',
      component: () => import('../features/games/GameDetails.vue'), //pobiera się dopiero jak wchodzi na ten path
      props: true
    },
    {
      path: '/events',
      name: 'events',
      component: () => import('../features/events/EventCalendar.vue'),
    }
  ],
})

export default router
