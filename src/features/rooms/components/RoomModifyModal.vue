<script setup>
import { reactive, watch } from 'vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: { type: Boolean, required: true },
    apartmanData: { type: Object, default: null }
})

const apartmanStore = useApartmanStore()

const formData = reactive({
    id: null,
    name: '',
})

watch(() => props.apartmanData, (newApartman) => {
    if (newApartman) {
        formData.id = newApartman.id
        formData.name = newApartman.name ?? ''
    }
}, { immediate: true })

const updateApartman = async () => {
    if (!formData.id) return

    const payload = {
        id: props.apartmanData.id,
        name: formData.name,

    }
    await apartmanStore.update(payload)
    emit('close')
}

const deleteApartman = async () => {
    if (!formData.id) return
    if (!confirm('Biztosan szeretnéd törölni ezt az apartmant?')) return

    await apartmanStore.delete(formData.id)
    emit('close')
}

const handleClose = () => {
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal && apartmanData">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md z-50 max-h-[90vh] overflow-y-auto">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <h2 class="text-lg font-semibold mb-4">Apartman módosítása</h2>

                    <form @submit.prevent="updateApartman">
                        <div class="form-group mb-3">
                            <label for="name" class="block text-sm font-medium mb-1">
                                Név:
                            </label>
                            <input v-model="formData.name" type="text" id="name"
                                class="w-full border rounded px-3 py-2 text-sm" required />
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <button type="button"
                                class="px-4 py-2 text-sm rounded border border-red-300 text-red-600 hover:bg-red-50"
                                @click="deleteApartman">
                                Törlés
                            </button>
                            <button type="button" class="px-4 py-2 text-sm rounded border hover:bg-gray-100"
                                @click="handleClose">
                                Mégse
                            </button>
                            <button type="submit"
                                class="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700">
                                Mentés
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>