<script setup>
import MainTitle from '@/components/MainTitle.vue'
import DashboardStatCard from '@/features/shared/DashboardStatCard.vue'
import { useDashboardStore } from '@/features/dashboard/stores/dashboard.store'
import { STATUS } from '@/constants/status'
import {
    BedDouble,
    CalendarRange,
    ChartColumnBig,
    CircleDollarSign,
    DoorOpen,
    Hotel,
    RefreshCcw,
    TrendingUp,
} from 'lucide-vue-next'

const dashboardStore = useDashboardStore()

const currencyFormatter = new Intl.NumberFormat('hu-HU', {
    style: 'currency',
    currency: 'HUF',
    maximumFractionDigits: 0,
})

const integerFormatter = new Intl.NumberFormat('hu-HU')

const monthFormatter = new Intl.DateTimeFormat('hu-HU', {
    year: 'numeric',
    month: 'long',
})

function toNumber(value) {
    return Number(value ?? 0)
}

function formatCurrency(value) {
    return currencyFormatter.format(toNumber(value))
}

function formatInteger(value) {
    return integerFormatter.format(toNumber(value))
}

function formatMonthLabel(year, month) {
    return monthFormatter.format(new Date(year, month - 1, 1))
}

function createEmptyMonthSeries(length = 6) {
    const months = []
    const today = new Date()

    for (let index = length - 1; index >= 0; index -= 1) {
        const date = new Date(today.getFullYear(), today.getMonth() - index, 1)
        months.push({
            year: date.getFullYear(),
            month: date.getMonth() + 1,
            revenue: 0,
        })
    }

    return months
}

const dashboard = computed(() => dashboardStore.dashboard)
const isLoading = computed(() => dashboardStore.ops.getDashboard.status === STATUS.loading)
const hasError = computed(() => dashboardStore.ops.getDashboard.status === STATUS.error)
const errorMessage = computed(
    () => dashboardStore.ops.getDashboard.message || 'Sikertelen volt a dashboard betöltése.',
)

const normalizedMonthlyRevenues = computed(() => {
    const source = Array.isArray(dashboard.value.monthlyRevenues) ? dashboard.value.monthlyRevenues : []
    const sorted = [...source]
        .map((item) => ({
            year: Number(item.year),
            month: Number(item.month),
            revenue: toNumber(item.revenue),
        }))
        .sort((a, b) => a.year - b.year || a.month - b.month)

    return sorted.length ? sorted : createEmptyMonthSeries()
})

const monthlyRevenueWindow = computed(() => normalizedMonthlyRevenues.value.slice(-6))

const latestMonth = computed(
    () => monthlyRevenueWindow.value[monthlyRevenueWindow.value.length - 1] ?? null,
)

const previousMonth = computed(
    () => monthlyRevenueWindow.value[monthlyRevenueWindow.value.length - 2] ?? null,
)

const monthlyRevenueGrowth = computed(() => {
    const current = toNumber(latestMonth.value?.revenue)
    const previous = toNumber(previousMonth.value?.revenue)

    if (!previous) {
        return current > 0 ? 100 : 0
    }

    return ((current - previous) / previous) * 100
})

const averageMonthlyRevenue = computed(() => {
    if (!monthlyRevenueWindow.value.length) return 0

    const total = monthlyRevenueWindow.value.reduce((sum, item) => sum + toNumber(item.revenue), 0)
    return total / monthlyRevenueWindow.value.length
})

const bestMonth = computed(() => {
    if (!monthlyRevenueWindow.value.length) return null

    return monthlyRevenueWindow.value.reduce((best, item) =>
        toNumber(item.revenue) > toNumber(best.revenue) ? item : best,
    )
})

const lowestMonth = computed(() => {
    if (!monthlyRevenueWindow.value.length) return null

    return monthlyRevenueWindow.value.reduce((lowest, item) =>
        toNumber(item.revenue) < toNumber(lowest.revenue) ? item : lowest,
    )
})

const monthlyBars = computed(() => {
    const items = monthlyRevenueWindow.value
    const maxRevenue = Math.max(...items.map((item) => toNumber(item.revenue)), 1)

    return items.map((item, index) => ({
        ...item,
        label: formatMonthLabel(item.year, item.month),
        shortLabel: `${String(item.month).padStart(2, '0')}/${String(item.year).slice(-2)}`,
        formattedRevenue: formatCurrency(item.revenue),
        height: `${Math.max((toNumber(item.revenue) / maxRevenue) * 100, item.revenue > 0 ? 14 : 6)}%`,
        isLatest: index === items.length - 1,
    }))
})

