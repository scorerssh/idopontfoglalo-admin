import z from 'zod'

export const userModifySchema = z.object({
  userName: z.string().min(3, 'A felhasználónévnek legalább 3 karakter kell lennie'),
  userEmail: z.string().email('Érvénytelen email cím'),
  role: z.enum(['Admin', 'User'], { errorMap: () => ({ message: 'Érvénytelen szerepkör' }) }),
})
