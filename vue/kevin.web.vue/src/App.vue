<template> 
  <kevinLogin />
</template>

<script>
import kevinLogin from './pages/kevinLogin.vue'

export default {
  name: 'App',
  components: {
    kevinLogin
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50; 
}
</style>
