import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { authService } from '@features/auth/services/auth.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_USER = { id: 1, name: 'Teszt Elek', email: 'teszt@example.com', role: 'Admin' }

test('authService.login - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Auth/login`, () => HttpResponse.json(MOCK_USER)))
  const res = await authService.login({ email: 'teszt@example.com', password: '1234' })
  expect(res.data).toEqual(MOCK_USER)
})
test('authService.login - 401-re errort dob', async () => {
  server.use(http.post(`${BASE}/api/Auth/login`, () => HttpResponse.json({}, { status: 401 })))
  await expect(authService.login({ email: 'x', password: 'x' })).rejects.toThrow()
})

test('authService.logout - sikeres kilépésnél nem dob errort', async () => {
  server.use(http.post(`${BASE}/api/Auth/logout`, () => HttpResponse.json({})))
  await expect(authService.logout()).resolves.not.toThrow()
})
test('authService.logout - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Auth/logout`, () => HttpResponse.json({}, { status: 500 })))
  await expect(authService.logout()).rejects.toThrow()
})

test('authService.getWhoAmI - visszaadja a bejelentkezett usert', async () => {
  server.use(http.get(`${BASE}/api/Auth/me`, () => HttpResponse.json(MOCK_USER)))
  expect(await authService.getWhoAmI()).toEqual(MOCK_USER)
})
test('authService.getWhoAmI - 401-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/Auth/me`, () => HttpResponse.json({}, { status: 401 })))
  await expect(authService.getWhoAmI()).rejects.toThrow()
})
