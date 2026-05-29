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
</style>
