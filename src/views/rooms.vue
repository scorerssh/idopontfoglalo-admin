<script setup>
import MainTitle from '@shared/components/MainTitle.vue'
import DashboardStatCard from '@shared/components/DashboardStatCard.vue'
import MainButton from '@components/MainButton.vue'
import RoomCreateModal from '@/features/rooms/components/RoomCreateModal.vue'
import RoomModifyModal from '@/features/rooms/components/RoomModifyModal.vue'
import { useRoomStore } from '@/features/rooms/stores/room.store'
import { ref, onMounted, reactive, computed } from 'vue'

const roomStore = useRoomStore()
const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedRoom = ref(null)

const statCardContent = computed(() => [
    { title: 'Összes szoba', text: String(roomStore.rooms.length), additional: '' },
    { title: 'Oldal', text: String(roomStore.pagination.page), additional: '' },
    { title: 'Szűrők', text: 'name/max/min', additional: '' },
])

const filterModel = reactive({
    name: '',
    minCapacity: '',
    maxCapacity: '',
})

function openCreateModal() {
    showCreateModal.value = true
}

function closeCreateModal() {
    showCreateModal.value = false
}

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
    <div>
        <div class="top">
            <MainTitle title="Apartmanok" barColor="#fbcfc4" />
            <div class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :text="card.text" :additional="card.additional" />
            </div>
        </div>

        <div class="filters mt-6 p-4 bg-white rounded shadow-sm border border-gray-200">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-3">
                <div>
                    <label class="block text-sm font-medium mb-1">Név</label>
                    <input v-model="filterModel.name" type="text" class="w-full border rounded px-3 py-2 text-sm" />
                </div>
                <div>
                    <label class="block text-sm font-medium mb-1">Min kapacitás</label>
                    <input v-model="filterModel.minCapacity" type="number" min="1"
                        class="w-full border rounded px-3 py-2 text-sm" />
                </div>
                <div>
                    <label class="block text-sm font-medium mb-1">Max kapacitás</label>
                    <input v-model="filterModel.maxCapacity" type="number" min="1"
                        class="w-full border rounded px-3 py-2 text-sm" />
                </div>
                <div class="flex items-end gap-2">
                    <button @click="applyRoomFilters" class="px-4 py-2 rounded bg-blue-600 text-white">Szűrés</button>
                    <button @click="clearRoomFilters" class="px-4 py-2 rounded border">Törlés</button>
                </div>
            </div>
        </div>

        <div class="title-and-actions flex items-center justify-between mt-6">
            <span class="font-semibold text-base">Szobák listája</span>
            <span class="actions">
                <MainButton @click="openCreateModal" :text="'Szoba hozzáadása'" />
            </span>
        </div>

        <div class="apartman-list w-full mt-4">
            <div v-if="roomStore.rooms.length === 0" class="text-center py-8 text-gray-500">
                Nincsenek szobák
            </div>
            <div v-else class="overflow-x-auto">
                <table class="w-full border-collapse">
                    <thead>
                        <tr class="bg-gray-100 border-b-2 border-gray-300">
                            <th class="text-left px-4 py-3 font-semibold">Cím</th>
                            <th class="text-left px-4 py-3 font-semibold">Leírás</th>
                            <th class="text-right px-4 py-3 font-semibold">Ár (HUF)</th>
                            <th class="text-center px-4 py-3 font-semibold">Szobák</th>
                            <th class="text-center px-4 py-3 font-semibold">Státusz</th>
                            <th class="text-center px-4 py-3 font-semibold">Műveletek</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="room in roomStore.rooms" :key="room.id"
                            class="border-b border-gray-200 hover:bg-gray-50">
                            <td class="px-4 py-3">{{ room.name || room.name || '-' }}</td>
                            <td class="px-4 py-3 text-gray-600">{{ room.description || '-' }}</td>
                            <td class="px-4 py-3 text-right font-medium">
                                {{ room.price?.toLocaleString('hu-HU') || '-' }}
                            </td>
                            <td class="px-4 py-3 text-center">{{ room.roomCount || '-' }}</td>
                            <td class="px-4 py-3 text-center">
                                <span :class="[
                                    'px-3 py-1 rounded-full text-sm font-medium',
                                    room.isAvailable
                                        ? 'bg-green-100 text-green-700'
                                        : 'bg-red-100 text-red-700'
                                ]">
                                    {{ room.isAvailable ? 'Elérhető' : 'Nem elérhető' }}
                                </span>
                            </td>
                            <td class="px-4 py-3 text-center">
                                <button @click="openModifyModal(room)"
                                    class="px-3 py-1 bg-blue-600 text-white rounded text-sm hover:bg-blue-700 transition">
                                    Módosítás
                                </button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="pagination flex justify-center gap-2 mt-6">
            <button @click="roomStore.goToPage(roomStore.pagination.page - 1)"
                :disabled="roomStore.pagination.page === 1"
                class="px-4 py-2 border rounded hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed">
                Előző oldal
            </button>
            <span class="px-4 py-2 text-gray-600">
                Oldal {{ roomStore.pagination.page }}
            </span>
            <button @click="roomStore.goToPage(roomStore.pagination.page + 1)"
                class="px-4 py-2 border rounded hover:bg-gray-100">
                Következő oldal
            </button>
        </div>
    </div>

    <RoomCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <RoomModifyModal :showModal="showModifyModal" @close="closeModifyModal" :roomData="selectedRoom" />
</template>

<style scoped></style>