import z from 'zod'

export const roomCreateSchema = z.object({
  name: z.string().min(1, 'A név megadása kötelező'),
  minCapacity: z.number().min(1, 'A minimum kapacitás nem lehet kisebb mint 1'),
  maxCapacity: z
    .number()
    .min(1, 'A maximum kapacitásnak legalább 1-nek kell lennie')
    .max(99, 'A maximum kapacitás nem lehet nagyobb 99-nél'),
  apartmanId: z.number().int().positive(),
})
