<script setup>
import { useRoomStore } from '@/features/rooms/stores/room.store'
import DefaultButton from '@/components/DefaultButton.vue'
import MainTitle from '@/components/MainTitle.vue'

const emit = defineEmits(['close'])
const props = defineProps({
    showModal: { type: Boolean, required: true },
    roomData: { type: Object, default: null },
})

const roomStore = useRoomStore()
const prices = reactive({})
const errors = reactive({})

const capacityRows = computed(() => {
    const room = props.roomData
    if (!room) return []

    const min = Number(room.minCapacity ?? 0)
    const max = Number(room.maxCapacity ?? 0)
    if (!min || !max || min > max) return []

    const tiers = Array.isArray(room.roomPriceTiers) ? room.roomPriceTiers : []

    return Array.from({ length: max - min + 1 }, (_, index) => {
        const guestCount = min + index
        const tier = tiers.find((item) => Number(item.guestCount) === guestCount)

        return {
            guestCount,
            id: tier?.id ?? null,
            price: Number(tier?.price ?? room.price ?? 0),
        }
    })
})

watch(
    capacityRows,
    (rows) => {
        Object.keys(prices).forEach((key) => delete prices[key])
        Object.keys(errors).forEach((key) => delete errors[key])

        rows.forEach((row) => {
            prices[row.guestCount] = row.price
            errors[row.guestCount] = null
        })
    },
    { immediate: true },
)

function handleClose() {
    emit('close')
}

async function updateTier(row) {
    errors[row.guestCount] = null

    if (!row.id) {
        errors[row.guestCount] = 'Ehhez a letszamhoz nincs mentheto arsav.'
        return
    }

    const price = Number(prices[row.guestCount])
    if (!Number.isFinite(price) || price < 0) {
        errors[row.guestCount] = 'Adj meg ervenyes arat.'
        return
    }

    await roomStore.updatePriceTier({
        roomPriceTierId: row.id,
        price,
    })
}
</script>

<template>
    <Teleport to="body">
        <template v-if="showModal && roomData">
            <div class="modal-backdrop fixed inset-0 bg-black/50 z-40" @click="handleClose" />
            <div
                class="modal bg-white p-6 rounded-lg shadow-lg fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[calc(100%-20px)] max-w-2xl z-50 max-h-[90vh] overflow-y-auto">
                <button
                    class="close-button absolute top-2 right-2 text-gray-500 hover:text-gray-700 text-xl leading-none"
                    @click="handleClose">
                    &times;
                </button>

                <div class="modal-content">
                    <MainTitle :title="`Letszam szerinti arak: ${roomData.name}`" bar-color="#fbcfc4" class="mb-4" />

                    <div v-if="capacityRows.length === 0" class="rounded-lg bg-gray-50 p-4 text-sm text-gray-600">
                        Nincs megadhato kapacitastartomany ehhez a szobahoz.
                    </div>

                    <div v-else class="overflow-x-auto">
                        <table class="w-full text-left text-sm">
                            <thead class="bg-gray-50 text-xs uppercase text-gray-500">
                                <tr>
                                    <th class="px-3 py-2 font-bold">Fo</th>
                                    <th class="px-3 py-2 font-bold">Ar / ej</th>
                                    <th class="px-3 py-2 font-bold text-right">Muvelet</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-gray-100">
                                <tr v-for="row in capacityRows" :key="row.guestCount">
                                    <td class="px-3 py-3 font-semibold text-gray-700">
                                        {{ row.guestCount }} fo
                                    </td>
                                    <td class="px-3 py-3">
                                        <input v-model="prices[row.guestCount]" type="number" min="0" step="1"
                                            class="w-full rounded-lg bg-gray-200 px-3 py-2 outline-none transition-all duration-100 focus:ring-2 focus:ring-blue-500" />
                                        <span v-if="errors[row.guestCount]" class="mt-1 block text-xs text-red-500">
                                            {{ errors[row.guestCount] }}
                                        </span>
                                    </td>
                                    <td class="px-3 py-3">
                                        <div class="flex justify-end">
                                            <DefaultButton text="Mentes" type="button" @click="updateTier(row)"
                                                :button-class="`px-4 py-2 text-sm rounded bg-blue-600 text-white hover:bg-blue-700 ${!row.id ? 'opacity-50' : ''}`" />
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="form-actions flex gap-2 justify-end pt-4">
                        <DefaultButton text="Megse" type="button" @click="handleClose"
                            button-class="px-4 py-2 text-sm rounded bg-gray-100 hover:bg-gray-200" />
                    </div>
                </div>
            </div>
        </template>
    </Teleport>
</template>

<style scoped></style>
