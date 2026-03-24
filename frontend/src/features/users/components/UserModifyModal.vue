<script setup>
import { reactive, watch } from 'vue'
import { useUserStore } from '@features/users/stores/user.store'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: { type: Boolean, required: true },
    userData: { type: Object, default: null }
})

const userStore = useUserStore()

const formData = reactive({
    userName: '',
    userEmail: '',
    role: 'User',
    id: null

})

watch(() => props.userData, (newUser) => {
    if (newUser) {
        formData.userName = newUser.userName ?? ''
        formData.userEmail = newUser.userEmail ?? ''
        formData.role = newUser.role ?? 'User'
        formData.id = props.userData.id ?? null
    }
}, { immediate: true })

const updateUser = async () => {
    await userStore.update(formData)
    emit('close')
}

const handleClose = () => {
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md z-50">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <h2 class="text-lg font-semibold mb-4">Felhasználó módosítása</h2>

                    <form @submit.prevent="updateUser">
                        <div class="form-group mb-3">
                            <label for="username" class="block text-sm font-medium mb-1">
                                Felhasználónév:
                            </label>
                            <input v-model="formData.userName" type="text" id="username"
                                class="w-full border rounded px-3 py-2 text-sm" required />
                        </div>

                        <div class="form-group mb-3">
                            <label for="email" class="block text-sm font-medium mb-1">
                                Email:
                            </label>
                            <input v-model="formData.userEmail" type="email" id="email"
                                class="w-full border rounded px-3 py-2 text-sm" required />
                        </div>
                        <div class="form-group mb-3">
                            <select v-model="formData.role" class="w-full border rounded px-3 py-2 text-sm">
                                <option value="User">User</option>
                                <option value="Admin">Admin</option>
                            </select>
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <button type="button" class="px-4 py-2 text-sm rounded border hover:bg-gray-100"
                                @click="handleClose">
                                Mégse
                            </button>
                            <button type="submit"
                                class="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700">
                                Mentés
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>