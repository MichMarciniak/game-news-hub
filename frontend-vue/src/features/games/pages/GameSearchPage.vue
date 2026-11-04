<script setup lang="ts">
import { ref } from 'vue'
import { useGameSearchService } from '@/features/games/services/useGameSearchService'
import DebouncedInput from '@/features/shared/DebouncedInput.vue'
import { OiArrowDown, OiArrowUp, OiFilter } from '@openvue/openicons-vue'
import GameCard from '@/features/games/components/GameCard.vue'
import SkeletonGameCard from '@/features/shared/skeletons/SkeletonGameCard.vue'
import { keepPreviousData } from '@tanstack/vue-query'
import FilterList from '@/features/games/components/FilterList.vue'
import GameGrid from '@/features/games/components/GameGrid.vue'
import SortButton from '@/features/shared/SortButton.vue'
import FadeTransition from '@/features/shared/transitions/FadeTransition.vue'
import MyPaginator from '@/features/shared/MyPaginator.vue'

const {
  searchQuery,
  sortOptions,
  gamesData,
  isPending,
  onPageChange
} = useGameSearchService()

const isMobileOpen = ref<boolean>(false)

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
        v-model:genres="searchQuery.genreIds!"
        v-model:platforms="searchQuery.platformIds!"
      />
    </Drawer>
     <div class="flex gap-6 flex-1">
      <!-- Desktop filters/sort (po lewej)-->
      <aside class="hidden md:block w-64 shrink-0">
        <FilterList
          v-model:genres="searchQuery.genreIds!"
          v-model:platforms="searchQuery.platformIds!"
        />
      </aside>

      <!-- Main Content -->
      <section class="flex-1 flex flex-col">
        <div class="hidden md:flex justify-between items-center mb-4">
          <span>Found: <strong>{{ gamesData?.total ?? 0 }}</strong> games</span>
          <MyPaginator
            variant="simple"
            v-model:page="searchQuery.page"
            v-model:page-size="searchQuery.pageSize"
            :total-records="gamesData?.total"
            class="not-lg:hidden"
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
        <FadeTransition>
          <GameGrid :key="isPending ? 'skeleton' : `${searchQuery.page}-${searchQuery.pageSize}-${searchQuery.query}-${searchQuery.sortBy}-${searchQuery.descending}`">
            <template v-if="isPending">
              <SkeletonGameCard v-for="num in searchQuery.pageSize" :key="`skeleton-${num}`" />
            </template>
            <template v-else>
              <GameCard v-for="game in gamesData?.items" :key="game.id" :game="game" />
            </template>
          </GameGrid>
        </FadeTransition>

        <!-- Paginator -->
        <div class="mt-4">
          <MyPaginator
            variant="default"
            v-model:page="searchQuery.page"
            v-model:page-size="searchQuery.pageSize"
            :total-records="gamesData?.total"
          />
        </div>
      </section>
    </div>
  </div>
</template>
