<!-- AppPaginator.vue -->
<script setup lang="ts">
import type { PageState } from 'openvue';
import { computed } from 'vue'


const props = withDefaults(
  defineProps<{
    totalRecords?: number | string | undefined
    rowsPerPageOptions?: number[]
  }>(),
  {
    totalRecords: 0,
    rowsPerPageOptions: () => [12, 24, 48],
  }
)

const page = defineModel<number | string | undefined>('page', { required: true })
const pageSize = defineModel<number | string | undefined>('pageSize', { required: true })

const first = computed(() => (Number(page.value) - 1) * Number(pageSize.value))

const handlePageChange = (event: PageState) => {
  page.value = event.page + 1
  pageSize.value = event.rows
}
</script>

<template>
  <div class="mt-4">
    <Paginator
      :first="first"
      :rows="Number(pageSize)"
      :total-records="Number(totalRecords)"
      :rows-per-page-options="rowsPerPageOptions"
      template="FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink JumpToPageInput RowsPerPageDropdown"
      @page="handlePageChange"
    />
  </div>
</template>