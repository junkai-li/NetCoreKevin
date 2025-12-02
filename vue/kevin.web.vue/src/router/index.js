import { createRouter, createWebHistory } from 'vue-router';
import KevinLogin from '@/pages/kevinLogin.vue';
import Home from '@/pages/kevinHome.vue';
import Dashboard from '@/pages/kevinDashboard.vue';
import UserList from '@/pages/UserList.vue';
import UserRole from '@/pages/UserRole.vue';
import UserProfile from '@/pages/UserProfile.vue';
import UnderDevelopment from '@/pages/UnderDevelopment.vue';
import { getTokenUser } from "../api/userapi";
import { message } from 'ant-design-vue';
import PermissionMg from '@/pages/PermissionMg.vue'; 
import HttpLogMg from '@/pages/HttpLog.vue'; 
import OSLogMG from '@/pages/OSLog.vue';   
// 消息管理相关页面
import SystemAnnouncement from '@/pages/SystemAnnouncement.vue';
import MyMessages from '@/pages/MyMessages.vue';
import DicConfig from '@/pages/DicMg.vue';
// AI管理相关页面
import AiAppsMg from '@/pages/ai/AgentManagement.vue';
import AiPromptsMg from '@/pages/ai/PromptManagement.vue';
// import AiKmssMg from '@/pages/AiKmssMg.vue';
import AiModelMg from '@/pages/ai/ModelManagement.vue';
import MyAIChat from '@/pages/ai/MyAIChat.vue'; // 添加AI对话界面组件
//组织架构相关页面
import PositionManagement from '@/pages/organizational/PositionManagement.vue';
import DepartmentManagement from '@/pages/organizational/DepartmentManagement.vue';
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
        name: 'PermissionMg',
        component: PermissionMg
      },
      {
        path: 'system/dic',
        name: 'dicConfig',
        component: DicConfig
      },
      {
        path: 'system/log',
        name: 'HttpLogMg',
        component: HttpLogMg
      },
      {
        path: 'system/oslog',
        name: 'OSLogMG',
        component: OSLogMG
      },
      {
        path: 'system/announcement',
        name: 'SystemAnnouncement',
        component: SystemAnnouncement
      },
      {
        path: 'my/message',
        name: 'MyMessages',
        component: MyMessages
      },
        {
        path: 'my/ai-chat',
        name: 'MyAIChat',
        component: MyAIChat
      },
      {
        path: 'underdevelopment',
        name: 'UnderDevelopment',
        component: UnderDevelopment
      },
      {
        path: 'aimanagement',
        name: 'aimanagement',
        component: UnderDevelopment
      },
        {
        path: 'aimanagement/aiappsmg',
        name: 'aiappsmg',
        component: AiAppsMg
      },
        {
        path: 'aimanagement/aipromptsmg',
        name: 'aipromptsmg',
        component: AiPromptsMg
      },
        {
        path: 'aimanagement/aikmssmg',
        name: 'aikmssmg',
        component: UnderDevelopment
      },
        {
        path: 'aimanagement/aimodelmg',
        name: 'aimodelmg',
        component: AiModelMg
      },{
        path: 'position/management',
        name: 'positionManagement',
        component: PositionManagement
      }  ,{
        path: 'department/management',
        name: 'departmentManagement',
        component: DepartmentManagement
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
  console.log("路由守卫");
  // 检查是否需要认证
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const isAuthenticated = localStorage.getItem('token'); 
  if (isAuthenticated) {
   getTokenUser().then((response) => 
        {
          if(response.code==200){ 
            if (response.code == 200) {
               localStorage.setItem('user',JSON.stringify(response.data));
               console.warn(response.data);
            }
          }else{
            message.warning("登录失败");
            localStorage.removeItem('token');
              localStorage.removeItem('user');
            next('/login');
          }
 });
  }
   
  if (requiresAuth && !isAuthenticated) {
     console.log("login");
    next('/login');
  } else if (to.path === '/login' && isAuthenticated) {
    next('/home');
     console.log("home");
  } else {
    next();
     console.log("next");
  }
});

export default router;