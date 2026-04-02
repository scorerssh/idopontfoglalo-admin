import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { bookingService } from '../services/booking.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = bookingService

export const useBookingStore = defineStore('bookingStore', {
  state: () => ({
    bookings: [],
    adminFilters: {
      name: '',
      email: '',
      roomId: null,
      apartmanId: null,
      startDate: '',
      endDate: '',
    },
    userFilters: {
      name: '',
      email: '',
      roomId: null,
      apartmanId: null,
      userId: null,
      startDate: '',
      endDate: '',
    },
    pagination: {
      page: 1,
    },
    ops: {
      getAllAdmin: defaultOp(),
      getAllUser: defaultOp(),
      getById: defaultOp(),
      createBooking: defaultOp(),
    },
  }),

  actions: {
    async getAllAdmin() {
      // 1. Kulcsok átnevezése a backend elvárásai szerint (startDate -> startTIme)
      const { startDate: startTIme, endDate: endTime, ...rest } = this.adminFilters
      const renamedFilters = { startTIme, endTime, ...rest }

      // 2. Üres vagy null értékek eltávolítása a payloadból
      const activeFilters = Object.fromEntries(
        Object.entries(renamedFilters).filter(
          ([_, value]) => value !== '' && value !== null && value !== undefined,
        ),
      )

      const payload = {
        page: this.pagination.page,
        ...activeFilters,
      }

      return runOp(
        this.ops.getAllAdmin,
        async () => {
          const response = await svc.getAllAdmin(payload)
          this.bookings = response
          return response
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Hiba a foglalások betöltésekor',
        },
      )
    },

    async getAllUser() {
      // 1. Kulcsok átnevezése a backend elvárásai szerint
      const { startDate: startTIme, endDate: endTime, ...rest } = this.userFilters
      const renamedFilters = { startTIme, endTime, ...rest }

      // 2. Üres vagy null értékek eltávolítása
      const activeFilters = Object.fromEntries(
        Object.entries(renamedFilters).filter(
          ([_, value]) => value !== '' && value !== null && value !== undefined,
        ),
      )

      const payload = {
        page: this.pagination.page,
        ...activeFilters,
      }

      return runOp(
        this.ops.getAllUser,
        async () => {
          const response = await svc.getAllUser(payload)
          this.bookings = response
          return response
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Hiba a foglalások betöltésekor',
        },
      )
    },

    async getById(id) {
      return runOp(
        this.ops.getById,
        async () => {
          const response = await svc.getById(id)
          return response
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Hiba a foglalás betöltésekor',
        },
      )
    },

    async createBooking(bookingData) {
      return runOp(
        this.ops.createBooking,
        async () => {
          const response = await svc.createBooking(bookingData)
          // Ha sikeres a mentés, adjuk hozzá a helyi listához
          if (response?.data) {
            this.bookings.push(response.data)
          }
          return response?.data
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Foglalás sikeresen létrehozva',
          errorMessage: 'Hiba a foglalás létrehozásakor',
        },
      )
    },

    /* FILTERS */
    async applyAdminFilters(newFilters = {}) {
      this.adminFilters = { ...this.adminFilters, ...newFilters }
      this.pagination.page = 1
      return this.getAllAdmin()
    },

    async applyUserFilters(newFilters = {}) {
      this.userFilters = { ...this.userFilters, ...newFilters }
      this.pagination.page = 1
      return this.getAllUser()
    },
  },
})
