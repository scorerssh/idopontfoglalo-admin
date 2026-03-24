<script setup>
import { Plus } from 'lucide-vue-next'
import MainTitle from '@shared/components/MainTitle.vue'
import DashboardStatCard from '@shared/components/DashboardStatCard.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import ApartmanCreateModal from '@/features/apartmans/components/ApartmanCreateModal.vue'
import ApartmanModifyModal from '@/features/apartmans/components/ApartmanModifyModal.vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import ApartmanCard from '@/features/apartmans/components/ApartmanCard.vue'

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

async function handleOpenModifyModal(id) {
    showModifyModal.value = true
    const res = await apartmanStore.getWithRooms(id)
    selectedApartman.value = res

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
                <DefaultButton @click="openCreateModal" :text="'Apartman hozzáadása'" :icon="Plus"
                    :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />
            </span>
        </div>

        <div class="apartman-list w-full mt-4">
            <div v-if="apartmanStore.apartmans.length === 0" class="text-center py-8 text-gray-500">
                Nincsenek apartmanok
            </div>
            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <ApartmanCard v-for="apartman in apartmanStore.apartmans" :key="apartman.id" :id="apartman.id"
                    :name="apartman.name" :description="apartman.description" :price="apartman.price"
                    :roomCount="apartman.roomCount" :isAvailable="apartman.isAvailable"
                    @openModifyModal="handleOpenModifyModal" />
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