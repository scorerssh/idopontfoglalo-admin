<script setup>
import { ChevronLeft, ChevronRight, SlidersHorizontal } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { useCalendar } from '@/features/calendar/composables/useCalendar.js'
import { useBookingStore } from '@/features/booking/stores/booking.store'
import BookingFiltersBar from '@/features/booking/components/BookingFiltersBar.vue'
import { useRole } from '@/composables/useRole'
import { useRouter } from 'vue-router'

const bookingStore = useBookingStore()
const { isAdmin } = useRole()
const router = useRouter()
const showFilters = ref(false)

const {
    year,
    monthName,
    calendarDays,
    prevMonth,
    nextMonth,
    DAY_NAMES,
} = useCalendar()

function openFilters() { showFilters.value = true }
function closeFilters() { showFilters.value = false }

onMounted(() => {
    if (isAdmin.value) {
        bookingStore.getAllAdmin()
    } else {
        bookingStore.getAllUser()
    }
})

const BOOKING_COLORS = [
    { bg: 'bg-blue-500', text: 'text-white', hover: 'hover:bg-blue-600' },
    { bg: 'bg-amber-500', text: 'text-white', hover: 'hover:bg-amber-600' },
    { bg: 'bg-purple-500', text: 'text-white', hover: 'hover:bg-purple-600' },
    { bg: 'bg-emerald-500', text: 'text-white', hover: 'hover:bg-emerald-600' },
    { bg: 'bg-rose-500', text: 'text-white', hover: 'hover:bg-rose-600' },
]

function toDateOnly(dateStr) {
    return new Date(dateStr + 'T00:00:00')
}

function navigateToBooking(booking) {
    router.push({ name: 'bookings', query: { openBooking: booking.id } })
}

const bookingsForCell = computed(() => {
    const bookings = Array.isArray(bookingStore.bookings?.reservations)
        ? bookingStore.bookings.reservations
        : []
    const rows = []
    for (let i = 0; i < calendarDays.value.length; i += 7) {
        rows.push(calendarDays.value.slice(i, i + 7))
    }

    const result = new Map()

    bookings.forEach((booking, bookingIndex) => {
        const start = toDateOnly(booking.startTIme)
        const end = toDateOnly(booking.endTime)
        const color = BOOKING_COLORS[bookingIndex % BOOKING_COLORS.length]

        rows.forEach(row => {
            let blockStartIndex = -1
            let blockEndIndex = -1

            row.forEach((cell, cellIndex) => {
                const cellDate = toDateOnly(`${cell.year}-${String(cell.month).padStart(2, '0')}-${String(cell.day).padStart(2, '0')}`)

                if (cellDate >= start && cellDate <= end) {
                    if (blockStartIndex === -1) blockStartIndex = cellIndex
                    blockEndIndex = cellIndex
                }
            })

            if (blockStartIndex === -1) return
            const spanDays = blockEndIndex - blockStartIndex + 1
            const startCell = row[blockStartIndex]
            const key = `${startCell.year}-${startCell.month}-${startCell.day}`

            if (!result.has(key)) result.set(key, [])
            result.get(key).push({
                booking,
                color,
                spanDays,
                bookingIndex,
            })
        })
    })

    return result
})

function getCellKey(cell) {
    return `${cell.year}-${cell.month}-${cell.day}`
}

function formatShortDate(dateStr) {
    const d = new Date(dateStr);
    return `${String(d.getMonth() + 1).padStart(2, '0')}.${String(d.getDate()).padStart(2, '0')}`;
}
</script>

