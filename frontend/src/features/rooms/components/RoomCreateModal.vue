<script setup>
import { useRoomStore } from '@/features/rooms/stores/room.store'
import { roomCreateSchema } from '../schemas/roomCreate.schema'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import MainTitle from '@/components/MainTitle.vue'
import { useRole } from '@/composables/useRole'

const { isAdmin } = useRole()
const emit = defineEmits(['close'])
const roomStore = useRoomStore()
const apartmanStore = useApartmanStore()
const props = defineProps({
    showModal: {
        type: Boolean,
        required: true,
    },
})

const inputs = computed(() => [
    { type: 'text', labelText: 'Név', inputName: 'name' },
    { type: 'number', labelText: 'Min. Kapacitás', inputName: 'minCapacity' },
    { type: 'number', labelText: 'Max. Kapacitás', inputName: 'maxCapacity' },
    { type: 'number', labelText: 'Szoba ára', inputName: 'price' },
    { type: 'select', labelText: 'Apartman kiválasztása', inputName: 'apartmanId', options: apartmanStore.apartmans }
])


const formData = reactive({
    name: '',
    minCapacity: 0,
    maxCapacity: 0,
    price: 0,
    apartmanId: null,
})

const resetForm = () => {
    formData.name = ''
    formData.minCapacity = 0
    formData.maxCapacity = 0
    formData.price = 0
    formData.apartmanId = null
    resetErrors()
}

const errors = reactive({
    name: null,
    minCapacity: null,
    maxCapacity: null,
    price: null,
    apartmanId: null
})

function resetErrors() {
    errors.name = null,
        errors.minCapacity = null,
        errors.maxCapacity = null,
        errors.price = null
    errors.apartmanId = null

}

async function createRoom() {
    resetErrors()
    const result = roomCreateSchema.safeParse({
        name: formData.name,
        minCapacity: formData.minCapacity,
        maxCapacity: formData.maxCapacity,
        apartmanId: formData.apartmanId,
        price: formData.price,
    })

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            errors[field] = err.message
        })
        return
    }

    const payload = {
        name: formData.name,
        MinCapacity: formData.minCapacity,
        MaxCapacity: formData.maxCapacity,
        price: formData.price,
        apartmanId: formData.apartmanId,
    }

    await roomStore.create(payload)
    resetForm()
    emit('close')
}

function handleClose() {
    resetForm()
    emit('close')
}


onMounted(async () => {
    if (isAdmin.value && !apartmanStore.apartmans?.length) {
        await apartmanStore.getAll()
    }
})
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md z-50">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <MainTitle title="Szoba létrehozása" bar-color="#fbcfc4" class="mb-4" />
                    <form @submit.prevent="createRoom">
                        <div class="grid gap-3">
                            <div v-for="input in inputs" :key="input.inputName">
                                <DefaultInput :input-name="input.inputName" :label-text="input.labelText"
                                    :type="input.type" label-class="text-sm text-black/60"
                                    v-model="formData[input.inputName]" :options="input.options || []" />
                                <span v-if="errors[input.inputName]" class="text-red-500 text-xs mt-1">
                                    {{ errors[input.inputName] }}
                                </span>
                            </div>
                        </div>


                        <div class="form-actions flex gap-2 justify-end pt-4">
                            <DefaultButton text="Mégse" type="button"
                                button-class="px-4 py-2 text-sm rounded border hover:bg-gray-100"
                                @click="handleClose" />
                            <DefaultButton text="Létrehozás" type="submit"
                                button-class="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700" />
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>