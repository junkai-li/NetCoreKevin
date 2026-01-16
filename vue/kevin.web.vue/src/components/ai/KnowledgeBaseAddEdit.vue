<template>
  <a-modal 
    v-model:open="visible"
    :title="modalTitle"
    @ok="handleOk"
    @cancel="handleCancel"
    :confirm-loading="confirmLoading"
    width="900px"
  >
    <a-form :model="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="ID">
            <a-input v-model:value="form.id" placeholder="知识库ID" disabled />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="名称" v-bind="validateInfos.name">
            <a-input v-model:value="form.name" placeholder="请输入知识库名称" />
          </a-form-item>
        </a-col>
      </a-row>
      
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="段落的最大数" v-bind="validateInfos.maxTokensPerParagraph">
            <a-input-number 
              v-model:value="form.maxTokensPerParagraph" 
              :min="1" 
              :max="10000"
              style="width: 100%"
              placeholder="段落的最大数"
            />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="行最大数" v-bind="validateInfos.maxTokensPerLine">
            <a-input-number 
              v-model:value="form.maxTokensPerLine" 
              :min="1" 
              :max="1000"
              style="width: 100%"
              placeholder="每行的最大Token数"
            />
          </a-form-item>
        </a-col>
      </a-row>
      
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="段落重叠数" v-bind="validateInfos.overlappingTokens">
            <a-input-number 
              v-model:value="form.overlappingTokens" 
              :min="0" 
              :max="1000"
              style="width: 100%"
              placeholder="段落之间重叠的Token数"
            />
          </a-form-item>
        </a-col>
      </a-row>
      
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="知识库文档">
            <a-upload
              v-model:file-list="fileList"
              name="file"
              :multiple="true"
              :before-upload="beforeUpload"
              :remove="handleRemoveFile"
            >
              <a-button>
                <UploadOutlined></UploadOutlined>
                上传文档
              </a-button>
            </a-upload>
          </a-form-item>
        </a-col>
          <a-col :span="12">  </a-col>
      </a-row>
      
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="直接添加">
            <a-button @click="showAddContentModal" style="margin-right: 8px;">
              <EditOutlined />
              添加文本内容
            </a-button>
            <a-button @click="showAddUrlModal">
              <LinkOutlined />
              添加远程地址
            </a-button>
          </a-form-item>
        </a-col>
              <a-col :span="12">  </a-col>
      </a-row>
      
      <a-table 
        :columns="detailColumns" 
        :data-source="form.kmsDetailsList" 
        :pagination="false"
        row-key="id"
      >
        <template #bodyCell="{ column, record, index }">
          <template v-if="column.key === 'fileName'">
            <template v-if="record.file">
              {{ record.file.fileName }}
            </template>
            <template v-else-if="record.url">
              {{ record.url }}
            </template>
            <template v-else>
              {{ record.content.substring(0, 30) + '...' }}
            </template>
          </template>
          <template v-else-if="column.key === 'fileType'">
            {{ record.fileType || 'Text' }}
          </template>
          <template v-else-if="column.key === 'status'">
            <a-tag :color="getStatusColor(record.status)">
              {{ getStatusText(record.status) }}
            </a-tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <a-button type="link" @click="viewDetail(record)">查看</a-button>
            <a-popconfirm 
              v-if="form.kmsDetailsList.length > 1" 
              title="确定要删除这条记录吗?" 
              @confirm="onDeleteDetail(index)"
            >
              <a-button type="link" danger>删除</a-button>
            </a-popconfirm>
          </template>
        </template>
      </a-table>
    </a-form>
    
    <!-- 添加文本内容模态框 -->
    <a-modal 
      v-model:open="contentModalVisible" 
      title="添加文本内容" 
      @ok="handleAddContent"
      @cancel="handleCancelContent"
      width="800px"
    >
      <a-form :model="contentForm">
        <a-form-item label="内容类型">
          <a-radio-group v-model:value="contentForm.type">
            <a-radio value="text">纯文本</a-radio>
            <a-radio value="markdown">Markdown</a-radio>
            <a-radio value="html">HTML</a-radio>
          </a-radio-group>
        </a-form-item>
            <a-form-item label="内容标题">
          <a-input 
            v-model:value="contentForm.contentName" 
            placeholder="例如：系统产品手册"
          />
        </a-form-item>
        <a-form-item label="文本内容">
          <a-textarea 
            v-model:value="contentForm.content" 
            :rows="15" 
            placeholder="请输入内容"
            :maxlength="50000"
            show-count
          />
        </a-form-item>
      </a-form>
    </a-modal>
    
    <!-- 添加远程地址模态框 -->
    <a-modal 
      v-model:open="urlModalVisible" 
      title="添加远程地址" 
      @ok="handleAddUrl"
      @cancel="handleCancelUrl"
      width="600px"
    >
      <a-form :model="urlForm">
        <a-form-item label="远程地址">
          <a-input 
            v-model:value="urlForm.url" 
            placeholder="请输入远程地址，例如：https://example.com/data.txt"
          />
        </a-form-item>
        <a-form-item label="内容类型">
          <a-select v-model:value="urlForm.type" placeholder="请选择内容类型">
            <a-select-option value="text">纯文本</a-select-option>
            <a-select-option value="markdown">Markdown</a-select-option>
            <a-select-option value="html">HTML</a-select-option>
            <a-select-option value="pdf">PDF</a-select-option>
            <a-select-option value="word">Word</a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
    </a-modal>
  </a-modal>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, reactive, computed, watch, h } from 'vue';
