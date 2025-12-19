import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import mkcert from 'vite-plugin-mkcert'

// https://vite.dev/config/
export default defineConfig({
  build: {
    outDir: '../GoFla.Api/wwwroot',
  },
  server: {
    port: 3000,
  },
  plugins: [react(), tailwindcss(), mkcert()],
})
