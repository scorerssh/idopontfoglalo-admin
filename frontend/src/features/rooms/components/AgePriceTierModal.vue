<script setup>
import { X, Plus, Pencil, Trash2 } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { useRoomStore } from '../stores/room.store'

const props = defineProps({
    showModal: { type: Boolean, required: true },
    room: { type: Object, default: null },
})

const emit = defineEmits(['close'])
const roomStore = useRoomStore()

const editingTier = ref(null)
const isAdding = ref(false)

const emptyForm = () => ({ price: '', ageRangeLow: '', ageRangeHigh: '' })
const formData = reactive(emptyForm())
const errors = reactive({ price: null, ageRangeLow: null, ageRangeHigh: null })

function resetErrors() {
    errors.price = null
    errors.ageRangeLow = null
    errors.ageRangeHigh = null
}

function resetForm() {
    Object.assign(formData, emptyForm())
    resetErrors()
}

function validateForm() {
    resetErrors()
    let valid = true
    if (formData.price === '' || isNaN(Number(formData.price)) || Number(formData.price) < 0) {
        errors.price = 'Az ár megadása kötelező és nem lehet negatív.'
        valid = false
    }
    if (formData.ageRangeLow === '' || isNaN(Number(formData.ageRangeLow)) || Number(formData.ageRangeLow) < 0) {
        errors.ageRangeLow = 'Az alsó korhatár megadása kötelező.'
        valid = false
    }
    if (formData.ageRangeHigh === '' || isNaN(Number(formData.ageRangeHigh)) || Number(formData.ageRangeHigh) < 0) {
        errors.ageRangeHigh = 'A felső korhatár megadása kötelező.'
        valid = false
    }
    if (valid && Number(formData.ageRangeLow) > Number(formData.ageRangeHigh)) {
        errors.ageRangeHigh = 'A felső korhatárnak nagyobbnak kell lennie az alsónál.'
        valid = false
    }
    return valid
}

watch(
    () => props.room,
    async (newRoom) => {
        if (newRoom && props.showModal) {
            editingTier.value = null
            isAdding.value = false
            resetForm()
            await roomStore.getAgePriceTiersByRoom(newRoom.id)
        }
    },
)

watch(
    () => props.showModal,
    async (val) => {
        if (val && props.room) {
            editingTier.value = null
            isAdding.value = false
            resetForm()
            await roomStore.getAgePriceTiersByRoom(props.room.id)
        }
    },
)

function startAdd() {
    editingTier.value = null
    isAdding.value = true
    resetForm()
}

function startEdit(tier) {
    isAdding.value = false
    editingTier.value = tier
    formData.price = tier.price
    formData.ageRangeLow = tier.ageRangeLow
    formData.ageRangeHigh = tier.ageRangeHigh
    resetErrors()
}

function cancelForm() {
    editingTier.value = null
    isAdding.value = false
    resetForm()
}

async function handleSave() {
    if (!validateForm()) return

    if (isAdding.value) {
        await roomStore.createAgePriceTier({
            roomId: props.room.id,
            price: Number(formData.price),
            ageRangeLow: Number(formData.ageRangeLow),
            ageRangeHigh: Number(formData.ageRangeHigh),
        })
    } else if (editingTier.value) {
        await roomStore.updateAgePriceTier({
            agePriceTierId: editingTier.value.id,
            roomId: props.room.id,
            price: Number(formData.price),
            ageRangeLow: Number(formData.ageRangeLow),
            ageRangeHigh: Number(formData.ageRangeHigh),
        })
    }

    cancelForm()
}

async function handleDelete(tier) {
    await roomStore.deleteAgePriceTier(tier.id)
    if (editingTier.value?.id === tier.id) cancelForm()
}

