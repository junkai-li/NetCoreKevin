import { createApp } from 'vue';
import App from './App.vue';
import Antd from 'ant-design-vue';
import router from './router';
import 'ant-design-vue/dist/reset.css';
import './css/enterprise-surface.css';
import './css/pagination-enterprise.css';
import ResizeObserver from 'resize-observer-polyfill';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn';

dayjs.locale('zh-cn');
window.ResizeObserver = ResizeObserver;

const app = createApp(App);
app.use(Antd);
app.use(router);
app.config.productionTip = false;

app.mount('#app');