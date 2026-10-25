import { fileURLToPath, URL } from 'node:url'
import { existsSync, readFileSync } from 'node:fs'

import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import Components from 'unplugin-vue-components/vite'
import { PrimeVueResolver } from '@openvue/auto-import-resolver'

import tailwindcss from '@tailwindcss/vite';

function loadEnvExample(): Record<string, string> {
  const envExamplePath = '.env.example'
  if (!existsSync(envExamplePath)) return {}

  const raw = readFileSync(envExamplePath, 'utf8')
  return raw
    .split('\n')
    .map((line) => line.trim())
    .filter((line) => line && !line.startsWith('#') && line.includes('='))
    .reduce<Record<string, string>>((acc, line) => {
      const [key, ...valueParts] = line.split('=')
      const value = valueParts.join('=').trim().replace(/^"|"$/g, '').replace(/^'|'$/g, '')
      acc[key.trim()] = value
      return acc
    }, {})
}

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const envExample = loadEnvExample()
  const proxyTarget = env.VITE_PROXY_TARGET || envExample.VITE_PROXY_TARGET || 'http://localhost:5190'

  return {
  plugins: [
    vue(),
    vueDevTools(),
    tailwindcss(),
    Components({
      resolvers:[
        PrimeVueResolver()
      ]
    })
    
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    allowedHosts: ['debpc', 'debpc:5173'],
    proxy: {
      '/api': {
        target: proxyTarget,
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
}})
