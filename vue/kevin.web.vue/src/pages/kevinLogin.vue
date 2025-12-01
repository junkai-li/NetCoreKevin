<template>
  <div class="login-container">
    <!-- 动态背景 -->
    <div class="background">
      <div class="particles"></div>
      <div class="grid-lines"></div>
      <div class="floating-elements">
        <div class="element element-1"></div>
        <div class="element element-2"></div>
        <div class="element element-3"></div>
        <div class="element element-4"></div>
      </div>
    </div>
    <!--  -->
    <div class="login-card">
      <div class="login-header">
        <div class="logo">
          <img :src="logoImage" alt="Logo" class="logo-image" />
        </div>
        <h1 class="title">NetCoreKevin后台管理系统</h1>
        <p class="subtitle">欢迎登陆NetCoreKevin开源框架系统后台</p>
      </div>

      <a-tabs v-model:activeKey="activeTab" class="login-tabs">
        <a-tab-pane key="password" tab="密码登录">
          <a-form :model="passwordForm" @finish="handlePasswordLogin">
            <a-form-item
              name="username"
              :rules="[{ required: true, message: '请输入用户名!' }]"
            >
              <a-input
                v-model:value="passwordForm.username"
                size="large"
                placeholder="用户名/邮箱/手机号"
                class="custom-input"
              >
                <template #prefix>
                  <UserOutlined />
                </template>
              </a-input>
            </a-form-item>

            <a-form-item
              name="password"
              :rules="[{ required: true, message: '请输入密码!' }]"
            >
              <a-input-password
                v-model:value="passwordForm.password"
                size="large"
                placeholder="密码"
                class="custom-input"
              >
                <template #prefix>
                  <LockOutlined />
                </template>
              </a-input-password>
            </a-form-item>

            <a-form-item
              name="tenantId"
              :rules="[{ required: true, message: '请输入租户id!' }]"
            >
               <a-input
                v-model:value="passwordForm.tenantId"
                size="large"
                prefix="🏠" 
                placeholder="租户"
                class="custom-input"
              > 
              </a-input>
            </a-form-item>

            <div class="form-options">
              <a-checkbox
                v-model:checked="passwordForm.remember"
                style="color: aliceblue"
                class="custom-checkbox"
                >记住我</a-checkbox
              >
              <a href="#" class="forgot-link">忘记密码？</a>
            </div>

            <a-button
              type="primary"
              html-type="submit"
              size="large"
              block
              :loading="loading"
              class="login-button"
            >
              登录
            </a-button>
          </a-form>
        </a-tab-pane>

        <a-tab-pane key="sms" tab="验证码登录">
          <a-form :model="smsForm" @finish="handleSmsLogin">
            <a-form-item
              name="phone"
              :rules="[{ required: true, message: '请输入手机号!' }]"
            >
              <a-input
                v-model:value="smsForm.phone"
                size="large"
                placeholder="手机号"
                class="custom-input"
              >
                <template #prefix>
                  <PhoneOutlined />
                </template>
              </a-input>
            </a-form-item>
         <a-form-item
              name="tenantId"
              :rules="[{ required: true, message: '请输入租户id!' }]"
            >
            <a-input
                v-model:value="smsForm.tenantId" 
               size="large"
                prefix="🏠"  
                placeholder="租户"
                class="custom-input"
              >
                <template #prefix>
                  <UserOutlined />
                </template>
              </a-input>
            </a-form-item>
            <a-form-item
              name="captcha"
              :rules="[{ required: true, message: '请输入验证码!' }]"
            >
              <div class="captcha-wrapper">
                <a-input
                  v-model:value="smsForm.captcha"
                  size="large"
                  placeholder="验证码"
                  class="custom-input"
                >
                  <template #prefix>
                    <MailOutlined />
                  </template>
                </a-input>
                <a-button
                  :disabled="captchaDisabled"
                  @click="getCaptcha"
                  class="captcha-button"
                >
                  {{ captchaText }}
                </a-button>
              </div>
            </a-form-item>
   
            <a-button
              type="primary"
              html-type="submit"
              size="large"
              block
              :loading="loading"
              class="login-button"
            >
              登录
            </a-button>
          </a-form>
        </a-tab-pane>
      </a-tabs>

      <div class="login-footer">
        <span>还没有账户？</span>
        <a href="#" class="register-link">立即注册</a>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, defineOptions, onMounted, onBeforeUnmount } from "vue";
