<script setup>
import { EllipsisVertical, House } from 'lucide-vue-next'
import placeholderImage from '@assets/pictures/profile_pic_placeholder.webp'
import { computed } from 'vue'

const emits = defineEmits(['openModifyModal'])

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
    if (props.activityStatus === 'active')
        return {
            label: 'Aktív',
            bgClass: 'bg-green-100',
            textClass: 'text-green-600',
            iconClass: 'bg-green-500',
        }
    if (props.activityStatus === 'inactive')
        return {
            label: 'Inaktív',
            bgClass: 'bg-gray-100',
            textClass: 'text-gray-600',
            iconClass: 'bg-gray-500',
        }
    return {
        label: 'Ismeretlen',
        bgClass: 'bg-yellow-100',
        textClass: 'text-yellow-600',
        iconClass: 'bg-yellow-500',
    }
})

const selectedUser = computed(() => {
    return {
        email: props.email,
        userName: props.userName,
        role: props.role,
        activityStatus: props.activityStatus,
        cityName: props.cityName,
        countryName: props.countryName,
        region: props.region,
    }
})

function openModifyModal(selectedUser) {
    emits('openModifyModal', selectedUser)
}
</script>

<template>
    <div class="user-card shadow rounded-lg flex flex-col gap-y-4 p-3">
        <section class="top-section w-full flex flex-row justify-between items-center">
            <div class="user-personal w-full flex justify-start gap-x-3">
                <div class="profile-pic">
                    <img :src="placeholderImage" alt="User placeholder image"
                        class="rounded-full w-12 h-12 object-cover" />
                </div>
                <div class="name-email flex flex-col justify-center">
                    <span class="text-black font-semibold">
                        {{ userName }}
                    </span>
                    <span class="text-black/70">
                        {{ email }}
                    </span>
                </div>
            </div>
            <div :class="[
                calculatedActivityStatus.bgClass,
                'text-sm rounded-full flex flex-row gap-x-1 items-center px-2 py-1 h-8',
            ]">
                <span :class="[calculatedActivityStatus.iconClass, 'h-2.75 w-2.75 rounded-full inline-block']"></span>
                <span :class="calculatedActivityStatus.textClass">
                    {{ calculatedActivityStatus.label }}
                </span>
            </div>
        </section>
        <section class="middle-section flex flex-col gap-y-2">
            <div class="user-role flex flex-row items-center mt-3 w-full gap-x-2">
                <div class="flex w-1/2 flex-col rounded-lg">
                    <span class="text-sm text-black/70">Jogosultság</span>
                    <span :class="['text-lg font-semibold', props.role === 'Admin' ? 'text-red-600' : 'text-blue-600']">
                        {{ role ?? 'Ismeretlen' }}
                    </span>
                </div>
                <div class="flex flex-col  w-1/2">
                    <span class="text-sm text-black/70">Város</span>
                    <span class="text-lg text-black font-semibold">{{ cityName ?? 'Ismeretlen' }}</span>
                </div>
            </div>

            <div class="user-destination flex flex-row justify-between items-center mt-3 w-full gap-x-2">
                <div class="flex flex-col w-1/2">
                    <span class="text-sm text-black/70">Ország</span>
                    <span class="text-lg font-semibold">{{ countryName ?? 'Ismeretlen' }}</span>
                </div>
                <div class="flex flex-col  w-1/2">
                    <span class="text-sm text-black/70">Régió</span>
                    <span class="text-lg font-semibold">{{ region ?? 'Ismeretlen' }}</span>
                </div>
            </div>
        </section>
        <section class="bottom-section flex flex-row justify-between items-center mt-3">
            <div class="room-counter">
                <House class="h-5 w-5" />
            </div>
            <button @click="openModifyModal(selectedUser)"
                class="actions p-2 rounded-full hover:bg-gray-200 transition-colors">
                <EllipsisVertical class="h-5 w-5" />
            </button>
        </section>
    </div>
</template>

<style scoped></style>
