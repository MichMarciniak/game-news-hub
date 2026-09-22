// composables/useSearchQuerySync.ts
import { useRoute, useRouter } from 'vue-router'
import { watch, type Ref } from 'vue'


export function useSearchQuerySync<T extends Record<string, any>>(
  searchQuery: Ref<T>,
  options?: { arrayKeys?: (keyof T)[] }
) {
  const route = useRoute()
  const router = useRouter()
  const arrayKeys = new Set(options?.arrayKeys ?? [])

  // 1. Wczytaj stan z URL przy starcie (np. po powrocie "wstecz" albo wejściu z linku)
  function loadFromUrl() {
    const query = route.query
    const restored: Record<string, any> = { ...searchQuery.value }

    for (const key in searchQuery.value) {
      if (!(key in query)) continue
      const rawValue = query[key]

      if (arrayKeys.has(key)) {
        // query param może być stringiem albo tablicą stringów w zależności od tego ile wartości
        restored[key] = Array.isArray(rawValue) ? rawValue : [rawValue]
      } else if (typeof searchQuery.value[key] === 'number') {
        restored[key] = Number(rawValue)
      } else if (typeof searchQuery.value[key] === 'boolean') {
        restored[key] = rawValue === 'true'
      } else {
        restored[key] = rawValue
      }
    }

    searchQuery.value = restored as T
  }

  // 2. Zapisz stan do URL przy każdej zmianie searchQuery
  function syncToUrl() {
    const query: Record<string, any> = {}

    for (const key in searchQuery.value) {
      const value = searchQuery.value[key]
      // pomiń puste/domyślne wartości, żeby URL nie był zaśmiecony
      if (value === undefined || value === null || value === '') continue
      if (Array.isArray(value) && value.length === 0) continue
      query[key] = value
    }

    router.replace({ query })
  }

  loadFromUrl()

  watch(searchQuery, syncToUrl, { deep: true })

  return { loadFromUrl, syncToUrl }
}