import { useRouter } from "vue-router"; 
import {
  UserOutlined,
  LockOutlined,
  PhoneOutlined,
  MailOutlined,
} from "@ant-design/icons-vue"; 
import { message } from "ant-design-vue";
import { login,getTokenUser } from "../api/userapi";
import { getUserPermissions } from "../api/baseapi";
import logoImage from '../assets/logo.png'; // 导入logo图片

defineOptions({ name: "KevinLogin" });
const activeTab = ref("password");
const loading = ref(false);
const router = useRouter();
// 密码登录表单
const passwordForm = reactive({
  username: "",
  password: "",
  tenantId: "",
  remember: true,
});

// 验证码登录表单
const smsForm = reactive({
  phone: "",
  tenantId: "",
  captcha: "",
});

// 验证码按钮状态
const captchaDisabled = ref(false);
const captchaText = ref("获取验证码");

// 获取验证码
const getCaptcha = () => {
  if (!smsForm.phone) {
    message.warning("请输入手机号");
    return;
  }
  if (!smsForm.tenantId) {
    message.warning("请输入租户id");
    return;
  }
  // 验证手机号格式
  const phoneRegex = /^1[3-9]\d{9}$/;
  if (!phoneRegex.test(smsForm.phone)) {
    message.warning("请输入正确的手机号");
    return;
  }

  // 模拟发送验证码
  captchaDisabled.value = true;
  let count = 60;
  captchaText.value = `${count}秒后重试`;

  const timer = setInterval(() => {
    count--;
    if (count <= 0) {
      clearInterval(timer);
      captchaDisabled.value = false;
      captchaText.value = "获取验证码";
    } else {
      captchaText.value = `${count}秒后重试`;
    }
  }, 1000);

  message.success("验证码已发送，请注意查收");
};

// 密码登录
const handlePasswordLogin = (values) => { 
  loading.value = true;
  try{ 
    login(values.username, values.password, values.tenantId).then((response) => {
    console.log(response); 
  if(response.code==200){  
      if(response.isSuccess){
        console.log(response);
        localStorage.setItem('token',response.data);
        getTokenUser().then((response) => 
        {
            if (response.code == 200) {
               localStorage.setItem('user',JSON.stringify(response.data));
               console.warn(response.data);
            }
        });
         getUserPermissions().then((response) => 
        {
            if (response.code == 200) {
               localStorage.setItem('UserPermissions',JSON.stringify(response.data));
               console.warn(response.data);
            }
        });
      }
      setTimeout(() => {
          loading.value = false;  
           message.success("登录成功！");
           //跳转登录页
           router.push('/home');
      }, 1000);  
  }else{ 
     setTimeout(() => {
        loading.value = false;
      }, 1000);
  } 
  });
  } catch (error) {  
    loading.value = false;
  }finally {
    loading.value = false;
  }
  
};

// 验证码登录
const handleSmsLogin = (values) => {
  console.log("验证码登录:", values);
  loading.value = true;

  // 模拟登录请求
  setTimeout(() => {
    loading.value = false;
    message.success("登录成功！");
    // 这里可以跳转到主页
  }, 1500);
};

// 初始化背景动画
onMounted(() => {
  initParticles();
});

onBeforeUnmount(() => {
  // 清理动画
});

