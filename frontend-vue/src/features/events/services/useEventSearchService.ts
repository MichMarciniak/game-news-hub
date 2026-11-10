import { useGetEventsSearch } from '@/api/generated'
import { EventSortBy, type GetEventsSearchParams } from '@/api/generated/models'
import type { PageState } from 'openvue'
import { ref, watch } from 'vue'
import { keepPreviousData } from '@tanstack/vue-query'
import { useSearchQuerySync } from '@/utils/saveSearch.ts'

export function useEventSearchService() {
  const searchQuery = ref<GetEventsSearchParams>({
    query: undefined,
    page: 1,
    pageSize: 12,
    descending: true,
    sortBy: EventSortBy.Relevance
  })

  useSearchQuerySync(searchQuery)

  const sortOptions = [
    { label: 'Relevance', value: EventSortBy.Relevance },
    { label: 'Title', value: EventSortBy.Name },
    { label: 'Popularity', value: EventSortBy.Popularity },
    { label: 'Date', value: EventSortBy.Date }
  ]

  const { data: eventsData, isPending } = useGetEventsSearch(searchQuery, {
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

  return {
    searchQuery,
    sortOptions,
    eventsData,
    isPending,
    onPageChange
  }
}
