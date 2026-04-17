<script setup>
import { reactive, computed, onMounted } from 'vue'
import { X, UserRound, UsersRound } from 'lucide-vue-next'
import BookingFormInput from './BookingFormInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { bookingCreateSchema } from '../schemas/booking.schema'
import { useBookingStore } from '../stores/booking.store'
import { useRoomStore } from '@/features/rooms/stores/room.store'
import { useRole } from '@/composables/useRole'

const { isAdmin } = useRole()
const emit = defineEmits(['close'])
const bookingStore = useBookingStore()
const roomStore = useRoomStore()

// --- INITIAL STATE (egy helyen definiálva, újrahasznosítható resetnél) ---
const initialFormState = () => ({
    name: '',
    email: '',
    phone: '',
    startDate: '',
    endDate: '',
    persons: [{ age: '' }],
    description: '',
    roomId: ''
})

const initialErrorsState = () => ({
    name: null,
    email: null,
    phone: null,
    startDate: null,
    endDate: null,
    persons: null,
    description: null,
    roomId: null
})

const bookingForm = reactive(initialFormState())
const errors = reactive(initialErrorsState())

// --- COMPUTED ---
const userIcon = computed(() =>
    bookingForm.persons.length > 1 ? UsersRound : UserRound
)

const bookingInputs = computed(() => [
    { name: 'name', label: 'Név', type: 'text' },
    { name: 'email', label: 'Email', type: 'email' },
    { name: 'phone', label: 'Telefonszám', type: 'tel' },
    { name: 'startDate', label: 'Érkezés dátuma', type: 'date' },
    { name: 'endDate', label: 'Távozás dátuma', type: 'date' },
    { name: 'description', label: 'Leírás', type: 'text' },
    { name: 'roomId', label: 'Szoba', type: 'select', options: roomStore.rooms }
])

// --- RESET HELPERS (egyszerűbbek, nem felejtesz el mezőt) ---
function resetErrors() {
    Object.assign(errors, initialErrorsState())
}

function resetForm() {
    Object.assign(bookingForm, initialFormState())
}

// --- PERSON LIST ---
function addPerson() {
    bookingForm.persons.push({ age: '' })
}

function removePerson(index) {
    bookingForm.persons.splice(index, 1)
}

// --- SUBMIT ---
async function submitForm() {
    resetErrors()

    const result = bookingCreateSchema.safeParse({ ...bookingForm })

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            if (field in errors) {
                errors[field] = err.message
            }
        })
        return
    }

    const payload = {
        name: bookingForm.name,
        email: bookingForm.email,
        phoneNumber: bookingForm.phone,
        startTIme: bookingForm.startDate,
        endTime: bookingForm.endDate,
        persons: bookingForm.persons.map(p => ({ age: Number(p.age) })),
        description: bookingForm.description,
        roomGUid: bookingForm.roomId,
        pearsonCount: bookingForm.persons.length,
    }

    try {
        await bookingStore.createBooking(payload)
        emit('close')
        resetForm()
    } catch (err) {
        console.error('Booking create failed:', err)
    }
}

// --- LIFECYCLE ---
onMounted(async () => {
    if (!roomStore.rooms || roomStore.rooms.length === 0) {
        if (isAdmin.value) {
            await roomStore.getAll()
        } else {
            await roomStore.getAllUser()
        }
    }
})
</script>

<template>
    <div class="flex flex-col">
        <form @submit.prevent="submitForm" class="grid grid-cols-[1fr_auto_1fr] gap-x-6">
            <!-- BAL OSZLOP: Alap foglalási adatok -->
            <div class="booking-general">
                <BookingFormInput v-for="input in bookingInputs" :key="input.name" :input-name="input.name"
                    :type="input.type" v-model="bookingForm[input.name]" :label-text="input.label"
                    label-class="text-black/60" class="mb-3" :error-text="errors[input.name]"
                    :options="input.options" />
            </div>

            <!-- FÜGGŐLEGES ELVÁLASZTÓ -->
            <div class="w-px bg-gray-200"></div>

            <!-- JOBB OSZLOP: Vendégek -->
            <div class="persons mt-5.5">
                <!-- Vendégszámláló fejléc ikonnal -->
                <div class="flex items-center gap-2 mb-4 px-3 py-2 bg-green-100 ring-1 ring-green-200 rounded-lg">
                    <component :is="userIcon" class="w-5 h-5 text-gray-600" />
                    <span class="font-medium text-gray-700">
                        {{ bookingForm.persons.length }} vendég
                    </span>
                </div>

                <!-- Persons szintű error (ha van) -->
                <p v-if="errors.persons" class="text-red-500 text-sm mb-2">
                    {{ errors.persons }}
                </p>

                <!-- Vendégek listája -->
                <div v-for="(person, index) in bookingForm.persons" :key="index"
                    class="flex items-center gap-2 mb-2 relative">
                    <BookingFormInput :input-name="`person-${index}-age`" type="number" v-model="person.age"
                        :label-text="`${index + 1}. vendég életkora`" label-class="text-black/60" class="flex-1" />
                    <button v-if="bookingForm.persons.length > 1" type="button" @click="removePerson(index)"
                        class="text-red-500 transition-colors duration-100 hover:bg-red-700 hover:text-white absolute top-3 p-1 rounded-full bg-white shadow-md -right-2">
                        <X class="w-4 h-4" />
                    </button>
                </div>

                <!-- Vendég hozzáadása gomb -->
                <button type="button" @click="addPerson"
                    class="text-blue-500 hover:text-blue-700 mb-4 text-sm font-medium">
                    + Vendég hozzáadása
                </button>
            </div>

            <!-- SUBMIT GOMB (mindkét oszlop alá) -->
            <DefaultButton text="Foglalás létrehozása" type="submit"
                button-class="col-span-3 bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100 mt-4" />
        </form>
    </div>
</template>

<style scoped></style>