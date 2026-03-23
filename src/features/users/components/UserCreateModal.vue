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

const resetErrors = () => {
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

const formInputs = [
    { inputName: 'username', labelText: 'Felahasználónév:', vModel: formData.username, type: 'text' },
    { inputName: 'email', labelText: 'Email:', vModel: formData.email, type: 'email' },
    { inputName: 'password', labelText: 'Jelszó:', vModel: formData.password, type: 'password' },
    { inputName: 'role', labelText: 'Szerep:', vModel: formData.role, type: 'select', options: ['Admin', 'User'] },
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
                    <MainTitle title="Új felhasználó létrehozása" barColor="#275bf6" :titleClass="'text-lg'" />
                    <form @submit.prevent="createUser" class="mt-5">
                        <div class="form-inputs flex flex-col gap-4 mb-6">
                            <DefaultInput v-for="(input, index) in formInputs" :key="index" :inputName="input.inputName"
                                :labelText="input.labelText" :type="input.type" v-model="input.vModel"
                                :labelClass="'text-black/60'" />
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <DefaultButton text="Mégse" type="button" @click="resetForm"
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

<style scoped></style>