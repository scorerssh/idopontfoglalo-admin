<script setup>
import { House, EllipsisVertical, User, LayoutGrid, Banknote } from 'lucide-vue-next'
import { computed } from 'vue'

const emit = defineEmits(['openModifyModal'])

const props = defineProps({
    id: { type: Number, required: true },
    name: { type: String, required: true },
    description: { type: String, default: null },
    price: { type: Number, default: null },
    roomCount: { type: Number, default: null },
    isAvailable: { type: Boolean, required: true },
})

const availabilityStatus = computed(() => {
    return props.isAvailable
        ? { label: 'Elérhető', bgClass: 'bg-green-100', textClass: 'text-green-700', dotClass: 'bg-green-500' }
        : { label: 'Nem elérhető', bgClass: 'bg-red-100', textClass: 'text-red-700', dotClass: 'bg-red-500' }
})

function handleModify() {
    emit('openModifyModal', props.id)
}
</script>

<template>
    <div class="bg-white shadow-sm border border-gray-100 rounded-xl flex flex-col p-4 transition-all hover:shadow-md">

        <section class="flex justify-between items-start mb-4">
            <div class="flex gap-x-3 items-center min-w-0">
                <div class="p-3 rounded-lg bg-purple-50 text-purple-600 shrink-0">
                    <House class="h-6 w-6" />
                </div>
                <div class="flex flex-col min-w-0">
                    <span class="font-bold text-gray-900 leading-tight truncate">{{ name }}</span>
                    <span class="text-xs text-gray-400 uppercase tracking-wider font-semibold">Apartman</span>
                </div>
            </div>
            <div
                :class="[availabilityStatus.bgClass, availabilityStatus.textClass, 'text-[11px] px-2.5 py-1 rounded-full flex items-center gap-1.5 font-bold shrink-0']">
                <span :class="[availabilityStatus.dotClass, 'h-2 w-2 rounded-full']"></span>
                {{ availabilityStatus.label }}
            </div>
        </section>

        <section class="flex-grow mb-4">
            <p class="text-sm text-gray-500 line-clamp-2 leading-relaxed">
                {{ description || 'Nincs megadott leírás az apartmanhoz.' }}
            </p>
        </section>

        <section class="grid grid-cols-2 gap-3 mb-4">
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Kapacitás</span>
                <div class="flex items-center gap-1.5 text-gray-700 font-bold text-sm">
                    <LayoutGrid class="h-3.5 w-3.5 text-purple-400" />
                    {{ roomCount ?? '0' }} szoba
                </div>
            </div>
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5 text-right">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Alapár</span>
                <div class="flex items-center justify-end gap-1.5 text-blue-600 font-bold text-sm">
                    <Banknote class="h-3.5 w-3.5" />
                    {{ price?.toLocaleString('hu-HU') || '—' }} Ft
                </div>
            </div>
        </section>

        <section class="flex justify-between items-center pt-3 border-t border-gray-50 mt-auto">
            <div class="flex items-center gap-2 text-xs text-gray-500 font-medium">
                <div class="w-6 h-6 rounded-full bg-gray-100 flex items-center justify-center">
                    <User class="w-3.5 h-3.5 text-gray-400" />
                </div>
                <span>Tulajdonos neve</span>
            </div>

            <button @click="handleModify"
                class="p-2 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors">
                <EllipsisVertical class="h-5 w-5" />
            </button>
        </section>
    </div>
</template>

<style scoped>
/* A line-clamp segít, hogy a leírás maximum 2 soros legyen */
.line-clamp-2 {
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
}
</style>