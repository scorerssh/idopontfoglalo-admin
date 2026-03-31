import { api } from '@/api/axios'

export const bookingService = {
  createBooking(bookingData) {
    return api.post('/api/Reservation/Create', bookingData)
  },
}
