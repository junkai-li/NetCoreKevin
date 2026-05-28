<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <BookOutlined class="title-icon" />
            <span>知识库管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索知识库..."
              style="width: 250px; margin-right: 16px"
              v-model:value="searchParams.name"
              @search="handleSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddModal">
              <template #icon>
                <PlusOutlined />
              </template>
              新增知识库
            </a-button>
          </div>
        </div>
      </template>

      <div class="cards-container">
        <a-row :gutter="[24, 24]">
          <a-col 
            :xs="24" 
            :sm="12" 
            :md="12" 
            :lg="8" 
            :xl="6" 
            v-for="kb in knowledgeBaseList" 
            :key="kb.id"
          >
            <a-card class="knowledge-base-card" hoverable>
              <template #title>
                <div class="card-title">
                  <span class="kb-name">{{ kb.name }}</span>
                </div>
              </template>
              <template #extra>
                <a-dropdown>
                  <a class="ant-dropdown-link" @click.prevent>
                    <EllipsisOutlined />
                  </a>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item @click="handleEdit(kb)">
                        <EditOutlined /> 编辑
                      </a-menu-item>
                      <a-menu-item @click="handleDelete(kb)">
                        <DeleteOutlined /> 删除
                      </a-menu-item> 
                    </a-menu>
                  </template>
                </a-dropdown>
              </template>
              
              <div @click="handleEdit(kb)" class="card-content">
                <div class="kb-info">
                  <div class="kb-section horizontal">
                    <div class="section-label">段落最大Token数:</div>
                    <div class="section-content">{{ kb.maxTokensPerParagraph }}</div>
                  </div>
                  <div class="kb-section horizontal">
                    <div class="section-label">每行最大Token数:</div>
                    <div class="section-content">{{ kb.maxTokensPerLine }}</div>
                  </div>
                  <div class="kb-section horizontal">
                    <div class="section-label">段落重叠Token数:</div>
                    <div class="section-content">{{ kb.overlappingTokens }}</div>
                  </div>
                  <div class="kb-section horizontal">
                    <div class="section-label">文档数量:</div>
                    <div class="section-content">{{ kb.documentCount || 0 }}</div>
                  </div>
                  <div class="kb-section horizontal">
                    <div class="section-label">状态:</div>
                    <div class="section-content">
                      <a-tag :color="getStatusColor(kb.status)">
                        {{ getStatusText(kb.status) }}
                      </a-tag>
                    </div>
                  </div>
                  <div class="kb-section horizontal" v-if="kb.createUser || kb.updateUser">
                    <div class="section-label" v-if="kb.createUser">创建人:</div>
                    <div class="section-content" v-if="kb.createUser">{{ kb.createUser }}</div>
                    <div class="section-label" v-if="kb.updateUser" style="margin-left: 16px;">更新人:</div>
                    <div class="section-content" v-if="kb.updateUser">{{ kb.updateUser }}</div>
                  </div>
                </div>
              </div>
            </a-card>
          </a-col>
        </a-row>
        
        <a-empty v-if="knowledgeBaseList.length === 0" description="暂无知识库数据" />
        
        <div class="pagination-container" v-if="knowledgeBaseList.length > 0">
          <a-pagination
            v-model:current="pagination.current"
            v-model:page-size="pagination.pageSize"
            :total="pagination.total"
            show-size-changer
            show-quick-jumper
            :show-total="(total) => `共 ${total} 条记录`"
            @change="handlePageChange"
          />
        </div>
      </div>
    </a-card>

    <!-- 知识库添加/编辑模态框 -->
    <KnowledgeBaseAddEdit
      :open="modalVisible"
      :knowledgeBaseData="currentRecord"
      :modalType="modalType"
      @ok="handleModalOk"
      @cancel="handleModalCancel"
    />
 
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { 
  BookOutlined,
  PlusOutlined, 
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined, 
} from '@ant-design/icons-vue';
import { message, Modal } from 'ant-design-vue';
import {  
  getAIKmssPageData, 
  addEditAIKmss, 
  deleteAIKmss 
} from '@/api/ai/aikmss';
import KnowledgeBaseAddEdit from '@/components/ai/KnowledgeBaseAddEdit.vue';

// 搜索参数
const searchParams = reactive({
  name: ''
});

// 分页信息
const pagination = reactive({
  current: 1,
  pageSize: 8, // 与PromptManagement一致
  total: 0
});

// 加载状态
const loading = ref(false);

// 知识库列表
const knowledgeBaseList = ref([]);

// 模态框相关
const modalVisible = ref(false); 
const modalType = ref('add'); // 'add' 或 'edit'
const currentRecord = ref({}); 

// 获取知识库列表
const loadKnowledgeBaseList = async () => {
  loading.value = true;
  try {
    const params = {
      search: searchParams.name,
      page: pagination.current,
      limit: pagination.pageSize
    };
    
    const response = await getAIKmssPageData(params);
    console.log('获取知识库列表成功:', response);
    if (response && response.code === 200 && response.data.data) {
      knowledgeBaseList.value = response.data.data || [];
      pagination.total = response.data.total || 0;
    }
  } catch (error) {
    console.error('加载知识库列表失败:', error);
    message.error('加载知识库列表失败: ' + (error.message || '未知错误'));
  } finally {
    loading.value = false;
  }
};

