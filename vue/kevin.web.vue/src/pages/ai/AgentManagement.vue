<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <RobotOutlined class="title-icon" />
            <span>智能体管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索智能体..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddAgentModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加智能体
            </a-button>
          </div>
        </div>
      </template>

      <div class="cards-container">
        <a-row :gutter="[24, 24]">
          <a-col :xs="24" :sm="12" :md="12" :lg="8" :xl="6" v-for="agent in agentList" :key="agent.id">
            <a-card class="agent-card" hoverable>
              <template #title>
                <div class="card-title">
                  <span class="agent-name">{{ agent.name }}</span>
                </div>
              </template>
              <template #extra>
                <a-dropdown>
                  <a class="ant-dropdown-link" @click.prevent>
                    <EllipsisOutlined />
                  </a>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item @click="showEditAgentModal(agent)">
                        <edit-outlined /> 编辑
                      </a-menu-item>
                      <a-menu-item @click="showDeleteConfirm(agent)">
                        <delete-outlined /> 删除
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </template>
              <div class="card-content">
                <div class="agent-icon">
                  <component :is="agent.icon" class="icon-element" />
                </div>
                <div class="agent-info">
                  <div class="info-item horizontal">
                    <span class="info-label">描述:</span>
                    <span class="info-value">{{ agent.describe }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">类型:</span>
                    <span class="info-value">{{ agent.type }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">温度:</span>
                    <div class="progress-wrapper">
                      <a-progress 
                        :percent="agent.temperature" 
                        :format="percent => `${percent}°`" 
                        size="small" 
                        :stroke-color="getTemperatureColor(agent.temperature)"
                      />
                    </div>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">相似度:</span>
                    <div class="progress-wrapper">
                      <a-progress 
                        :percent="agent.relevance" 
                        :format="percent => `${percent}%`" 
                        size="small" 
                        :stroke-color="getRelevanceColor(agent.relevance)"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </a-card>
          </a-col>
        </a-row>
        
        <a-empty v-if="agentList.length === 0" description="暂无智能体数据" />
        
        <div class="pagination-container" v-if="agentList.length > 0">
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

    <!-- 添加/编辑智能体模态框 -->
    <a-modal 
      v-model:open="agentModalVisible" 
      :title="agentModalTitle" 
      @ok="handleAgentModalOk" 
      @cancel="handleAgentModalCancel"
      :confirm-loading="confirmLoading"
      width="700px"
    >
      <a-form :model="agentForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="名称" v-bind="validateInfos.name">
              <a-input v-model:value="agentForm.name" placeholder="请输入名称" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="类型" v-bind="validateInfos.type">
              <a-input v-model:value="agentForm.type" placeholder="请输入类型" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item label="描述" v-bind="validateInfos.describe">
          <a-textarea v-model:value="agentForm.describe" :rows="2" placeholder="请输入描述" />
        </a-form-item>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="图标" v-bind="validateInfos.icon">
              <a-input v-model:value="agentForm.icon" placeholder="请输入图标" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="提示词绑定" v-bind="validateInfos.aiPromptID">
              <a-input-number 
                v-model:value="agentForm.aiPromptID" 
                :min="0" 
                style="width: 100%"
                placeholder="请输入提示词ID"
              />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="会话模型ID" v-bind="validateInfos.chatModelID">
              <a-input v-model:value="agentForm.chatModelID" placeholder="请输入会话模型ID" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Embedding模型ID" v-bind="validateInfos.embeddingModelID">
              <a-input v-model:value="agentForm.embeddingModelID" placeholder="请输入Embedding模型ID" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Rerank模型ID" v-bind="validateInfos.rerankModelID">
              <a-input v-model:value="agentForm.rerankModelID" placeholder="请输入Rerank模型ID" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="图像模型ID" v-bind="validateInfos.imageModelID">
              <a-input v-model:value="agentForm.imageModelID" placeholder="请输入图像模型ID" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="温度" v-bind="validateInfos.temperature">
              <div class="slider-container">
                <a-slider 
                  v-model:value="agentForm.temperature" 
                  :min="0" 
                  :max="100" 
                  :step="1"
                />
                <div class="slider-value">{{ agentForm.temperature }}°</div>
              </div>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="相似度" v-bind="validateInfos.relevance">
              <div class="slider-container">
                <a-slider 
                  v-model:value="agentForm.relevance" 
                  :min="0" 
                  :max="100" 
                  :step="1"
                />
                <div class="slider-value">{{ agentForm.relevance }}%</div>
              </div>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="提问最大Token" v-bind="validateInfos.maxAskPromptSize">
              <a-input-number 
                v-model:value="agentForm.maxAskPromptSize" 
                :min="0" 
                style="width: 100%"
                placeholder="请输入最大Token数"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="回答最大Token" v-bind="validateInfos.answerTokens">
              <a-input-number 
                v-model:value="agentForm.answerTokens" 
                :min="0" 
                style="width: 100%"
                placeholder="请输入最大Token数"
              />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="向量匹配数" v-bind="validateInfos.maxMatchesCount">
              <a-input-number 
                v-model:value="agentForm.maxMatchesCount" 
                :min="0" 
                style="width: 100%"
                placeholder="请输入匹配数量"
              />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="Rerank数量" v-bind="validateInfos.rerankCount">
              <a-input-number 
                v-model:value="agentForm.rerankCount" 
                :min="0" 
                style="width: 100%"
                placeholder="请输入Rerank数量"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup> 
import '../../css/AgentManagement.css';
import { ref, reactive, onMounted } from 'vue';
import { 
  RobotOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined
} from '@ant-design/icons-vue';
import { message, Modal } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { 
  getAIAppsPageData, 
  addEditAIApp, 
  deleteAIApp 
} from '@/api/ai/aiapps';

const useForm = Form.useForm;

// 智能体数据
const agentList = ref([]);

// 分页配置
const pagination = reactive({
  current: 1,
  pageSize: 8,
  total: 0
});

// 模态框状态
const agentModalVisible = ref(false);
const confirmLoading = ref(false);
const agentModalTitle = ref('添加智能体');

// 当前编辑的智能体
const currentAgent = ref(null);

// 智能体表单
const agentForm = reactive({
  id: '',
  name: '',
  describe: '',
  icon: 'windows',
  type: '',
  chatModelID: '',
  embeddingModelID: '',
  rerankModelID: '',
  imageModelID: '',
  temperature: 70,
  relevance: 60,
  maxAskPromptSize: 2048,
  answerTokens: 2048,
  maxMatchesCount: 3,
  rerankCount: 20,
  aiPromptID: 0
});

// 表单验证规则
const agentRules = reactive({
  name: [
    { required: true, message: '请输入名称' }
  ],
  describe: [
    { required: true, message: '请输入描述' }
  ],
  icon: [
    { required: true, message: '请输入图标' }
  ],
  type: [
    { required: true, message: '请输入类型' }
  ],
  chatModelID: [
    { required: true, message: '请输入会话模型ID' }
  ],
  embeddingModelID: [
    { required: true, message: '请输入Embedding模型ID' }
  ]
});

// 表单验证
const { validate: validateAgentForm, validateInfos } = useForm(agentForm, agentRules);

// 搜索关键字
const searchKeyword = ref('');

// 获取温度颜色
const getTemperatureColor = (temp) => {
  if (temp < 30) return '#667eea';
  if (temp < 70) return '#764ba2';
  return '#f5222d';
};

// 获取相似度颜色
const getRelevanceColor = (relevance) => {
  if (relevance < 30) return '#f5222d';
  if (relevance < 70) return '#764ba2';
  return '#52c41a';
};

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.current = 1;
  loadAgentData();
};

// 加载智能体数据
const loadAgentData = async () => {
  try {
    const params = {
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
      searchKey: searchKeyword.value
    };

    const response = await getAIAppsPageData(params);
    if (response && response.code === 200 && response.data.data) {
      agentList.value = response.data.data.map(item => ({
        ...item,
        key: item.id
      }));
      pagination.total = response.data.total;
    }
  } catch (error) {
    console.error('加载智能体数据失败:', error);
    message.error('加载智能体数据失败: ' + (error.message || '未知错误'));
  }
};

// 显示添加智能体模态框
const showAddAgentModal = () => {
  agentModalTitle.value = '添加智能体';
  currentAgent.value = null;
  // 重置表单
  Object.assign(agentForm, {
    id: '',
    name: '',
    describe: '',
    icon: 'windows',
    type: '',
    chatModelID: '',
    embeddingModelID: '',
    rerankModelID: '',
    imageModelID: '',
    temperature: 70,
    relevance: 60,
    maxAskPromptSize: 2048,
    answerTokens: 2048,
    maxMatchesCount: 3,
    rerankCount: 20,
    aiPromptID: 0
  }); 
  agentModalVisible.value = true;
};

// 显示编辑智能体模态框
const showEditAgentModal = (record) => {
  agentModalTitle.value = '编辑智能体';
  currentAgent.value = record;
  // 填充表单数据
  agentForm.id = record.id || '';
  agentForm.name = record.name || '';
  agentForm.describe = record.describe || '';
  agentForm.icon = record.icon || 'windows';
  agentForm.type = record.type || '';
  agentForm.chatModelID = record.chatModelID || '';
  agentForm.embeddingModelID = record.embeddingModelID || '';
  agentForm.rerankModelID = record.rerankModelID || '';
  agentForm.imageModelID = record.imageModelID || '';
  agentForm.temperature = record.temperature !== undefined ? record.temperature : 70;
  agentForm.relevance = record.relevance !== undefined ? record.relevance : 60;
  agentForm.maxAskPromptSize = record.maxAskPromptSize !== undefined ? record.maxAskPromptSize : 2048;
  agentForm.answerTokens = record.answerTokens !== undefined ? record.answerTokens : 2048;
  agentForm.maxMatchesCount = record.maxMatchesCount !== undefined ? record.maxMatchesCount : 3;
  agentForm.rerankCount = record.rerankCount !== undefined ? record.rerankCount : 20;
  agentForm.aiPromptID = record.aiPromptID !== undefined ? record.aiPromptID : 0; 
  agentModalVisible.value = true;
};

// 删除智能体确认
const showDeleteConfirm = (record) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除智能体"${record.name}"吗？`,
    okText: '确认',
    cancelText: '取消',
    onOk: () => handleDelete(record.id, record.name)
  });
};

// 删除智能体
const handleDelete = async (id, name) => {
  try {
    await deleteAIApp(id);
    message.success(`智能体"${name}"删除成功`);
    loadAgentData(); // 重新加载数据
  } catch (error) {
    console.error('删除智能体失败:', error);
    message.error('删除智能体失败: ' + (error.message || '未知错误'));
  }
};

// 分页变化处理
const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadAgentData();
};

// 智能体模态框确认
const handleAgentModalOk = () => {
  validateAgentForm().then(async () => {
    confirmLoading.value = true;
    try {
      const params = {
        id: agentForm.id,
        name: agentForm.name,
        describe: agentForm.describe,
        icon: agentForm.icon,
        type: agentForm.type,
        chatModelID: agentForm.chatModelID,
        embeddingModelID: agentForm.embeddingModelID,
        rerankModelID: agentForm.rerankModelID,
        imageModelID: agentForm.imageModelID,
        temperature: agentForm.temperature,
        relevance: agentForm.relevance,
        maxAskPromptSize: agentForm.maxAskPromptSize,
        answerTokens: agentForm.answerTokens,
        maxMatchesCount: agentForm.maxMatchesCount,
        rerankCount: agentForm.rerankCount,
        aiPromptID: agentForm.aiPromptID
      };

      await addEditAIApp(params);
      
      message.success(currentAgent.value ? '智能体信息更新成功' : '智能体信息添加成功');
      
      agentModalVisible.value = false;
      loadAgentData(); // 重新加载数据
    } catch (error) {
      console.error('保存智能体失败:', error);
      message.error('保存智能体失败: ' + (error.message || '未知错误'));
    } finally {
      confirmLoading.value = false;
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 智能体模态框取消
const handleAgentModalCancel = () => {
  agentModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('智能体管理页面已加载');
  loadAgentData();
});
</script>