<template>
  <div class="user-profile-container">
    <a-card class="user-profile-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <UserOutlined class="title-icon" />
            <span style="color: aliceblue;">个人中心</span>
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
          <a-col :span="16" class="info-section">
            <a-descriptions :column="2" bordered>
              <a-descriptions-item label="用户名">
                {{ user.name }}
              </a-descriptions-item>
               <a-descriptions-item label="工号">
                {{ user.dtoUserInfo.employeeNo }}
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
                <a-descriptions-item label="岗位">
                <a-tag v-for="role in user.positions" :key="role.id" color="blue">
                  {{ role.name }}
                </a-tag>
              </a-descriptions-item>
                <a-descriptions-item label="部门">
                <a-tag color="blue">
                 {{ user.dtoUserInfo.departmentName}}
                </a-tag>
              </a-descriptions-item>
                <a-descriptions-item  label="密码">
                <a-tag @click="changePasswordAction" color="red">
                  修改密码
                </a-tag>
              </a-descriptions-item>
            </a-descriptions> 
          </a-col>
        </a-row>
      </div>
    </a-card> 

    <!-- 修改密码模态框 -->
    <a-modal v-model:open="changePasswordVisible" title="修改密码" @ok="handleChangePassword"
      @cancel="cancelChangePassword" :confirm-loading="changePasswordLoading">
      <a-form :model="changePasswordForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="原密码" name="OldPwd" :rules="[{ required: true, message: '请输入原密码' }]">
          <a-input-password v-model:value="changePasswordForm.OldPwd" />
        </a-form-item>
        <a-form-item label="新密码" name="NewPwd" :rules="[{ required: true, message: '请输入新密码' }]">
          <a-input-password v-model:value="changePasswordForm.NewPwd" />
        </a-form-item>
        <a-form-item label="确认密码" name="confirmPassword"
          :rules="[{ required: true, message: '请确认新密码' }, { validator: validateConfirmPassword }]">
          <a-input-password v-model:value="changePasswordForm.confirmPassword" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { UserOutlined } from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import { changePassword } from '@/api/userapi';

// 定义用户数据
const user = ref({
  id: '',
  name: '',
  nickName: '',
  phone: '',
  email: '',
  roles: [],
  createTime: '',
  headImgs: [],
  dtoUserInfo:{},
  positions:[],

});

const defaultAvatar = '/img/hede.4f50e202.png';
   

// 修改密码相关
const changePasswordVisible = ref(false);
const changePasswordLoading = ref(false);
const changePasswordForm = reactive({
  OldPwd: '',
  NewPwd: '',
  confirmPassword: ''
});

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
 
 

// 修改密码
const changePasswordAction = () => {
  // 重置表单
  changePasswordForm.OldPwd = '';
  changePasswordForm.NewPwd = '';
  changePasswordForm.confirmPassword = '';
  changePasswordVisible.value = true;
};

// 验证确认密码
const validateConfirmPassword = (_, value) => {
  if (value && value !== changePasswordForm.NewPwd) {
    return Promise.reject('两次输入的密码不一致');
  }
  return Promise.resolve();
};

// 处理修改密码
const handleChangePassword = async () => {
  try {
    changePasswordLoading.value = true;
    
    // 简单的前端验证
    if (!changePasswordForm.OldPwd) {
      message.error('请输入原密码');
      return;
    }
    
    if (!changePasswordForm.NewPwd) {
      message.error('请输入新密码');
      return;
    }
    
    if (changePasswordForm.NewPwd !== changePasswordForm.confirmPassword) {
      message.error('两次输入的密码不一致');
      return;
    }
    
    // 调用API修改密码
    const data = {
      OldPwd: changePasswordForm.OldPwd,
      NewPwd: changePasswordForm.NewPwd
    };
    
    await changePassword(data);
    
    message.success('密码修改成功');
    changePasswordVisible.value = false;
  } catch (error) {
    console.error('修改密码失败:', error);
    message.error('修改密码失败: ' + (error.message || '未知错误'));
  } finally {
    changePasswordLoading.value = false;
  }
};

// 取消修改密码
const cancelChangePassword = () => {
  changePasswordVisible.value = false;
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
  color: aliceblue;
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