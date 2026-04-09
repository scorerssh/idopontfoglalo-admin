<script setup>
import { useUserStore } from '@features/users/stores/user.store'
import { userModifySchema } from '@features/users/schemas/userModify.schema'
import MainTitle from '@/components/MainTitle.vue'
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

const errors = reactive({
    userName: null,
    userEmail: null,
    role: null,
})

function resetErrors() {
    errors.userName = null
    errors.userEmail = null
    errors.role = null
}

const inputs = [
    { name: 'userName', label: 'Felhasználónév', labelClass: 'mt-0 text-black/60' },
    { name: 'userEmail', label: 'Email', labelClass: 'mt-0 text-black/60' },
]

watch(() => props.userData, (newUser) => {
    if (newUser) {
        formData.userName = newUser.userName ?? ''
        formData.userEmail = newUser.userEmail ?? ''
        formData.role = newUser.role ?? 'User'
        formData.id = props.userData.id ?? null
    }
    resetErrors()
}, { immediate: true })

const updateUser = async () => {
    resetErrors()
    const result = userModifySchema.safeParse({
        userName: formData.userName,
        userEmail: formData.userEmail,
        role: formData.role,
    })

    if (!result.success) {
        result.error.issues.forEach(err => {
            const field = err.path[0]
            errors[field] = err.message
        })
        return
    }

    await userStore.update(formData)
    emit('close')
}

const deleteUser = async () => {
    if (formData.id) {
        await userStore.delete(formData.id)
        emit('close')
    }
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
                        <div v-for="input in inputs" :key="input.name">
                            <DefaultInput v-model="formData[input.name]" :label-text="input.label"
                                :label-class="input.labelClass" />
                            <span v-if="errors[input.name]" class="text-red-500 text-xs mt-1 block">
                                {{ errors[input.name] }}
                            </span>
                        </div>

                        <div>
                            <label class="text-black/60 text-sm">Szerep:</label>
                            <select v-model="formData.role"
                                class="px-3 py-2 w-full bg-gray-100 focus-within:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100 mt-1">
                                <option value="Admin">Admin</option>
                                <option value="User">User</option>
                            </select>
                            <span v-if="errors.role" class="text-red-500 text-xs mt-1 block">
                                {{ errors.role }}
                            </span>
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <DefaultButton type="button" text="Törlés" @click="deleteUser"
                                button-class="px-4 py-2 text-sm rounded bg-red-600 text-white hover:bg-red-700">
                            </DefaultButton>
                            <DefaultButton type="button" text="Mégse" @click="handleClose"
                                button-class="px-4 py-2 text-sm rounded bg-gray-300 hover:bg-gray-400"></DefaultButton>
                            <DefaultButton type="submit" text="Mentés"
                                button-class="px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700">
                            </DefaultButton>
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>
