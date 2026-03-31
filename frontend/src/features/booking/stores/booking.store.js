import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { bookingService } from '../services/booking.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = bookingService
export const useBookingStore = defineStore('bookingStore', {
  state: () => ({
    booking: [],
    ops: {
      createBooking: defaultOp(),
    },
  }),
  actions: {
    async createBooking(bookingData) {
      return runOp(
        this.ops.createBooking,
        async () => {
          const response = await svc.createBooking(bookingData)
          this.booking.push(response.data)
          return response.data
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Booking created successfully',
          errorMessage: 'Failed to create booking',
        },
      )
    },
  },
})
