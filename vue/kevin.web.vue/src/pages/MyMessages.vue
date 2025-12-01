<template>
  <div class="user-management-container">
    <a-card class="user-management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <MailOutlined class="title-icon" />
            <span>我的消息</span>
          </div>
        </div>
      </template>

      <div class="toolbar">
        <div class="toolbar-left">
          <!-- 可以添加导出等工具按钮 -->
        </div>
        <div class="toolbar-right">
          <a-input-search
            v-model:value="searchKey"
            placeholder="搜索消息..."
            style="width: 250px; margin-right: 16px"
            @search="onSearch"
          />
        </div>
      </div>

      <div class="table-container">
        <a-tabs v-model:activeKey="activeTab" @change="handleTabChange">
          <a-tab-pane key="1" tab="系统私信">
            <div class="tab-content">
              <message-table 
                message-type="private-system"
                :data-source="privateSystemData" 
                :loading="privateSystemLoading"
                :pagination="privateSystemPagination"
                @change="handlePrivateSystemTableChange"
                @show-detail="showDetail"
              />
            </div>
          </a-tab-pane>
          
          <a-tab-pane key="2" tab="私人私信">
            <div class="tab-content">
              <message-table 
                message-type="private-user"
                :data-source="privateUserData" 
                :loading="privateUserLoading"
                :pagination="privateUserPagination"
                @change="handlePrivateUserTableChange"
                @show-detail="showDetail"
              />
            </div>
          </a-tab-pane>
          
          <a-tab-pane key="3" tab="公司公告">
            <div class="tab-content">
              <message-table 
                message-type="announcement"
                :data-source="announcementData" 
                :loading="announcementLoading"
                :pagination="announcementPagination"
                @change="handleAnnouncementTableChange"
                @show-detail="showDetail"
              />
            </div>
          </a-tab-pane>
          
          <a-tab-pane key="4" tab="系统消息">
            <div class="tab-content">
              <message-table 
                message-type="system"
                :data-source="systemData" 
                :loading="systemLoading"
                :pagination="systemPagination"
                @change="handleSystemTableChange"
                @show-detail="showDetail"
              />
            </div>
          </a-tab-pane>
        </a-tabs>
      </div>
    </a-card>

    <!-- 消息详情模态框 -->
    <a-modal 
      v-model:open="detailModalVisible" 
      :title="detailData.title || '消息详情'" 
      width="600px"
      :footer="null"
      @cancel="detailModalVisible = false"
    >
      <div class="message-detail">
        <a-descriptions :column="1" bordered>
          <a-descriptions-item label="标题">
            {{ detailData.title }}
          </a-descriptions-item>
          <a-descriptions-item :label="activeTab === '3' ? '发布人' : '发送人'">
            {{ activeTab === '3' ? detailData.createUser : detailData.sendUserName }}
          </a-descriptions-item>
          <a-descriptions-item label="时间">
            {{ detailData.createdTime ? formatDate(detailData.createdTime) : '' }}
          </a-descriptions-item>
          <a-descriptions-item label="内容">
            <div class="detail-content" v-html="detailData.messageContent"></div>
          </a-descriptions-item>
        </a-descriptions>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { 
  getPrivateUserSystemPageData,
  getPrivateUserPageData,
  getAnnouncementPageData,
  getSystemPageData
} from '@/api/message.js'
import { MailOutlined } from '@ant-design/icons-vue'
import MessageTable from '@/components/MessageTable.vue'

// 当前激活的标签页
const activeTab = ref('1')

// 搜索关键词
const searchKey = ref("")

// 系统私信相关
const privateSystemData = ref([])
const privateSystemLoading = ref(false)
const privateSystemPagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
})

// 私人私信相关
const privateUserData = ref([])
const privateUserLoading = ref(false)
const privateUserPagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
})

// 公司公告相关
const announcementData = ref([])
const announcementLoading = ref(false)
const announcementPagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
})

