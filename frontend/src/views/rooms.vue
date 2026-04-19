<script setup>
import {
    CalendarX,
    Rss,
    GalleryHorizontalEnd,
    Plus,
    ChevronLeft,
    ChevronRight,
    SlidersHorizontal,
} from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import RoomCreateModal from '@/features/rooms/components/RoomCreateModal.vue'
import RoomModifyModal from '@/features/rooms/components/RoomModifyModal.vue'
import RoomPriceTiersModal from '@/features/rooms/components/RoomPriceTiersModal.vue'
import RoomCard from '@/features/rooms/components/RoomCard.vue'
import RoomFiltersModal from '@/features/rooms/components/RoomFiltersModal.vue'
import AgePriceTierModal from '@/features/rooms/components/AgePriceTierModal.vue'
import RoomSpecialPricingRulesModal from '@/features/rooms/components/RoomSpecialPricingRulesModal.vue'
import { useRoomStore } from '@/features/rooms/stores/room.store'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import { useRole } from '@/composables/useRole'

const roomStore = useRoomStore()
const apartmanStore = useApartmanStore()
const { isAdmin, isUser } = useRole()

const canGoNext = computed(() => roomStore.rooms.length >= 10)
const canGoPrev = computed(() => roomStore.pagination.page > 1)

const showCreateModal = ref(false)
const showModifyModal = ref(false)
const showAgePriceTierModal = ref(false)
const showPriceTiersModal = ref(false)
const showSpecialPricingRulesModal = ref(false)
const selectedRoom = ref(null)
const showFilters = ref(false)
const selectedApartmanId = ref('')

const userApartmanOptions = computed(() => apartmanStore.apartmans ?? [])
const hasSelectedApartman = computed(() => selectedApartmanId.value !== '')

const statCardContent = computed(() => [
    {
        title: 'Összes szoba',
        content: String(roomStore.rooms.length),
        icon: GalleryHorizontalEnd,
        additional: 'Összesen',
        bgColor: '#f3fbff',
        iconBgColor: '#c8f1fb',
    },
    {
        title: 'Jelenleg aktív',
        content: '1',
        icon: Rss,
        additional: 'Foglalható',
        bgColor: '#fef5f8',
        iconBgColor: '#fbc3d7',
    },
    {
        title: 'Nem elérhető',
        content: '2',
        icon: CalendarX,
        additional: 'Karbantartás',
        bgColor: '#fff0ec',
        iconBgColor: '#fdd1c5',
    },
])

function openCreateModal() {
    showCreateModal.value = true
}

function closeCreateModal() {
    showCreateModal.value = false
}

function openFilters() {
    showFilters.value = !showFilters.value
}

function closeFilters() {
    showFilters.value = false
}

function openModifyModal(room) {
    showModifyModal.value = true
    selectedRoom.value = room
}

function closeModifyModal() {
    showModifyModal.value = false
    selectedRoom.value = null
}

function openAgePriceTierModal(room) {
    showAgePriceTierModal.value = true
    selectedRoom.value = room
}

function closeAgePriceTierModal() {
    showAgePriceTierModal.value = false
    selectedRoom.value = null
}

function openPriceTiersModal(room) {
    showPriceTiersModal.value = true
    selectedRoom.value = room
}

function closePriceTiersModal() {
    showPriceTiersModal.value = false
    selectedRoom.value = null
}

function openSpecialPricingRulesModal(room) {
    showSpecialPricingRulesModal.value = true
    selectedRoom.value = room
}

function closeSpecialPricingRulesModal() {
    showSpecialPricingRulesModal.value = false
    selectedRoom.value = null
}

async function loadRoomsForSelectedApartman(value = selectedApartmanId.value) {
    selectedApartmanId.value = value
    const apartmanId = value === '' ? null : Number(value)

    if (!apartmanId) {
        roomStore.setRooms([], 'user')
        return
    }

    const apartman = await apartmanStore.getWithRooms(apartmanId)
    roomStore.setRooms(apartman?.rooms ?? [], 'user')
}