// 搜索
const handleSearch = () => {
  pagination.current = 1;
  loadKnowledgeBaseList();
};

// 分页变化
const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadKnowledgeBaseList();
};

// 显示添加模态框
const showAddModal = () => {
  modalType.value = 'add';
  currentRecord.value = {};
  modalVisible.value = true;
};

// 显示编辑模态框
const handleEdit = (record) => {
  modalType.value = 'edit';
  currentRecord.value = { ...record };
  modalVisible.value = true;
};

// 删除确认
const handleDelete = (record) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除知识库"${record.name}"吗？`,
    okText: '确认',
    cancelText: '取消',
    onOk: () => deleteKnowledgeBase(record.id, record.name),
  });
};

// 删除知识库
const deleteKnowledgeBase = async (id, name) => {
  try {
    const result = await deleteAIKmss(id);
    if (result) {
      message.success(`知识库"${name}"删除成功`);
      loadKnowledgeBaseList();
    }
  } catch (error) {
    console.error('删除知识库失败:', error);
    message.error('删除知识库失败: ' + (error.message || '未知错误'));
  }
};
 
 

// 处理模态框确定
const handleModalOk = async (data, closeLoadingCallback) => {
  try {
    const result = await addEditAIKmss(data);
    if (result) {
      message.success(
        currentRecord.value && currentRecord.value.id 
          ? "知识库信息更新成功" 
          : "知识库信息添加成功"
      );
      modalVisible.value = false;
      loadKnowledgeBaseList();
    }
  } catch (error) {
    console.error('保存知识库失败:', error);
    message.error('保存知识库失败: ' + (error.message || '未知错误'));
  } finally {
    // 确保无论成功还是失败都调用回调函数来关闭加载状态
    if (typeof closeLoadingCallback === 'function') {
      closeLoadingCallback();
    }
  }
};

// 处理模态框取消
const handleModalCancel = () => {
  modalVisible.value = false;
};
 
 
// 获取状态颜色
const getStatusColor = (status) => {
  switch(status) {
    case 0: return 'orange'; // 待处理
    case 1: return 'blue'; // 处理中
    case 2: return 'green'; // 已完成
    case 3: return 'red'; // 失败
    default: return 'default';
  }
};

// 获取状态文本
const getStatusText = (status) => {
  switch(status) {
    case 0: return '待处理';
    case 1: return '处理中';
    case 2: return '已完成';
    case 3: return '失败';
    default: return '未知';
  }
};

// 初始化
onMounted(() => {
  loadKnowledgeBaseList();
});
</script>

<style scoped>
.management-container {
  padding: 0;
  background-color: transparent;
}

.management-card {
  border-radius: 8px;
  overflow: hidden;
  background: #fff;
  border: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.03);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-title {
  display: flex;
  align-items: center;
  font-size: 16px;
  font-weight: 600;
  color: rgba(0, 0, 0, 0.88);
}

.title-icon {
  margin-right: 8px;
  font-size: 18px;
  color: #1677ff;
}

.header-actions {
  display: flex;
  align-items: center;
}

.search-input {
  margin-right: 16px;
}

:deep(.ant-card-head) {
  background: #fafafa;
  border-bottom: 1px solid #f0f0f0;
}

:deep(.ant-card) {
  background: #fff;
  border: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
}

:deep(.ant-card-hoverable:hover) {
  border-color: #d9d9d9;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.knowledge-base-card {
  min-height: 240px;
  display: flex;
  flex-direction: column;
}

.card-title {
  word-break: break-all;
  font-weight: 600;
}

.kb-name {
  font-weight: 600;
  color: rgba(0, 0, 0, 0.88);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%;
}

.kb-info {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  gap: 8px;
  width: 100%;
  overflow: hidden;
}

.kb-section {
  margin-bottom: 0;
}

.kb-section.horizontal {
  display: flex;
  gap: 12px;
  align-items: flex-start;
  flex-shrink: 0;
}

.section-label {
  font-weight: 500;
  color: rgba(0, 0, 0, 0.55);
  white-space: nowrap;
  text-align: left;
  min-width: 50px;
  font-size: 14px;
}

.section-content {
  color: rgba(0, 0, 0, 0.88);
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  text-align: left;
  font-size: 14px;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  white-space: nowrap;
  margin: 0;
  padding: 0;
}

.card-content {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.pagination-container {
  margin-top: 24px;
  display: flex;
  justify-content: flex-end;
}

:deep(.ant-input),
:deep(.ant-input-number),
:deep(.ant-select-selector),
:deep(.ant-picker),
:deep(.ant-input-search > .ant-input-group > .ant-input-group-addon .ant-input-search-button) {
  background: #fff !important;
  border: 1px solid #d9d9d9 !important;
  color: rgba(0, 0, 0, 0.88) !important;
}

:deep(.ant-form-item-label > label) {
  color: rgba(0, 0, 0, 0.88) !important;
}

:deep(.ant-empty) {
  color: rgba(0, 0, 0, 0.45);
}

:deep(.ant-empty-description) {
  color: rgba(0, 0, 0, 0.45);
}
</style>