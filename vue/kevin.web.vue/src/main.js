import { createApp } from 'vue';
import App from './App.vue';
// 引入Antd组件库
import Antd from 'ant-design-vue';
// 引入路由
import router from './router';
// 引入Antd组件库CSS文件
import 'ant-design-vue/dist/reset.css'; 

const app = createApp(App);
// Vue使用Antd组件库
app.use(Antd);
app.use(router);
app.config.productionTip = false;

app.mount('#app');