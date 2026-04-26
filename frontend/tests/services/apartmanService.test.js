import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { apartmanService } from '@features/apartmans/services/apartman.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_APARTMAN = { id: 1, name: 'Panoráma Apartman' }
const MOCK_APARTMANS = [MOCK_APARTMAN, { id: 2, name: 'Kilátó Apartman' }]
const MOCK_APARTMAN_WITH_ROOMS = { ...MOCK_APARTMAN, rooms: [{ id: 1, name: 'Kis terem' }] }

test('apartmanService.getAll - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAll`, () => HttpResponse.json(MOCK_APARTMANS)))
  expect(await apartmanService.getAll()).toEqual(MOCK_APARTMANS)
})
test('apartmanService.getAll - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAll`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.getAll()).rejects.toThrow()
})

test('apartmanService.getAllUser - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllUser`, () => HttpResponse.json(MOCK_APARTMANS)))
  expect(await apartmanService.getAllUser()).toEqual(MOCK_APARTMANS)
})
test('apartmanService.getAllUser - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllUser`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.getAllUser()).rejects.toThrow()
})

test('apartmanService.getAllWithRooms - visszaadja a szobákkal együtt', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllWithRooms`, () => HttpResponse.json([MOCK_APARTMAN_WITH_ROOMS])))
  expect(await apartmanService.getAllWithRooms()).toEqual([MOCK_APARTMAN_WITH_ROOMS])
})
test('apartmanService.getAllWithRooms - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllWithRooms`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.getAllWithRooms()).rejects.toThrow()
})

test('apartmanService.getById - visszaadja a szerver válaszát', async () => {
  server.use(http.get(`${BASE}/api/Apartman/1`, () => HttpResponse.json(MOCK_APARTMAN)))
  expect(await apartmanService.getById(1)).toEqual(MOCK_APARTMAN)
})
test('apartmanService.getById - 404-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/Apartman/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(apartmanService.getById(99)).rejects.toThrow()
})

test('apartmanService.getWithRooms - visszaadja az apartmant szobákkal', async () => {
  server.use(http.get(`${BASE}/api/Apartman/WithRooms/1`, () => HttpResponse.json(MOCK_APARTMAN_WITH_ROOMS)))
  expect(await apartmanService.getWithRooms(1)).toEqual(MOCK_APARTMAN_WITH_ROOMS)
})
test('apartmanService.getWithRooms - 404-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/Apartman/WithRooms/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(apartmanService.getWithRooms(99)).rejects.toThrow()
})

test('apartmanService.create - visszaadja a létrehozott apartmant', async () => {
  server.use(http.post(`${BASE}/api/Apartman/Create`, () => HttpResponse.json(MOCK_APARTMAN, { status: 201 })))
  expect(await apartmanService.create({ name: 'Panoráma Apartman' })).toEqual(MOCK_APARTMAN)
})
test('apartmanService.create - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Apartman/Create`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.create({})).rejects.toThrow()
})

test('apartmanService.update - visszaadja a frissített apartmant', async () => {
  server.use(http.patch(`${BASE}/api/Apartman/Update`, () => HttpResponse.json(MOCK_APARTMAN)))
  expect(await apartmanService.update({ id: 1, name: 'Panoráma Apartman' })).toEqual(MOCK_APARTMAN)
})
test('apartmanService.update - 500-ra errort dob', async () => {
  server.use(http.patch(`${BASE}/api/Apartman/Update`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.update({})).rejects.toThrow()
})

test('apartmanService.delete - sikeres törlésnél nem dob errort', async () => {
  server.use(http.delete(`${BASE}/api/Apartman/1`, () => HttpResponse.json({})))
  await expect(apartmanService.delete(1)).resolves.not.toThrow()
})
test('apartmanService.delete - 404-re errort dob', async () => {
  server.use(http.delete(`${BASE}/api/Apartman/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(apartmanService.delete(99)).rejects.toThrow()
})