// 初始化粒子效果
const initParticles = () => {
  const particlesContainer = document.querySelector(".particles");
  if (!particlesContainer) return;

  // 创建粒子
  for (let i = 0; i < 50; i++) {
    const particle = document.createElement("div");
    particle.className = "particle";
    particle.style.left = `${Math.random() * 100}%`;
    particle.style.top = `${Math.random() * 100}%`;
    particle.style.animationDelay = `${Math.random() * 5}s`;
    particlesContainer.appendChild(particle);
  }
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  position: relative;
  overflow: hidden;
  background: linear-gradient(135deg, #000000, #000000, #001529) !important;
} 
/* 背景层 */
.background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

/* 粒子效果 */
.particles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  animation: float 6s infinite ease-in-out;
  box-shadow: 0 0 10px rgba(255, 255, 255, 0.8);
}

@keyframes float {
  0%,
  100% {
    transform: translate(0, 0);
    opacity: 0.6;
  }
  50% {
    transform: translate(20px, 20px);
    opacity: 1;
  }
}

/* 网格线 */
.grid-lines {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: linear-gradient(rgba(102, 126, 234, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(102, 126, 234, 0.1) 1px, transparent 1px);
  background-size: 30px 30px;
  animation: grid-move 20s linear infinite;
}

@keyframes grid-move {
  0% {
    transform: translate(0, 0);
  }
  100% {
    transform: translate(30px, 30px);
  }
}

/* 悬浮元素 */
.floating-elements {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.element {
  position: absolute;
  border: 1px solid rgba(102, 126, 234, 0.3);
  border-radius: 50%;
  animation: float-element 15s infinite linear;
}

.element-1 {
  width: 80px;
  height: 80px;
  top: 10%;
  left: 5%;
  animation-duration: 20s;
}

.element-2 {
  width: 120px;
  height: 120px;
  bottom: 15%;
  right: 10%;
  animation-duration: 25s;
  animation-direction: reverse;
}

.element-3 {
  width: 60px;
  height: 60px;
  top: 40%;
  right: 20%;
  animation-duration: 18s;
}

.element-4 {
  width: 100px;
  height: 100px;
  bottom: 20%;
  left: 15%;
  animation-duration: 22s;
  animation-direction: reverse;
}

@keyframes float-element {
  0% {
    transform: translate(0, 0) rotate(0deg);
  }
  25% {
    transform: translate(20px, 30px) rotate(90deg);
  }
  50% {
    transform: translate(0, 60px) rotate(180deg);
  }
  75% {
    transform: translate(-20px, 30px) rotate(270deg);
  }
  100% {
    transform: translate(0, 0) rotate(360deg);
  }
}

/* 登录卡片 */
.login-card {
  width: 100%;
  max-width: 420px;
  padding: 40px 30px;
  background: rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(31, 38, 135, 0.37);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.18);
  z-index: 10;
  position: relative;
  overflow: hidden;
}

.login-card::before {
  content: "";
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, rgba(102, 126, 234, 0.1), transparent);
  transform: rotate(45deg);
  animation: shine 6s infinite;
}

@keyframes shine {
  0% {
    transform: rotate(45deg) translateX(-100%);
  }
  100% {
    transform: rotate(45deg) translateX(100%);
  }
}

/* 头部样式 */
.login-header {
  text-align: center;
  margin-bottom: 30px;
  position: relative;
  z-index: 2;
}

