<script setup>
const props = defineProps({
    title: {
        type: String,
        required: true,
    },
    barColor: {
        type: String,
        default: 'bg-system-blue',
    },
})

const isHexColor = (str) => str.startsWith('#');
const isTailwindClass = !isHexColor(props.barColor);
</script>

<template>
    <div class="title flex flex-row items-center gap-x-2">
        <span class="h-7 w-[14px] rounded-[3px]" :class="isTailwindClass ? props.barColor : ''"
            :style="isHexColor(props.barColor) ? { backgroundColor: props.barColor } : {}"></span>
        <h2 class="text-2xl font-semibold tracking-tight">{{ props.title }}</h2>
    </div>
</template>

<style scoped></style>