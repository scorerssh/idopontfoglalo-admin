import { api } from '@/api/axios'

export const roomService = {
  async getAll() {
    const { data } = await api.get('/api/rooms')
    return data
  },
  async create(payload) {
    const { data } = await api.post('/api/rooms', payload)
    return data
  },
  async update(payload) {
    const { data } = await api.patch('/api/rooms/', payload)
    return data
  },
  async delete(id) {
    const { data } = await api.delete(`/api/rooms/${id}`)
    return data
  },
}
