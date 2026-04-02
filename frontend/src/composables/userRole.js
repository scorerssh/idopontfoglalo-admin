import { computed } from 'vue'
import { useAuthStore } from '@/features/auth/stores/auth'

export function useRole() {
  const authStore = useAuthStore()

  const role = computed(() => authStore.user?.role ?? null)
  const isAdmin = computed(() => role.value === 'Admin')
  const isUser = computed(() => role.value === 'User')

  return { role, isAdmin, isUser }
}
