import { api } from '@api/axios'

const EVER_LOGGED = 'dc.auth.everLogged'

export const authService = {
  // 👈 const objektum, nem function
  async login(credentials) {
    const res = await api.post('/api/Auth/login', credentials)
    try {
      localStorage.setItem(EVER_LOGGED, '1')
    } catch {}
    return res
  },
  async logout() {
    const res = await api.post('/api/Auth/logout')
    try {
      localStorage.removeItem(EVER_LOGGED)
    } catch {}
    return res
  },
  async getWhoAmI() {
    return api.get('/api/Auth/me')
  },
  hasSessionHint() {
    try {
      return localStorage.getItem(EVER_LOGGED) === '1'
    } catch {}
    return false
  },
}
