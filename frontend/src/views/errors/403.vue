<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

function goBack() {
    router.back()
}
</script>

<template>
    <section class="error-page">
        <h1>403</h1>
        <h2>Ehhez az oldalhoz nincs jogosultságod.</h2>
        <p>
            Szép próbálkozás volt, de ez most nem a te pályád.
        </p>

        <div class="actions">
            <button @click="goBack">Vissza</button>
            <RouterLink to="/">Bejelentkezés</RouterLink>
        </div>
    </section>
</template>

<style scoped></style>