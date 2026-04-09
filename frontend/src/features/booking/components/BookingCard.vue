<script setup>
import { EllipsisVertical, CalendarDays, Users, Phone, Mail, Clock } from 'lucide-vue-next'

const emits = defineEmits(['openModifyModal'])

const props = defineProps({
    booking: { type: Object, required: true }
})

const initials = computed(() =>
    props.booking.name
        .split(' ')
        .map(n => n[0])
        .join('')
        .toUpperCase()
        .slice(0, 2)
)

const nightCount = computed(() => {
    const start = new Date(props.booking.startTIme)
    const end = new Date(props.booking.endTime)
    return Math.round((end - start) / (1000 * 60 * 60 * 24))
})

const selectedBooking = computed(() => ({
    id: props.booking.id,
    startTIme: props.booking.startTIme,
    endTime: props.booking.endTime,
    pearsonCount: props.booking.pearsonCount,
    name: props.booking.name,
    phoneNumber: props.booking.phoneNumber,
    email: props.booking.email,
    description: props.booking.description,
}))

function openModifyModal() {
    emits('openModifyModal', selectedBooking.value)
}
</script>

<template>
    <div class="bg-white shadow-sm border border-gray-100 rounded-xl flex flex-col p-4">

        <section class="flex justify-between items-start mb-4">
            <div class="flex gap-x-3 items-center min-w-0">
                <div class="bg-blue-100 text-blue-600 p-3 rounded-full shrink-0 font-bold text-sm">
                    {{ initials }}
                </div>
                <div class="flex flex-col min-w-0">
                    <span class="font-bold text-gray-900 leading-tight truncate">{{ booking.name }}</span>
                    <div class="flex items-center gap-1 text-gray-400">
                        <Mail class="h-3 w-3 shrink-0" />
                        <span class="text-xs truncate">{{ booking.email }}</span>
                    </div>
                </div>
            </div>
            <div class="flex flex-row gap-x-1 items-center justify-center">
                <span class="bg-green-100 text-green-700 px-2 py-1 rounded-lg ">
                    Szoba:
                </span>
                <span>
                    {{ booking.room?.bindedApartmanName }}
                </span>

            </div>
        </section>

        <section class="grid grid-cols-2 gap-3 mb-4">
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Érkezés</span>
                <div class="flex items-center gap-1.5 font-bold text-sm text-gray-700">
                    <CalendarDays class="h-3.5 w-3.5 text-gray-400" />
                    <span>{{ booking.startTIme }}</span>
                </div>
            </div>
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Távozás</span>
                <div class="flex items-center gap-1.5 font-bold text-sm text-gray-700">
                    <CalendarDays class="h-3.5 w-3.5 text-gray-400" />
                    <span>{{ booking.endTime }}</span>
                </div>
            </div>
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Vendégek</span>
                <div class="flex items-center gap-1.5 font-bold text-sm text-gray-700">
                    <Users class="h-3.5 w-3.5 text-gray-400" />
                    <span>{{ booking.pearsonCount }} fő</span>
                </div>
            </div>
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Időtartam</span>
                <div class="flex items-center gap-1.5 font-bold text-sm text-gray-700">
                    <Clock class="h-3.5 w-3.5 text-gray-400" />
                    <span>{{ nightCount }} nap</span>
                </div>
            </div>
        </section>

        <section class="flex justify-between items-center pt-3 border-t border-gray-50 mt-auto">
            <div class="flex items-center gap-1.5 text-xs text-gray-500 font-medium">
                <Phone class="h-4 w-4 text-gray-400" />
                <span>{{ booking.phoneNumber }}</span>
            </div>
            <button @click="openModifyModal"
                class="p-2 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors">
                <EllipsisVertical class="h-5 w-5" />
            </button>
        </section>

    </div>
</template>