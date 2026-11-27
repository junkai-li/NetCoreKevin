<template>
  <a-modal
    :open="open"
    :title="contentName ? `碎片预览 - ${contentName}` : '碎片预览'"
    :footer="null"
    :width="modalWidth"
    destroy-on-close
    @cancel="handleCancel"
  >
    <a-spin :spinning="loading">
      <a-empty v-if="!loading && total === 0" description="该文件尚未向量化入库" />

      <div v-else class="chunk-list">
        <div v-for="item in list" :key="item.id" class="chunk-item">
          <div class="chunk-header">
            <span class="chunk-index">#{{ item.chunkIndex }}</span>
            <span class="chunk-chars">{{ (item.content || '').length }} 字符</span>
          </div>
          <a-typography-paragraph
            class="chunk-content"
            :ellipsis="{ rows: 3, expandable: true }"
            :content="item.content"
          />
        </div>
      </div>

      <div v-if="total > 0" class="chunk-pagination">
        <a-pagination
          v-model:current="pageNum"
          :total="total"
          :page-size="pageSize"
          show-total="(t) => `共 ${t} 条碎片`"
          @change="loadChunks"
        />
      </div>
    </a-spin>
  </a-modal>
</template>

<script setup>
/* global defineProps, defineEmits */
import { ref, computed, watch, onUnmounted } from 'vue';
import { message } from 'ant-design-vue';

// 弹窗宽度：屏幕占比 + 像素上限，小屏按比例、大屏不超上限
const modalWidth = computed(() => Math.min(Math.round(window.innerWidth * 0.7), 1000) + 'px');
import { getKmsChunks } from '@/api/ai/aikmss';

const props = defineProps({
  open: { type: Boolean, default: false },
  kmsId: { type: Number, default: null },
  sourceFile: { type: String, default: '' },
  contentName: { type: String, default: '' },
});

const emit = defineEmits(['cancel']);

const loading = ref(false);
const total = ref(0);
const list = ref([]);
const pageNum = ref(1);
const pageSize = 10;

const loadChunks = async () => {
  if (!props.open || props.kmsId == null || !props.sourceFile) return;
  loading.value = true;
  try {
    const response = await getKmsChunks({
      kmsId: props.kmsId,
      sourceFile: props.sourceFile,
      pageNum: pageNum.value,
      pageSize: pageSize,
    });
    if (response && response.code === 200) {
      const data = response.data || {};
      total.value = Number(data.total) || 0;
      list.value = Array.isArray(data.list) ? data.list : [];
    } else {
      total.value = 0;
      list.value = [];
    }
  } catch (error) {
    console.error('加载碎片失败:', error);
    message.error('加载碎片失败，请稍后重试');
    total.value = 0;
    list.value = [];
  } finally {
    loading.value = false;
  }
};

const handleCancel = () => {
  emit('cancel');
};

watch(
  () => props.open,
  (val) => {
    if (val) {
      pageNum.value = 1;
      list.value = [];
      total.value = 0;
      loadChunks();
    }
  }
);

onUnmounted(() => {
  list.value = [];
  total.value = 0;
});
</script>

<style scoped>
.chunk-list {
  max-height: 74vh;
  overflow: auto;
}

.chunk-item {
  padding: 10px 12px;
  border: 1px solid #f0f0f0;
  border-radius: 6px;
  margin-bottom: 8px;
  background: #fafafa;
}

.chunk-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 4px;
}

.chunk-index {
  font-size: 12px;
  font-weight: 600;
  color: var(--fn-color-brand, #1677ff);
}

.chunk-chars {
  font-size: 12px;
  color: rgba(0, 0, 0, 0.45);
}

.chunk-content {
  margin: 0;
  font-size: 14px;
  line-height: 1.8;
  color: rgba(0, 0, 0, 0.88);
  word-break: break-word;
  white-space: pre-wrap;
}

.chunk-pagination {
  margin-top: 12px;
  display: flex;
  justify-content: flex-end;
}
</style>
