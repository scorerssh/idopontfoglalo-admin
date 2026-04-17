import { api } from '@/api/axios'

export const roomService = {
  async getAll(payload) {
    const { data } = await api.post('/api/Room/GetAll', payload)
    return data
  },
  async getAllUser(payload) {
    const { data } = await api.post('/api/Room/GetAllUser', payload)
    return data
  },
  async getById(id) {
    const { data } = await api.get(`/api/Room/${id}`)
    return data
  },
  async create(payload) {
    const { data } = await api.post('/api/Room/Create', payload)
    return data
  },
  async update(payload) {
    const { data } = await api.patch('/api/Room/Update', payload)
    return data
  },
  async updatePriceTier(payload) {
    const { data } = await api.patch('/api/RoomPriceTier/Update', payload)
    return data
  },
  async delete(id) {
    const { data } = await api.delete(`/api/Room/${id}`)
    return data
  },
}
