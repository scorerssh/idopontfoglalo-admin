import z from 'zod'

export const bookingModifySchema = z.object({
  name: z.string().min(1, 'A vendég nevének megadása kötelező'),
  email: z.string().email('Érvénytelen email cím'),
  phoneNumber: z.string().min(10, 'A telefonszámnak legalább 10 karakternek kell lennie'),
  startTIme: z.string().min(1, 'Az érkezés dátumának megadása kötelező'),
  endTime: z.string().min(1, 'A távozás dátumának megadása kötelező'),
  pearsonCount: z.coerce.number().min(1, 'A vendégek száma legalább 1 kell legyen'),
  description: z
    .string()
    .max(200, 'A megjegyzés maximum 200 karakter lehet')
    .optional()
    .or(z.literal('')),
})