const normalizedRoomRevenues = computed(() => {
    const source = Array.isArray(dashboard.value.roomRevenues) ? dashboard.value.roomRevenues : []

    return [...source]
        .map((item) => ({
            roomId: item.roomId,
            roomName: item.roomName || `Szoba #${item.roomId}`,
            revenue: toNumber(item.revenue),
        }))
        .sort((a, b) => b.revenue - a.revenue)
})

const topRoomRevenues = computed(() => normalizedRoomRevenues.value.slice(0, 5))

const roomRevenueBars = computed(() => {
    const maxRevenue = Math.max(...topRoomRevenues.value.map((item) => item.revenue), 1)

    return topRoomRevenues.value.map((item) => ({
        ...item,
        formattedRevenue: formatCurrency(item.revenue),
        share: dashboard.value.totalRevenue
            ? ((item.revenue / toNumber(dashboard.value.totalRevenue)) * 100).toFixed(1)
            : '0.0',
        width: `${Math.max((item.revenue / maxRevenue) * 100, item.revenue > 0 ? 12 : 4)}%`,
    }))
})

const occupancyRate = computed(() => {
    const activeRooms = toNumber(dashboard.value.activeRooms)
    const totalReservations = toNumber(dashboard.value.totalReservations)

    if (!activeRooms) return 0
    return (totalReservations / activeRooms) * 100
})

const revenuePerReservation = computed(() => {
    const totalReservations = toNumber(dashboard.value.totalReservations)
    if (!totalReservations) return 0

    return toNumber(dashboard.value.totalRevenue) / totalReservations
})

const revenuePerActiveRoom = computed(() => {
    const activeRooms = toNumber(dashboard.value.activeRooms)
    if (!activeRooms) return 0

    return toNumber(dashboard.value.totalRevenue) / activeRooms
})

const roomRevenueConcentration = computed(() => {
    const topThreeRevenue = normalizedRoomRevenues.value
        .slice(0, 3)
        .reduce((sum, item) => sum + item.revenue, 0)

    if (!dashboard.value.totalRevenue) return 0
    return (topThreeRevenue / toNumber(dashboard.value.totalRevenue)) * 100
})

const statCardContent = computed(() => [
    {
        title: 'Összes foglalás',
        content: formatInteger(dashboard.value.totalReservations),
        icon: CalendarRange,
        additional: `${formatCurrency(revenuePerReservation.value)} / foglalás`,
        bgColor: '#f3fbff',
        iconBgColor: '#c8f1fb',
    },
    {
        title: 'Aktív szobák',
        content: formatInteger(dashboard.value.activeRooms),
        icon: BedDouble,
        additional: `${occupancyRate.value.toFixed(1)}% foglalás / szoba`,
        bgColor: '#fef5f8',
        iconBgColor: '#fbc3d7',
    },
    {
        title: 'Új foglalások ebben a hónapban',
        content: formatInteger(dashboard.value.reservationsCreatedThisMonth),
        icon: TrendingUp,
        additional: `${monthlyRevenueGrowth.value >= 0 ? '+' : ''}${monthlyRevenueGrowth.value.toFixed(1)}% havi bevételváltozás`,
        bgColor: '#fff0ec',
        iconBgColor: '#fdd1c5',
    },
    {
        title: 'Teljes bevétel',
        content: formatCurrency(dashboard.value.totalRevenue),
        icon: CircleDollarSign,
        additional: `${formatCurrency(revenuePerActiveRoom.value)} / aktív szoba`,
        bgColor: '#fdf6d8',
        iconBgColor: '#f4d778',
    },
    {
        title: 'Bevételt termelő szobák',
        content: formatInteger(normalizedRoomRevenues.value.length),
        icon: Hotel,
        additional: `Top 3 szoba: ${roomRevenueConcentration.value.toFixed(1)}%`,
        bgColor: '#fef3ff',
        iconBgColor: '#fbcffd',
    },
])

const comparisonItems = computed(() => [
    {
        title: 'Utolsó hónap',
        value: latestMonth.value ? formatCurrency(latestMonth.value.revenue) : formatCurrency(0),
        subtitle: latestMonth.value ? formatMonthLabel(latestMonth.value.year, latestMonth.value.month) : 'Nincs adat',
    },
    {
        title: 'Előző hónap',
        value: previousMonth.value ? formatCurrency(previousMonth.value.revenue) : formatCurrency(0),
        subtitle: previousMonth.value ? formatMonthLabel(previousMonth.value.year, previousMonth.value.month) : 'Nincs összehasonlítás',
    },
    {
        title: '6 havi átlag',
        value: formatCurrency(averageMonthlyRevenue.value),
        subtitle: 'Stabilitási referencia',
    },
    {
        title: 'Legjobb hónap',
        value: bestMonth.value ? formatCurrency(bestMonth.value.revenue) : formatCurrency(0),
        subtitle: bestMonth.value ? formatMonthLabel(bestMonth.value.year, bestMonth.value.month) : 'Nincs adat',
    },
])

