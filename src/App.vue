<script setup>
import { ref, watch, onBeforeUnmount } from 'vue'
import { storeToRefs } from 'pinia'
import { RouterView } from 'vue-router'
import NotifierModal from '@components/NotifierModal.vue'
import { useNotificationStore } from '@/stores/notifcation'

const notificationStore = useNotificationStore()
const { message, id } = storeToRefs(notificationStore)

const isMainModalOpen = ref(false)
const mainModalContent = ref('')
let timeoutId = null

watch(
  () => id.value,
  (newId) => {
    if (!newId || !message.value) return

    mainModalContent.value = message.value
    isMainModalOpen.value = true

    // ha volt korábbi timer, töröljük
    if (timeoutId) {
      clearTimeout(timeoutId)
    }

    // 5 másodperc után automatikusan zárjuk
    timeoutId = setTimeout(() => {
      isMainModalOpen.value = false
      notificationStore.clear()
      timeoutId = null
    }, 5000)
  },
)

// Manuális zárás (X gomb)
function handleMainModalClose() {
  isMainModalOpen.value = false
  notificationStore.clear()
  if (timeoutId) {
    clearTimeout(timeoutId)
    timeoutId = null
  }
}

onBeforeUnmount(() => {
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
})
</script>

<template>
  <RouterView />
  <NotifierModal :content="mainModalContent" :is-open="isMainModalOpen" @close="handleMainModalClose" />
</template>

<style scoped></style>
