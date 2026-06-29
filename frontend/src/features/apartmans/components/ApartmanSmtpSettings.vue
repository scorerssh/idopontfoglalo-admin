<script setup>
import { Mail, Save, Trash2 } from 'lucide-vue-next'
import { useApartmanStore } from '@/features/apartmans/stores/apartman.store'

const DEFAULT_GUEST_EMAIL_INTRO_TEMPLATE =
    'Köszönjük a foglalást. Az alábbi adatokkal rögzítettük a foglalásodat a(z) {apartmanName} apartmanhoz.'
const DEFAULT_GUEST_SMS_TEMPLATE =
    'Kedves {guestName}! Foglalásodat rögzítettük: {apartmanName}, {roomName}, érkezés: {startDate}, távozás: {endDate}. Végösszeg: {totalPrice}.'

const props = defineProps({
    apartmanId: { type: Number, required: true },
})

const apartmanStore = useApartmanStore()
const smtpExists = ref(false)

const formData = reactive({
    apartmanId: null,
    host: '',
    port: 587,
    userName: '',
    password: '',
    senderEmail: '',
    senderName: '',
    guestEmailIntroTemplate: DEFAULT_GUEST_EMAIL_INTRO_TEMPLATE,
    guestSmsTemplate: DEFAULT_GUEST_SMS_TEMPLATE,
    useSsl: true,
    isEnabled: true,
    hasPassword: false,
})

function applySetting(setting) {
    smtpExists.value = Boolean(setting)
    formData.apartmanId = props.apartmanId
    formData.host = setting?.host ?? ''
    formData.port = setting?.port ?? 587
    formData.userName = setting?.userName ?? ''
    formData.password = ''
    formData.senderEmail = setting?.senderEmail ?? ''
    formData.senderName = setting?.senderName ?? ''
    formData.guestEmailIntroTemplate = setting?.guestEmailIntroTemplate ?? DEFAULT_GUEST_EMAIL_INTRO_TEMPLATE
    formData.guestSmsTemplate = setting?.guestSmsTemplate ?? DEFAULT_GUEST_SMS_TEMPLATE
    formData.useSsl = setting?.useSsl ?? true
    formData.isEnabled = setting?.isEnabled ?? true
    formData.hasPassword = setting?.hasPassword ?? false
}

async function loadSmtpSetting() {
    if (!props.apartmanId) return
    const setting = await apartmanStore.getSmtpSetting(props.apartmanId)
    applySetting(setting)
}

async function saveSmtpSetting() {
    const payload = {
        apartmanId: props.apartmanId,
        host: formData.host,
        port: Number(formData.port),
        userName: formData.userName || null,
        password: formData.password || null,
        senderEmail: formData.senderEmail,
        senderName: formData.senderName || null,
        guestEmailIntroTemplate: formData.guestEmailIntroTemplate || null,
        guestSmsTemplate: formData.guestSmsTemplate || null,
        useSsl: formData.useSsl,
        isEnabled: formData.isEnabled,
    }

    const saved = await apartmanStore.upsertSmtpSetting(payload)
    applySetting(saved)
}

async function deleteSmtpSetting() {
    if (!smtpExists.value) return
    if (!confirm('Biztosan törlöd az SMTP beállítást?')) return

    await apartmanStore.deleteSmtpSetting(props.apartmanId)
    applySetting(null)
}

watch(() => props.apartmanId, loadSmtpSetting, { immediate: true })
</script>

<template>
    <section class="smtp-settings border-t border-gray-200 pt-4 mt-4">
        <div class="flex items-center justify-between gap-3 mb-3">
            <div class="flex items-center gap-2 min-w-0">
                <div class="h-9 w-9 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center shrink-0">
                    <Mail class="h-5 w-5" />
                </div>
                <div class="min-w-0">
                    <h3 class="text-sm font-bold text-gray-900 leading-tight">SMTP beállítások</h3>
                    <p class="text-xs text-gray-500 truncate">
                        {{ smtpExists ? 'Aktív apartman email fiók' : 'Nincs mentett SMTP fiók' }}
                    </p>
                </div>
            </div>
            <label class="flex items-center gap-2 text-xs font-semibold text-gray-600">
                <input v-model="formData.isEnabled" type="checkbox" class="h-4 w-4 accent-blue-600">
                Aktív
            </label>
        </div>

        <form class="grid grid-cols-1 md:grid-cols-2 gap-3" @submit.prevent="saveSmtpSetting">
            <div class="md:col-span-2 grid grid-cols-1 md:grid-cols-[1fr_120px] gap-3">
                <label class="flex flex-col gap-1 text-sm text-black/60">
                    SMTP szerver
                    <input v-model="formData.host" required maxlength="255"
                        class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100">
                </label>
                <label class="flex flex-col gap-1 text-sm text-black/60">
                    Port
                    <input v-model.number="formData.port" required type="number" min="1" max="65535"
                        class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100">
                </label>
            </div>

            <label class="flex flex-col gap-1 text-sm text-black/60">
                Felhasználónév
                <input v-model="formData.userName" maxlength="255" autocomplete="off"
                    class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100">
            </label>
            <label class="flex flex-col gap-1 text-sm text-black/60">
                Jelszó
                <input v-model="formData.password" type="password" maxlength="1024" autocomplete="new-password"
                    :placeholder="formData.hasPassword ? 'Már van mentett jelszó' : ''"
                    class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100">
            </label>

            <label class="flex flex-col gap-1 text-sm text-black/60">
                Feladó email
                <input v-model="formData.senderEmail" required type="email" maxlength="255"
                    class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100">
            </label>
            <label class="flex flex-col gap-1 text-sm text-black/60">
                Feladó név
                <input v-model="formData.senderName" maxlength="100"
                    class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100">
            </label>

            <div class="md:col-span-2 grid grid-cols-1 lg:grid-cols-2 gap-3 border-t border-gray-200 pt-3 mt-1">
                <label class="flex flex-col gap-1 text-sm text-black/60">
                    Email beköszönő szöveg
                    <textarea v-model="formData.guestEmailIntroTemplate" maxlength="1000" rows="4"
                        class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100 resize-y"></textarea>
                </label>
                <label class="flex flex-col gap-1 text-sm text-black/60">
                    SMS sablon
                    <textarea v-model="formData.guestSmsTemplate" maxlength="1000" rows="4"
                        class="px-3 py-2 w-full bg-gray-200 focus:ring-2 ring-0 ring-blue-500 rounded-lg outline-none transition-all duration-100 resize-y"></textarea>
                </label>
            </div>

            <div class="md:col-span-2 flex flex-col sm:flex-row sm:items-center justify-between gap-3 pt-1">
                <label class="flex items-center gap-2 text-sm font-semibold text-gray-600">
                    <input v-model="formData.useSsl" type="checkbox" class="h-4 w-4 accent-blue-600">
                    SSL/TLS
                </label>

                <div class="flex items-center justify-end gap-2">
                    <button v-if="smtpExists" type="button" @click="deleteSmtpSetting"
                        class="inline-flex items-center gap-2 px-3 py-2 text-sm rounded-lg text-red-600 border border-red-200 hover:bg-red-50 transition-colors">
                        <Trash2 class="h-4 w-4" />
                        Törlés
                    </button>
                    <button type="submit"
                        class="inline-flex items-center gap-2 px-3 py-2 text-sm rounded-lg bg-blue-600 text-white hover:bg-blue-700 transition-colors">
                        <Save class="h-4 w-4" />
                        SMTP mentés
                    </button>
                </div>
            </div>
        </form>
    </section>
</template>
