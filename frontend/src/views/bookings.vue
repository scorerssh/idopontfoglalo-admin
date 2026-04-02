<script setup>
import { GalleryHorizontalEnd, Check, ClockPlus, Plus, ChevronLeft, ChevronRight, SlidersHorizontal } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import BookingCard from '@/features/booking/components/BookingCard.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue'
import DefaultButton from '@components/DefaultButton.vue'
import BookingCreateModal from '@/features/booking/components/BookingCreateModal.vue'
import BookingFiltersBar from '@/features/booking/components/BookingFiltersBar.vue'
import BookingModifyModal from '@/features/booking/components/BookingModifyModal.vue'
import { useBookingStore } from '@/features/booking/stores/booking.store'
import { useRole } from '@/composables/useRole'
import { ref, onMounted, computed } from 'vue'

const bookingStore = useBookingStore()
const { isAdmin } = useRole()
const showCreateModal = ref(false)
const showFilters = ref(false)
const selectedBooking = ref(null)
const showModifyModal = ref(false)


const bookingCount = computed(() => bookingStore.bookings.length)

const statCardContent = [
    { title: 'Összes foglalás', content: bookingCount, icon: GalleryHorizontalEnd, additional: 'asdf', bgColor: '#f3fbff', iconBgColor: '#c8f1fb' },
    { title: 'Teljesített foglalások', content: '15', icon: Check, additional: 'asdf', bgColor: '#fef5f8', iconBgColor: '#fbc3d7' },
    { title: 'A hónapban beérkezők', content: '42', icon: ClockPlus, additional: 'asdf', bgColor: '#fef3ff', iconBgColor: '#fbcffd' },
]

function openCreateModal() { showCreateModal.value = true }
function closeCreateModal() { showCreateModal.value = false }
function openFilters() { showFilters.value = true }
function closeFilters() { showFilters.value = false }

function openModifyModal(booking) {
    selectedBooking.value = booking
    showModifyModal.value = true
}
function closeModifyModal() {
    showModifyModal.value = false
    selectedBooking.value = null
}

onMounted(() => {
    if (isAdmin.value) {
        bookingStore.getAllAdmin()
    } else {
        bookingStore.getAllUser()
    }
})

</script>

<template>
    <div class="relative w-full">

        <section class="overview-section">
            <MainTitle title="Foglalások áttekintése" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :icon="card.icon" :content="card.content" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </section>

        <section class="relative my-6 w-full flex flex-row items-center justify-between">
            <div class="flex items-center justify-between">
                <MainTitle title="Foglalások" barColor="#c8f1fb" />
            </div>
            <div class="flex flex-row gap-x-2 justify-center">
                <DefaultButton @click="bookingStore.goToPage(bookingStore.pagination.page - 1)" :icon="ChevronLeft"
                    :buttonClass="'mt-4 bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
                <DefaultButton @click="bookingStore.goToPage(bookingStore.pagination.page + 1)" :icon="ChevronRight"
                    :buttonClass="'mt-4 bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
            </div>
            <span class="flex flex-row items-center gap-x-2">
                <DefaultButton @click="openCreateModal" :text="'Foglalás hozzáadása'" :icon="Plus"
                    :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />
                <div class="filter-results">
                    <span v-if="bookingStore.bookings.length > 0"
                        class="flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow ring-1 bg-green-100 ring-green-300 text-black font-medium">
                        <span class="font-base">Találatok:</span> {{ bookingStore.bookings.length }} foglalás
                    </span>
                    <span v-else
                        class="flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow bg-gray-100 text-black font-medium">
                        Nincsenek találatok
                    </span>
                </div>
                <DefaultButton @click="openFilters" :icon="SlidersHorizontal"
                    :button-class="`${showFilters ? 'bg-gray-200' : 'bg-white hover:bg-gray-200'} ml-2 text-black shadow rounded-lg transition duration-100`" />
            </span>
            <Transition name="fade-in">
                <BookingFiltersBar v-if="showFilters" :show="showFilters" @close="closeFilters"
                    :is-open="showFilters" />
            </Transition>
        </section>

        <section class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <BookingCard v-for="booking in bookingStore.bookings" :key="booking.startTIme + booking.name"
                :booking="booking" @openModifyModal="openModifyModal" />
        </section>

        <BookingCreateModal v-if="showCreateModal" :showModal="showCreateModal" @close="closeCreateModal" />
        <BookingModifyModal v-if="showModifyModal" :showModal="showModifyModal" :booking="selectedBooking"
            @close="closeModifyModal" />

    </div>
</template>

<style scoped>
@keyframes slideUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.card-enter-active {
    animation: slideUp 0.4s ease both;
}

.card-enter-from {
    opacity: 0;
}

.fade-in-enter-active,
.fade-in-leave-active {
    transition: all 0.2s ease;
}

.fade-in-enter-from,
.fade-in-leave-to {
    transform: translateY(-15px);
    opacity: 0;
}

.fade-in-enter-to,
.fade-in-leave-from {
    transform: translateY(0px);
    opacity: 1;
}
</style>