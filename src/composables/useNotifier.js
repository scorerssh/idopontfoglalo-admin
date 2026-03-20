// composables/useNotifier.js
import { storeToRefs } from 'pinia'
import { useNotificationStore } from '@/stores/notifcation'
export function useNotifier() {
  const store = useNotificationStore()
  const { type, message, id } = storeToRefs(store)

  return {
    type,
    message,
    id,

    success: store.success.bind(store),
    error: store.error.bind(store),
    info: store.info.bind(store),
    clear: store.clear.bind(store),
  }
}
