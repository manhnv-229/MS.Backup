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
    server: { port: 8166 },
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

// https://vitejs.dev/config/
// export default defineConfig({
//   plugins: [vue()],
//   server: {
//     port: 8166,
//     proxy: "http://localhost:8166",
//     // proxy: {
//     //   // string shorthand: http://localhost:5173/foo -> http://localhost:4567/foo
//     //   // '/foo': 'http://localhost:8166',
//     //   // // with options: http://localhost:5173/api/bar-> http://jsonplaceholder.typicode.com/bar
//     //   // '/api': {
//     //   //   target: 'http://jsonplaceholder.typicode.com',
//     //   //   changeOrigin: true,
//     //   //   rewrite: (path) => path.replace(/^\/api/, ''),
//     //   // },
//     //   // // with RegEx: http://localhost:5173/fallback/ -> http://jsonplaceholder.typicode.com/
//     //   // '^/fallback/.*': {
//     //   //   target: 'http://jsonplaceholder.typicode.com',
//     //   //   changeOrigin: true,
//     //   //   rewrite: (path) => path.replace(/^\/fallback/, ''),
//     //   // },
//     //   // // Using the proxy instance
//     //   // '/api': {
//     //   //   target: 'http://jsonplaceholder.typicode.com',
//     //   //   changeOrigin: true,
//     //   //   configure: (proxy, options) => {
//     //   //     // proxy will be an instance of 'http-proxy'
//     //   //   },
//     //   // },
//     //   // // Proxying websockets or socket.io: ws://localhost:5173/socket.io -> ws://localhost:5174/socket.io
//     //   // '/socket.io': {
//     //   //   target: 'ws://localhost:5174',
//     //   //   ws: true,
//     //   // },
//     // },
//   },
// })
