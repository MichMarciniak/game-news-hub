<script setup lang="ts">
import { useGetEventSearch } from '@/api/generated';
import { EventSortBy, type GetEventSearchParams, } from '@/api/generated/models';
import type { PageState } from 'openvue';
import { ref, watch } from 'vue';
import DebouncedInput from '../shared/DebouncedInput.vue';
import { keepPreviousData } from '@tanstack/vue-query';
import { OiArrowDown, OiArrowUp } from '@openvue/openicons-vue';
import EventItem from './EventItem.vue';

const gridCols = 'grid grid-cols-[1fr_9rem_9rem_auto] items-center gap-4'

const searchQuery = ref<GetEventSearchParams>({
  query: undefined,
  page: 1,
  pageSize: 12,
  descending: true,
  sortBy: EventSortBy.Relevance
})

const sortOptions = [
  {label: 'Relevance', value: EventSortBy.Relevance},
  {label: 'Title', value: EventSortBy.Name},
  {label: 'Popularity', value: EventSortBy.Popularity},
  {label: 'Date', value: EventSortBy.Date}
]

const {data: eventsData, isPending, isPlaceholderData, isFetching} = useGetEventSearch(searchQuery, {
  query: {
    placeholderData: keepPreviousData
  }
})

watch(
  () => [searchQuery.value.query, searchQuery.value.sortBy],
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
      <Select
        v-model="searchQuery.sortBy"
        :options="sortOptions"
        option-label="label"
        option-value="value"
        placeholder="Sort by..."
        class="flex-2"
      ></Select>
      <Button
        class="h-[2.7rem]!"
        variant="outlined"
        :aria-label="searchQuery.descending ? 'Descending' : 'Ascending'"
        @click="searchQuery.descending = !searchQuery.descending"
      >
        <OiArrowDown v-if="searchQuery.descending"/>
        <OiArrowUp v-else/>
      </Button>
    </div>

     <div class="flex gap-6 flex-1">
      <!-- Main Content -->
      <section class="flex-1 flex flex-col">
        <div class="hidden md:flex justify-between items-center mb-4">
          <span>Found: {{ eventsData?.total ?? 0 }} events</span>
          <div class="flex items-center gap-2">
            <span>Sort: </span>
            <Select
              v-model="searchQuery.sortBy"
              :options="sortOptions"
              option-label="label"
              option-value="value"
            />
            <Button
              class="h-[2.7rem]!"
              variant="outlined"
              :aria-label="searchQuery.descending ? 'Descending' : 'Ascending'"
              @click="searchQuery.descending = !searchQuery.descending"
            >
              <OiArrowDown v-if="searchQuery.descending"/>
              <OiArrowUp v-else/>
            </Button>
          </div> 
        </div>

        <div
          :class="gridCols"
          class="hidden md:grid px-4 py-2 text-sm font-semibold text-muted-color border-b"
        >
          <span>Name</span>
          <span>Start</span>
          <span>End</span>
          <span class="w-10"></span>

        </div>

        <!-- Event list -->


        <template v-if="isPending">
          <p>test</p>
        </template>
        <template v-else>
          <EventItem v-for="event in eventsData?.items" :event="event" :key="event.id"></EventItem>
        </template>
          

        <!-- Paginator -->
        <div class="mt-4">
          <Paginator
            :first="(searchQuery.page as number - 1) * (searchQuery.pageSize as number)"
            :rows="searchQuery.pageSize as number"
            :total-records="eventsData?.total as number ?? 0"
            :rows-per-page-options="[12, 24, 48]"
            template="FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink JumpToPageInput RowsPerPageDropdown"
            @page="onPageChange"
          />
        </div>

      </section>
    </div>
  </div>
</template>