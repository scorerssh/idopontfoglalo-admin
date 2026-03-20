import { defineStore } from 'pinia'
import { runOp } from '@/utils/storeUtils'
import { roomService } from '../services/room.service'
import { defaultOp } from '@/utils/opsHelper'

const svc = roomService
export const useRoomStore = defineStore('roomStore', {
  state: () => ({
    rooms: [],
    ops: {
      getAll: defaultOp(),
      create: defaultOp(),
      update: defaultOp(),
      delete: defaultOp(),
    },
  }),

  getters: {},

  actions: {
    async getAll() {
      return runOp(
        this.ops.getAll,
        async () => {
          const data = await svc.getAll()
          this.rooms = data
          return data
        },
        {
          notifyOnSuccess: false,
          errorMessage: 'Sikertelen volt a szobák betöltése.',
        },
      )
    },

    async create(payload) {
      if (!payload) throw new Error('Nincs payload')
      return runOp(
        this.ops.create,
        async () => {
          const data = await svc.create(payload)
          this.rooms.push(data)
          return data
        },
        {
          notifyOnSuccess: true,
          successMessage: 'Sikeresen létrehozta a szobát!',
          errorMessage: 'Sikertelen volt a szoba létrehozása.',
        },
      )
    },

    async update(id, payload) {
      if (!payload) throw new Error('Nincs payload')
      if (!id) throw new Error('Nincs azonosító')
      return runOp(
        this.ops.update,
        async () => {
          const updated = await svc.update(id, payload)
          const key = updated?.id ?? updated?.userId ?? payload.id
          const index = this.rooms.findIndex((r) => r.id === key)
          if (index !== -1) {
            this.rooms[index] = { ...this.rooms[index], ...updated }
          }
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
  },
})
