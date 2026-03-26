<script setup>
import { House, EllipsisVertical } from 'lucide-vue-next'
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
    <div class="apartman-card bg-purple-50 shadow rounded-xl p-4 flex flex-col gap-3 transition-shadow">

        <div class="top flex justify-between items-start gap-2">
            <div class="min-w-0">
                <p class="font-semibold text-lg truncate">{{ name }}</p>
            </div>
            <div
                :class="[availabilityStatus.bgClass, 'flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs shrink-0']">
                <span :class="[availabilityStatus.dotClass, 'w-2 h-2 rounded-full inline-block']" />
                <span :class="availabilityStatus.textClass">{{ availabilityStatus.label }}</span>
            </div>
        </div>

        <div class="mid grid grid-cols-2 gap-2">
            <div class="flex flex-col">
                <span class="text-xs text-gray-500">Szobák száma</span>
                <span class="text-base font-semibold text-gray-900">{{ roomCount ?? '—' }}</span>
            </div>
            <div class="flex flex-col">
                <span class="text-xs text-gray-500">Tulajdonos</span>
                <span class="text-base font-semibold text-gray-900">{{ roomCount ?? '—' }}</span>
            </div>
        </div>

        <div class="bottom flex justify-between items-center border-t border-gray-100 pt-3">
            <div class="flex items-center gap-1.5 text-[13px] text-gray-500">
                <House class="w-4 h-4" />
                <span>{{ roomCount ?? '—' }} szoba</span>
            </div>
            <button @click="handleModify" class="text-[13px]  p-2 rounded-full hover:bg-gray-200 transition-colors">
                <EllipsisVertical class="h-5 w-5" />
            </button>
        </div>
    </div>
</template>

<style scoped></style>