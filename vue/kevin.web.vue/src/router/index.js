import { createRouter, createWebHistory } from 'vue-router';
import KevinLogin from '@/pages/kevinLogin.vue';
import Home from '@/pages/kevinHome.vue';
import Dashboard from '@/pages/kevinDashboard.vue';
import UserList from '@/pages/UserList.vue';
import UserRole from '@/pages/UserRole.vue';
import UserProfile from '@/pages/UserProfile.vue';
import { getTokenUser } from "../api/userapi";
import { message } from 'ant-design-vue';
// 子页面组件 - 这里可以后续替换为实际的组件
const UserPermission = { template: '<div><h2>权限管理</h2><p>这里是权限管理页面</p></div>' }
const SystemConfig = { template: '<div><h2>系统配置</h2><p>这里是系统配置页面</p></div>' }
const LogManagement = { template: '<div><h2>日志管理</h2><p>这里是日志管理页面</p></div>' }
const Notifications = { template: '<div><h2>通知中心</h2><p>这里是通知中心页面</p></div>' }
const Analytics = { template: '<div><h2>数据分析</h2><p>这里是数据分析页面</p></div>' }

const routes = [
  {
    path: '/',
    redirect: '/login'
  },
  {
    path: '/login',
    name: 'Login',
    component: KevinLogin
  },
  {
    path: '/home',
    name: 'Home',
    component: Home,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: Dashboard
      },
      {
        path: 'user/list',
        name: 'UserList',
        component: UserList
      },
      {
        path: 'user/role',
        name: 'UserRole',
        component: UserRole
      },
      {
        path: 'user/profile',
        name: 'UserProfile',
        component: UserProfile
      },
      {
        path: 'user/permission',
        name: 'UserPermission',
        component: UserPermission
      },
      {
        path: 'system/config',
        name: 'SystemConfig',
        component: SystemConfig
      },
      {
        path: 'system/log',
        name: 'LogManagement',
        component: LogManagement
      },
      {
        path: 'system/notifications',
        name: 'Notifications',
        component: Notifications
      },
      {
        path: 'analytics',
        name: 'Analytics',
        component: Analytics
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// 添加路由守卫
router.beforeEach((to, from, next) => {
  // 检查是否需要认证
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const isAuthenticated = localStorage.getItem('token'); 
   getTokenUser().then((response) => 
        {
          if(response.status==200){ 
            if (response.data.code == 200) {
               localStorage.setItem('user',JSON.stringify(response.data.data));
               console.warn(response.data.data);
            }
          }else{
            message.warning("登录失败");
            localStorage.removeItem('token');
              localStorage.removeItem('user');
            next('/login');
          }
 });
  if (requiresAuth && !isAuthenticated) {
    next('/login');
  } else if (to.path === '/login' && isAuthenticated) {
    next('/home');
  } else {
    next();
  }
});

export default router;