.logo {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.logo-image {
  width: 80px;
  height: 80px;
  animation: pulse 3s infinite;
  object-fit: contain;
  filter: drop-shadow(0 0 8px rgba(102, 126, 234, 0.7));
}

@keyframes pulse {
  0%,
  100% {
    transform: scale(1);
    filter: drop-shadow(0 0 8px rgba(102, 126, 234, 0.7));
  }
  50% {
    transform: scale(1.1);
    filter: drop-shadow(0 0 15px rgba(102, 126, 234, 0.9));
  }
}

.title {
  font-size: 26px;
  font-weight: 700;
  color: #fff;
  margin-bottom: 10px;
  text-shadow: 0 0 10px rgba(102, 126, 234, 0.5);
}

.subtitle {
  font-size: 15px;
  color: rgba(255, 255, 255, 0.7);
}

/* Tabs样式 */
.login-tabs {
  margin-top: 20px;
}

:deep(.ant-tabs-tab) {
  font-size: 16px;
  padding: 12px 0;
  color: rgba(255, 255, 255, 0.7);
}

:deep(.ant-tabs-tab:hover) {
  color: #fff;
}

:deep(.ant-tabs-tab-active) {
  color: #fff;
  font-weight: 600;
}

:deep(.ant-tabs-ink-bar) {
  background: linear-gradient(90deg, #667eea, #764ba2);
  height: 3px;
}

:deep(.ant-tabs-nav::before) {
  border-color: rgba(255, 255, 255, 0.2);
}

/* 自定义输入框 */
.custom-input {
  background: rgba(255, 255, 255, 0.1) !important;;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;;
  border-radius: 10px !important;;
  color: white !important;;
  transition: all 0.3s ease !important;;
} 
:deep(.custom-input:hover) {
  border-color: rgba(102, 126, 234, 0.6);
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
}

:deep(.custom-input:focus) {
  border-color: #667eea;
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
  background: rgba(255, 255, 255, 0.15);
}

:deep(.custom-input .ant-input) {
  background: transparent;
  color: white;
}

:deep(.custom-input .ant-input-prefix) {
  color: rgba(255, 255, 255, 0.7);
}

:deep(.custom-input .ant-input::placeholder) {
  color: rgba(255, 255, 255, 0.5);
}

/* 自定义复选框 */
.custom-checkbox :deep(.ant-checkbox-inner) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.3);
}

.custom-checkbox :deep(.ant-checkbox-checked .ant-checkbox-inner) {
  background-color: #667eea;
  border-color: #667eea;
}

.custom-checkbox :deep(.ant-checkbox-wrapper) {
  color: rgba(255, 250, 250, 0.8);
}

/* 表单选项 */
.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.forgot-link {
  color: #667eea;
  transition: all 0.3s ease;
}

.forgot-link:hover {
  color: #764ba2;
  text-shadow: 0 0 8px rgba(102, 126, 234, 0.6);
}

/* 登录按钮 */
.login-button {
  height: 48px;
  font-size: 16px;
  font-weight: 600;
  background: linear-gradient(90deg, #667eea, #764ba2);
  border: none;
  border-radius: 10px;
  margin-top: 10px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.login-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
}

.login-button:active {
  transform: translateY(0);
}

.login-button::after {
  content: "";
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: linear-gradient(45deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transform: rotate(45deg);
  animation: button-shine 3s infinite;
}

@keyframes button-shine {
  0% {
    transform: rotate(45deg) translateX(-100%);
  }
  100% {
    transform: rotate(45deg) translateX(100%);
  }
}

/* 验证码区域 */
.captcha-wrapper {
  display: flex;
  gap: 10px;
}

.captcha-wrapper :deep(.ant-input) {
  flex: 1;
}

.captcha-button {
  height: 40px;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  transition: all 0.3s ease;
}

.captcha-button:hover:not(:disabled) {
  background: rgba(102, 126, 234, 0.3);
  border-color: rgba(102, 126, 234, 0.6);
}

.captcha-button:disabled {
  color: rgba(255, 255, 255, 0.5);
  background: rgba(255, 255, 255, 0.05);
}

/* 登录页脚 */
.login-footer {
  text-align: center;
  margin-top: 30px;
  color: rgba(255, 255, 255, 0.7);
  position: relative;
  z-index: 2;
}

.register-link {
  color: #667eea;
  font-weight: 600;
  margin-left: 5px;
  transition: all 0.3s ease;
}

.register-link:hover {
  color: #764ba2;
  text-shadow: 0 0 8px rgba(102, 126, 234, 0.6);
}

/* 响应式设计 */
@media (max-width: 480px) {
  .login-card {
    margin: 20px;
    padding: 30px 20px;
  }

  .title {
    font-size: 22px;
  }

  .captcha-wrapper {
    flex-direction: column;
  }

  .captcha-button {
    width: 100%;
  }
}
</style>
