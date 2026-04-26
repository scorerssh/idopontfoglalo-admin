<script setup>
import { X } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { useBookingStore } from '../stores/booking.store'
import { bookingModifySchema } from '../schemas/bookingModify.schema'

const props = defineProps({
    showModal: { type: Boolean, required: true },
    booking: { type: Object, default: null }
})

const selectedBookingId = computed(() => props.booking?.id)
const emit = defineEmits(['close'])
const bookingStore = useBookingStore()

const formData = reactive({
    startTIme: '',
    endTime: '',
    pearsonCount: '',
    name: '',
    phoneNumber: '',
    email: '',
    description: '',
})

const errors = reactive({
    name: null,
    email: null,
    phoneNumber: null,
    startTIme: null,
    endTime: null,
    pearsonCount: null,
    description: null,
})

const inputs = [
    { name: 'name', label: 'Vendég neve' },
    { name: 'email', label: 'Email' },
    { name: 'phoneNumber', label: 'Telefonszám' },
    { name: 'startTIme', label: 'Érkezés dátuma' },
    { name: 'endTime', label: 'Távozás dátuma' },
    { name: 'pearsonCount', label: 'Vendégek száma' },
    { name: 'description', label: 'Megjegyzés' },
]

function resetErrors() {
    Object.keys(errors).forEach(k => errors[k] = null)
}

watch(() => props.booking, (newBooking) => {
    if (newBooking) {
        formData.startTIme = newBooking.startTIme ?? ''
        formData.endTime = newBooking.endTime ?? ''
        formData.pearsonCount = newBooking.pearsonCount ?? ''
        formData.name = newBooking.name ?? ''
        formData.phoneNumber = newBooking.phoneNumber ?? ''
        formData.email = newBooking.email ?? ''
        formData.description = newBooking.description ?? ''
    }
    resetErrors()
}, { immediate: true })

const modifiedPayload = computed(() => {
    if (!props.booking) return {}

    const changes = {}
    for (const key in formData) {
        const original = String(props.booking[key] ?? '')
        const current = String(formData[key] ?? '')
        if (current !== original) {
            changes[key] = formData[key]
        }
    }

    return {
        reservationId: props.booking.id,
        ...changes
    }
})

const hasChanges = computed(() => Object.keys(modifiedPayload.value).length > 1)

async function handleSave() {
    if (!hasChanges.value) return

    resetErrors()
    const result = bookingModifySchema.safeParse({
        name: formData.name,
        email: formData.email,
        phoneNumber: formData.phoneNumber,
        startTIme: formData.startTIme,
        endTime: formData.endTime,
        pearsonCount: formData.pearsonCount,
        description: formData.description,
    })

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            errors[field] = err.message
        })
        return
    }

    await bookingStore.updateBooking(modifiedPayload.value)
    emit('close')
}

function handleReset() {
    if (!props.booking) return
    formData.startTIme = props.booking.startTIme ?? ''
    formData.endTime = props.booking.endTime ?? ''
    formData.pearsonCount = props.booking.pearsonCount ?? ''
    formData.name = props.booking.name ?? ''
    formData.phoneNumber = props.booking.phoneNumber ?? ''
    formData.email = props.booking.email ?? ''
    formData.description = props.booking.description ?? ''
    resetErrors()
}

function handleDelete() {
    bookingStore.deleteBooking(selectedBookingId.value)
    emit('close')
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
                class="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[calc(100%-20px)] max-w-md bg-white rounded-xl shadow-lg z-50 p-6 max-h-[90vh] overflow-y-auto">

                <button @click="handleClose"
                    class="absolute top-3 right-3 p-1.5 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors">
                    <X class="h-4 w-4" />
                </button>

                <MainTitle title="Foglalás módosítása" barColor="#c8f1fb" class="mb-5" />

                <div class="flex flex-col gap-y-3">
                    <div v-for="input in inputs" :key="input.name">
                        <DefaultInput v-model="formData[input.name]" :label-text="input.label"
                            label-class="mt-0 text-black/60" />
                        <span v-if="errors[input.name]" class="text-red-500 text-xs mt-1 block">
                            {{ errors[input.name] }}
                        </span>
                    </div>
                </div>

                <div class="flex gap-2 justify-end mt-6 pt-4 border-t border-gray-100">
                    <DefaultButton type="button" text="Törlés" @click="handleDelete"
                        button-class="px-4 py-2 text-sm rounded-lg bg-red-50 text-red-600 hover:bg-red-100 transition-colors" />
                    <DefaultButton type="button" text="Visszaállít" @click="handleReset"
                        button-class="px-4 py-2 text-sm rounded-lg bg-gray-100 hover:bg-gray-200 text-gray-700 transition-colors" />
                    <DefaultButton type="button" text="Mentés" @click="handleSave" :button-class="[
                        'px-4 py-2 text-sm rounded-lg text-white transition-colors',
                        hasChanges ? 'bg-blue-600 hover:bg-blue-700' : 'bg-blue-300 cursor-not-allowed'
                    ]" />
                </div>

            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>
