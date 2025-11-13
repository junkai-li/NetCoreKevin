 <template>
  <a-layout class="layout-container">
    <!-- 左侧导航栏 -->
    <a-layout-sider 
      v-model:collapsed="collapsed" 
      :trigger="null" 
      collapsible 
      class="sider"
      width="256"
    >
      <div class="logo">
        <img v-if="!collapsed" src="@/assets/logo.png" alt="Logo" class="logo-img" />
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
         <a-menu-item key="analytics">
          <template #icon>
            <BarChartOutlined />
          </template>
          <span>AI</span>
        </a-menu-item> 
        <a-sub-menu key="user-management">
          <template #icon>
            <UserOutlined />
          </template>
          <template #title>用户管理</template>
          <a-menu-item key="user-list">用户列表</a-menu-item>
          <a-menu-item key="user-role">角色管理</a-menu-item>
          <a-menu-item key="user-permission">权限管理</a-menu-item>
        </a-sub-menu>
        
        <a-sub-menu key="system-settings">
          <template #icon>
            <SettingOutlined />
          </template>
          <template #title>系统管理</template>
          <a-menu-item key="system-config">系统配置</a-menu-item>
          <a-menu-item key="log-management">日志管理</a-menu-item>
            <a-menu-item key="notifications">通知中心</a-menu-item>
        </a-sub-menu>
        
       
      </a-menu>
    </a-layout-sider>
    
    <!-- 右侧内容区域 -->
    <a-layout>
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
          <a-dropdown>
            <a-badge dot>
              <BellOutlined class="header-icon" />
            </a-badge>
            <template #overlay>
              <a-menu>
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
              <a-menu>
                <a-menu-item key="profile">
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
          <h1>欢迎来到管理系统主页</h1>
          <p>您已成功登录系统</p>
        </div>
      </a-layout-content>
      
      <!-- 底部 -->
      <a-layout-footer class="footer">
        <div class="footer-content">
          <span>© 2024 Kevin管理系统. All Rights Reserved.</span>
        </div>
      </a-layout-footer>
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import {
  MenuUnfoldOutlined,
  MenuFoldOutlined,
  DashboardOutlined,
  UserOutlined,
  SettingOutlined,
  BarChartOutlined,
  BellOutlined,
  LogoutOutlined
} from '@ant-design/icons-vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const collapsed = ref(false)
const selectedKeys = ref(['dashboard'])
const openKeys = ref(['user-management'])

// 用户信息
const userInfo = reactive({
  name: '管理员',
  avatar: 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'
})

// 当前路由标题
const currentRouteTitle = computed(() => {
  const titles = {
    'dashboard': '仪表盘',
    'user-list': '用户列表',
    'user-role': '角色管理',
    'user-permission': '权限管理',
    'system-config': '系统配置',
    'log-management': '日志管理',
    'analytics': '数据分析',
    'notifications': '通知中心'
  }
  return titles[selectedKeys.value[0]] || '仪表盘'
})

// 菜单点击事件
const handleMenuClick = ({ key }) => {
  selectedKeys.value = [key]
  // 这里可以根据key跳转到对应路由
  console.log('Navigate to:', key)
}

// 退出登录
const handleLogout = () => {
  console.log('用户退出登录')
  // 清除认证状态并跳转到登录页
  localStorage.removeItem('token')
  router.push('/login')
}
</script>

<style scoped>
.layout-container {
  min-height: 100vh;
}

.sider {
  height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  box-shadow: 2px 0 8px 0 rgba(29, 35, 41, 0.05);
  z-index: 10;
}

.logo {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.02);
  overflow: hidden;
}

.logo-img {
  height: 32px;
  margin-right: 12px;
}

.logo-text {
  color: white;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
}

.header {
  background: #fff;
  padding: 0 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  position: sticky;
  top: 0;
  z-index: 9;
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
}

.trigger:hover {
  color: #1890ff;
}

.breadcrumb {
  margin-left: 16px;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 24px;
}

.header-icon {
  font-size: 18px;
  color: rgba(0, 0, 0, 0.65);
  cursor: pointer;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  gap: 8px;
}

.user-name {
  color: rgba(0, 0, 0, 0.85);
}

.content {
  margin: 24px 16px 0;
  overflow: initial;
}

.content-wrapper {
  padding: 24px;
  background: #fff;
  min-height: calc(100vh - 168px);
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
}

.footer {
  text-align: center;
  padding: 16px 24px;
  background: #fff;
  margin-top: 24px;
}

.footer-content {
  color: rgba(0, 0, 0, 0.45);
  font-size: 14px;
}

/* 响应式设计 */
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
}
</style>