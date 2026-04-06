<script setup>
import { ref, onMounted, computed } from 'vue'
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
import RoomCard from '@/features/rooms/components/RoomCard.vue'
import RoomFiltersModal from '@/features/rooms/components/RoomFiltersModal.vue'
import { useRoomStore } from '@/features/rooms/stores/room.store'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import { useRole } from '@/composables/useRole'

const roomStore = useRoomStore()
const apartmanStore = useApartmanStore()
const { isAdmin, isUser } = useRole()

const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedRoom = ref(null)
const showFilters = ref(false)
const selectedApartmanId = ref('')

const userApartmanOptions = computed(() => apartmanStore.apartmans ?? [])
const hasSelectedApartman = computed(() => selectedApartmanId.value !== '')

const statCardContent = computed(() => [
    { title: 'Ă–sszes szoba', content: String(roomStore.rooms.length), icon: GalleryHorizontalEnd, additional: 'Ă–sszesen', bgColor: '#f3fbff', iconBgColor: '#c8f1fb' },
    { title: 'Jelenleg aktĂ­v', content: '1', icon: Rss, additional: 'FoglalhatĂł', bgColor: '#fef5f8', iconBgColor: '#fbc3d7' },
    { title: 'Nem elĂ©rhetĹ‘k', content: '2', icon: CalendarX, additional: 'KarbantartĂˇs', bgColor: '#fff0ec', iconBgColor: '#fdd1c5' },
])

function openCreateModal() { showCreateModal.value = true }
function closeCreateModal() { showCreateModal.value = false }
function openFilters() { showFilters.value = !showFilters.value }
function closeFilters() { showFilters.value = false }

function openModifyModal(room) {
    showModifyModal.value = true
    selectedRoom.value = room
}

function closeModifyModal() {
    showModifyModal.value = false
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

    if (!admin && !user) return  // ← role helyett ezt használd

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
            <MainTitle title="ĂltalĂˇnos" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :content="card.content" :icon="card.icon" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </div>

        <div class="relative flex items-center justify-between">
            <div class="title">
                <MainTitle title="SzobĂˇk listĂˇja" barColor="#c8f1fb" />
            </div>
            <div class="buttons flex flex-row items-end gap-x-2">
                <div v-if="isUser" class="min-w-[260px]">
                    <DefaultInput input-name="apartmanSelect" label-text="Apartman" type="select"
                        v-model="selectedApartmanId" :options="userApartmanOptions"
                        @update:modelValue="loadRoomsForSelectedApartman" />
                </div>

                <DefaultButton v-if="isAdmin" @click="openCreateModal" :text="'Szoba hozzĂˇadĂˇsa'" :icon="Plus"
                    :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />

                <span v-if="roomStore.rooms.length > 0"
                    class="flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow ring-1 bg-green-100 ring-green-300 text-black font-medium">
                    <span class="font-base">TalĂˇlatok:</span> {{ roomStore.rooms.length }} szoba
                </span>
                <span v-else
                    class="flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow bg-gray-100 text-black font-medium">
                    Nincsenek talĂˇlatok
                </span>

                <DefaultButton @click="openFilters" :icon="SlidersHorizontal"
                    :button-class="`${showFilters ? 'bg-gray-200' : 'bg-white hover:bg-gray-200'} ml-2 text-black shadow rounded-lg transition duration-100`" />
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
                <p class="text-gray-500 font-medium">Jelenleg nincsenek megjelenĂ­thetĹ‘ szobĂˇk.</p>
            </div>

            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <RoomCard v-for="room in roomStore.rooms" :key="room.id" :room="room"
                    @openModifyModal="openModifyModal" />
            </div>
        </div>

        <div v-if="isAdmin" class="pagination flex items-center justify-center gap-x-4 mt-8 pb-10">
            <DefaultButton @click="roomStore.goToPage(roomStore.pagination.page - 1)" :icon="ChevronLeft"
                :buttonClass="'bg-white hover:bg-gray-100 text-black shadow-sm border border-gray-200 rounded-lg px-2'" />

            <span class="text-md font-semibold text-gray-700 px-1 py-2 rounded-full">
                {{ roomStore.pagination.page }}. oldal
            </span>

            <DefaultButton @click="roomStore.goToPage(roomStore.pagination.page + 1)" :icon="ChevronRight"
                :buttonClass="'bg-white hover:bg-gray-100 text-black shadow-sm border border-gray-200 rounded-lg px-2'" />
        </div>
    </div>

    <RoomCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <RoomModifyModal :showModal="showModifyModal" @close="closeModifyModal" :roomData="selectedRoom" />
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
