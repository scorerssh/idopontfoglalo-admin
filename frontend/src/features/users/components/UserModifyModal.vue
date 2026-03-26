<script setup>
import { reactive, watch } from 'vue'
import { useUserStore } from '@features/users/stores/user.store'
import MainTitle from '@/shared/components/MainTitle.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'

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

const inputs = [
    { name: 'userName', label: 'Felhasználónév', labelClass: 'mt-0 text-black/60' },
    { name: 'userEmail', label: 'Email', labelClass: 'mt-0 text-black/60' },
]

const actions = [
    { text: 'Mégse', action: 'cancel', buttonClass: 'px-4 py-2 text-sm rounded bg-gray-300 hover:bg-gray-400' },
    { text: 'Mentés', action: 'save', buttonClass: 'px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700' }
]

watch(() => props.userData, (newUser) => {
    if (newUser) {
        formData.userName = newUser.userName ?? ''
        formData.userEmail = newUser.userEmail ?? ''
        formData.role = newUser.role ?? 'User'
        formData.id = props.userData.id ?? null
    }
    console.log('User data updated in modal:', formData) // Debug log
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
                    <MainTitle bar-color="#fbcfc4" title="Felhasználó módosítása" class="mb-4" />

                    <form @submit.prevent="updateUser" class="flex flex-col gap-y-3">
                        <DefaultInput v-for="input in inputs" :key="input.name" v-model="formData[input.name]"
                            :label-text="input.label" :label-class="input.labelClass" />
                        <div>
                            <label class="text-black/60 text-sm">Szerep:</label>
                            <select v-model="formData.role"
                                class="px-3 py-2 w-full bg-gray-100 focus-within:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100 mt-1">
                                <option value="Admin">Admin</option>
                                <option value="User">User</option>
                            </select>
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <DefaultButton v-for="value in actions" :key="value.action" :text="value.text"
                                :buttonClass="value.buttonClass"
                                @click="value.action === 'cancel' ? handleClose() : updateUser()" />
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>