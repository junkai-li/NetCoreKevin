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
          <template v-for="menu in menuList" :key="menu.key">
            <a-sub-menu :key="menu.key" v-if="hasPermission(menu.permission) && menu.children">
              <template #icon>
                <component :is="menu.icon" />
              </template>
              <template #title>{{ menu.title }}</template>
              <template v-for="child in menu.children" :key="child.key">
                <a-menu-item :key="child.key" v-if="hasPermission(child.permission)">
                  <template #icon>
                    <component :is="child.icon" />
                  </template>
                  {{ child.title }}
                </a-menu-item>
              </template>
            </a-sub-menu>
          </template>
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

          <a-button type="text" class="theme-switch-button" @click="toggleFullScreen">
            <FullscreenOutlined v-if="!isFullScreen" />
            <FullscreenExitOutlined v-else />
          </a-button>

          <a-dropdown>
            <a-badge :dot="noReadCount > 0">
              <BellOutlined class="header-icon" />
            </a-badge>
            <template #overlay>
              <a-menu class="notification-menu">
                <a-menu-item>{{ noReadCount > 0 ? `您有${noReadCount}条未读消息` : '暂无未读消息' }}</a-menu-item>
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

      <!-- 标签页导航 -->
      <div class="tab-navigation" v-if="openTabs.length > 0">
        <div class="tab-list">
          <div
            v-for="tab in openTabs"
            :key="tab.key"
            class="tab-item"
            :class="{ active: activeTabKey === tab.key }"
            @click="handleTabClick(tab.key)"
          >
            <span class="tab-title">{{ tab.title }}</span>
            <span
              v-if="tab.closable"
              class="tab-close"
              @click.stop="closeTab(tab.key)"
            >
              <CloseOutlined />
            </span>
            <a-dropdown class="tab-action" :trigger="['hover']" @click.stop>
              <MoreOutlined />
              <template #overlay>
                <a-menu>
                  <a-menu-item key="refresh" @click="refreshTab(tab.key)">
                    <ReloadOutlined /> 刷新当前页
                  </a-menu-item>
                  <a-menu-item key="closeOther" @click="closeOtherTabs(tab.key)" v-if="openTabs.length > 1">
                    <CloseSquareOutlined /> 关闭其他
                  </a-menu-item>
                  <a-menu-item key="closeAll" @click="closeAllTabs" v-if="openTabs.length > 1">
                    <MinusSquareOutlined /> 关闭所有
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </div>
        </div>
      </div>

      <!-- 主要内容 -->
      <a-layout-content class="content">
        <div class="content-wrapper">
          <!-- 标签页内容区域 - 只渲染当前活跃的标签页 -->
          <div v-if="openTabs.length > 0 && activeTabKey" class="tab-content">
            <component
              v-if="activeTab"
              :is="getComponentByKey(activeTab.key)"
              v-bind="getComponentProps(activeTab.key)"
            />
          </div>
          <!-- 无标签页时显示默认内容 -->
          <router-view v-else />
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
  MessageOutlined,
  ScheduleOutlined,
  AppstoreOutlined,
  FileTextOutlined,
  BookOutlined,
  ToolOutlined,
  AuditOutlined,
  TeamOutlined,
  SafetyOutlined,
  KeyOutlined,
  NotificationOutlined,
  BankOutlined,
  CodeOutlined,
  HistoryOutlined,
  FullscreenOutlined,
  FullscreenExitOutlined,
  CloseOutlined,
  ReloadOutlined,
  MoreOutlined,
  CloseSquareOutlined,
  MinusSquareOutlined,
} from "@ant-design/icons-vue";
import { GetMyNoReadCount } from "@/api/message";
//import { Button } from 'ant-design-vue';
import { useRouter } from "vue-router";
import "../css/kevinHome.css";
import hedeImage from "../assets/hede.png"; // 导入用户头像图片
import logoImage from "../assets/logo.png"; // 导入logo图片

