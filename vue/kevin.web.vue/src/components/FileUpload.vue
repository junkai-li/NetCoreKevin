<template>
  <div class="file-upload-container">
    <a-upload
      v-model:file-list="fileList"
      :custom-request="customRequest"
      :before-upload="beforeUpload"
      :accept="accept"
      :multiple="multiple"
      :max-count="maxCount"
      :disabled="disabled || uploading"
      :show-upload-list="showUploadList"
    >
      <a-button :disabled="disabled || uploading" :loading="uploading">
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
          <a
            v-if="file.url"
            :href="file.url"
            target="_blank"
            class="file-name-link"
          >
            {{ file.name }}
          </a>
          <span v-else class="file-name">{{ file.name }}</span>
          <span class="file-size" v-if="file.size">({{ formatFileSize(file.size) }})</span>
        </div>
        <div class="file-actions">
          <a-tooltip v-if="enablePreview && canPreview(file)" title="预览">
            <a-button
              type="text"
              @click="handlePreview(file)"
              :disabled="disabled"
            >
              <template #icon><EyeOutlined /></template>
            </a-button>
          </a-tooltip>
          <a-tooltip v-if="enableZipEdit && isZipFile(file.name)" title="编辑压缩包">
            <a-button
              type="text"
              @click="handleZipEdit(file)"
              :disabled="disabled"
            >
              <template #icon><EditOutlined /></template>
            </a-button>
          </a-tooltip>
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

    <!-- 文件预览Modal -->
    <a-modal
      v-model:open="previewModalVisible"
      :title="previewFile?.name"
      :footer="null"
      width="800px"
    >
      <div v-if="previewLoading" class="preview-loading">
        <a-spin tip="加载中..." />
      </div>
      <div v-else-if="previewType === 'image'" class="preview-content">
        <img :src="previewFile.url" :alt="previewFile?.name" class="preview-image" />
      </div>
      <div v-else-if="previewType === 'pdf'" class="preview-content preview-pdf">
        <embed :src="previewFile.url" type="application/pdf" class="preview-embed" />
      </div>
      <div v-else-if="previewType === 'word'" class="preview-content preview-word">
        <iframe
          :src="'https://docs.google.com/gview?url=' + encodeURIComponent(previewFile.url) + '&embedded=true'"
          class="preview-iframe"
        />
      </div>
      <div v-else-if="previewType === 'markdown'" class="preview-content preview-markdown" v-html="previewHtml"></div>
      <div v-else-if="previewContent" class="preview-content">
        <pre class="preview-text">{{ previewContent }}</pre>
      </div>
      <a-empty v-else description="暂不支持预览此文件" />
    </a-modal>

    <!-- Zip编辑Modal -->
    <a-modal
      v-model:open="zipEditModalVisible"
      :title="'编辑: ' + (zipEditFile?.name || '')"
      width="90%"
      style="top: 20px"
      @cancel="handleZipEditCancel"
      :footer="null"
      :maskClosable="false"
    >
      <div class="zip-edit-container">
        <div class="zip-edit-header">
          <div class="zip-edit-actions">
            <a-button
              type="primary"
              :loading="zipEditSaving"
              :disabled="!hasZipChanges"
              @click="handleSaveZip"
            >
              <template #icon><SaveOutlined /></template>
              保存并上传
            </a-button>
            <a-button @click="handleDownloadZip" :disabled="zipEditLoading">
              <template #icon><DownloadOutlined /></template>
              下载Zip
            </a-button>
            <a-button @click="handleZipEditCancel">
              <template #icon><CloseOutlined /></template>
              关闭
            </a-button>
          </div>
        </div>
        <div class="zip-edit-content">
          <div class="zip-file-tree">
            <div class="tree-header">
              <span>文件列表</span>
              <a-badge v-if="hasZipChanges" status="warning" text="已修改" />
            </div>
            <div class="tree-content">
              <a-empty v-if="zipFileTree.length === 0 && !zipEditLoading" description="暂无文件" />
              <a-spin v-if="zipEditLoading" tip="加载中..." />
              <div v-else>
                <div
                  v-for="item in zipFileTree"
                  :key="item.path"
                  class="tree-item"
                  :class="{
                    'file-item-directory': item.isDirectory,
                    'selected': selectedZipFile?.path === item.path
                  }"
                  :style="{ paddingLeft: (item.level * 16 + 12) + 'px' }"
                  @click="handleFileItemClick(item)"
                >
                  <component
                    :is="item.isDirectory ? (item.isExpanded ? FolderOpenOutlined : FolderOutlined) : getFileIcon(item.name)"
                    :class="['file-icon', { 'icon-directory': item.isDirectory }]"
                  />
                  <span class="file-name" :title="item.path">{{ item.name }}</span>
                  <span v-if="zipContent[item.path]?.modified" class="modified-dot"></span>
                </div>
              </div>
            </div>
          </div>
          <div class="zip-editor-area">
            <div class="editor-header" v-if="selectedZipFile">
              <span class="editor-file-name">{{ selectedZipFile.path }}</span>
              <a-tag v-if="hasZipChanges && zipContent[selectedZipFile.path]?.modified" color="orange">已修改</a-tag>
            </div>
            <div class="editor-content" ref="zipEditorContainer">
              <a-empty v-if="!selectedZipFile" description="请选择要编辑的文件" />
            </div>
          </div>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, watch, nextTick } from 'vue';
