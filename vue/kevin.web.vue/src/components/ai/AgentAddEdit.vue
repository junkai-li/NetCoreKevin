<template>
  <a-modal
    v-model:open="visible"
    :title="modalTitle"
    @ok="handleOk"
    @cancel="handleCancel"
    :confirm-loading="confirmLoading"
    width="900px"
    :footer="isViewMode ? null : undefined"
  >
    <a-tabs v-model:activeKey="mainTabKey">
      <a-tab-pane key="info" tab="智能体信息">
        <a-form :model="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="名称" v-bind="validateInfos.name">
            <a-input v-model:value="form.name" placeholder="请输入名称" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="类型" v-bind="validateInfos.type">
            <a-input v-model:value="form.type" placeholder="请输入类型" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
      </a-row> 
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="描述" v-bind="validateInfos.describe">
        <a-textarea v-model:value="form.describe" :rows="2" placeholder="请输入描述" :disabled="isViewMode" />
      </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="提示词" v-bind="validateInfos.aiPromptID">
            <a-select
              v-model:value="form.aiPromptID"
              placeholder="请选择提示词"
              :options="promptOptions"
              allow-clear
              show-search
              optionFilterProp="label"
              :disabled="isViewMode"
            >
              <a-select-option 
                v-for="prompt in promptList" 
                :key="prompt.id" 
                :value="prompt.id"
              >
                {{ prompt.name }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="模型" v-bind="validateInfos.chatModelID">
            <a-select
              v-model:value="form.chatModelID"
              placeholder="请选择会话模型"
              :options="modelOptions"
              allow-clear
              show-search
              optionFilterProp="label"
              :disabled="isViewMode"
            >
              <a-select-option 
                v-for="model in modelList" 
                :key="model.id" 
                :value="model.id"
              >
                {{ model.modelName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="知识库" v-bind="validateInfos.kmsId">
            <a-select
              v-model:value="form.kmsId"
              placeholder="请选择知识库"
              :options="kmsOptions"
              allow-clear
              show-search
              optionFilterProp="label"
              :disabled="isViewMode"
            >
              <a-select-option 
                v-for="kms in kmsList" 
                :key="kms.id" 
                :value="kms.id"
              >
                {{ kms.name }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row> 
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="温度" v-bind="validateInfos.temperature">
            <div class="slider-container">
              <a-slider
                v-model:value="form.temperature"
                :min="0"
                :max="100"
                :step="1"
                :disabled="isViewMode"
              />
              <div class="slider-value">{{ form.temperature }}°</div>
            </div>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="相似度" v-bind="validateInfos.relevance">
            <div class="slider-container">
              <a-slider
                v-model:value="form.relevance"
                :min="0"
                :max="100"
                :step="1"
                :disabled="isViewMode"
              />
              <div class="slider-value">{{ form.relevance }}%</div>
            </div>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="提问Token" v-bind="validateInfos.maxAskPromptSize">
            <a-input-number
              v-model:value="form.maxAskPromptSize"
              :min="0"
              style="width: 100%"
              placeholder="请输入最大Token数"
              :disabled="isViewMode"
            />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="回答Token" v-bind="validateInfos.answerTokens">
            <a-input-number
              v-model:value="form.answerTokens"
              :min="0"
              style="width: 100%"
              placeholder="请输入最大Token数"
              :disabled="isViewMode"
            />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="向量匹配数" v-bind="validateInfos.maxMatchesCount">
            <a-input-number
              v-model:value="form.maxMatchesCount"
              :min="0"
              style="width: 100%"
              placeholder="请输入匹配数量"
              :disabled="isViewMode"
            />
          </a-form-item>
        </a-col> 
          <a-col :span="12">
          <a-form-item label="消息输出类型" v-bind="validateInfos.msgType">
            <a-select v-model:value="form.msgType" placeholder="消息输出类型" :disabled="isViewMode">
                <a-select-option   :value="1">非流式文本</a-select-option>
                <a-select-option  :value="2">流式文本</a-select-option>
              </a-select>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="AI请求日志">
            <a-switch v-model:checked="form.isHttpLog" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="最大重试次数">
            <a-input-number v-model:value="form.maxRetries" :min="0" style="width: 100%" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="对话消息限制">
            <a-input-number v-model:value="form.chatMessageLimit" :min="1" style="width: 100%" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item
            label="内容长度限制"
            extra="超过限制后会进行截断（知识库、互联网搜索、AI工具内容、文件内容等）"
          >
            <a-input-number v-model:value="form.contentLengthLimit" :min="1" style="width: 100%" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="流式AI思考过程">
            <a-switch v-model:checked="form.isThinkingLog" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="流式AI工具调用">
            <a-switch v-model:checked="form.isToolLog" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="安全拦截" extra="开启后会对python脚本和命令进行安全拦截">
            <a-switch v-model:checked="form.isSecurityIntercept" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="请求超时(分钟)">
            <a-input-number v-model:value="form.networkTimeout" :min="1" style="width: 100%" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
            <a-col :span="12">
          <a-form-item label="请求白名单">
             <a-textarea v-model:value="form.authorizedDomains" placeholder="*为所有，逗号分隔多个域名前缀" :rows="3" :maxlength="300" :disabled="isViewMode" />
          </a-form-item>
        </a-col>
      </a-row> 
        </a-form>
      </a-tab-pane>
      <a-tab-pane key="skillTool" tab="技能工具">
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="AI工具">
              <a-switch v-model:checked="form.isAITools" :disabled="isViewMode" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="Skill技能">
              <a-switch v-model:checked="form.isSkill" :disabled="isViewMode" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-tabs v-model:activeKey="skillToolTabKey">
          <a-tab-pane key="tools" tab="Tools工具" :disabled="!form.isAITools">
            <a-table
              :columns="skillToolColumns"
              :data-source="toolsList"
              :row-selection="toolsRowSelection"
              :pagination="false"
              :scroll="{ y: 250 }"
              row-key="id"
              size="small"
            />
          </a-tab-pane>
          <a-tab-pane key="skills" tab="Skills技能" :disabled="!form.isSkill">
            <a-table
              :columns="skillToolColumns"
              :data-source="skillsList"
              :row-selection="skillsRowSelection"
              :pagination="false"
              :scroll="{ y: 250 }"
              row-key="id"
              size="small"
            />
          </a-tab-pane>
        </a-tabs>
      </a-tab-pane>
      <a-tab-pane key="bind" tab="用户角色绑定" v-if="!props.hideBindTab">
        <a-tabs v-model:activeKey="bindTabKey">
          <a-tab-pane key="users" tab="绑定用户">
            <div style="margin-bottom: 8px;">
              <a-input-search
                v-model:value="userSearchValue"
                placeholder="搜索用户名或账号"
                style="width: 250px;"
                @search="handleUserSearch"
                allowClear
              />
            </div>
          <a-table
            :columns="userColumns"
            :data-source="userList"
            :pagination="userPagination"
            :row-selection="userRowSelection"
            :loading="userLoading"
            :scroll="{ y: 300 }"
            row-key="id"
            size="small"
            @change="handleUserTableChange"
          />
        </a-tab-pane>
        <a-tab-pane key="roles" tab="绑定角色">
          <div style="margin-bottom: 8px;">
            <a-input-search
              v-model:value="roleSearchValue"
              placeholder="搜索角色名称"
              style="width: 250px;"
              @search="handleRoleSearch"
              allowClear
            />
          </div>
          <a-table
            :columns="roleColumns"
            :data-source="roleList"
            :pagination="rolePagination"
            :row-selection="roleRowSelection"
            :loading="roleLoading"
            :scroll="{ y: 300 }"
            row-key="id"
            size="small"
            @change="handleRoleTableChange"
          />
        </a-tab-pane>
      </a-tabs>
      <div class="selected-summary" v-if="selectedBindCount > 0">
        已选择 {{ selectedBindCount }} 个用户/角色
      </div>
      </a-tab-pane>
    </a-tabs>
  </a-modal>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, reactive, computed, watch, onMounted } from 'vue';
import { Form } from 'ant-design-vue';
import { getAIModelsALLList } from '@/api/ai/aiModels';
import { getAIPromptsALLList } from '@/api/ai/aiPrompts';
import { getAIKmssList } from '@/api/ai/aikmss';
import { getUserList } from '@/api/userapi';
import { getRolePage } from '@/api/roleapi';
import { GetAllTools, GetAllSkills } from '@/api/ai/aiskilltoolManagement';
const emit = defineEmits(['ok', 'cancel']);

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  agentData: {
    type: Object,
    default: () => ({})
  },
  modalType: {
    type: String,
    default: 'add'
  },
  hideBindTab: {
    type: Boolean,
    default: false
  }
});

const useForm = Form.useForm;

const visible = ref(false);
const confirmLoading = ref(false);

// 表单数据
const form = reactive({
  id: '',
  name: '',
  describe: '',
  icon: 'windows',
  type: '',
  chatModelID: undefined,
  kmsId: undefined,
  embeddingModelID: undefined,
  rerankModelID: '',
  imageModelID: '',
  temperature: 70,
  relevance: 60,
  maxAskPromptSize: 2048,
  answerTokens: 2048,
  maxMatchesCount: 3,
  rerankCount: 20,
  aiPromptID: undefined,
  msgType: 1,
  isAITools: true,
  isSkill: true,
  tools: [],
  skills: [],
  isHttpLog: false,
  maxRetries: 3,
  networkTimeout: 10,
  authorizedDomains: '*',
  bindIds: [],
  chatMessageLimit: 100,
  contentLengthLimit: 30000,
  isThinkingLog: true,
  isToolLog: true,
  isSecurityIntercept: true
});

// 表单验证规则
const rules = reactive({
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
  aiPromptID: [
    { required: true, message: '请选择提示词' }
  ],
  chatModelID: [
    { required: true, message: '请选择会话模型' }
  ] , msgType: [
    { required: true, message: '请选择消息输出类型' }
  ]
});

// 表单验证
const { validate, validateInfos, resetFields } = useForm(form, rules);

// 模型列表
const modelList = ref([]);
const promptList = ref([]);
const kmsList = ref([]); // 知识库列表
const userList = ref([]);
const roleList = ref([]);

// 绑定Tab
const bindTabKey = ref('users');
const mainTabKey = ref('info');
const skillToolTabKey = ref('tools');

// 技能工具表格列
const skillToolColumns = [
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '描述', dataIndex: 'description', key: 'description' }
];

