// useCalendar.js
// Egy teljesen önálló naptár composable — semmi külső függőség, csak Vue reaktivitás.
// Hétfőtől indul a hét (magyar szokás szerint), kezeli az összes edge case-t.


// Magyar napnevek (hétfőtől kezdve — mert vasárnap NEM az első nap, ez nem Amerika)
export const DAY_NAMES = ['Hétfő', 'Kedd', 'Szerda', 'Csütörtök', 'Péntek', 'Szombat', 'Vasárnap']
export const DAY_NAMES_SHORT = ['H', 'K', 'Sze', 'Cs', 'P', 'Szo', 'V']

// Magyar hónapnevek
export const MONTH_NAMES = [
  'Január',
  'Február',
  'Március',
  'Április',
  'Május',
  'Június',
  'Július',
  'Augusztus',
  'Szeptember',
  'Október',
  'November',
  'December',
]

/**
 * Megmondja, hogy egy adott nap hányadik oszlopba kerüljön a rácsban.
 * JS-ben a getDay() 0 = vasárnap, 1 = hétfő... de nekünk hétfő = 0 kell.
 * Ezért: (jsDay + 6) % 7
 * Pl.: vasárnap (0) → (0 + 6) % 7 = 6 ✓
 *      hétfő   (1) → (1 + 6) % 7 = 0 ✓
 */
function getMondayBasedDayIndex(date) {
  return (date.getDay() + 6) % 7
}

/**
 * Generálja az adott hónap celláit, beleértve az előző/következő hónap
 * "kitöltő" napjait is, hogy a rács mindig teljes heteket mutasson.
 *
 * Visszatér egy tömbbel, ahol minden elem:
 * {
 *   date: Date,           // a nap dátuma
 *   day: number,          // hányadika (1-31)
 *   isCurrentMonth: bool, // az aktuális hónaphoz tartozik-e
 *   isToday: bool,        // mai nap-e
 *   isWeekend: bool,      // hétvége-e (szo/vas)
 *   dayIndex: number,     // 0=hétfő ... 6=vasárnap (a rácsban melyik oszlop)
 * }
 */
function buildCalendarDays(year, month) {
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const firstDayOfMonth = new Date(year, month, 1)
  const lastDayOfMonth = new Date(year, month + 1, 0)

  // Hány "előző hónapos" nap kell a sor elejére?
  const leadingDays = getMondayBasedDayIndex(firstDayOfMonth)

  // Hány "következő hónapos" nap kell a sor végére? (hogy 7-es rácsban záródjunk)
  const trailingDays = (7 - getMondayBasedDayIndex(lastDayOfMonth) - 1) % 7

  const cells = []

  // Előző hónap kitöltő napjai
  for (let i = leadingDays - 1; i >= 0; i--) {
    const date = new Date(year, month, -i)
    cells.push(makeCell(date, false, today))
  }

  // Az aktuális hónap napjai
  for (let d = 1; d <= lastDayOfMonth.getDate(); d++) {
    const date = new Date(year, month, d)
    cells.push(makeCell(date, true, today))
  }

  // Következő hónap kitöltő napjai
  for (let i = 1; i <= trailingDays; i++) {
    const date = new Date(year, month + 1, i)
    cells.push(makeCell(date, false, today))
  }

  return cells
}

function makeCell(date, isCurrentMonth, today) {
  const dayIndex = getMondayBasedDayIndex(date)
  return {
    date,
    day: date.getDate(),
    month: date.getMonth(),
    year: date.getFullYear(),
    isCurrentMonth,
    isToday: date.getTime() === today.getTime(),
    isWeekend: dayIndex >= 5, // 5=Szombat, 6=Vasárnap
    dayIndex,
  }
}

/**
 * A főbb composable, amit a komponensekben használsz.
 *
 * Használat:
 * const { year, month, monthName, calendarDays, prevMonth, nextMonth, goToToday } = useCalendar()
 */
export function useCalendar() {
  const now = new Date()
  const currentYear = ref(now.getFullYear())
  const currentMonth = ref(now.getMonth()) // 0-indexed

  const monthName = computed(() => MONTH_NAMES[currentMonth.value])

  const calendarDays = computed(() => buildCalendarDays(currentYear.value, currentMonth.value))

  // Hetekre bontva (minden belső tömb = 1 sor = 7 nap)
  // Hasznos ha <tr>-ekben akarod renderelni
  const calendarWeeks = computed(() => {
    const days = calendarDays.value
    const weeks = []
    for (let i = 0; i < days.length; i += 7) {
      weeks.push(days.slice(i, i + 7))
    }
    return weeks
  })

  function prevMonth() {
    if (currentMonth.value === 0) {
      currentMonth.value = 11
      currentYear.value--
    } else {
      currentMonth.value--
    }
  }

  function nextMonth() {
    if (currentMonth.value === 11) {
      currentMonth.value = 0
      currentYear.value++
    } else {
      currentMonth.value++
    }
  }

  function goToToday() {
    const now = new Date()
    currentYear.value = now.getFullYear()
    currentMonth.value = now.getMonth()
  }

  return {
    year: currentYear,
    month: currentMonth,
    monthName,
    calendarDays,
    calendarWeeks,
    prevMonth,
    nextMonth,
    goToToday,
    DAY_NAMES,
    DAY_NAMES_SHORT,
    MONTH_NAMES,
  }
}
