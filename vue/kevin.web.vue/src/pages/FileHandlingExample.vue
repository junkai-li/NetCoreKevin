<template>
  <div class="file-handling-example">
    <a-card title="文件上传示例">
      <a-upload
        :before-upload="beforeUpload"
        :custom-request="handleUpload"
        :show-upload-list="false"
      >
        <a-button>
          <upload-outlined></upload-outlined>
          选择文件上传
        </a-button>
      </a-upload>
      
      <div v-if="uploadProgress > 0" style="margin-top: 16px;">
        <a-progress :percent="uploadProgress" />
      </div>
      
      <div v-if="uploadedFile" style="margin-top: 16px;">
        <p>已上传文件: {{ uploadedFile.name }}</p>
        <a-button @click="handleDownload">下载文件</a-button>
      </div>
    </a-card>

    <a-card title="批量文件上传" style="margin-top: 20px;">
      <a-upload
        :before-upload="beforeUploadMultiple"
        :custom-request="handleUploadMultiple"
        :show-upload-list="false"
        multiple
      >
        <a-button>
          <upload-outlined></upload-outlined>
          选择多个文件上传
        </a-button>
      </a-upload>
      
      <div v-if="uploadProgressMultiple > 0" style="margin-top: 16px;">
        <a-progress :percent="uploadProgressMultiple" />
      </div>
      
      <div v-if="uploadedFiles.length > 0" style="margin-top: 16px;">
        <p>已上传 {{ uploadedFiles.length }} 个文件:</p>
        <ul>
          <li v-for="(file, index) in uploadedFiles" :key="index">
            {{ file.name }}
          </li>
        </ul>
      </div>
    </a-card>

    <a-card title="文件下载示例" style="margin-top: 20px;">
      <a-button @click="downloadSampleFile">
        <download-outlined></download-outlined>
        下载示例文件
      </a-button>
    </a-card>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import { UploadOutlined, DownloadOutlined } from '@ant-design/icons-vue';
import { uploadFile, uploadFiles, downloadFile } from '@/utils/fileUtil';
import myAxios from '@/utils/http';

// 单文件上传相关
const uploadedFile = ref(null);
const uploadProgress = ref(0);

// 多文件上传相关
const uploadedFiles = ref([]);
const uploadProgressMultiple = ref(0);

// 限制文件类型
const beforeUpload = (file) => {
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isLt2M) {
    message.error('文件大小不能超过 2MB!');
  }
  return isLt2M;
};

// 限制多文件类型
const beforeUploadMultiple = (file) => {
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isLt2M) {
    message.error('单个文件大小不能超过 2MB!');
  }
  return isLt2M;
};

// 处理单文件上传
const handleUpload = async (options) => {
  const { file, onSuccess, onError } = options;
  
  try {
    uploadProgress.value = 0;
    const result = await uploadFile('/api/file/upload', file, {
      onUploadProgress: (progressEvent) => {
        const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
        uploadProgress.value = percentCompleted;
      }
    });
    
    uploadedFile.value = file;
    onSuccess(result);
    message.success('文件上传成功');
  } catch (error) {
    onError(error);
    message.error('文件上传失败');
  }
};

// 处理多文件上传
const handleUploadMultiple = async (options) => {
  const { file, onSuccess, onError } = options;
  
  try {
    uploadProgressMultiple.value = 0;
    
    // 收集所有文件
    const allFiles = [];
    const uploadPromises = [];
    
    // 添加当前文件到列表
    allFiles.push(file);
    
    // 如果需要处理多个文件，可以在这里添加逻辑
    
    const result = await uploadFiles('/api/files/upload', allFiles, {
      onUploadProgress: (progressEvent) => {
        const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
        uploadProgressMultiple.value = percentCompleted;
      }
    });
    
    uploadedFiles.value = allFiles;
    onSuccess(result);
    message.success('文件上传成功');
  } catch (error) {
    onError(error);
    message.error('文件上传失败');
  }
};

// 处理文件下载
const handleDownload = async () => {
  if (!uploadedFile.value) {
    message.warning('请先上传文件');
    return;
  }
  
  try {
    // 这里应该使用实际的文件ID或URL
    await downloadFile('/api/file/download/' + uploadedFile.value.uid, uploadedFile.value.name);
  } catch (error) {
    message.error('文件下载失败');
  }
};

// 下载示例文件
const downloadSampleFile = async () => {
  try {
    await downloadFile('/api/file/sample', 'sample.txt');
  } catch (error) {
    message.error('示例文件下载失败');
  }
};
</script>

<style scoped>
.file-handling-example {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}
</style>