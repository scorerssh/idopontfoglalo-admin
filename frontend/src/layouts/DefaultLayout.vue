<script setup>
import { useRole } from '@/composables/useRole'
import { useAuthStore } from '@/features/auth/stores/auth.store'
import { useTheme } from '@/composables/useTheme'
import {
    PanelRightClose,
    LayoutGrid,
    UsersRound,
    Calendar,
    FilePlay,
    BookMarked,
    Settings,
    Bell,
    LogOut,
    House,
    BedDouble,
    Sun,
    Moon,
} from 'lucide-vue-next'
import { RouterView, useRoute } from 'vue-router'

const { isAdmin, isUser } = useRole()
const authStore = useAuthStore()
const { isDark, toggleTheme } = useTheme()
const route = useRoute()
const isSidebarOpen = ref(true)
const sidebarRailWidthClass = 'w-20'

const handleLogout = async () => {
    await authStore.logout()
}

function toggleSidebar() {
    isSidebarOpen.value = !isSidebarOpen.value
}

const navbarLinks = computed(() => {
    if (isAdmin.value) {
        return adminNavbarLinks
    } else if (isUser.value) {
        return userNavbarLinks
    } else {
        return []
    }
})

const adminNavbarLinks = [
    { title: 'Vezérlőpult', icon: LayoutGrid, route: '/admin-dashboard' },
    { title: 'Felhasználók', icon: UsersRound, route: '/users' },
    { title: 'Naptár', icon: Calendar, route: '/calendar' },
    { title: 'Videók', icon: FilePlay, route: '/videos' },
    { title: 'Foglalások', icon: BookMarked, route: '/bookings' },
    { title: 'Szobák', icon: BedDouble, route: '/rooms' },
    { title: 'Apartmanok', icon: House, route: '/apartments' },
]

const userNavbarLinks = [
    { title: 'Vezérlőpult', icon: LayoutGrid, route: '/user-dashboard' },
    { title: 'Naptár', icon: Calendar, route: '/calendar' },
    { title: 'Videók', icon: FilePlay, route: '/videos' },
    { title: 'Foglalások', icon: BookMarked, route: '/bookings' },
    { title: 'Szobák', icon: BedDouble, route: '/rooms' },
]
</script>

