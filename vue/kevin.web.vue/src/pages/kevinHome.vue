<template>
  <a-layout class="layout-container" :class="currentTheme">
    <!-- 动态背景 -->
    <div class="background">
      <div class="particles"></div>
      <div class="grid-lines"></div>
      <div class="floating-elements">
        <div class="element element-1"></div>
        <div class="element element-2"></div>
        <div class="element element-3"></div>
        <div class="element element-4"></div>
      </div>
    </div>

    <!-- 左侧导航栏 -->
    <a-layout-sider
      v-model:collapsed="collapsed"
      :trigger="null"
      collapsible
      class="sider"
      width="256"
      :style="{ position: 'fixed', height: '100vh', left: 0, top: 0, bottom: 0 }"
    >
      <div class="logo">
        <div class="logo-wrapper">
          <img :src="logoImage" alt="Logo" class="logo-image" />
        </div>
        <span v-if="!collapsed" class="logo-text">NetCoreKevin后台管理系统</span>
      </div>

      <a-menu
        v-model:selectedKeys="selectedKeys"
        v-model:openKeys="openKeys"
        theme="dark"
        mode="inline"
        @click="handleMenuClick"
      >
        <a-sub-menu key="dashboard">
          <template #icon>
            <DashboardOutlined />
          </template>
          <template #title>首页</template>
          <a-menu-item key="">首页</a-menu-item>
        </a-sub-menu>
        <a-sub-menu key="my">
          <template #icon>
            <RobotOutlined />
          </template>
          <template #title>我的</template>
          <a-menu-item key="my-message">我的消息</a-menu-item>
        </a-sub-menu>
        <a-sub-menu key="aimanagement">
          <template #icon>
            <GitlabOutlined />
          </template>
          <template #title>AI管理</template>
          <a-menu-item key="ai-appsmg">智能体管理</a-menu-item>
          <a-menu-item key="ai-promptsmg">提示词管理</a-menu-item>
          <a-menu-item key="ai-kmssmg">知识库管理</a-menu-item>
          <a-menu-item key="ai-modelmg">模型配置管理</a-menu-item>
        </a-sub-menu>
        <a-sub-menu key="organizational-management">
          <template #icon>
            <UserOutlined />
          </template>
          <template #title>组织架构</template>
          <a-menu-item key="organizational-position">岗位管理</a-menu-item>
          <a-menu-item key="organizational-department">部门管理</a-menu-item>
        </a-sub-menu>
        <a-sub-menu key="user-management">
          <template #icon>
            <UserOutlined />
          </template>
          <template #title>用户管理</template>
          <a-menu-item key="user-list">用户管理</a-menu-item>
          <a-menu-item key="user-role">角色管理</a-menu-item>
          <a-menu-item key="user-permission">权限管理</a-menu-item>
        </a-sub-menu>

        <a-sub-menu key="system-settings">
          <template #icon>
            <SettingOutlined />
          </template>
          <template #title>系统管理</template>
          <a-menu-item key="system-announcement">系统公告</a-menu-item>
          <a-menu-item key="system-dic">系统配置</a-menu-item>
          <a-menu-item key="log-management">日志管理</a-menu-item>
          <a-menu-item key="oslog">关键数据变动日志</a-menu-item>
        </a-sub-menu>
      </a-menu>
    </a-layout-sider>

    <!-- 右侧内容区域 -->
    <a-layout
      class="main-layout"
      :style="{
        marginLeft: collapsed ? '80px' : '256px',
        transition: 'margin-left 0.3s ease',
      }"
    >
      <!-- 头部 -->
      <a-layout-header class="header">
        <div class="header-left">
          <menu-unfold-outlined
            v-if="collapsed"
            class="trigger"
            @click="() => (collapsed = !collapsed)"
          />
          <menu-fold-outlined
            v-else
            class="trigger"
            @click="() => (collapsed = !collapsed)"
          />

          <a-breadcrumb class="breadcrumb">
            <a-breadcrumb-item>首页</a-breadcrumb-item>
            <a-breadcrumb-item style="color: aliceblue">{{
              currentRouteTitle
            }}</a-breadcrumb-item>
          </a-breadcrumb>
        </div>

        <div class="header-right">
          <!-- 主题切换下拉菜单 -->
          <a-dropdown>
            <a-Button
              type="text"
              style="color: rgba(255, 255, 255, 0.85)"
              class="theme-switch-button"
            >
              <BgColorsOutlined />
              主题
            </a-Button>
            <template #overlay>
              <a-menu class="theme-menu">
                <a-menu-item key="blackblue" @click="switchTheme('blackblue')">
                  <div class="theme-option">
                    <div class="theme-preview theme-blackblue"></div>
                    <span>黑蓝主题</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="default" @click="switchTheme('default')">
                  <div class="theme-option">
                    <div class="theme-preview theme-default"></div>
                    <span>默认主题</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="green" @click="switchTheme('green')">
                  <div class="theme-option">
                    <div class="theme-preview theme-green"></div>
                    <span>绿色主题</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="purple" @click="switchTheme('purple')">
                  <div class="theme-option">
                    <div class="theme-preview theme-purple"></div>
                    <span>紫色主题</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="darkblue" @click="switchTheme('darkblue')">
                  <div class="theme-option">
                    <div class="theme-preview theme-darkblue"></div>
                    <span>深蓝色主题</span>
                  </div>
                </a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>

          <a-dropdown>
            <a-badge dot>
              <BellOutlined class="header-icon" />
            </a-badge>
            <template #overlay>
              <a-menu class="notification-menu">
                <a-menu-item>您有3条未读消息</a-menu-item>
                <a-menu-item>系统维护通知</a-menu-item>
                <a-menu-item>新版本更新提醒</a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>

          <a-dropdown>
            <div class="user-info">
              <a-avatar :src="userInfo.avatar" />
              <span class="user-name">{{ userInfo.name }}</span>
            </div>
            <template #overlay>
              <a-menu class="user-menu">
                <a-menu-item @click="handleUserInfo" key="profile">
                  <UserOutlined />
                  个人中心
                </a-menu-item>
                <a-menu-item key="settings">
                  <SettingOutlined />
                  设置
                </a-menu-item>
                <a-menu-divider />
                <a-menu-item key="logout" @click="handleLogout">
                  <LogoutOutlined />
                  退出登录
                </a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>
        </div>
      </a-layout-header>

      <!-- 主要内容 -->
      <a-layout-content class="content">
        <div class="content-wrapper">
          <!-- 将静态内容替换为动态路由加载区域 -->
          <router-view />
        </div>
      </a-layout-content>

      <!-- 底部 -->
      <a-layout-footer class="footer">
        <div class="footer-content">
          <span>© 2025 NetCoreKevin后台管理系统. All Rights Reserved.</span>
        </div>
      </a-layout-footer>
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount } from "vue";
import {
  MenuUnfoldOutlined,
  MenuFoldOutlined,
  DashboardOutlined,
  UserOutlined,
  SettingOutlined,
  BellOutlined,
  LogoutOutlined,
  BgColorsOutlined,
  GitlabOutlined,
  RobotOutlined,
} from "@ant-design/icons-vue";
//import { Button } from 'ant-design-vue';
import { useRouter } from "vue-router";
import "../css/kevinHome.css";
import hedeImage from "../assets/hede.png"; // 导入用户头像图片
import logoImage from "../assets/logo.png"; // 导入logo图片

