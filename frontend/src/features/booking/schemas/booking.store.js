import { defineStore } from 'pinia'
import bookingShecmas from './booking.schema'

export const useBookingStore = defineStore('bookingStore', {
  state: () => ({
    booking: [],
  }),

  getters: {},

  actions: {},
})
