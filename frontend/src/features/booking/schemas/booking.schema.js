import z from 'zod'

export const bookingCreateSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  email: z.string().email('Invalid email address'),
  phone: z.string().min(10, 'Phone number must be at least 10 digits'),
  startDate: z.string().min(1, 'Date is required'),
  endDate: z.string().min(1, 'Date is required'),
  guests: z.number().min(1, 'At least one guest is required'),
  description: z.string().max(200, 'Description must be at most 200 characters').optional(),
})
