import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { roomService } from '../services/room.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = roomService

function filterRooms(rooms = [], filters = {}) {
  return rooms.filter((room) => {
    const matchesName =
      !filters.name || room.name?.toLowerCase().includes(String(filters.name).toLowerCase())
    const matchesMinCapacity =
      filters.minCapacity === null ||
      filters.minCapacity === undefined ||
      Number(room.minCapacity ?? room.maxCapacity ?? 0) >= Number(filters.minCapacity)
    const matchesMaxCapacity =
      filters.maxCapacity === null ||
      filters.maxCapacity === undefined ||
      Number(room.maxCapacity ?? room.minCapacity ?? 0) <= Number(filters.maxCapacity)
    const matchesApartman =
      filters.apartmanId === null ||
      filters.apartmanId === undefined ||
      Number(room.apartmanId) === Number(filters.apartmanId)

    return matchesName && matchesMinCapacity && matchesMaxCapacity && matchesApartman
  })
}

export const useRoomStore = defineStore('roomStore', {
  state: () => ({
    rooms: [],
    allRooms: [],
    sourceMode: 'admin',
    pagination: {
      page: 1,
    },
    filters: {
      name: '',
      minCapacity: null,
      maxCapacity: null,
      apartmanId: null,
    },
    ops: {
      getAll: defaultOp(),
      getAllUser: defaultOp(),
      getById: defaultOp(),
      create: defaultOp(),
      update: defaultOp(),
      delete: defaultOp(),
    },
  }),

  actions: {
    setRooms(rooms = [], sourceMode = 'admin') {
      this.sourceMode = sourceMode
      this.allRooms = Array.isArray(rooms) ? rooms : []
      this.rooms = filterRooms(this.allRooms, this.filters)
      this.pagination.page = 1
      return this.rooms
    },

    async getAll(overrides = {}) {
      const activeFilters = Object.fromEntries(
        Object.entries(this.filters).filter(
          ([_, value]) => value !== '' && value !== null && value !== undefined,
        ),
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
          this.sourceMode = 'admin'
          this.allRooms = data
          this.rooms = data
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt a szobák betöltése.',
        },
      )
    },

    async getAllUser(overrides = {}) {
      const activeFilters = Object.fromEntries(
        Object.entries(this.filters).filter(
          ([_, value]) => value !== '' && value !== null && value !== undefined,
        ),
      )

      const payload = {
        page: this.pagination.page,
        ...activeFilters,
        ...overrides,
      }

      return runOp(
        this.ops.getAllUser,
        async () => {
          const data = await svc.getAllUser(payload)
          this.sourceMode = 'user'
          this.allRooms = data
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
      if (this.sourceMode === 'user') return

      this.pagination.page = page
      return this.getAll()
    },

    async applyFilters(newFilters = {}) {
      this.filters = { ...this.filters, ...newFilters }
      this.pagination.page = 1

      if (this.sourceMode === 'user') {
        this.rooms = filterRooms(this.allRooms, this.filters)
        return this.rooms
      }

      return this.getAll()
    },

    async create(payload) {
      return runOp(
        this.ops.create,
        async () => {
          const data = await svc.create(payload)

          if (this.sourceMode === 'admin') {
            await this.getAll()
          } else if (data) {
            this.allRooms.push(data)
            this.rooms = filterRooms(this.allRooms, this.filters)
          }

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

          if (this.sourceMode === 'admin') {
            await this.getAll()
          } else {
            const roomId = payload?.roomId ?? payload?.id
            if (roomId) {
              const { roomId: _r, ...fieldsToUpdate } = payload
              this.allRooms = this.allRooms.map((room) =>
                room.id === roomId ? { ...room, ...fieldsToUpdate } : room,
              )
              this.rooms = filterRooms(this.allRooms, this.filters)
            }
          }
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
          this.allRooms = this.allRooms.filter((r) => r.id !== id)
          this.rooms = this.rooms.filter((r) => r.id !== id)
        },
        { notifyOnSuccess: true, successMessage: 'Sikeres törlés!' },
      )
    },
  },
})
