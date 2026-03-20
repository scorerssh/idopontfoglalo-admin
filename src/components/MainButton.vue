<script setup>
import { Icon } from 'lucide-vue-next';
import { computed } from 'vue';
import { RouterLink } from 'vue-router';

const props = defineProps({
    text: String,
    type: String,
    onClick: Function,
    to: String,
    icon: Object
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
    <component :is="componentType" :to="componentType === RouterLink ? to : undefined" @click="handleClick">
        <span v-if="props.icon">
            <component :is="props.icon" class="w-6 h-6" />
        </span>
        <span v-if="props.text">{{ props.text }}</span>
    </component>
</template>

<style scoped></style>