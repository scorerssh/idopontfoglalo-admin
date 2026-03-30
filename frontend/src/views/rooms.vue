<script setup>
import { CalendarX, Rss, GalleryHorizontalEnd, Plus, ChevronLeft, ChevronRight, Search, RotateCcw } from 'lucide-vue-next'
import MainTitle from '@shared/components/MainTitle.vue'
import DashboardStatCard from '@shared/components/DashboardStatCard.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import RoomCreateModal from '@/features/rooms/components/RoomCreateModal.vue'
import RoomModifyModal from '@/features/rooms/components/RoomModifyModal.vue'
import RoomCard from '@/features/rooms/components/RoomCard.vue' // Az új child
import { useRoomStore } from '@/features/rooms/stores/room.store'
import { ref, onMounted, reactive, computed } from 'vue'

const roomStore = useRoomStore()
const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedRoom = ref(null)

const statCardContent = computed(() => [
    { title: 'Összes szoba', content: String(roomStore.rooms.length), icon: GalleryHorizontalEnd, additional: 'Összesen', bgColor: '#f3fbff', iconBgColor: '#c8f1fb' },
    { title: 'Jelenleg aktív', content: '1', icon: Rss, additional: 'Foglalható', bgColor: '#fef5f8', iconBgColor: '#fbc3d7' },
    { title: 'Nem elérhetők', content: '2', icon: CalendarX, additional: 'Karbantartás', bgColor: '#fff0ec', iconBgColor: '#fdd1c5' },
])

const filterModel = reactive({
    name: '',
    minCapacity: '',
    maxCapacity: '',
})

function openCreateModal() { showCreateModal.value = true }
function closeCreateModal() { showCreateModal.value = false }

function openModifyModal(room) {
    showModifyModal.value = true
    selectedRoom.value = room
}
function closeModifyModal() {
    showModifyModal.value = false
    selectedRoom.value = null
}

function applyRoomFilters() {
    roomStore.applyFilters({
        name: filterModel.name || '',
        minCapacity: filterModel.minCapacity === '' ? null : Number(filterModel.minCapacity),
        maxCapacity: filterModel.maxCapacity === '' ? null : Number(filterModel.maxCapacity),
    })
}

function clearRoomFilters() {
    filterModel.name = ''
    filterModel.minCapacity = ''
    filterModel.maxCapacity = ''
    roomStore.applyFilters({ name: '', minCapacity: null, maxCapacity: null })
}

onMounted(() => {
    roomStore.getAll()
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
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.1}s` }" />
            </TransitionGroup>
        </div>

        <div class="filters p-5 bg-white rounded-xl shadow-sm border border-gray-100">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div class="space-y-1">
                    <label class="text-xs font-bold text-gray-500 uppercase ml-1">Keresés</label>
                    <input v-model="filterModel.name" type="text" placeholder="Szoba neve..."
                        class="w-full border-gray-200 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 outline-none border transition-all" />
                </div>
                <div class="space-y-1">
                    <label class="text-xs font-bold text-gray-500 uppercase ml-1">Min. Kapacitás</label>
                    <input v-model="filterModel.minCapacity" type="number"
                        class="w-full border-gray-200 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 outline-none border transition-all" />
                </div>
                <div class="space-y-1">
                    <label class="text-xs font-bold text-gray-500 uppercase ml-1">Max. Kapacitás</label>
                    <input v-model="filterModel.maxCapacity" type="number"
                        class="w-full border-gray-200 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 outline-none border transition-all" />
                </div>
                <div class="flex items-end gap-2">
                    <button @click="applyRoomFilters"
                        class="flex-1 bg-blue-600 hover:bg-blue-700 text-white rounded-lg py-2 text-sm font-medium transition flex items-center justify-center gap-2">
                        <Search class="h-4 w-4" /> Szűrés
                    </button>
                    <button @click="clearRoomFilters"
                        class="p-2 border border-gray-200 rounded-lg hover:bg-gray-50 transition">
                        <RotateCcw class="h-5 w-5 text-gray-500" />
                    </button>
                </div>
            </div>
        </div>

        <div class="flex items-center justify-between">
            <MainTitle title="Szobák listája" barColor="#c8f1fb" />
            <DefaultButton @click="openCreateModal" :text="'Szoba hozzáadása'" :icon="Plus"
                :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />
        </div>

        <div class="room-list-container">
            <div v-if="roomStore.rooms.length === 0"
                class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
                <GalleryHorizontalEnd class="h-12 w-12 text-gray-300 mx-auto mb-3" />
                <p class="text-gray-500 font-medium">Jelenleg nincsenek megjeleníthető szobák.</p>
            </div>

            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <RoomCard v-for="room in roomStore.rooms" :key="room.id" :room="room"
                    @openModifyModal="openModifyModal" />
            </div>
        </div>

        <div class="pagination flex items-center justify-center gap-x-4 mt-8 pb-10">
            <DefaultButton @click="roomStore.goToPage(roomStore.pagination.page - 1)" :icon="ChevronLeft"
                :buttonClass="'bg-white hover:bg-gray-100 text-black shadow-sm border border-gray-200 rounded-lg px-4'" />

            <span class="text-sm font-semibold text-gray-700 bg-gray-100 px-4 py-2 rounded-full">
                {{ roomStore.pagination.page }}. oldal
            </span>

            <DefaultButton @click="roomStore.goToPage(roomStore.pagination.page + 1)" :icon="ChevronRight"
                :buttonClass="'bg-white hover:bg-gray-100 text-black shadow-sm border border-gray-200 rounded-lg px-4'" />
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
</style>