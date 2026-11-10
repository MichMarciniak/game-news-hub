import { ref, computed } from 'vue'

export function useCooldown(initialSeconds: number = 60) {
  const secondsLeft = ref(0)
  let intervalId: number | undefined

  const isDisabled = computed(() => secondsLeft.value > 0)

  const start = (seconds?: number) => {
    secondsLeft.value = seconds ?? initialSeconds
    if (intervalId) clearInterval(intervalId)
    intervalId = setInterval(() => {
      secondsLeft.value -= 1
      if (secondsLeft.value <= 0) {
        clearInterval(intervalId)
        intervalId = undefined
      }
    }, 1000)
  }

  const stop = () => {
    if (intervalId) clearInterval(intervalId)
    intervalId = undefined
    secondsLeft.value = 0
  }

  return { secondsLeft, isDisabled, start, stop }
}