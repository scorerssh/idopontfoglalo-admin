<script setup>
const videoSrc = new URL('../assets/videos/2026-04-11 17-51-57.mp4', import.meta.url).href
</script>

<template>
    <div class="flex flex-col items-center justify-center min-h-[60vh] px-4">
        <video
            :src="videoSrc"
            controls
            class="w-full max-w-4xl rounded-2xl shadow-lg border border-gray-200"
        />
    </div>
</template>

<style scoped></style>
