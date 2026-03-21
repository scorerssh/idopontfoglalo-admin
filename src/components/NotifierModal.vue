<!-- src/components/modals/MainModal.vue -->
<script setup>
import { Info } from 'lucide-vue-next';
const props = defineProps({
    isOpen: {
        type: Boolean,
        default: false,
    },
    content: {
        type: String,
        required: true,
    },
})

const emit = defineEmits(['close'])

function onClose() {
    emit('close')
}
</script>

<template>
    <!-- A Transition KÖZVETLEN gyereke kapja a v-if-et -->
    <Transition name="fade-slide">
        <div v-if="props.isOpen" class="main-modal fixed top-5 transform left-1/2 -translate-x-1/2 z-50">
            <div
                class="relative modal-content min-w-[300px] max-w-[500px] space-x-2 min-h-[70px] flex items-center justify-center gap-y-2 p-4 rounded-lg shadow bg-gray-50">
                <Info size="30" class="text-sky-700" />
                <p class="text-black font-semibold">
                    {{ props.content }}
                </p>
            </div>
        </div>
    </Transition>
</template>

<style>
.fade-slide-enter-from,
.fade-slide-leave-to {
    opacity: 0;
    transform: translateX(6px);
    /* ha balról akarod "úsztatni", legyen -6px */
}

.fade-slide-enter-to,
.fade-slide-leave-from {
    opacity: 1;
    transform: translateX(0);
}

.fade-slide-enter-active,
.fade-slide-leave-active {
    transition:
        opacity 0.18s ease-out,
        transform 0.18s ease-out;
}
</style>
