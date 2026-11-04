<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import FullCalendar from '@fullcalendar/vue3'
import dayGridPlugin from '@fullcalendar/daygrid'
import interactionPlugin from '@fullcalendar/interaction'
import type { CalendarOptions, DatesSetArg, EventClickArg, DayCellMountArg } from '@fullcalendar/core'
import type { DateClickArg } from '@fullcalendar/interaction'
import type { EventListItemDto } from '@/api/generated/models'
import CalendarHeader from './CalendarHeader.vue'
import { isTodayInRange } from '@/utils/date.ts'
import EventPopover from './EventPopover.vue'

const props = defineProps<{
  events: EventListItemDto[],
  selectedDate?: Date
}>()

const emit = defineEmits<{
  rangeChange: [start: Date, end: Date],
  dateSelect: [date: Date]
}>()

const title = ref('')
const currentView = ref('dayGridMonth')
const today = ref(true)

const popoverRef = ref<InstanceType<typeof EventPopover> | null>(null)
const selectedEvent = ref<EventListItemDto | null>(null)

const toCalendarEvent = (event: EventListItemDto) => {
  return {
    id: String(event.id),
    title: event.name,
    start: event.startTime,
    end: event.endTime ?? undefined,
    extendedProps: {raw: event }
  }
}


const events = computed(() => (props.events ?? []).map((e) => toCalendarEvent(e)))


function handleEventClick(clickInfo: EventClickArg) {
  selectedEvent.value = clickInfo.event.extendedProps.raw as EventListItemDto
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
  dayCellDidMount: handleDayCellMount,
  headerToolbar: false,
  views: {
    dayGridMonth: {
      displayEventTime: false
    }
  }
})

function handleDayCellMount(arg: DayCellMountArg) {
  if (!props.selectedDate) return
  const cellDate = arg.date
  const isSelected =
    cellDate.getFullYear() === props.selectedDate.getFullYear() &&
    cellDate.getMonth() === props.selectedDate.getMonth() &&
    cellDate.getDate() === props.selectedDate.getDate()

  if (isSelected) {
    arg.el.classList.add('calendar-day-selected')
  }
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
      ref="popoverRef"
      :event="selectedEvent"
    />
  </div>
</template>

<style lang="css" scoped>
.calendar-day-selected {
  background-color: rgba(99, 102, 241, 0.15);
  border-radius: 0.5rem;
  position: relative;
}

.calendar-day-selected::after {
  content: '';
  position: absolute;
  inset: 0;
  border: 2px solid #6366f1;
  border-radius: 0.5rem;
  pointer-events: none;
}
</style>