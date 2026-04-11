<script setup>
import { useTheme } from '@/composables/useTheme'
import { vividForDark } from '@/utils/colorUtils'

const { isDark } = useTheme()

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

const isHex = (str) => str.startsWith('#')

const barStyle = computed(() => {
    if (!isHex(props.barColor)) return {}
    const color = isDark.value ? vividForDark(props.barColor) : props.barColor
    return { backgroundColor: color }
})

const barClass = computed(() => {
    if (isHex(props.barColor)) return ''
    // Tailwind osztálynál dark módban növeljük a fényességet CSS-sel
    return props.barColor
})
</script>

<template>
    <div class="title flex flex-row items-center gap-x-2">
        <span class="h-7 w-3.5 rounded-[3px]"
            :class="barClass"
            :style="barStyle">
        </span>
        <h2 class="text-2xl font-semibold tracking-tight">{{ props.title }}</h2>
    </div>
</template>

<style scoped></style>
