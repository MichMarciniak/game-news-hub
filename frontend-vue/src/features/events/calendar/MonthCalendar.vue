<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import FullCalendar from '@fullcalendar/vue3'
import dayGridPlugin from '@fullcalendar/daygrid'
import interactionPlugin from '@fullcalendar/interaction'
import type { CalendarOptions, DatesSetArg, EventClickArg } from '@fullcalendar/core'
import type { DateClickArg } from '@fullcalendar/interaction'
import type { EventListItemDto } from '@/api/generated/models'

const props = defineProps<{
  events: EventListItemDto[],
}>()

const emit = defineEmits<{
  rangeChange: [start: Date, end: Date],
  dateSelect: [date: Date]
}>()

const toCalendarEvent = (event: EventListItemDto) => {
  return {
    id: String(event.id),
    title: event.name,
    start: event.startTime,
    end: event.endTime ?? undefined,
  }
}


const events = computed(() => (props.events ?? []).map((e) => toCalendarEvent(e)))


function handleEventClick(clickInfo: EventClickArg) {
  console.log(clickInfo)
}

function handleDatesSet(arg: DatesSetArg) {
  emit('rangeChange', arg.view.activeStart, arg.view.activeEnd)
}

function handleDateClick(arg: DateClickArg) {
  emit('dateSelect', arg.date)
}

const calendarOptions = ref<CalendarOptions>({
  plugins: [dayGridPlugin, interactionPlugin],
  initialView: 'dayGridMonth',
  events: events.value,
  eventClick: handleEventClick,
  datesSet: handleDatesSet,
  dateClick: handleDateClick,
  headerToolbar: {
    left: 'prev,next today',
    center: 'title',
    right: 'dayGridWeek,dayGridMonth'
  },
})

watch(events, (newEvents) => {
  calendarOptions.value.events = newEvents
})

</script>

<template>
  <FullCalendar :options="calendarOptions"/>
</template>

<style lang="css" scoped>

</style>