<script setup>
import { reactive } from 'vue'
import { useUserStore } from '@features/users/stores/user.store'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: {
        type: Boolean,
        required: true
    }
})

const userStore = useUserStore()

const formData = reactive({
    username: '',
    email: '',
    password: '',
    role: 'User'
})

const resetForm = () => {
    formData.username = ''
    formData.email = ''
    formData.password = ''
    formData.role = 'User'

}

const createUser = async () => {
    const payload = {
        userName: formData.username,
        userEmail: formData.email,
        password: formData.password,
        role: formData.role
    }

    await userStore.create(payload)
    resetForm()
    emit('close')
}

const handleClose = () => {
    resetForm()
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
                    <h2 class="text-lg font-semibold mb-4">Felhasználó létrehozása</h2>

                    <form @submit.prevent="createUser">
                        <div class="form-group mb-3">
                            <label for="username" class="block text-sm font-medium mb-1">
                                Felhasználónév:
                            </label>
                            <input v-model="formData.username" type="text" id="username"
                                class="w-full border rounded px-3 py-2 text-sm" required />
                        </div>

                        <div class="form-group mb-3">
                            <label for="email" class="block text-sm font-medium mb-1">
                                Email:
                            </label>
                            <input v-model="formData.email" type="email" id="email"
                                class="w-full border rounded px-3 py-2 text-sm" required />
                        </div>

                        <div class="form-group mb-5">
                            <label for="password" class="block text-sm font-medium mb-1">
                                Jelszó:
                            </label>
                            <input v-model="formData.password" type="password" id="password"
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
                                Létrehozás
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>