import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { defaultOp } from '@/utils/opsHelper'
import { dashboardService } from '../services/dashboard.service'

const svc = dashboardService

function createDefaultDashboard() {
  return {
    totalReservations: 0,
    activeRooms: 0,
    reservationsCreatedThisMonth: 0,
    totalRevenue: 0,
    monthlyRevenues: [],
    roomRevenues: [],
  }
}

export const useDashboardStore = defineStore('dashboardStore', {
  state: () => ({
    dashboard: createDefaultDashboard(),
    ops: {
      getDashboard: defaultOp(),
    },
  }),

  actions: {
    async getDashboard() {
      return runOp(
        this.ops.getDashboard,
        async () => {
          const data = await svc.getDashboard()
          this.dashboard = {
            ...createDefaultDashboard(),
            ...data,
            monthlyRevenues: Array.isArray(data?.monthlyRevenues) ? data.monthlyRevenues : [],
            roomRevenues: Array.isArray(data?.roomRevenues) ? data.roomRevenues : [],
          }
          return this.dashboard
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt a dashboard adatok betoltese.',
        },
      )
    },
  },
})
