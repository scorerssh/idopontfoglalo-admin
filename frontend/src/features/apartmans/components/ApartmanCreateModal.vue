<script setup>
import { reactive } from 'vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import DefaultInput from '@/components/DefaultInput.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import MainTitle from '@/components/MainTitle.vue'

const emit = defineEmits(['close'])

const props = defineProps({
    showModal: {
        type: Boolean,
        required: true
    }
})

const apartmanStore = useApartmanStore()

const formData = reactive({
    name: '',
})

function resetForm() {
    formData.name = ''
}

const createApartman = async () => {
    const payload = {
        name: formData.name,
    }

    await apartmanStore.create(payload)
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
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md z-50 max-h-[90vh] overflow-y-auto">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <MainTitle title="Apartman létrehozása" barColor="#fbcfc4" class="mb-4" />
                    <form @submit.prevent="createApartman">
                        <div class="form-group mb-3">
                            <DefaultInput v-model="formData.name" label="Név" placeholder="Apartman neve"
                                labelText="Apartman neve:" labelClass="text-black/60" />
                        </div>

                        <div class="form-actions flex gap-2 justify-end">
                            <DefaultButton @click="resetForm" type="button" :text="'Mégse'"
                                :buttonClass="'bg-gray-300 hover:bg-gray-400 text-gray-700 rounded-lg transition duration-100'" />
                            <DefaultButton type="submit" :text="'Létrehozás'"
                                :buttonClass="'bg-[#275bf6] hover:bg-[#1a4ad5] text-white rounded-lg transition duration-100'" />
                        </div>
                    </form>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>