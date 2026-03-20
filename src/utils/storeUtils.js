import { useNotifier } from '@/composables/useNotifier'
import { startOp, successOp, errorOp } from './opsHelper'

/**
 * @param {Object} op
 * @param {Function} fn
 * @param {Object} options
 */
export async function runOp(op, fn, options = {}) {
  const {
    silent = false,
    successMessage = '',
    errorMessage = 'Valami nem sikerült',
    notifyOnSuccess = true,
  } = options

  const notifier = useNotifier()

  startOp(op)

  try {
    const result = await fn()

    successOp(op, successMessage)

    if (!silent && notifyOnSuccess && successMessage) {
      notifier.success(successMessage)
    }

    return result
  } catch (e) {
    errorOp(op, e, errorMessage)

    if (!silent) {
      notifier.error(op.message || op.error || errorMessage)
    }

    throw e
  }
}
