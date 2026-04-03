const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/VarApi': {
        target: 'http://192.168.110.84:9901',
        changeOrigin: true,
        pathRewrite: {
          '^/VarApi': ''
        }
      }
    },
 client: {
      overlay: false
    },  
}
})