// 工具列表
const toolsList = ref([]);
// 技能列表
const skillsList = ref([]);

// Tools行选择
const toolsRowSelection = computed(() => {
  return {
    selectedRowKeys: form.tools,
    onChange: isViewMode.value ? () => {} : (selectedRowKeys) => {
      form.tools = [...selectedRowKeys];
    },
    preserveSelectedRows: true
  };
});

// Skills行选择
const skillsRowSelection = computed(() => {
  return {
    selectedRowKeys: form.skills,
    onChange: isViewMode.value ? () => {} : (selectedRowKeys) => {
      form.skills = [...selectedRowKeys];
    },
    preserveSelectedRows: true
  };
});

// 用户搜索状态
const userSearchValue = ref('');

// 角色搜索状态
const roleSearchValue = ref('');

// 用户表格列
const userColumns = [
  { title: '用户名', dataIndex: 'name', key: 'name' },
  { title: '账号', dataIndex: 'phone', key: 'phone' }
];

// 角色表格列
const roleColumns = [
  { title: '角色名称', dataIndex: 'name', key: 'name' }
];

// 用户分页
const userPagination = reactive({
  current: 1,
  pageSize: 50,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条`
});

// 角色分页
const rolePagination = reactive({
  current: 1,
  pageSize: 50,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条`
});

