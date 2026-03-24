// src/utils/errorHelper.js

export function extractError(e, fallbackUserMessage = 'Ismeretlen hiba történt') {
  if (!e) {
    return {
      userMessage: fallbackUserMessage,
      devMessage: 'No error object received',
    }
  }

  const devMessage = e?.stack || e?.message || safeStringify(e)

  // 🥇 1) Ha már van userMessage (HttpError / vagy errorOp már beírta)
  if (typeof e.userMessage === 'string' && e.userMessage.trim()) {
    return {
      userMessage: e.userMessage.trim(),
      devMessage,
    }
  }

  // 🥈 2) HttpError / fetch wrapper shape
  // 🥈 2/b) axios shape
  const status = e?.status ?? e?.response?.status
  const data = e?.data ?? e?.response?.data

  if (status && data) {
    if (typeof data.userMessage === 'string' && data.userMessage.trim()) {
      return { userMessage: data.userMessage.trim(), devMessage }
    }
    if (typeof data.message === 'string' && data.message.trim()) {
      return { userMessage: data.message.trim(), devMessage }
    }
    if (typeof data.error === 'string' && data.error.trim()) {
      return { userMessage: data.error.trim(), devMessage }
    }
    if (typeof data.title === 'string' && data.title.trim()) {
      return { userMessage: data.title.trim(), devMessage }
    }
  }

  // 🥉 3) Általános Error.message, de ne "HTTP Error 400"-at mutassunk usernek
  const msg = String(e?.message ?? '').trim()
  if (msg && !/^HTTP Error\s*\d+/i.test(msg)) {
    return { userMessage: msg, devMessage }
  }

  // 🪦 4) Fallback
  return { userMessage: fallbackUserMessage, devMessage }
}

function safeStringify(v) {
  try {
    return JSON.stringify(v)
  } catch {
    return String(v)
  }
}
