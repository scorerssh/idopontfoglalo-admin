<script setup>
import { Search, RotateCcw, X } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import { useRoomStore } from '@/features/rooms/stores/room.store'

const emits = defineEmits(['close'])
const roomStore = useRoomStore()

// Reaktív lokális állapot a szűrőknek
const filterModel = reactive({
    name: '',
    minCapacity: '',
    maxCapacity: '',
    apartmanId: ''
})

// Betöltéskor szinkronizáljuk a store-ban lévő értékekkel
onMounted(() => {
    filterModel.name = roomStore.filters.name || ''
    filterModel.minCapacity = roomStore.filters.minCapacity || ''
    filterModel.maxCapacity = roomStore.filters.maxCapacity || ''
    filterModel.apartmanId = roomStore.filters.apartmanId || ''
})

function applyFilters() {
    roomStore.applyFilters({
        name: filterModel.name,
        minCapacity: filterModel.minCapacity === '' ? null : Number(filterModel.minCapacity),
        maxCapacity: filterModel.maxCapacity === '' ? null : Number(filterModel.maxCapacity),
        apartmanId: filterModel.apartmanId === '' ? null : Number(filterModel.apartmanId),
    })
    emits('close')
}

function clearFilters() {
    filterModel.name = ''
    filterModel.minCapacity = ''
    filterModel.maxCapacity = ''
    filterModel.apartmanId = ''
    roomStore.applyFilters({
        name: '',
        minCapacity: null,
        maxCapacity: null,
        apartmanId: null
    })
}

function closeFilter() {
    emits('close')
}
</script>

<template>
    <div
        class="filters p-5 bg-white rounded-xl shadow-2xl border border-gray-100 absolute top-16 right-0 flex flex-col justify-start items-start z-50 min-w-[300px]">
        <button type="button" class="absolute top-2 right-2 text-gray-400 hover:text-gray-600 transition"
            @click="closeFilter">
            <X class="h-5 w-5" />
        </button>

        <MainTitle title="Szűrés" barColor="#fbcfc4" class="mb-4" />

        <div class="flex flex-col gap-4 w-full">
            <DefaultInput label-text="Név" type="text" v-model="filterModel.name" />

            <div class="grid grid-cols-2 gap-2">
                <DefaultInput label-text="Min. kapacitás" type="number" v-model="filterModel.minCapacity" />
                <DefaultInput label-text="Max. kapacitás" type="number" v-model="filterModel.maxCapacity" />
            </div>

            <div class="flex items-end gap-2 mt-2">
                <DefaultButton @click="applyFilters" text="Szűrés" :icon="Search"
                    button-class="flex-1 bg-blue-600 hover:bg-blue-700 text-white rounded-lg py-2 text-sm font-medium transition flex items-center justify-center gap-2" />

                <DefaultButton @click="clearFilters" :icon="RotateCcw"
                    button-class="p-2 border border-gray-200 rounded-lg hover:bg-gray-50 transition text-gray-600" />
            </div>
        </div>
    </div>
</template>