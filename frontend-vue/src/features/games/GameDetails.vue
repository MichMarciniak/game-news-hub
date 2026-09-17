
<script lang="ts" setup>
import { useGetGameGameId } from '@/api/generated';
import { type GameDetailDto  } from '@/api/generated/models';
import { unwrap } from '@/api/utils';
import { computed } from 'vue';


const props = defineProps<{
  id: string
}>()

const gameId = computed(() => String(props.id))

const { data: gameResponse, isLoading } = useGetGameGameId(gameId)
const game = computed(() => unwrap<GameDetailDto>(gameResponse.value))

console.log(props)
console.log(gameId)
console.log(game)

</script>

<template>
  <p v-if="isLoading">Loading...</p>
  <Card v-else-if="game" class="bg-highlight">
    <template #title>{{ game.title }}</template>
    <template #content>
      <ul v-for="genre in game.genres" :key="genre.id">
        <li :key="genre.id">{{genre.name}}</li>
      </ul>
    </template>
  </Card> 
</template>