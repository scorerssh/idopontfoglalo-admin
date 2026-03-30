import { defineStore } from 'pinia'
import { authService } from '@features/auth/services/authService'
import { STATUS } from '@constants/status'
import router from '@/router'

const AUTH_TTL_MS = 60_000

export const useAuthStore = defineStore('auth', {
  state: () => ({
    authenticated: false,
    user: null,
    role: null,
    status: STATUS.idle,
    error: null,
    lastCheckedAt: null,
    inFlight: null,
    bootstrapStatus: 'idle',
  }),

  getters: {
    isFresh(state) {
      if (!state.lastCheckedAt) return false
      return Date.now() - state.lastCheckedAt < AUTH_TTL_MS
    },

    isLoading: (state) => state.status === STATUS.loading,
    hasError: (state) => Boolean(state.error),
    userName: (state) => state.user?.name ?? state.user?.fullName ?? null,
  },

  actions: {
    _toUserError(error, fallback = 'Hiba történt.') {
      return error?.userMessage || error?.response?.data?.message || error?.message || fallback
    },

    resetAuthState() {
      this.authenticated = false
      this.user = null
      this.role = null
      this.lastCheckedAt = null
    },

    clearError() {
      this.error = null
    },
    async ensureCheckedOnce() {
      if (this.bootstrapStatus === 'done') return

      if (this.bootstrapStatus === 'checking') {
        if (this.inFlight) await this.inFlight
        return
      }

      this.bootstrapStatus = 'checking'
      try {
        await this.fetchWhoAmI()
      } catch {
      } finally {
        this.bootstrapStatus = 'done'
      }
    },

    async fetchWhoAmI({ force = false } = {}) {
      if (!force && this.isFresh) return this.user

      if (this.inFlight) {
        await this.inFlight
        return this.user
      }

      this.status = STATUS.loading
      this.error = null

      this.inFlight = (async () => {
        try {
          const me = await authService.getWhoAmI()

          const user = me?.user ?? me ?? null

          this.user = user
          this.authenticated = Boolean(user)
          this.role = me?.role ?? user?.role ?? null
          this.status = STATUS.success
          this.lastCheckedAt = Date.now()
        } catch (error) {
          this.status = STATUS.error
          this.error = this._toUserError(error, 'Sikertelen azonosítás.')

          const statusCode = error?.response?.status
          if (statusCode === 401 || statusCode === 403) {
            this.resetAuthState()
          }
        } finally {
          this.inFlight = null
        }
      })()

      await this.inFlight
      return this.user
    },

    async login(credentials) {
      this.status = STATUS.loading
      this.error = null

      try {
        await authService.login(credentials)

        await this.fetchWhoAmI({ force: true })

        this.bootstrapStatus = 'done'
      } catch (error) {
        this.status = STATUS.error
        this.error = this._toUserError(error, 'Sikertelen bejelentkezés.')
        this.resetAuthState()
        throw error
      }
    },

    async logout() {
      this.status = STATUS.loading
      this.error = null

      try {
        await authService.logout()
        this.resetAuthState()
        this.status = STATUS.success
        this.bootstrapStatus = 'idle'
        router.push('/login')
      } catch (error) {
        this.status = STATUS.error
        this.error = this._toUserError(error, 'Sikertelen kijelentkezés.')
        throw error
      }
    },
  },
})
