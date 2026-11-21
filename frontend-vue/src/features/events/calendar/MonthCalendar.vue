<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import FullCalendar from '@fullcalendar/vue3'
import dayGridPlugin from '@fullcalendar/daygrid'
import interactionPlugin from '@fullcalendar/interaction'
import type { CalendarOptions, DatesSetArg, EventClickArg, DayCellContentArg } from '@fullcalendar/core'
import type { DateClickArg } from '@fullcalendar/interaction'
import type { EventListItemDto, RecommendationDto } from '@/api/generated/models'
import CalendarHeader from './CalendarHeader.vue'
import { isTodayInRange } from '@/utils/date.ts'
import EventPopover from './EventPopover.vue'
import RecommendationPopover from './RecommendationPopover.vue'

const props = defineProps<{
  events: (EventListItemDto | RecommendationDto)[],
  selectedDate?: Date,
  isRecommendations?: boolean
}>()

const emit = defineEmits<{
  rangeChange: [start: Date, end: Date],
  dateSelect: [date: Date]
}>()

const title = ref('')
const currentView = ref('dayGridMonth')
const today = ref(true)

const popoverRef = ref<InstanceType<typeof EventPopover> | InstanceType<typeof RecommendationPopover> | null>(null)
const selectedEvent = ref<EventListItemDto | RecommendationDto | null>(null)

const toCalendarEvent = (event: EventListItemDto | RecommendationDto) => {
  return {
    id: String(event.id),
    title: event.name,
    start: event.startTime,
    end: event.endTime ?? undefined,
    extendedProps: { raw: event }
  }
}


const events = computed(() => (props.events ?? []).map((e) => toCalendarEvent(e)))


function handleEventClick(clickInfo: EventClickArg) {
  selectedEvent.value = clickInfo.event.extendedProps.raw as EventListItemDto | RecommendationDto
  popoverRef.value?.open(clickInfo.jsEvent, clickInfo.el, clickInfo.event.id)
}

function handleDatesSet(arg: DatesSetArg) {
  title.value = arg.view.title
  currentView.value = arg.view.type
  today.value = isTodayInRange(arg.view.currentStart, arg.view.currentEnd)
  emit('rangeChange', arg.view.activeStart, arg.view.activeEnd)
}

function handleDateClick(arg: DateClickArg) {
  emit('dateSelect', arg.date)
  popoverRef.value?.hide()
  
  // Manually highlight the selected date cell
  document.querySelectorAll('.fc-daygrid-day').forEach(el => {
    el.classList.remove('calendar-day-selected')
  })
  arg.dayEl.classList.add('calendar-day-selected')
}

const calendarOptions = ref<CalendarOptions>({
  plugins: [dayGridPlugin, interactionPlugin],
  initialView: 'dayGridMonth',
  height: 'auto',
  stickyHeaderDates: false,
  events: events.value,
  eventClick: handleEventClick,
  datesSet: handleDatesSet,
  dateClick: handleDateClick,
  dayCellClassNames: handleDayCellClassNames,
  headerToolbar: false,
  views: {
    dayGridMonth: {
      displayEventTime: false
    }
  }
})

function handleDayCellClassNames(arg: DayCellContentArg): string[] {
  if (!props.selectedDate) return []
  const cellDate = arg.date
  const isSelected =
    cellDate.getFullYear() === props.selectedDate.getFullYear() &&
    cellDate.getMonth() === props.selectedDate.getMonth() &&
    cellDate.getDate() === props.selectedDate.getDate()

  return isSelected ? ['calendar-day-selected'] : []
}

watch(
  () => props.selectedDate,
  () => {
    calendarApi.value?.render()
  }
)

watch(events, (newEvents) => {
  calendarOptions.value.events = newEvents
})

const calendarRef = ref<InstanceType<typeof FullCalendar> | null>(null)
const calendarApi = computed(() => calendarRef.value?.getApi())

const viewOptions = [
  { label: 'Month', value: 'dayGridMonth' },
  { label: 'Week', value: 'dayGridWeek' },
]

function goPrev() {
  calendarApi.value?.prev()
}

function goNext() {
  calendarApi.value?.next()
}

function goToday() {
  calendarApi.value?.today()
}

function changeView(view: string) {
  calendarApi.value?.changeView(view)
  currentView.value = view
}

</script>

<template>
  <div >
    <CalendarHeader 
      :today="today"
      :title="title"
      :current-view="currentView"
      :view-options="viewOptions"
      @prev="goPrev"
      @next="goNext"
      @today="goToday"
      @update:current-view="changeView"
    />
    <FullCalendar ref="calendarRef" :options="calendarOptions"/>

    <EventPopover 
      v-if="!props.isRecommendations"
      ref="popoverRef"
      :event="selectedEvent"
    />
    <RecommendationPopover 
      v-else
      ref="popoverRef"
      :event="selectedEvent"
    />
  </div>
</template>

<style lang="css">
.calendar-day-selected {
  position: relative;
}

.calendar-day-selected::after {
  content: '';
  position: absolute;
  inset: 0;
  border: 2px solid;
  border-radius: 0;
  pointer-events: none;
}
</style>