<script setup>
import MainTitle from '@/components/MainTitle.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue';
import { BookMarked, Rss, CalendarCheck, GalleryHorizontalEnd, MapPinHouse } from 'lucide-vue-next';

const statCardContent = [
    {
        title: 'Ma aktív szobák', content: '23', icon: CalendarCheck, additional: 'asdf', bgColor: '#f3fbff', iconBgColor: '#c8f1fb',
    },
    {
        title: 'Foglalt szobák', content: '34', icon: BookMarked, additional: 'asdf', bgColor: '#fef5f8', iconBgColor: '#fbc3d7',
    },
    {
        title: 'Elérhető szobák', content: '56', icon: Rss, additional: 'asdf', bgColor: '#fff0ec', iconBgColor: '#fdd1c5',
    },
    {
        title: 'Leadott foglalások', content: '540', icon: GalleryHorizontalEnd, additional: 'asdf', bgColor: '#fdf6d8', iconBgColor: '#f4d778',
    },
    {
        title: 'Összes apartman', content: '90', icon: MapPinHouse, additional: 'asdf', bgColor: '#fef3ff', iconBgColor: '#fbcffd',
    }
]
</script>

<template>
    <div class="listing-overview-continer w-full pb-4 gap-4 flex flex-col">
        <MainTitle title="Vezérlőpult áttekintése" barColor="#fbcfc4" />
        <TransitionGroup name="card" appear tag="div"
            class="stats-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
            <DashboardStatCard v-for="(card, index) in statCardContent" :key="index" :title="card.title"
                :icon="card.icon" :additional="card.additional" :bgColor="card.bgColor" :iconBgColor="card.iconBgColor"
                :content="card.content" :style="{ animationDelay: `${index * 0.2}s` }" />
        </TransitionGroup>
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

<style scoped>
@keyframes slideUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.card-enter-active {
    animation: slideUp 0.4s ease both;
}

.card-enter-from {
    opacity: 0;
}
</style>