import {
  UploadOutlined,
  PaperClipOutlined,
  DeleteOutlined,
  EyeOutlined,
  EditOutlined,
  SaveOutlined,
  CloseOutlined,
  DownloadOutlined,
  FolderOutlined,
  FolderOpenOutlined,
  FileOutlined,
  FileTextOutlined
} from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import { uploadFile, deleteFileById, getFileInfoById } from '@/api/file';
import JSZip from 'jszip';
import { saveAs } from 'file-saver';
import { marked } from 'marked';
import { EditorView, basicSetup } from 'codemirror';
import { EditorState } from '@codemirror/state';
import { javascript } from '@codemirror/lang-javascript';
import { markdown } from '@codemirror/lang-markdown';
import { oneDark } from '@codemirror/theme-one-dark';

const props = defineProps({
  business: {
    type: String,
    required: true
  },
  keyValue: {
    type: String,
    required: true
  },
  sign: {
    type: String,
    required: true
  },
  accept: {
    type: String,
    default: ''
  },
  multiple: {
    type: Boolean,
    default: false
  },
  maxCount: {
    type: Number,
    default: undefined
  },
  disabled: {
    type: Boolean,
    default: false
  },
  showUploadList: {
    type: Boolean,
    default: false
  },
  uploadButtonText: {
    type: String,
    default: '上传文件'
  },
  initialFiles: {
    type: Array,
    default: () => []
  },
  enablePreview: {
    type: Boolean,
    default: true
  },
  enableZipEdit: {
    type: Boolean,
    default: true
  }
});

const emit = defineEmits(['upload-success', 'upload-error', 'delete-success', 'delete-error', 'zip-updated']);

const fileList = ref([]);
const uploading = ref(false);
const uploadedFiles = ref([]);

const normalizeFile = (fileObj) => {
  if (!fileObj) return null;
  return {
    id: fileObj.id || fileObj.fileId || '',
    name: fileObj.name || fileObj.fileName || '未知文件',
    size: fileObj.size || fileObj.fileSize || 0,
    url: fileObj.url || '',
    rawFile: fileObj.rawFile || null
  };
};

const initUploadedFiles = () => {
  if (!props.initialFiles) {
    uploadedFiles.value = [];
    return;
  }
  const files = Array.isArray(props.initialFiles) ? props.initialFiles : [props.initialFiles];
  uploadedFiles.value = files.map(normalizeFile).filter(f => f.id);
};

initUploadedFiles();

watch(() => props.initialFiles, () => {
  initUploadedFiles();
}, { deep: true });

