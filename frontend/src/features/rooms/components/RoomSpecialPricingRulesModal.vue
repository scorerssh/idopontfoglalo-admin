<script setup>
import { X, Plus, Pencil, Trash2 } from 'lucide-vue-next'
import MainTitle from '@/components/MainTitle.vue'
import DefaultButton from '@/components/DefaultButton.vue'
import { useRoomStore } from '../stores/room.store'

const props = defineProps({
  showModal: { type: Boolean, required: true },
  room: { type: Object, default: null },
})

const emit = defineEmits(['close'])
const roomStore = useRoomStore()

const editingRule = ref(null)
const isAdding = ref(false)

const emptyForm = () => ({ ruleType: '', priority: 0, active: true })
const formData = reactive(emptyForm())
const errors = reactive({ ruleType: null, priority: null })

const ruleTypes = computed(() => {
  const types = Array.isArray(roomStore.specialPricingRuleTypes)
    ? roomStore.specialPricingRuleTypes
    : []

  return types.map((type) => ({
    ruleType: type.ruleType ?? type.value ?? type.name ?? '',
    description: type.description ?? type.name ?? type.ruleType ?? '',
  }))
})

const rules = computed(() => {
  const currentRules = Array.isArray(roomStore.specialPricingRules)
    ? roomStore.specialPricingRules
    : []

  return [...currentRules].sort((a, b) => Number(a.priority ?? 0) - Number(b.priority ?? 0))
})

function resetErrors() {
  errors.ruleType = null
  errors.priority = null
}

function resetForm() {
  Object.assign(formData, emptyForm())
  resetErrors()
}

function validateForm() {
  resetErrors()
  let valid = true

  if (!formData.ruleType) {
    errors.ruleType = 'Válassz szabálytípust.'
    valid = false
  }

  if (formData.priority === '' || !Number.isFinite(Number(formData.priority))) {
    errors.priority = 'Adj meg érvényes prioritást.'
    valid = false
  }

  return valid
}

async function loadData(room) {
  if (!room) return

  editingRule.value = null
  isAdding.value = false
  resetForm()

  if (roomStore.specialPricingRuleTypes.length === 0) {
    await roomStore.getSpecialPricingRuleTypes()
  }

  await roomStore.getSpecialPricingRulesByRoom(room.id)
}

watch(
  () => props.room,
  async (newRoom) => {
    if (newRoom && props.showModal) {
      await loadData(newRoom)
    }
  },
)

watch(
  () => props.showModal,
  async (val) => {
    if (val && props.room) {
      await loadData(props.room)
    }
  },
)

function getRuleDescription(ruleType) {
  return (
    ruleTypes.value.find((type) => type.ruleType === ruleType)?.description ??
    rules.value.find((rule) => rule.ruleType === ruleType)?.description ??
    ruleType
  )
}

function startAdd() {
  editingRule.value = null
  isAdding.value = true
  resetForm()
  formData.ruleType = ruleTypes.value[0]?.ruleType ?? ''
}

function startEdit(rule) {
  isAdding.value = false
  editingRule.value = rule
  formData.ruleType = rule.ruleType
  formData.priority = rule.priority
  formData.active = Boolean(rule.active)
  resetErrors()
}

function cancelForm() {
  editingRule.value = null
  isAdding.value = false
  resetForm()
}

async function handleSave() {
  if (!props.room || !validateForm()) return

  const payload = {
    roomId: props.room.id,
    ruleType: formData.ruleType,
    priority: Number(formData.priority),
    active: Boolean(formData.active),
  }

  if (isAdding.value) {
    await roomStore.createSpecialPricingRule(payload)
  } else if (editingRule.value) {
    await roomStore.updateSpecialPricingRule({
      roomSpecialPricingRuleId: editingRule.value.id,
      ...payload,
    })
  }

  cancelForm()
}

async function handleDelete(rule) {
  await roomStore.deleteSpecialPricingRule(rule.id)
  if (editingRule.value?.id === rule.id) cancelForm()
}

function handleClose() {
  emit('close')
}
</script>

