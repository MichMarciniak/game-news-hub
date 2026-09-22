import { presets, type PresetName } from "@/features/themes/theme";
import { usePreset } from "@openvue/themes";
import { useDark, useStorage } from "@vueuse/core";

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

// zapisany wybór presetu w localStorage, domyślnie 'blue'
export const activePreset = useStorage<PresetName>('theme-preset', 'green');

export const applyPreset = (name: PresetName) => {
  if (!document.startViewTransition) {
    activePreset.value = name
    usePreset(presets[name].preset)
    return
  }

  document.startViewTransition(() => {
    activePreset.value = name;
    usePreset(presets[name].preset);
  })

};

// wywołaj raz przy starcie appki, żeby przywrócić zapisany preset
export const initTheme = () => {
  usePreset(presets[activePreset.value].preset);
};