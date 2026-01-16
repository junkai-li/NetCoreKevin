<template>
  <div class="file-upload-container">
    <a-upload
      v-model:file-list="fileList"
      :custom-request="customRequest"
      :before-upload="beforeUpload"
      :accept="accept"
      :multiple="multiple"
      :max-count="maxCount"
      :disabled="disabled"
      :show-upload-list="showUploadList"
    >
      <a-button :disabled="disabled">
        <UploadOutlined />
        {{ uploadButtonText }}
      </a-button>
    </a-upload>

    <!-- 已上传文件列表 -->
    <div v-if="uploadedFiles.length > 0" class="uploaded-files-list">
      <div
        v-for="file in uploadedFiles"
        :key="file.id"
        class="uploaded-file-item"
      >
        <div class="file-info">
          <PaperClipOutlined class="file-icon" />
          <span class="file-name">{{ file.name }}</span>
          <span class="file-size">({{ formatFileSize(file.size) }})</span>
        </div>
        <div class="file-actions">
          <a-tooltip title="删除文件">
            <a-button
              type="text"
              danger
              @click="handleDeleteFile(file)"
              :disabled="disabled"
            >
              <template #icon><DeleteOutlined /></template>
            </a-button>
          </a-tooltip>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, watch } from 'vue';
import { UploadOutlined, PaperClipOutlined, DeleteOutlined } from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import { uploadFile, deleteFileById } from '@/api/file';

// 定义组件属性
const props = defineProps({
  // 业务领域
  business: {
    type: String,
    required: true
  },
  // 记录值
  keyValue: {
    type: String,
    required: true
  },
  // 自定义标记
  sign: {
    type: String,
    required: true
  },
  // 接受的文件类型
  accept: {
    type: String,
    default: ''
  },
  // 是否允许多文件上传
  multiple: {
    type: Boolean,
    default: false
  },
  // 最大上传数量
  maxCount: {
    type: Number,
    default: undefined
  },
  // 是否禁用
  disabled: {
    type: Boolean,
    default: false
  },
  // 是否显示上传列表
  showUploadList: {
    type: Boolean,
    default: false
  },
  // 上传按钮文字
  uploadButtonText: {
    type: String,
    default: '上传文件'
  }
});

// 定义事件
const emit = defineEmits(['upload-success', 'upload-error', 'delete-success', 'delete-error']);

// 文件列表
const fileList = ref([]);
// 已上传的文件列表
const uploadedFiles = ref([]);

// 自定义上传请求
const customRequest = async (options) => {
  const { file, onSuccess, onError } = options;
  
  try {
    // 上传文件
    const fileId = await uploadFile(props.business, props.keyValue, props.sign, file);
    
    // 添加到已上传文件列表
    uploadedFiles.value.push({
      id: fileId,
      name: file.name,
      size: file.size,
      rawFile: file
    });

    // 触发上传成功的事件
    emit('upload-success', {
      fileId: fileId,
      fileName: file.name,
      fileSize: file.size
    });

    // 如果是单文件上传，清空文件列表
    if (!props.multiple) {
      fileList.value = [];
    }

    onSuccess(fileId);
  } catch (error) {
    console.error('文件上传失败:', error);
    onError(error);
    
    // 触发上传失败的事件
    emit('upload-error', {
      error: error,
      fileName: file.name
    });
  }
};

// 上传前的钩子
const beforeUpload = () => {
  return true; // 允许上传
};

// 删除文件
const handleDeleteFile = async (file) => {
  try {
    const result = await deleteFileById(file.id);
    
    if (result) {
      // 从已上传文件列表中移除
      uploadedFiles.value = uploadedFiles.value.filter(f => f.id !== file.id);
      
      // 触发删除成功的事件
      emit('delete-success', {
        fileId: file.id,
        fileName: file.name
      });
      
      message.success('文件删除成功');
    } else {
      throw new Error('删除失败');
    }
  } catch (error) {
    console.error('文件删除失败:', error);
    
    // 触发删除失败的事件
    emit('delete-error', {
      error: error,
      fileId: file.id,
      fileName: file.name
    });
    
    message.error('文件删除失败: ' + error.message);
  }
};

// 格式化文件大小
const formatFileSize = (size) => {
  if (size === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(size) / Math.log(k));
  return parseFloat((size / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
};

// 监听已上传文件列表的变化，向外暴露文件ID
watch(uploadedFiles, (newFiles) => {
  if (newFiles.length > 0) {
    const fileIds = newFiles.map(file => file.id);
    emit('file-ids-change', fileIds);
  }
}, { deep: true });

// 提供获取已上传文件ID的方法
defineExpose({
  getUploadedFileIds: () => uploadedFiles.value.map(file => file.id),
  getUploadedFiles: () => [...uploadedFiles.value],
  clearUploadedFiles: () => {
    uploadedFiles.value = [];
  }
});
</script>

<style scoped>
.file-upload-container {
  width: 100%;
}

.uploaded-files-list {
  margin-top: 16px;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  padding: 12px;
  max-height: 300px;
  overflow-y: auto;
}

.uploaded-file-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid #f0f0f0;
}

.uploaded-file-item:last-child {
  border-bottom: none;
}

.file-info {
  display: flex;
  align-items: center;
  flex: 1;
}

.file-icon {
  margin-right: 8px;
  color: #1890ff;
}

.file-name {
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-size {
  color: #888;
  margin-left: 8px;
  font-size: 12px;
}

.file-actions {
  display: flex;
  align-items: center;
}
</style>