const router = useRouter();

const collapsed = ref(false);
const selectedKeys = ref(["dashboard"]);
const openKeys = ref(["user-management"]);

// 主题状态 - 将默认主题改为黑蓝主题
const currentTheme = ref("theme-blackblue");

// 用户信息
const userInfo = reactive({
  name: "管理员",
  avatar: hedeImage, // 使用导入的图片
});

// 当前路由标题
const currentRouteTitle = computed(() => {
  const titles = {
    dashboard: "首页",
    "user-list": "用户列表",
    "user-role": "角色管理",
    "user-permission": "权限管理",
    "system-dic": "系统配置",
    "system-announcement": "系统公告",
    "log-management": "日志管理",
    oslog: "关键数据变动日志",
    handleUserInfo: "通知中心",
    "ai-appsmg": "智能体管理",
    "ai-promptsmg": "提示词管理",
    "ai-kmssmg": "知识库管理",
    "ai-modelmg": "模型配置管理",
    "my-message": "我的消息",
    "organizational-position": "岗位管理",
    "organizational-department": "部门管理",
  };
  return titles[selectedKeys.value[0]] || "首页";
});

// 切换主题
const switchTheme = (theme) => {
  currentTheme.value = `theme-${theme}`;
  // 保存主题选择到本地存储
  localStorage.setItem("app-theme", theme);
};