function handleClose() {
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal">
            <div class="fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md bg-white rounded-xl shadow-lg z-50 p-6 max-h-[90vh] overflow-y-auto">

                <button @click="handleClose"
                    class="absolute top-3 right-3 p-1.5 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors">
                    <X class="h-4 w-4" />
                </button>

                <MainTitle :title="`Korhatár árak – ${room?.name ?? ''}`" barColor="#fde68a" class="mb-5" />

                <div v-if="roomStore.agePriceTiers.length === 0 && !isAdding"
                    class="text-center py-6 text-gray-400 text-sm">
                    Nincs megadott korhatár ár.
                </div>

                <div v-else class="flex flex-col gap-y-2 mb-4">
                    <div v-for="tier in roomStore.agePriceTiers" :key="tier.id"
                        class="flex items-center justify-between bg-gray-50 rounded-lg px-3 py-2 border border-gray-100">
                        <div class="text-sm text-gray-700">
                            <span class="font-semibold">{{ tier.ageRangeLow }}–{{ tier.ageRangeHigh }} év</span>
                            <span class="ml-2 text-blue-600 font-bold">{{ Number(tier.price).toLocaleString('hu-HU') }} Ft</span>
                        </div>
                        <div class="flex gap-1">
                            <button @click="startEdit(tier)"
                                class="p-1.5 rounded-lg text-gray-400 hover:text-blue-600 hover:bg-blue-50 transition-colors">
                                <Pencil class="h-3.5 w-3.5" />
                            </button>
                            <button @click="handleDelete(tier)"
                                class="p-1.5 rounded-lg text-gray-400 hover:text-red-600 hover:bg-red-50 transition-colors">
                                <Trash2 class="h-3.5 w-3.5" />
                            </button>
                        </div>
                    </div>
                </div>

                <template v-if="isAdding || editingTier">
                    <div class="border-t border-gray-100 pt-4 mt-2">
                        <p class="text-xs font-semibold text-gray-500 uppercase mb-3">
                            {{ isAdding ? 'Új árkategória' : 'Szerkesztés' }}
                        </p>
                        <div class="flex flex-col gap-y-3">
                            <div>
                                <DefaultInput v-model="formData.ageRangeLow" label-text="Alsó korhatár (év)"
                                    type="number" label-class="mt-0 text-black/60" />
                                <span v-if="errors.ageRangeLow" class="text-red-500 text-xs mt-1 block">
                                    {{ errors.ageRangeLow }}
                                </span>
                            </div>
                            <div>
                                <DefaultInput v-model="formData.ageRangeHigh" label-text="Felső korhatár (év)"
                                    type="number" label-class="mt-0 text-black/60" />
                                <span v-if="errors.ageRangeHigh" class="text-red-500 text-xs mt-1 block">
                                    {{ errors.ageRangeHigh }}
                                </span>
                            </div>
                            <div>
                                <DefaultInput v-model="formData.price" label-text="Ár (Ft)"
                                    type="number" label-class="mt-0 text-black/60" />
                                <span v-if="errors.price" class="text-red-500 text-xs mt-1 block">
                                    {{ errors.price }}
                                </span>
                            </div>
                        </div>

                        <div class="flex gap-2 justify-end mt-4">
                            <DefaultButton type="button" text="Mégse" @click="cancelForm"
                                button-class="px-4 py-2 text-sm rounded-lg bg-gray-100 hover:bg-gray-200 text-gray-700 transition-colors" />
                            <DefaultButton type="button" text="Mentés" @click="handleSave"
                                button-class="px-4 py-2 text-sm rounded-lg text-white bg-blue-600 hover:bg-blue-700 transition-colors" />
                        </div>
                    </div>
                </template>

                <div v-if="!isAdding && !editingTier" class="flex justify-end mt-4 pt-4 border-t border-gray-100">
                    <DefaultButton type="button" text="Hozzáadás" :icon="Plus" @click="startAdd"
                        button-class="px-4 py-2 text-sm rounded-lg text-white bg-blue-600 hover:bg-blue-700 transition-colors" />
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>