<template>
    <div class="relative h-screen w-full flex overflow-hidden bg-white">
        <div :class="[sidebarRailWidthClass, 'relative shrink-0 border-r border-gray-200 bg-white']">
            <aside
                :class="[
                    isSidebarOpen ? 'w-64 shadow-xl' : sidebarRailWidthClass,
                    'absolute left-0 top-0 z-40 flex h-screen flex-col gap-y-2 overflow-y-scroll border-r border-gray-200 bg-white p-4 transition-all duration-300 ease-in-out [scrollbar-width:none] [&::-webkit-scrollbar]:hidden',
                ]"
            >
                <h1 class="font-semibold tracking-tight w-full flex items-center justify-center">
                    <span
                        :class="[
                            isSidebarOpen ? 'md:block text-3xl' : 'md:block text-xl',
                            'bg-gradient-to-r from-blue-500 to-indigo-600 bg-clip-text text-transparent transition-all duration-300',
                        ]"
                    >
                        Host
                    </span>
                </h1>

                <button
                    @click="toggleSidebar"
                    class="mt-2 flex items-center justify-center rounded-md border border-gray-200 p-2 hover:bg-gray-200"
                >
                    <PanelRightClose
                        :class="[
                            isSidebarOpen ? 'rotate-180 text-[#275bf6]' : 'text-gray-500',
                            'h-4 w-4 transition-transform duration-300 ease-in-out',
                        ]"
                    />
                </button>

                <nav class="flex flex-col justify-between h-full">
                    <ul class="mt-2 flex flex-col gap-y-1">
                        <li v-for="link in navbarLinks" :key="link.title">
                            <RouterLink
                                :to="link.route"
                                :class="[
                                    isSidebarOpen ? 'justify-start gap-x-3' : 'justify-center',
                                    route.path === link.route
                                        ? 'bg-[#275bf6] text-white shadow-md'
                                        : 'text-black/60 hover:bg-blue-100',
                                    'flex items-center overflow-hidden rounded-lg p-3 transition-all duration-200 ease-in-out',
                                ]"
                            >
                                <component :is="link.icon" class="h-6 w-6 shrink-0" />

                                <span
                                    :class="[
                                        isSidebarOpen ? 'ml-0 max-w-[140px] opacity-100' : 'ml-0 max-w-0 opacity-0',
                                        'overflow-hidden whitespace-nowrap text-sm font-medium transition-all duration-200 ease-in-out',
                                    ]"
                                >
                                    {{ link.title }}
                                </span>
                            </RouterLink>
                        </li>
                    </ul>

                    <ul class="mt-2 flex flex-col gap-y-1">
                        <li>
                            <RouterLink
                                to="/settings"
                                :class="[
                                    isSidebarOpen ? 'justify-start gap-x-3' : 'justify-center',
                                    route.path === '/settings'
                                        ? 'bg-[#275bf6] text-white shadow-md'
                                        : 'text-black/60 hover:bg-blue-100',
                                    'flex items-center overflow-hidden rounded-lg p-3 transition-all duration-200 ease-in-out',
                                ]"
                            >
                                <Settings class="h-6 w-6 shrink-0" />

                                <span
                                    :class="[
                                        isSidebarOpen ? 'max-w-[140px] opacity-100' : 'max-w-0 opacity-0',
                                        'overflow-hidden whitespace-nowrap text-sm font-medium transition-all duration-200 ease-in-out',
                                    ]"
                                >
                                    Beállítások
                                </span>
                            </RouterLink>
                        </li>

                        <li>
                            <button
                                @click="handleLogout"
                                :class="[
                                    isSidebarOpen ? 'justify-start gap-x-3' : 'justify-center',
                                    'flex w-full items-center overflow-hidden rounded-lg bg-red-200 p-3 text-red-700 transition-all duration-200 ease-in-out hover:bg-red-300',
                                ]"
                            >
                                <LogOut class="h-6 w-6 shrink-0" />

                                <span
                                    :class="[
                                        isSidebarOpen ? 'max-w-[140px] opacity-100' : 'max-w-0 opacity-0',
                                        'overflow-hidden whitespace-nowrap text-sm font-medium transition-all duration-200 ease-in-out',
                                    ]"
                                >
                                    Kijelentkezés
                                </span>
                            </button>
                        </li>
                    </ul>
                </nav>
            </aside>
        </div>

        <button
            v-if="isSidebarOpen"
            @click="toggleSidebar"
            class="absolute inset-y-0 left-20 right-0 z-30 bg-black/10 transition-opacity duration-300"
            aria-label="Oldalsáv bezárása"
        />

        <div class="flex-1 flex flex-col min-h-screen min-w-0">
            <header class="flex items-center justify-between p-6 h-24 shrink-0">
                <div class="unknown"></div>
                <div class="flex items-center gap-x-3">
                    <!-- Dark / Light mode toggle -->
                    <button
                        @click="toggleTheme"
                        :title="isDark ? 'Váltás világos módra' : 'Váltás sötét módra'"
                        class="flex items-center gap-x-1.5 rounded-full bg-gray-100 px-3 py-2 transition-colors hover:bg-gray-200"
                    >
                        <Sun v-if="isDark" class="w-4 h-4 text-amber-400" />
                        <Moon v-else class="w-4 h-4 text-slate-500" />
                        <!-- pill indicator -->
                        <span class="relative inline-flex h-5 w-9 shrink-0 rounded-full transition-colors duration-300"
                            :class="isDark ? 'bg-slate-600' : 'bg-slate-200'">
                            <span class="absolute top-0.5 h-4 w-4 rounded-full bg-white shadow transition-transform duration-300"
                                :class="isDark ? 'translate-x-4' : 'translate-x-0.5'" />
                        </span>
                    </button>

                    <RouterLink
                        to="/settings"
                        class="flex items-center justify-center rounded-full bg-gray-100 p-2 transition-colors hover:bg-gray-200"
                    >
                        <Settings class="w-6 h-6 text-gray-600" />
                    </RouterLink>

                    <RouterLink
                        to="/notifications"
                        class="flex items-center justify-center rounded-full bg-gray-100 p-2 transition-colors hover:bg-gray-200"
                    >
                        <Bell class="w-6 h-6 text-gray-600" />
                    </RouterLink>
                </div>
            </header>

            <main class="flex-1 overflow-y-auto p-6 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
                <RouterView />
            </main>
        </div>
    </div>
</template>

<style scoped></style>
