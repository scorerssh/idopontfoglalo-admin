<script setup>
import { useUserStore } from '@/features/users/stores/user.store'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import { UserPlus } from 'lucide-vue-next'

const props = defineProps({
    bindedUsersToApartman: { type: Array, default: () => [] },
    apartmanId: { type: Number, default: null }
})

const emit = defineEmits(['userAdded'])

const userStore = useUserStore()
const apartmanStore = useApartmanStore()
const selectedUsers = ref([])

onMounted(() => {
    if (props.apartmanId) {
        userStore.getAll({ notInApartmanId: props.apartmanId })
    }
})

const availableUsers = computed(() => userStore.users)

function toggleUserSelection(userId) {
    const index = selectedUsers.value.indexOf(userId)
    if (index > -1) {
        selectedUsers.value.splice(index, 1)
    } else {
        selectedUsers.value.push(userId)
    }
}

async function addSelectedUsers() {
    if (selectedUsers.value.length === 0) return

    await apartmanStore.update({
        id: props.apartmanId,
        users: {
            userIdsToAdd: selectedUsers.value
        }
    })
    selectedUsers.value = []
    emit('userAdded')
    userStore.getAll({ notInApartmanId: props.apartmanId })
}
</script>

<template>
    <div class="flex flex-col gap-y-1">
        <p class="text-xs text-black/50 mb-1">Elérhető felhasználók</p>

        <div v-if="availableUsers.length === 0" class="text-xs text-gray-400 py-2">
            Nincs elérhető felhasználó
        </div>

        <div v-else class="flex flex-col gap-y-2">
            <div v-for="user in availableUsers" :key="user.id"
                class="flex items-center justify-between px-3 py-2 rounded-lg shadow ">
                <div class="flex items-center gap-2">
                    <input type="checkbox" :value="user.id" v-model="selectedUsers"
                        class="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500" />
                    <div class="flex flex-col min-w-0">
                        <span class="text-sm font-medium text-gray-800 truncate">{{ user.userName }}</span>
                        <span class="text-xs text-gray-400 truncate">{{ user.userEmail }}</span>
                    </div>
                </div>
                <div class="flex items-center gap-2 shrink-0 ml-2">
                    <span :class="[
                        user.role === 'Admin' ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-600',
                        'text-xs px-2 py-0.5 rounded-full'
                    ]">
                        {{ user.role }}
                    </span>
                </div>
            </div>

            <div v-if="selectedUsers.length > 0" class="flex justify-end mt-2">
                <button @click="addSelectedUsers"
                    class="flex items-center gap-2 px-3 py-1 text-sm bg-green-500 text-white rounded hover:bg-green-600 transition-colors">
                    <UserPlus class="w-4 h-4" />
                    Hozzáadás ({{ selectedUsers.length }})
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped></style>