import { defineStore } from 'pinia'
import { authService } from '@features/auth/services/authService'
import { STATUSES } from '@constants/status'
import router from '@/router'

const AUTH_TTL_MS = 60_000

export const useAuthStore = defineStore('auth', {
  state: () => ({
    authenticated: false,
    user: null,
    role: null,
    status: STATUSES.idle,
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

    isLoading: (state) => state.status === STATUSES.loading,
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

      this.bootstrapStatus = 'checking'

      try {
        if (!authService.hasSessionHint?.()) {
          this.bootstrapStatus = 'done'
          return
        }

        await this.fetchWhoAmI()
        this.bootstrapStatus = 'done'
      } catch (error) {
        this.bootstrapStatus = 'idle'
        throw error
      }
    },

    async fetchWhoAmI({ force = false } = {}) {
      if (!force && this.isFresh) return this.user

      if (this.inFlight) {
        await this.inFlight
        return this.user
      }

      this.status = STATUSES.loading
      this.error = null

      this.inFlight = (async () => {
        try {
          const me = await authService.getWhoAmI()
          const user = me?.user ?? me ?? null

          this.user = user
          this.authenticated = Boolean(user)
          this.role = me?.role ?? null
          this.status = STATUSES.success
          this.lastCheckedAt = Date.now()
        } catch (error) {
          this.status = STATUSES.error
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
      this.status = STATUSES.loading
      this.error = null

      try {
        await authService.login(credentials)
        await this.fetchWhoAmI({ force: true })
      } catch (error) {
        this.status = STATUSES.error
        this.error = this._toUserError(error, 'Sikertelen bejelentkezés.')
        this.resetAuthState()
        throw error
      }
    },

    async logout() {
      this.status = STATUSES.loading
      this.error = null

      try {
        await authService.logout()
        router.push('/login')
        this.resetAuthState()
        this.status = STATUSES.success
        this.bootstrapStatus = 'idle'
      } catch (error) {
        this.status = STATUSES.error
        this.error = this._toUserError(error, 'Sikertelen kijelentkezés.')
        throw error
      }
    },
  },
})
