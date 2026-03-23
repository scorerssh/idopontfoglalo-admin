<script setup>
const props = defineProps({
    inputName: {
        type: String,
        default: ''
    },
    hasLabel: {
        type: Boolean,
        required: true,
        default: true
    },
    modelValue: {
        type: String,
        default: '',
    },
    labelText: {
        type: String,
        default: '',
    },
    type: {
        type: String,
        default: 'text'
    },
    inputClass: {
        type: String,
        default: ''
    },
    labelClass: {
        type: String,
        default: ''
    }
})
</script>

<template>
    <div class="input-box flex flex-col gap-y-1 justify-start  w-full">
        <label v-if="hasLabel" :for="inputName" :class="[labelClass, 'text-sm']">
            {{ props.labelText }}
        </label>
        <input v-model="props.modelValue" :type="type" :name="inputName"
            :class="[inputClass, 'px-3 py-2 w-full bg-gray-100 focus-within:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100']" />
    </div>
</template>

<style scoped></style>