function growthTone(value) {
    if (value > 0) return 'text-emerald-600'
    if (value < 0) return 'text-rose-600'
    return 'text-slate-500'
}

function progressTone(value, max = 100) {
    const percent = max ? (value / max) * 100 : 0
    if (percent >= 70) return 'from-emerald-400 to-emerald-600'
    if (percent >= 40) return 'from-amber-300 to-amber-500'
    return 'from-sky-300 to-sky-500'
}

async function refreshDashboard() {
    await dashboardStore.getDashboard()
}

onMounted(() => {
    refreshDashboard()
})
</script>

<template>
    <div class="space-y-6 pb-8">
        <section class="listing-overview-continer w-full gap-4 flex flex-col">
            <div class="flex flex-col gap-4 xl:flex-row xl:items-center xl:justify-between">
                <MainTitle title="Vezérlőpult áttekintése" barColor="#fbcfc4" />

                <button type="button"
                    class="inline-flex items-center justify-center gap-2 self-start rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 shadow-sm transition hover:bg-slate-50"
                    :disabled="isLoading" @click="refreshDashboard">
                    <RefreshCcw class="h-4 w-4" :class="{ 'animate-spin': isLoading }" />
                    Frissítés
                </button>
            </div>

            <TransitionGroup name="card" appear tag="div"
                class="stats-grid grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-5">
                <DashboardStatCard v-for="(card, index) in statCardContent" :key="card.title" :title="card.title"
                    :icon="card.icon" :additional="card.additional" :bgColor="card.bgColor"
                    :iconBgColor="card.iconBgColor" :content="card.content"
                    :style="{ animationDelay: `${index * 0.12}s` }" />
            </TransitionGroup>
        </section>

        <div v-if="hasError"
            class="rounded-2xl border border-rose-200 bg-rose-50 px-5 py-4 text-sm text-rose-700 shadow-sm">
            {{ errorMessage }}
        </div>

        <div v-if="isLoading && !dashboard.totalReservations && !dashboard.totalRevenue"
            class="grid grid-cols-1 gap-4 lg:grid-cols-3">
            <div v-for="index in 3" :key="index"
                class="h-52 animate-pulse rounded-2xl border border-slate-100 bg-slate-100"></div>
        </div>

        <template v-else>
            <section class="revenue-sale-progress grid grid-cols-1 gap-4 xl:grid-cols-[2fr_1fr]">
                <div class="revenue-stats-container rounded-2xl  bg-white p-5">
                    <MainTitle title="Bevétel statisztika" barColor="#f4cbfe" />

                    <div class="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-4">
                        <div v-for="item in comparisonItems" :key="item.title"
                            class="rounded-2xl border border-slate-100 bg-slate-50 px-4 py-4">
                            <p class="text-sm font-medium text-slate-500">{{ item.title }}</p>
                            <p class="mt-2 text-2xl font-semibold text-slate-900">{{ item.value }}</p>
                            <p class="mt-1 text-xs text-slate-400">{{ item.subtitle }}</p>
                        </div>
                    </div>

                    <div class="mt-8 rounded-2xl bg-[linear-gradient(180deg,#faf5ff_0%,#ffffff_100%)] p-5">
                        <div class="flex items-center justify-between gap-4">
                            <div>
                                <p class="text-sm font-medium text-slate-500">Havi bevétel alakulása</p>
                                <p class="mt-1 text-lg font-semibold text-slate-900">Utolsó 6 hónap összehasonlítása</p>
                            </div>

                            <div class="text-right">
                                <p class="text-xs uppercase tracking-[0.2em] text-slate-400">Trend</p>
                                <p class="text-lg font-semibold" :class="growthTone(monthlyRevenueGrowth)">
                                    {{ monthlyRevenueGrowth >= 0 ? '+' : '' }}{{ monthlyRevenueGrowth.toFixed(1) }}%
                                </p>
                            </div>
                        </div>

                        <div class="mt-3 grid grid-cols-2 gap-2 text-xs text-slate-500 md:grid-cols-4">
                            <div class="rounded-xl bg-white/80 px-3 py-2">
                                Aktuális: <span class="font-semibold text-slate-900">{{ latestMonth ? latestMonth.label
                                    || formatMonthLabel(latestMonth.year, latestMonth.month) : 'Nincs adat' }}</span>
                            </div>
                            <div class="rounded-xl bg-white/80 px-3 py-2">
                                Előző: <span class="font-semibold text-slate-900">{{ previousMonth ?
                                    formatMonthLabel(previousMonth.year, previousMonth.month) : 'Nincs adat' }}</span>
                            </div>
                            <div class="rounded-xl bg-white/80 px-3 py-2">
                                Legjobb: <span class="font-semibold text-slate-900">{{ bestMonth ?
                                    formatMonthLabel(bestMonth.year, bestMonth.month) : 'Nincs adat' }}</span>
                            </div>
                            <div class="rounded-xl bg-white/80 px-3 py-2">
                                Átlag: <span class="font-semibold text-slate-900">{{
                                    formatCurrency(averageMonthlyRevenue) }}</span>
                            </div>
                        </div>

                        <div class="mt-8 grid h-72 grid-cols-6 items-end gap-3">
                            <div v-for="bar in monthlyBars" :key="`${bar.year}-${bar.month}`"
                                class="flex h-full flex-col justify-end">
                                <p class="mb-2 text-center text-xs font-medium text-slate-400">
                                    {{ bar.formattedRevenue }}
                                </p>
                                <div
                                    class="relative flex h-56 items-end justify-center rounded-t-[24px] bg-slate-100/70 px-2 pb-2">
                                    <div class="w-full rounded-t-[18px] transition-all duration-500"
                                        :class="bar.isLatest ? 'bg-[linear-gradient(180deg,#f59e0b_0%,#f97316_100%)]' : 'bg-[linear-gradient(180deg,#c084fc_0%,#818cf8_100%)]'"
                                        :style="{ height: bar.height }"></div>
                                </div>
                                <p class="mt-3 text-center text-xs font-semibold text-slate-500">{{ bar.shortLabel }}
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="sale-progress rounded-2xl bg-white p-5">
                    <MainTitle title="Szobák bevételi rangsora" barColor="#c5edfc" />

                    <div class="mt-6 space-y-4">
                        <div v-if="!roomRevenueBars.length"
                            class="rounded-2xl border border-dashed border-slate-200 bg-slate-50 px-4 py-10 text-center text-sm text-slate-500">
                            Még nincs megjeleníthető szoba bevétel adat.
                        </div>

                        <div v-for="room in roomRevenueBars" :key="room.roomId"
                            class="rounded-2xl border border-slate-100 bg-slate-50 p-4">
                            <div class="flex items-start justify-between gap-3">
                                <div>
                                    <p class="font-semibold text-slate-900">{{ room.roomName }}</p>
                                    <p class="mt-1 text-xs text-slate-400">{{ room.share }}% részesedés a teljes
                                        bevételből</p>
                                </div>
                                <p class="text-sm font-semibold text-slate-700">{{ room.formattedRevenue }}</p>
                            </div>

                            <div class="mt-4 h-3 overflow-hidden rounded-full bg-white">
                                <div class="h-full rounded-full bg-[linear-gradient(90deg,#7dd3fc_0%,#38bdf8_100%)]"
                                    :style="{ width: room.width }"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section class="listing space-y-4">
                <MainTitle title="Összehasonlítások" barColor="#d8d6f9" />

                <div class="grid grid-cols-1 gap-4 xl:grid-cols-[1.2fr_0.8fr]">
                    <div class="rounded-2xl bg-white p-5">
                        <div class="flex items-center gap-3">
                            <div class="rounded-full bg-[#ede9fe] p-3 text-[#6d28d9]">
                                <ChartColumnBig class="h-5 w-5" />
                            </div>
                            <div>
                                <p class="text-sm font-medium text-slate-500">Működési mutatók</p>
                                <p class="text-lg font-semibold text-slate-900">Mit érdemes most figyelni</p>
                            </div>
                        </div>

                        <div class="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2">
                            <div class="rounded-2xl bg-slate-50 p-4">
                                <p class="text-sm text-slate-500">Foglalások / aktív szoba</p>
                                <p class="mt-2 text-3xl font-semibold text-slate-900">
                                    {{ occupancyRate.toFixed(1) }}%
                                </p>
                                <p class="mt-2 text-xs text-slate-400">
                                    Ennyi foglalás jut átlagosan egy aktív szobára.
                                </p>
                            </div>

                            <div class="rounded-2xl bg-slate-50 p-4">
                                <p class="text-sm text-slate-500">Átlag bevétel / foglalás</p>
                                <p class="mt-2 text-3xl font-semibold text-slate-900">
                                    {{ formatCurrency(revenuePerReservation) }}
                                </p>
                                <p class="mt-2 text-xs text-slate-400">
                                    Hasznos gyors benchmark az árbevétel követéséhez.
                                </p>
                            </div>

                            <div class="rounded-2xl bg-slate-50 p-4">
                                <p class="text-sm text-slate-500">Átlag bevétel / aktív szoba</p>
                                <p class="mt-2 text-3xl font-semibold text-slate-900">
                                    {{ formatCurrency(revenuePerActiveRoom) }}
                                </p>
                                <p class="mt-2 text-xs text-slate-400">
                                    Segít látni, mennyire húzza a portfolió az összbevételt.
                                </p>
                            </div>

                            <div class="rounded-2xl bg-slate-50 p-4">
                                <p class="text-sm text-slate-500">Leggyengebb hónap</p>
                                <p class="mt-2 text-2xl font-semibold text-slate-900">
                                    {{ lowestMonth ? formatCurrency(lowestMonth.revenue) : formatCurrency(0) }}
                                </p>
                                <p class="mt-2 text-xs text-slate-400">
                                    {{ lowestMonth ? formatMonthLabel(lowestMonth.year, lowestMonth.month)
                                        : 'Nincs adat' }}
                                </p>
                            </div>
                        </div>
                    </div>

                    <div class="rounded-2xl  bg-white p-5 ">
                        <div class="flex items-center gap-3">
                            <div class="rounded-full bg-[#e0f2fe] p-3 text-[#0369a1]">
                                <DoorOpen class="h-5 w-5" />
                            </div>
                            <div>
                                <p class="text-sm font-medium text-slate-500">Kiemelt megfigyelések</p>
                                <p class="text-lg font-semibold text-slate-900">Gyors összkép</p>
                            </div>
                        </div>

                        <div class="mt-6 space-y-5">
                            <div>
                                <div class="mb-2 flex items-center justify-between text-sm">
                                    <span class="font-medium text-slate-600">Top 3 szoba bevételi súlya</span>
                                    <span class="font-semibold text-slate-900">{{ roomRevenueConcentration.toFixed(1)
                                        }}%</span>
                                </div>
                                <div class="h-3 overflow-hidden rounded-full bg-slate-100">
                                    <div class="h-full rounded-full bg-[linear-gradient(90deg,#60a5fa_0%,#2563eb_100%)]"
                                        :style="{ width: `${Math.min(roomRevenueConcentration, 100)}%` }"></div>
                                </div>
                            </div>

                            <div>
                                <div class="mb-2 flex items-center justify-between text-sm">
                                    <span class="font-medium text-slate-600">Havi növekedési lendület</span>
                                    <span class="font-semibold" :class="growthTone(monthlyRevenueGrowth)">
                                        {{ monthlyRevenueGrowth >= 0 ? '+' : '' }}{{ monthlyRevenueGrowth.toFixed(1) }}%
                                    </span>
                                </div>
                                <div class="h-3 overflow-hidden rounded-full bg-slate-100">
                                    <div class="h-full rounded-full bg-[linear-gradient(90deg,#34d399_0%,#059669_100%)]"
                                        :style="{ width: `${Math.min(Math.abs(monthlyRevenueGrowth), 100)}%` }"></div>
                                </div>
                            </div>

                            <div>
                                <div class="mb-2 flex items-center justify-between text-sm">
                                    <span class="font-medium text-slate-600">Aktív szobák kihasználtsági indexe</span>
                                    <span class="font-semibold text-slate-900">{{ occupancyRate.toFixed(1) }}%</span>
                                </div>
                                <div class="h-3 overflow-hidden rounded-full bg-slate-100">
                                    <div class="h-full rounded-full bg-gradient-to-r"
                                        :class="progressTone(occupancyRate)"
                                        :style="{ width: `${Math.min(occupancyRate, 100)}%` }"></div>
                                </div>
                            </div>

                            <div class="rounded-2xl bg-slate-50 p-4">
                                <p class="text-sm font-medium text-slate-500">Rövid értelmezés</p>
                                <p class="mt-2 text-sm leading-6 text-slate-600">
                                    A legerősebb hónap
                                    <span class="font-semibold text-slate-900">
                                        {{ bestMonth ? formatMonthLabel(bestMonth.year, bestMonth.month)
                                            : 'még nem látszik' }}
                                    </span>,
                                    miközben a havi átlag
                                    <span class="font-semibold text-slate-900">{{ formatCurrency(averageMonthlyRevenue)
                                        }}</span>.
                                    Ez jó gyors indikátor arra, hogy a mostani teljesítmény mennyire tér el a közelmúlt
                                    mintájától.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </template>
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
