<template>
  <a-modal
    :open="open"
    :title="file && file.contentName ? file.contentName : '文件预览'"
    :footer="null"
    :width="modalWidth"
    destroy-on-close
    @cancel="handleCancel"
  >
    <a-spin :spinning="loading">
      <!-- 文本/Markdown/Html：渲染抽取文本（后端 HtmlReader 抽取结果为纯文本，
           但 DeEntitize 会复活源文档中被转义的字面量标签，必须文本插值渲染，禁止 v-html 防 XSS） -->
      <pre v-if="renderType === 'text' || renderType === 'markdown' || renderType === 'html'" class="content-text">{{ displayContent }}</pre>

      <!-- Pdf：blob 预览 -->
      <iframe
        v-else-if="renderType === 'pdf' && pdfUrl"
        :src="pdfUrl"
        class="pdf-frame"
        frameborder="0"
      ></iframe>

      <!-- 图片：blob 预览 -->
      <div v-else-if="renderType === 'image' && imageUrl" class="image-wrap">
        <img :src="imageUrl" class="image-preview" />
      </div>

      <!-- Word/Excel/Pptx：仅下载 -->
      <div v-else-if="isOfficeType" class="office-fallback">
        <a-empty description="该类型仅支持下载原文件" />
        <a-button type="primary" class="office-download" @click="downloadFile">下载原文件</a-button>
      </div>

      <!-- 加载失败/无内容兜底 -->
      <div v-else class="office-fallback">
        <a-empty description="预览加载失败，请下载查看" />
        <a-button type="primary" class="office-download" @click="downloadFile">下载原文件</a-button>
      </div>
    </a-spin>
  </a-modal>
</template>

<script setup>
/* global defineProps, defineEmits */
import { ref, computed, watch, onUnmounted } from 'vue';
import { message } from 'ant-design-vue';
import { getFileById, getImageById } from '@/api/file';

const props = defineProps({
  open: { type: Boolean, default: false },
  // 文件对象：{ fileId, fileType, content, contentName, url, status, dataCount, errorMessage }
  file: { type: Object, default: () => ({}) },
});

const emit = defineEmits(['cancel']);

const loading = ref(false);
const pdfUrl = ref('');
const imageUrl = ref('');
// 原文件流读取的文本（Content 字段为空时的兜底，存量数据 Content 未回写为空）
const textContent = ref('');

// 展示内容：优先用详情 Content，为空时用原文件流兜底
const displayContent = computed(() => {
  const c = props.file && props.file.content;
  return (c && c.length > 0) ? c : textContent.value;
});

// 类型归一：小写后分发
const renderType = computed(() => {
  const t = (props.file && props.file.fileType ? props.file.fileType : '').trim().toLowerCase();
  if (t === 'word' || t === 'excel' || t === 'pptx' || t === 'ppt') return t;
  if (t === 'text' || t === 'markdown' || t === 'html' || t === 'pdf' || t === 'image') return t;
  return 'unknown';
});

const isOfficeType = computed(
  () => renderType.value === 'word' || renderType.value === 'excel' || renderType.value === 'pptx'
);

// 弹窗宽度：屏幕占比 + 像素上限，小屏按比例、大屏不超上限
const fitWidth = (pct, max) => Math.min(Math.round(window.innerWidth * pct), max) + 'px';
const modalWidth = computed(() => {
  if (renderType.value === 'pdf') return fitWidth(0.85, 1400);
  if (renderType.value === 'image') return fitWidth(0.6, 960);
  if (renderType.value === 'text' || renderType.value === 'markdown' || renderType.value === 'html') return fitWidth(0.7, 1100);
  return '640px';
});

// 绕过 axios 全局拦截器（拦截器会把 pdf/octet-stream 响应自动触发下载，无法拿到 blob 预览）
const fetchFileBlob = async (fileId) => {
  const apiBase = process.env.VUE_APP_API_BASE_URL || '';
  const resp = await fetch(`${apiBase}/api/File/GetFile?fileid=${fileId}`, {
    method: 'GET',
    credentials: 'include',
    headers: { Authorization: `Bearer ${localStorage.getItem('token') || ''}` },
  });
  if (!resp.ok) {
    throw new Error(`HTTP ${resp.status}`);
  }
  return await resp.blob();
};

