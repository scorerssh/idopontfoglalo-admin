import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@features/auth/stores/auth'

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
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/users',
          name: 'users',
          component: () => import('@views/users.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/bookings',
          name: 'bookings',
          component: () => import('@/views/bookings.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/rooms',
          name: 'rooms',
          component: () => import('@/views/rooms.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/settings',
          name: 'settings',
          component: () => import('@/views/settings.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/apartments',
          name: 'apartments',
          component: () => import('@/views/apartments.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/videos',
          name: 'videos',
          component: () => import('@views/videos.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
        },
        {
          path: '/calendar',
          name: 'calendar',
          component: () => import('@views/calendar.vue'),
          meta: { requiresAuth: true, roles: ['Admin'] },
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

router.beforeEach((to) => {
  if (import.meta.env.VITE_DEV_BYPASS_AUTH === 'true') {
    return true
  }

  if (!to.meta.requiresAuth) {
    return true
  }

  const authStore = useAuthStore()
  const user = authStore.user

  if (!user) {
    return { name: 'login' }
  }

  // 3. Ha nincs roles a meta-ban, bejelentkezés elég
  if (!to.meta.roles || to.meta.roles.length === 0) {
    return true
  }

  // 4. Megvan a szükséges role?
  if (to.meta.roles.includes(user.role)) {
    return true
  }

  // 5. Nincs jogosultság -> 403
  return { name: 'forbidden' }
})

export default router
