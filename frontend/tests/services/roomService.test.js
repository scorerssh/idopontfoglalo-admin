import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { roomService } from '@features/rooms/services/room.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_ROOM = { id: 1, name: 'Kis terem', capacity: 10 }
const MOCK_ROOMS = [MOCK_ROOM, { id: 2, name: 'Nagy terem', capacity: 50 }]
const MOCK_AGE_PRICE_TIER = { id: 1, roomId: 1, label: 'Felnőtt', price: 5000 }
const MOCK_SPECIAL_RULE = { id: 1, roomId: 1, type: 'Weekend', discount: 10 }

// ─── roomService ─────────────────────────────────────────────────

test('roomService.getAll - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Room/GetAll`, () => HttpResponse.json(MOCK_ROOMS)))
  expect(await roomService.getAll({ page: 1 })).toEqual(MOCK_ROOMS)
})
test('roomService.getAll - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Room/GetAll`, () => HttpResponse.json({}, { status: 500 })))
  await expect(roomService.getAll({ page: 1 })).rejects.toThrow()
})

test('roomService.getAllUser - visszaadja a szerver válaszát', async () => {
  server.use(http.post(`${BASE}/api/Room/GetAllUser`, () => HttpResponse.json(MOCK_ROOMS)))
  expect(await roomService.getAllUser({ page: 1 })).toEqual(MOCK_ROOMS)
})
test('roomService.getAllUser - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Room/GetAllUser`, () => HttpResponse.json({}, { status: 500 })))
  await expect(roomService.getAllUser({ page: 1 })).rejects.toThrow()
})

test('roomService.getById - visszaadja a szerver válaszát', async () => {
  server.use(http.get(`${BASE}/api/Room/1`, () => HttpResponse.json(MOCK_ROOM)))
  expect(await roomService.getById(1)).toEqual(MOCK_ROOM)
})
test('roomService.getById - 404-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/Room/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(roomService.getById(99)).rejects.toThrow()
})

test('roomService.create - visszaadja a létrehozott szobát', async () => {
  server.use(
    http.post(`${BASE}/api/Room/Create`, () => HttpResponse.json(MOCK_ROOM, { status: 201 })),
  )
  expect(await roomService.create({ name: 'Kis terem', capacity: 10 })).toEqual(MOCK_ROOM)
})
test('roomService.create - 500-ra errort dob', async () => {
  server.use(http.post(`${BASE}/api/Room/Create`, () => HttpResponse.json({}, { status: 500 })))
  await expect(roomService.create({})).rejects.toThrow()
})

test('roomService.update - visszaadja a frissített szobát', async () => {
  server.use(http.patch(`${BASE}/api/Room/Update`, () => HttpResponse.json(MOCK_ROOM)))
  expect(await roomService.update({ id: 1, name: 'Kis terem' })).toEqual(MOCK_ROOM)
})
test('roomService.update - 500-ra errort dob', async () => {
  server.use(http.patch(`${BASE}/api/Room/Update`, () => HttpResponse.json({}, { status: 500 })))
  await expect(roomService.update({})).rejects.toThrow()
})

test('roomService.updatePriceTier - visszaadja a frissített árat', async () => {
  const mock = { id: 1, price: 8000 }
  server.use(http.patch(`${BASE}/api/RoomPriceTier/Update`, () => HttpResponse.json(mock)))
  expect(await roomService.updatePriceTier({ id: 1, price: 8000 })).toEqual(mock)
})
test('roomService.updatePriceTier - 500-ra errort dob', async () => {
  server.use(
    http.patch(`${BASE}/api/RoomPriceTier/Update`, () => HttpResponse.json({}, { status: 500 })),
  )
  await expect(roomService.updatePriceTier({})).rejects.toThrow()
})