const userLoading = ref(false);
const roleLoading = ref(false);

// 用户行选择
const userRowSelection = computed(() => ({
  selectedRowKeys: form.bindIds
    .filter(id => String(id).startsWith('user_'))
    .map(id => String(id).replace('user_', '')),
  onChange: (selectedRowKeys) => {
    const roleIds = form.bindIds.filter(id => String(id).startsWith('role_'));
    form.bindIds = [...roleIds, ...selectedRowKeys.map(id => `user_${id}`)];
  },
  preserveSelectedRows: true
}));

// 角色行选择
const roleRowSelection = computed(() => ({
  selectedRowKeys: form.bindIds
    .filter(id => String(id).startsWith('role_'))
    .map(id => String(id).replace('role_', '')),
  onChange: (selectedRowKeys) => {
    const userIds = form.bindIds.filter(id => String(id).startsWith('user_'));
    form.bindIds = [...userIds, ...selectedRowKeys.map(id => `role_${id}`)];
  },
  preserveSelectedRows: true
}));

// 已选择数量
const selectedBindCount = computed(() => form.bindIds?.length || 0);

// 用户搜索
const handleUserSearch = () => {
  userPagination.current = 1;
  loadUserList();
};

// 角色搜索
const handleRoleSearch = () => {
  rolePagination.current = 1;
  loadRoleList();
};

// 用户表格变化
const handleUserTableChange = (pagination) => {
  userPagination.current = pagination.current;
  userPagination.pageSize = pagination.pageSize;
  loadUserList();
};

// 角色表格变化
const handleRoleTableChange = (pagination) => {
  rolePagination.current = pagination.current;
  rolePagination.pageSize = pagination.pageSize;
  loadRoleList();
};

// 模型选项
const modelOptions = computed(() => {
  return modelList.value.map(model => ({
    label: model.modelName,
    value: model.id
  }));
});

