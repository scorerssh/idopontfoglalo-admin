// src/utils/opsHelper.js
import { STATUSES } from '@/constants/status'
import { extractError } from './errorHelper'

export function defaultOp() {
  return {
    status: STATUSES.idle,
    error: null,
    devError: null,
    message: '',
  }
}

export function startOp(op) {
  op.status = STATUSES.loading
  op.error = null
  op.devError = null
  op.message = ''
}

export function successOp(op, message = '') {
  op.status = STATUSES.success
  op.error = null
  op.devError = null
  op.message = message
}

export function errorOp(op, e, fallbackUserMessage, fallbackDevMessage = '') {
  const { userMessage, devMessage } = extractError(e, fallbackUserMessage)

  if (e && (!e.userMessage || typeof e.userMessage !== 'string' || !e.userMessage.trim())) {
    try {
      e.userMessage = userMessage
    } catch {}
  }

  op.status = STATUSES.error
  op.error = userMessage
  op.devError = fallbackDevMessage || devMessage
  op.message = userMessage
}
