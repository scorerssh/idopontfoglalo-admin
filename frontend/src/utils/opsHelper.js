// src/utils/opsHelper.js
import { STATUS } from '@/constants/status'
import { extractError } from './errorHelper'

export function defaultOp() {
  return {
    status: STATUS.idle,
    error: null,
    devError: null,
    message: '',
  }
}

export function startOp(op) {
  op.status = STATUS.loading
  op.error = null
  op.devError = null
  op.message = ''
}

export function successOp(op, message = '') {
  op.status = STATUS.success
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

  op.status = STATUS.error
  op.error = userMessage
  op.devError = fallbackDevMessage || devMessage
  op.message = userMessage
}
