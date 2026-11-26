<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <BookOutlined class="title-icon" />
            <span>我的智能体记忆</span>
          </div>
        </div>
      </template>

      <div class="memories-layout">
        <!-- 左栏：开启记忆的智能体列表 + 记忆数徽标 -->
        <div class="agent-panel">
          <div class="agent-panel-title">有记忆的智能体</div>
          <div
            v-for="item in agentMemoryList"
            :key="String(item.aiAppsId)"
            class="agent-item"
            :class="{ active: item.aiAppsId === selectedAIAppsId }"
            @click="selectAgent(item.aiAppsId)"
          >
            <RobotOutlined v-if="item.aiAppsId !== 0" class="agent-item-icon" />
            <GlobalOutlined v-else class="agent-item-icon" />
            <span class="agent-item-name">{{ item.name }}</span>
            <a-badge
              class="agent-item-badge"
              :count="item.count"
              :number-style="{ backgroundColor: item.aiAppsId === selectedAIAppsId ? '#fff' : '#f5222d', color: item.aiAppsId === selectedAIAppsId ? '#f5222d' : '#fff' }"
              :overflow-count="999"
            />
          </div>
          <a-empty v-if="agentMemoryList.length === 0" description="暂无智能体记忆" />
        </div>

        <!-- 右栏：记忆表格 + 筛选工具条 -->
        <div class="memory-panel">
          <div class="toolbar">
            <a-select
              v-model:value="importanceFilter"
              class="toolbar-item"
              style="width: 120px"
              :options="importanceOptions"
              @change="handleFilterChange"
            />
            <a-input-search
              v-model:value="searchKeyword"
              class="toolbar-item"
              placeholder="搜索内容/关键词..."
              style="width: 250px"
              allow-clear
              @search="handleFilterChange"
            />
          </div>

          <a-table
            :columns="columns"
            :data-source="memoryList"
            :loading="tableLoading"
            :pagination="false"
            row-key="id"
            size="middle"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'content'">
                <a-tooltip :title="record.content">
                  <span class="content-cell">{{ briefContent(record.content) }}</span>
                </a-tooltip>
              </template>
              <template v-else-if="column.key === 'memoryType'">
                <a-tag :color="memoryTypeColor(record.memoryType)">{{ memoryTypeText(record.memoryType) }}</a-tag>
              </template>
              <template v-else-if="column.key === 'importance'">
                <a-tag :color="importanceColor(record.importance)">{{ record.importance }}</a-tag>
              </template>
              <template v-else-if="column.key === 'memoryTerm'">
                <div class="memory-term-cell">
                  <a-tag color="orange" v-if="record.expireTime">短期</a-tag>
                  <a-tag color="blue" v-else>长期</a-tag>
                  <span v-if="record.expireTime" class="expire-time">{{ formatTime(record.expireTime) }}</span>
                </div>
              </template>
              <template v-else-if="column.key === 'keywords'">
                <span class="keywords-cell">{{ record.keywords }}</span>
              </template>
              <template v-else-if="column.key === 'createTime'">
                <span>{{ formatTime(record.createTime) }}</span>
              </template>
              <template v-else-if="column.key === 'action'">
                <a-space>
                  <a @click="openEditModal(record)">编辑</a>
                  <a class="danger-link" @click="confirmDelete(record)">删除</a>
                </a-space>
              </template>
            </template>
          </a-table>

          <div class="pagination-container" v-if="pagination.total > 0">
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
      </div>
    </a-card>

    <!-- 编辑弹窗（记忆内容 + 关键词可编辑，其余字段只读展示） -->
    <a-modal
      v-model:open="editModalVisible"
      title="编辑记忆"
      :confirm-loading="saving"
      @ok="handleSave"
    >
      <a-form :label-col="{ span: 5 }" :wrapper-col="{ span: 17 }">
        <a-form-item label="记忆内容" required>
          <a-textarea v-model:value="editForm.content" :rows="4" :maxlength="4000" show-count placeholder="用一句话完整描述要记住的信息" />
        </a-form-item>
        <a-form-item label="记忆类型">
          <a-tag :color="memoryTypeColor(editForm.memoryType)">{{ memoryTypeText(editForm.memoryType) }}</a-tag>
        </a-form-item>
        <a-form-item label="重要程度">
          <a-tag :color="importanceColor(editForm.importance)">{{ editForm.importance }}</a-tag>
          <span class="importance-tip">重要程度由 AI 沉淀时评估，不可编辑</span>
        </a-form-item>
        <a-form-item label="关键词">
          <a-input v-model:value="editForm.keywords" :maxlength="200" placeholder="检索关键词，逗号分隔（可选）" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import '../../css/CardTable.css';