watchEffect(() => {
    const admin = isAdmin.value
    const user = isUser.value

    if (!admin && !user) return

    if (admin) {
        roomStore.getAll()
        return
    }

    if (user) {
        apartmanStore.getAllUser().then(() => {
            roomStore.setRooms([], 'user')
        })
    }
})
</script>

<template>
    <div class="space-y-6">
        <div class="top">
            <MainTitle title="Általános" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :content="card.content" :icon="card.icon" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </div>

        <div class="relative flex flex-col md:flex-row md:items-center justify-between gap-3">
            <div class="title">
                <MainTitle title="Szobák listája" barColor="#c8f1fb" />
            </div>
            <div class="buttons flex flex-row flex-wrap items-end gap-x-2 w-full md:w-auto">
                <div v-if="isUser" class="min-w-[260px]">
                    <DefaultInput input-name="apartmanSelect" label-text="Apartman" type="select"
                        v-model="selectedApartmanId" :options="userApartmanOptions"
                        @update:modelValue="loadRoomsForSelectedApartman" />
                </div>

                <DefaultButton v-if="isAdmin" @click="openCreateModal" :text="'Szoba hozzáadása'" :icon="Plus"
                    :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />

                <span v-if="roomStore.rooms.length > 0"
                    class="flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow ring-1 bg-green-100 ring-green-300 text-black font-medium md:w-auto w-full md:mt-0 mt-2">
                    <span class="font-base">Találatok:</span> {{ roomStore.rooms.length }} szoba
                </span>
                <span v-else
                    class="flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow bg-gray-100 text-black font-medium md:w-auto w-full md:mt-0 mt-2">
                    Nincsenek találatok
                </span>

                <DefaultButton @click="openFilters" :icon="SlidersHorizontal"
                    :button-class="`${showFilters ? 'bg-gray-200' : 'bg-white hover:bg-gray-200'} md:ml-2 ml-0 md:mt-0 mt-2 text-black shadow rounded-lg transition duration-100`" />
            </div>

            <Transition name="fade-in">
                <RoomFiltersModal v-if="showFilters" @close="closeFilters" />
            </Transition>
        </div>

        <div class="room-list-container">
            <div v-if="isUser && !hasSelectedApartman"
                class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
                <GalleryHorizontalEnd class="h-12 w-12 text-gray-300 mx-auto mb-3" />
                <p class="text-gray-500 font-medium">Válassz egy apartmant a szobák megjelenítéséhez.</p>
            </div>

            <div v-else-if="roomStore.rooms.length === 0"
                class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
                <GalleryHorizontalEnd class="h-12 w-12 text-gray-300 mx-auto mb-3" />
                <p class="text-gray-500 font-medium">Jelenleg nincsenek megjeleníthető szobák.</p>
            </div>

            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <RoomCard v-for="room in roomStore.rooms" :key="room.id" :room="room" @openModifyModal="openModifyModal"
                    @openAgePriceTierModal="openAgePriceTierModal" @openPriceTiersModal="openPriceTiersModal"
                    @openSpecialPricingRulesModal="openSpecialPricingRulesModal" />
            </div>
        </div>

        <div v-if="isAdmin" class="pagination flex items-center justify-center gap-x-4 mt-8 pb-10">
            <DefaultButton @click="canGoPrev && roomStore.goToPage(roomStore.pagination.page - 1)" :icon="ChevronLeft"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoPrev ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />

            <span class="text-md font-semibold text-gray-700 px-1 py-2 rounded-full">
                {{ roomStore.pagination.page }}. oldal
            </span>

            <DefaultButton @click="canGoNext && roomStore.goToPage(roomStore.pagination.page + 1)" :icon="ChevronRight"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoNext ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />
        </div>
    </div>

    <RoomCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <RoomModifyModal :showModal="showModifyModal" @close="closeModifyModal" :roomData="selectedRoom" />
    <AgePriceTierModal :showModal="showAgePriceTierModal" @close="closeAgePriceTierModal" :room="selectedRoom" />
    <RoomPriceTiersModal :showModal="showPriceTiersModal" @close="closePriceTiersModal" :roomData="selectedRoom" />
    <RoomSpecialPricingRulesModal :showModal="showSpecialPricingRulesModal" @close="closeSpecialPricingRulesModal"
        :room="selectedRoom" />
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