// blob URL 统一登记，关闭/卸载时全部 revoke，防止内存泄漏
const blobUrls = ref([]);
const createBlobUrl = (blob, mimeType) => {
  const url = window.URL.createObjectURL(new Blob([blob], { type: mimeType }));
  blobUrls.value.push(url);
  return url;
};
const revokeAll = () => {
  blobUrls.value.forEach((url) => window.URL.revokeObjectURL(url));
  blobUrls.value = [];
  pdfUrl.value = '';
  imageUrl.value = '';
};

const loadPreview = async () => {
  if (!props.open || !props.file) return;
  const file = props.file;
  const fileId = file.fileId;
  const hasUrl = !!file.url;
  textContent.value = '';
  // 文本/Markdown/Html：Content 为空（存量文件型数据未回写）时拉原文件流读取文本
  if (renderType.value === 'text' || renderType.value === 'markdown' || renderType.value === 'html') {
    const c = file.content;
    if (c && c.length > 0) return;
    if (!fileId && hasUrl) {
      // 远程文件无 fileId：直接用远程地址拉文本
      loading.value = true;
      try {
        const resp = await fetch(file.url);
        if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
        textContent.value = await resp.text();
      } catch (error) {
        console.error('文本预览加载失败:', error);
        message.error('预览加载失败，请下载查看');
      } finally {
        loading.value = false;
      }
      return;
    }
    if (!fileId) return;
    loading.value = true;
    try {
      const blob = await fetchFileBlob(fileId);
      textContent.value = await blob.text();
    } catch (error) {
      console.error('文本预览加载失败:', error);
      message.error('预览加载失败，请下载查看');
    } finally {
      loading.value = false;
    }
    return;
  }
  if (renderType.value === 'pdf') {
    if (!fileId && hasUrl) {
      // 远程文件无 fileId：直接用远程地址
      pdfUrl.value = file.url;
      return;
    }
    if (!fileId) return;
    loading.value = true;
    try {
      const blob = await fetchFileBlob(fileId);
      pdfUrl.value = createBlobUrl(blob, 'application/pdf');
    } catch (error) {
      console.error('PDF预览加载失败:', error);
      message.error('预览加载失败，请下载查看');
    } finally {
      loading.value = false;
    }
  } else if (renderType.value === 'image') {
    if (!fileId && hasUrl) {
      imageUrl.value = file.url;
      return;
    }
    if (!fileId) return;
    loading.value = true;
    try {
      const blob = await getImageById(fileId);
      imageUrl.value = createBlobUrl(blob, blob.type || 'image/png');
    } catch (error) {
      console.error('图片预览加载失败:', error);
      message.error('预览加载失败，请下载查看');
    } finally {
      loading.value = false;
    }
  }
};

const downloadFile = () => {
  const file = props.file || {};
  if (!file.fileId && !file.url) {
    message.error('文件不存在，无法下载');
    return;
  }
  if (file.fileId) {
    getFileById(file.fileId, file.contentName).catch((error) => {
      console.error('下载失败:', error);
      message.error('下载失败，请稍后重试');
    });
  } else if (file.url) {
    const a = document.createElement('a');
    a.href = file.url;
    a.download = file.contentName || 'downloaded_file';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  }
};

const handleCancel = () => {
  revokeAll();
  emit('cancel');
};

watch(
  () => props.open,
  (val) => {
    if (val) {
      loadPreview();
    } else {
      revokeAll();
    }
  }
);

onUnmounted(() => {
  revokeAll();
});
</script>

<style scoped>
.content-text {
  max-height: 74vh;
  overflow: auto;
  white-space: pre-wrap;
  word-break: break-word;
  margin: 0;
  font-size: 14px;
  line-height: 1.8;
  color: rgba(0, 0, 0, 0.88);
}

.pdf-frame {
  width: 100%;
  height: 78vh;
  border: none;
}

.image-wrap {
  display: flex;
  justify-content: center;
  align-items: center;
  max-height: 76vh;
  overflow: auto;
}

.image-preview {
  max-width: 100%;
  max-height: 76vh;
  object-fit: contain;
}

.office-fallback {
  padding: 24px 0;
  text-align: center;
}

.office-download {
  margin-top: 12px;
}
</style>
