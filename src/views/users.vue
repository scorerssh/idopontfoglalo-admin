<script setup>
import MainTitle from '@shared/components/MainTitle.vue'
import DashboardStatCard from '@shared/components/DashboardStatCard.vue'
import MainButton from '@components/MainButton.vue'
import UserCreateModal from '@features/users/components/UserCreateModal.vue'
import UserModifyModal from '@features/users/components/UserModifyModal.vue'
import { useUserStore } from '@/features/users/stores/user.store'
import { ref, onMounted } from 'vue'

const userStore = useUserStore()
const showCreateModal = ref(false)
const showModifyModal = ref(false)
const selectedUser = ref(null)

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
    { title: 'Összes felhasználó', icon: '', text: '', additional: '' },
    { title: 'Aktív felhasználók', icon: '', text: '', additional: '' },
    { title: 'A hónapban létrehozottak', icon: '', text: '', additional: '' },
]

onMounted(() => {
    userStore.getAll()
})
</script>

<template>
    <div>
        <div class="top">
            <MainTitle title="Felhasználók" barColor="#fbcfc4" />
            <div class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 pt-4 w-full">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                    :icon="card.icon" :text="card.text" :additional="card.additional" />
            </div>
        </div>

        <div class="title-and-actions flex items-center justify-between mt-6">
            <span class="font-semibold text-base">Felhasználók</span>
            <span class="actions">
                <MainButton @click="openCreateModal" :text="'Felhasználó hozzáadása'" />
            </span>
        </div>
        <div class="user-list w-full">
            <ul class="flex flex-col gap-y-2">
                <li class="flex flex-row border-b border-gray-200" v-for="user in userStore.users" :key="user.id">
                    <span>{{ user.userName }}</span>
                    <span>{{ user.userEmail }}</span>
                    <span>{{ user.role }}</span>
                    <button @click="openModifyModal(user)">Módosítás</button>
                </li>
            </ul>
        </div>
        <button @click="userStore.goToPage(userStore.pagination.page - 1)">
            Előző oldal
        </button>
        <button @click="userStore.goToPage(userStore.pagination.page + 1)">
            Következő oldal
        </button>
    </div>

    <UserCreateModal :showModal="showCreateModal" @close="closeCreateModal" />
    <UserModifyModal :showModal="showModifyModal" @close="closeModifyModal" :userData="selectedUser" />
</template>

<style scoped></style>