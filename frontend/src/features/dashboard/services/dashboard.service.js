import { api } from '@/api/axios'

export const dashboardService = {
  async getDashboard() {
    const { data } = await api.get('/api/Dashboard/Get')
    return data
  },
}
