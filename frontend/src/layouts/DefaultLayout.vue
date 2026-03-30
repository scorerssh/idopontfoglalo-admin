<script setup>
import { useAuthStore } from '@/features/auth/stores/auth'
import { ref, computed } from 'vue'
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
} from 'lucide-vue-next'
import { RouterView, useRoute } from 'vue-router'

const authStore = useAuthStore()
const route = useRoute()
const isSidebarOpen = ref(true)
const handleLogout = () => {
    authStore.logout()
}
function toggleSidebar() {
    isSidebarOpen.value = !isSidebarOpen.value
}

const navbarLinks = computed(() => {
    if (authStore.role === 'Admin') {
        return adminNavbarLinks
    } else if (authStore.role === 'User') {
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
    { title: 'Apartmanok', icon: House, route: '/apartments' }
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
    <div class=" h-screen w-full flex bg-white">
        <aside :class="[
            isSidebarOpen ? 'w-64' : 'w-20',
            'flex flex-col gap-y-2 p-4 shrink-0 border-r border-gray-200 transition-all duration-300 ease-in-out h-screen overflow-y-scroll [scrollbar-width:none] [&::-webkit-scrollbar]:hidden'
        ]">
            <h1 class=" font-semibold tracking-tight w-full flex items-center justify-center">
                <span :class="[
                    isSidebarOpen ? 'md:block text-3xl' : 'md:block text-xl',
                    'bg-gradient-to-r from-blue-500 to-indigo-600 bg-clip-text text-transparent transition-all duration-300'
                ]">
                    Host
                </span>
            </h1>

            <button @click="toggleSidebar"
                class="p-2 rounded-md hover:bg-gray-200 flex items-center justify-center mt-2 border border-gray-200">
                <PanelRightClose :class="[
                    isSidebarOpen ? 'text-[#275bf6] rotate-180' : 'text-gray-500',
                    'w-4 h-4 transition-transform duration-300 ease-in-out'
                ]" />
            </button>

            <nav class="flex flex-col justify-between h-full">
                <ul class="flex flex-col gap-y-1 mt-2">
                    <li v-for="link in navbarLinks" :key="link.title">
                        <RouterLink :to="link.route" :class="[
                            isSidebarOpen ? 'gap-x-3 justify-start' : 'justify-center',
                            route.path === link.route
                                ? 'bg-[#275bf6] text-white shadow-md'
                                : 'hover:bg-blue-100 text-black/60',
                            'flex items-center p-3 rounded-lg transition-all duration-200 ease-in-out overflow-hidden'
                        ]">
                            <component :is="link.icon" class="w-6 h-6 shrink-0" />

                            <span :class="[
                                isSidebarOpen ? 'opacity-100 max-w-[140px] ml-0' : 'opacity-0 max-w-0 ml-0',
                                'text-sm font-medium whitespace-nowrap overflow-hidden transition-all duration-200 ease-in-out'
                            ]">
                                {{ link.title }}
                            </span>
                        </RouterLink>
                    </li>
                </ul>

                <ul class="flex flex-col gap-y-1 mt-2">
                    <li>
                        <RouterLink to="/settings" :class="[
                            isSidebarOpen ? 'gap-x-3 justify-start' : 'justify-center',
                            route.path === '/settings'
                                ? 'bg-[#275bf6] text-white shadow-md'
                                : 'hover:bg-blue-100 text-black/60',
                            'flex items-center p-3 rounded-lg transition-all duration-200 ease-in-out overflow-hidden'
                        ]">
                            <Settings class="w-6 h-6 shrink-0" />

                            <span :class="[
                                isSidebarOpen ? 'opacity-100 max-w-[140px]' : 'opacity-0 max-w-0',
                                'text-sm font-medium whitespace-nowrap overflow-hidden transition-all duration-200 ease-in-out'
                            ]">
                                Beállítások
                            </span>
                        </RouterLink>
                    </li>

                    <li>
                        <button @click="handleLogout" :class="[
                            isSidebarOpen ? 'gap-x-3 justify-start' : 'justify-center',
                            'w-full flex items-center p-3 rounded-lg transition-all duration-200 ease-in-out overflow-hidden bg-red-200 text-red-700 hover:bg-red-300'
                        ]">
                            <LogOut class="w-6 h-6 shrink-0" />

                            <span :class="[
                                isSidebarOpen ? 'opacity-100 max-w-[140px]' : 'opacity-0 max-w-0',
                                'text-sm font-medium whitespace-nowrap overflow-hidden transition-all duration-200 ease-in-out'
                            ]">
                                Kijelentkezés
                            </span>
                        </button>
                    </li>
                </ul>
            </nav>
        </aside>

        <div class="flex-1 flex flex-col min-w-0 min-h-screen">
            <header class="flex items-center justify-between p-6 h-24 shrink-0">

                <div class="unknown"></div>
                <div class="flex items-center gap-x-3">
                    <RouterLink to="/settings"
                        class="p-2 rounded-full hover:bg-gray-200 bg-gray-100 flex items-center justify-center transition-colors">
                        <Settings class="w-6 h-6 text-gray-600" />
                    </RouterLink>

                    <RouterLink to="/notifications"
                        class="p-2 rounded-full hover:bg-gray-200 bg-gray-100 flex items-center justify-center transition-colors">
                        <Bell class="w-6 h-6 text-gray-600" />
                    </RouterLink>
                </div>
            </header>

            <main class="flex-1 overflow-y-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden p-6">
                <RouterView />
            </main>
        </div>
    </div>
</template>

<style scoped></style>