<template>
  <div class="user-management-container">
    <a-card class="user-management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <BellOutlined class="title-icon" />
            <span>系统公告</span>
          </div>
          <div class="header-actions">
            <a-button type="primary" class="add-button" @click="showAddEditModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加公告
            </a-button>
          </div>
        </div>
      </template>

      <div class="toolbar">
        <div class="toolbar-left">
          <!-- 可以添加导出等工具按钮 -->
        </div>
        <div class="toolbar-right">
          <a-form :model="searchForm" layout="inline" @submit.prevent="handleSearch">
            <a-form-item >
              <a-input v-model:value="searchForm.title" placeholder="请输入标题" style="width: 200px;" />
            </a-form-item>
            <a-form-item>
              <a-button type="primary" html-type="submit">
                <template #icon>
                  <SearchOutlined />
                </template>
                查询
              </a-button>
              <a-button style="margin-left: 8px" @click="resetSearch">
                重置
              </a-button>
            </a-form-item>
          </a-form>
        </div>
      </div>

      <div class="table-container">
        <a-table 
          :columns="columns" 
          :data-source="dataSource" 
          :pagination="pagination" 
          :loading="loading"
          @change="handleTableChange"
          class="my-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'title'">
              {{ record.title }}
            </template>
            
            <template v-else-if="column.dataIndex === 'createUser'">
              {{ record.createUser }}
            </template>
            
            <template v-else-if="column.dataIndex === 'createTime'">
              {{ formatDate(record.createTime) }}
            </template>
            
            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showDetail(record)">
                  <template #icon>
                    <EyeOutlined />
                  </template>
                  查看
                </a-button>
                <a-button type="link" size="small" @click="showAddEditModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-popconfirm
                  title="确定要删除这条公告吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  @confirm="handleDelete(record.id)"
                >
                  <a-button type="link" size="small" danger>
                    <template #icon>
                      <DeleteOutlined />
                    </template>
                    删除
                  </a-button>
                </a-popconfirm>
              </div>
            </template>
          </template>
        </a-table>
      </div>
    </a-card>

    <!-- 新增/编辑公告模态框 -->
    <a-modal 
      v-model:open="modalVisible" 
      :title="modalTitle" 
      @ok="handleModalOk" 
      @cancel="handleModalCancel"
      width="600px"
    >
      <a-form :model="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }" ref="formRef">
        <a-form-item label="标题" name="title" :rules="[{ required: true, message: '请输入标题' }]">
          <a-input v-model:value="form.title" />
        </a-form-item>
        <a-form-item label="内容" name="messageContent" :rules="[{ required: true, message: '请输入内容' }]">
          <a-textarea v-model:value="form.messageContent" :rows="6" />
        </a-form-item>
      </a-form>
    </a-modal>

    <!-- 公告详情模态框 -->
    <a-modal 
      v-model:open="detailModalVisible" 
      title="公告详情" 
      width="600px"
      :footer="null"
      @cancel="detailModalVisible = false"
    >
      <div class="announcement-detail">
        <a-descriptions :column="1" bordered>
          <a-descriptions-item label="标题">
            {{ detailData.title }}
          </a-descriptions-item>
          <a-descriptions-item label="创建人">
            {{ detailData.createUser }}
          </a-descriptions-item>
          <a-descriptions-item label="创建时间"> 
            {{ detailData.createTime}}
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
  getAnnouncementPageData, 
  addEditMessage, 
  deleteMessage 
} from '@/api/message.js'
import { 
  PlusOutlined, 
  SearchOutlined, 
  EyeOutlined, 
  EditOutlined, 
  DeleteOutlined, 
  BellOutlined 
} from '@ant-design/icons-vue'

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
}

// 数据加载状态
const loading = ref(false);

// 表格数据
const dataSource = ref([])

// 表格列定义
const columns = [
  {
    title: '标题',
    dataIndex: 'title',
    key: 'title'
  },{
    title: "内容",
    dataIndex: "messageContent",
    key: "messageContent", 
     ellipsis: true,
     width: 450
  },
  {
    title: '创建人',
    dataIndex: 'createUser',
    key: 'createUser'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '操作',
    dataIndex: 'action',
    key: 'action',
    width: '200px'
  }
]

