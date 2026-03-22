import { defineStore } from 'pinia'

export const useNotificationStore = defineStore('notificationStore', {
  state: () => ({
    type: null,
    message: '',
    id: 0,
  }),

  actions: {
    show(type, message) {
      this.type = type
      this.message = message || ''
      this.id = Date.now() //
    },

    success(message) {
      this.show('success', message)
    },

    error(message) {
      this.show('error', message)
    },

    info(message) {
      this.show('info', message)
    },

    clear() {
      this.type = null
      this.message = ''
      this.id = 0
    },
  },
})
