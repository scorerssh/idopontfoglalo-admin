<script setup>
import { reactive } from 'vue'
import { useUserStore } from '@features/users/stores/user.store'
import userCreateSchema from '../schemas/userCreate.schema'
import DefaultInput from '@components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import MainTitle from '@/shared/components/MainTitle.vue'

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

const errors = reactive({
    username: null,
    email: null,
    password: null,
    role: null
})

function resetErrors() {
    errors.username = null
    errors.email = null
    errors.password = null
    errors.role = null
}

function resetForm() {
    formData.username = ''
    formData.email = ''
    formData.password = ''
    formData.role = 'User'
    resetErrors()
}

async function createUser() {
    resetErrors()
    const result = userCreateSchema.safeParse({
        username: formData.username,
        email: formData.email,
        password: formData.password,
        role: formData.role
    })

    console.log('result:', result)          // ← add hozzá ezt
    console.log('result.error:', result.error)

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            errors[field] = err.message
        })
        return
    }

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



function handleClose() {
    resetForm()
    emit('close')
}

const formInputs = [
    { inputName: 'username', labelText: 'Felhasználónév:', type: 'text' },
    { inputName: 'email', labelText: 'Email:', type: 'email' },
    { inputName: 'password', labelText: 'Jelszó:', type: 'password' },
]
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
                    <MainTitle title="Új felhasználó létrehozása" barColor="#fbcfc4" :titleClass="'text-lg'" />
                    <form @submit.prevent="createUser" class="mt-5">
                        <div class="form-inputs flex flex-col gap-4 mb-6">

                            <!-- ✅ v-for csak szöveges inputokra -->
                            <div v-for="input in formInputs" :key="input.inputName">
                                <DefaultInput v-model="formData[input.inputName]" :inputName="input.inputName"
                                    :labelText="input.labelText" :type="input.type" :labelClass="'text-black/60'" />
                                <span v-if="errors[input.inputName]" class="text-red-500 text-xs mt-1">
                                    {{ errors[input.inputName] }}
                                </span>
                            </div>

                            <!-- ✅ Select külön kezelve, mert más HTML elem -->
                            <div>
                                <label class="text-black/60 text-sm">Szerep:</label>
                                <select v-model="formData.role"
                                    class="px-3 py-2 w-full bg-gray-100 focus-within:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100 mt-1">
                                    <option value="Admin">Admin</option>
                                    <option value="User">User</option>
                                </select>
                                <span v-if="errors.role" class="text-red-500 text-xs mt-1">
                                    {{ errors.role }}
                                </span>
                            </div>

                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <DefaultButton text="Mégse" type="button" @click="handleClose"
                                buttonClass="bg-gray-300 text-gray-700 hover:bg-gray-400" />
                            <DefaultButton text="Létrehozás" type="submit"
                                buttonClass="bg-blue-500 text-white hover:bg-blue-600" />
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>