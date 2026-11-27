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
        <div class="octop-card-grid">
          <div class="octop-card" v-for="kb in knowledgeBaseList" :key="kb.id">
            <div class="octop-card-accent"></div>
            <div class="octop-card-header">
              <div class="octop-card-icon">
                <BookOutlined />
              </div>
              <div class="octop-card-title-block">
                <div class="octop-card-name">{{ kb.name }}</div>
                <div class="octop-card-meta">
                  <a-tag :color="getStatusColor(kb.status)">
                    {{ getStatusText(kb.status) }}
                  </a-tag>
                </div>
              </div>
              <div class="octop-card-tools">
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
                      <a-menu-item @click="openFileDrawer(kb)">
                        <FileAddOutlined /> 查看文档
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </div>
            </div>
            <div class="octop-card-body" @click="openFileDrawer(kb)">
              <div class="octop-card-rows">
                <div class="octop-card-row">
                  <span class="octop-card-row-label">段落Token</span>
                  <span class="octop-card-row-value">{{ kb.maxTokensPerParagraph }}</span>
                </div>
                <div class="octop-card-row">
                  <span class="octop-card-row-label">行Token</span>
                  <span class="octop-card-row-value">{{ kb.maxTokensPerLine }}</span>
                </div>
                <div class="octop-card-row">
                  <span class="octop-card-row-label">重叠Token</span>
                  <span class="octop-card-row-value">{{ kb.overlappingTokens }}</span>
                </div>
                <div class="octop-card-row">
                  <span class="octop-card-row-label">文档数量</span>
                  <span class="octop-card-row-value">{{ kb.documentCount || 0 }}</span>
                </div>
              </div>
            </div>
            <div class="octop-card-footer" v-if="kb.createUser || kb.updateUser">
              <span v-if="kb.createUser">创建人 {{ kb.createUser }}</span>
              <span v-if="kb.updateUser">更新人 {{ kb.updateUser }}</span>
            </div>
          </div>
        </div>
        
        <a-empty v-if="knowledgeBaseList.length === 0" description="暂无知识库数据" />
        
        <div class="pagination-container" v-if="knowledgeBaseList.length > 0">
          <a-pagination
            v-model:current="pagination.current"
            v-model:page-size="pagination.pageSize"
            :total="pagination.total"
            show-size-changer
            :page-size-options="['8', '10', '20', '50', '100']"
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

    <!-- 文档列表抽屉 -->
    <a-drawer
      :open="fileDrawerVisible"
      :title="currentKb ? currentKb.name : '文档列表'"
      width="60%"
      destroy-on-close
      @close="handleDrawerClose"
    >
      <a-spin :spinning="drawerLoading">
        <!-- 抽屉内直接上传文档：与编辑弹窗同口径（全量列表 + 新文件，后端增量合并） -->
        <div class="drawer-upload">
          <FileUpload
            business="AIKmss"
            :keyValue="currentKb ? currentKb.id : null"
            sign="AIKmssDetailFlies"
            :accept="'.txt,.pdf,.md,.docx,.html,.ppt,.pptx,.doc,.xls,.xlsx,.jpg,.jpeg,.png,.gif,.bmp,.webp,.svg'"
            :multiple="true"
            :max-count="30"
            @upload-success="handleDrawerUploadSuccess"
            @upload-error="(e) => message.error('文件上传失败')"
          />
        </div>
        <a-table
          :columns="fileColumns"
          :data-source="fileDetails"
          row-key="id"
          :pagination="false"
          size="middle"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'contentName'">
              {{ record.contentName || '未命名文件' }}
            </template>
            <template v-else-if="column.key === 'fileType'">
              <a-tag>{{ (record.fileType || '未知').toUpperCase() }}</a-tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <a-tooltip v-if="record.status === 2 && record.errorMessage" :title="record.errorMessage">
                <a-tag :color="getDetailStatusColor(record.status)">{{ getDetailStatusText(record.status) }}</a-tag>
              </a-tooltip>
              <a-tag v-else :color="getDetailStatusColor(record.status)">{{ getDetailStatusText(record.status) }}</a-tag>
            </template>
            <template v-else-if="column.key === 'dataCount'">
              {{ record.dataCount || 0 }}
            </template>
            <template v-else-if="column.key === 'action'">
              <a-space>
                <a-button
                  type="link"
                  size="small"
                  :disabled="!record.fileId && !record.url"
                  @click="handlePreviewFile(record)"
                >预览</a-button>
                <a-tooltip
                  v-if="record.status !== 1"
                  title="文件导入完成后才可查看碎片"
                >
                  <a-button type="link" size="small" disabled>碎片</a-button>
                </a-tooltip>
                <a-button
                  v-else
                  type="link"
                  size="small"
                  @click="handleViewChunks(record)"
                >碎片</a-button>
                <a-button
                  type="link"
                  size="small"
                  :disabled="!record.fileId && !record.url"
                  @click="handleDownloadFile(record)"
                >下载</a-button>
              </a-space>
            </template>
          </template>
        </a-table>
      </a-spin>
    </a-drawer>

    <!-- 文件混合预览 -->
    <KnowledgeBaseFilePreview
      :open="filePreviewVisible"
      :file="currentFile"
      @cancel="filePreviewVisible = false"
    />

    <!-- 碎片分页预览 -->
    <KnowledgeBaseChunkPreview
      :open="chunkPreviewVisible"
      :kmsId="chunkTarget.kmsId"
      :sourceFile="chunkTarget.sourceFile"
      :contentName="chunkTarget.contentName"
      @cancel="chunkPreviewVisible = false"
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
  FileAddOutlined,
} from '@ant-design/icons-vue';
import { message, Modal } from 'ant-design-vue';
import {
  getAIKmssPageData,
  addEditAIKmss,
  deleteAIKmss,
  getAIKmssDetails,
} from '@/api/ai/aikmss';
import { getFileById } from '@/api/file';
import KnowledgeBaseAddEdit from '@/components/ai/KnowledgeBaseAddEdit.vue';
import KnowledgeBaseFilePreview from '@/components/ai/KnowledgeBaseFilePreview.vue';
import KnowledgeBaseChunkPreview from '@/components/ai/KnowledgeBaseChunkPreview.vue';
import FileUpload from '@/components/FileUpload.vue';

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

