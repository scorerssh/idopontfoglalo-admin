<script setup>
import MainTitle from '@shared/components/MainTitle.vue'
import DashboardStatCard from '@shared/components/DashboardStatCard.vue'
import MainButton from '@components/MainButton.vue'
import ApartmanCreateModal from '@/features/apartmans/components/ApartmanCreateModal.vue'
import ApartmanModifyModal from '@/features/apartmans/components/ApartmanModifyModal.vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import { ref, onMounted, computed } from 'vue'

const apartmanStore = useApartmanStore()
const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedApartman = ref(null)

function openCreateModal() {
    showCreateModal.value = true
}

function closeCreateModal() {
    showCreateModal.value = false
}

function openModifyModal(apartman) {
    showModifyModal.value = true
    selectedApartman.value = apartman
}

function closeModifyModal() {
    showModifyModal.value = false
    selectedApartman.value = null
}

const statCardContent = computed(() => [
    {
        title: 'Összes apartman',
        text: apartmanStore.apartmans.length.toString(),
        additional: ''
    },
    {
        title: 'Elérhető',
        text: apartmanStore.apartmans.filter(a => a.isAvailable).length.toString(),
        additional: ''
    },
    {
        title: 'Nem elérhető',
        text: apartmanStore.apartmans.filter(a => !a.isAvailable).length.toString(),
        additional: ''
    },
])

onMounted(() => {
    apartmanStore.getAll()
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

        <div class="title-and-actions flex items-center justify-between mt-6">
            <span class="font-semibold text-base">Apartmanok listája</span>
            <span class="actions">
                <MainButton @click="openCreateModal" :text="'Apartman hozzáadása'" />
            </span>
        </div>

        <div class="apartman-list w-full mt-4">
            <div v-if="apartmanStore.apartmans.length === 0" class="text-center py-8 text-gray-500">
                Nincsenek apartmanok
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
                        <tr v-for="apartman in apartmanStore.apartmans" :key="apartman.id"
                            class="border-b border-gray-200 hover:bg-gray-50">
                            <td class="px-4 py-3">{{ apartman.name || apartman.name || '-' }}</td>
                            <td class="px-4 py-3 text-gray-600">{{ apartman.description || '-' }}</td>
                            <td class="px-4 py-3 text-right font-medium">{{ apartman.price?.toLocaleString('hu-HU') ||
                                '-' }}</td>
                            <td class="px-4 py-3 text-center">{{ apartman.roomCount || '-' }}</td>
                            <td class="px-4 py-3 text-center">
                                <span :class="[
                                    'px-3 py-1 rounded-full text-sm font-medium',
                                    apartman.isAvailable
                                        ? 'bg-green-100 text-green-700'
                                        : 'bg-red-100 text-red-700'
                                ]">
                                    {{ apartman.isAvailable ? 'Elérhető' : 'Nem elérhető' }}
                                </span>
                            </td>
                            <td class="px-4 py-3 text-center">
                                <button @click="openModifyModal(apartman)"
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
            <button @click="apartmanStore.goToPage(apartmanStore.pagination.page - 1)"
                :disabled="apartmanStore.pagination.page === 1"
                class="px-4 py-2 border rounded hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed">
                Előző oldal
            </button>
            <span class="px-4 py-2 text-gray-600">
                Oldal {{ apartmanStore.pagination.page }}
            </span>
            <button @click="apartmanStore.goToPage(apartmanStore.pagination.page + 1)"
                class="px-4 py-2 border rounded hover:bg-gray-100">
                Következő oldal
            </button>
        </div>
    </div>

    <ApartmanCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <ApartmanModifyModal :showModal="showModifyModal" @close="closeModifyModal" :apartmanData="selectedApartman" />
</template>

<style scoped></style>