<script setup>
import { EllipsisVertical, House, UserRound, Mail, MapPin, ShieldCheck } from 'lucide-vue-next'
import { computed } from 'vue'

const emits = defineEmits(['openModifyModal'])

const props = defineProps({
    user: {
        type: Object,
        required: true
    }
})

const calculatedActivityStatus = computed(() => {
    if (props.user.activityStatus === 'active')
        return {
            label: 'Aktív',
            bgClass: 'bg-green-100',
            textClass: 'text-green-700',
            dotClass: 'bg-green-500',
        }
    if (props.user.activityStatus === 'inactive')
        return {
            label: 'Inaktív',
            bgClass: 'bg-gray-100',
            textClass: 'text-gray-600',
            dotClass: 'bg-gray-500',
        }
    return {
        label: 'Ismeretlen',
        bgClass: 'bg-yellow-100',
        textClass: 'text-yellow-700',
        dotClass: 'bg-yellow-500',
    }
})

const selectedUser = computed(() => {
    return {
        id: props.user.id,
        userEmail: props.user.userEmail,
        userName: props.user.userName,
        role: props.user.role,
        activityStatus: props.user.activityStatus,
        cityName: props.user.cityName,
        countryName: props.user.countryName,
        region: props.user.region,
    }
})

function openModifyModal(user) {
    emits('openModifyModal', user)
}
</script>

<template>
    <div class="user-card bg-white shadow-sm border border-gray-100 rounded-xl flex flex-col p-4">

        <section class="flex justify-between items-start mb-4">
            <div class="flex gap-x-3 items-center min-w-0">
                <div :class="[
                    props.user.role === 'Admin' ? 'bg-purple-100 text-purple-600' : 'bg-green-100 text-green-600',
                    'p-3 rounded-full shrink-0'
                ]">
                    <UserRound class="h-6 w-6" />
                </div>
                <div class="flex flex-col min-w-0">
                    <span class="font-bold text-gray-900 leading-tight truncate">{{ props.user.userName }}</span>
                    <div class="flex items-center gap-1 text-gray-400">
                        <Mail class="h-3 w-3" />
                        <span class="text-xs truncate">{{ props.user.userEmail }}</span>
                    </div>
                </div>
            </div>
            <!--
            <div :class="[
                calculatedActivityStatus.bgClass,
                calculatedActivityStatus.textClass,
                'text-[11px] px-2.5 py-1 rounded-full flex items-center gap-1.5 font-bold shrink-0'
            ]">
                <span :class="[calculatedActivityStatus.dotClass, 'h-2 w-2 rounded-full']"></span>
                {{ calculatedActivityStatus.label }}
            </div>
            -->

        </section>

        <section class="grid grid-cols-2 gap-3 mb-4">
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Jogosultság</span>
                <div class="flex items-center gap-1.5 font-bold text-sm">
                    <ShieldCheck
                        :class="[props.user.role === 'Admin' ? 'text-purple-500' : 'text-green-500', 'h-3.5 w-3.5']" />
                    <span :class="props.user.role === 'Admin' ? 'text-purple-700' : 'text-green-700'">
                        {{ props.user.role ?? 'User' }}
                    </span>
                </div>
            </div>
            <div class="bg-gray-50 p-2.5 rounded-lg flex flex-col gap-y-0.5">
                <span class="text-[10px] text-gray-400 uppercase font-bold tracking-tight">Helyszín</span>
                <div class="flex items-center gap-1.5 text-gray-700 font-bold text-sm truncate">
                    <MapPin class="h-3.5 w-3.5 text-gray-400" />
                    <span class="truncate">{{ props.user.cityName ?? 'Ismeretlen' }}</span>
                </div>
            </div>
        </section>

        <section class="flex justify-between items-center pt-3 border-t border-gray-50 mt-auto">
            <div class="flex items-center gap-1.5 text-xs text-gray-500 font-medium">
                <House class="h-4 w-4 text-gray-400" />
                <span>Hozzárendelt ingatlanok: <span class="text-gray-900 font-bold">2</span></span>
            </div>

            <button @click="openModifyModal(selectedUser)"
                class="p-2 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors">
                <EllipsisVertical class="h-5 w-5" />
            </button>
        </section>
    </div>
</template>

<style scoped></style>