import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { roomService } from '../services/room.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = roomService

export const useRoomStore = defineStore('roomStore', {
  state: () => ({
    rooms: [],
    pagination: {
      page: 1, // 1-ről indul
    },
    filters: {
      name: '',
      minCapacity: null,
      maxCapacity: null,
      apartmanId: null,
    },
    ops: {
      getAll: defaultOp(),
      getById: defaultOp(),
      create: defaultOp(),
      update: defaultOp(),
      delete: defaultOp(),
    },
  }),

  actions: {
    async getAll(overrides = {}) {
      // Csak azokat a szűrőket küldjük, amiknek van értéke
      const activeFilters = Object.fromEntries(
        Object.entries(this.filters).filter(
          ([_, value]) => value !== '' && value !== null && value !== undefined,
        ),
      )

      // A backend által elvárt lapos struktúra összeállítása
      const payload = {
        page: this.pagination.page,
        ...activeFilters,
        ...overrides,
      }

      return runOp(
        this.ops.getAll,
        async () => {
          const data = await svc.getAll(payload)
          this.rooms = data
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt a szobák betöltése.',
        },
      )
    },

    async goToPage(page) {
      if (page < 1) return
      this.pagination.page = page
      return this.getAll()
    },

    async applyFilters(newFilters = {}) {
      this.filters = { ...this.filters, ...newFilters }
      this.pagination.page = 1 // Új szűrésnél vissza az első oldalra
      return this.getAll()
    },

    // A többi CRUD művelet (getById, create, update, delete) változatlan maradhat
    async create(payload) {
      return runOp(
        this.ops.create,
        async () => {
          const data = await svc.create(payload)
          await this.getAll()
          return data
        },
        { notifyOnSuccess: true, successMessage: 'Sikeres létrehozás!' },
      )
    },

    async update(payload) {
      return runOp(
        this.ops.update,
        async () => {
          const updated = await svc.update(payload)
          await this.getAll()
          return updated
        },
        { notifyOnSuccess: true, successMessage: 'Sikeres frissítés!' },
      )
    },

    async delete(id) {
      return runOp(
        this.ops.delete,
        async () => {
          await svc.delete(id)
          this.rooms = this.rooms.filter((r) => r.id !== id)
        },
        { notifyOnSuccess: true, successMessage: 'Sikeres törlés!' },
      )
    },
  },
})
