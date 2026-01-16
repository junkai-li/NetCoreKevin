<template>
  <div class="example-container">
    <h2>文件上传组件示例</h2>
    
    <div class="example-section">
      <h3>单文件上传示例</h3>
      <FileUpload
        business="test-business"
        key="test-key"
        sign="test-sign"
        :accept="'.jpg,.jpeg,.png,.pdf,.doc,.docx'"
        :multiple="false"
        @upload-success="handleSingleUploadSuccess"
        @upload-error="handleUploadError"
        @delete-success="handleDeleteSuccess"
        @file-ids-change="handleFileIdsChange"
      />
    </div>
    
    <div class="example-section">
      <h3>多文件上传示例</h3>
      <FileUpload
        business="test-business"
        key="multi-test-key"
        sign="test-sign"
        :accept="'.jpg,.jpeg,.png,.pdf'"
        :multiple="true"
        :max-count="5"
        @upload-success="handleMultipleUploadSuccess"
        @upload-error="handleUploadError"
        @delete-success="handleDeleteSuccess"
        @file-ids-change="handleFileIdsChange"
      />
    </div>
    
    <div class="result-section">
      <h3>上传结果</h3>
      <div v-if="uploadedFileIds.length > 0">
        <p>已上传文件ID列表：</p>
        <ul>
          <li v-for="id in uploadedFileIds" :key="id">{{ id }}</li>
        </ul>
      </div>
      <div v-else>
        <p>暂无上传文件</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import FileUpload from '@/components/FileUpload.vue';

// 存储已上传文件ID
const uploadedFileIds = ref([]);

// 处理单文件上传成功
const handleSingleUploadSuccess = (data) => {
  message.success(`文件 ${data.fileName} 上传成功，ID: ${data.fileId}`);
};

// 处理多文件上传成功
const handleMultipleUploadSuccess = (data) => {
  message.success(`文件 ${data.fileName} 上传成功，ID: ${data.fileId}`);
};

// 处理上传失败
const handleUploadError = (data) => {
  message.error(`文件 ${data.fileName} 上传失败: ${data.error.message}`);
};

// 处理删除成功
const handleDeleteSuccess = (data) => {
  message.success(`文件 ${data.fileName} 删除成功`);
};

// 处理文件ID变更
const handleFileIdsChange = (fileIds) => {
  uploadedFileIds.value = [...fileIds];
  console.log('当前上传的文件ID列表:', fileIds);
};
</script>

<style scoped>
.example-container {
  padding: 20px;
}

.example-section {
  margin-bottom: 30px;
  padding: 20px;
  border: 1px solid #e8e8e8;
  border-radius: 4px;
}

.result-section {
  padding: 20px;
  border: 1px solid #e8e8e8;
  border-radius: 4px;
  background-color: #fafafa;
}
</style>