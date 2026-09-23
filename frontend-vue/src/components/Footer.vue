<script setup lang="ts">
import { OiGithub } from '@openvue/openicons-vue';

</script>


<template>

  <footer class="min-h-8 flex flex-col items-center justify-center pb-2 shrink-0">
    <Divider class="max-w-[95%]" />
    <div class="flex gap-3">
      <a href="https://github.com/MichMarciniak/game-news-hub" rel="noreferrer noopener" target="_blank" class="link ul">
        <OiGithub size="1.2rem"/>
        <span>Github</span>
      </a>
      <Divider layout="vertical" class="m-0 p-0" />
      <span>Michał Marciniak</span>
    </div>
    <span>Data from <a href="https://igdb.com" rel="noreferrer noopener" target="_blank" class="ul">igdb.com</a></span>
  </footer>
</template>

<style scoped>
.link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.ul:hover {
  text-decoration: underline;
}
footer {
  color: var(--p-text-muted-color);
}
</style>