const customRequest = async (options) => {
  const { file, onSuccess, onError } = options;
  uploading.value = true;

  try {
    if (props.maxCount && uploadedFiles.value.length >= props.maxCount) {
      throw new Error(`最多只能上传${props.maxCount}个文件`);
    }
    const fileId = (await uploadFile(props.business, props.keyValue, props.sign, file)).data;

    const fileInfoResponse = await getFileInfoById(fileId);
    const fileInfo = fileInfoResponse.data || {};

    uploadedFiles.value.push({
      id: fileId,
      name: fileInfo.name || file.name,
      size: fileInfo.size || file.size,
      url: fileInfo.url || '',
      path: fileInfo.path || '',
      rawFile: file
    });

    emit('upload-success', {
      fileId: fileId,
      fileName: fileInfo.name || file.name,
      fileSize: fileInfo.size || file.size,
      url: fileInfo.url || '',
      path: fileInfo.path || ''
    });

    if (!props.multiple) {
      fileList.value = [];
    }

    onSuccess(fileId);
  } catch (error) {
    console.error('文件上传失败:', error);
    onError(error);

    emit('upload-error', {
      error: error,
      fileName: file.name
    });
  } finally {
    uploading.value = false;
  }
};

const beforeUpload = () => {
  return true;
};

const handleDeleteFile = async (file) => {
  try {
    const result = await deleteFileById(file.id);

    if (result) {
      uploadedFiles.value = uploadedFiles.value.filter(f => f.id !== file.id);

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

    emit('delete-error', {
      error: error,
      fileId: file.id,
      fileName: file.name
    });

    message.error('文件删除失败: ' + error.message);
  }
};

const formatFileSize = (size) => {
  if (size === 0) return '0 Bytes';
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(size) / Math.log(k));
  return parseFloat((size / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
};

const getFileExtension = (filename) => {
  const parts = filename.split('.');
  return parts.length > 1 ? parts[parts.length - 1].toLowerCase() : '';
};

const isImageFile = (filename) => {
  const ext = getFileExtension(filename);
  return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext);
};

const isTextFile = (filename) => {
  const ext = getFileExtension(filename);
  return ['txt', 'md', 'markdown', 'json', 'xml', 'html', 'css', 'js', 'ts', 'py', 'java', 'c', 'cpp', 'h', 'cs', 'go', 'rb', 'php', 'sh', 'yaml', 'yml', 'ini', 'conf', 'log'].includes(ext);
};

const isZipFile = (filename) => {
  const ext = getFileExtension(filename);
  return ext === 'zip';
};

const isPDFFile = (filename) => {
  const ext = getFileExtension(filename);
  return ext === 'pdf';
};

const isWordFile = (filename) => {
  const ext = getFileExtension(filename);
  return ['doc', 'docx'].includes(ext);
};

const isMarkdownFile = (filename) => {
  const ext = getFileExtension(filename);
  return ['md', 'markdown'].includes(ext);
};

const canPreview = (file) => {
  return isImageFile(file.name) || isTextFile(file.name) || isZipFile(file.name) || isPDFFile(file.name) || isWordFile(file.name);
};

const previewModalVisible = ref(false);
const previewFile = ref(null);
const previewContent = ref('');
const previewHtml = ref('');
const previewLoading = ref(false);
const previewType = ref('text');

const handlePreview = async (file) => {
  previewFile.value = file;

  if (isImageFile(file.name)) {
    previewType.value = 'image';
    previewModalVisible.value = true;
    return;
  }

  if (isZipFile(file.name)) {
    handleZipEdit(file);
    return;
  }

  if (isPDFFile(file.name)) {
    previewType.value = 'pdf';
    previewModalVisible.value = true;
    return;
  }

  if (isWordFile(file.name)) {
    previewType.value = 'word';
    previewModalVisible.value = true;
    return;
  }

  if (isTextFile(file.name) && file.url) {
    previewLoading.value = true;
    const isMarkdown = isMarkdownFile(file.name);
    previewType.value = isMarkdown ? 'markdown' : 'text';
    previewModalVisible.value = true;

    try {
      const response = await fetch(file.url);
      const arrayBuffer = await response.arrayBuffer();

      const tryDecode = (buffer, encodings) => {
        for (const encoding of encodings) {
          try {
            const decoder = new TextDecoder(encoding);
            const text = decoder.decode(buffer);
            if (text && !text.includes('\ufffd')) {
              return { text, encoding };
            }
          } catch (e) {
            // ignore encoding error
          }
        }
        return { text: new TextDecoder('utf-8').decode(buffer), encoding: 'utf-8' };
      };

      const { text } = tryDecode(arrayBuffer, ['utf-8', 'gbk', 'gb2312', 'gb18030', 'big5']);
      previewContent.value = text;

      if (isMarkdown) {
        previewHtml.value = marked(previewContent.value);
      }
    } catch (error) {
      console.error('预览加载失败:', error);
      previewContent.value = null;
      previewHtml.value = '';
    } finally {
      previewLoading.value = false;
    }
    return;
  }

  previewType.value = 'text';
  previewModalVisible.value = true;
  previewContent.value = null;
};

const zipEditModalVisible = ref(false);
const zipEditLoading = ref(false);
const zipEditSaving = ref(false);
const zipEditFile = ref(null);
const zipFileTree = ref([]);
const zipFileTreeOriginal = ref([]);
const expandedFolders = ref(new Set());
const zipContent = ref({});
const selectedZipFile = ref(null);
const zipEditorContainer = ref(null);
const zipEditorView = ref(null);
const hasZipChanges = ref(false);
const currentZipBlob = ref(null);

const getFileIcon = (fileName) => {
  const ext = getFileExtension(fileName);
  if (['md', 'markdown'].includes(ext)) return FileTextOutlined;
  return FileOutlined;
};

const handleFileItemClick = (item) => {
  if (item.isDirectory) {
    const newExpanded = new Set(expandedFolders.value);
    if (newExpanded.has(item.path)) {
      newExpanded.delete(item.path);
    } else {
      newExpanded.add(item.path);
    }
    expandedFolders.value = newExpanded;
    rebuildZipFileTree();
  } else {
    selectZipFile(item);
  }
};

const rebuildZipFileTree = () => {
  const result = [];
  const processedPaths = new Set();

  const flatten = (items, level) => {
    items.forEach((item) => {
      if (processedPaths.has(item.path)) return;
      processedPaths.add(item.path);

      const isExpanded = expandedFolders.value.has(item.path);
      result.push({ ...item, level, isExpanded });

      if (item.isDirectory && isExpanded && item.children && item.children.length > 0) {
        flatten(item.children, level + 1);
      }
    });
  };

  flatten(zipFileTreeOriginal.value, 0);

  zipFileTree.value = result;
};

const handleZipEdit = async (file) => {
  zipEditFile.value = file;
  zipEditModalVisible.value = true;
  zipEditLoading.value = true;
  zipFileTree.value = [];
  zipContent.value = {};
  selectedZipFile.value = null;
  hasZipChanges.value = false;
  currentZipBlob.value = null;

  try {
    const response = await fetch(file.url);
    const blob = await response.blob();
    currentZipBlob.value = blob;
    const zip = await JSZip.loadAsync(blob);

    const filePromises = [];
    const contentMap = {};
    const folderSet = new Set();

    zip.forEach((relativePath, zipFile) => {
      const parts = relativePath.split('/').filter(p => p);
      for (let i = 1; i < parts.length; i++) {
        folderSet.add(parts.slice(0, i).join('/'));
      }

      if (!zipFile.dir) {
        const filePromise = zipFile.async('uint8array').then((uint8array) => {
          const ext = relativePath.split('.').pop().toLowerCase();
          const textExts = [
            'txt', 'md', 'markdown', 'json', 'xml', 'html', 'css', 'js', 'ts', 'jsx', 'tsx',
            'py', 'java', 'c', 'cpp', 'h', 'hpp', 'cs', 'go', 'rb', 'php', 'sh', 'bash',
            'zsh', 'ps1', 'psm1', 'bat', 'cmd', 'yaml', 'yml', 'ini', 'conf', 'log',
            'sql', 'swift', 'kt', 'scala', 'r', 'lua', 'pl', 'pm', 'groovy', 'gradle',
            'vue', 'svelte', 'jsx', 'tsx', 'dart', 'ex', 'exs', 'erl', 'hrl',
            'toml', 'properties', 'env', 'gitignore', 'dockerfile', 'makefile'
          ];

          if (textExts.includes(ext)) {
            const tryDecodeText = (buffer) => {
              const encodings = ['utf-8', 'gbk', 'gb2312', 'gb18030', 'big5'];
              for (const encoding of encodings) {
                try {
                  const decoder = new TextDecoder(encoding);
                  const text = decoder.decode(buffer);
                  if (text && !text.includes('\ufffd')) {
                    return text;
                  }
                } catch (e) {
                  // ignore
                }
              }
              return new TextDecoder('utf-8').decode(buffer);
            };
            const content = tryDecodeText(uint8array);
            contentMap[relativePath] = {
              content: content,
              originalContent: content,
              modified: false,
            };
          } else {
            const base64 = btoa(String.fromCharCode.apply(null, uint8array));
            contentMap[relativePath] = {
              content: base64,
              originalContent: base64,
              modified: false,
              isBinary: true,
            };
          }
        });
        filePromises.push(filePromise);
      }
    });

    await Promise.all(filePromises);

    const treeData = [];

    folderSet.forEach((folderPath) => {
      const parts = folderPath.split('/');
      const name = parts[parts.length - 1];
      const parentPath = parts.length > 1 ? parts.slice(0, -1).join('/') : '';
      treeData.push({
        name: name,
        path: folderPath,
        isDirectory: true,
        isExpanded: false,
        children: [],
        parentPath: parentPath,
      });
    });

    zip.forEach((relativePath, zipFile) => {
      if (!zipFile.dir) {
        const name = relativePath.split('/').pop();
        const parts = relativePath.split('/');
        const parentPath = parts.length > 1 ? parts.slice(0, -1).join('/') : '';

        treeData.push({
          name: name,
          path: relativePath,
          isDirectory: false,
          modified: false,
          parentPath: parentPath,
          children: null,
        });
      }
    });

    const pathToItem = new Map();

    treeData.forEach((item) => {
      pathToItem.set(item.path, { ...item, children: [] });
    });

    const rootItems = [];

    pathToItem.forEach((item) => {
      if (item.parentPath && pathToItem.has(item.parentPath)) {
        pathToItem.get(item.parentPath).children.push(item);
      } else {
        rootItems.push(item);
      }
    });

    zipFileTreeOriginal.value = rootItems;
    rebuildZipFileTree();
    zipContent.value = contentMap;

    if (zipFileTree.value.length > 0) {
      await nextTick();
      const firstFile = zipFileTree.value.find((item) => !item.isDirectory);
      if (firstFile) {
        selectZipFile(firstFile);
      }
    }
  } catch (error) {
    console.error('加载压缩包失败:', error);
    message.error('加载压缩包失败: ' + (error.message || '未知错误'));
  } finally {
    zipEditLoading.value = false;
  }
};

const selectZipFile = async (file) => {
  if (selectedZipFile.value?.path === file.path) return;

  selectedZipFile.value = file;

  await nextTick();

  if (zipEditorView.value) {
    zipEditorView.value.destroy();
    zipEditorView.value = null;
  }

  if (zipEditorContainer.value) {
    zipEditorContainer.value.innerHTML = '';
  }

  const fileData = zipContent.value[file.path];
  if (!fileData) return;

  if (fileData.isBinary) {
    zipEditorContainer.value.innerHTML = `
      <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100%; color: #999;">
        <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round">
          <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
          <polyline points="14 2 14 8 20 8"></polyline>
        </svg>
        <p style="margin-top: 16px;">二进制文件不支持在线编辑</p>
        <p style="font-size: 12px;">${file.name}</p>
      </div>
    `;
    return;
  }

  const isMarkdown = ['md', 'markdown'].includes(getFileExtension(file.name));
  const language = isMarkdown ? markdown() : javascript();

  const state = EditorState.create({
    doc: fileData.content,
    extensions: [
      basicSetup,
      language,
      oneDark,
      EditorView.updateListener.of((update) => {
        if (update.docChanged) {
          const newContent = update.state.doc.toString();
          zipContent.value[file.path].content = newContent;
          zipContent.value[file.path].modified = newContent !== zipContent.value[file.path].originalContent;
          hasZipChanges.value = Object.values(zipContent.value).some((f) => f.modified);
        }
      }),
      EditorView.theme({
        '&': { height: '100%' },
        '.cm-scroller': { overflow: 'auto' },
      }),
    ],
  });

  zipEditorView.value = new EditorView({
    state,
    parent: zipEditorContainer.value,
  });
};

const handleSaveZip = async () => {
  if (!hasZipChanges.value) return;

  zipEditSaving.value = true;

  try {
    const zip = new JSZip();

    Object.entries(zipContent.value).forEach(([path, data]) => {
      if (data.isBinary) {
        const binaryString = atob(data.content);
        const bytes = new Uint8Array(binaryString.length);
        for (let i = 0; i < binaryString.length; i++) {
          bytes[i] = binaryString.charCodeAt(i);
        }
        zip.file(path, bytes);
      } else {
        zip.file(path, data.content);
      }
    });

    const blob = await zip.generateAsync({ type: 'blob' });

    const fileName = zipEditFile.value.name;
    const newFile = new File([blob], fileName, { type: 'application/zip' });

    const oldFileId = zipEditFile.value.id;

    await deleteFileById(oldFileId);

    const newFileId = (await uploadFile(props.business, props.keyValue, props.sign, newFile)).data;

    const fileInfoResponse = await getFileInfoById(newFileId);
    const fileInfo = fileInfoResponse.data;

    const updatedFile = {
      id: newFileId,
      name: fileInfo.name || fileName,
      size: fileInfo.size || blob.size,
      url: fileInfo.url,
    };

    const index = uploadedFiles.value.findIndex((f) => f.id === oldFileId);
    if (index !== -1) {
      uploadedFiles.value[index] = { ...uploadedFiles.value[index], ...updatedFile };
    }

    hasZipChanges.value = false;
    zipEditModalVisible.value = false;
    currentZipBlob.value = null;

    message.success('文件保存成功');
    emit('zip-updated', { oldFileId, newFileId, file: updatedFile });
  } catch (error) {
    console.error('保存失败:', error);
    message.error('保存失败: ' + (error.message || '未知错误'));
  } finally {
    zipEditSaving.value = false;
  }
};

const handleDownloadZip = () => {
  if (!currentZipBlob.value) {
    message.warning('请先加载压缩包');
    return;
  }

  const zip = new JSZip();

  Object.entries(zipContent.value).forEach(([path, data]) => {
    if (data.isBinary) {
      const binaryString = atob(data.content);
      const bytes = new Uint8Array(binaryString.length);
      for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
      }
      zip.file(path, bytes);
    } else {
      zip.file(path, data.content);
    }
  });

  zip.generateAsync({ type: 'blob' }).then((blob) => {
    saveAs(blob, zipEditFile.value?.name || 'modified.zip');
  });
};

