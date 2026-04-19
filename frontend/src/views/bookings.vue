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
import { useRoute } from 'vue-router'

const bookingStore = useBookingStore()
const { isAdmin } = useRole()
const route = useRoute()
const showCreateModal = ref(false)
const showFilters = ref(false)
const selectedBooking = ref(null)
const showModifyModal = ref(false)

const bookingCount = computed(() => bookingStore.bookings.count)
const createdThisMont = computed(() => bookingStore.bookings.reservationsCreatedThisMonth)
const createdToday = computed(() => bookingStore.bookings.reservationsCreatedToday)
const canGoNext = computed(() => bookingStore.bookings.reservations?.length >= 10)
const canGoPrev = computed(() => bookingStore.pagination.page > 1)

const statCardContent = [
    { title: 'Összes foglalás', content: bookingCount, icon: GalleryHorizontalEnd, additional: 'Összesen', bgColor: '#f3fbff', iconBgColor: '#c8f1fb' },
    { title: 'A hónapban beérkező', content: createdThisMont, icon: Check, additional: 'Teljesített', bgColor: '#fef5f8', iconBgColor: '#fbc3d7' },
    { title: 'Ma beérkezők', content: createdToday, icon: ClockPlus, additional: 'Várható', bgColor: '#fef3ff', iconBgColor: '#fbcffd' },
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

onMounted(async () => {
    if (isAdmin.value) {
        await bookingStore.getAllAdmin()
    } else {
        await bookingStore.getAllUser()
    }

    const openBookingId = route.query.openBooking
    if (openBookingId) {
        const booking = bookingStore.bookings.reservations?.find(b => b.id === Number(openBookingId))
        if (booking) {
            openModifyModal(booking)
        }
    }
})
</script>

<template>
    <div class="relative w-full space-y-6">

        <section class="overview-section">
            <MainTitle title="Foglalások áttekintése" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :icon="card.icon" :content="card.content" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </section>

        <section class="relative flex flex-col md:flex-row md:items-center justify-between gap-3">
            <MainTitle title="Foglalások" barColor="#c8f1fb" />

            <div class="flex flex-row flex-wrap items-center justify-end gap-2 w-full">
                <DefaultButton @click="openCreateModal" :text="'Foglalás hozzáadása'" :icon="Plus"
                    :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg md:w-auto w-full transition duration-100'" />

                <span v-if="bookingStore.bookings.reservations?.length > 0"
                    class="flex items-center gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow ring-1 md:w-auto w-full bg-green-100 ring-green-300 text-black font-medium">
                    <span class="font-base">Találatok:</span> {{ bookingStore.bookings.reservations?.length }} foglalás
                </span>
                <span v-else
                    class="flex items-center gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow bg-gray-100 text-black font-medium">
                    Nincsenek találatok
                </span>

                <DefaultButton @click="openFilters" :icon="SlidersHorizontal"
                    :button-class="`${showFilters ? 'bg-gray-200' : 'bg-white hover:bg-gray-200'} text-black shadow rounded-lg transition duration-100`" />
            </div>

            <Transition name="fade-in">
                <BookingFiltersBar v-if="showFilters" :show="showFilters" @close="closeFilters"
                    :is-open="showFilters" />
            </Transition>
        </section>

        <section v-if="!bookingStore.bookings.reservations?.length"
            class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
            <GalleryHorizontalEnd class="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p class="text-gray-500 font-medium">Jelenleg nincsenek megjeleníthető foglalások.</p>
        </section>
        <section v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <BookingCard v-for="booking in bookingStore.bookings.reservations" :key="booking.id" :booking="booking"
                @openModifyModal="openModifyModal" @close="closeModifyModal" />
        </section>

        <div class="flex items-center justify-center gap-x-4 pb-10">
            <DefaultButton @click="canGoPrev && bookingStore.goToPage(bookingStore.pagination.page - 1)"
                :icon="ChevronLeft"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoPrev ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />
            <span class="text-md font-semibold text-gray-700 px-1 py-2 rounded-full">
                {{ bookingStore.pagination.page }}. oldal
            </span>
            <DefaultButton @click="canGoNext && bookingStore.goToPage(bookingStore.pagination.page + 1)"
                :icon="ChevronRight"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoNext ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />
        </div>

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
