import { defineStore } from 'pinia'
import { authService } from '@features/auth/services/authService'
import { STATUSES } from '@constants/status'
import router from '@/router'

// Ennyi ideig tekintjük "frissnek" a /me választ — nem kérdezzük újra feleslegesen
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

    // 'idle' | 'checking' | 'done'
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
    // ─── Belső segédek ────────────────────────────────────────────────────────

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

    // ─── Bootstrap ────────────────────────────────────────────────────────────
    //
    // A router beforeEach guard hívja app indulásakor (F5, direkt URL).
    // Garantálja hogy a /me egyszer lefut mielőtt bármilyen védett route-ra
    // navigálunk — függetlenül attól, hogy HttpOnly cookie van-e vagy sem.
    //
    // Race condition védelem: ha két guard egyszerre hívja (pl. nested route),
    // a második megvárja az első promise-át ahelyett hogy dupla /me-t indítana.

    async ensureCheckedOnce() {
      // Már lefutott → azonnal vissza
      if (this.bootstrapStatus === 'done') return

      // Éppen fut → megvárjuk a már folyamatban lévő fetchWhoAmI-t
      if (this.bootstrapStatus === 'checking') {
        if (this.inFlight) await this.inFlight
        return
      }

      this.bootstrapStatus = 'checking'

      try {
        await this.fetchWhoAmI()
      } catch {
        // 401/403 = nincs aktív session → normális állapot, nem crash
        // resetAuthState már megtörtént a fetchWhoAmI catch ágában
      } finally {
        // Mindig 'done'-ra állítjuk — ne próbáljon újra minden navigációnál
        this.bootstrapStatus = 'done'
      }
    },

    // ─── /me lekérés ─────────────────────────────────────────────────────────
    //
    // force=false → ha friss adat van (TTL-en belül), nem kérdez újra
    // force=true  → login után mindig friss adatot kér (ne cache-elje a régi role-t)
    //
    // inFlight pattern: ha már van folyamatban lévő /me request,
    // nem indít újat — mindenki ugyanarra a promise-ra vár.

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

          // A backend { userId, email, role } formában adja vissza (nem { user: {...} })
          // A me?.user ?? me fallback megvéd mindkét formátum ellen
          const user = me?.user ?? me ?? null

          this.user = user
          this.authenticated = Boolean(user)
          this.role = me?.role ?? user?.role ?? null // ← mindkét helyről megpróbálja
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

    // ─── Login ────────────────────────────────────────────────────────────────

    async login(credentials) {
      this.status = STATUSES.loading
      this.error = null

      try {
        await authService.login(credentials)

        // force: true → ne használja a cache-t, mindig kérjen friss /me-t
        await this.fetchWhoAmI({ force: true })

        // Bootstrap kész — ne fusson le újra az első navigációnál
        this.bootstrapStatus = 'done'
      } catch (error) {
        this.status = STATUSES.error
        this.error = this._toUserError(error, 'Sikertelen bejelentkezés.')
        this.resetAuthState()
        throw error
      }
    },

    // ─── Logout ───────────────────────────────────────────────────────────────

    async logout() {
      this.status = STATUSES.loading
      this.error = null

      try {
        await authService.logout()
        this.resetAuthState()
        this.status = STATUSES.success
        this.bootstrapStatus = 'idle' // következő látogatáskor újra bootstrap
        router.push('/login')
      } catch (error) {
        this.status = STATUSES.error
        this.error = this._toUserError(error, 'Sikertelen kijelentkezés.')
        throw error
      }
    },
  },
})
