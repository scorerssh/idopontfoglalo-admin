<script setup>
import BookingFormInput from './BookingFormInput.vue'
import { bookingCreateSchema } from '../schemas/booking.schema';
import { useBookingStore } from '../stores/booking.store';
import { useRoomStore } from '@/features/rooms/stores/room.store';
import { onMounted, reactive } from 'vue';

const bookingStore = useBookingStore()
const roomStore = useRoomStore()
const selectedRoomId = ref(null)
const bookingInputs = [
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
        label: 'Start Date',
        type: 'date'
    },
    {
        name: 'endDate',
        label: 'End Date',
        type: 'date'
    },
    {
        name: 'guests',
        label: 'Number of Guests',
        type: 'number'
    },
    {
        name: 'description',
        label: 'Description',
        type: 'text'
    },
]

const errors = reactive({
    name: null,
    email: null,
    phone: null,
    startDate: null,
    endDate: null,
    guests: null,
})

function resetErrors() {
    errors.name = null;
    errors.email = null;
    errors.phone = null;
    errors.startDate = null;
    errors.endDate = null;
    errors.guests = null;
}

const bookingForm = reactive({
    name: '',
    email: '',
    phone: '',
    startDate: '',
    endDate: '',
    guests: '',
    description: '',
})

function resetForm() {
    bookingForm.name = '';
    bookingForm.email = '';
    bookingForm.phone = '';
    bookingForm.startDate = '';
    bookingForm.endDate = '';
    bookingForm.guests = '';
    bookingForm.description = '';
    bookingForm.room = '';
}

async function submitForm() {
    resetErrors()
    const result = bookingCreateSchema.safeParse({
        name: bookingForm.name,
        email: bookingForm.email,
        phone: bookingForm.phone,
        startDate: bookingForm.startDate,
        endDate: bookingForm.endDate,
        guests: Number(bookingForm.guests),
        description: bookingForm.description
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
        startTIme: bookingForm.startDate, // igen, így...
        endTime: bookingForm.endDate,
        pearsonCount: Number(bookingForm.guests),
        description: bookingForm.description,
        roomGUid: selectedRoomId.value
    }

    await bookingStore.createBooking(payload)
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
                label-class="text-black/60" class="mb-3" />
            <select name="roomGuidId" id="" v-model="selectedRoomId">
                <option :value="selectedRoomId" disabled selected>Válassz szobát</option>
                <option v-for="room in roomStore.rooms" :key="room.id" :value="room.guidId">{{
                    room.name }}</option>
            </select>
            <button type="submit"
                class="mt-4 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition-colors">
                Create Booking
            </button>
        </form>
    </div>
</template>

<style scoped></style>