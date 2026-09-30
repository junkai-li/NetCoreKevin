<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <RobotOutlined class="title-icon" />
            <span>我的可用智能体</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索智能体..."
              style="width: 250px"
              @search="onSearch"
            />
          </div>
        </div>
      </template>

      <div class="cards-container">
        <div class="octop-card-grid">
          <div class="octop-card" v-for="agent in agentList" :key="agent.id">
            <div class="octop-card-accent"></div>
            <div class="octop-card-header">
              <div class="octop-card-icon">
                <RobotOutlined />
              </div>
              <div class="octop-card-title-block">
                <div class="octop-card-name">{{ agent.name }}</div>
                <div class="octop-card-meta">
                  <span class="octop-card-pill" v-if="agent.type">{{ agent.type }}</span>
                </div>
              </div>
            </div>
            <div class="octop-card-body" @click="showPreviewModal(agent)">
              <div class="octop-card-desc">{{ agent.describe }}</div>
              <div class="octop-card-rows">
                <div class="octop-card-row">
                  <span class="octop-card-row-label">温度</span>
                  <div class="progress-wrapper">
                    <a-progress
                      :percent="agent.temperature"
                      :format="percent => `${percent}°`"
                      size="small"
                      :stroke-color="getTemperatureColor(agent.temperature)"
                    />
                  </div>
                </div>
                <div class="octop-card-row">
                  <span class="octop-card-row-label">相似度</span>
                  <div class="progress-wrapper">
                    <a-progress
                      :percent="agent.relevance"
                      :format="percent => `${percent}%`"
                      size="small"
                      :stroke-color="getRelevanceColor(agent.relevance)"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <a-empty v-if="agentList.length === 0" description="暂无可用智能体数据" />

        <div class="pagination-container" v-if="agentList.length > 0">
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
    </a-card>

    <AgentAddEdit
      :open="agentModalVisible"
      modal-type="view"
      :agent-data="currentAgent"
      hide-bind-tab
      @ok="handleAgentModalOk"
      @cancel="handleAgentModalCancel"
    />
  </div>
</template>

<script setup>
import '../../css/CardTable.css';
import { ref, reactive, onMounted } from 'vue';
import {
  RobotOutlined
} from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import {
  getMyAIAppsALLList,
  getAIAppsDetails
} from '@/api/ai/aiapps';
import AgentAddEdit from '@/components/ai/AgentAddEdit.vue';

const agentList = ref([]);

const pagination = reactive({
  current: 1,
  pageSize: 8,
  total: 0
});

const agentModalVisible = ref(false);
const currentAgent = ref(null);
const searchKeyword = ref('');

const getTemperatureColor = (temp) => {
  if (temp < 30) return '#667eea';
  if (temp < 70) return '#764ba2';
  return '#f5222d';
};

const getRelevanceColor = (relevance) => {
  if (relevance < 30) return '#f5222d';
  if (relevance < 70) return '#764ba2';
  return '#52c41a';
};

const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.current = 1;
  loadAgentData();
};

const loadAgentData = async () => {
  try {
    const response = await getMyAIAppsALLList();
    if (response && response.code === 200) {
      let data = response.data || [];
      if (searchKeyword.value) {
        data = data.filter(item =>
          item.name?.toLowerCase().includes(searchKeyword.value.toLowerCase()) ||
          item.describe?.toLowerCase().includes(searchKeyword.value.toLowerCase())
        );
      }
      pagination.total = data.length;
      const start = (pagination.current - 1) * pagination.pageSize;
      const end = start + pagination.pageSize;
      agentList.value = data.slice(start, end).map(item => ({
        ...item,
        key: item.id
      }));
    }
  } catch (error) {
    console.error('加载智能体数据失败:', error);
    message.error('加载智能体数据失败: ' + (error.message || '未知错误'));
  }
};

const showPreviewModal = async (record) => {
  try {
    const response = await getAIAppsDetails(record.id);
    if (response && response.code === 200) {
      currentAgent.value = response.data;
    }
  } catch (error) {
    console.error('获取智能体详情失败:', error);
    message.error('获取智能体详情失败');
  }
  agentModalVisible.value = true;
};

const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadAgentData();
};

const handleAgentModalOk = () => {
  agentModalVisible.value = false;
};

const handleAgentModalCancel = () => {
  agentModalVisible.value = false;
};

onMounted(() => {
  loadAgentData();
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

.header-actions {
  display: flex;
  align-items: center;
}

.cards-container {
  min-height: 400px;
}

.pagination-container {
  margin-top: 24px;
  display: flex;
  justify-content: flex-end;
}
</style>
