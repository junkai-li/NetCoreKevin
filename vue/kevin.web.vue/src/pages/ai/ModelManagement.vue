<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <DatabaseOutlined class="title-icon" />
            <span>模型配置管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索模型..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddModelModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加模型
            </a-button>
          </div>
        </div>
      </template>

      <div class="cards-container">
        <a-row :gutter="[24, 24]">
          <a-col :xs="24" :sm="12" :md="12" :lg="8" :xl="6" v-for="model in modelList" :key="model.id">
            <a-card class="model-card" hoverable>
              <template #title>
                <div class="card-title">
                  <span class="model-name">{{ model.modelName }}</span>
                </div>
              </template>
              <template #extra>
                <a-dropdown>
                  <a class="ant-dropdown-link" @click.prevent>
                    <EllipsisOutlined />
                  </a>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item @click="showEditModelModal(model)">
                        <edit-outlined /> 编辑
                      </a-menu-item>
                      <a-menu-item @click="showDeleteConfirm(model)">
                        <delete-outlined /> 删除
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </template>
              <div @click="showEditModelModal(model)" class="card-content">
                <div class="model-info horizontal-layout">
                  <div class="info-item horizontal">
                    <span class="info-label">AI类型:</span>
                    <span class="info-value">{{ getAITypeName(model.aiType) }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">模型类型:</span>
                    <span class="info-value">{{ getModelTypeName(model.aiModelType) }}</span>
                  </div>
                </div>
                <div class="model-info horizontal-layout">
                  <div class="info-item horizontal">
                    <span class="info-label">模型地址:</span>
                    <span class="info-value url-value">{{ model.endPoint }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">部署名:</span>
                    <span class="info-value">{{ model.modelDescription }}</span>
                  </div>
                </div>
              </div>
            </a-card>
          </a-col>
        </a-row>
        
        <a-empty v-if="modelList.length === 0" description="暂无模型配置数据" />
        
        <div class="pagination-container" v-if="modelList.length > 0">
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

    <!-- 添加/编辑模型配置模态框 -->
    <a-modal 
      v-model:open="modelModalVisible" 
      :title="modelModalTitle" 
      @ok="handleModelModalOk" 
      @cancel="handleModelModalCancel"
      :confirm-loading="confirmLoading"
      width="600px"
    >
      <a-form :model="modelForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }"> 
       <a-form-item label="模型类型" v-bind="validateInfos.aiModelType">
              <a-select v-model:value="modelForm.aiModelType" placeholder="请选择模型类型">
                <a-select-option :value="1">Chat</a-select-option>
                <a-select-option :value="2">Embedding</a-select-option>
              </a-select>
            </a-form-item>
              <a-form-item label="AI类型" v-bind="validateInfos.aiType">
              <a-select v-model:value="modelForm.aiType" placeholder="请选择AI类型">
                <a-select-option :value="1">OpenAI</a-select-option>
                <a-select-option :value="2">Azure</a-select-option>
                <a-select-option :value="3">智谱AI</a-select-option>
                <a-select-option :value="7">Bge Embedding</a-select-option>
                <a-select-option :value="8">Bge Rerank</a-select-option>
                <a-select-option :value="10">Ollama</a-select-option>
                <a-select-option :value="11">OllamaEmbedding</a-select-option> 
              </a-select>
            </a-form-item>
         <a-form-item label="模型地址" v-bind="validateInfos.aiType">
          <a-input v-model:value="modelForm.endPoint" placeholder="请输入模型地址" />
        </a-form-item> 
        <a-form-item label="模型名称" v-bind="validateInfos.modelName">
          <a-input v-model:value="modelForm.modelName" placeholder="请输入模型名称" />
        </a-form-item>
        <a-form-item label="模型秘钥">
          <a-input-password v-model:value="modelForm.modelKey" placeholder="请输入模型秘钥" />
        </a-form-item>
        <a-form-item label="部署名" v-bind="validateInfos.modelDescription">
          <a-input v-model:value="modelForm.modelDescription" placeholder="请输入部署名" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>

import '../../css/ModelManagement.css';
import { ref, reactive, onMounted } from 'vue';
import { 
  DatabaseOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined
} from '@ant-design/icons-vue';
import { message, Modal } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { 
  getAIModelsPageData, 
  addEditAIModel, 
  deleteAIModel 
} from '@/api/ai/aiModels';

const useForm = Form.useForm;

// 模型数据
const modelList = ref([]);

// 分页配置
const pagination = reactive({
  current: 1,
  pageSize: 8,
  total: 0
});

// 模态框状态
const modelModalVisible = ref(false);
const confirmLoading = ref(false);
const modelModalTitle = ref('添加模型配置');

// 当前编辑的模型
const currentModel = ref(null);

// 模型表单
const modelForm = reactive({
  id: '',
  aiType: undefined,
  aiModelType: undefined,
  endPoint: '',
  modelName: '',
  modelKey: '',
  modelDescription: ''
});

// 表单验证规则
const modelRules = reactive({
  aiType: [
    { required: true, message: '请选择AI类型' }
  ],
  aiModelType: [
    { required: true, message: '请选择模型类型' }
  ],
  endPoint: [
    { required: true, message: '请输入模型地址' }
  ],
  modelName: [
    { required: true, message: '请输入模型名称' }
  ], 
  modelDescription: [
    { required: true, message: '请输入部署名' }
  ]
});

// 表单验证
const { validate: validateModelForm, validateInfos } = useForm(modelForm, modelRules);

// 搜索关键字
const searchKeyword = ref('');

// 获取AI类型名称
const getAITypeName = (type) => {
  const types = {
    1: 'OpenAI',
    2: 'Azure',
    3:'智谱AI',
    7:'Bge Embedding',
    8:'Bge Rerank',
    10:'Ollama',
    11:'OllamaEmbedding'
  };
  return types[type] || '未知';
};

// 获取模型类型名称
const getModelTypeName = (type) => {
  const types = {
    1: 'Chat',
    2: 'Embedding',
    4:'Rerank'
  };
  return types[type] || '未知';
};

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.current = 1;
  loadModelData();
};

// 加载模型数据
const loadModelData = async () => {
  try {
    const params = {
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
      searchKey: searchKeyword.value
    };

    const response = await getAIModelsPageData(params);
    if (response && response.code === 200 && response.data.data) {
      modelList.value = response.data.data.map(item => ({
        ...item,
        key: item.id
      }));
      pagination.total = response.data.total;
    }
  } catch (error) {
    console.error('加载模型数据失败:', error);
    message.error('加载模型数据失败: ' + (error.message || '未知错误'));
  }
};

// 显示添加模型模态框
const showAddModelModal = () => {
  modelModalTitle.value = '添加模型配置';
  currentModel.value = null;
  // 重置表单
  Object.assign(modelForm, {
    id: '',
    aiType: undefined,
    aiModelType: undefined,
    endPoint: '',
    modelName: '',
    modelKey: '',
    modelDescription: ''
  }); 
  modelModalVisible.value = true;
};

// 显示编辑模型模态框
const showEditModelModal = (record) => {
  modelModalTitle.value = '编辑模型配置';
  currentModel.value = record;
  // 填充表单数据，确保数字类型正确转换
  modelForm.id = record.id || '';
  modelForm.aiType = record.aiType !== undefined ? record.aiType : undefined;
  modelForm.aiModelType = record.aiModelType !== undefined ? record.aiModelType : undefined;
  modelForm.endPoint = record.endPoint || '';
  modelForm.modelName = record.modelName || '';
  modelForm.modelKey = record.modelKey || '';
  modelForm.modelDescription = record.modelDescription || ''; 
  modelModalVisible.value = true;
};

// 删除模型确认
const showDeleteConfirm = (record) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除模型"${record.modelName}"吗？`,
    okText: '确认',
    cancelText: '取消',
    onOk: () => handleDelete(record.id, record.modelName)
  });
};

// 删除模型
const handleDelete = async (id, modelName) => {
  try {
    await deleteAIModel(id);
    message.success(`模型"${modelName}"删除成功`);
    loadModelData(); // 重新加载数据
  } catch (error) {
    console.error('删除模型失败:', error);
    message.error('删除模型失败: ' + (error.message || '未知错误'));
  }
};

// 分页变化处理
const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadModelData();
};

// 模型模态框确认
const handleModelModalOk = () => {
  validateModelForm().then(async () => {
    confirmLoading.value = true;
    try { 
      await addEditAIModel(currentModel.value ? {
        id: modelForm.id,
        aiType: modelForm.aiType,
        aiModelType: modelForm.aiModelType,
        endPoint: modelForm.endPoint,
        modelName: modelForm.modelName,
        modelKey: modelForm.modelKey,
        modelDescription: modelForm.modelDescription
      }:{ 
        aiType: modelForm.aiType,
        aiModelType: modelForm.aiModelType,
        endPoint: modelForm.endPoint,
        modelName: modelForm.modelName,
        modelKey: modelForm.modelKey,
        modelDescription: modelForm.modelDescription
      });
      
      message.success(currentModel.value ? '模型信息更新成功' : '模型信息添加成功');
      
      modelModalVisible.value = false;
      loadModelData(); // 重新加载数据
    } catch (error) {
      console.error('保存模型失败:', error);
      message.error('保存模型失败: ' + (error.message || '未知错误'));
    } finally {
      confirmLoading.value = false;
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 模型模态框取消
const handleModelModalCancel = () => {
  modelModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('模型配置管理页面已加载');
  loadModelData();
});
</script>