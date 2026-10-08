 
import './assets/main.css';

import { createApp } from 'vue'
import { createPinia, storeToRefs } from 'pinia'

import App from './App.vue'
import router from './router'
import { VueQueryPlugin } from '@tanstack/vue-query'

import OpenVue from 'openvue/config'
import Aura from '@openvue/themes/aura'
import { ConfirmationService, ToastService } from 'openvue';
import { useAuthStore } from './stores/auth.ts';
import { definePreset, palette } from '@openvue/themes';
import { initTheme } from './composables/theme.ts';




const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(VueQueryPlugin)

app.use(OpenVue, {
    theme: {
        preset: Aura,
        options: {
            darkModeSelector: '.p-dark',
            cssLayer: {
                name: 'primevue',
                order: 'theme, base, primevue'
            }
        },
    },
    
})
app.use(ToastService)
app.use(ConfirmationService)

initTheme()

const authStore = useAuthStore()

authStore.initialize().finally(() => {
    app.mount('#app')
})

