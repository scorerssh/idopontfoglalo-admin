import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { userService } from '@features/users/services/user.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_USER = { id: 1, name: 'Teszt Elek', email: 'teszt@example.com' }
const MOCK_USERS = [MOCK_USER, { id: 2, name: 'Másik Elek', email: 'masik@example.com' }]

test('userService.getAll - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/User/GetAll`, () => HttpResponse.json(MOCK_USERS)))
  expect(await userService.getAll()).toEqual(MOCK_USERS)
})
test('userService.getAll - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/User/GetAll`, () => HttpResponse.json({}, { status: 500 })))
  await expect(userService.getAll()).rejects.toThrow()
})

test('userService.getById - visszaadja a szerver válaszát', async () => {
  server.use(http.get(`${BASE}/api/User/1`, () => HttpResponse.json(MOCK_USER)))
  expect(await userService.getById(1)).toEqual(MOCK_USER)
})
test('userService.getById - 404-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/User/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(userService.getById(99)).rejects.toThrow()
})

test('userService.create - visszaadja a létrehozott felhasználót', async () => {
  server.use(http.post(`${BASE}/api/User/Create`, () => HttpResponse.json(MOCK_USER, { status: 201 })))
  expect(await userService.create({ name: 'Teszt Elek' })).toEqual(MOCK_USER)
})
test('userService.create - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/User/Create`, () => HttpResponse.json({}, { status: 500 })))
  await expect(userService.create({})).rejects.toThrow()
})

test('userService.update - visszaadja a frissített felhasználót', async () => {
  server.use(http.patch(`${BASE}/api/User/Update`, () => HttpResponse.json(MOCK_USER)))
  expect(await userService.update({ id: 1, name: 'Teszt Elek' })).toEqual(MOCK_USER)
})
test('userService.update - 500-ra errort dob', async () => {
  server.use(http.patch(`${BASE}/api/User/Update`, () => HttpResponse.json({}, { status: 500 })))
  await expect(userService.update({})).rejects.toThrow()
})

test('userService.delete - sikeres törlésnél nem dob errort', async () => {
  server.use(http.delete(`${BASE}/api/User/1`, () => HttpResponse.json({})))
  await expect(userService.delete(1)).resolves.not.toThrow()
})
test('userService.delete - 404-re errort dob', async () => {
  server.use(http.delete(`${BASE}/api/User/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(userService.delete(99)).rejects.toThrow()
})
