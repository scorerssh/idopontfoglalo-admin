<script setup>
import { useTheme } from '@/composables/useTheme'
import { darkenForDark, tintForDark } from '@/utils/colorUtils'

const { isDark } = useTheme()

const props = defineProps({
    title: String,
    content: String,
    icon: Object,
    additional: String,
    bgColor: String,
    iconBgColor: String,
})

const cardStyle = computed(() =>
    isDark.value
        ? { backgroundColor: tintForDark(props.bgColor) }
        : { backgroundColor: props.bgColor }
)

const iconStyle = computed(() =>
    isDark.value
        ? { backgroundColor: darkenForDark(props.iconBgColor) }
        : { backgroundColor: props.iconBgColor }
)
</script>

<template>
    <div>
        <div class="p-4 bg-white flex-row rounded-lg shadow flex items-center gap-x-4 border border-gray-100"
            :style="cardStyle">
            <div class="icon-container">
                <div class="p-2 flex items-center justify-center rounded-full" :style="iconStyle">
                    <component :is="icon" class="w-6 h-6 text-black" />
                </div>
            </div>
            <div>
                <h3 class="text-sm font-medium text-gray-500">{{ title }}</h3>
                <p class="text-3xl font-semibold text-gray-900">{{ content }}</p>
                <p v-if="additional" class="text-xs text-gray-400">{{ additional }}</p>
            </div>
        </div>
    </div>
</template>

<style scoped></style>
