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
          path: '/user-dashboard',
          name: 'user-dashboard',
          component: () => import('@views/dashboard/user-dashboard.vue'),
          meta: { requiresAuth: true, roles: ['User'] },
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

router.beforeEach(async (to) => {
  if (import.meta.env.VITE_DEV_BYPASS_AUTH === 'true') return true
  if (!to.meta.requiresAuth) return true

  const authStore = useAuthStore()

  console.log('🔒 GUARD INDUL | bootstrapStatus:', authStore.bootstrapStatus)

  await authStore.ensureCheckedOnce()

  console.log(
    '✅ GUARD UTÁN  | user:',
    authStore.user,
    '| role:',
    authStore.role,
    '| required:',
    to.meta.roles,
  )

  if (!authStore.user) return { name: 'login' }
  if (!to.meta.roles?.length) return true

  const userRole = authStore.role?.toLowerCase()
  const allowed = to.meta.roles.map((r) => r.toLowerCase())

  console.log(
    '🎭 ROLE CHECK  | userRole:',
    userRole,
    '| allowed:',
    allowed,
    '| match:',
    allowed.includes(userRole),
  )

  return allowed.includes(userRole) ? true : { name: 'forbidden' }
})

export default router
