<script setup>
import MainTitle from '@/shared/components/MainTitle.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import { useCalendar } from '@/features/calendar/composables/useCalendar.js'

const filterInputs = [
    { name: 'apartmans', label: 'Apartmanok', labelClass: 'text-sm text-black/60' },
    { name: 'rooms', label: 'Szobák', labelClass: 'text-sm text-black/60' },
]

const {
    year,
    month,
    monthName,
    calendarDays,
    prevMonth,
    nextMonth,
    goToToday,
    DAY_NAMES,
} = useCalendar()
</script>

<template>
    <div class="w-full h-full overflow-y-scroll">

        <header class="w-full flex flex-row justify-between items-center mb-6">
            <div class="title">
                <MainTitle title="Naptár" bar-color="bg-system-orange" />
            </div>
            <div class="inputs flex flex-row gap-x-2">
                <DefaultInput v-for="input in filterInputs" :key="input.name" :label-text="input.label"
                    :label-class="input.labelClass" />
            </div>
        </header>

        <div class="flex items-center justify-between mb-4">
            <div class="flex items-center gap-x-3">
                <button class="p-2 rounded hover:bg-black/5 transition-colors" @click="prevMonth"
                    aria-label="Előző hónap">
                    &#8249;
                </button>

                <h2 class="text-lg font-semibold min-w-[180px] text-center">
                    {{ year }}. {{ monthName }}
                </h2>

                <button class="p-2 rounded hover:bg-black/5 transition-colors" @click="nextMonth"
                    aria-label="Következő hónap">
                    &#8250;
                </button>
            </div>

            <DefaultButton @click="goToToday">Ma</DefaultButton>
        </div>

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
                    <span class="self-end text-sm font-medium leading-none" :class="{
                        'text-black': cell.isCurrentMonth && !cell.isToday,
                        'text-black/30': !cell.isCurrentMonth,
                        'bg-system-orange text-white rounded-full w-6 h-6 flex items-center justify-center text-xs': cell.isToday,
                    }">
                        {{ cell.day }}
                    </span>
                </div>
            </div>

        </div>
    </div>
</template>

<style></style>