const handleZipEditCancel = () => {
  if (hasZipChanges.value) {
    zipEditModalVisible.value = false;
    zipFileTree.value = [];
    zipFileTreeOriginal.value = [];
    expandedFolders.value = new Set();
    zipContent.value = {};
    selectedZipFile.value = null;
    currentZipBlob.value = null;
    hasZipChanges.value = false;

    if (zipEditorView.value) {
      zipEditorView.value.destroy();
      zipEditorView.value = null;
    }
  } else {
    zipEditModalVisible.value = false;
  }
};

watch(uploadedFiles, (newFiles) => {
  if (newFiles.length > 0) {
    const fileIds = newFiles.map((file) => file.id);
    emit('file-ids-change', fileIds);
  }
}, { deep: true });

defineExpose({
  getUploadedFileIds: () => uploadedFiles.value.map((file) => file.id),
  getUploadedFiles: () => [...uploadedFiles.value],
  clearUploadedFiles: () => {
    uploadedFiles.value = [];
  },
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

.file-name-link {
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  color: #1890ff;
  text-decoration: none;
}

.file-name-link:hover {
  text-decoration: underline;
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

.preview-loading {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 300px;
}

.preview-content {
  max-height: 60vh;
  overflow: auto;
}

.preview-image {
  max-width: 100%;
  max-height: 60vh;
  object-fit: contain;
}

.preview-iframe {
  width: 100%;
  height: 60vh;
  border: none;
}

.preview-embed {
  width: 100%;
  height: 60vh;
  border: none;
}

.preview-markdown {
  background: #fff;
  padding: 16px;
  max-height: 60vh;
  overflow: auto;
}

.preview-markdown :deep(h1),
.preview-markdown :deep(h2),
.preview-markdown :deep(h3),
.preview-markdown :deep(h4),
.preview-markdown :deep(h5),
.preview-markdown :deep(h6) {
  margin-top: 16px;
  margin-bottom: 8px;
  font-weight: 600;
}

.preview-markdown :deep(p) {
  margin-bottom: 8px;
}

.preview-markdown :deep(code) {
  background: #f5f5f5;
  padding: 2px 6px;
  border-radius: 4px;
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
}

.preview-markdown :deep(pre) {
  background: #f5f5f5;
  padding: 12px;
  border-radius: 4px;
  overflow-x: auto;
}

.preview-markdown :deep(pre code) {
  background: none;
  padding: 0;
}

.preview-markdown :deep(ul),
.preview-markdown :deep(ol) {
  padding-left: 24px;
  margin-bottom: 8px;
}

.preview-markdown :deep(blockquote) {
  border-left: 4px solid #ddd;
  padding-left: 16px;
  margin-left: 0;
  color: #666;
}

.preview-word {
  width: 100%;
  height: 60vh;
  border: none;
}

.preview-text {
  background: #f5f5f5;
  padding: 16px;
  border-radius: 4px;
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 60vh;
  overflow: auto;
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
  font-size: 13px;
  line-height: 1.5;
}

.zip-edit-container {
  display: flex;
  flex-direction: column;
  height: 70vh;
}

.zip-edit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 16px;
  border-bottom: 1px solid #f0f0f0;
}

.zip-edit-actions {
  display: flex;
  gap: 8px;
}

.zip-edit-content {
  display: flex;
  flex: 1;
  margin-top: 16px;
  min-height: 0;
}

.zip-file-tree {
  width: 280px;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  margin-right: 16px;
}

.tree-header {
  padding: 12px;
  border-bottom: 1px solid #f0f0f0;
  font-weight: 500;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tree-content {
  flex: 1;
  overflow-y: auto;
  padding: 8px 0;
}

.tree-item {
  display: flex;
  align-items: center;
  padding: 6px 12px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.tree-item:hover {
  background-color: #f5f5f5;
}

.tree-item.selected {
  background-color: #e6f7ff;
}

.tree-item.file-item-directory {
  font-weight: 500;
}

.file-icon {
  margin-right: 8px;
  color: #1890ff;
}

.file-icon.icon-directory {
  color: #faad14;
}

.file-name {
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.modified-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background-color: #faad14;
  margin-left: 4px;
}

.zip-editor-area {
  flex: 1;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.editor-header {
  padding: 12px;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.editor-file-name {
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
  font-size: 13px;
}

.editor-content {
  flex: 1;
  overflow: hidden;
}

.editor-content :deep(.cm-editor) {
  height: 100%;
}

.editor-content :deep(.cm-scroller) {
  overflow: auto;
}
</style>
