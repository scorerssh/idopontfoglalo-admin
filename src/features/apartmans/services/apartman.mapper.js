// services/apartman.mapper.js

// ─────────────────────────────────────────────────────────────
// TYPEMAP
// Ez a "szótár" a backend és a frontend között.
// Ha a backend változtat egy mezőnevet vagy típust,
// csak itt kell módosítani — sehol máshol.
// ─────────────────────────────────────────────────────────────
const apartmanTypeMap = [
  { from: '_id', to: 'id', type: 'string', default: null },
  { from: 'cim', to: 'title', type: 'string', default: '' },
  { from: 'leiras', to: 'description', type: 'string', default: '' },
  { from: 'ar_huf', to: 'price', type: 'number', default: 0 },
  { from: 'szobak_szama', to: 'roomCount', type: 'number', default: 0 },
  { from: 'elerheto', to: 'isAvailable', type: 'boolean', default: false },
  { from: 'letrehozva', to: 'createdAt', type: 'date', default: null },
]

// ─────────────────────────────────────────────────────────────
// SZÖVEGJAVÍTÁSOK
// Backend által küldött hibás szövegek javítása.
// Bal oldal: hibás, jobb oldal: helyes.
// ─────────────────────────────────────────────────────────────
const textCorrections = {
  Apartaman: 'Apartman',
  Elerheto: 'Elérhető',
  'Nem elerheto': 'Nem elérhető',
}

// ─────────────────────────────────────────────────────────────
// SEGÉDFÜGGVÉNYEK
// Ezeket nem kell exportálni, csak a mapper használja őket.
// ─────────────────────────────────────────────────────────────

// Először javítjuk a nyers stringet, aztán castoljuk típusra —
// fontos a sorrend! Stringet javítunk, nem numbert vagy Date-et.
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
    return Number.isNaN(n) ? null : n // "abc" helyett null, nem NaN
  }

  if (type === 'boolean') {
    // Kezeli ha a backend "true"/"false" stringet küld szám helyett
    return value === true || value === 'true' || value === 1
  }

  if (type === 'date') {
    const d = new Date(value)
    return isNaN(d.getTime()) ? null : d // érvénytelen dátum helyett null
  }

  return value
}

// ─────────────────────────────────────────────────────────────
// NORMALIZÁLÓK — ezeket használja a service
// ─────────────────────────────────────────────────────────────
export function normalizeApartman(raw) {
  if (!raw) return null

  const result = {}

  for (const mezo of apartmanTypeMap) {
    const nyersErtek = raw[mezo.from] // backend értéke
    const javitottErtek = correctText(nyersErtek) // szövegjavítás először
    const castoltErtek = castValue(javitottErtek, mezo.type) // típusra castolás

    result[mezo.to] = castoltErtek ?? mezo.default // ha null, a default kerül be
  }

  return result
}

export function normalizeApartmanList(rawList) {
  if (!Array.isArray(rawList)) return []
  return rawList.map(normalizeApartman)
}
