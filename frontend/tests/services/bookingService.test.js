import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { bookingService } from '@features/booking/services/booking.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_BOOKING = { id: 1, roomId: 1, userId: 1, date: '2026-05-01' }
const MOCK_BOOKINGS = [MOCK_BOOKING, { id: 2, roomId: 2, userId: 1, date: '2026-05-02' }]

test('bookingService.createBooking - visszaadja a létrehozott foglalást', async () => {
  server.use(http.post(`${BASE}/api/Reservation/Create`, () => HttpResponse.json(MOCK_BOOKING, { status: 201 })))
  expect(await bookingService.createBooking({ roomId: 1 })).toEqual(MOCK_BOOKING)
})
test('bookingService.createBooking - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Reservation/Create`, () => HttpResponse.json({}, { status: 500 })))
  await expect(bookingService.createBooking({})).rejects.toThrow()
})

test('bookingService.getAllAdmin - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Reservation/GetAllAdmin`, () => HttpResponse.json(MOCK_BOOKINGS)))
  expect(await bookingService.getAllAdmin({ page: 1 })).toEqual(MOCK_BOOKINGS)
})
test('bookingService.getAllAdmin - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Reservation/GetAllAdmin`, () => HttpResponse.json({}, { status: 500 })))
  await expect(bookingService.getAllAdmin({})).rejects.toThrow()
})

test('bookingService.getAllUser - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Reservation/GetAllUser`, () => HttpResponse.json(MOCK_BOOKINGS)))
  expect(await bookingService.getAllUser({ page: 1 })).toEqual(MOCK_BOOKINGS)
})
test('bookingService.getAllUser - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Reservation/GetAllUser`, () => HttpResponse.json({}, { status: 500 })))
  await expect(bookingService.getAllUser({})).rejects.toThrow()
})

test('bookingService.getById - visszaadja a szerver válaszát', async () => {
  server.use(http.get(`${BASE}/api/Reservation/1`, () => HttpResponse.json(MOCK_BOOKING)))
  expect(await bookingService.getById(1)).toEqual(MOCK_BOOKING)
})
test('bookingService.getById - 404-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/Reservation/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(bookingService.getById(99)).rejects.toThrow()
})

test('bookingService.updateBoooking - visszaadja a frissített foglalást', async () => {
  server.use(http.patch(`${BASE}/api/Reservation/Update`, () => HttpResponse.json(MOCK_BOOKING)))
  expect(await bookingService.updateBoooking({ id: 1 })).toEqual(MOCK_BOOKING)
})
test('bookingService.updateBoooking - 500-ra errort dob', async () => {
  server.use(http.patch(`${BASE}/api/Reservation/Update`, () => HttpResponse.json({}, { status: 500 })))
  await expect(bookingService.updateBoooking({})).rejects.toThrow()
})

test('bookingService.deleteBooking - sikeres törlésnél nem dob errort', async () => {
  server.use(http.delete(`${BASE}/api/Reservation/1`, () => HttpResponse.json({})))
  await expect(bookingService.deleteBooking(1)).resolves.not.toThrow()
})
test('bookingService.deleteBooking - 404-re errort dob', async () => {
  server.use(http.delete(`${BASE}/api/Reservation/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(bookingService.deleteBooking(99)).rejects.toThrow()
})
