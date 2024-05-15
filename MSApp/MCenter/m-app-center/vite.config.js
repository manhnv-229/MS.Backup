import vue from '@vitejs/plugin-vue'
import { defineConfig, loadEnv } from 'vite'
import { fileURLToPath, URL } from 'node:url'
import vitePluginRequire from 'vite-plugin-require'

export default defineConfig(({ command, mode }) => {
  // Load env file based on `mode` in the current working directory.
  // Set the third parameter to '' to load all env regardless of the `VITE_` prefix.
  const env = loadEnv(mode, process.cwd(), '')
  return {
    // vite config
    plugins: [vue(), vitePluginRequire.default()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    server: { port: 8866 },
    define: {
      __APP_ENV__: JSON.stringify(env.APP_ENV),
      VUE_APP_BASE_URL: JSON.stringify(env.VUE_APP_BASE_URL),
      VUE_APP_NOTICE_URL: JSON.stringify(env.VUE_APP_NOTICE_URL),
      VUE_APP_ServerFileUrl: JSON.stringify(env.VUE_APP_ServerFileUrl)
    },
    build: {
      commonjsOptions: { transformMixedEsModules: true } // Change
    }
  }
})
