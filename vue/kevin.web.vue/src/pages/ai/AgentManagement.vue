<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <RobotOutlined class="title-icon" />
            <span>智能体管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索智能体..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddAgentModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加智能体
            </a-button>
          </div>
        </div>
      </template>

      <div class="cards-container">
        <a-row :gutter="[24, 24]">
          <a-col :xs="24" :sm="12" :md="12" :lg="8" :xl="6" v-for="agent in agentList" :key="agent.id">
            <a-card class="agent-card" hoverable>
              <template #title>
                <div class="card-title">
                  <span class="agent-name">{{ agent.name }}</span>
                </div>
              </template>
              <template #extra>
                <a-dropdown>
                  <a class="ant-dropdown-link" @click.prevent>
                    <EllipsisOutlined />
                  </a>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item @click="showEditAgentModal(agent)">
                        <edit-outlined /> 编辑
                      </a-menu-item>
                      <a-menu-item @click="showDeleteConfirm(agent)">
                        <delete-outlined /> 删除
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </template>
              <div @click="showEditAgentModal(agent)" class="card-content">
                <div class="agent-icon">
                  <component :is="agent.icon" class="icon-element" />
                </div>
                <div class="agent-info">
                  <div class="info-item horizontal">
                    <span class="info-label">描述:</span>
                    <span class="info-value">{{ agent.describe }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">类型:</span>
                    <span class="info-value">{{ agent.type }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">温度:</span>
                    <div class="progress-wrapper">
                      <a-progress 
                        :percent="agent.temperature" 
                        :format="percent => `${percent}°`" 
                        size="small" 
                        :stroke-color="getTemperatureColor(agent.temperature)"
                      />
                    </div>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">相似度:</span>
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
            </a-card>
          </a-col>
        </a-row>
        
        <a-empty v-if="agentList.length === 0" description="暂无智能体数据" />
        
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

    <!-- 添加/编辑智能体模态框 -->
    <AgentAddEdit 
      :open="agentModalVisible"
      :modal-type="agentModalType"
      :agent-data="currentAgent"
      @ok="handleAgentModalOk"
      @cancel="handleAgentModalCancel"
      ref="agentAddEditRef"
    />
  </div>
</template>

<script setup> 
import '../../css/AgentManagement.css';
import { ref, reactive, onMounted } from 'vue';
import { 
  RobotOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined
} from '@ant-design/icons-vue';
import { message, Modal } from 'ant-design-vue';
import { 
  getAIAppsPageData, 
  addEditAIApp, 
  deleteAIApp 
} from '@/api/ai/aiapps';
import AgentAddEdit from '@/components/ai/AgentAddEdit.vue';

// 智能体数据
const agentList = ref([]);

// 分页配置
const pagination = reactive({
  current: 1,
  pageSize: 8,
  total: 0
});

// 模态框状态
const agentModalVisible = ref(false);
const agentModalType = ref('add'); // 'add' 或 'edit'
const confirmLoading = ref(false);

// 当前编辑的智能体
const currentAgent = ref(null);

// 搜索关键字
const searchKeyword = ref('');

// 获取温度颜色
const getTemperatureColor = (temp) => {
  if (temp < 30) return '#667eea';
  if (temp < 70) return '#764ba2';
  return '#f5222d';
};
 
  
// 获取相似度颜色
const getRelevanceColor = (relevance) => {
  if (relevance < 30) return '#f5222d';
  if (relevance < 70) return '#764ba2';
  return '#52c41a';
};

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.current = 1;
  loadAgentData();
};

// 加载智能体数据
const loadAgentData = async () => {
  try {
    const params = {
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
      searchKey: searchKeyword.value
    };

    const response = await getAIAppsPageData(params);
    if (response && response.code === 200 && response.data.data) {
      agentList.value = response.data.data.map(item => ({
        ...item,
        key: item.id
      }));
      pagination.total = response.data.total;
    }
  } catch (error) {
    console.error('加载智能体数据失败:', error);
    message.error('加载智能体数据失败: ' + (error.message || '未知错误'));
  }
};

// 显示添加智能体模态框
const showAddAgentModal = () => {
  agentModalType.value = 'add';
  currentAgent.value = null;
  agentModalVisible.value = true;
};

// 显示编辑智能体模态框
const showEditAgentModal = (record) => {
  agentModalType.value = 'edit';
  currentAgent.value = record;
  agentModalVisible.value = true;
};

// 删除智能体确认
const showDeleteConfirm = (record) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除智能体"${record.name}"吗？`,
    okText: '确认',
    cancelText: '取消',
    onOk: () => handleDelete(record.id, record.name)
  });
};

// 删除智能体
const handleDelete = async (id, name) => {
  try {
    await deleteAIApp(id);
    message.success(`智能体"${name}"删除成功`);
    loadAgentData(); // 重新加载数据
  } catch (error) {
    console.error('删除智能体失败:', error);
    message.error('删除智能体失败: ' + (error.message || '未知错误'));
  }
};

// 分页变化处理
const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadAgentData();
};

import { GetSnowflakeId } from "../../api/baseapi"; 
// 智能体模态框确认
const handleAgentModalOk = async (params) => {
  confirmLoading.value = true;
  try {
    if(!(agentModalType.value === 'edit')){
    params.id= await GetSnowflakeId().data;
    } 
    await addEditAIApp(params);
    
    message.success(agentModalType.value === 'edit' ? '智能体信息更新成功' : '智能体信息添加成功');
    
    agentModalVisible.value = false;
    loadAgentData(); // 重新加载数据
  } catch (error) {
    console.error('保存智能体失败:', error);
    message.error('保存智能体失败: ' + (error.message || '未知错误'));
  } finally {
    confirmLoading.value = false;
  }
};

// 智能体模态框取消
const handleAgentModalCancel = () => {
  agentModalVisible.value = false;
};

// 引用子组件实例
const agentAddEditRef = ref();

// 组件挂载时的初始化
onMounted(() => {
  console.log('智能体管理页面已加载');
  loadAgentData();
});
</script>