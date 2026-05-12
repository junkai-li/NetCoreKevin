<template>
  <div class="dashboard-container">
    <div class="welcome-card">
      <h1>欢迎来到AI智能体后台管理系统主页</h1>
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
        <a-button type="primary" @click="showAddUserModal">添加用户</a-button>
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
    if (response.code === 200) {
      userCount.value = response.data
    }
  } catch (error) {
    console.error('获取用户数量失败:', error)
  }
}
// 获取用户总数
const MyNoReadCount = async () => {
  try {
    const response = await GetMyNoReadCount()
    if (response.code === 200) {
      myNoReadCount.value = response.data
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
  color: rgba(0, 0, 0, 0.88);
}

.welcome-card {
  background: #fff;
  border-radius: 8px;
  padding: 28px 24px;
  margin-bottom: 20px;
  text-align: center;
  border: 1px solid #f0f0f0;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.03);
}

.welcome-card h1 {
  color: rgba(0, 0, 0, 0.88);
  font-size: 22px;
  font-weight: 600;
  margin-bottom: 8px;
}

.welcome-card p {
  color: rgba(0, 0, 0, 0.45);
  font-size: 14px;
  margin-bottom: 24px;
}

.welcome-stats {
  display: flex;
  justify-content: center;
  gap: 48px;
  margin-top: 8px;
  flex-wrap: wrap;
}

.stat-item {
  text-align: center;
}

.stat-value {
  display: block;
  font-size: 26px;
  font-weight: 600;
  color: #1677ff;
}

.stat-label {
  font-size: 13px;
  color: rgba(0, 0, 0, 0.45);
  margin-top: 4px;
  display: inline-block;
}

.info-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 16px;
  margin-top: 8px;
}

.info-card {
  border-radius: 8px;
  border: 1px solid #f0f0f0;
}

:deep(.info-card .ant-card-head) {
  border-bottom: 1px solid #f0f0f0;
  font-weight: 600;
}

:deep(.info-card .ant-card-body) {
  color: rgba(0, 0, 0, 0.65);
}
</style>
