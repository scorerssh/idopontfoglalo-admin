import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { apartmanService } from '@features/apartmans/services/apartman.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_APARTMAN = { id: 1, name: 'Panorama Apartman' }
const MOCK_APARTMANS = [MOCK_APARTMAN, { id: 2, name: 'Kilato Apartman' }]
const MOCK_APARTMAN_WITH_ROOMS = { ...MOCK_APARTMAN, rooms: [{ id: 1, name: 'Kis terem' }] }
const MOCK_SMTP_SETTING = {
  id: 1,
  apartmanId: 1,
  host: 'smtp.example.com',
  port: 587,
  userName: 'mailer',
  senderEmail: 'noreply@example.com',
  senderName: 'Panorama Apartman',
  useSsl: true,
  isEnabled: true,
  hasPassword: true,
}

test('apartmanService.getAll - returns server response', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAll`, () => HttpResponse.json(MOCK_APARTMANS)))
  expect(await apartmanService.getAll()).toEqual(MOCK_APARTMANS)
})
test('apartmanService.getAll - throws on 500', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAll`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.getAll()).rejects.toThrow()
})

test('apartmanService.getAllUser - returns server response', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllUser`, () => HttpResponse.json(MOCK_APARTMANS)))
  expect(await apartmanService.getAllUser()).toEqual(MOCK_APARTMANS)
})
test('apartmanService.getAllUser - throws on 500', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllUser`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.getAllUser()).rejects.toThrow()
})

test('apartmanService.getAllWithRooms - returns apartmans with rooms', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllWithRooms`, () => HttpResponse.json([MOCK_APARTMAN_WITH_ROOMS])))
  expect(await apartmanService.getAllWithRooms()).toEqual([MOCK_APARTMAN_WITH_ROOMS])
})
test('apartmanService.getAllWithRooms - throws on 500', async () => {
  server.use(http.post(`${BASE}/api/Apartman/GetAllWithRooms`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.getAllWithRooms()).rejects.toThrow()
})

test('apartmanService.getById - returns server response', async () => {
  server.use(http.get(`${BASE}/api/Apartman/1`, () => HttpResponse.json(MOCK_APARTMAN)))
  expect(await apartmanService.getById(1)).toEqual(MOCK_APARTMAN)
})
test('apartmanService.getById - throws on 404', async () => {
  server.use(http.get(`${BASE}/api/Apartman/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(apartmanService.getById(99)).rejects.toThrow()
})

test('apartmanService.getWithRooms - returns apartman with rooms', async () => {
  server.use(http.get(`${BASE}/api/Apartman/WithRooms/1`, () => HttpResponse.json(MOCK_APARTMAN_WITH_ROOMS)))
  expect(await apartmanService.getWithRooms(1)).toEqual(MOCK_APARTMAN_WITH_ROOMS)
})
test('apartmanService.getWithRooms - throws on 404', async () => {
  server.use(http.get(`${BASE}/api/Apartman/WithRooms/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(apartmanService.getWithRooms(99)).rejects.toThrow()
})

test('apartmanService.create - returns created apartman', async () => {
  server.use(http.post(`${BASE}/api/Apartman/Create`, () => HttpResponse.json(MOCK_APARTMAN, { status: 201 })))
  expect(await apartmanService.create({ name: 'Panorama Apartman' })).toEqual(MOCK_APARTMAN)
})
test('apartmanService.create - throws on 500', async () => {
  server.use(http.post(`${BASE}/api/Apartman/Create`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.create({})).rejects.toThrow()
})

test('apartmanService.update - returns updated apartman', async () => {
  server.use(http.patch(`${BASE}/api/Apartman/Update`, () => HttpResponse.json(MOCK_APARTMAN)))
  expect(await apartmanService.update({ id: 1, name: 'Panorama Apartman' })).toEqual(MOCK_APARTMAN)
})
test('apartmanService.update - throws on 500', async () => {
  server.use(http.patch(`${BASE}/api/Apartman/Update`, () => HttpResponse.json({}, { status: 500 })))
  await expect(apartmanService.update({})).rejects.toThrow()
})

test('apartmanService.getSmtpSetting - returns SMTP setting', async () => {
  server.use(http.get(`${BASE}/api/ApartmanSmtpSetting/1`, () => HttpResponse.json(MOCK_SMTP_SETTING)))
  expect(await apartmanService.getSmtpSetting(1)).toEqual(MOCK_SMTP_SETTING)
})
test('apartmanService.upsertSmtpSetting - returns saved SMTP setting', async () => {
  server.use(http.put(`${BASE}/api/ApartmanSmtpSetting`, () => HttpResponse.json(MOCK_SMTP_SETTING)))
  expect(await apartmanService.upsertSmtpSetting(MOCK_SMTP_SETTING)).toEqual(MOCK_SMTP_SETTING)
})
test('apartmanService.deleteSmtpSetting - does not throw on success', async () => {
  server.use(http.delete(`${BASE}/api/ApartmanSmtpSetting/1`, () => HttpResponse.json({})))
  await expect(apartmanService.deleteSmtpSetting(1)).resolves.not.toThrow()
})

test('apartmanService.delete - does not throw on success', async () => {
  server.use(http.delete(`${BASE}/api/Apartman/1`, () => HttpResponse.json({})))
  await expect(apartmanService.delete(1)).resolves.not.toThrow()
})
test('apartmanService.delete - throws on 404', async () => {
  server.use(http.delete(`${BASE}/api/Apartman/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(apartmanService.delete(99)).rejects.toThrow()
})
