<script setup>
import { EllipsisVertical, BedDouble, Users, Banknote, Coins, UsersRound } from 'lucide-vue-next'

const emits = defineEmits([
    'openModifyModal',
    'openAgePriceTierModal',
    'openPriceTiersModal',
    'openSpecialPricingRulesModal',
])

const props = defineProps({
    room: {
        type: Object,
        required: true
    }
})

const statusStyles = computed(() => {
    return props.room.isAvailable
        ? { label: 'Elérhető', bg: 'bg-green-100', text: 'text-green-700', dot: 'bg-green-500' }
        : { label: 'Foglalt', bg: 'bg-red-100', text: 'text-red-700', dot: 'bg-red-500' }
})

function handleModify() {
    emits('openModifyModal', props.room)
}

function handleAgePriceTier() {
    emits('openAgePriceTierModal', props.room)
}

function handlePriceTiers() {
    emits('openPriceTiersModal', props.room)
}

function handleSpecialPricingRules() {
    emits('openSpecialPricingRulesModal', props.room)
}
</script>

<template>
    <div class="bg-white shadow-sm border border-gray-100 rounded-xl flex flex-col p-4">
        <section class="flex justify-between items-start mb-4">
            <div class="flex gap-x-3 items-center">
                <div class="p-3 rounded-lg bg-blue-50 text-blue-600">
                    <BedDouble class="h-6 w-6" />
                </div>
                <div class="flex flex-col">
                    <span class="font-bold text-gray-900 leading-tight">{{ room.name }}</span>
                    <span class="text-xs text-gray-500 uppercase tracking-wider">Szoba</span>
                </div>
            </div>

        </section>

        <section class="grid grid-cols-2 gap-3 mb-4">
            <div class="bg-gray-50 p-2 rounded-lg flex flex-col">
                <span class="text-[10px] text-gray-500 uppercase font-bold">Kapacitás</span>
                <div class="flex items-center gap-1 text-gray-700 font-semibold">
                    <Users class="h-3.5 w-3.5" />
                    {{ room.maxCapacity || '-' }} fő
                </div>
            </div>
            <div class="bg-gray-50 p-2 rounded-lg flex flex-col text-right">
                <span class="text-[10px] text-gray-500 uppercase font-bold">Ár / éj</span>
                <div class="flex items-center justify-end gap-1 text-blue-600 font-bold">
                    <Banknote class="h-3.5 w-3.5" />
                    {{ room.price?.toLocaleString('hu-HU') }} Ft
                </div>
            </div>
        </section>

        <section class="flex justify-end gap-1 pt-3 border-t border-gray-50">
            <button @click="handleAgePriceTier"
                class="p-2 rounded-lg hover:bg-yellow-50 text-gray-400 hover:text-yellow-500 transition-colors">
                <Coins class="h-5 w-5" />
            </button>
            <button @click="handlePriceTiers"
                class="p-2 rounded-lg hover:bg-blue-50 text-gray-400 hover:text-blue-500 transition-colors">
                <UsersRound class="h-5 w-5" />
            </button>
            <button @click="handleSpecialPricingRules"
                class="p-2 rounded-lg hover:bg-green-50 text-gray-400 hover:text-green-600 transition-colors">
                <Banknote class="h-5 w-5" />
            </button>
            <button @click="handleModify"
                class="p-2 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors">
                <EllipsisVertical class="h-5 w-5" />
            </button>
        </section>
    </div>
</template>
