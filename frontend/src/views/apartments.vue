<script setup>
import { Plus, ChevronLeft, ChevronRight, Rss, CalendarX, GalleryHorizontalEnd } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import ApartmanCreateModal from '@/features/apartmans/components/ApartmanCreateModal.vue'
import ApartmanModifyModal from '@/features/apartmans/components/ApartmanModifyModal.vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import ApartmanCard from '@/features/apartmans/components/ApartmanCard.vue'


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
    showModifyModal.value = true
}

function closeModifyModal() {
    showModifyModal.value = false
    selectedApartman.value = null
}

const canGoNext = computed(() => apartmanStore.apartmans.length >= 10)
const canGoPrev = computed(() => apartmanStore.pagination.page > 1)

const statCardContent = computed(() => [
    {
        title: 'Összes apartman',
        text: apartmanStore.apartmans.length.toString(),
        icon: GalleryHorizontalEnd, additional: 'Összesen', bgColor: '#f3fbff', iconBgColor: '#c8f1fb',
    },
    {
        title: 'Elérhető',
        text: apartmanStore.apartmans.filter(a => a.isAvailable).length.toString(),
        icon: Rss, additional: 'Foglalható', bgColor: '#fff0ec', iconBgColor: '#fdd1c5',
    },
    {
        title: 'Nem elérhető',
        text: apartmanStore.apartmans.filter(a => !a.isAvailable).length.toString(),
        icon: CalendarX, additional: 'Karbantartás', bgColor: '#fef5f8', iconBgColor: '#fbc3d7'
    },
])

onMounted(() => {
    apartmanStore.getAll()
})
</script>

<template>
    <div class="space-y-6">
        <div class="top">
            <MainTitle title="Apartmanok" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :content="card.text" :additional="card.additional" :icon="card.icon" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </div>

        <div class="title-and-actions flex flex-col md:flex-row md:items-center justify-between gap-3">
            <MainTitle title="Apartmanok listája" barColor="#f4cbfe" />
            <DefaultButton @click="openCreateModal" :text="'Apartman hozzáadása'" :icon="Plus"
                :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100 w-full md:w-auto'" />
        </div>

        <div class="apartman-list w-full">
            <div v-if="apartmanStore.apartmans.length === 0"
                class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
                <GalleryHorizontalEnd class="h-12 w-12 text-gray-300 mx-auto mb-3" />
                <p class="text-gray-500 font-medium">Jelenleg nincsenek megjeleníthető apartmanok.</p>
            </div>
            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                <ApartmanCard v-for="apartman in apartmanStore.apartmans" :key="apartman.id" :id="apartman.id"
                    :name="apartman.name" :description="apartman.description" :price="apartman.price"
                    :roomCount="apartman.roomCount" :isAvailable="apartman.isAvailable"
                    @openModifyModal="handleOpenModifyModal" />
            </div>
        </div>

        <div class="pagination flex items-center justify-center gap-x-4 mt-8 pb-10">
            <DefaultButton
                @click="canGoPrev && apartmanStore.goToPage(apartmanStore.pagination.page - 1)"
                :icon="ChevronLeft"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoPrev ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />

            <span class="text-md font-semibold text-gray-700 px-1 py-2 rounded-full">
                {{ apartmanStore.pagination.page }}. oldal
            </span>

            <DefaultButton
                @click="canGoNext && apartmanStore.goToPage(apartmanStore.pagination.page + 1)"
                :icon="ChevronRight"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoNext ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />
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
