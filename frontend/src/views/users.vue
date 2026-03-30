<script setup>
import { GalleryHorizontalEnd, GlobeOff, Rss, ClockPlus, Plus, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import MainTitle from '@shared/components/MainTitle.vue'
import DashboardStatCard from '@shared/components/DashboardStatCard.vue'
import DefaultButton from '@components/DefaultButton.vue'
import UserCreateModal from '@features/users/components/UserCreateModal.vue'
import UserModifyModal from '@features/users/components/UserModifyModal.vue'
import UserCard from '@features/users/components/UserCard.vue'
import { useUserStore } from '@/features/users/stores/user.store'
import { ref, onMounted, computed } from 'vue'

const userStore = useUserStore()
const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedUser = ref(null)
const contactLenght = computed(() => {
    return userStore.users.length
})

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
    { title: 'Összes felhasználó', content: contactLenght, icon: GalleryHorizontalEnd, additional: 'asdf', bgColor: '#f3fbff', iconBgColor: '#c8f1fb', },
    { title: 'Aktív felhasználók', content: '15', icon: Rss, additional: 'asdf', bgColor: '#fef5f8', iconBgColor: '#fbc3d7', },
    { title: 'Inaktív felhasználók', content: '8', icon: GlobeOff, additional: 'asdf', bgColor: '#fff0ec', iconBgColor: '#fdd1c5', },
    { title: 'A hónapban létrehozottak', content: '42', icon: ClockPlus, additional: 'asdf', bgColor: '#fef3ff', iconBgColor: '#fbcffd', },
]

onMounted(() => {
    userStore.getAll()
})
</script>

<template>
    <div>
        <div class="top">
            <MainTitle title="Áttekintés" barColor="#fbcfc4" />
            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :icon="card.icon" :content="card.content" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :style="{ animationDelay: `${index * 0.2}s` }" />
            </TransitionGroup>
        </div>
        <div class="nav-and-titles my-6 w-full flex flex-row items-center justify-between">
            <div class="title-and-actions flex items-center justify-between ">
                <MainTitle title="Felhasználók" barColor="#c8f1fb" />

            </div>
            <div class="pagination flex flex-row gap-x-2 justify-center">
                <DefaultButton @click="userStore.goToPage(userStore.pagination.page - 1)" :icon="ChevronLeft"
                    :buttonClass="'mt-4 bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
                <DefaultButton @click="userStore.goToPage(userStore.pagination.page + 1)" :icon="ChevronRight"
                    :buttonClass="'mt-4 bg-white hover:bg-gray-200 text-black shadow rounded-lg transition duration-100'" />
            </div>
            <span class="actions">
                <DefaultButton @click="openCreateModal" :text="'Felhasználó hozzáadása'" :icon="Plus"
                    :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />
            </span>

        </div>
        <div class="user-list w-full h-full grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <UserCard v-for="user in userStore.users" :key="user.id" :user="user" @openModifyModal="openModifyModal" />
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