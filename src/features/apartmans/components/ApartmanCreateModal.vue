<script setup>
import { reactive } from 'vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: {
        type: Boolean,
        required: true
    }
})

const apartmanStore = useApartmanStore()

const formData = reactive({
    name: '',
})

const resetForm = () => {
    formData.name = ''
}

const createApartman = async () => {
    const payload = {
        name: formData.name,
    }

    await apartmanStore.create(payload)
    resetForm()
    emit('close')
}

const handleClose = () => {
    resetForm()
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md z-50 max-h-[90vh] overflow-y-auto">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <h2 class="text-lg font-semibold mb-4">Apartman létrehozása</h2>

                    <form @submit.prevent="createApartman">
                        <div class="form-group mb-3">
                            <label for="name" class="block text-sm font-medium mb-1">
                                Név:
                            </label>
                            <input v-model="formData.name" type="text" id="name"
                                class="w-full border rounded px-3 py-2 text-sm" required />
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <button type="button" class="px-4 py-2 text-sm rounded border hover:bg-gray-100"
                                @click="handleClose">
                                Mégse
                            </button>
                            <button type="submit"
                                class="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700">
                                Létrehozás
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>