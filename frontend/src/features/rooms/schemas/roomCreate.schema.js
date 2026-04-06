import z from 'zod'

export const roomCreateSchema = z.object({
  name: z.string().min(1, 'A név megadása kötelező'),
  minCapacity: z.coerce.number().min(1, 'A minimum kapacitás nem lehet kisebb mint 1'),
  maxCapacity: z.coerce
    .number()
    .min(1, 'A maximum kapacitásnak legalább 1-nek kell lennie')
    .max(99, 'A maximum kapacitás nem lehet nagyobb 99-nél'),
  price: z.coerce
    .number()
    .min(1, 'Az ár nem lehet 0')
    .int('Az ár nem lehet 0')
    .positive('Az ár nem lehet 0'),
  apartmanId: z.coerce
    .number('A szoba létrehozásához kötelező apartmant választani')
    .int('A szoba létrehozásához kötelező apartmant választani')
    .positive('A szoba létrehozásához kötelező apartmant választani'),
})
