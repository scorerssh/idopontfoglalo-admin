<script setup>
import { ChevronLeft, ChevronRight, SlidersHorizontal } from 'lucide-vue-next'
import MainTitle from '@/shared/components/MainTitle.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { useCalendar } from '@/features/calendar/composables/useCalendar.js'
import { useBookingStore } from '@/features/booking/stores/booking.store'
import BookingFiltersBar from '@/features/booking/components/BookingFiltersBar.vue'
import { useRole } from '@/composables/useRole'

const bookingStore = useBookingStore()
const { isAdmin } = useRole()
const showFilters = ref(false)

const {
    year,
    month,
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

const bookingsForCell = computed(() => {
    const bookings = bookingStore.bookings ?? []
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

// A script setup-on belül bárhová
function formatShortDate(dateStr) {
    const d = new Date(dateStr);
    return `${String(d.getMonth() + 1).padStart(2, '0')}.${String(d.getDate()).padStart(2, '0')}`;
}
</script>

<template>
    <div class="w-full h-full ">
        <header class="w-full flex flex-row justify-between items-center relative mb-6">
            <div class="title w-1/3">
                <MainTitle title="Naptár" bar-color="bg-system-orange" />
            </div>
            <div class="flex w-1/3 items-center justify-center gap-x-3">
                <DefaultButton @click="prevMonth" :icon="ChevronLeft"
                    :buttonClass="'bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
                <h2 class="text-lg font-semibold min-w-[120px] text-center">
                    {{ year }}. {{ monthName }}
                </h2>
                <DefaultButton @click="nextMonth" :icon="ChevronRight"
                    :buttonClass="'bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
            </div>
            <div class="w-1/3 flex flex-row justify-end gap-x-2">
                <DefaultButton @click="openFilters" :icon="SlidersHorizontal"
                    :button-class="`${showFilters ? 'bg-gray-200' : 'bg-white hover:bg-gray-200'} ml-2 text-black shadow rounded-lg transition duration-100`" />
            </div>
            <Transition name="fade-in">
                <BookingFiltersBar v-if="showFilters" :show="showFilters" @close="closeFilters" />
            </Transition>
        </header>

        <div class="w-full">

            <div class="grid grid-cols-7 mb-1">
                <div v-for="dayName in DAY_NAMES" :key="dayName"
                    class="text-xs font-medium text-black/40 text-center py-2 uppercase tracking-wide">
                    {{ dayName }}
                </div>
            </div>

            <div class="grid grid-cols-7 gap-px bg-black/10 border border-black/10 rounded-lg overflow-hidden">
                <div v-for="cell in calendarDays" :key="`${cell.year}-${cell.month}-${cell.day}`"
                    class="bg-white min-h-[90px] p-2 flex flex-col relative" :class="{
                        'bg-white': cell.isCurrentMonth && !cell.isWeekend,
                        'bg-gray-50': cell.isCurrentMonth && cell.isWeekend,
                        'bg-gray-100/60': !cell.isCurrentMonth,
                        'ring-2 ring-inset ring-system-orange': cell.isToday,
                    }">
                    <!-- Nap száma -->
                    <span class="self-end text-sm font-medium leading-none z-10" :class="{
                        'text-black': cell.isCurrentMonth && !cell.isToday,
                        'text-black/30': !cell.isCurrentMonth,
                        'bg-system-orange text-white rounded-full w-6 h-6 flex items-center justify-center text-xs': cell.isToday,
                    }">
                        {{ cell.day }}
                    </span>

                    <!-- Foglalás blokkok - csak a blokk kezdő celláján rendereljük -->
                    <div class="mt-1 flex flex-col gap-y-0.5">
                        <div v-for="entry in (bookingsForCell.get(getCellKey(cell)) ?? [])" :key="entry.bookingIndex"
                            class="absolute left-0.5 h-7 flex items-center px-2 rounded-md cursor-pointer select-none overflow-hidden transition-all duration-200 hover:z-20 hover:shadow-md"
                            :class="[entry.color.bg, entry.color.text, entry.color.hover]" :style="{
                                top: '32px',
                                width: `calc(${entry.spanDays} * 100% + ${entry.spanDays - 1}px - 4px)`,
                                zIndex: 10,
                            }">
                            <div class="flex items-center w-full gap-x-1.5 overflow-hidden text-[10px]">
                                <span class="font-bold truncate shrink-0 max-w-[50%]">
                                    {{ entry.booking.name }}
                                </span>

                                <span class="opacity-60">•</span>

                                <span class="font-medium whitespace-nowrap opacity-90">
                                    {{ formatShortDate(entry.booking.startTIme) }} - {{
                                        formatShortDate(entry.booking.endTime) }}
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