// 导入页面组件
import UserList from "./UserList.vue";
import UserRole from "./UserRole.vue";
import PermissionMg from "./PermissionMg.vue";
import DicConfig from "./DicMg.vue";
import SystemAnnouncement from "./SystemAnnouncement.vue";
import HttpLogMg from "./HttpLog.vue";
import OSLogMG from "./OSLog.vue";
import AiAppsMg from "./ai/AgentManagement.vue";
import AiPromptsMg from "./ai/PromptManagement.vue";
import AiKmssMg from "./ai/KnowledgeBaseManagement.vue";
import AiModelMg from "./ai/ModelManagement.vue";
import AiSkillToolMg from "./ai/SkillToolManagement.vue";
import MyMessages from "./MyMessages.vue";
import MyAIChat from "./ai/MyAIChat.vue";
import MyAITasks from "./ai/MyAITasks.vue";
import MyAgentList from "./ai/MyAgentList.vue";
import PositionManagement from "./organizational/PositionManagement.vue";
import DepartmentManagement from "./organizational/DepartmentManagement.vue";
import TenantManagement from "./TenantManagement.vue";
import CodeGenerator from "./CodeGenerator.vue";
import KevinDashboard from "./kevinDashboard.vue";
import UserProfile from "./UserProfile.vue";

const router = useRouter();

const collapsed = ref(false);
const selectedKeys = ref(["dashboard"]);
const openKeys = ref(["user-management"]);

// 标签页相关
const openTabs = ref([]);
const activeTabKey = ref('');

// 路由key到组件的映射
const routeComponentMap = {
  'dashboard': KevinDashboard,
  'user-list': UserList,
  'user-role': UserRole,
  'user-permission': PermissionMg,
  'system-dic': DicConfig,
  'system-announcement': SystemAnnouncement,
  'log-management': HttpLogMg,
  'oslog': OSLogMG,
  'ai-appsmg': AiAppsMg,
  'ai-promptsmg': AiPromptsMg,
  'ai-kmssmg': AiKmssMg,
  'ai-modelmg': AiModelMg,
  'ai-skilltoolmg': AiSkillToolMg,
  'my-message': MyMessages,
  'my-ai-chat': MyAIChat,
  'my-ai-tasks': MyAITasks,
  'my-ai-agents': MyAgentList,
  'organizational-position': PositionManagement,
  'organizational-department': DepartmentManagement,
  'system-tenant': TenantManagement,
  'system-code-generator': CodeGenerator,
  'handleUserInfo': UserProfile,
};

// 计算当前活动的标签页
const activeTab = computed(() => {
  return openTabs.value.find(tab => tab.key === activeTabKey.value);
});

// 根据key获取组件
const getComponentByKey = (key) => {
  return routeComponentMap[key] || null;
};

// 获取组件props
const getComponentProps = (key) => {
  return { key: `tab_${key}_${Date.now()}` };
};

// 页面标题映射
const pageTitleMap = {
  'user-list': '用户列表',
  'user-role': '角色管理',
  'user-permission': '权限管理',
  'system-dic': '系统配置',
  'system-announcement': '系统公告',
  'log-management': '日志管理',
  'oslog': '关键数据变动日志',
  'ai-appsmg': '智能体管理',
  'ai-promptsmg': '提示词管理',
  'ai-kmssmg': '知识库管理',
  'ai-modelmg': '模型配置管理',
  'ai-skilltoolmg': 'AI技能工具管理',
  'my-message': '我的消息',
  'my-ai-chat': '我的AI对话',
  'my-ai-tasks': '我的AI自动任务',
  'my-ai-agents': '我的可用智能体',
  'organizational-position': '岗位管理',
  'organizational-department': '部门管理',
  'system-tenant': '租户管理',
  'system-code-generator': '代码生成器',
  'dashboard': '首页',
  'handleUserInfo': '个人中心',
};

