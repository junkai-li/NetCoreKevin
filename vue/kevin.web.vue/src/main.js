import { createApp } from 'vue';
import App from './App.vue';
// 引入Antd组件库
import Antd from 'ant-design-vue';
// 引入路由
import router from './router';
// 引入Antd组件库CSS文件
import 'ant-design-vue/dist/reset.css'; 
// 使用 resize-observer-polyfill 替换原生 ResizeObserver
import ResizeObserver from 'resize-observer-polyfill';
window.ResizeObserver = ResizeObserver;

const app = createApp(App);
// Vue使用Antd组件库
app.use(Antd);
app.use(router);
app.config.productionTip = false;

app.mount('#app');