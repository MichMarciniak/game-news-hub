/* eslint-disable vue/multi-word-component-names */
import './assets/main.css';

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { VueQueryPlugin } from '@tanstack/vue-query'

import OpenVue from 'openvue/config'
import Aura from '@openvue/themes/aura'
import Button from 'openvue/button'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(VueQueryPlugin)

app.use(OpenVue, {
    theme: {
        preset: Aura,
        options: {
            darkModeSelector: '.p-dark', // Toggles dark mode via class
        },
    },
    
})
app.component("Button", Button)
app.mount('#app')