// 知识库选项
const kmsOptions = computed(() => {
  if(!kmsList.value || !Array.isArray(kmsList.value)) {
    return [];
  }
  return kmsList.value.map(kms => ({
    label: kms.name,
    value: kms.id
  }));
});

// 提示词选项
const promptOptions = computed(() => {
  return promptList.value.map(prompt => ({
    label: prompt.name,
    value: prompt.id
  }));
});


// 加载用户列表
const loadUserList = async () => {
  try {
    userLoading.value = true;
    const response = await getUserList({
      pageNum: userPagination.current,
      pageSize: userPagination.pageSize,
      searchKey: userSearchValue.value || undefined
    });
    if (response && response.code === 200 && response.data) {
      userList.value = response.data.data || response.data.list || [];
      userPagination.total = response.data.total || 0;
    }
  } catch (error) {
    console.error('加载用户列表失败:', error);
  } finally {
    userLoading.value = false;
  }
};

// 加载角色列表
const loadRoleList = async () => {
  try {
    roleLoading.value = true;
    const response = await getRolePage({
      pageNum: rolePagination.current,
      pageSize: rolePagination.pageSize,
      searchKey: roleSearchValue.value || undefined
    });
    if (response && response.code === 200) {
      roleList.value = response.data.data || response.data.list || response.data || [];
      rolePagination.total = response.data.total || 0;
    }
  } catch (error) {
    console.error('加载角色列表失败:', error);
  } finally {
    roleLoading.value = false;
  }
};
 
 

// 模态框标题
const modalTitle = computed(() => {
  if (props.modalType === 'view') return '智能体详情';
  return props.modalType === 'edit' ? '编辑智能体' : '添加智能体';
});

const isViewMode = computed(() => props.modalType === 'view');

// 监听open属性变化
watch(() => props.open, (newVal) => {
  visible.value = newVal;
  if (newVal) {
    // 重置表单
    resetFields();

    // 如果是编辑或预览模式，填充表单数据
    if ((props.modalType === 'edit' || props.modalType === 'view') && props.agentData) {
      Object.keys(form).forEach(key => {
        if (props.agentData[key] !== undefined) {
          if (key === 'bindIds') {
            // 跳过 bindIds，单独处理
          } else {
            form[key] = props.agentData[key];
          }
        }
      });

      // 从 AISkillsToolsBindIds 中提取工具和技能的 ID
      const skillsToolsBindIds = props.agentData.aiSkillsToolsBindIds || [];
      const toolIds = toolsList.value.map(t => t.id);
      const skillIds = skillsList.value.map(s => s.id);

      form.tools = skillsToolsBindIds.filter(id => toolIds.includes(id));
      form.skills = skillsToolsBindIds.filter(id => skillIds.includes(id));

      // bindIds 只保留用户和角色
      form.bindIds = props.agentData.bindIds || [];
    } else {
      // 添加模式，设置默认值
      Object.assign(form, {
        id: props.agentData?.id || '',
        name: '',
        describe: '',
        icon: 'windows',
        type: '',
        chatModelID: undefined,
        embeddingModelID: undefined,
        kmsId: undefined,
        rerankModelID: '',
        imageModelID: '',
        temperature: 70,
        relevance: 60,
        maxAskPromptSize: 2048,
        answerTokens: 2048,
        maxMatchesCount: 3,
        rerankCount: 20,
        aiPromptID: undefined,
        msgType: 1,
        isAITools: true,
        isSkill: true,
        tools: [],
        skills: [],
        isHttpLog: false,
        maxRetries: 3,
        networkTimeout: 10,
        authorizedDomains: '*',
        bindIds: [],
        chatMessageLimit: 100,
        contentLengthLimit: 30000,
        isThinkingLog: true,
        isToolLog: true
      });
    }
  }
});

// 监听visible变化
watch(visible, (newVal) => {
  if (!newVal) {
    emit('cancel');
  }
});