// 系统消息相关
const systemData = ref([])
const systemLoading = ref(false)
const systemPagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
})

// 获取系统私信数据
const fetchPrivateSystemData = async () => {
  try {
    privateSystemLoading.value = true
    const params = {
      page: privateSystemPagination.current,
      pageSize: privateSystemPagination.pageSize,
      searchKey:searchKey.value,
      parameter: {
        searchKey: searchKey.value
      }
    }
    
    const response = await getPrivateUserSystemPageData(params)
    if (response.code === 200) {
      privateSystemData.value = response.data.data
      privateSystemPagination.total = response.data.total
      privateSystemPagination.current = response.data.pageNum
      privateSystemPagination.pageSize = response.data.pageSize
    } else {
      message.error(response.data.msg || '获取系统私信数据失败')
    }
  } catch (error) {
    console.error('获取系统私信数据失败:', error)
    message.error('获取系统私信数据失败: ' + (error.message || '未知错误'))
  } finally {
    privateSystemLoading.value = false
  }
}

// 获取私人私信数据
const fetchPrivateUserData = async () => {
  try {
    privateUserLoading.value = true
    const params = {
      page: privateUserPagination.current,
      pageSize: privateUserPagination.pageSize,
      parameter: {
        searchKey: searchKey.value
      }
    }
    
    const response = await getPrivateUserPageData(params)
    if (response.code === 200) {
      privateUserData.value = response.data.data
      privateUserPagination.total = response.data.total
      privateUserPagination.current = response.data.pageNum
      privateUserPagination.pageSize = response.data.pageSize
    } else {
      message.error(response.data.msg || '获取私人私信数据失败')
    }
  } catch (error) {
    console.error('获取私人私信数据失败:', error)
    message.error('获取私人私信数据失败: ' + (error.message || '未知错误'))
  } finally {
    privateUserLoading.value = false
  }
}

// 获取公司公告数据
const fetchAnnouncementData = async () => {
  try {
    announcementLoading.value = true
    const params = {
      page: announcementPagination.current,
      pageSize: announcementPagination.pageSize,
      parameter: {
        searchKey: searchKey.value
      }
    }
    
    const response = await getAnnouncementPageData(params)
    if (response.code === 200) {
      announcementData.value = response.data.data
      announcementPagination.total = response.data.total
      announcementPagination.current = response.data.pageNum
      announcementPagination.pageSize = response.data.pageSize
    } else {
      message.error(response.data.msg || '获取公司公告数据失败')
    }
  } catch (error) {
    console.error('获取公司公告数据失败:', error)
    message.error('获取公司公告数据失败: ' + (error.message || '未知错误'))
  } finally {
    announcementLoading.value = false
  }
}

// 获取系统消息数据
const fetchSystemData = async () => {
  try {
    systemLoading.value = true
    const params = {
      page: systemPagination.current,
      pageSize: systemPagination.pageSize,
      parameter: {
        searchKey: searchKey.value
      }
    }
    
    const response = await getSystemPageData(params)
    if (response.code === 200) {
      systemData.value = response.data.data
      systemPagination.total = response.data.total
      systemPagination.current = response.data.pageNum
      systemPagination.pageSize = response.data.pageSize
    } else {
      message.error(response.data.msg || '获取系统消息数据失败')
    }
  } catch (error) {
    console.error('获取系统消息数据失败:', error)
    message.error('获取系统消息数据失败: ' + (error.message || '未知错误'))
  } finally {
    systemLoading.value = false
  }
}

// 处理标签页切换
const handleTabChange = (key) => {
  switch (key) {
    case '1':
      if (!privateSystemData.value || privateSystemData.value.length === 0) {
        fetchPrivateSystemData()
      }
      break
    case '2':
      if (!privateUserData.value || privateUserData.value.length === 0) {
        fetchPrivateUserData()
      }
      break
    case '3':
      if (!announcementData.value || announcementData.value.length === 0) {
        fetchAnnouncementData()
      }
      break
    case '4':
      if (!systemData.value || systemData.value.length === 0) {
        fetchSystemData()
      }
      break
  }
}

