<script setup>
import DefaultInput from '@/components/DefaultInput.vue';
import DefaultButton from '@/components/DefaultButton.vue';
import MainTitle from '@/shared/components/MainTitle.vue';
import { Search, RotateCcw, X } from 'lucide-vue-next';
import { useBookingStore } from '@/features/booking/stores/booking.store';
import { useAuthStore } from '@/features/auth/stores/auth';
import { useRole } from '@/composables/userRole';
import { reactive } from 'vue';

const { isAdmin } = useRole();
const emits = defineEmits(['close']);

const authStore = useAuthStore();
const bookingStore = useBookingStore();

const filterModel = reactive({
    name: '',
    email: '',
    startDate: '',
    endDate: '',
    roomId: '',
    apartmanId: '',
});

const formFields = [
    { label: 'Név', model: 'name', type: 'text' },
    { label: 'Email', model: 'email', type: 'text' },
    { label: 'Érkezés dátuma', model: 'startDate', type: 'date' },
    { label: 'Távozás dátuma', model: 'endDate', type: 'date' },
    { label: 'Szoba ID', model: 'roomId', type: 'number' },
    { label: 'Apartman ID', model: 'apartmanId', type: 'number' },
];

function closeFilter() {
    emits('close');
}

function applyFilters() {
    if (isAdmin.value) {
        bookingStore.applyAdminFilters({
            name: filterModel.name || '',
            email: filterModel.email || '',
            startDate: filterModel.startDate || '',
            endDate: filterModel.endDate || '',
            roomId: filterModel.roomId === '' ? null : Number(filterModel.roomId),
            apartmanId: filterModel.apartmanId === '' ? null : Number(filterModel.apartmanId),
        })
    }
    else {
        bookingStore.applyUserFilters({
            name: filterModel.name || '',
            email: filterModel.email || '',
            startDate: filterModel.startDate || '',
            endDate: filterModel.endDate || '',
            roomId: filterModel.roomId === '' ? null : Number(filterModel.roomId),
            apartmanId: filterModel.apartmanId === '' ? null : Number(filterModel.apartmanId),
            userId: authStore.user?.id === '' ? null : Number(authStore.user?.id),
        })
    }
}

function clearFilters() {
    filterModel.name = ''
    filterModel.email = ''
    filterModel.startDate = ''
    filterModel.endDate = ''
    filterModel.roomId = ''
    filterModel.apartmanId = ''
    if (isAdmin.value) {
        bookingStore.applyAdminFilters({
            name: '',
            email: '',
            startDate: '',
            endDate: '',
            roomId: null,
            apartmanId: null,
        })
    }
    else {
        bookingStore.applyUserFilters({
            name: '',
            email: '',
            startDate: '',
            endDate: '',
            roomId: null,
            apartmanId: null,
            userId: authStore.user?.id === '' ? null : Number(authStore.user?.id),
        })
    }
}

</script>

<template>
    <div
        class="filters p-5 bg-white rounded-xl shadow-sm border border-gray-100 absolute top-16 left-0 w-full flex flex-col justify-start items-start z-50">
        <button class="exit">
            <X class="h-5 w-5 absolute top-2 right-2 text-gray-500" @click="closeFilter" />
        </button>
        <MainTitle title="Szűrő" barColor="#fbcfc4" class="mb-4" />
        <div class="grid lg:grid-cols-7 md:grid-cols-4 grid-cols-2 gap-4">
            <DefaultInput v-for="(field, index) in formFields" :key="index" :label-text="field.label" :type="field.type"
                v-model="filterModel[field.model]" />
            <div class="flex items-end gap-2">
                <DefaultButton @click="applyFilters" text="Szűrés" :icon="Search"
                    button-class="flex-1 bg-blue-600 hover:bg-blue-700 text-white rounded-lg py-2 text-sm font-medium transition flex items-center justify-center gap-2" />
                <DefaultButton @click="clearFilters" :icon="RotateCcw"
                    button-class="p-2 border border-gray-200 rounded-lg hover:bg-gray-50 transition" />
            </div>
        </div>
    </div>
</template>

<style scoped></style>