// 分页配置
const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
})

// 搜索表单
const searchForm = reactive({
  title: ''
})

// 模态框相关
const modalVisible = ref(false)
const detailModalVisible = ref(false)
const modalTitle = ref('')
const isEdit = ref(false)
const currentRecord = ref(null)

// 表单数据
const form = reactive({
  id: '',
  title: '',
  messageContent: '',
  sysMsgType: 1 // 公告类型
})

// 表单引用
const formRef = ref()

// 获取数据
const fetchData = async () => {
  try {
    loading.value = true
    const params = {
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
      parameter: {
        title: searchForm.title
      }
    }
    
    const response = await getAnnouncementPageData(params)
    console.log('获取公告数据成功:', response)
    if (response && response.code == 200 && response.data.data) {
      // 处理返回的数据
      dataSource.value = response.data.data;
   pagination.total = response.data.total
    }
    else {
      message.error(response.data.msg || '获取数据失败')
    }
  } catch (error) {
    console.error('获取公告数据失败:', error)
    message.error('获取公告数据失败: ' + (error.message || '未知错误'))
  } finally {
    loading.value = false
  }
}

// 处理表格分页、排序、筛选变化
const handleTableChange = (pager) => {
  pagination.current = pager.current
  pagination.pageSize = pager.pageSize
  fetchData()
}

// 搜索
const handleSearch = () => {
  pagination.current = 1
  fetchData()
}

// 重置搜索
const resetSearch = () => {
  searchForm.title = ''
  pagination.current = 1
  fetchData()
}

// 显示新增/编辑模态框
const showAddEditModal = (record = null) => {
  isEdit.value = !!record
  modalTitle.value = record ? '编辑公告' : '新增公告'
  currentRecord.value = record
  
  if (record) {
    form.id = record.id
    form.title = record.title
    form.messageContent = record.messageContent
  } else {
    // 重置表单
    form.id = ''
    form.title = ''
    form.messageContent = ''
  }
  
  modalVisible.value = true
}

// 模态框确认
const handleModalOk = async () => {
  try {
    await formRef.value.validateFields()
    
    const params = {
      ...form
    }
    
    const response = await addEditMessage(params)
    if (response.code === 200) {
      message.success(`${isEdit.value ? '编辑' : '新增'}公告成功`)
      modalVisible.value = false
      fetchData()
    } else {
      message.error(response.data.msg || `${isEdit.value ? '编辑' : '新增'}公告失败`)
    }
  } catch (error) {
    console.error(`${isEdit.value ? '编辑' : '新增'}公告失败:`, error)
    message.error(`${isEdit.value ? '编辑' : '新增'}公告失败: ` + (error.message || '未知错误'))
  }
}

// 模态框取消
const handleModalCancel = () => {
  modalVisible.value = false
}

// 显示详情
const showDetail = (record) => {
  detailData.value = record
  detailModalVisible.value = true
}

// 删除公告
const handleDelete = async (id) => {
  try {
    const response = await deleteMessage(id)
    if (response.code === 200) {
      message.success('删除公告成功')
      fetchData()
    } else {
      message.error(response.data.msg || '删除公告失败')
    }
  } catch (error) {
    console.error('删除公告失败:', error)
    message.error('删除公告失败: ' + (error.message || '未知错误'))
  }
}

// 详情数据
const detailData = ref({})

// 组件挂载时获取数据
onMounted(() => {
  fetchData()
})
</script>

<style scoped>
@import "../css/MyTable.css";
@import "../css/UserList.css";  
@import "../css/management.css";

.system-announcement-container {
  color: white;
  height: 100%;
}

.system-announcement-card {
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

:deep(.system-announcement-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.system-announcement-card .ant-card-head-title) {
  color: white;
  padding: 16px 0;
}

.table-container {
  background: transparent;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.announcement-detail {
  padding: 16px 0;
}

.detail-content {
  white-space: pre-wrap;
  line-height: 1.6;
}
</style>