// 打开标签页
const openTab = (key, title) => {
  // 如果页面已经打开，则激活并刷新
  const existingTab = openTabs.value.find(tab => tab.key === key);
  if (existingTab) {
    activeTabKey.value = key;
    // 刷新页面
    refreshTab(key);
    return;
  }

  // 添加新标签页
  const newTab = {
    key: key,
    title: title,
    closable: true,
    cacheKey: routeComponentMap[key] || key,
    refreshKey: Date.now()
  };
  openTabs.value.push(newTab);
  activeTabKey.value = key;
};

// 关闭标签页
const closeTab = (key) => {
  const index = openTabs.value.findIndex(tab => tab.key === key);
  if (index === -1) return;

  // 如果关闭的是当前激活的标签，切换到上一个或下一个
  if (activeTabKey.value === key) {
    const nextTab = openTabs.value[index + 1] || openTabs.value[index - 1];
    if (nextTab) {
      activeTabKey.value = nextTab.key;
      // 路由到下一个页面
      router.push(getRoutePath(nextTab.key));
    } else {
      activeTabKey.value = '';
    }
  }

  // 移除标签页
  openTabs.value.splice(index, 1);
};

// 根据key获取路由路径
const getRoutePath = (key) => {
  const paths = {
    'user-list': '/home/user/list',
    'user-role': '/home/user/role',
    'user-permission': '/home/user/permission',
    'system-dic': '/home/system/dic',
    'system-announcement': '/home/system/announcement',
    'log-management': '/home/system/log',
    'oslog': '/home/system/oslog',
    'ai-appsmg': '/home/aimanagement/aiappsmg',
    'ai-promptsmg': '/home/aimanagement/aipromptsmg',
    'ai-kmssmg': '/home/aimanagement/aikmssmg',
    'ai-modelmg': '/home/aimanagement/aimodelmg',
    'ai-skilltoolmg': '/home/aimanagement/aiskilltoolmg',
    'my-message': '/home/my/message',
    'my-ai-chat': '/home/my/ai-chat',
    'my-ai-tasks': '/home/my/ai-tasks',
    'my-ai-agents': '/home/my/ai-agents',
    'organizational-position': '/home/position/management',
    'organizational-department': '/home/department/management',
    'system-tenant': '/home/system/tenant',
    'system-code-generator': '/home/system/code-generator',
    'handleUserInfo': '/home/user/profile',
  };
  return paths[key] || '/home';
};

// 刷新单个标签页
const refreshTab = (key) => {
  const tab = openTabs.value.find(t => t.key === key);
  if (tab) {
    // 更新refreshKey来触发组件重新渲染
    tab.refreshKey = Date.now();
    activeTabKey.value = key;
  }
};

// 关闭其他标签页
const closeOtherTabs = (key) => {
  openTabs.value = openTabs.value.filter(tab => tab.key === key || !tab.closable);
  activeTabKey.value = key;
};

// 关闭所有标签页
const closeAllTabs = () => {
  openTabs.value = openTabs.value.filter(tab => !tab.closable);
  activeTabKey.value = '';
};

// Tab点击事件
const handleTabClick = (key) => {
  activeTabKey.value = key;
  router.push(getRoutePath(key));
};

const currentTheme = ref("theme-simple-white");

const menuTheme = computed(() => {
  return currentTheme.value === "theme-simple-white" ? "light" : "dark";
});

// 用户信息
const userInfo = reactive({
  name: "管理员",
  avatar: hedeImage,
});

const noReadCount = ref(0);
const isFullScreen = ref(false);

const toggleFullScreen = () => {
  if (!document.fullscreenElement) {
    document.documentElement.requestFullscreen();
    isFullScreen.value = true;
  } else {
    if (document.exitFullscreen) {
      document.exitFullscreen();
      isFullScreen.value = false;
    }
  }
};

const fetchNoReadCount = async () => {
  try {
    const response = await GetMyNoReadCount();
    if (response.code === 200) {
      noReadCount.value = response.data || 0;
    }
  } catch (error) {
    console.error("获取未读消息数量失败:", error);
  }
};