// 菜单点击事件
const handleMenuClick = ({ key }) => {
  selectedKeys.value = [key];
  // 根据key跳转到对应路由
  switch (key) {
    case "user-list":
      router.push("/home/user/list");
      break;
    case "user-role":
      router.push("/home/user/role");
      break;
    case "user-permission":
      router.push("/home/user/permission");
      break;
    case "system-dic":
      router.push("/home/system/dic");
      break;
    case "system-announcement":
      router.push("/home/system/announcement");
      break;
    case "my-message":
      router.push("/home/my/message");
      break;
    case "log-management":
      router.push("/home/system/log");
      break;
    case "oslog":
      router.push("/home/system/oslog");
      break;
    case "aimanagement":
      router.push("/home/aimanagement");
      break;
    case "ai-appsmg":
      router.push("/home/aimanagement/aiappsmg");
      break;
    case "ai-promptsmg":
      router.push("/home/aimanagement/aipromptsmg");
      break;
    case "ai-kmssmg":
      router.push("/home/aimanagement/aikmssmg");
      break;
    case "ai-modelmg":
      router.push("/home/aimanagement/aimodelmg");
      break;
    case "organizational-position":
      router.push("/home/position/management");
      break;
    case "organizational-department":
      router.push("/home/department/management");
      break;
    default:
      router.push("/home");
  }
};

// 退出登录
const handleLogout = () => {
  console.log("用户退出登录");
  // 清除认证状态并跳转到登录页
  localStorage.removeItem("token");
  router.push("/login");
};
// 个人中心
const handleUserInfo = () => {
  console.log("个人中心");
  selectedKeys.value = ["handleUserInfo"];
  router.push("/home/user/profile");
};
// 初始化背景动画
onMounted(() => {
  initParticles();
  // 检查本地存储的主题设置
  const savedTheme = localStorage.getItem("app-theme");
  if (savedTheme) {
    currentTheme.value = `theme-${savedTheme}`;
  } else {
    // 如果没有保存的主题设置，默认使用黑蓝主题
    currentTheme.value = "theme-blackblue";
    localStorage.setItem("app-theme", "blackblue");
  }

  const response = JSON.parse(localStorage.getItem("user"));
  userInfo.name = response.name;
});

onBeforeUnmount(() => {
  // 清理动画
});

// 初始化粒子效果
const initParticles = () => {
  const particlesContainer = document.querySelector(".particles");
  if (!particlesContainer) return;

  // 清空现有的粒子
  particlesContainer.innerHTML = "";

  // 创建粒子
  for (let i = 0; i < 50; i++) {
    const particle = document.createElement("div");
    particle.className = "particle";
    particle.style.left = `${Math.random() * 100}%`;
    particle.style.top = `${Math.random() * 100}%`;
    particle.style.animationDelay = `${Math.random() * 5}s`;
    particle.style.width = `${Math.random() * 4 + 2}px`;
    particle.style.height = particle.style.width;
    particlesContainer.appendChild(particle);
  }
};
</script>

<style scoped>
.layout-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
  position: relative;
  overflow: hidden;
}

/* 背景层 */
.background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 0;
}

/* 粒子效果 */
.particles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  animation: float 6s infinite ease-in-out;
  box-shadow: 0 0 10px rgba(255, 255, 255, 0.8);
}

