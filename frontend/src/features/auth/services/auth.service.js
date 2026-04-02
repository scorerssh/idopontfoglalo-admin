import { api } from '@api/axios'

const EVER_LOGGED = 'dc.auth.everLogged'

export const authService = {
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
    const res = await api.get('/api/Auth/me')
    return res.data // ← mindig clean objektumot ad vissza
  },
  hasSessionHint() {
    try {
      return localStorage.getItem(EVER_LOGGED) === '1'
    } catch {}
    return false
  },
}
