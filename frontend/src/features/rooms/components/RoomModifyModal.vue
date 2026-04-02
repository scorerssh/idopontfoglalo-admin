<script setup>
import { reactive, watch } from 'vue'
import { useRoomStore } from '@/features/rooms/stores/room.store'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import MainTitle from '@/components/MainTitle.vue'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: { type: Boolean, required: true },
    roomData: { type: Object, default: null },
})

const inputs = [
    { type: 'text', labelText: 'Név', inputName: 'name' },
    { type: 'number', labelText: 'Min. Kapacitás', inputName: 'minCapacity' },
    { type: 'number', labelText: 'Max. Kapacitás', inputName: 'maxCapacity' },
]


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
                    <MainTitle :title="`Szoba módosítása: ${formData.name}`" bar-color="#fbcfc4" class="mb-4" />

                    <form @submit.prevent="updateRoom">
                        <div class="grid gap-3">
                            <DefaultInput v-for="input in inputs" :key="input.inputName" :input-name="input.inputName"
                                v-model="formData[input.inputName]" :type="input.type" :label-text="input.labelText" />

                            <div class="form-group">
                                <label for="apartmanId" class="block text-sm font-medium mb-1">Apartman ID</label>
                                <input v-model.number="formData.apartmanId" type="number" id="apartmanId"
                                    class="w-full border rounded px-3 py-2 text-sm" min="0" required />
                            </div>
                        </div>

                        <div class="form-actions flex gap-2 justify-end pt-4">
                            <DefaultButton type="button" text="Törlés" @click="deleteRoom"
                                button-class="px-4 py-2 text-sm rounded bg-red-600 text-white hover:bg-red-700">
                            </DefaultButton>
                            <DefaultButton type="button" text="Mégse" @click="handleClose"
                                button-class="px-4 py-2 text-sm rounded bg-gray-100  hover:bg-gray-200"></DefaultButton>
                            <DefaultButton type="submit" text="Mentés"
                                button-class="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700">
                            </DefaultButton>
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>