import { Form, message, Modal } from 'ant-design-vue';
import { UploadOutlined, EditOutlined, LinkOutlined } from '@ant-design/icons-vue';
import { GetSnowflakeId } from '../../api/baseapi';

const emit = defineEmits(['ok', 'cancel']);

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  knowledgeBaseData: {
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
const fileList = ref([]);

// 表单数据
const form = reactive({
  id: null,
  name: '',
  maxTokensPerParagraph: 299,
  maxTokensPerLine: 99,
  overlappingTokens: 49,
  kmsDetailsList: []
});

// 内容添加相关
const contentModalVisible = ref(false);
const contentForm = reactive({
  content: '',
  type: 'text',
  ContentName:'',
});

// URL添加相关
const urlModalVisible = ref(false);
const urlForm = reactive({
  url: '',
  type: 'text'
});

// 表单验证规则
const rules = reactive({
  name: [
    { required: true, message: '请输入知识库名称' }
  ]
});

// 表单验证
const { validate, validateInfos, resetFields } = useForm(form, rules);

// 详情表格列
const detailColumns = [
  {
    title: '文件名',
    key: 'fileName',
    width: '30%'
  },
  {
    title: '文件类型',
    key: 'fileType',
    width: '20%'
  },
  {
    title: '状态',
    key: 'status',
    width: '20%'
  },
  {
    title: '操作',
    key: 'action',
    width: '30%'
  }
];

// 模态框标题
const modalTitle = computed(() => {
  return props.modalType === 'edit' ? '编辑知识库' : '添加知识库';
});

// 监听open属性变化
watch(() => props.open, async (newVal) => {
  visible.value = newVal;
  if (newVal) {
    // 重置表单
    resetFields(); 
    // 如果是编辑模式，填充表单数据
    if (props.modalType === 'edit' && props.knowledgeBaseData) {
      Object.keys(form).forEach(key => {
        if (props.knowledgeBaseData[key] !== undefined) {
          form[key] = props.knowledgeBaseData[key];
        }
      }); 
      // 处理详情列表
      if(props.knowledgeBaseData.aiKmssDetailsList) { 
        form.kmsDetailsList = [...props.knowledgeBaseData.aiKmssDetailsList];
      }
    } else {
      // 添加模式，设置默认值
      // 获取雪花ID并赋值给id字段
      try {
        const snowflakeId = await GetSnowflakeId();
        form.id = snowflakeId.data;
      } catch (error) {
        console.error('获取雪花ID失败:', error);
        form.id = null;
      }
      
      Object.assign(form, {
        name: '',
        maxTokensPerParagraph: 299,
        maxTokensPerLine: 99,
        overlappingTokens: 49,
        kmsDetailsList: []
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
        maxTokensPerParagraph: form.maxTokensPerParagraph,
        maxTokensPerLine: form.maxTokensPerLine,
        overlappingTokens: form.overlappingTokens,
        aIKmssDetailsList: form.kmsDetailsList
      };
      
      emit('ok', params);
    } catch (error) {
      console.error('保存知识库失败:', error);
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

// 上传文件前的处理
const beforeUpload = (file) => {
  // 创建知识库详情对象
  const detailItem = {
    id: Date.now(),
    kmsId: form.id || 0,
    fileId: null,
    file: {
      fileName: file.name,
      size: file.size
    },
    fileType: getFileType(file.name),
    content: "",
    url: "",
    dataCount: null,
    status: 0, // 0表示待处理
    errorMessage: ""
  };
  
  form.kmsDetailsList.push(detailItem);
  return false; // 返回false以阻止自动上传
};

// 显示添加文本内容模态框
const showAddContentModal = () => {
  contentForm.content = '';
  contentForm.type = 'text';
    contentForm.contentName = '';
  contentModalVisible.value = true;
};

// 处理添加文本内容
const handleAddContent = () => {
  if (!contentForm.content.trim()) {
    message.warning('请输入内容');
    return;
  }
   if (!contentForm.contentName.trim()) {
    message.warning('请输入内容标题名称');
    return;
  }
  
  // 创建知识库详情对象
  const detailItem = {
    id: Date.now(),
    kmsId: form.id || 0,
    fileId: null,
    file: null,
    fileType: contentForm.type,
    content: contentForm.content,
    contentName: contentForm.contentName,
    url: "",
    dataCount: null,
    status: 0, // 0表示待处理
    errorMessage: ""
  };
  
  form.kmsDetailsList.push(detailItem);
  contentModalVisible.value = false;
};

// 处理取消添加文本内容
const handleCancelContent = () => {
  contentModalVisible.value = false;
};

// 显示添加远程地址模态框
const showAddUrlModal = () => {
  urlForm.url = '';
  urlForm.type = 'text';
  urlModalVisible.value = true;
};

// 处理添加远程地址
const handleAddUrl = () => {
  if (!urlForm.url.trim()) {
    message.warning('请输入远程地址');
    return;
  }
  
  try {
    new URL(urlForm.url); // 验证URL格式
  } catch (e) {
    message.warning('请输入有效的URL地址');
    return;
  }
  
  // 创建知识库详情对象
  const detailItem = {
    id: Date.now(),
    kmsId: form.id || 0,
    fileId: null,
    file: null,
    fileType: urlForm.type,
    content: "",
    url: urlForm.url,
    dataCount: null,
    status: 0, // 0表示待处理
    errorMessage: ""
  };
  
  form.kmsDetailsList.push(detailItem);
  urlModalVisible.value = false;
};

// 处理取消添加远程地址
const handleCancelUrl = () => {
  urlModalVisible.value = false;
};

// 删除详情
const onDeleteDetail = (index) => {
  form.kmsDetailsList.splice(index, 1);
};

// 查看详情
const viewDetail = (record) => {
  let content;
  
  if (record.file) {
    // 如果是文件类型
    content = h('div', null, [
      h('p', null, ['文件名: ', h('strong', null, record.file.fileName)]),
      h('p', null, ['文件大小: ', h('strong', null, formatFileSize(record.file.size))]),
      h('p', null, ['文件类型: ', h('strong', null, record.fileType)]),
      h('p', null, ['状态: ', h('strong', null, getStatusText(record.status))])
    ]);
  } else if (record.url) {
    // 如果是URL类型
    content = h('div', null, [
      h('p', null, ['URL地址: ', h('strong', null, record.url)]),
      h('p', null, ['内容类型: ', h('strong', null, record.fileType)]),
      h('p', null, ['状态: ', h('strong', null, getStatusText(record.status))])
    ]);
  } else {
    // 如果是文本内容
    content = h('div', null, [
      h('p', null, ['内容标题: ', h('strong', null, record.contentName || '未设置')]),
      h('p', null, ['内容类型: ', h('strong', null, record.fileType)]),
      h('p', null, ['状态: ', h('strong', null, getStatusText(record.status))]),
      h('h4', null, '内容预览:'),
      h('div', { 
        style: 'max-height: 200px; overflow-y: auto; border: 1px solid #ddd; padding: 10px; white-space: pre-wrap;', 
        innerHTML: record.content
      })
    ]);
  }
  
  Modal.info({
    title: '详情信息',
    width: 600,
    content: content,
    okText: '关闭'
  });
};

// 格式化文件大小
const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
};

// 获取文件类型
const getFileType = (fileName) => {
  const ext = fileName.split('.').pop()?.toLowerCase();
  if(['pdf'].includes(ext)) return 'PDF';
  if(['doc', 'docx'].includes(ext)) return 'Word';
  if(['txt'].includes(ext)) return 'Text';
  if(['md'].includes(ext)) return 'Markdown';
  if(['html', 'htm'].includes(ext)) return 'Html';
  return 'Other';
};

// 获取状态颜色
const getStatusColor = (status) => {
  switch(status) {
    case 0: return 'orange'; // 待处理 
    case 1: return 'green'; // 已完成
    case 2: return 'red'; // 失败
    default: return 'default';
  }
};

// 获取状态文本
const getStatusText = (status) => {
  switch(status) {
    case 0: return '待处理'; 
    case 1: return '已完成';
    case 2: return '失败';
    default: return '未知';
  }
};

// 删除上传文件
const handleRemoveFile = (file) => {
  const index = form.kmsDetailsList.findIndex(item => 
    item.file && item.file.fileName === file.name
  );
  if(index !== -1) {
    form.kmsDetailsList.splice(index, 1);
  }
};

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