@keyframes float {
  0%,
  100% {
    transform: translate(0, 0);
    opacity: 0.6;
  }
  50% {
    transform: translate(20px, 20px);
    opacity: 1;
  }
}

/* 网格线 */
.grid-lines {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: linear-gradient(rgba(102, 126, 234, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(102, 126, 234, 0.1) 1px, transparent 1px);
  background-size: 30px 30px;
  animation: grid-move 20s linear infinite;
}

@keyframes grid-move {
  0% {
    transform: translate(0, 0);
  }
  100% {
    transform: translate(30px, 30px);
  }
}

/* 悬浮元素 */
.floating-elements {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.element {
  position: absolute;
  border: 1px solid rgba(102, 126, 234, 0.3);
  border-radius: 50%;
  animation: float-element 15s infinite linear;
}

.element-1 {
  width: 80px;
  height: 80px;
  top: 10%;
  left: 5%;
  animation-duration: 20s;
}

.element-2 {
  width: 120px;
  height: 120px;
  bottom: 15%;
  right: 10%;
  animation-duration: 25s;
  animation-direction: reverse;
}

.element-3 {
  width: 60px;
  height: 60px;
  top: 40%;
  right: 20%;
  animation-duration: 18s;
}

.element-4 {
  width: 100px;
  height: 100px;
  bottom: 20%;
  left: 15%;
  animation-duration: 22s;
  animation-direction: reverse;
}

@keyframes float-element {
  0% {
    transform: translate(0, 0) rotate(0deg);
  }
  25% {
    transform: translate(20px, 30px) rotate(90deg);
  }
  50% {
    transform: translate(0, 60px) rotate(180deg);
  }
  75% {
    transform: translate(-20px, 30px) rotate(270deg);
  }
  100% {
    transform: translate(0, 0) rotate(360deg);
  }
}

.main-layout {
  position: relative;
  z-index: 1;
  margin-left: 256px;
  transition: margin-left 0.3s ease;
  background: transparent !important;
}

.main-layout.collapsed {
  margin-left: 80px;
}

.sider {
  height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  box-shadow: 2px 0 8px 0 rgba(29, 35, 41, 0.05);
  z-index: 10;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-right: 1px solid rgba(255, 255, 255, 0.18);
  transition: width 0.3s ease;
}

.logo {
  height: 64px;
  display: flex;
  align-items: center;
  padding: 0 24px;
  background: rgba(255, 255, 255, 0.02);
  overflow: hidden;
}

.logo-wrapper {
  display: flex;
  justify-content: center;
  margin-right: 12px;
}

.logo-image {
  width: 32px;
  height: 32px;
  animation: pulse 3s infinite;
}

@keyframes pulse {
  0%,
  100% {
    transform: scale(1);
    filter: drop-shadow(0 0 5px rgba(102, 126, 234, 0.5));
  }
  50% {
    transform: scale(1.05);
    filter: drop-shadow(0 0 15px rgba(102, 126, 234, 0.8));
  }
}

.logo-text {
  color: white;
  font-size: 14px;
  font-weight: 600;
  white-space: nowrap;
  text-shadow: 0 0 10px rgba(102, 126, 234, 0.5);
}

:deep(.ant-menu-dark) {
  background: transparent;
}

:deep(.ant-menu-dark .ant-menu-inline) {
  background: transparent;
}

:deep(.ant-menu-dark .ant-menu-item:hover) {
  background: rgba(102, 126, 234, 0.2);
}

:deep(.ant-menu-dark .ant-menu-item-selected) {
  background: linear-gradient(90deg, rgba(102, 126, 234, 0.3), rgba(118, 75, 162, 0.3));
}

.header {
  background: rgba(255, 255, 255, 0.08) !important;
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.18);
  padding: 0 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  position: sticky;
  top: 0;
  z-index: 9;
  color: white;
}

.header-left {
  display: flex;
  align-items: center;
}

.trigger {
  font-size: 18px;
  line-height: 64px;
  padding: 0 24px;
  cursor: pointer;
  transition: color 0.3s;
  color: rgba(255, 255, 255, 0.85);
}

.trigger:hover {
  color: #667eea;
  text-shadow: 0 0 8px rgba(102, 126, 234, 0.6);
}

.breadcrumb {
  margin-left: 16px;
}

:deep(.ant-breadcrumb) {
  color: white;
}

:deep(.ant-breadcrumb a) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-breadcrumb span:last-child) {
  color: rgba(255, 255, 255, 0.7);
}

