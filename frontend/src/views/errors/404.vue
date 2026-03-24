<script setup>
import notFountImage from '@assets/pictures/not_found_img.webp'
</script>


<template>
    <section class="error-page min-h-screen w-full flex flex-col items-center justify-center gap-6 text-center p-6">
        <img :src=notFountImage alt="404 Error" class="w-64 h-64 object-contain" />
        <h1 class="font-semibold text-6xl text-orange-600">404</h1>
        <h2 class="text-2xl text-blacks font-semibold">Ez az oldal nem található.</h2>
        <p class="text-md text-black/70">

            Az oldal vagy nem létezik, vagy valahol félúton eltévedt a bitek erdejében.
        </p>

        <div class="actions ">
            <RouterLink to="/">Főoldal</RouterLink>
        </div>
    </section>
</template> 

<style scoped></style>