import { useGetRecommendationEvents } from '@/api/generated';
import { addDays } from '@/utils/date';
import { ref, computed, watch } from 'vue';
import EventFullDateItem from './EventFullDateItem.vue';
import type { RecommendationResponse } from '@/api/generated/models';
import type { EventListItemDto } from '@/api/generated/models';
import Select from 'openvue/select';

const limit = ref(5)

const query = computed(() => ({
  from: new Date().toISOString(),
  to: addDays(new Date(), 7).toISOString(),
  limit: limit.value
}))

const {data: responseData, isPending, refetch} = useGetRecommendationEvents(query)

const events = computed<EventListItemDto[]>(() => {
  const items = responseData.value?.events ?? []
  return items.map(e => ({
    id: e.id,
    name: e.name,
    followerCount: e.followerCount,
    startTime: e.startTime,
    endTime: e.endTime
  }))
})

const limitOptions = [
  { label: '2', value: 2 },
  { label: '5', value: 5 },
  { label: '10', value: 10 },
  { label: '20', value: 20 }
]

function onLimitChange(value: number) {
  limit.value = value
  refetch()
}