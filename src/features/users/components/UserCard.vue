<script setup>
import { EllipsisVertical, HouseHeart } from 'lucide-vue-next'
import placeholderImage from '@assets/pictures/profile_pic_placeholder.webp'
import { computed } from 'vue'

const props = defineProps({
    email: {
        type: String,
        required: true,
    },
    userName: {
        type: String,
        required: true,
    },
    role: {
        type: String,
        required: true,
    },
    activityStatus: {
        type: String,
        required: true,
    },
    cityName: {
        type: String,
        required: false,
    },
    countryName: {
        type: String,
        required: false,
    },
    region: {
        type: String,
        required: false,
    },
})

const calculatedActivityStatus = computed(() => {
    if (props.activityStatus === 'active') return { label: 'Aktív', bgClass: 'bg-green-100', textClass: 'text-green-600', iconClass: 'bg-green-500' }
    if (props.activityStatus === 'inactive') return { label: 'Inaktív', bgClass: 'bg-gray-100', textClass: 'text-gray-600', iconClass: 'bg-gray-500' }
    return { label: 'Ismeretlen', bgClass: 'bg-yellow-100', textClass: 'text-yellow-600', iconClass: 'bg-yellow-500' }
})
</script>

<template>
    <div class="user-card bg-white rounded-lg shadow p-3">
        <section class="top-section w-full flex flex-row justify-between p-2">
            <div>{{ cityName ?? 'Ismeretlen város' }}</div>
            <div :class="[calculatedActivityStatus.bgClass, 'text-sm rounded-full flex flex-row gap-x-1 items-center px-2 py-1']">
                <span :class="[calculatedActivityStatus.iconClass, 'h-2.75 w-2.75 rounded-full inline-block']"></span>
                <span :class="calculatedActivityStatus.textClass">
                    {{ calculatedActivityStatus.label }}
                </span>
            </div>
        </section>
        <section class="middle-section">
            <div class="user-personal w-full flex justify-start gap-x-3">
                <div class="profile-pic">
                    <img :src="placeholderImage" alt="User placeholder image" class="rounded-full w-12 h-12 object-cover" />
                </div>
                <div class="name-email flex flex-col gap-y-1">
                    <span class="text-black font-semibold">
                        {{ userName }}
                    </span>
                    <span class="text-black/70">
                        {{ email }}
                    </span>
                </div>
            </div>
            <div class="user-destination flex flex-row justify-between items-center mt-3">
                <div class="flex flex-col">
                    <span class="text-sm text-black/70">Ország</span>
                    <span>{{ countryName ?? 'Ismeretlen' }}</span>
                </div>
                <div class="flex flex-col">
                    <span class="text-sm text-black/70">Régió</span>
                    <span>{{ region ?? 'Ismeretlen' }}</span>
                </div>
            </div>
        </section>
        <section class="bottom-section flex flex-row justify-between items-center mt-3">
            <div class="room-counter">
                <HouseHeart />
            </div>
            <div class="actions">
                <EllipsisVertical />
            </div>
        </section>
    </div>
</template>

<style scoped></style>