<script setup lang="ts">
import type { PageState } from 'openvue';
import { computed } from 'vue'


const props = withDefaults(
  defineProps<{
    totalRecords?: number | string | undefined
    rowsPerPageOptions?: number[]
    variant?: 'default' | 'simple'
  }>(),
  {
    totalRecords: 0,
    rowsPerPageOptions: () => [12, 24, 48],
    variant: 'default',
  }
)

const page = defineModel<number | string | undefined>('page', { required: true })
const pageSize = defineModel<number | string | undefined>('pageSize', { required: true })

const template = computed(() => {
  if (props.variant === 'simple') {
    return 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink'
  }
  return 'FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink JumpToPageInput RowsPerPageDropdown'
})

const first = computed(() => (Number(page.value) - 1) * Number(pageSize.value))

const handlePageChange = (event: PageState) => {
  page.value = event.page + 1
  pageSize.value = event.rows
}
</script>

<template>
  <Paginator
    :first="first"
    :rows="Number(pageSize)"
    :total-records="Number(totalRecords)"
    :rows-per-page-options="variant === 'default' ? rowsPerPageOptions : undefined"
    :template="template"
    @page="handlePageChange"
  />
</template>