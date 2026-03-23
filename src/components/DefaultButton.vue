<script setup>
import { computed } from 'vue';
import { RouterLink } from 'vue-router';

const props = defineProps({
    text: String,
    type: String,
    onClick: Function,
    to: String,
    icon: Object,
    iconClass: String,
    buttonClass: String,
})

function handleClick() {
    props.onClick?.()
}

const componentType = computed(() => {
    if (props.type === 'router-link') return RouterLink
    return 'button'
})

</script>

<template>
    <component :is="componentType" :to="componentType === RouterLink ? to : undefined" @click="handleClick"
        :class="[props.buttonClass, 'flex items-center gap-2 flex-row gap-x-2 p-2 rounded-lg transition-colors duration-100 shadow']">
        <span v-if="props.icon">
            <component :is="props.icon" :class="[props.iconClass]" />
        </span>
        <span v-if="props.text">{{ props.text }}</span>
    </component>
</template>

<style scoped></style>