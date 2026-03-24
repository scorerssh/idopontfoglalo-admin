<script setup>
import { House } from 'lucide-vue-next'
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
    <div
        class="apartman-card bg-white border border-gray-200 border-l-[3px] border-l-[#fbcfc4] rounded-xl p-4 flex flex-col gap-3 shadow-sm hover:shadow-md transition-shadow">

        <div class="top flex justify-between items-start gap-2">
            <div class="min-w-0">
                <p class="font-semibold text-[15px] text-gray-900 truncate">{{ name }}</p>
                <p class="text-[13px] text-gray-500 truncate mt-0.5">{{ description || '—' }}</p>
            </div>
            <div
                :class="[availabilityStatus.bgClass, 'flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs shrink-0']">
                <span :class="[availabilityStatus.dotClass, 'w-2 h-2 rounded-full inline-block']" />
                <span :class="availabilityStatus.textClass">{{ availabilityStatus.label }}</span>
            </div>
        </div>

        <div class="mid grid grid-cols-2 gap-2">
            <div class="flex flex-col">
                <span class="text-xs text-gray-500">Ár (HUF)</span>
                <span class="text-base font-semibold text-blue-600">
                    {{ price?.toLocaleString('hu-HU') ?? '—' }}
                </span>
            </div>
            <div class="flex flex-col">
                <span class="text-xs text-gray-500">Szobák</span>
                <span class="text-base font-semibold text-gray-900">{{ roomCount ?? '—' }}</span>
            </div>
        </div>

        <div class="bottom flex justify-between items-center border-t border-gray-100 pt-3">
            <div class="flex items-center gap-1.5 text-[13px] text-gray-500">
                <House class="w-4 h-4" />
                <span>{{ roomCount ?? '—' }} szoba</span>
            </div>
            <button @click="handleModify"
                class="text-[13px] px-3 py-1.5 rounded-lg border border-gray-200 hover:bg-gray-50 transition-colors">
                Módosítás
            </button>
        </div>
    </div>
</template>

<style scoped></style>