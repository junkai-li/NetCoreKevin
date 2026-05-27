const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/VarApi': {
            target: 'http://localhost:32769',
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

