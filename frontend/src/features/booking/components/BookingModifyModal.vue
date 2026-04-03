<script setup>
import { reactive, computed, watch } from 'vue'
import { CalendarDays, Users, Phone, Mail, FileText, X } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { useBookingStore } from '../stores/booking.store'


const props = defineProps({
    showModal: { type: Boolean, required: true },
    booking: { type: Object, default: null }
})

const selectedBookingId = computed(() => props.booking.id)
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

const inputs = [
    { name: 'name', label: 'Vendég neve', icon: null },
    { name: 'email', label: 'Email', icon: null },
    { name: 'phoneNumber', label: 'Telefonszám', icon: null },
    { name: 'startTIme', label: 'Érkezés dátuma', icon: null },
    { name: 'endTime', label: 'Távozás dátuma', icon: null },
    { name: 'pearsonCount', label: 'Vendégek száma', icon: null },
    { name: 'description', label: 'Megjegyzés', icon: null },
]

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
}, { immediate: true })

const modifiedPayload = computed(() => {
    if (!props.booking) return {}

    const payload = {}
    for (const key in formData) {
        const original = String(props.booking[key] ?? '')
        const current = String(formData[key] ?? '')
        if (current !== original) {
            payload[key] = formData[key]
        }
    }
    return payload
})

const hasChanges = computed(() => Object.keys(modifiedPayload.value).length > 0)

async function handleSave() {
    if (!hasChanges.value) return
    await bookingStore.updateBooking(modifiedPayload.value)
    console.log('Modified payload:', modifiedPayload.value)
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
}

function handleDelete() {
    bookingStore.deleteBooking(selectedBookingId.value)
    console.log('Delete booking:', props.booking)
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
                class="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md bg-white rounded-xl shadow-lg z-50 p-6">

                <button @click="handleClose"
                    class="absolute top-3 right-3 p-1.5 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors">
                    <X class="h-4 w-4" />
                </button>

                <MainTitle title="Foglalás módosítása" barColor="#c8f1fb" class="mb-5" />

                <div class="flex flex-col gap-y-3">
                    <DefaultInput v-for="input in inputs" :key="input.name" v-model="formData[input.name]"
                        :label-text="input.label" label-class="mt-0 text-black/60" />
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