import z from 'zod'

export const roomModifySchema = z.object({
  name: z.string().min(1, 'A név megadása kötelező'),
  minCapacity: z.coerce.number().min(1, 'A minimum kapacitás nem lehet kisebb mint 1'),
  maxCapacity: z.coerce
    .number()
    .min(1, 'A maximum kapacitásnak legalább 1-nek kell lennie')
    .max(99, 'A maximum kapacitás nem lehet nagyobb 99-nél'),
  price: z.coerce
    .number()
    .min(1, 'Az ár nem lehet 0')
    .int('Az ár egész szám kell legyen')
    .positive('Az ár pozitív szám kell legyen'),
})
