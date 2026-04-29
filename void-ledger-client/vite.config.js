import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  // Local dev proxy avoids browser CORS issues when the React app calls the deployed API.
  server: {
    proxy: {
      '/api': {
        target: 'https://voidledger-api-rifh.azurewebsites.net',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
})
