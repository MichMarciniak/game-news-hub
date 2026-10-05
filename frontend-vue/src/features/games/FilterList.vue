<script setup lang="ts">
import { useGetGenres, useGetPlatformsGroups } from '@/api/generated';

export interface FilterOption {
  id: number | string,
  name: string
}

const props = defineProps<{
  genres: FilterOption[],
  platforms: FilterOption[]
}>()


const selectedGenres = defineModel<Array<number | string>>('genres', {default: () => []})
const selectedPlatforms = defineModel<Array<number | string>>('platforms', {default: () => []})

const {data: genreData} = useGetGenres({
  query: {staleTime: Infinity, gcTime: Infinity}
})

const {data: platformData} = useGetPlatformsGroups({
  query: {staleTime: Infinity, gcTime: Infinity}
})

</script>


<template>

  <Accordion :value="['0','1']" multiple>
    <AccordionPanel value="0">
      <AccordionHeader>Platforms</AccordionHeader>
      <AccordionContent>
        <div class="flex flex-col gap-2 pt-1">
          <div v-for="platform in platformData" :key="platform.id" class="flex items-center gap-2 cursor-pointer">
            <Checkbox 
              :input-id="`platform-${platform.id}`"
              :value="platform.id"
              v-model="selectedPlatforms"
            />
            <label 
              :for="`platform-${platform.id}`"
              class="text-sm text-color cursor-pointer select-none"
            >
              {{ platform.name }}
            </label>
          </div>

        </div>
      </AccordionContent>
    </AccordionPanel>
  <AccordionPanel value="1">
      <AccordionHeader>Genres</AccordionHeader>
      <AccordionContent>
        <div class="flex flex-col gap-2 pt-1">
          <div v-for="genre in genreData" :key="genre.id" class="flex items-center gap-2 cursor-pointer">
            <Checkbox 
              :input-id="`genre-${genre.id}`"
              :value="genre.id"
              v-model="selectedGenres"
            />
            <label 
              :for="`genre-${genre.id}`"
              class="text-sm text-color cursor-pointer select-none"
            >
              {{ genre.name }}
            </label>
          </div>

        </div>
      </AccordionContent>
    </AccordionPanel>
  </Accordion>

</template>