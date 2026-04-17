<script setup>
const props = defineProps({
    inputName: { type: String, default: '' },
    hasLabel: { type: Boolean, default: true },
    modelValue: { type: [String, Number, null], default: '' },
    labelText: { type: String, default: '' },
    type: { type: String, default: 'text' },
    inputClass: { type: String, default: '' },
    labelClass: { type: String, default: '' },
    options: { type: Array, default: () => [] }
})

const emit = defineEmits(['update:modelValue'])
</script>

<template>
    <div class="input-box flex flex-col gap-y-1 justify-start w-full">
        <label v-if="hasLabel" :for="inputName" :class="[labelClass, 'text-sm']">
            {{ props.labelText }}
        </label>

        <select v-if="type === 'select'" :value="props.modelValue"
            @change="emit('update:modelValue', $event.target.value)" :name="inputName"
            :class="[inputClass, 'px-3 py-2 md:w-auto w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100']">
            <slot />
            <option :value="null" disabled>Válassz apartmant...</option>

            <option v-for="option in options" :key="option.id" :value="option.id">
                {{ option.name }}
            </option>
        </select>

        <input v-else :value="props.modelValue" @input="emit('update:modelValue', $event.target.value)" :type="type"
            :name="inputName"
            :class="[inputClass, 'px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100']" />
    </div>
</template>