// 文档明细状态（ImportKmsStatus 枚举：0=Loadding 1=Success 2=Fail）
// 注意与上方卡片 kb.status（后端 AIKmssDto.GetStatus 的 4 态计算值）区分，不可混用
const getDetailStatusColor = (status) => {
  switch(status) {
    case 0: return 'orange'; // 导入中
    case 1: return 'green';  // 成功
    case 2: return 'red';    // 失败
    default: return 'default';
  }
};

const getDetailStatusText = (status) => {
  switch(status) {
    case 0: return '导入中';
    case 1: return '成功';
    case 2: return '失败';
    default: return '未知';
  }
};

// ==================== 文档抽屉与预览 ====================

// 文档抽屉
const fileDrawerVisible = ref(false);
const currentKb = ref(null);
const drawerLoading = ref(false);
const fileDetails = ref([]);

// 文件混合预览
const filePreviewVisible = ref(false);
const currentFile = ref({});

// 碎片分页预览
const chunkPreviewVisible = ref(false);
const chunkTarget = ref({ kmsId: null, sourceFile: '', contentName: '' });

// 文档表格列
const fileColumns = [
  { title: '文件名', key: 'contentName', dataIndex: 'contentName' },
  { title: '类型', key: 'fileType', width: 90 },
  { title: '状态', key: 'status', width: 100 },
  { title: '碎片数', key: 'dataCount', width: 90 },
  { title: '操作', key: 'action', width: 180 },
];

// 打开文档抽屉
const openFileDrawer = (kb) => {
  currentKb.value = kb;
  fileDrawerVisible.value = true;
  loadFileDetails(kb);
};

// 加载文档列表（复用 GetDetails）
const loadFileDetails = async (kb) => {
  drawerLoading.value = true;
  try {
    const response = await getAIKmssDetails(kb.id);
    if (response && response.code === 200) {
      const data = response.data || {};
      fileDetails.value = Array.isArray(data.aiKmssDetailsList) ? data.aiKmssDetailsList : [];
    } else {
      fileDetails.value = [];
    }
  } catch (error) {
    console.error('加载文档列表失败:', error);
    message.error('加载文档列表失败: ' + (error.message || '未知错误'));
    fileDetails.value = [];
  } finally {
    drawerLoading.value = false;
  }
};

// 抽屉内上传文档（与编辑弹窗同口径：全量列表 + 新文件，后端增量合并，不误删已有文档）
const drawerUploading = ref(false);
let drawerSubmitChain = Promise.resolve(); // 多文件并发上传时串行提交，避免相互覆盖列表

