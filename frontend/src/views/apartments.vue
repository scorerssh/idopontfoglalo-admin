<script setup>
import { Plus, ChevronLeft, ChevronRight, Rss, CalendarX, GalleryHorizontalEnd } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue'
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
    const res = await apartmanStore.getWithRooms(id)
    selectedApartman.value = res
    showModifyModal.value = true  // ← csak akkor nyílik ki, ha már van adat
}

function closeModifyModal() {
    showModifyModal.value = false
    selectedApartman.value = null
}

const statCardContent = computed(() => [
    {
        title: 'Összes apartman',
        text: apartmanStore.apartmans.length.toString(),
        icon: GalleryHorizontalEnd, additional: 'asdf', bgColor: '#f3fbff', iconBgColor: '#c8f1fb',
    },
    {
        title: 'Elérhető',
        text: apartmanStore.apartmans.filter(a => a.isAvailable).length.toString(),
        icon: Rss, additional: 'asdf', bgColor: '#fff0ec', iconBgColor: '#fdd1c5',
    },
    {
        title: 'Nem elérhető',
        text: apartmanStore.apartmans.filter(a => !a.isAvailable).length.toString(),
        icon: CalendarX, additional: 'asdf', bgColor: '#fef5f8', iconBgColor: '#fbc3d7'
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
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :content="card.text" :additional="card.additional" :icon="card.icon" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
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

        <div class="pagination flex flex-row gap-x-2 justify-center">
            <DefaultButton @click="apartmanStore.goToPage(apartmanStore.pagination.page - 1)" :icon="ChevronLeft"
                :buttonClass="'mt-4 bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
            <DefaultButton @click="apartmanStore.goToPage(apartmanStore.pagination.page + 1)" :icon="ChevronRight"
                :buttonClass="'mt-4 bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
        </div>
    </div>

    <ApartmanCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <ApartmanModifyModal :showModal="showModifyModal" @close="closeModifyModal"
        @apartmanUpdated="selectedApartman = $event" :apartmanData="selectedApartman" />
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