import { ref, reactive, onMounted } from 'vue';
import {
  BookOutlined,
  RobotOutlined,
  GlobalOutlined
} from '@ant-design/icons-vue';
import { Modal, message } from 'ant-design-vue';
import {
  getMyAIAppsALLList
} from '@/api/ai/aiapps';
import {
  getMemoryPageData,
  addEditMemory,
  deleteMemory,
  getMemoryCountsByAIApps
} from '@/api/ai/aiagentmemories';

// 重要程度三档映射（全部不传区间参数）：高 8-10 / 中 5-7 / 低 0-4
const importanceOptions = [
  { label: '全部', value: 'all' },
  { label: '高（8-10）', value: 'high' },
  { label: '中（5-7）', value: 'middle' },
  { label: '低（0-4）', value: 'low' }
];
const importanceRangeMap = {
  high: { min: 8, max: 10 },
  middle: { min: 5, max: 7 },
  low: { min: 0, max: 4 }
};

// MemoryType 7 种合法值与中文映射
const memoryTypeMap = {
  preference: { text: '偏好', color: 'purple' },
  fact: { text: '事实', color: 'blue' },
  task: { text: '短期记忆', color: 'orange' },
  decision: { text: '决策', color: 'geekblue' },
  pitfall: { text: '踩坑', color: 'red' },
  skill: { text: '技能', color: 'green' },
  other: { text: '其他', color: 'default' }
};

const columns = [
  { title: '内容', key: 'content', ellipsis: true },
  { title: '类型', key: 'memoryType', width: 110 },
  { title: '重要程度', key: 'importance', width: 100 },
  { title: '期限', key: 'memoryTerm', width: 150 },
  { title: '关键词', key: 'keywords', ellipsis: true },
  { title: '命中次数', key: 'hitCount', width: 100 },
  { title: '创建时间', key: 'createTime', width: 170 },
  { title: '操作', key: 'action', width: 120 }
];

const agentMemoryList = ref([]);
const selectedAIAppsId = ref(0);
const memoryList = ref([]);
const tableLoading = ref(false);
const importanceFilter = ref('all');
const searchKeyword = ref('');
const pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0
});

const editModalVisible = ref(false);
const saving = ref(false);
const editForm = reactive({
  id: null,
  aiAppsId: 0,
  content: '',
  memoryType: 'other',
  importance: 5,
  keywords: ''
});

const memoryTypeText = (type) => memoryTypeMap[type]?.text || type || '其他';
const memoryTypeColor = (type) => memoryTypeMap[type]?.color || 'default';

const importanceColor = (importance) => {
  if (importance >= 8) return 'red';
  if (importance >= 5) return 'orange';
  return 'blue';
};

// 内容截断：超过 100 字以 ... 结尾，全文通过悬停 tooltip 查看
const briefContent = (text) => {
  const t = String(text || '').trim();
  return t.length > 100 ? t.slice(0, 100) + '...' : t;
};

const formatTime = (time) => {
  if (!time) return '-';
  const d = new Date(time);
  if (Number.isNaN(d.getTime())) return time;
  const pad = (n) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
};

// 左栏数据：并发拉取“开启记忆的智能体”与记忆计数，AIAppsId=0 且有计数时追加“全局记忆”节点
const loadLeftPanel = async () => {
  const [appsRes, countsRes] = await Promise.all([
    getMyAIAppsALLList(),
    getMemoryCountsByAIApps()
  ]);
  const apps = appsRes && appsRes.code === 200 ? (appsRes.data || []) : [];
  const counts = countsRes && countsRes.code === 200 ? (countsRes.data || []) : [];
  const countMap = {};
  counts.forEach(item => {
    countMap[item.aiAppsId] = item.count;
  });
  const list = apps
    .filter(item => item.isMemory === true && countMap[item.id] > 0)
    .map(item => ({
      aiAppsId: item.id,
      name: item.name,
      count: countMap[item.id] || 0
    }));
  if (countMap[0] > 0) {
    list.push({ aiAppsId: 0, name: '全局记忆', count: countMap[0] });
  }
  agentMemoryList.value = list;
  // 默认选中第一项
  if (list.length > 0 && !list.some(item => item.aiAppsId === selectedAIAppsId.value)) {
    selectedAIAppsId.value = list[0].aiAppsId;
  }
};

const loadMemoryData = async () => {
  tableLoading.value = true;
  try {
    const range = importanceRangeMap[importanceFilter.value];
    const params = {
      aiAppsId: selectedAIAppsId.value,
      searchKey: searchKeyword.value || '',
      // dtoPagePar<string> 的 Parameter 为非空引用类型，模型绑定缺省必填，需显式传空串（与 SkillToolManagement 等既有页面口径一致）
      parameter: '',
      pageNum: pagination.current,
      pageSize: pagination.pageSize
    };
    if (range) {
      params.importanceMin = range.min;
      params.importanceMax = range.max;
    }
    const response = await getMemoryPageData(params);
    if (response && response.code === 200 && response.data) {
      memoryList.value = (response.data.data || []).map(item => ({
        ...item,
        key: item.id
      }));
      pagination.total = response.data.total || 0;
    }
  } catch (error) {
    console.error('加载记忆数据失败:', error);
    message.error('加载记忆数据失败: ' + (error.message || '未知错误'));
  } finally {
    tableLoading.value = false;
  }
};

