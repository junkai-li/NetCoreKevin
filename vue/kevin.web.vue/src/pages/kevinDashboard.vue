<template>
  <div class="dashboard-container">
    <div class="welcome-card">
      <h1>欢迎来到NetCoreKevin后台管理系统主页</h1>
      <p>您已成功登录系统</p>
      <div class="welcome-stats">
        <div class="stat-item">
          <span class="stat-value">{{ userCount }}</span>
          <span class="stat-label">用户数</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">{{myNoReadCount}}</span>
          <span class="stat-label">我的未读消息</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">98%</span>
          <span class="stat-label">系统健康度</span>
        </div>
      </div>
    </div>
    
    <div class="info-cards">
      <a-card class="info-card" title="系统状态">
        <p>系统运行正常</p>
        <p>最后更新: 2025-11-13 12:00</p>
      </a-card>
      
      <a-card class="info-card" title="最新消息">
        <p>系统将在今晚进行维护</p>
        <p>预计时间: 2小时</p>
      </a-card>
      
      <a-card class="info-card" title="快捷操作">
        <a-button type="primary" @click="showAddUserModal" ghost>添加用户</a-button>
        <!-- <a-button type="primary" ghost style="margin-left: 10px;">系统设置</a-button> -->
      </a-card>
    </div>
      <!-- 用户管理模态框 -->
    <UserAddEdit
      :visible="userModalVisible"
      :title="userModalTitle"
      :user="currentUser"
      @ok="handleUserModalOk"
      @cancel="handleUserModalCancel"
    />
  </div>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, onMounted } from 'vue'
import { getAllUserCount } from '@/api/userapi.js'
import { GetMyNoReadCount } from '@/api/message.js' 
import UserAddEdit from '@/components/UserAddEdit.vue'
// 用户数量
const userCount = ref(128)
// not数量
const myNoReadCount = ref(247)
// 模态框状态
const userModalVisible = ref(false);
const userModalTitle = ref("添加用户");
// 当前编辑的用户
const currentUser = ref(null);
// 显示添加用户模态框
const showAddUserModal = () => {
  userModalTitle.value = "添加用户";    
  userModalVisible.value = true;
};
// Emit事件
const emit = defineEmits(['user-added', 'user-updated', 'user-deleted', 'user-exported', 'load-success', 'load-error']);

// 用户模态框确认
const handleUserModalOk = async (userData) => {
  userModalVisible.value = false; 
    emit('user-added', userData); 
};
// 用户模态框取消
const handleUserModalCancel = () => {
  userModalVisible.value = false;
};
// 获取用户总数
const fetchUserCount = async () => {
  try {
    const response = await getAllUserCount()
    if (response.data.code === 200) {
      userCount.value = response.data.data
    }
  } catch (error) {
    console.error('获取用户数量失败:', error)
  }
}
// 获取用户总数
const MyNoReadCount = async () => {
  try {
    const response = await GetMyNoReadCount()
    if (response.data.code === 200) {
      myNoReadCount.value = response.data.data
    }
  } catch (error) {
    console.error('获取未读数量失败:', error)
  }
}

// 组件挂载时获取用户数量
onMounted(() => {
  fetchUserCount()
  MyNoReadCount();
})
</script>

<style scoped>
.dashboard-container {
  color: white;
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

:deep(.ant-btn-background-ghost.ant-btn-primary) {
  color: #667eea;
  border-color: #667eea;
  background: transparent;
}

:deep(.ant-btn-background-ghost.ant-btn-primary:hover) {
  color: #764ba2;
  border-color: #764ba2;
  box-shadow: 0 0 8px rgba(102, 126, 234, 0.6);
}
</style>