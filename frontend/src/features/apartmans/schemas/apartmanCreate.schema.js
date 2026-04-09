import z from 'zod'

export const apartmanCreateSchema = z.object({
  name: z
    .string()
    .min(1, 'Az apartman nevének megadása kötelező')
    .max(100, 'Az apartman neve maximum 100 karakter lehet'),
})
