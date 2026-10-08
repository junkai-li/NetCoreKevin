<template>
  <a-config-provider :theme="antdTheme">
    <router-view />
  </a-config-provider>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { theme } from 'ant-design-vue';

// 默认品牌色与 design-tokens.css 的 --fn-color-brand 一致（Elegant Rose）
const DEFAULT_BRAND = '#e85d75';
const colorPrimary = ref(DEFAULT_BRAND);

// 必须是 computed：切换主题时 colorPrimary 变化要让 ConfigProvider 重新生成
// antd 组件样式（按钮 / 下拉 / 选中态 / 聚焦环等，含 teleport 到 body 的弹层）。
const antdTheme = computed(() => ({
  algorithm: theme.defaultAlgorithm,
  token: {
    colorPrimary: colorPrimary.value,
    borderRadius: 6,
    fontFamily:
      "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', 'PingFang SC', 'Microsoft YaHei', sans-serif",
    colorBgLayout: '#f0f2f5',
  },
}));

// 读取当前生效的品牌色：主题类挂在 .layout-container（首页）或 .login-container（登录页）上，
// 该元素的 --fn-color-brand 即当前主题色；两处都不存在时回退默认色。
const THEME_HOST = '.layout-container, .login-container';
const readBrand = () => {
  const host = document.querySelector(THEME_HOST) || document.documentElement;
  const brand = getComputedStyle(host).getPropertyValue('--fn-color-brand').trim();
  return /^#[0-9a-fA-F]{3,8}$/.test(brand) ? brand : DEFAULT_BRAND;
};

const syncBrand = () => {
  const brand = readBrand();
  if (brand === colorPrimary.value) return;
  colorPrimary.value = brand;
  // 同步到 :root，让自定义 CSS 里 var(--accent) 及品牌派生值在 teleport 到 body
  // 的弹层（模态框、下拉菜单、选择框面板）中也解析成当前主题色。
  document.documentElement.style.setProperty('--fn-color-brand', brand);
};

let observer = null;
onMounted(() => {
  syncBrand();
  setTimeout(syncBrand, 100);
  setTimeout(syncBrand, 500);
  // switchTheme 只改主题宿元素的 class（同标签页不触发 storage 事件），
  // 用 MutationObserver 监听主题类变化后重读品牌色。
  observer = new MutationObserver((mutations) => {
    for (const m of mutations) {
      if (
        m.type === 'attributes' &&
        m.target instanceof Element &&
        m.target.matches(THEME_HOST)
      ) {
        syncBrand();
        break;
      }
    }
  });
  observer.observe(document.documentElement, {
    subtree: true,
    attributes: true,
    attributeFilter: ['class'],
  });
});
onBeforeUnmount(() => {
  if (observer) observer.disconnect();
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
