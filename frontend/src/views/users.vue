<script setup>
import { GalleryHorizontalEnd, GlobeOff, Rss, ClockPlus, Plus, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue'
import DefaultButton from '@components/DefaultButton.vue'
import UserCreateModal from '@features/users/components/UserCreateModal.vue'
import UserModifyModal from '@features/users/components/UserModifyModal.vue'
import UserCard from '@features/users/components/UserCard.vue'
import { useUserStore } from '@/features/users/stores/user.store'

const userStore = useUserStore()
const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedUser = ref(null)

const contactLenght = computed(() => userStore.users.length)
const canGoNext = computed(() => userStore.users.length >= 10)
const canGoPrev = computed(() => userStore.pagination.page > 1)

function openCreateModal() {
    showCreateModal.value = true
}
function closeCreateModal() {
    showCreateModal.value = false
}
function openModifyModal(user) {
    showModifyModal.value = true
    selectedUser.value = user
}
function closeModifyModal() {
    showModifyModal.value = false
    selectedUser.value = null
}

const statCardContent = [
    { title: 'Összes felhasználó', content: contactLenght, icon: GalleryHorizontalEnd, additional: 'Összesen', bgColor: '#f3fbff', iconBgColor: '#c8f1fb', },
    { title: 'Aktív felhasználók', content: '15', icon: Rss, additional: 'Aktív', bgColor: '#fef5f8', iconBgColor: '#fbc3d7', },
    { title: 'Inaktív felhasználók', content: '8', icon: GlobeOff, additional: 'Inaktív', bgColor: '#fff0ec', iconBgColor: '#fdd1c5', },
    { title: 'A hónapban létrehozottak', content: '42', icon: ClockPlus, additional: 'Új', bgColor: '#fef3ff', iconBgColor: '#fbcffd', },
]

onMounted(() => {
    userStore.getAll()
})
</script>

<template>
    <div class="space-y-6">
        <div class="top">
            <MainTitle title="Felhasználók áttekintése" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :icon="card.icon" :content="card.content" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </div>

        <div class="flex flex-col md:flex-row md:items-center justify-between gap-3">
            <MainTitle title="Felhasználók" barColor="#c8f1fb" />
            <DefaultButton @click="openCreateModal" :text="'Felhasználó hozzáadása'" :icon="Plus"
                buttonClass="bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100 w-full md:w-auto" />
        </div>

        <div v-if="userStore.users.length === 0"
            class="text-center py-20 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
            <GalleryHorizontalEnd class="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p class="text-gray-500 font-medium">Jelenleg nincsenek megjeleníthető felhasználók.</p>
        </div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <UserCard v-for="user in userStore.users" :key="user.id" :user="user" @openModifyModal="openModifyModal" />
        </div>

        <div class="pagination flex items-center justify-center gap-x-4 mt-8 pb-10">
            <DefaultButton
                @click="canGoPrev && userStore.goToPage(userStore.pagination.page - 1)"
                :icon="ChevronLeft"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoPrev ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />

            <span class="text-md font-semibold text-gray-700 px-1 py-2 rounded-full">
                {{ userStore.pagination.page }}. oldal
            </span>

            <DefaultButton
                @click="canGoNext && userStore.goToPage(userStore.pagination.page + 1)"
                :icon="ChevronRight"
                :buttonClass="`bg-white text-black shadow-sm border border-gray-200 rounded-lg px-2 ${!canGoNext ? 'opacity-40 pointer-events-none' : 'hover:bg-gray-100'}`" />
        </div>
    </div>

    <UserCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <UserModifyModal :showModal="showModifyModal" @close="closeModifyModal" :userData="selectedUser" />
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
