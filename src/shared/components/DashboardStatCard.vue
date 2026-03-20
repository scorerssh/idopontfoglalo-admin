<script setup>
import { Component } from 'lucide-vue-next';

const props = defineProps({
    title: String,
    content: String,
    icon: Component,
    additional: String,

})
</script>

<template>
    <div>
        <div class="p-4 bg-white rounded-lg shadow-md flex items-center gap-x-4">
            <div class="p-3 bg-blue-100 rounded-full">
                <component :is="icon" class="w-6 h-6 text-blue-500" />
            </div>
            <div>
                <h3 class="text-sm font-medium text-gray-500">{{ title }}</h3>
                <p class="text-lg font-semibold text-gray-900">{{ content }}</p>
                <p v-if="additional" class="text-xs text-gray-400">{{ additional }}</p>
            </div>
        </div>
    </div>
</template>

<style scoped></style>