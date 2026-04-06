<script setup>
import BookingFormInput from './BookingFormInput.vue'
import DefaultButton from '@/components/DefaultButton.vue';
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
    description: null,
})

function resetErrors() {
    errors.name = null;
    errors.email = null;
    errors.phone = null;
    errors.startDate = null;
    errors.endDate = null;
    errors.guests = null;
    errors.description = null
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
        ...bookingForm,
        guests: Number(bookingForm.guests),
        roomGUid: selectedRoomId.value // Ha hozzáadod a sémához
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
        startTime: bookingForm.startDate, // Javítva
        endTime: bookingForm.endDate,     // Javítva
        personCount: Number(bookingForm.guests), // Javítva
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
                label-class="text-black/60" class="mb-3" :error-text="errors[input.name]" />
            <select name="roomGuidId" id="" v-model="selectedRoomId">
                <option :value="selectedRoomId" disabled selected>Válassz szobát</option>
                <option v-for="room in roomStore.rooms" :key="room.id" :value="room.guidId">{{
                    room.name }}</option>
            </select>
            <DefaultButton text="Foglalás létrehozása" type="submit"
                button-class="bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100" />
        </form>
    </div>
</template>

<style scoped></style>