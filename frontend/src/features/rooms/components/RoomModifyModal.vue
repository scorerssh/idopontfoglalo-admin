<script setup>
import { reactive, watch } from 'vue'
import { useRoomStore } from '@/features/rooms/stores/room.store'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: { type: Boolean, required: true },
    roomData: { type: Object, default: null },
})

const roomStore = useRoomStore()

const formData = reactive({
    id: null,
    name: '',
    minCapacity: 0,
    maxCapacity: 0,
    apartmanId: null,
})

watch(
    () => props.roomData,
    (newRoom) => {
        if (newRoom) {
            formData.id = newRoom.id
            formData.name = newRoom.name ?? ''
            formData.minCapacity = Number(newRoom.minCapacity ?? 0)
            formData.maxCapacity = Number(newRoom.maxCapacity ?? 0)
            formData.apartmanId = Number(newRoom.apartmanId ?? null)
        }
    },
    { immediate: true },
)

const updateRoom = async () => {
    if (!formData.id) return

    const payload = {
        roomId: formData.id,
        name: formData.name,
        minCapacity: formData.minCapacity,
        maxCapacity: formData.maxCapacity,
        apartmanId: formData.apartmanId,
    }

    await roomStore.update(payload)
    emit('close')
}

const deleteRoom = async () => {
    if (!formData.id) return
    if (!confirm('Biztosan szeretnéd törölni ezt a szobát?')) return

    await roomStore.delete(formData.id)
    emit('close')
}

const handleClose = () => {
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal && roomData">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md z-50 max-h-[90vh] overflow-y-auto">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <h2 class="text-lg font-semibold mb-4">Szoba módosítása</h2>

                    <form @submit.prevent="updateRoom">
                        <div class="grid gap-3">
                            <div class="form-group">
                                <label for="name" class="block text-sm font-medium mb-1">Név</label>
                                <input v-model="formData.name" type="text" id="name"
                                    class="w-full border rounded px-3 py-2 text-sm" required />
                            </div>

                            <div class="form-group">
                                <label for="minCapacity" class="block text-sm font-medium mb-1">Min kapacitás</label>
                                <input v-model.number="formData.minCapacity" type="number" id="minCapacity"
                                    class="w-full border rounded px-3 py-2 text-sm" min="0" required />
                            </div>

                            <div class="form-group">
                                <label for="maxCapacity" class="block text-sm font-medium mb-1">Max kapacitás</label>
                                <input v-model.number="formData.maxCapacity" type="number" id="maxCapacity"
                                    class="w-full border rounded px-3 py-2 text-sm" min="0" required />
                            </div>

                            <div class="form-group">
                                <label for="apartmanId" class="block text-sm font-medium mb-1">Apartman ID</label>
                                <input v-model.number="formData.apartmanId" type="number" id="apartmanId"
                                    class="w-full border rounded px-3 py-2 text-sm" min="0" required />
                            </div>
                        </div>

                        <div class="form-actions flex gap-2 justify-end pt-4">
                            <button type="button"
                                class="px-4 py-2 text-sm rounded border border-red-300 text-red-600 hover:bg-red-50"
                                @click="deleteRoom">
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