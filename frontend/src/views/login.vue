<script setup>
import { useRouter } from 'vue-router'
import { useRole } from '@/composables/useRole'
import { useAuthStore } from '@/features/auth/stores/auth.store'
import login_1 from '@/assets/pictures/login-bg.webp'
import login_2 from '@/assets/pictures/login-bg-2.webp'
import login_3 from '@/assets/pictures/login-bg-3.webp'
import login_4 from '@/assets/pictures/login-bg-4.webp'
import login_5 from '@/assets/pictures/login-bg-5.webp'
import login_6 from '@/assets/pictures/login-bg-6.webp'
import login_7 from '@/assets/pictures/login-bg-7.webp'
import login_8 from '@/assets/pictures/login-bg-8.webp'
import login_9 from '@/assets/pictures/login-bg-9.webp'
import login_10 from '@/assets/pictures/login-bg-10.webp'

const authStore = useAuthStore()
const { role } = useRole()
const router = useRouter()
const images = [login_1, login_2, login_3, login_4, login_5, login_6, login_7, login_8, login_9, login_10]
const currentBg = ref(0)
const showPassword = ref(false)
const loginError = ref(null)
const isSubmitting = ref(false)  // ← CSAK ez vezérli a gombot, semmi más
let interval = null


const form = reactive({
    email: '',
    password: '',
    rememberMe: false,
})

const submitLogin = async () => {
    loginError.value = null
    isSubmitting.value = true

    try {
        await authStore.login(form)
        await nextTick()

        if (role.value?.toLowerCase() === 'admin') {
            router.push('/admin-dashboard')
        } else {
            router.push('/user-dashboard')
        }
    } catch (error) {
        loginError.value = authStore.error ?? 'Sikertelen bejelentkezés.'
        console.error('Login failed:', error)
    } finally {
        // finally = mindig lefut, akár siker, akár hiba — sosem ragad bent a spinner
        isSubmitting.value = false
    }
}

onMounted(() => {
    interval = setInterval(() => {
        currentBg.value = (currentBg.value + 1) % images.length
    }, 10_000)
})

onUnmounted(() => {
    if (interval) clearInterval(interval)
})
</script>

