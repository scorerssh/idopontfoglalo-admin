import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { apartmanService } from '../services/apartman.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = apartmanService
export const useApartmanStore = defineStore('apartmanStore', {
  state: () => ({
    apartmans: [],
    pagination: {
      page: 1,
    },
    filters: {
      name: '',
      notInUserId: null,
      notInRoomId: null,
    },
    ops: {
      getAll: defaultOp(),
      getById: defaultOp(),
      getWithRooms: defaultOp(),
      getAllWithRooms: defaultOp(),
      create: defaultOp(),
      update: defaultOp(),
      delete: defaultOp(),
    },
  }),

  getters: {},

  actions: {
    async getAll(overrides = {}) {
      const activeFilters = Object.fromEntries(
        Object.entries(this.filters).filter(([_, v]) => v !== '' && v !== null),
      )

      const payload = {
        page: this.pagination.page,
        ...activeFilters,
        ...overrides,
      }
      return runOp(
        this.ops.getAll,
        async () => {
          const data = await svc.getAll(payload)
          this.apartmans = data
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt az apartmanok betöltése.',
        },
      )
    },

    async getAllWithRooms(overrides = {}) {
      const activeFilters = Object.fromEntries(
        Object.entries(this.filters).filter(([_, v]) => v !== '' && v !== null),
      )

      const payload = {
        page: this.pagination.page,
        ...activeFilters,
        ...overrides,
      }
      return runOp(
        this.ops.getAllWithRooms,
        async () => {
          const data = await svc.getAllWithRooms(payload)
          this.apartmans = data
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt az apartmanok és szobák betöltése.',
        },
      )
    },

    async getWithRooms(id) {
      if (!id) throw new Error('Nincs azonosító')
      return runOp(
        this.ops.getWithRooms,
        async () => {
          const data = await svc.getWithRooms(id)
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt az apartman és szobák betöltése.',
        },
      )
    },

    async goToPage(page) {
      this.pagination.page = page
      return this.getAll()
    },

    async applyFilters(newFilters = {}) {
      this.filters = { ...this.filters, ...newFilters }
      this.pagination.page = 1
      return this.getAll()
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
          errorMessage: 'Sikertelen volt az apartman betöltése.',
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
          successMessage: 'Sikeresen létrehozta az apartmant!',
          errorMessage: 'Sikertelen volt az apartman létrehozása.',
        },
      )
    },

    async update(payload) {
      if (!payload) throw new Error('Nincs payload')
      return runOp(
        this.ops.update,
        async () => {
          const updated = await svc.update(payload)
          await this.getAll() // Frissítjük a listát, hogy biztosan naprakész legyen
          return updated
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen frissítette az apartman adatait!',
          errorMessage: 'Sikertelen volt az apartman adatainak frissítése.',
        },
      )
    },

    async delete(id) {
      if (!id) throw new Error('Nincs azonosító')
      return runOp(
        this.ops.delete,
        async () => {
          await svc.delete(id)
          const index = this.apartmans.findIndex((u) => u.id === id)
          if (index !== -1) {
            this.apartmans.splice(index, 1)
          }
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen törölte az apartmant!',
          errorMessage: 'Sikertelen volt az apartman törlése.',
        },
      )
    },
  },
})
