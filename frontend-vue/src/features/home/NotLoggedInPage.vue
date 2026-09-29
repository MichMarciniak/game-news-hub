<script setup lang="ts">
import EventWeekList from './EventWeekList.vue';

</script>

<template>
  <div class="flex flex-col max-w-7xl w-full gap-4 h-full items-center">

    <!-- Hero: bez Panelu, sam tekst i przyciski -->
    <section class="flex flex-col items-center text-center gap-4 py-8">
      <h1 class="text-4xl font-semibold">Some title</h1>
      <p class="text-muted-color max-w-2xl">
        App tracks showcases, direct presentations, and release events,
        and shows you the ones you care about.
      </p>
      <div class="flex gap-3">
        <RouterLink to="/register"><Button label="Sign up" /></RouterLink>
        <RouterLink to="/login"><Button label="Log in" variant="outlined" /></RouterLink>
      </div>
    </section>

    <!-- Panele z listami -->
    <EventWeekList />

    <!-- Wezwanie do akcji na dole -->
    <p class="text-center text-muted-color">
      Log in to get recommendations based on your favorite games, genres, and platforms.
    </p>
  </div>
</template>