// 用户权限列表
const userPermissions = ref([]);

// 权限检查函数
const hasPermission = (permission) => {
  if (!permission) return true;
  // 如果没有权限数据（本地存储为空或无效），默认显示所有菜单
  if (!userPermissions.value || userPermissions.value.length === 0) return true;
  return userPermissions.value.includes(permission);
};

// 菜单列表
const menuList = ref([
  {
    key: 'dashboard',
    title: '首页',
    icon: DashboardOutlined,
    permission: 'Menu/Home',
    children: [
      { key: 'dashboard', title: '首页', permission: 'Menu/Home/Index', icon: DashboardOutlined }
    ]
  },
  {
    key: 'my',
    title: '我的',
    icon: RobotOutlined,
    permission: 'Menu/MyMenu',
    children: [
      { key: 'my-message', title: '我的消息', permission: 'Menu/MyMenu/MyMessage', icon: MessageOutlined },
      { key: 'my-ai-chat', title: '我的AI对话', permission: 'Menu/MyMenu/MyAiChat', icon: RobotOutlined },
      { key: 'my-ai-tasks', title: '我的AI自动任务', permission: 'Menu/MyMenu/MyAiTasks', icon: ScheduleOutlined },
      { key: 'my-ai-agents', title: '我的可用智能体', permission: 'Menu/MyMenu/MyAiAgents', icon: AppstoreOutlined }
    ]
  },
  {
    key: 'aimanagement',
    title: 'AI管理',
    icon: GitlabOutlined,
    permission: 'Menu/AIManagement',
    children: [
      { key: 'ai-appsmg', title: '智能体管理', permission: 'Menu/AIManagement/AIAppsManagement', icon: RobotOutlined },
      { key: 'ai-promptsmg', title: '提示词管理', permission: 'Menu/AIManagement/AIPromptsManagement', icon: FileTextOutlined },
      { key: 'ai-kmssmg', title: '知识库管理', permission: 'Menu/AIManagement/AIKmssManagement', icon: BookOutlined },
      { key: 'ai-modelmg', title: '模型配置管理', permission: 'Menu/AIManagement/AIModelManagement', icon: SettingOutlined },
      { key: 'ai-skilltoolmg', title: 'AI技能工具管理', permission: 'Menu/AIManagement/AISkillToolManagement', icon: ToolOutlined }
    ]
  },
  {
    key: 'organizational-management',
    title: '组织架构',
    icon: UserOutlined,
    permission: 'Menu/OrganizationalManagement',
    children: [
      { key: 'organizational-position', title: '岗位管理', permission: 'Menu/OrganizationalManagement/PositionManagement', icon: AuditOutlined },
      { key: 'organizational-department', title: '部门管理', permission: 'Menu/OrganizationalManagement/DepartmentManagement', icon: TeamOutlined }
    ]
  },
  {
    key: 'user-management',
    title: '用户管理',
    icon: UserOutlined,
    permission: 'Menu/UserManagement',
    children: [
      { key: 'user-list', title: '用户列表', permission: 'Menu/UserManagement/UserList', icon: UserOutlined },
      { key: 'user-role', title: '角色管理', permission: 'Menu/UserManagement/UserRole', icon: SafetyOutlined },
      { key: 'user-permission', title: '权限管理', permission: 'Menu/UserManagement/UserPermission', icon: KeyOutlined }
    ]
  },
  {
    key: 'system-settings',
    title: '系统管理',
    icon: SettingOutlined,
    permission: 'Menu/SystemSettings',
    children: [
      { key: 'system-announcement', title: '系统公告', permission: 'Menu/SystemSettings/SystemAnnouncement', icon: NotificationOutlined },
      { key: 'system-dic', title: '系统配置', permission: 'Menu/SystemSettings/SystemDic', icon: SettingOutlined },
      { key: 'system-tenant', title: '租户管理', permission: 'Menu/SystemSettings/SystemTenant', icon: BankOutlined },
      { key: 'system-code-generator', title: '代码生成器', permission: 'Menu/SystemSettings/SystemCodeGenerator', icon: CodeOutlined },
      { key: 'log-management', title: '日志管理', permission: 'Menu/SystemSettings/LogManagement', icon: FileTextOutlined }, 
      { key: 'oslog', title: '关键数据变动日志', permission: 'Menu/SystemSettings/OsLog', icon: HistoryOutlined }
    ]
  }
]);

