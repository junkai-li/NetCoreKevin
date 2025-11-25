<template>
  <div class="under-development-container">
    <div class="welcome-card">
      <h1>页面正在开发中</h1>
      <p>该功能模块正在紧张开发中，敬请期待！</p>
      
      <div class="progress-section">
        <a-progress :percent="75" status="active" class="progress-bar" />
        <p class="progress-text">当前开发进度: 75%</p>
      </div>
      
      <div class="info-cards">
        <a-card class="info-card" title="即将上线的功能">
          <a-row :gutter="[16, 16]">
            <a-col :span="12">
              <p><CheckCircleOutlined style="color: #52c41a; margin-right: 8px;" />智能体模块</p>
            </a-col>
            <a-col :span="12">
              <p><CheckCircleOutlined style="color: #52c41a; margin-right: 8px;" />提示词模块</p>
            </a-col>
            <a-col :span="12">
              <p><CheckCircleOutlined style="color: #52c41a; margin-right: 8px;" />知识库管理</p>
            </a-col>
            <a-col :span="12">
              <p><CloseCircleOutlined style="color: #ff4d4f; margin-right: 8px;" />AI大模型</p>
            </a-col>
          </a-row>
        </a-card>
        
        <a-card class="info-card" title="联系我们">
          <p>如有紧急需求，请联系作者：</p>
          <p><MailOutlined style="margin-right: 8px;" />18570740519@163.com</p>
          <p><PhoneOutlined style="margin-right: 8px;" />18570740519</p>
        </a-card>
      </div>
      
      <div class="actions">
        <a-button type="primary" @click="goBack">
          <RollbackOutlined />
          返回上一页
        </a-button>
        <a-button style="margin-left: 16px;" @click="goHome">
          <HomeOutlined />
          返回首页
        </a-button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { 
  CheckCircleOutlined, 
  CloseCircleOutlined, 
  MailOutlined, 
  PhoneOutlined, 
  RollbackOutlined,
  HomeOutlined
} from '@ant-design/icons-vue'

const router = useRouter()

const goBack = () => {
  router.go(-1)
}

const goHome = () => {
  router.push('/home')
}
</script>

<style scoped>
.under-development-container {
  padding: 24px;
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
  max-width: 1200px;
  margin: 24px auto;
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

.progress-section {
  margin: 30px 0;
}

.progress-bar {
  max-width: 600px;
  margin: 0 auto;
}

.progress-text {
  margin-top: 10px;
  font-size: 16px;
  color: rgba(255, 255, 255, 0.8);
}

.info-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin: 30px 0;
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

.actions {
  margin-top: 30px;
}
</style>