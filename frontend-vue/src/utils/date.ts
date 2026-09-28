const dateTimeFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'medium',
  timeStyle: 'short',
})

const dateFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'medium',
})

export const monthDayFormatter = new Intl.DateTimeFormat(undefined, {
  month: 'short',
  day: '2-digit'
})

const timeFormatter = new Intl.DateTimeFormat(undefined, {
  timeStyle: 'short'
})

export function formatEventDate(value: string | Date | null | undefined, withTime = true): string {
  if (!value) return '—'
  const date = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(date.getTime())) return '—'
  return (withTime ? dateTimeFormatter : dateFormatter).format(date)
}

export function formatTime(value: string | Date | null | undefined): string {
  if (!value) return ''
  const date = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(date.getTime())) return ''
  return (timeFormatter.format(date))
}


export function isSameDay(a: Date | string, b: Date | string): boolean {
  const dateA = a instanceof Date ? a : new Date(a)
  const dateB = b instanceof Date ? b : new Date(b)

  return (
    dateA.getFullYear() == dateB.getFullYear()
    && dateA.getMonth() == dateB.getMonth()
    && dateA.getDate() == dateB.getDate()
  )
}

export function isTodayInRange(start: Date, end: Date): boolean {
  const now = new Date()
  const todayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate())
  return todayStart >= start && todayStart < end
}

function startOfDay(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate())
}


export function isInRange(
  day: Date | string,
  start: Date | string,
  end?: Date | string | null,
): boolean {
  const dayDate = startOfDay(day instanceof Date ? day : new Date(day))
  const startDate = startOfDay(start instanceof Date ? start : new Date(start))

  if (!end) {
    return dayDate.getTime() === startDate.getTime()
  }

  const endDate = startOfDay(end instanceof Date ? end : new Date(end))

  return dayDate >= startDate && dayDate <= endDate
}

export function addDays(date: Date, days: number) {
  var result = new Date(date);
  result.setDate(result.getDate() + 7)
  return result
}