// 处理确认
const handleOk = () => {
  validate().then(async () => {
    confirmLoading.value = true;
    try {
      const buildSelectedData = (selectedIds, originalData) => {
        return originalData.map(item => ({
          aiSkillToolManagementId: item.id,
          aiSkillToolManagementName: item.name,
          aiSkillToolManagementDescription: item.description,
          isSelect: selectedIds.includes(item.id)
        }));
      };

      // 构建 bindIds，只包含用户和角色（不包含工具和技能）
      const userRoleBindIds = (form.bindIds || []).filter(id => !String(id).startsWith('tool_') && !String(id).startsWith('skill_'));

      const params = {
        id: form.id,
        name: form.name,
        describe: form.describe,
        icon: form.icon,
        type: form.type,
        chatModelID: form.chatModelID,
        kmsId: form.kmsId,
        embeddingModelID: form.embeddingModelID,
        rerankModelID: form.rerankModelID,
        imageModelID: form.imageModelID,
        temperature: form.temperature,
        relevance: form.relevance,
        maxAskPromptSize: form.maxAskPromptSize,
        answerTokens: form.answerTokens,
        maxMatchesCount: form.maxMatchesCount,
        rerankCount: form.rerankCount,
        aiPromptID: form.aiPromptID,
        msgType: form.msgType,
        isAITools: form.isAITools,
        isSkill: form.isSkill,
        tools: form.isAITools ? buildSelectedData(form.tools || [], toolsList.value) : [],
        skills: form.isSkill ? buildSelectedData(form.skills || [], skillsList.value) : [],
        isHttpLog: form.isHttpLog,
        maxRetries: form.maxRetries,
        networkTimeout: form.networkTimeout,
        authorizedDomains: form.authorizedDomains,
        bindIds: userRoleBindIds,
        chatMessageLimit: form.chatMessageLimit,
        contentLengthLimit: form.contentLengthLimit,
        isThinkingLog: form.isThinkingLog,
        isToolLog: form.isToolLog,
        isSecurityIntercept: form.isSecurityIntercept
      };
      
      emit('ok', params);
    } catch (error) {
      console.error('保存智能体失败:', error);
    } finally {
      confirmLoading.value = false;
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 处理取消
const handleCancel = () => {
  visible.value = false;
};

// 加载模型列表
const loadModelList = async () => {
  try {
    const response = await getAIModelsALLList(1);
    if (response && response.code === 200 && response.data) {
      modelList.value = response.data;
    }
  } catch (error) {
    console.error('加载模型列表失败:', error);
  }
};

// 加载提示词列表
const loadPromptList = async () => {
  try {
    const response = await getAIPromptsALLList();
    if (response && response.code === 200 && response.data) {
      promptList.value = response.data;
    }
  } catch (error) {
    console.error('加载提示词列表失败:', error);
  }
};

// 加载知识库列表
const loadKmsList = async () => {
  try {
    const response = await getAIKmssList({});
    if (response && response.code === 200) {
      if(response.data && Array.isArray(response.data)) {
        kmsList.value = response.data;
      } else if(response.data && response.data.list && Array.isArray(response.data.list)) {
        kmsList.value = response.data.list;
      } else if(Array.isArray(response)) {
        kmsList.value = response;
      } else {
        console.warn('Unexpected response format for getAIKmssList:', response);
        kmsList.value = [];
      }
    } else {
      console.error('Failed to load knowledge base list:', response?.message || 'Unknown error');
      kmsList.value = [];
    }
  } catch (error) {
    console.error('加载知识库列表失败:', error);
    kmsList.value = [];
  }
};

// 加载工具列表
const loadToolsList = async () => {
  try {
    const response = await GetAllTools();
    if (response && response.code === 200 && response.data) {
      toolsList.value = response.data;
    }
  } catch (error) {
    console.error('加载工具列表失败:', error);
  }
};

// 加载技能列表
const loadSkillsList = async () => {
  try {
    const response = await GetAllSkills();
    if (response && response.code === 200 && response.data) {
      skillsList.value = response.data;
    }
  } catch (error) {
    console.error('加载技能列表失败:', error);
  }
};

// 组件挂载时加载数据
onMounted(() => {
  loadModelList();
  loadPromptList();
  loadKmsList();
  loadUserList();
  loadRoleList();
  loadToolsList();
  loadSkillsList();
});

// 暴露方法给父组件
defineExpose({
  resetFields
});
</script>

<style scoped>
.checkbox-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  max-height: 250px;
  overflow-y: auto;
  padding: 4px;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 4px;
}

.checkbox-grid :deep(.ant-checkbox-wrapper) {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 4px;
  padding: 6px 12px;
  margin: 0;
  transition: all 0.3s;
}

.checkbox-grid :deep(.ant-checkbox-wrapper:hover) {
  background: rgba(255, 255, 255, 0.1);
  border-color: #1890ff;
}

.checkbox-grid :deep(.ant-checkbox-wrapper-checked) {
  background: rgba(24, 144, 255, 0.15);
  border-color: #1890ff;
}

.slider-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.slider-value {
  text-align: center;
  font-size: 12px;
  color: rgba(255, 255, 255, 0.75);
}
.selected-summary {
  margin-top: 8px;
  padding: 8px;
  background: #f0f0f0;
  border-radius: 4px;
  font-size: 12px;
  color: #1890ff;
}
</style>