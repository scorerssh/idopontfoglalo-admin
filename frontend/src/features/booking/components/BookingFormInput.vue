<script setup>
const props = defineProps({
    inputName: { type: String, default: '' },
    hasLabel: { type: Boolean, default: true },
    modelValue: { type: String, default: '' },
    labelText: { type: String, default: '' },
    type: { type: String, default: 'text' },
    inputClass: { type: String, default: '' },
    labelClass: { type: String, default: '' },
    errorText: { type: String, default: '' }
})

const emit = defineEmits(['update:modelValue'])
</script>

<template>
    <div class="input-box flex flex-col gap-y-1 justify-start w-full">
        <label v-if="hasLabel" :for="inputName" :class="[labelClass, 'text-sm']">
            {{ props.labelText }}
        </label>
        <input :value="props.modelValue" @input="emit('update:modelValue', $event.target.value)" :type="type"
            :name="inputName"
            :class="[inputClass, 'px-3 py-2 w-full bg-gray-100 focus-within:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100']" />
        <span v-if="props.errorText"
            class="errors w-full px-2 py-1 rounded-lg border border-red-400 bg-red-200 text-red-800 text-xs">
            {{ props.errorText }}
        </span>
    </div>
</template>