<template>
  <Teleport to="body">
    <template v-if="showModal && room">
      <div class="fixed inset-0 bg-black/50 z-40" @click="handleClose" />
      <div
        class="fixed top-1/2 left-1/2 z-50 max-h-[90vh] w-full max-w-2xl -translate-x-1/2 -translate-y-1/2 overflow-y-auto rounded-xl bg-white p-6 shadow-lg">
        <button
          @click="handleClose"
          class="absolute right-3 top-3 rounded-lg p-1.5 text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-600">
          <X class="h-4 w-4" />
        </button>

        <MainTitle :title="`Árszabályok - ${room?.name ?? ''}`" barColor="#c8f1fb" class="mb-5" />

        <div v-if="rules.length === 0 && !isAdding" class="py-6 text-center text-sm text-gray-400">
          Nincs megadott speciális árszabály.
        </div>

        <div v-else class="mb-4 flex flex-col gap-y-2">
          <div
            v-for="rule in rules"
            :key="rule.id"
            class="rounded-lg border border-gray-100 bg-gray-50 px-3 py-3">
            <div class="flex items-start justify-between gap-3">
              <div>
                <div class="flex flex-wrap items-center gap-2">
                  <span class="font-semibold text-gray-800">{{ getRuleDescription(rule.ruleType) }}</span>
                  <span
                    :class="[
                      'rounded px-2 py-0.5 text-xs font-semibold',
                      rule.active ? 'bg-green-100 text-green-700' : 'bg-gray-200 text-gray-600',
                    ]">
                    {{ rule.active ? 'Aktív' : 'Inaktív' }}
                  </span>
                  <span class="rounded bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-700">
                    Prioritás: {{ rule.priority }}
                  </span>
                </div>
                <p class="mt-1 text-xs text-gray-500">{{ rule.ruleType }}</p>
              </div>

              <div class="flex shrink-0 gap-1">
                <button
                  @click="startEdit(rule)"
                  class="rounded-lg p-1.5 text-gray-400 transition-colors hover:bg-blue-50 hover:text-blue-600">
                  <Pencil class="h-3.5 w-3.5" />
                </button>
                <button
                  @click="handleDelete(rule)"
                  class="rounded-lg p-1.5 text-gray-400 transition-colors hover:bg-red-50 hover:text-red-600">
                  <Trash2 class="h-3.5 w-3.5" />
                </button>
              </div>
            </div>
          </div>
        </div>

        <template v-if="isAdding || editingRule">
          <div class="mt-2 border-t border-gray-100 pt-4">
            <p class="mb-3 text-xs font-semibold uppercase text-gray-500">
              {{ isAdding ? 'Új árszabály' : 'Szerkesztés' }}
            </p>

            <div class="flex flex-col gap-y-3">
              <div>
                <label class="mb-1 block text-sm text-black/60">Szabálytípus</label>
                <select
                  v-model="formData.ruleType"
                  class="w-full rounded-lg bg-gray-200 px-3 py-2 outline-none transition-all duration-100 focus:ring-2 focus:ring-blue-500">
                  <option value="" disabled>Válassz szabálytípust...</option>
                  <option v-for="type in ruleTypes" :key="type.ruleType" :value="type.ruleType">
                    {{ type.description }}
                  </option>
                </select>
                <span v-if="errors.ruleType" class="mt-1 block text-xs text-red-500">
                  {{ errors.ruleType }}
                </span>
              </div>

              <div>
                <label class="mb-1 block text-sm text-black/60">Prioritás</label>
                <input
                  v-model="formData.priority"
                  type="number"
                  step="1"
                  class="w-full rounded-lg bg-gray-200 px-3 py-2 outline-none transition-all duration-100 focus:ring-2 focus:ring-blue-500" />
                <span v-if="errors.priority" class="mt-1 block text-xs text-red-500">
                  {{ errors.priority }}
                </span>
              </div>

              <label class="flex items-center gap-2 text-sm text-gray-700">
                <input v-model="formData.active" type="checkbox" class="h-4 w-4 rounded border-gray-300" />
                Aktív szabály
              </label>
            </div>

            <div class="mt-4 flex justify-end gap-2">
              <DefaultButton
                type="button"
                text="Mégse"
                @click="cancelForm"
                button-class="px-4 py-2 text-sm rounded-lg bg-gray-100 hover:bg-gray-200 text-gray-700 transition-colors" />
              <DefaultButton
                type="button"
                text="Mentés"
                @click="handleSave"
                button-class="px-4 py-2 text-sm rounded-lg text-white bg-blue-600 hover:bg-blue-700 transition-colors" />
            </div>
          </div>
        </template>

        <div v-if="!isAdding && !editingRule" class="mt-4 flex justify-end border-t border-gray-100 pt-4">
          <DefaultButton
            type="button"
            text="Hozzáadás"
            :icon="Plus"
            @click="startAdd"
            button-class="px-4 py-2 text-sm rounded-lg text-white bg-blue-600 hover:bg-blue-700 transition-colors" />
        </div>
      </div>
    </template>
  </Teleport>
</template>
