<script setup lang="ts">
import { getGetAdminSeriesSearchInfiniteQueryKey, useGetAdminSeriesSearchInfinite, useGetEventSearchInfinite, usePatchAdminReassignSeries } from '@/api/generated';
import { keepPreviousData, useQueryClient } from '@tanstack/vue-query';
import { computed, ref } from 'vue';
import DebouncedInput from '../shared/DebouncedInput.vue';
import { useConfirm, useToast } from 'openvue';

const eventQuery = ref('')
const seriesQuery = ref('')

const EVENT_PAGE_SIZE = 20;
const SERIES_PAGE_SIZE = 12;

const queryClient = useQueryClient()
const confirm = useConfirm()
const toast = useToast()

const selectedEventId = ref<string | number | undefined>(undefined)
const selectedSeriesId = ref<string | number | undefined>(undefined)


const {
  data: eventPages,
  fetchNextPage: fetchMoreEvents,
  hasNextPage: hasMoreEvents,
  isFetchingNextPage: loadingMoreEvents  
} = useGetEventSearchInfinite(
    computed(() => ({query: eventQuery.value, pageSize: SERIES_PAGE_SIZE})),
    {
      query: { 
        initialPageParam: 1,
        getNextPageParam: (lastPage, allPages) => {
          const total = Number(lastPage.total)
          return allPages.length * EVENT_PAGE_SIZE < total ? allPages.length + 1 : undefined
        },
        placeholderData: keepPreviousData
      }
    }
)

const events = computed(() => eventPages.value?.pages.flatMap(p => p.items) ?? [])

const {
  data: seriesPages,
  fetchNextPage: fetchMoreSeries,
  hasNextPage: hasMoreSeries,
  isFetchingNextPage: loadingMoreSeries,
} = useGetAdminSeriesSearchInfinite(
  computed(() => ({ query: seriesQuery.value, pageSize: SERIES_PAGE_SIZE })),
  {
    query: {
      initialPageParam: 1,
      getNextPageParam: (lastPage, allPages) => {
        const total = Number(lastPage.total)
        return allPages.length * SERIES_PAGE_SIZE < total ? allPages.length + 1 : undefined
      },
      placeholderData: keepPreviousData,
    },
  },
);

const series = computed(() => seriesPages.value?.pages.flatMap(p => p.items) ?? []);


const {mutate: reassignEventToSeries} = usePatchAdminReassignSeries()


function onSelectEvent(id: number | string) {
  selectedEventId.value = id
}

function onSelectSeries(id: number | string) {
  selectedSeriesId.value = id
}

function reassignSeries() {
  if (!selectedEventId.value || !selectedSeriesId.value) return;

  const targetEvent = events.value.find((e) => e?.id == selectedEventId.value)
  const newSeries = series.value.find((s) => s?.id == selectedSeriesId.value)
  const currentSeries = series.value.find((s) => 
    s?.events?.some((e) => e.id == selectedEventId.value))

  const eventName = targetEvent?.name ?? `Event #${selectedEventId.value}`
  const newSeriesName = newSeries?.displayName ?? `Series #${selectedSeriesId.value}`
  const currentSeriesName = currentSeries?.displayName ?? `No series`

  confirm.require({
    header: 'Confirm Reassignment',
    message: `Reassigning "${eventName}" from "${currentSeriesName}" to "${newSeriesName}". Are you sure?`,
    rejectProps: {
      label: 'Cancel',
      severity: 'secondary',
      outlined: true
    },
    acceptProps: {
      label: 'Reassign',
      severity: 'primary'
    },
    accept: () => {
      reassignEventToSeries(
        {
          params: 
          {
            eventId: targetEvent?.id,
            seriesId: newSeries?.id
          }
        }, 
        {
          onSuccess: () => {
            toast.add({severity: 'success', summary: 'Event reassigned', life: 3000})
            queryClient.invalidateQueries({
              queryKey: getGetAdminSeriesSearchInfiniteQueryKey()
            })

            selectedEventId.value = undefined
            selectedSeriesId.value = undefined
          },
          onError: () => {
            toast.add({severity: 'error', summary: 'Reassignment failed', life: 3000})
          }
        }
      )
    }
  })
}

</script>

<template>
  <div class="relative flex flex-col items-center w-full -m-3">
    <Splitter class="flex h-full shrink-0 w-full max-h-[calc(100vh-5rem)]">
      <SplitterPanel class="overflow-hidden flex flex-col min-h-0 p-2" :min-size="20">
        <DebouncedInput 
          placeholder="Search event..."
          v-model="eventQuery"
        />
        <div class="overflow-y-scroll h-full p-2">
          <ul class="flex flex-col gap-1">
            <li
              v-for="e in events" :key="e?.id"  
              @click="onSelectEvent(Number(e?.id))"
              class="flex items-center p-2 rounded-md cursor-pointer transition-colors "
              :class="[selectedEventId == e?.id ? 'bg-primary text-primary-contrast font-medium' : 'hover:bg-highlight']"
            >
              <span>{{ e?.name }}</span>
            </li>
          </ul>
          <div class="flex flex-col items-center mt-4">
            <Button v-if="hasMoreEvents" label="Show more" :loading="loadingMoreEvents" @click="fetchMoreEvents()" variant="outlined"/>
          </div>
        </div>
        
      </SplitterPanel>
      <SplitterPanel class="overflow-hidden flex flex-col p-2">
        <DebouncedInput 
        placeholder="Search series..."
        v-model="seriesQuery"
        />
        <div class="overflow-y-scroll h-full min-h-0">

          <Accordion multiple values="[]">
            <AccordionPanel v-for="s in series" :value="s?.id" :key="s?.id">
              <AccordionHeader :class="[selectedSeriesId == s?.id ? 'bg-primary text-primary-contrast font-medium' : 'hover:bg-highlight']"">
                {{ s?.displayName }} ({{ s?.events?.length }})
              </AccordionHeader>
              <AccordionContent class="p-2">
                <div class="cursor-pointer flex flex-col gap-1 pl-2 border-l-2 border-surface">
                  <div
                    v-for="e in s?.events"
                    @click="onSelectSeries(Number(s?.id))"
                    class="group flex items-center justify-between p-2 rounded-md cursor-pointer transition-all duration-150 text-sm"
                    :class="[selectedSeriesId === s?.id 
                      ? 'bg-highlight border-primary text-primary font-medium shadow-sm' 
                      : 'border-transparent hover:bg-highlight'
                    ]"
                  >
                    <span>
                      {{ e.name }}
                    </span>
                  </div>
                </div>
              </AccordionContent>
            </AccordionPanel>
          </Accordion>
          <div class="flex flex-col items-center mt-4">
            <Button v-if="hasMoreSeries" label="Show more" :loading="loadingMoreSeries" @click="fetchMoreSeries()" variant="outlined"/>
          </div>
        </div>
      </SplitterPanel>
    </Splitter>
    <Button @click="reassignSeries" label="Reassign" :disabled="!selectedEventId && !selectedSeriesId"/>
  </div>

</template>