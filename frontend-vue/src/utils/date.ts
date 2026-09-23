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