import { http, HttpResponse } from 'msw'
import { server } from '../setup'
import { dashboardService } from '@features/dashboard/services/dashboard.service.js'

const BASE = import.meta.env.VITE_API_URL

const MOCK_DASHBOARD = { totalBookings: 42, totalRevenue: 150000 }

test('dashboardService.getDashboard - visszaadja a szerver válaszát', async () => {
  server.use(http.get(`${BASE}/api/Dashboard/Get`, () => HttpResponse.json(MOCK_DASHBOARD)))
  expect(await dashboardService.getDashboard()).toEqual(MOCK_DASHBOARD)
})
test('dashboardService.getDashboard - 500-ra errort dob', async () => {
  server.use(http.get(`${BASE}/api/Dashboard/Get`, () => HttpResponse.json({}, { status: 500 })))
  await expect(dashboardService.getDashboard()).rejects.toThrow()
})
