import { useDark } from "@vueuse/core";

export const isDark = useDark({
  selector: 'html',
  attribute: 'class',
  valueDark: 'p-dark',
  valueLight: ''
})

export const toggleDarkMode = (event?: MouseEvent) => {
  if (!document.startViewTransition) {
    isDark.value = !isDark.value
    return;
  }

  document.startViewTransition(() => {
    isDark.value = !isDark.value
  })
}