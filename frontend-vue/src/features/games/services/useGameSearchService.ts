import { useGetGamesSearch } from '@/api/generated'
import { GameSortBy, type GetGamesSearchParams } from '@/api/generated/models'
import type { PageState } from 'openvue'
import { ref, watch } from 'vue'
import { keepPreviousData } from '@tanstack/vue-query'
import { useSearchQuerySync } from '@/utils/saveSearch.ts'

export function useGameSearchService() {
  const searchQuery = ref<GetGamesSearchParams>({
    query: undefined,
    page: 1,
    pageSize: 12,
    descending: true,
    sortBy: GameSortBy.Relevance,
    genreIds: [],
    platformIds: []
  })

  useSearchQuerySync(searchQuery, { arrayKeys: ['genreIds', 'platformIds'], numberKeys: ['genreIds', 'platformIds'] })

  const sortOptions = [
    { label: 'Relevance', value: GameSortBy.Relevance },
    { label: 'Title', value: GameSortBy.Name },
    { label: 'Popularity', value: GameSortBy.Popularity }
  ]

  const { data: gamesData, isPending } = useGetGamesSearch(searchQuery, {
    query: {
      placeholderData: keepPreviousData
    }
  })

  watch(
    () => searchQuery.value.query,
    () => { searchQuery.value.page = 1 }
  )
  watch(
    () => searchQuery.value.sortBy,
    () => { searchQuery.value.page = 1 }
  )
  watch(
    () => (searchQuery.value.genreIds ?? []).join(','),
    () => { searchQuery.value.page = 1 }
  )
  watch(
    () => (searchQuery.value.platformIds ?? []).join(','),
    () => { searchQuery.value.page = 1 }
  )

  function onPageChange(event: PageState) {
    searchQuery.value.page = event.page + 1
    searchQuery.value.pageSize = event.rows
  }

  return {
    searchQuery,
    sortOptions,
    gamesData,
    isPending,
    onPageChange
  }
}
