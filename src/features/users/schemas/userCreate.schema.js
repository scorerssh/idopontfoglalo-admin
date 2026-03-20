import { z } from 'zod'

const userCreateSchema = z.object({
  username: z.string().min(3, 'Minimum 3 karakter'),
  email: z.string().email('Érvényes email cím szükséges'),
  password: z.string().min(6, 'Minimum 6 karakter'),
  role: z.enum(['User', 'Admin'], {
    errorMap: () => ({ message: 'Érvényes szerepkör szükséges' }),
  }),
})

export default userCreateSchema
