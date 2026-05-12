<template>
  <a-layout class="layout-container" :class="currentTheme">
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
        <span v-if="!collapsed" class="logo-text">AI智能体后台管理系统</span>
      </div>

      <div class="menu-wrapper">
        <a-menu
          v-model:selectedKeys="selectedKeys"
          v-model:openKeys="openKeys"
          :theme="menuTheme"
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
            <a-menu-item key="my-ai-chat">我的AI对话</a-menu-item>
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
            <a-menu-item key="system-tenant">租户管理</a-menu-item>
            <a-menu-item key="system-code-generator">代码生成器</a-menu-item>
            <a-menu-item key="log-management">日志管理</a-menu-item>
            <a-menu-item key="oslog">关键数据变动日志</a-menu-item>
          </a-sub-menu>
        </a-menu>
      </div>
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
            <a-breadcrumb-item>{{ currentRouteTitle }}</a-breadcrumb-item>
          </a-breadcrumb>
        </div>

        <div class="header-right">
          <!-- 主题切换下拉菜单 -->
          <a-dropdown>
            <a-button type="text" class="theme-switch-button">
              <BgColorsOutlined />
              主题
            </a-button>
            <template #overlay>
              <a-menu class="theme-menu">
                <a-menu-item key="enterprise" @click="switchTheme('enterprise')">
                  <div class="theme-option">
                    <div class="theme-preview theme-enterprise"></div>
                    <span>企业蓝</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="blackblue" @click="switchTheme('blackblue')">
                  <div class="theme-option">
                    <div class="theme-preview theme-blackblue"></div>
                    <span>墨黑</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="default" @click="switchTheme('default')">
                  <div class="theme-option">
                    <div class="theme-preview theme-default"></div>
                    <span>灰蓝</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="green" @click="switchTheme('green')">
                  <div class="theme-option">
                    <div class="theme-preview theme-green"></div>
                    <span>绿色</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="purple" @click="switchTheme('purple')">
                  <div class="theme-option">
                    <div class="theme-preview theme-purple"></div>
                    <span>紫色</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="darkblue" @click="switchTheme('darkblue')">
                  <div class="theme-option">
                    <div class="theme-preview theme-darkblue"></div>
                    <span>深海蓝</span>
                  </div>
                </a-menu-item>
                <a-menu-item key="simple-white" @click="switchTheme('simple-white')">
                  <div class="theme-option">
                    <div class="theme-preview theme-simple-white"></div>
                    <span>企业简约白</span>
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
          <span>© 2026 AI智能体后台管理系统. All Rights Reserved.</span>
        </div>
      </a-layout-footer>
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from "vue";
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

const currentTheme = ref("theme-simple-white");

const menuTheme = computed(() => {
  return currentTheme.value === "theme-simple-white" ? "light" : "dark";
});

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
    handleUserInfo: "个人中心",
    "ai-appsmg": "智能体管理",
    "ai-promptsmg": "提示词管理",
    "ai-kmssmg": "知识库管理",
    "ai-modelmg": "模型配置管理",
    "my-message": "我的消息",
    "my-ai-chat": "我的AI对话",
    "organizational-position": "岗位管理",
    "organizational-department": "部门管理",
    "system-tenant": "租户管理",
    "system-code-generator": "代码生成器",
  };
  return titles[selectedKeys.value[0]] || "系统页面";
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
    case "my-ai-chat":
      router.push("/home/my/ai-chat");
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
      router.push("/home/aimanagement/aikmssmg"); // 添加知识库管理路由
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
      case "system-tenant":
      router.push("/home/system/tenant");
      break;
    case "system-code-generator":
      router.push("/home/system/code-generator");
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
const ALLOWED_THEMES = ["enterprise", "blackblue", "default", "green", "purple", "darkblue", "simple-white"];

onMounted(() => {
  const savedTheme = localStorage.getItem("app-theme");
  if (savedTheme && ALLOWED_THEMES.includes(savedTheme)) {
    currentTheme.value = `theme-${savedTheme}`;
  } else {
    currentTheme.value = "theme-simple-white";
    localStorage.setItem("app-theme", "simple-white");
  }

  const raw = localStorage.getItem("user");
  if (raw) {
    try {
      const response = JSON.parse(raw);
      if (response?.name) userInfo.name = response.name;
    } catch {
      /* ignore */
    }
  }
});
</script>