// 切换主题
const switchTheme = (theme) => {
  currentTheme.value = `theme-${theme}`;
  // 保存主题选择到本地存储
  localStorage.setItem("app-theme", theme);
};

// 菜单点击事件
const handleMenuClick = ({ key }) => {
  selectedKeys.value = [key];

  // 获取页面标题
  const title = pageTitleMap[key] || key;

  // 打开标签页
  openTab(key, title);

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
    case "my-ai-tasks":
      router.push("/home/my/ai-tasks");
      break;
    case "my-ai-agents":
      router.push("/home/my/ai-agents");
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
    case "ai-skilltoolmg":
      router.push("/home/aimanagement/aiskilltoolmg");
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
    case "dashboard":
      router.push("/home");
      break;
    case "handleUserInfo":
      router.push("/home/user/profile");
      break;
    default:
      router.push("/home");
  }
};

// 退出登录
const handleLogout = () => {
  console.log("用户退出登录");
  localStorage.removeItem("token");
  localStorage.removeItem("UserPermissions");
  router.push("/login");
};
// 个人中心
const handleUserInfo = () => {
  console.log("个人中心");
  selectedKeys.value = ["handleUserInfo"];
  openTab("handleUserInfo", "个人中心");
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
  const permissionsRaw = localStorage.getItem("UserPermissions");
  if (permissionsRaw) {
    try {
      const permissions = JSON.parse(permissionsRaw);
       const user = JSON.parse(raw);
      if (Array.isArray(permissions)) { 
         permissions.forEach((item, index) => { 
          permissions[index] = item.replace(String(user.tenantId+'/'), "");
        });     
        userPermissions.value = permissions;   
      }
    } catch {
      // 如果本地存储的权限数据无效，使用模拟数据
      userPermissions.value = getAllPermissions();
    }
  } else {
    // 模拟权限数据（开发阶段使用）
    userPermissions.value = getAllPermissions();
  }

  fetchNoReadCount();

  document.addEventListener('fullscreenchange', () => {
    isFullScreen.value = !!document.fullscreenElement;
  });
});

// 获取所有权限（用于默认显示全部菜单）
const getAllPermissions = () => {
  return [
    'Menu/Home','Menu/Home/Index',
    'Menu/MyMenu', 'Menu/MyMenu/MyMessage', 'Menu/MyMenu/MyAiChat', 'Menu/MyMenu/MyAiTasks', 'Menu/MyMenu/MyAiAgents',
    'Menu/AIManagement', 'Menu/AIManagement/AIAppsManagement', 'Menu/AIManagement/AIPromptsManagement', 'Menu/AIManagement/AIKmssManagement', 'Menu/AIManagement/AIModelManagement', 'Menu/AIManagement/AISkillToolManagement',
    'Menu/OrganizationalManagement', 'Menu/OrganizationalManagement/PositionManagement', 'Menu/OrganizationalManagement/DepartmentManagement',
    'Menu/UserManagement', 'Menu/UserManagement/UserList', 'Menu/UserManagement/UserRole', 'Menu/UserManagement/UserPermission',
    'Menu/SystemSettings', 'Menu/SystemSettings/SystemAnnouncement', 'Menu/SystemSettings/SystemDic', 'Menu/SystemSettings/SystemTenant', 'Menu/SystemSettings/SystemCodeGenerator', 'Menu/SystemSettings/LogManagement', 'Menu/SystemSettings/OsLog'
  ];
};
</script>

