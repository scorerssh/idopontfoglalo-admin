<script setup>
import DefaultInput from '@/components/DefaultInput.vue';
import DefaultButton from '@/components/DefaultButton.vue';
import MainTitle from '@/components/MainTitle.vue'
import { storeToRefs } from 'pinia';
import { Search, RotateCcw, X } from 'lucide-vue-next';
import { useBookingStore } from '@/features/booking/stores/booking.store';
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store';
import { useAuthStore } from '@/features/auth/stores/auth.store';
import { useRole } from '@/composables/useRole';
import { useRoute } from 'vue-router';
import { computed } from 'vue';

const props = defineProps({
    isOpen: {
        type: Boolean,
        required: true
    }
});
const route = useRoute();
const { isAdmin } = useRole();
const emits = defineEmits(['close']);
const apartmanStore = useApartmanStore();
const { apartmans } = storeToRefs(apartmanStore);
const authStore = useAuthStore();
const bookingStore = useBookingStore();
const isCalendarView = computed(() => route.path === '/calendar');

const filterModel = reactive({
    name: '',
    email: '',
    startDate: '',
    endDate: '',
    roomId: '',
    apartmanId: '',
});

const nonCalendarFields = [
    { label: 'Név', model: 'name', type: 'text' },
    { label: 'Email', model: 'email', type: 'text' },
    { label: 'Érkezés dátuma', model: 'startDate', type: 'date' },
    { label: 'Távozás dátuma', model: 'endDate', type: 'date' },
];

const calendarFields = [
    { label: 'Név', model: 'name', type: 'text' },
    { label: 'Email', model: 'email', type: 'text' },
];

const selectedFields = computed(() =>
    isCalendarView.value ? calendarFields : nonCalendarFields
);

function closeFilter() {
    emits('close');
}

function getBaseFilters() {
    return {
        name: filterModel.name || '',
        email: filterModel.email || '',
        startDate: filterModel.startDate || '',
        endDate: filterModel.endDate || '',
        roomId: filterModel.roomId === '' ? null : Number(filterModel.roomId),
        apartmanId: filterModel.apartmanId === '' ? null : Number(filterModel.apartmanId),
    };
}

function getUserId() {
    return authStore.user?.id != null ? Number(authStore.user.id) : null;
}

function applyFilters() {
    const filters = getBaseFilters();

    if (isAdmin.value) {
        bookingStore.applyAdminFilters(filters);
        return;
    }

    bookingStore.applyUserFilters({
        ...filters,
        userId: getUserId(),
    });
}

function clearFilters() {
    Object.assign(filterModel, {
        name: '',
        email: '',
        startDate: '',
        endDate: '',
        roomId: '',
        apartmanId: '',
    });

    const emptyFilters = {
        name: '',
        email: '',
        startDate: '',
        endDate: '',
        roomId: null,
        apartmanId: null,
    };

    if (isAdmin.value) {
        bookingStore.applyAdminFilters(emptyFilters);
        return;
    }

    bookingStore.applyUserFilters({
        ...emptyFilters,
        userId: getUserId(),
    });
}

onMounted(async () => {
    if (!apartmanStore.apartmans.length) {
        await apartmanStore.getAll();
    }
})
</script>

<template>
    <div :class="[
        'filters p-5 bg-white rounded-xl shadow-2xl border border-gray-100 absolute top-16 right-0 flex flex-col justify-start items-start z-50',
        isCalendarView ? 'w-auto' : 'w-auto'
    ]">
        <button type="button" class="absolute top-2 right-2 text-gray-500 hover:text-gray-700 transition"
            @click="closeFilter">
            <X class="h-5 w-5" />
        </button>

        <MainTitle title="Szűrő" barColor="#fbcfc4" class="mb-4" />

        <div class="grid md:grid-cols-2 grid-cols-2 gap-4 ">
            <DefaultInput v-for="(field, index) in selectedFields" :key="index" :label-text="field.label"
                :type="field.type" v-model="filterModel[field.model]" />
            <DefaultInput label-text="Apartmanok" type="select" v-model="filterModel.apartmanId">
                <option value="">Összes</option>
                <option v-for="apartman in apartmans" :key="apartman.id" :value="apartman.id">
                    {{ apartman.name }}
                </option>
            </DefaultInput>

            <div class="flex items-end gap-2">
                <DefaultButton @click="applyFilters" text="Szűrés" :icon="Search"
                    button-class="flex-1 bg-blue-600 hover:bg-blue-700 text-white rounded-lg py-2 text-sm font-medium transition flex items-center justify-center gap-2" />
                <DefaultButton @click="clearFilters" :icon="RotateCcw"
                    button-class="p-2 border border-gray-200 rounded-lg hover:bg-gray-50 transition" />
            </div>
        </div>
    </div>
</template>