<template>
  <div class="user-profile-container">
    <a-card class="user-profile-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <UserOutlined class="title-icon" />
            <span>个人中心</span>
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

        <!-- 本月 AI Token 额度 -->
        <div class="token-quota-section">
          <div class="quota-header">
            <div class="header-title">
              <ThunderboltOutlined class="title-icon" />
              <span>本月 AI Token 额度</span>
            </div>
            <span class="quota-month">{{ quotaMonthText }}</span>
          </div>
          <a-row :gutter="24" class="quota-stats">
            <a-col :span="8">
              <div class="quota-item">
                <div class="quota-label">本月额度</div>
                <div class="quota-value">{{ formatToken(quota.totalQuota) }}</div>
              </div>
            </a-col>
            <a-col :span="8">
              <div class="quota-item">
                <div class="quota-label">已用额度</div>
                <div class="quota-value quota-used">{{ formatToken(quota.usedQuota) }}</div>
              </div>
            </a-col>
            <a-col :span="8">
              <div class="quota-item">
                <div class="quota-label">剩余额度</div>
                <div class="quota-value quota-remaining">{{ formatToken(quota.remainingQuota) }}</div>
              </div>
            </a-col>
          </a-row>
          <a-progress :percent="usedPercent" :status="usedPercent >= 100 ? 'exception' : 'normal'" :show-info="false"
            :stroke-color="progressStrokeColor" trail-color="#f0f0f0" :size="12" class="quota-progress" />
          <div class="quota-tip">额度每月 1 日自动更新，用量按每轮对话的输入 + 输出 Token 累计</div>
        </div>
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
import { ref, reactive, computed, onMounted } from 'vue';
import { UserOutlined, ThunderboltOutlined } from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import { changePassword } from '@/api/userapi';
import { getMyTokenQuota } from '@/api/ai/aiTokenQuota';

// 用户数据默认结构（后端 dtoUserInfo 等字段可为 null，模板取值前必须兜底）
const createDefaultUser = () => ({
  id: '',
  name: '',
  nickName: '',
  phone: '',
  email: '',
  roles: [],
  createTime: '',
  headImgs: [],
  dtoUserInfo: {},
  positions: [],
});

// 定义用户数据
const user = ref(createDefaultUser());

const defaultAvatar = '/img/hede.4f50e202.png';

// 本月 AI Token 额度（long 在接口中序列化为字符串，展示前统一转数字）
const quota = ref({
  quotaMonth: 0,
  totalQuota: 0,
  usedQuota: 0,
  remainingQuota: 0
});

const quotaMonthText = computed(() => {
  const month = Number(quota.value.quotaMonth || 0);
  if (!month) return '';
  return `${Math.floor(month / 100)} 年 ${month % 100} 月`;
});

const usedPercent = computed(() => {
  const total = Number(quota.value.totalQuota || 0);
  if (total <= 0) return 0;
  const percent = (Number(quota.value.usedQuota || 0) / total) * 100;
  // 保留两位小数，避免四舍五入成整数后进度条看起来没动
  return Math.min(100, Math.round(percent * 100) / 100);
});

// 消耗进度条颜色：80% 起预警变橙，用完交给 exception 状态色（红），其余用主题强调色
const progressStrokeColor = computed(() => {
  if (usedPercent.value >= 100) return undefined;
  return usedPercent.value >= 80 ? '#faad14' : 'var(--accent)';
});

const formatToken = (value) => {
  const num = Number(value || 0);
  if (num >= 100000000) return `${(num / 100000000).toFixed(2)} 亿`;
  if (num >= 10000) return `${(num / 10000).toFixed(2)} 万`;
  return num.toLocaleString();
};

// 获取当月 Token 额度
const fetchTokenQuota = async () => {
  try {
    const res = await getMyTokenQuota();
    // http 拦截器返回完整响应包 {code, msg, isSuccess, data}
    if (res && res.code === 200 && res.data) {
      quota.value = res.data;
    }
  } catch (error) {
    console.error('获取Token额度失败:', error);
  }
};
   

// 修改密码相关
const changePasswordVisible = ref(false);
const changePasswordLoading = ref(false);
const changePasswordForm = reactive({
  OldPwd: '',
  NewPwd: '',
  confirmPassword: ''
});

// 获取用户信息
// 获取用户信息
const fetchUserProfile = async () => {
  try {
    const response = JSON.parse(localStorage.getItem('user') || 'null');
    if (!response || typeof response !== 'object') return;
    user.value = {
      ...createDefaultUser(),
      ...response,
      // 这些字段接口可能返回 null，直接渲染会导致模板取值报错
      headImgs: Array.isArray(response.headImgs) ? response.headImgs : [],
      roles: Array.isArray(response.roles) ? response.roles : [],
      positions: Array.isArray(response.positions) ? response.positions : [],
      dtoUserInfo: response.dtoUserInfo || {},
    };
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
  fetchTokenQuota();
});
</script>

<style scoped>
@import '../css/management.css';

.user-profile-container {
  height: 100%;
  padding: 20px;
}

.user-profile-card {
  height: 100%;
  background: #fff;
  border-radius: 8px;
  border: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
  overflow: hidden;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.03);
}

:deep(.user-profile-card .ant-card-head) {
  border-bottom: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
  padding: 0 20px;
  background: #fafafa;
}

:deep(.user-profile-card .ant-card-head-title) {
  color: rgba(0, 0, 0, 0.88);
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
  color: rgba(0, 0, 0, 0.88);
}

.title-icon {
  margin-right: 8px;
  font-size: 20px;
  color: var(--accent);
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
  color: rgba(0, 0, 0, 0.88);
}

.user-id {
  color: rgba(0, 0, 0, 0.45);
  font-size: 14px;
  margin: 0;
}

.info-section {
  padding-top: 20px;
}

.token-quota-section {
  margin-top: 24px;
  padding: 20px;
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  background: #fafafa;
}

.quota-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.quota-month {
  color: rgba(0, 0, 0, 0.45);
  font-size: 14px;
}

.quota-item {
  text-align: center;
}

.quota-label {
  color: rgba(0, 0, 0, 0.45);
  font-size: 14px;
  margin-bottom: 8px;
}

.quota-value {
  color: rgba(0, 0, 0, 0.88);
  font-size: 22px;
  font-weight: 600;
}

.quota-used {
  color: var(--accent);
}

.quota-remaining {
  color: #52c41a;
}

.quota-progress {
  margin-top: 16px;
}

.quota-tip {
  color: rgba(0, 0, 0, 0.45);
  font-size: 12px;
  margin-top: 8px;
}

:deep(.info-section .ant-descriptions-item-label) {
  color: rgba(0, 0, 0, 0.88);
  background-color: #fafafa;
}

:deep(.info-section .ant-descriptions-item-content) {
  color: rgba(0, 0, 0, 0.88);
  background-color: #fff;
}

:deep(.info-section .ant-descriptions-view) {
  border: 1px solid #f0f0f0;
}

:deep(.info-section .ant-descriptions-row) {
  border-bottom: 1px solid #f0f0f0;
}

:deep(.info-section .ant-descriptions-row:last-child) {
  border-bottom: none;
}

:deep(.ant-tag) {
  color: var(--accent);
  background: var(--fn-color-brand-light);
  border: 1px solid var(--fn-color-brand-border);
}

.action-buttons {
  margin-top: 30px;
  text-align: right;
}
</style>