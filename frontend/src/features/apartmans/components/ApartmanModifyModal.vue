<script setup>
import { Pencil, Plus } from 'lucide-vue-next'
import { reactive, watch, ref, computed } from 'vue'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'
import DefaultButton from '@/components/DefaultButton.vue'
import DefaultInput from '@/components/DefaultInput.vue'
import MainTitle from '@/shared/components/MainTitle.vue'
import BindedUsers from './BindedUsers.vue'
import AvailableUsers from './AvailableUsers.vue'

const emit = defineEmits(['close', 'apartmanUpdated'])

const props = defineProps({
    showModal: { type: Boolean, required: true },
    apartmanData: { type: Object, default: null }
})

const apartmanStore = useApartmanStore()

// --- Toggle logika ---
const currentUserList = ref('binded')

const activeComponent = computed(() =>
    currentUserList.value === 'binded' ? BindedUsers : AvailableUsers
)

function toggleUserComponent() {
    currentUserList.value = currentUserList.value === 'binded' ? 'available' : 'binded'
}

// --- Adatok ---
const bindedUsersToApartman = computed(() => props.apartmanData?.users)
const bindedRoomsToApartman = computed(() => props.apartmanData?.rooms)

const formData = reactive({
    id: null,
    name: '',
})

watch(() => props.apartmanData, (newApartman) => {
    if (newApartman) {
        formData.id = newApartman.id
        formData.name = newApartman.name ?? ''
    }
}, { immediate: true })

// --- Frissítés user változás után ---
// Újratölti az apartman adatait (users tömb frissül), hogy a BindedUsers mindig aktuális legyen
async function refreshApartman() {
    const newData = await apartmanStore.getWithRooms(props.apartmanData.id)
    emit('apartmanUpdated', newData)
}

// --- Akciók ---
const updateApartman = async () => {
    if (!formData.id) return

    const payload = {
        id: props.apartmanData.id,
        name: formData.name,
    }
    await apartmanStore.update(payload)
    emit('close')
}

const deleteApartman = async () => {
    if (!formData.id) return
    if (!confirm('Biztosan szeretnéd törölni ezt az apartmant?')) return

    await apartmanStore.delete(formData.id)
    emit('close')
}

const handleClose = () => {
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal && apartmanData">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[60%] z-50 h-[60vh] overflow-y-auto scrollable">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content flex flex-col gap-y-3 w-full justify-center">
                    <MainTitle title="Apartman módosítása" barColor="#fbcfc4" :class="'text-lg text-black mb-4'" />
                    <div class="list-container flex flex-row gap-x-4 w-full">

                        <!-- Bal oldal: form + toggle + felhasználók -->
                        <div class="users-list-and-title flex flex-col gap-y-3 w-1/2">
                            <form @submit.prevent="updateApartman">
                                <div class="form-group mb-3">
                                    <DefaultInput v-model="formData.name" label="Név" id="name" labelText="Név:"
                                        labelClass="text-sm text-black/60" required />
                                </div>

                                <div class="form-actions flex gap-2 justify-end">
                                    <DefaultButton @click="handleClose" :text="'Mégse'"
                                        :buttonClass="'px-4 py-2 text-sm rounded bg-gray-300 hover:bg-gray-400'" />
                                    <DefaultButton @click="updateApartman" :text="'Mentés'"
                                        :buttonClass="'px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700'" />
                                    <DefaultButton @click="deleteApartman" :text="'Törlés'"
                                        :buttonClass="'px-4 py-2 text-sm rounded border-red-300 text-white bg-red-500 hover:bg-red-600'" />
                                </div>
                            </form>

                            <div class="users-list flex flex-col gap-y-1">
                                <!-- Toggle switch -->
                                <div class="toggle-button relative p-0.5 bg-gray-200 h-7 w-13 rounded-full cursor-pointer"
                                    @click="toggleUserComponent">
                                    <div :style="{ left: currentUserList === 'binded' ? '2px' : '26px' }"
                                        class="circle absolute h-6 w-6 rounded-full bg-blue-500 flex items-center justify-center p-1 transition-all duration-200">
                                        <component :is="currentUserList === 'binded' ? Pencil : Plus"
                                            class="text-white pointer-events-none" />
                                    </div>
                                </div>

                                <!-- Dinamikus komponens -->
                                <component :is="activeComponent" :key="currentUserList"
                                    :bindedUsersToApartman="bindedUsersToApartman" :apartmanId="apartmanData?.id"
                                    @userAdded="refreshApartman" @userRemoved="refreshApartman" />
                            </div>
                        </div>

                        <!-- Jobb oldal: szobák -->
                        <div class="room-list-container w-1/2">
                            <h3 class="text-sm text-black/60 mb-2">Szobák:</h3>
                            <div class="rooms-list rounded-lg bg-gray-100 h-full p-2 ">
                                <ul class="list-disc list-inside">
                                    <li v-for="room in bindedRoomsToApartman" :key="room.id" class="text-gray-700">
                                        {{ room.name }} - {{ room.maxCapacity }} fő
                                    </li>
                                </ul>
                            </div>
                        </div>


                    </div>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>