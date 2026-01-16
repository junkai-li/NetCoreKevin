<template>
  <a-modal 
    v-model:open="visible"
    :title="modalTitle"
    @ok="handleOk"
    @cancel="handleCancel"
    :confirm-loading="confirmLoading"
    width="800px"
  >
    <a-form :model="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="名称" v-bind="validateInfos.name">
            <a-input v-model:value="form.name" placeholder="请输入名称" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="类型" v-bind="validateInfos.type">
            <a-input v-model:value="form.type" placeholder="请输入类型" />
          </a-form-item>
        </a-col>
      </a-row> 
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="描述" v-bind="validateInfos.describe">
        <a-textarea v-model:value="form.describe" :rows="2" placeholder="请输入描述" />
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
            />
          </a-form-item>
        </a-col> 
      </a-row>
    </a-form>
  </a-modal>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, reactive, computed, watch, onMounted } from 'vue';
import { Form } from 'ant-design-vue';
import { getAIModelsALLList } from '@/api/ai/aiModels';
import { getAIPromptsALLList } from '@/api/ai/aiPrompts';
import { getAIKmssList } from '@/api/ai/aikmss';
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
    default: 'add' // 'add' 或 'edit'
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
  kmsId: undefined, // 知识库ID
   embeddingModelID: undefined,
  rerankModelID: '',
  imageModelID: '',
  temperature: 70,
  relevance: 60,
  maxAskPromptSize: 2048,
  answerTokens: 2048,
  maxMatchesCount: 3,
  rerankCount: 20,
  aiPromptID: undefined
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
  chatModelID: [
    { required: true, message: '请选择会话模型' }
  ],
  kmsId: [
    { required: true, message: '请选择知识库' }
  ]
});

// 表单验证
const { validate, validateInfos, resetFields } = useForm(form, rules);

// 模型列表
const modelList = ref([]);
const promptList = ref([]);
const kmsList = ref([]); // 知识库列表

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

// 模态框标题
const modalTitle = computed(() => {
  return props.modalType === 'edit' ? '编辑智能体' : '添加智能体';
});

// 监听open属性变化
watch(() => props.open, (newVal) => {
  visible.value = newVal;
  if (newVal) {
    // 重置表单
    resetFields();
    
    // 如果是编辑模式，填充表单数据
    if (props.modalType === 'edit' && props.agentData) {
      Object.keys(form).forEach(key => {
        if (props.agentData[key] !== undefined) {
          form[key] = props.agentData[key];
        }
      });
    } else {
      // 添加模式，设置默认值
      Object.assign(form, {
        id: '',
        name: '',
        describe: '',
        icon: 'windows',
        type: '',
        chatModelID: undefined,
        embeddingModelID: undefined,
         kmsId: undefined, // 知识库ID
        rerankModelID: '',
        imageModelID: '',
        temperature: 70,
        relevance: 60,
        maxAskPromptSize: 2048,
        answerTokens: 2048,
        maxMatchesCount: 3,
        rerankCount: 20,
        aiPromptID: undefined
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
      // 构造参数
      const params = {
        id: form.id,
        name: form.name,
        describe: form.describe,
        icon: form.icon,
        type: form.type,
        chatModelID: form.chatModelID,
        kmsId: form.kmsId, // 知识库ID
         embeddingModelID: form.embeddingModelID,
        rerankModelID: form.rerankModelID,
        imageModelID: form.imageModelID,
        temperature: form.temperature,
        relevance: form.relevance,
        maxAskPromptSize: form.maxAskPromptSize,
        answerTokens: form.answerTokens,
        maxMatchesCount: form.maxMatchesCount,
        rerankCount: form.rerankCount,
        aiPromptID: form.aiPromptID
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
    const response = await getAIModelsALLList();
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
      // 根据API响应调整数据结构，通常POST类型的列表接口会将数据放在data.result或类似属性下
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

// 组件挂载时加载数据
onMounted(() => {
  loadModelList();
  loadPromptList();
  loadKmsList();
});

// 暴露方法给父组件
defineExpose({
  resetFields
});
</script>

<style scoped>
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
</style>