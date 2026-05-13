<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <div class="logo">
          <img :src="logoImage" alt="Logo" class="logo-image" />
        </div>
        <h1 class="title">AI智能体后台管理系统</h1>
        <p class="subtitle">欢迎登录AI智能体开源框架系统后台</p>
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
                placeholder="租户 ID"
                class="custom-input"
              >
                <template #prefix>
                  <HomeOutlined />
                </template>
              </a-input>
            </a-form-item>
            <a-form-item
              name="remember"
            >
            <div class="form-options">
              <a-checkbox v-model:checked="passwordForm.remember">记住我</a-checkbox>
              <a href="#" class="forgot-link">忘记密码？</a>
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
                placeholder="租户 ID"
                class="custom-input"
              >
                <template #prefix>
                  <HomeOutlined />
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
import { ref, reactive, defineOptions, onMounted } from "vue";
import { useRouter } from "vue-router"; 
import {
  UserOutlined,
  LockOutlined,
  PhoneOutlined,
  MailOutlined,
  HomeOutlined,
} from "@ant-design/icons-vue";
import { message } from "ant-design-vue";
import { login,getTokenUser } from "../api/userapi";
import { getUserPermissions } from "../api/baseapi";
import logoImage from '../assets/logo.png'; // 导入logo图片
import "../css/kevinLogin.css";

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
    login(values.username, values.password, values.tenantId)
      .then((response) => {
        console.log(response); 
        if(response.code==200){  
          if(response.isSuccess){
            console.log(response);
            localStorage.setItem('token',response.data);
            getTokenUser().then((response) => 
            {
                if (response.code == 200) {
                   localStorage.setItem('user',JSON.stringify(response.data)); 
                }
            });
             getUserPermissions().then((response) => 
            {
                if (response.code == 200) {
                   localStorage.setItem('UserPermissions',JSON.stringify(response.data)); 
                }
            });
             // 保存登录信息（如果选择了记住我）
            if (values.remember) {
              localStorage.setItem('savedLoginInfo', JSON.stringify({
                username: values.username,
                password: values.password,
                tenantId: values.tenantId,
                remember: values.remember
              }));
            } else {
              // 如果没有选择记住我，清除之前保存的信息
              localStorage.removeItem('savedLoginInfo');
            }
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
      })
      .catch((error) => {
        console.error('Login request failed:', error);
        message.error('登录请求失败，请稍后重试');
        setTimeout(() => {
          loading.value = false;
        }, 1000);
      });
  } catch (error) {  
    console.error('Unexpected error:', error);
    message.error('发生意外错误，请稍后重试');
     setTimeout(() => {
          loading.value = false;
        }, 1000);
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

// 从本地存储加载保存的登录信息
const loadSavedLoginInfo = () => {
  const savedInfo = localStorage.getItem('savedLoginInfo');
  if (savedInfo) {
    try {
      const info = JSON.parse(savedInfo);
      if (info.username && info.password && info.tenantId) {
        passwordForm.username = info.username;
        passwordForm.password = info.password;
        passwordForm.tenantId = info.tenantId;
        passwordForm.remember = info.remember !== false;
        return;
      }
    } catch (error) {
      console.error('Failed to parse saved login info:', error);
    }
  }else{ 
      passwordForm.username = 'admin';
  passwordForm.password = '123456';
  passwordForm.tenantId = '1000';
  passwordForm.remember = true;
  } 
};

onMounted(() => {
  loadSavedLoginInfo();
});
</script>

