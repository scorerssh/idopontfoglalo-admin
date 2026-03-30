<script setup>
import DefaultInput from '@/components/DefaultInput.vue';
import BookingFormInput from './BookingFormInput.vue'
import bookingShecmas from '../schemas/booking.schema';

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
        name: 'date',
        label: 'Date',
        type: 'date'
    },
    {
        name: 'time',
        label: 'Time',
        type: 'time'
    },
    {
        name: 'guests',
        label: 'Number of Guests',
        type: 'number'
    }
]

const errors = reactive({
    name: null,
    email: null,
    phone: null,
    date: null,
    time: null,
    guests: null,
    paymentMethod: null
})

function resetErrors() {
    errors.name = null;
    errors.email = null;
    errors.phone = null;
    errors.date = null;
    errors.time = null;
    errors.guests = null;
    errors.paymentMethod = null;
}

const bookingForm = reactive({
    name: '',
    email: '',
    phone: '',
    date: '',
    time: '',
    guests: ''
})

function resetForm() {
    bookingForm.name = '';
    bookingForm.email = '';
    bookingForm.phone = '';
    bookingForm.date = '';
    bookingForm.time = '';
    bookingForm.guests = '';
}

function submitForm() {
    resetErrors()
    const result = bookingShecmas.bookingCreateShecma.safeParse({
        name: bookingForm.name,
        email: bookingForm.email,
        phone: bookingForm.phone,
        date: bookingForm.date,
        time: bookingForm.time,
        guests: bookingForm.guests
    })

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            errors[field] = err.message
        })
        return
    }
}
</script>

<template>
    <div class="grid grid-cols-2 p-3">
        <form @submit.prevent="submitForm">
            <DefaultInput v-for="input in bookingInputs" :key="input.name" :input-name="input.name"
                :type="input.type" />
        </form>
    </div>
</template>

<style scoped></style>