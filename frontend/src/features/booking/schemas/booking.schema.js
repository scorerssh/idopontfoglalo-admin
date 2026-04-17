import z from 'zod'

export const bookingCreateSchema = z.object({
  name: z.string().min(1, 'A név megadása kötelező'),
  email: z.string().email('Érvénytelen email cím'),
  phone: z.string().min(10, 'A telefonszámnak legalább 10 karakternek kell lennie'),
  startDate: z.string().min(1, 'A dátum megadása kötelező'),
  endDate: z.string().min(1, 'A dátum megadása kötelező'),
  persons: z
    .array(
      z.object({
        age: z.coerce
          .number()
          .min(0, 'Az életkor nem lehet negatív')
          .max(150, 'Az életkor nem lehet több 150-nél'),
      }),
    )
    .min(1, 'Legalább egy vendég szükséges'),
  // personsCount KITÖRÖLVE — nem kell, a persons.min(1) úgyis biztosítja
  description: z.string().max(200, 'A leírás maximum 200 karakter lehet').optional(),
  roomId: z.coerce.string().min(1, 'Kötelező szobát választani a foglalás létrehozásához'),
})