test('roomService.delete - sikeres törlésnél nem dob errort', async () => {
  server.use(http.delete(`${BASE}/api/Room/1`, () => HttpResponse.json({})))
  await expect(roomService.delete(1)).resolves.not.toThrow()
})
test('roomService.delete - 404-re errort dob', async () => {
  server.use(http.delete(`${BASE}/api/Room/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(roomService.delete(99)).rejects.toThrow()
})

// ─── roomService.agePriceTier ─────────────────────────────────────

test('roomService.agePriceTier.getByRoom - visszaadja a szerver válaszát', async () => {
  server.use(
    http.get(`${BASE}/api/AgePriceTier/Room/1`, () => HttpResponse.json([MOCK_AGE_PRICE_TIER])),
  )
  expect(await roomService.agePriceTier.getByRoom(1)).toEqual([MOCK_AGE_PRICE_TIER])
})
test('roomService.agePriceTier.getByRoom - 500-ra errort dob', async () => {
  server.use(
    http.get(`${BASE}/api/AgePriceTier/Room/1`, () => HttpResponse.json({}, { status: 500 })),
  )
  await expect(roomService.agePriceTier.getByRoom(1)).rejects.toThrow()
})

test('roomService.agePriceTier.getById - visszaadja a szerver válaszát', async () => {
  server.use(http.get(`${BASE}/api/AgePriceTier/1`, () => HttpResponse.json(MOCK_AGE_PRICE_TIER)))
  expect(await roomService.agePriceTier.getById(1)).toEqual(MOCK_AGE_PRICE_TIER)
})
test('roomService.agePriceTier.getById - 404-re errort dob', async () => {
  server.use(http.get(`${BASE}/api/AgePriceTier/99`, () => HttpResponse.json({}, { status: 404 })))
  await expect(roomService.agePriceTier.getById(99)).rejects.toThrow()
})

test('roomService.agePriceTier.create - visszaadja a létrehozott rekordot', async () => {
  server.use(
    http.post(`${BASE}/api/AgePriceTier/Create`, () =>
      HttpResponse.json(MOCK_AGE_PRICE_TIER, { status: 201 }),
    ),
  )
  expect(await roomService.agePriceTier.create({ roomId: 1 })).toEqual(MOCK_AGE_PRICE_TIER)
})
test('roomService.agePriceTier.create - 500-ra errort dob', async () => {
  server.use(
    http.post(`${BASE}/api/AgePriceTier/Create`, () => HttpResponse.json({}, { status: 500 })),
  )
  await expect(roomService.agePriceTier.create({})).rejects.toThrow()
})

test('roomService.agePriceTier.update - visszaadja a frissített rekordot', async () => {
  server.use(
    http.patch(`${BASE}/api/AgePriceTier/Update`, () => HttpResponse.json(MOCK_AGE_PRICE_TIER)),
  )
  expect(await roomService.agePriceTier.update({ id: 1 })).toEqual(MOCK_AGE_PRICE_TIER)
})
test('roomService.agePriceTier.update - 500-ra errort dob', async () => {
  server.use(
    http.patch(`${BASE}/api/AgePriceTier/Update`, () => HttpResponse.json({}, { status: 500 })),
  )
  await expect(roomService.agePriceTier.update({})).rejects.toThrow()
})

test('roomService.agePriceTier.delete - sikeres törlésnél nem dob errort', async () => {
  server.use(http.delete(`${BASE}/api/AgePriceTier/1`, () => HttpResponse.json({})))
  await expect(roomService.agePriceTier.delete(1)).resolves.not.toThrow()
})
test('roomService.agePriceTier.delete - 404-re errort dob', async () => {
  server.use(
    http.delete(`${BASE}/api/AgePriceTier/99`, () => HttpResponse.json({}, { status: 404 })),
  )
  await expect(roomService.agePriceTier.delete(99)).rejects.toThrow()
})

// ─── roomService.specialPricingRule ───────────────────────────────

test('roomService.specialPricingRule.getTypes - visszaadja a típusok listáját', async () => {
  const mock = ['Weekend', 'Holiday']
  server.use(http.get(`${BASE}/api/RoomSpecialPricingRule/Types`, () => HttpResponse.json(mock)))
  expect(await roomService.specialPricingRule.getTypes()).toEqual(mock)
})
test('roomService.specialPricingRule.getTypes - 500-ra errort dob', async () => {
  server.use(
    http.get(`${BASE}/api/RoomSpecialPricingRule/Types`, () =>
      HttpResponse.json({}, { status: 500 }),
    ),
  )
  await expect(roomService.specialPricingRule.getTypes()).rejects.toThrow()
})

test('roomService.specialPricingRule.getByRoom - visszaadja a szerver válaszát', async () => {
  server.use(
    http.get(`${BASE}/api/RoomSpecialPricingRule/Room/1`, () =>
      HttpResponse.json([MOCK_SPECIAL_RULE]),
    ),
  )
  expect(await roomService.specialPricingRule.getByRoom(1)).toEqual([MOCK_SPECIAL_RULE])
})
test('roomService.specialPricingRule.getByRoom - 500-ra errort dob', async () => {
  server.use(
    http.get(`${BASE}/api/RoomSpecialPricingRule/Room/1`, () =>
      HttpResponse.json({}, { status: 500 }),
    ),
  )
  await expect(roomService.specialPricingRule.getByRoom(1)).rejects.toThrow()
})

test('roomService.specialPricingRule.getById - visszaadja a szerver válaszát', async () => {
  server.use(
    http.get(`${BASE}/api/RoomSpecialPricingRule/1`, () => HttpResponse.json(MOCK_SPECIAL_RULE)),
  )
  expect(await roomService.specialPricingRule.getById(1)).toEqual(MOCK_SPECIAL_RULE)
})
test('roomService.specialPricingRule.getById - 404-re errort dob', async () => {
  server.use(
    http.get(`${BASE}/api/RoomSpecialPricingRule/99`, () => HttpResponse.json({}, { status: 404 })),
  )
  await expect(roomService.specialPricingRule.getById(99)).rejects.toThrow()
})

test('roomService.specialPricingRule.create - visszaadja a létrehozott rekordot', async () => {
  server.use(
    http.post(`${BASE}/api/RoomSpecialPricingRule/Create`, () =>
      HttpResponse.json(MOCK_SPECIAL_RULE, { status: 201 }),
    ),
  )
  expect(await roomService.specialPricingRule.create({ roomId: 1 })).toEqual(MOCK_SPECIAL_RULE)
})
test('roomService.specialPricingRule.create - 500-ra errort dob', async () => {
  server.use(
    http.post(`${BASE}/api/RoomSpecialPricingRule/Create`, () =>
      HttpResponse.json({}, { status: 500 }),
    ),
  )
  await expect(roomService.specialPricingRule.create({})).rejects.toThrow()
})

test('roomService.specialPricingRule.update - visszaadja a frissített rekordot', async () => {
  server.use(
    http.patch(`${BASE}/api/RoomSpecialPricingRule/Update`, () =>
      HttpResponse.json(MOCK_SPECIAL_RULE),
    ),
  )
  expect(await roomService.specialPricingRule.update({ id: 1 })).toEqual(MOCK_SPECIAL_RULE)
})
test('roomService.specialPricingRule.update - 500-ra errort dob', async () => {
  server.use(
    http.patch(`${BASE}/api/RoomSpecialPricingRule/Update`, () =>
      HttpResponse.json({}, { status: 500 }),
    ),
  )
  await expect(roomService.specialPricingRule.update({})).rejects.toThrow()
})

test('roomService.specialPricingRule.delete - sikeres törlésnél nem dob errort', async () => {
  server.use(http.delete(`${BASE}/api/RoomSpecialPricingRule/1`, () => HttpResponse.json({})))
  await expect(roomService.specialPricingRule.delete(1)).resolves.not.toThrow()
})
test('roomService.specialPricingRule.delete - 404-re errort dob', async () => {
  server.use(
    http.delete(`${BASE}/api/RoomSpecialPricingRule/99`, () =>
      HttpResponse.json({}, { status: 404 }),
    ),
  )
  await expect(roomService.specialPricingRule.delete(99)).rejects.toThrow()
})