const selectAgent = (aiAppsId) => {
  selectedAIAppsId.value = aiAppsId;
  pagination.current = 1;
  loadMemoryData();
};

const handleFilterChange = () => {
  pagination.current = 1;
  loadMemoryData();
};

const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadMemoryData();
};

// 仅编辑：记忆由 AI 自动沉淀，页面不提供新增入口；
// Content/Keywords 可编辑，Importance/MemoryType 只读展示
const openEditModal = (record) => {
  editForm.id = record.id;
  editForm.aiAppsId = record.aiAppsId;
  editForm.content = record.content;
  editForm.memoryType = record.memoryType || 'other';
  editForm.importance = record.importance;
  editForm.keywords = record.keywords;
  editModalVisible.value = true;
};

const handleSave = async () => {
  if (!editForm.content || !editForm.content.trim()) {
    message.error('记忆内容不能为空');
    return;
  }
  saving.value = true;
  try {
    // 提交编辑后的 Content/Keywords；其余归属/评估字段透传原值，
    // 防止服务端 AddEdit 的实体映射把未提交字段覆盖为默认值
    const payload = {
      id: editForm.id,
      aiAppsId: editForm.aiAppsId,
      content: editForm.content.trim(),
      memoryType: editForm.memoryType,
      importance: editForm.importance,
      keywords: editForm.keywords || ''
    };
    const response = await addEditMemory(payload);
    if (response && response.code === 200) {
      message.success('记忆已更新');
      editModalVisible.value = false;
      await Promise.all([loadLeftPanel(), loadMemoryData()]);
    } else {
      message.error(response?.msg || '保存失败');
    }
  } catch (error) {
    console.error('保存记忆失败:', error);
    message.error('保存记忆失败: ' + (error.message || '未知错误'));
  } finally {
    saving.value = false;
  }
};

const confirmDelete = (record) => {
  Modal.confirm({
    title: '删除记忆',
    content: '确定删除该条记忆吗？删除后不再参与召回。',
    okText: '删除',
    okType: 'danger',
    cancelText: '取消',
    onOk: () => handleDelete(record)
  });
};

const handleDelete = async (record) => {
  try {
    const response = await deleteMemory(record.id);
    if (response && response.code === 200) {
      message.success('记忆已删除');
      await Promise.all([loadLeftPanel(), loadMemoryData()]);
    } else {
      message.error(response?.msg || '删除失败');
    }
  } catch (error) {
    console.error('删除记忆失败:', error);
    message.error('删除记忆失败: ' + (error.message || '未知错误'));
  }
};

onMounted(async () => {
  try {
    await loadLeftPanel();
    await loadMemoryData();
  } catch (error) {
    console.error('初始化页面数据失败:', error);
    message.error('初始化页面数据失败: ' + (error.message || '未知错误'));
  }
});
</script>

<style scoped>
.management-container {
  padding: 24px;
}

.management-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 16px;
  font-weight: 500;
}

.title-icon {
  font-size: 20px;
  color: var(--accent);
}

.memories-layout {
  display: flex;
  gap: 16px;
  min-height: 400px;
}

.agent-panel {
  width: 240px;
  flex-shrink: 0;
  border-right: 1px solid #f0f0f0;
  padding-right: 16px;
}

.agent-panel-title {
  font-size: 14px;
  font-weight: 500;
  color: rgba(0, 0, 0, 0.65);
  margin-bottom: 12px;
}

.agent-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 12px;
  border-radius: 6px;
  cursor: pointer;
  margin-bottom: 4px;
  transition: background-color 0.2s;
}

.agent-item:hover {
  background-color: #f5f5f5;
}

.agent-item.active {
  background-color: #e6f4ff;
}

.agent-item-icon {
  color: var(--accent);
}

.agent-item-name {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.memory-panel {
  flex: 1;
  min-width: 0;
}

.toolbar {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 16px;
}

.toolbar-item {
  flex-shrink: 0;
}

.content-cell {
  white-space: pre-wrap;
  word-break: break-all;
}

.keywords-cell {
  color: rgba(0, 0, 0, 0.65);
}

.memory-term-cell {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 2px;
}

.expire-time {
  font-size: 11px;
  color: #999;
  line-height: 14px;
  white-space: nowrap;
}

.danger-link {
  color: #ff4d4f;
}

.pagination-container {
  margin-top: 24px;
  display: flex;
  justify-content: flex-end;
}

.importance-tip {
  margin-left: 8px;
  font-size: 12px;
  color: rgba(0, 0, 0, 0.45);
}
</style>