:deep(.ant-breadcrumb .ant-breadcrumb-link) {
  color: white;
}

:deep(.ant-breadcrumb .ant-breadcrumb-separator) {
  color: rgba(255, 255, 255, 0.5);
}

.header-right {
  display: flex;
  align-items: center;
  gap: 24px;
}

.header-icon {
  font-size: 18px;
  color: rgba(255, 255, 255, 0.85);
  cursor: pointer;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  gap: 8px;
}

.user-name {
  color: rgba(255, 255, 255, 0.85);
}

.content {
  margin: 24px 16px 0;
  overflow: initial;
  position: relative;
  z-index: 1;
  background: transparent !important;
}

.content-wrapper {
  padding: 24px;
  background: rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(31, 38, 135, 0.37);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.18);
  min-height: calc(100vh - 168px);
  position: relative;
  overflow: hidden;
  color: white;
}

.content-wrapper::before {
  content: "";
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, rgba(102, 126, 234, 0.1), transparent);
  transform: rotate(45deg);
  animation: shine 6s infinite;
  z-index: -1;
}

@keyframes shine {
  0% {
    transform: rotate(45deg) translateX(-100%);
  }
  100% {
    transform: rotate(45deg) translateX(100%);
  }
}

.welcome-card {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 15px;
  padding: 30px;
  margin-bottom: 30px;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.2);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  color: white;
}

.welcome-card h1 {
  color: white;
  font-size: 28px;
  margin-bottom: 10px;
  text-shadow: 0 0 10px rgba(102, 126, 234, 0.5);
}

.welcome-card p {
  color: rgba(255, 255, 255, 0.8);
  font-size: 16px;
  margin-bottom: 30px;
}

.welcome-stats {
  display: flex;
  justify-content: center;
  gap: 40px;
  margin-top: 20px;
}

.stat-item {
  text-align: center;
}

.stat-value {
  display: block;
  font-size: 28px;
  font-weight: bold;
  color: #667eea;
  text-shadow: 0 0 10px rgba(102, 126, 234, 0.5);
}

.stat-label {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.7);
}

.info-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin-top: 30px;
}

.info-card {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
}

:deep(.info-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.info-card .ant-card-head-title) {
  color: white;
}

:deep(.info-card .ant-card-body) {
  color: rgba(255, 255, 255, 0.8);
  background: transparent;
}

.footer {
  text-align: center;
  padding: 16px 24px;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-top: 1px solid rgba(255, 255, 255, 0.18);
  margin-top: 24px;
  position: relative;
  z-index: 1;
  color: rgba(255, 255, 255, 0.7);
}

.footer-content {
  color: rgba(255, 255, 255, 0.7);
  font-size: 14px;
}

/* 下拉菜单样式 */
:deep(.notification-menu),
:deep(.user-menu) {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: 10px;
  overflow: hidden;
}

:deep(.notification-menu .ant-menu-item),
:deep(.user-menu .ant-menu-item) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.notification-menu .ant-menu-item:hover),
:deep(.user-menu .ant-menu-item:hover) {
  background: rgba(102, 126, 234, 0.2);
  color: white;
}

/* 响应式设计 - 移动端适配 */
@media (max-width: 768px) {
  .sider {
    position: absolute;
  }

  .header {
    padding: 0 12px;
  }

  .content {
    margin: 12px 8px 0;
  }

  .content-wrapper {
    padding: 16px;
  }

  .welcome-stats {
    flex-direction: column;
    gap: 20px;
  }

  .info-cards {
    grid-template-columns: 1fr;
  }

  .main-layout {
    margin-left: 0;
  }
}
</style>
