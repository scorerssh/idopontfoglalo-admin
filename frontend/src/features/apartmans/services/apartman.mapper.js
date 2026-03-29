const apartmanTypeMap = [
  { from: '_id', to: 'id', type: 'string', default: null },
  { from: 'cim', to: 'title', type: 'string', default: '' },
  { from: 'leiras', to: 'description', type: 'string', default: '' },
  { from: 'ar_huf', to: 'price', type: 'number', default: 0 },
  { from: 'szobak_szama', to: 'roomCount', type: 'number', default: 0 },
  { from: 'elerheto', to: 'isAvailable', type: 'boolean', default: false },
  { from: 'letrehozva', to: 'createdAt', type: 'date', default: null },
]

const textCorrections = {
  Apartaman: 'Apartman',
  Elerheto: 'Elérhető',
  'Nem elerheto': 'Nem elérhető',
}

function correctText(value) {
  if (typeof value !== 'string') return value

  return Object.entries(textCorrections).reduce(
    (text, [wrong, correct]) => text.replaceAll(wrong, correct),
    value,
  )
}

function castValue(value, type) {
  if (value === null || value === undefined) return null

  if (type === 'string') {
    return String(value)
  }

  if (type === 'number') {
    const n = Number(value)
    return Number.isNaN(n) ? null : n
  }

  if (type === 'boolean') {
    return value === true || value === 'true' || value === 1
  }

  if (type === 'date') {
    const d = new Date(value)
    return isNaN(d.getTime()) ? null : d
  }

  return value
}

export function normalizeApartman(raw) {
  if (!raw) return null

  const result = {}

  for (const mezo of apartmanTypeMap) {
    const nyersErtek = raw[mezo.from]
    const javitottErtek = correctText(nyersErtek)
    const castoltErtek = castValue(javitottErtek, mezo.type)

    result[mezo.to] = castoltErtek ?? mezo.default
  }

  return result
}

export function normalizeApartmanList(rawList) {
  if (!Array.isArray(rawList)) return []
  return rawList.map(normalizeApartman)
}