// 处理系统私信表格分页变化
const handlePrivateSystemTableChange = (pager) => {
  privateSystemPagination.current = pager.current
  privateSystemPagination.pageSize = pager.pageSize
  fetchPrivateSystemData()
}

// 处理私人私信表格分页变化
const handlePrivateUserTableChange = (pager) => {
  privateUserPagination.current = pager.current
  privateUserPagination.pageSize = pager.pageSize
  fetchPrivateUserData()
}

// 处理公司公告表格分页变化
const handleAnnouncementTableChange = (pager) => {
  announcementPagination.current = pager.current
  announcementPagination.pageSize = pager.pageSize
  fetchAnnouncementData()
}

// 处理系统消息表格分页变化
const handleSystemTableChange = (pager) => {
  systemPagination.current = pager.current
  systemPagination.pageSize = pager.pageSize
  fetchSystemData()
}

// 显示详情
const showDetail = (record) => {
  detailData.value = record
  detailModalVisible.value = true
}

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
}

// 搜索处理
const onSearch = (value) => {
  searchKey.value = value;
  // 根据当前激活的标签页刷新对应的数据
  switch (activeTab.value) {
    case '1':
      privateSystemPagination.current = 1;
      if (!privateSystemData.value || privateSystemData.value.length === 0) {
        fetchPrivateSystemData();
      } else {
        fetchPrivateSystemData();
      }
      break;
    case '2':
      privateUserPagination.current = 1;
      if (!privateUserData.value || privateUserData.value.length === 0) {
        fetchPrivateUserData();
      } else {
        fetchPrivateUserData();
      }
      break;
    case '3':
      announcementPagination.current = 1;
      if (!announcementData.value || announcementData.value.length === 0) {
        fetchAnnouncementData();
      } else {
        fetchAnnouncementData();
      }
      break;
    case '4':
      systemPagination.current = 1;
      if (!systemData.value || systemData.value.length === 0) {
        fetchSystemData();
      } else {
        fetchSystemData();
      }
      break;
  }
}

// // 重置搜索
// const resetSearch = () => {
//   searchKey.value = '';
//   onSearch('');
// }

// 模态框相关
const detailModalVisible = ref(false)
const detailData = ref({})

// 组件挂载时获取初始数据
onMounted(() => {
  fetchPrivateSystemData()
})
</script>

<style scoped>
@import "../css/MyTable.css";
@import "../css/UserList.css";  
@import "../css/management.css";

.user-management-container {
  color: white;
  height: 100%;
}

:deep(.user-management-card) {
  background: rgba(255, 255, 255, 0.08);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  overflow: hidden;
  backdrop-filter: blur(10px);
  height: 100%;
  display: flex;
  flex-direction: column;
}

:deep(.user-management-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.user-management-card .ant-card-head-title) {
  color: white;
  padding: 16px 0;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding: 12px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.toolbar-left,
.toolbar-right {
  display: flex;
  gap: 8px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.header-title {
  display: flex;
  align-items: center;
  font-size: 18px;
  font-weight: 500;
}

.title-icon {
  margin-right: 8px;
  color: #667eea;
}

.tab-content {
  padding: 16px 0;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.unread {
  font-weight: bold;
}

.message-detail {
  padding: 16px 0;
}

.detail-content {
  white-space: pre-wrap;
  line-height: 1.6;
}

/* 自定义 tabs 样式 */
:deep(.ant-tabs-tab) {
  color: white;
}

:deep(.ant-tabs-tab:hover) {
  color: #667eea;
}

:deep(.ant-tabs-tab-active) {
  color: #667eea;
}

:deep(.ant-tabs-ink-bar) {
  background: #667eea;
}
</style>