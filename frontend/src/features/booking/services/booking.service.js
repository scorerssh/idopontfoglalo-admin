import { api } from '@/api/axios'

export const bookingService = {
  async createBooking(bookingData) {
    const { data } = await api.post('/api/Reservation/Create', bookingData)
    return data
  },

  async getAllAdmin(payload) {
    const { data } = await api.post('/api/Reservation/GetAllAdmin', payload)
    return data
  },

  async getAllUser(payload) {
    const { data } = await api.post('/api/Reservation/GetAllUser', payload)
    return data
  },

  async updateBoooking(payload) {
    const { data } = await api.patch('/api/Reservation/Update', payload)
    return data
  },

  async deleteBooking(id) {
    const { data } = await api.delete(`/api/Reservation/${id}`)
    return data
  },

  async getById(id) {
    const { data } = await api.get(`/api/Reservation/${id}`)
    return data
  },
}
