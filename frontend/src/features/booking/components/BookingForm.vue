<script setup>
import BookingFormInput from './BookingFormInput.vue'
import DefaultButton from '@/components/DefaultButton.vue';
import { bookingCreateSchema } from '../schemas/booking.schema';
import { useBookingStore } from '../stores/booking.store';
import { useRoomStore } from '@/features/rooms/stores/room.store';
const emit = defineEmits(['close'])
const bookingStore = useBookingStore()
const roomStore = useRoomStore()
const bookingInputs = computed(() => [
    {
        name: 'name',
        label: 'Név',
        type: 'text'
    },
    {
        name: 'email',
        label: 'Email',
        type: 'email'
    },
    {
        name: 'phone',
        label: 'Telefonszám',
        type: 'tel'
    },
    {
        name: 'startDate',
        label: 'Érkezés dátuma',
        type: 'date'
    },
    {
        name: 'endDate',
        label: 'Távozás dátuma',
        type: 'date'
    },
    {
        name: 'guests',
        label: 'Vendégek száma (összesen)',
        type: 'number'
    },
    {
        name: 'description',
        label: 'Leírás',
        type: 'text'
    },
    {
        name: 'roomId',
        label: 'Szoba',
        type: 'select',
        options: roomStore.rooms
    }
])

const errors = reactive({
    name: null,
    email: null,
    phone: null,
    startDate: null,
    endDate: null,
    guests: null,
    description: null,
    roomId: null
})

function resetErrors() {
    errors.name = null;
    errors.email = null;
    errors.phone = null;
    errors.startDate = null;
    errors.endDate = null;
    errors.guests = null;
    errors.description = null,
        errors.roomId = null
}

const bookingForm = reactive({
    name: '',
    email: '',
    phone: '',
    startDate: '',
    endDate: '',
    guests: '',
    description: '',
    roomId: ''
})

function resetForm() {
    bookingForm.name = '';
    bookingForm.email = '';
    bookingForm.phone = '';
    bookingForm.startDate = '';
    bookingForm.endDate = '';
    bookingForm.guests = '';
    bookingForm.description = '';
    bookingForm.roomId = '';
}

async function submitForm() {
    resetErrors()

    const result = bookingCreateSchema.safeParse({
        ...bookingForm,
        guests: Number(bookingForm.guests),
    })

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            errors[field] = err.message
        })
        return
    }

    const payload = {
        name: bookingForm.name,
        email: bookingForm.email,
        phoneNumber: bookingForm.phone,
        startTIme: bookingForm.startDate, // Javítva
        endTime: bookingForm.endDate,     // Javítva
        pearsonCount: Number(bookingForm.guests), // Javítva
        description: bookingForm.description,
        roomGUid: bookingForm.roomId
    }
    try {
        await bookingStore.createBooking(payload)
    } catch (err) {
        console.error('Booking create failed:', err)
    }
    emit('close')
    resetForm()
}
onMounted(async () => {
    if (!roomStore.rooms || roomStore.rooms.length === 0)
        await roomStore.getAll()
})
</script>

<template>
    <div class="grid">
        <form @submit.prevent="submitForm">
            <BookingFormInput v-for="input in bookingInputs" :key="input.name" :input-name="input.name"
                :type="input.type" v-model="bookingForm[input.name]" :labelText="input.label"
                label-class="text-black/60" class="mb-3" :error-text="errors[input.name]" :options="input.options" />
            <DefaultButton text="Foglalás létrehozása" type="submit"
                button-class="bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100" />
        </form>
    </div>
</template>

<style scoped></style>