<script setup lang="ts">
import { useEventSearchService } from '@/features/events/services/useEventSearchService';
import DebouncedInput from '@/features/shared/DebouncedInput.vue';
import { OiArrowDown, OiArrowUp } from '@openvue/openicons-vue';
import EventItem from '@/features/events/components/EventItem.vue';
import SkeletonEventItem from '@/features/shared/skeletons/SkeletonEventItem.vue';
import SortButton from '@/features/shared/SortButton.vue';
import FadeTransition from '@/features/shared/transitions/FadeTransition.vue';
import MyPaginator from '@/features/shared/MyPaginator.vue';

const gridCols = 'grid grid-cols-[1rem_1fr_11rem_11rem_auto] items-center gap-4'

const {
  searchQuery,
  sortOptions,
  eventsData,
  isPending,
  onPageChange
} = useEventSearchService()
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
      <Select
        v-model="searchQuery.sortBy"
        :options="sortOptions"
        option-label="label"
        option-value="value"
        placeholder="Sort by..."
        class="flex-2"
      ></Select>
      <SortButton v-model="searchQuery.descending"/>
    </div>

     <div class="flex gap-6 flex-1">
      <!-- Main Content -->
      <section class="flex-1 flex flex-col">
        <div class="hidden md:flex justify-between items-center mb-4">
          <span>Found: {{ eventsData?.total ?? 0 }} events</span>
          <MyPaginator 
            variant="simple"
            v-model:page="searchQuery.page"
            v-model:page-size="searchQuery.pageSize"
            :total-records="eventsData?.total"
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

        <div
          :class="gridCols"
          class="hidden md:grid px-4 py-2 text-sm font-semibold text-muted-color border-b mb-2"
        >
          <span></span>
          <span>Name</span>
          <span>Start</span>
          <span>End</span>
          <span class="w-10"></span>

        </div>

        <!-- Event list -->

        <FadeTransition>

          <div 
            v-if="isPending" 
            key="skeleton" 
            class="flex flex-col gap-2"
          >
            <SkeletonEventItem v-for="i in searchQuery.pageSize" :key="`skeleton-${i}`" />
          </div>
        
          <div v-else 
            :key="`${searchQuery.page}-${searchQuery.pageSize}-${searchQuery.query}-${searchQuery.sortBy}-${searchQuery.descending}`"
            class="flex flex-col gap-2"
          >
          <EventItem v-for="event in eventsData?.items" :event="event" :key="event.id"></EventItem>
          </div>
        </FadeTransition>
          

        <div class="mt-4">        
          <MyPaginator 
          variant="default"
          v-model:page="searchQuery.page"
          v-model:page-size="searchQuery.pageSize"
          :total-records="eventsData?.total"
          />
        </div>
          
      </section>
    </div>
  </div>
</template>
