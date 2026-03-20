// user.service.js
import { api } from '@/api/axios'

export const userService = {
  async getAll(params = {}) {
    const { data } = await api.post('/api/User/GetAll', params)
    return data
  },
  async getById(id) {
    const { data } = await api.get(`/api/User/${id}`)
    return data
  },
  async create(payload) {
    const { data } = await api.post('/api/User/Create', payload)
    return data
  },
  async update(payload) {
    const { data } = await api.patch('/api/User/Update', payload)
    return data
  },
  async delete(id) {
    const { data } = await api.delete(`/api/User/${id}`)
    return data
  },
}
