// user.store.js
import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { userService } from '../services/user.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = userService
export const useUserStore = defineStore('userStore', {
  state: () => ({
    users: [],
    pagination: {
      page: 1,
    },
    filters: {
      userName: '',
      userEmail: '',
      role: '',
      notInApartmanId: null,
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
          this.users = data.items ?? data
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt a felhasználók betöltése.',
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
          errorMessage: 'Sikertelen volt a felhasználó adatainak betöltése.',
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

    async create(payload) {
      if (!payload) throw new Error('Nincs payload')
      return runOp(
        this.ops.create,
        async () => {
          const data = await svc.create(payload)
          this.users.push(data)
          return data
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen létrehozta a felhasználót!',
          errorMessage: 'Sikertelen volt a felhasználó létrehozása.',
        },
      )
    },

    async update(payload) {
      if (!payload) throw new Error('Nincs payload')
      return runOp(
        this.ops.update,
        async () => {
          const updated = await svc.update(payload)
          const key = updated?.id ?? updated?.userId ?? payload.id
          const index = this.users.findIndex((u) => u.id === key)
          if (index !== -1) {
            this.users[index] = { ...this.users[index], ...updated }
          }
          await this.getAll()
          return updated
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen frissítette a felhasználó adatait!',
          errorMessage: 'Sikertelen volt a felhasználó adatainak frissítése.',
        },
      )
    },

    async delete(id) {
      if (!id) throw new Error('Nincs azonosító')
      return runOp(
        this.ops.delete,
        async () => {
          await svc.delete(id)
          const index = this.users.findIndex((u) => u.id === id)
          if (index !== -1) {
            this.users.splice(index, 1)
          }
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen törölte a felhasználót!',
          errorMessage: 'Sikertelen volt a felhasználó törlése.',
        },
      )
    },
  },
})