<template>
    <div class="relative min-h-screen w-full overflow-hidden p-4 md:p-10 flex items-center justify-center">
        <div class="overlay bg-black/30 absolute inset-0 z-10"></div>
        <div class="absolute inset-0 z-0">
            <TransitionGroup name="fade">
                <div v-for="(img, index) in images" v-show="currentBg === index" :key="img"
                    class="absolute inset-0 bg-cover bg-center bg-no-repeat transition-transform duration-[10000ms] scale-110"
                    :style="{ backgroundImage: `url(${img})` }"></div>
            </TransitionGroup>
            <div class="absolute inset-0 bg-black/20"></div>
        </div>

        <div
            class="relative z-10 mx-auto flex min-h-[calc(100vh-5rem)] w-full max-w-[1500px] overflow-hidden rounded-[40px]">

            <!-- Bal panel -->
            <div class="relative hidden w-[45%] p-6 lg:block">
                <div
                    class="relative h-full w-full rounded-[36px] border-[5px] backdrop-blur-[2px] border-white/80 z-0 shadow-[0_0_0_100vw_#f5f5f3]">

                    <div class="absolute left-4 top-4 w-50 flex items-center gap-3 text-white">
                        <img src="@/assets/pictures/devcorner_full_logo.png" alt="">
                    </div>

                    <div class="absolute bottom-12 left-4 max-w-[480px] text-white p-4">
                        <h2
                            class="font-serif text-[70px] font-medium leading-[0.92] tracking-[-0.04em] drop-shadow-lg shadow-black/30">
                            Foglalások.<br>Egyszerűen.
                        </h2>
                        <p class="mt-6 text-sm leading-7 text-white/90 font-medium drop-shadow-md">
                            Egyszerű eszköz a foglalások gyors és átlátható kezeléséhez.
                        </p>
                    </div>
                </div>
            </div>

            <!-- Jobb panel — login form -->
            <div class="relative flex w-full items-center justify-center bg-[#f5f5f3] px-10 md:px-20 lg:w-[55%] z-20">
                <div class="w-full max-w-[460px]">

                    <div class="text-center mb-12 flex flex-col items-center">
                        <h1 class="text-6xl font-semibold tracking-tight">
                            <span class="bg-gradient-to-r from-blue-500 to-indigo-600 bg-clip-text text-transparent">
                                Host
                            </span>
                        </h1>
                        <p class="mt-4 text-[16px] text-black/55">Foglalások kezelése egyszerűen.</p>
                    </div>

                    <form class="space-y-5" @submit.prevent="submitLogin">
                        <div>
                            <label class="mb-2.5 block text-[15px] font-medium text-black/90">Email</label>
                            <input v-model="form.email" type="email" placeholder="Add meg az email címed"
                                autocomplete="email"
                                class="h-14 w-full rounded-xl border border-black/5 bg-white px-5 outline-none focus:ring-1 focus:ring-black/10 transition-all" />
                        </div>

                        <div>
                            <label class="mb-2.5 block text-[15px] font-medium text-black/90">Jelszó</label>
                            <div class="relative">
                                <input v-model="form.password" :type="showPassword ? 'text' : 'password'"
                                    placeholder="Add meg a jelszavad" autocomplete="current-password"
                                    class="h-14 w-full rounded-xl border border-black/5 bg-white px-5 outline-none focus:ring-1 focus:ring-black/10 transition-all" />
                                <button type="button" @click="showPassword = !showPassword"
                                    class="absolute right-5 top-1/2 -translate-y-1/2 opacity-30 hover:opacity-100 focus:outline-none transition-opacity">
                                    {{ showPassword ? '🔒' : '👁️' }}
                                </button>
                            </div>
                        </div>

                        <div class="flex items-center justify-between">
                            <div class="flex items-center gap-2">
                                <input v-model="form.rememberMe" type="checkbox" id="remember"
                                    class="h-4 w-4 rounded border-gray-300 accent-black cursor-pointer" />
                                <label for="remember"
                                    class="text-sm font-medium text-black/60 cursor-pointer">Emlékezzen rám</label>
                            </div>
                            <a href="#" class="text-sm font-medium text-black/80 hover:underline">Elfelejtette a
                                jelszavát?</a>
                        </div>

                        <Transition name="shake">
                            <div v-if="loginError"
                                class="rounded-xl bg-red-50 border border-red-200 px-4 py-3 text-sm text-red-600 font-medium">
                                {{ loginError }}
                            </div>
                        </Transition>

                        <div class="pt-4 space-y-4">
                            <!-- ✅ isSubmitting — NEM authStore.isLoading -->
                            <button type="submit" :disabled="isSubmitting"
                                class="h-14 w-full rounded-xl bg-black font-semibold text-white hover:bg-black/90 transition-all active:scale-[0.98] shadow-lg disabled:opacity-50 disabled:cursor-not-allowed disabled:active:scale-100">
                                <span v-if="isSubmitting" class="flex items-center justify-center gap-2">
                                    <svg class="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg"
                                        fill="none" viewBox="0 0 24 24">
                                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor"
                                            stroke-width="4" />
                                        <path class="opacity-75" fill="currentColor"
                                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                                    </svg>
                                    Bejelentkezés...
                                </span>
                                <span v-else>Bejelentkezés</span>
                            </button>
                        </div>
                    </form>

                    <p class="mt-10 text-center text-[13px] text-black/40">
                        Bármilyen probléma esetén forduljon hozzánk bizalommal! <br>
                        <a href="#" class="font-semibold text-black hover:underline">Kapcsolatfelvétel</a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.font-serif {
    font-family: 'Inter', sans-serif;
}

.fade-enter-active,
.fade-leave-active {
    transition: opacity 1.5s ease-in-out;
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}

.scale-110 {
    animation: slowZoom 20s infinite alternate linear;
}

@keyframes slowZoom {
    from {
        transform: scale(1);
    }

    to {
        transform: scale(1.1);
    }
}

.shake-enter-active {
    animation: shake 0.4s ease;
}

.shake-leave-active {
    transition: opacity 0.2s ease;
}

.shake-leave-to {
    opacity: 0;
}

@keyframes shake {

    0%,
    100% {
        transform: translateX(0);
    }

    20% {
        transform: translateX(-6px);
    }

    40% {
        transform: translateX(6px);
    }

    60% {
        transform: translateX(-4px);
    }

    80% {
        transform: translateX(4px);
    }
}
</style>