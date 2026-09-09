<template>
  <a-config-provider :theme="antdTheme">
    <router-view />
  </a-config-provider>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { theme } from 'ant-design-vue';

const colorPrimary = ref('#1677ff');

const antdTheme = {
  algorithm: theme.defaultAlgorithm,
  token: {
    colorPrimary: colorPrimary.value,
    borderRadius: 6,
    fontFamily:
      "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', 'PingFang SC', 'Microsoft YaHei', sans-serif",
    colorBgLayout: '#f0f2f5',
  },
};

const updateColorPrimary = () => {
  const styles = getComputedStyle(document.documentElement);
  const accent = styles.getPropertyValue('--accent').trim();
  if (accent) {
    colorPrimary.value = accent;
  }
};

onMounted(() => {
  updateColorPrimary();
  setTimeout(updateColorPrimary, 100);
  setTimeout(updateColorPrimary, 500);
});

window.addEventListener('storage', () => {
  setTimeout(updateColorPrimary, 50);
});
</script>

<style>
#app {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial,
    'Noto Sans', 'PingFang SC', 'Microsoft YaHei', sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: rgba(0, 0, 0, 0.88);
  min-height: 100vh;
}
/* 全局：message 提示层置于最高层。
   它是3秒即消的瞬时反馈（包括接口报错），不该被任何东西盖住：
   业务里嵌套弹窗（如智能体编辑页的接口授权申请理由、接口JSON字段详情）为了压住外层弹窗用到了 z-index:1100，
   而 message 默认只有 1010，在弹窗里提交失败时那条报错会被弹窗连蒙层一起盖住，用户以为没有报错。
   抬只能整层抬：.ant-message 自身构成层叠上下文，给内部的 -error/-success 单设 z-index 抬不出父容器；
   !important 必需，ant-design-vue 4 用 CSS-in-JS 在运行时注入规则，顺序在打包样式之后 */
.ant-message {
  z-index: 9999 !important;
}
</style>
