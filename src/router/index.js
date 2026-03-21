import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login',
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@views/login.vue'),
    },
    {
      path: '/',
      component: () => import('@layouts/DefaultLayout.vue'),
      children: [
        {
          path: '/admin-dashboard',
          name: 'admin-dashboard',
          component: () => import('@views/dashboard/admin-dashboard.vue'),
        },
        {
          path: '/users',
          name: 'users',
          component: () => import('@views/users.vue'),
        },
        {
          path: '/bookings',
          name: 'bookings',
          component: () => import('@/views/bookings.vue'),
        },
        {
          path: '/rooms',
          name: 'rooms',
          component: () => import('@/views/rooms.vue'),
        },  
        {
          path: '/settings',
          name: 'settings',
          component: () => import('@/views/settings.vue'),
        },
        {
          path: '/apartments',
          name: 'apartments',
          component: () => import('@/views/apartments.vue'),
        },
        {
          path: '/videos',
          name: 'videos',
          component: () => import('@views/videos.vue'),
        },
        {
          path: '/calendar',
          name: 'calendar',
          component: () => import('@views/calendar.vue'),
        },

        {
          path: '/403',
          name: 'forbidden',
          component: () => import('@views/errors/403.vue'),
        },
      ],
    },

    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@views/errors/404.vue'),
    },
  ],
})

export default router
