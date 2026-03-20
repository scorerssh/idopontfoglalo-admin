<script setup>
import MainTitle from '@shared/components/MainTitle.vue'
const statCardContent = [
    {
        title: 'Ma aktív szobák', icon: '', text: '', additional: '',
    },
    {
        title: 'Foglalt szobák', icon: '', text: '', additional: '',
    },
    {
        title: 'Elérhető szobák', icon: '', text: '', additional: '',
    },
    {
        title: 'Leadott foglalások', icon: '', text: '', additional: '',
    }
]
</script>

<template>
    <div class="listing-overview-continer w-full pb-4 gap-4 flex flex-col">
        <MainTitle title="Áttekintés" barColor="#fbcfc4" />
        <div class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        </div>
    </div>
    <!-- Revenue -->
    <div class="revenue-sale-progress w-full flex flex-row justify-between pb-4">
        <div class="revenue-stats-container w-[70%]">
            <MainTitle title="Bevétel statisztika" barColor="#f4cbfe" />
        </div>
        <div class="sale-progress w-[30%]">
            <MainTitle title="Szobák bevételei" barColor="#c5edfc" />
        </div>
    </div>
    <!-- Listing -->

    <div class="listing">
        <MainTitle title="Listázás" barColor="#d8d6f9" />
    </div>

</template>

<style scoped></style>