const getKmsFileType = (fileName) => {
  const ext = (fileName || '').split('.').pop()?.toLowerCase();
  if (['pdf'].includes(ext)) return 'PDF';
  if (['doc', 'docx'].includes(ext)) return 'Word';
  if (['xls', 'xlsx'].includes(ext)) return 'Excel';
  if (['txt'].includes(ext)) return 'Text';
  if (['md'].includes(ext)) return 'Markdown';
  if (['html', 'htm'].includes(ext)) return 'Html';
  if (['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext)) return 'Image';
  return 'Other';
};

const handleDrawerUploadSuccess = (file) => {
  drawerSubmitChain = drawerSubmitChain
    .then(() => submitDrawerFile(file))
    .catch(() => {});
};

const submitDrawerFile = async (file) => {
  const kb = currentKb.value;
  if (!kb) return;
  drawerUploading.value = true;
  try {
    // 每次提交前重新拉取最新全量文档列表（含上一次刚入库的文件，防止误删/重复）
    const response = await getAIKmssDetails(kb.id);
    const existing = (response && response.code === 200 && Array.isArray(response.data?.aiKmssDetailsList))
      ? response.data.aiKmssDetailsList
      : (kb.aiKmssDetailsList || []);
    const details = existing.map(d => ({ ...d }));
    details.push({
      id: Date.now(),
      kmsId: kb.id,
      fileId: file.fileId,
      fileType: getKmsFileType(file.fileName),
      content: '',
      url: '',
      dataCount: null,
      status: 0,
      errorMessage: ''
    });
    await addEditAIKmss({
      id: kb.id,
      name: kb.name,
      maxTokensPerParagraph: kb.maxTokensPerParagraph,
      maxTokensPerLine: kb.maxTokensPerLine,
      aIModelsId: kb.aIModelsId,
      aIRerankModelsId: kb.aIRerankModelsId,
      overlappingTokens: kb.overlappingTokens,
      aIKmssDetailsList: details
    });
    message.success('「' + file.fileName + '」已上传，后台解析入库中');
    await loadFileDetails(kb);
    loadKnowledgeBaseList();
  } catch (error) {
    console.error('抽屉上传导入失败:', error);
    message.error('文档导入失败: ' + (error.message || '未知错误'));
  } finally {
    drawerUploading.value = false;
  }
};

// 关闭抽屉清空临时状态
const handleDrawerClose = () => {
  fileDrawerVisible.value = false;
  currentKb.value = null;
  fileDetails.value = [];
  currentFile.value = {};
  chunkTarget.value = { kmsId: null, sourceFile: '', contentName: '' };
};

// 文件预览
const handlePreviewFile = (record) => {
  currentFile.value = { ...record };
  filePreviewVisible.value = true;
};

// 碎片预览（仅导入成功可用：ImportKmsStatus.Success=1）
const handleViewChunks = (record) => {
  if (record.status !== 1) return;
  chunkTarget.value = {
    kmsId: currentKb.value ? currentKb.value.id : null,
    sourceFile: record.contentName || '',
    contentName: record.contentName || '',
  };
  chunkPreviewVisible.value = true;
};

// 下载原文件
const handleDownloadFile = (record) => {
  if (!record.fileId && !record.url) {
    message.error('文件不存在，无法下载');
    return;
  }
  if (record.fileId) {
    getFileById(record.fileId, record.contentName).catch((error) => {
      console.error('下载失败:', error);
      message.error('下载失败，请稍后重试');
    });
  } else if (record.url) {
    const a = document.createElement('a');
    a.href = record.url;
    a.download = record.contentName || 'downloaded_file';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
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

.drawer-upload {
  margin-bottom: 16px;
  padding-bottom: 8px;
  border-bottom: 1px solid #f0f0f0;
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
  color: var(--accent);
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

/* ===================================================================
   Octop 卡片网格（与 CardTable.css 一致，本页未 import 故 scoped 自带）
   =================================================================== */
.cards-container {
  padding: 20px;
}

.octop-card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  row-gap: 16px;
  column-gap: 12px;
}

.octop-card {
  position: relative;
  background: var(--fn-bg-primary);
  border: 1px solid var(--fn-card-border-normal);
  border-radius: var(--fn-radius-lg);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 196px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  transition:
    box-shadow 0.2s,
    transform 0.2s,
    border-color 0.15s;
}

.octop-card:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  transform: translateY(-1px);
  border-color: color-mix(
    in srgb,
    var(--fn-color-brand) 22%,
    var(--fn-card-border-normal)
  );
}

.octop-card-accent {
  height: 2px;
  background: var(--fn-color-brand);
  flex-shrink: 0;
}

.octop-card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 16px 10px;
  min-width: 0;
}

.octop-card-icon {
  width: 42px;
  height: 42px;
  border-radius: 10px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  font-size: 20px;
  color: var(--fn-color-brand);
  background: var(--fn-color-brand-light);
}

.octop-card-title-block {
  flex: 1;
  min-width: 0;
}

.octop-card-name {
  font-size: 14px;
  font-weight: 600;
  color: var(--fn-text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.3;
}

.octop-card-meta {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 5px;
}

.octop-card-tools {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.octop-card-tools .ant-dropdown-link {
  display: inline-flex;
  align-items: center;
  padding: 4px 6px;
  border-radius: var(--fn-radius-sm);
  color: var(--fn-text-tertiary);
  transition:
    color 0.15s,
    background 0.15s;
}

.octop-card-tools .ant-dropdown-link:hover {
  color: var(--fn-text-primary);
  background: var(--fn-sidebar-item-hover);
}

.octop-card-body {
  flex: 1;
  min-width: 0;
  padding: 0 16px 2px;
  cursor: pointer;
}

.octop-card-rows {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.octop-card-row {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 12px;
}

.octop-card-row-label {
  flex-shrink: 0;
  width: 56px;
  color: var(--fn-text-tertiary);
}

.octop-card-row-value {
  flex: 1;
  min-width: 0;
  color: var(--fn-text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.octop-card-footer {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 16px 14px;
  margin-top: auto;
  font-size: 11px;
  color: var(--fn-text-tertiary);
  white-space: nowrap;
  overflow: hidden;
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