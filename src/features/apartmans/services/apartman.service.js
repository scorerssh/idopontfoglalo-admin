import { api } from '@/api/axios'

export const apartmanService = {
  async getAll(params = {}) {
    const { data } = await api.post('/api/Apartman/GetAll', params)
    return data
  },
  async getAllWithRooms(params = {}) {
    const { data } = await api.post('/api/Apartman/GetAllWithRooms', params)
    return data
  },
  async getById(id) {
    const { data } = await api.get(`/api/Apartman/${id}`)
    return data
  },
  async getWithRooms(id) {
    const { data } = await api.get(`/api/Apartman/WithRooms/${id}`)
    return data
  },
  async create(payload) {
    const { data } = await api.post('/api/Apartman/Create', payload)
    return data
  },
  async update(payload) {
    const { data } = await api.patch('/api/Apartman/Update', payload)
    return data
  },
  async delete(id) {
    const { data } = await api.delete(`/api/Apartman/${id}`)
    return data
  },
}
