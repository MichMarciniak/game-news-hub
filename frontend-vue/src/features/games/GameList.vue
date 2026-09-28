<script setup lang="ts">
import { useGetGameSearch } from '@/api/generated';
import { GameSortBy, type GetGameSearchParams } from '@/api/generated/models';
import type { PageState } from 'openvue';
import { ref, watch } from 'vue';
import DebouncedInput from '../shared/DebouncedInput.vue';
import { OiArrowDown, OiArrowUp, OiFilter } from '@openvue/openicons-vue';
import GameCard from './GameCard.vue';
import SkeletonGameCard from '../shared/skeletons/SkeletonGameCard.vue';
import { keepPreviousData } from '@tanstack/vue-query';
import FilterList from '@/features/games/FilterList.vue';
import GameGrid from './GameGrid.vue';
import { useSearchQuerySync } from '@/utils/saveSearch.ts';
import SortButton from '../shared/SortButton.vue';

const searchQuery = ref<GetGameSearchParams>({
  query: undefined,
  page: 1,
  pageSize: 12,
  descending: true,
  sortBy: GameSortBy.Relevance,
  genreIds: [],
  platformIds: []
})

useSearchQuerySync(searchQuery)

const sortOptions = [
  {label: 'Relevance', value: GameSortBy.Relevance},
  {label: 'Title', value: GameSortBy.Name},
  {label: 'Popularity', value: GameSortBy.Popularity}
]

const isMobileOpen = ref<boolean>(false)
const {data: gamesData, isPending} = useGetGameSearch(searchQuery, {
  query: {
    placeholderData: keepPreviousData
  }
})

watch(
  () => [searchQuery.value.query, searchQuery.value.genreIds, searchQuery.value.platformIds, searchQuery.value.sortBy],
  () => { searchQuery.value.page = 1 },
  { deep: true }
)

function onPageChange(event: PageState) {
  searchQuery.value.page = event.page + 1
  searchQuery.value.pageSize = event.rows
}

</script>

<template>
  <div class="w-full max-w-7xl flex flex-col">
    <div class="mb-4">
      <DebouncedInput
      v-model="searchQuery.query"
      :delay="400"
      placeholder='Search...'
      />
    </div>

    <!-- Mobile filters/sort -->
    <div class="flex md:hidden justify-between items-center mb-4 gap-2">
      <Button 
        :label="`Filters`"
        @click="isMobileOpen = true"
        variant="outlined"
        class="flex-1"
      >
        <OiFilter />
      </Button>
      <Select
        v-model="searchQuery.sortBy"
        :options="sortOptions"
        option-label="label"
        option-value="value"
        placeholder="Sort by..."
        class="flex-2"
      />
      <SortButton v-model="searchQuery.descending"/>
    </div>

    <Drawer
      v-model:visible="isMobileOpen"
      header="Filters"
      position="left"
      class="md:hidden"
    >
      <FilterList
        v-model:genres="searchQuery.genreIds"
        v-model:platforms="searchQuery.platformIds"
      />
    </Drawer>
     <div class="flex gap-6 flex-1">
      <!-- Desktop filters/sort (po lewej)-->
      <aside class="hidden md:block w-64 shrink-0">
        
        <FilterList 
          v-model:genres="searchQuery.genreIds"
          v-model:platforms="searchQuery.platformIds"
        />
      </aside>

      <!-- Main Content -->
      <section class="flex-1 flex flex-col">
        <div class="hidden md:flex justify-between items-center mb-4">
          <span>Found: <strong>{{ gamesData?.total ?? 0 }}</strong> games</span>
          <Paginator
            :first="(searchQuery.page as number - 1) * (searchQuery.pageSize as number)"
            :rows="searchQuery.pageSize as number"
            :total-records="gamesData?.total as number ?? 0"
            template="FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
            class="not-lg:hidden"
            @page="onPageChange"
          />
          <div class="flex items-center gap-2">
            <span>Sort: </span>
            <Select
              v-model="searchQuery.sortBy"
              :options="sortOptions"
              option-label="label"
              option-value="value"
            />
            <SortButton v-model="searchQuery.descending"/>
          </div> 
        </div>

        <!-- Game grid -->
        <Transition
          mode="out-in"
          enter-active-class="transition-opacity duration-200 ease-out"
          enter-from-class="opacity-0"
          leave-active-class="transition-opacity duration-150 ease-in"
          leave-to-class="opacity-0"
        >
          <GameGrid :key="isPending ? 'skeleton' : `${searchQuery.page}-${searchQuery.pageSize}-${searchQuery.query}-${searchQuery.sortBy}-${searchQuery.descending}`">
            <template v-if="isPending">
              <SkeletonGameCard v-for="num in searchQuery.pageSize" :key="`skeleton-${num}`" />
            </template>
            <template v-else>
              <GameCard v-for="game in gamesData?.items" :key="game.id" :game="game" />
            </template>
          </GameGrid>
        </Transition>

        <!-- Paginator -->
        <div class="mt-4">
          <Paginator
            :first="(searchQuery.page as number - 1) * (searchQuery.pageSize as number)"
            :rows="searchQuery.pageSize as number"
            :total-records="gamesData?.total as number ?? 0"
            :rows-per-page-options="[12, 24, 48]"
            template="FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink JumpToPageInput RowsPerPageDropdown"
            @page="onPageChange"
          />
        </div>

      </section>
    </div>
  </div>
</template>