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

  agePriceTier: {
    async getByRoom(roomId) {
      const { data } = await api.get(`/api/AgePriceTier/Room/${roomId}`)
      return data
    },
    async getById(agePriceTierId) {
      const { data } = await api.get(`/api/AgePriceTier/${agePriceTierId}`)
      return data
    },
    async create(payload) {
      const { data } = await api.post('/api/AgePriceTier/Create', payload)
      return data
    },
    async update(payload) {
      const { data } = await api.patch('/api/AgePriceTier/Update', payload)
      return data
    },
    async delete(agePriceTierId) {
      const { data } = await api.delete(`/api/AgePriceTier/${agePriceTierId}`)
      return data
    },
  },

  specialPricingRule: {
    async getTypes() {
      const { data } = await api.get('/api/RoomSpecialPricingRule/Types')
      return data
    },
    async getByRoom(roomId) {
      const { data } = await api.get(`/api/RoomSpecialPricingRule/Room/${roomId}`)
      return data
    },
    async getById(roomSpecialPricingRuleId) {
      const { data } = await api.get(`/api/RoomSpecialPricingRule/${roomSpecialPricingRuleId}`)
      return data
    },
    async create(payload) {
      const { data } = await api.post('/api/RoomSpecialPricingRule/Create', payload)
      return data
    },
    async update(payload) {
      const { data } = await api.patch('/api/RoomSpecialPricingRule/Update', payload)
      return data
    },
    async delete(roomSpecialPricingRuleId) {
      const { data } = await api.delete(`/api/RoomSpecialPricingRule/${roomSpecialPricingRuleId}`)
      return data
    },
  },
}
