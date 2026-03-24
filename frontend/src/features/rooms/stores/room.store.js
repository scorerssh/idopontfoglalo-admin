import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { roomService } from '../services/room.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = roomService
export const useRoomStore = defineStore('roomStore', {
  state: () => ({
    rooms: [],
    pagination: {
      page: 1,
      perPage: 10,
    },
    filters: {
      name: '',
      minCapacity: null,
      maxCapacity: null,
    },
    order: {
      sortBy: 'name',
      sort: 'Ascending',
      combineWith: 'And',
    },
    ops: {
      getAll: defaultOp(),
      getById: defaultOp(),
      create: defaultOp(),
      update: defaultOp(),
      delete: defaultOp(),
    },
  }),

  getters: {},

  actions: {
    async getAll(overrides = {}) {
      const activeFilters = Object.fromEntries(
        Object.entries(this.filters).filter(
          ([key, value]) =>
            ['name', 'minCapacity', 'maxCapacity'].includes(key) &&
            value !== '' &&
            value !== null &&
            value !== undefined,
        ),
      )

      const payload = {
        page: this.pagination.page,
        ...(Object.keys(activeFilters).length > 0 ? { filter: activeFilters } : {}),
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

    async getById(id) {
      if (!id) throw new Error('Nincs azonosító')
      return runOp(
        this.ops.getById,
        async () => {
          const data = await svc.getById(id)
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt a szoba betöltése.',
        },
      )
    },

    async create(payload) {
      if (!payload) throw new Error('Nincs payload')
      return runOp(
        this.ops.create,
        async () => {
          const data = await svc.create(payload)
          await this.getAll()
          return data
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen létrehozta a szobát!',
          errorMessage: 'Sikertelen volt a szoba létrehozása.',
        },
      )
    },

    async update(payload) {
      if (!payload) throw new Error('Nincs payload')
      return runOp(
        this.ops.update,
        async () => {
          const updated = await svc.update(payload)
          await this.getAll()
          return updated
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen frissítette a szoba adatait!',
          errorMessage: 'Sikertelen volt a szoba adatainak frissítése.',
        },
      )
    },

    async delete(id) {
      if (!id) throw new Error('Nincs azonosító')
      return runOp(
        this.ops.delete,
        async () => {
          await svc.delete(id)
          const index = this.rooms.findIndex((r) => r.id === id)
          if (index !== -1) {
            this.rooms.splice(index, 1)
          }
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen törölte a szobát!',
          errorMessage: 'Sikertelen volt a szoba törlése.',
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
      this.pagination.page = 1
      return this.getAll()
    },
  },
})
