<template>
  <div class="user-profile-container">
    <a-card class="user-profile-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <UserOutlined class="title-icon" />
            <span style="color: aliceblue;" >个人中心</span>
          </div>
        </div>
      </template>

      <div class="profile-content">
        <a-row :gutter="24">
          <!-- 用户头像部分 -->
          <a-col :span="8" class="avatar-section">
            <div class="avatar-wrapper">
              <a-avatar :size="128" :src="user.headImgs && user.headImgs.length > 0 ? user.headImgs[0] : defaultAvatar">
                <template #icon>
                  <UserOutlined />
                </template>
              </a-avatar>
              <h3>{{ user.nickName || user.name }}</h3>
              <p class="user-id">ID: {{ user.id }}</p>
            </div>
          </a-col>

          <!-- 用户信息部分 -->
          <a-col :span="16"   class="info-section">
            <a-descriptions  :column="1" bordered>
              <a-descriptions-item label="用户名">
                {{ user.name }}
              </a-descriptions-item>
              <a-descriptions-item label="昵称">
                {{ user.nickName }}
              </a-descriptions-item>
              <a-descriptions-item label="手机号">
                {{ user.phone }}
              </a-descriptions-item>
              <a-descriptions-item label="邮箱">
                {{ user.email }}
              </a-descriptions-item>
              <a-descriptions-item label="创建时间">
                {{ user.createTime }}
              </a-descriptions-item>
              <a-descriptions-item label="角色">
                <a-tag v-for="role in user.roles" :key="role.id" color="blue">
                  {{ role.name }}
                </a-tag>
              </a-descriptions-item>
            </a-descriptions>

            <div class="action-buttons">
              <a-button type="primary" @click="editProfile">
                编辑资料
              </a-button>
              <a-button @click="changePassword" style="margin-left: 12px;">
                修改密码
              </a-button>
            </div>
          </a-col>
        </a-row>
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { UserOutlined } from '@ant-design/icons-vue'; 

// 定义用户数据
const user = ref({
  id: '',
  name: '',
  nickName: '',
  phone: '',
  email: '',
  roles: [],
  createTime: '',
  headImgs: []
});

const defaultAvatar = '/img/hede.4f50e202.png';

// 获取用户信息
const fetchUserProfile = async () => {
  try {
    const response = JSON.parse(localStorage.getItem('user'));
    console.log(response);
    user.value = response;
  } catch (error) {
    console.error('获取用户信息失败:', error);
  }
};

// 编辑资料
const editProfile = () => {
  // TODO: 实现编辑资料功能
  console.log('编辑资料');
};

// 修改密码
const changePassword = () => {
  // TODO: 实现修改密码功能
  console.log('修改密码');
};

// 组件挂载时获取用户信息
onMounted(() => {
  fetchUserProfile();
});
</script>

<style scoped>
@import '../css/UserList.css';
@import '../css/UserProfile.css';

.user-profile-container {
  height: 100%;
  padding: 20px;
}

.user-profile-card {
  height: 100%;
  background: rgba(255, 255, 255, 0.08);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  overflow: hidden;
  backdrop-filter: blur(10px);
}

:deep(.user-profile-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.user-profile-card .ant-card-head-title) {
  color: white;
  padding: 16px 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-title {
  display: flex;
  align-items: center;
  font-size: 18px;
  font-weight: 500;
}

.title-icon {
  margin-right: 8px;
  font-size: 20px;
  color:aliceblue;
}

.profile-content {
  padding: 20px;
}

.avatar-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start;
  padding-top: 20px;
}

.avatar-wrapper {
  text-align: center;
  margin-bottom: 20px;
}

.avatar-wrapper h3 {
  margin-top: 16px;
  margin-bottom: 8px;
  font-size: 20px;
  color: white;
}

.user-id {
  color: rgba(255, 255, 255, 0.7);
  font-size: 14px;
  margin: 0;
}

.info-section {
  padding-top: 20px;
}

/* 确保描述列表中的文字为白色 */
:deep(.info-section .ant-descriptions-item-label) {
  color: white;
  background-color: rgba(255, 255, 255, 0.08);
}

:deep(.info-section .ant-descriptions-item-content) {
  color: white;
  background-color: rgba(255, 255, 255, 0.08);
}

:deep(.info-section .ant-descriptions-view) {
  border: 1px solid rgba(255, 255, 255, 0.2);
}

:deep(.info-section .ant-descriptions-row) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

:deep(.info-section .ant-descriptions-row:last-child) {
  border-bottom: none;
}

.action-buttons {
  margin-top: 30px;
  text-align: right;
}
</style>