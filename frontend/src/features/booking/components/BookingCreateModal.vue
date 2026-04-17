<script setup>
import BookingForm from './BookingForm.vue';
import MainTitle from '@/components/MainTitle.vue';
const emit = defineEmits(['close'])
const props = defineProps({
    showModal: {
        type: Boolean,
        required: true
    }
})
function handleClose() {
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />

            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full md:w-1/3 z-50">
                <MainTitle title="Új foglalás" class="pb-5" bar-color="#fbcfc4" />
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>
                <BookingForm @close="handleClose" />
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>