<template>
    <div class="w-full min-h-full">
        <!-- Header: mobilon egymás alatt, tablettől sorban -->
        <header class="w-full flex flex-col sm:flex-row sm:items-center relative mb-4 sm:mb-6 gap-3">
            <div class="shrink-0">
                <MainTitle title="Naptár" bar-color="bg-system-orange" />
            </div>

            <!-- Hónap navigáció — mobilon középre, tablettől szintén -->
            <div class="flex flex-1 items-center justify-center gap-x-3">
                <DefaultButton @click="prevMonth" :icon="ChevronLeft"
                    :buttonClass="'bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
                <h2 class="text-base sm:text-lg font-semibold min-w-[110px] sm:min-w-[140px] text-center">
                    {{ year }}. {{ monthName }}
                </h2>
                <DefaultButton @click="nextMonth" :icon="ChevronRight"
                    :buttonClass="'bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
            </div>

            <!-- Filter sáv + eredmény — mobilon jobbra igazítva -->
            <div class="flex flex-row items-center justify-end gap-x-2 shrink-0">
                <span v-if="bookingStore.bookings.reservations?.length > 0"
                    class="hidden sm:flex items-center gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow ring-1 bg-green-100 ring-green-300 text-black font-medium text-sm whitespace-nowrap">
                    <span>Találatok:</span> {{ bookingStore.bookings.reservations.length }} foglalás
                </span>
                <span v-if="bookingStore.bookings.reservations?.length > 0"
                    class="flex sm:hidden items-center gap-x-1 px-2 py-1 rounded-lg shadow ring-1 bg-green-100 ring-green-300 text-black font-medium text-xs">
                    {{ bookingStore.bookings.reservations.length }}
                </span>
                <span v-if="!bookingStore.bookings.reservations?.length"
                    class="hidden sm:flex items-center gap-x-2 p-2 rounded-lg shadow ring-1 ring-gray-300 bg-gray-100 text-black font-medium text-sm">
                    Nincsenek találatok
                </span>
                <DefaultButton @click="openFilters" :icon="SlidersHorizontal"
                    :button-class="`${showFilters ? 'bg-gray-200' : 'bg-white hover:bg-gray-200'} text-black shadow rounded-lg transition duration-100`" />
            </div>

            <Transition name="fade-in">
                <BookingFiltersBar v-if="showFilters" :show="showFilters" @close="closeFilters" :is-open="showFilters" />
            </Transition>
        </header>

        <div class="w-full">
            <!-- Napok fejléce -->
            <div class="grid grid-cols-7 mb-1">
                <div v-for="dayName in DAY_NAMES" :key="dayName"
                    class="text-[10px] sm:text-xs font-medium text-black/40 text-center py-1.5 sm:py-2 uppercase tracking-wide truncate px-0.5">
                    <!-- Mobilon csak 1–2 betű, sm-től teljes -->
                    <span class="sm:hidden">{{ dayName.slice(0, 2) }}</span>
                    <span class="hidden sm:inline">{{ dayName }}</span>
                </div>
            </div>

            <!-- Naptár rács -->
            <div class="grid grid-cols-7 gap-px bg-black/10 border border-black/10 rounded-lg overflow-hidden">
                <div v-for="cell in calendarDays" :key="`${cell.year}-${cell.month}-${cell.day}`"
                    class="bg-white min-h-[50px] sm:min-h-[90px] p-1 sm:p-2 flex flex-col relative"
                    :class="{
                        'bg-white':        cell.isCurrentMonth && !cell.isWeekend,
                        'bg-gray-50':      cell.isCurrentMonth && cell.isWeekend,
                        'bg-gray-100/60':  !cell.isCurrentMonth,
                        'ring-2 ring-inset ring-system-orange': cell.isToday,
                    }">
                    <!-- Nap száma -->
                    <span class="self-end text-[10px] sm:text-sm font-medium leading-none z-10" :class="{
                        'text-black':    cell.isCurrentMonth && !cell.isToday,
                        'text-black/30': !cell.isCurrentMonth,
                        'bg-system-orange text-white rounded-full w-5 h-5 sm:w-6 sm:h-6 flex items-center justify-center text-[9px] sm:text-xs': cell.isToday,
                    }">
                        {{ cell.day }}
                    </span>

                    <!-- Foglalás blokkok -->
                    <div class="mt-0.5 sm:mt-1">
                        <div v-for="entry in (bookingsForCell.get(getCellKey(cell)) ?? [])" :key="entry.bookingIndex"
                            class="absolute left-0.5 flex items-center rounded-sm sm:rounded-md cursor-pointer select-none overflow-hidden transition-all duration-200 hover:z-20 hover:shadow-md"
                            :class="[entry.color.bg, entry.color.text, entry.color.hover]"
                            :style="{
                                top: '22px',
                                height: '14px',
                                width: `calc(${entry.spanDays} * 100% + ${entry.spanDays - 1}px - 4px)`,
                                zIndex: 10,
                            }"
                            @click="navigateToBooking(entry.booking)">
                        </div>
                        <!-- Teljes blokk szöveggel sm+-on -->
                        <div v-for="entry in (bookingsForCell.get(getCellKey(cell)) ?? [])" :key="`label-${entry.bookingIndex}`"
                            class="absolute left-0.5 h-7 hidden sm:flex items-center px-2 rounded-md cursor-pointer select-none overflow-hidden transition-all duration-200 hover:z-20 hover:shadow-md"
                            :class="[entry.color.bg, entry.color.text, entry.color.hover]"
                            :style="{
                                top: '32px',
                                width: `calc(${entry.spanDays} * 100% + ${entry.spanDays - 1}px - 4px)`,
                                zIndex: 10,
                            }"
                            @click="navigateToBooking(entry.booking)">
                            <div class="flex items-center w-full gap-x-1.5 overflow-hidden text-[10px]">
                                <span class="font-bold truncate shrink-0 max-w-[35%]">
                                    {{ entry.booking.name }}
                                </span>
                                <span class="opacity-60">•</span>
                                <span class="font-medium truncate shrink-0 max-w-[30%] opacity-90">
                                    {{ entry.booking.room?.name }}
                                </span>
                                <span class="opacity-60">•</span>
                                <span class="font-medium whitespace-nowrap opacity-90">
                                    {{ formatShortDate(entry.booking.startTIme) }} – {{ formatShortDate(entry.booking.endTime) }}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.fade-in-enter-active,
.fade-in-leave-active {
    transition: all 0.2s ease;
}

.fade-in-enter-from,
.fade-in-leave-to {
    transform: translateY(-15px);
    opacity: 0;
}

.fade-in-enter-to,
.fade-in-leave-from {
    transform: translateY(0px);
    opacity: 1;
}
</style>