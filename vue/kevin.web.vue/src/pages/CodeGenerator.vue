<template>
  <div class="code-generator-container">
    <a-card class="code-generator-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <CodeOutlined class="title-icon" />
            <span>代码生成器</span>
          </div>
        </div>
      </template>

      <div class="generator-content">
        <!-- 第一步：选择区域 -->
        <div class="step-section">
          <h3>第一步：选择区域</h3>
          <a-select
            v-model:value="selectedArea"
            placeholder="请选择一个区域"
            style="width: 300px;"
            @change="onAreaChange"
          >
            <a-select-option v-for="area in areas" :key="area" :value="area">
              {{ area }}
            </a-select-option>
          </a-select>
          <p class="step-desc">当前已选择区域: <strong>{{ selectedArea || '尚未选择' }}</strong></p>
        </div>

        <!-- 第二步：选择实体 -->
        <div class="step-section" v-if="selectedArea">
          <h3>第二步：选择实体</h3>
          <div class="entity-actions" v-if="selectedEntities.length > 0">
            <a-button 
              type="primary" 
              @click="generateCode" 
              :loading="generateLoading"
              style="margin-right: 16px;"
            >
              为选中的 {{ selectedEntities.length }} 个实体生成代码
            </a-button>
            <a-button @click="clearSelection">
              清空选择
            </a-button>
          </div>
          <a-spin :spinning="entitiesLoading">
            <a-table
              :columns="entityColumns"
              :data-source="entities"
              :pagination="false"
              row-key="entityName"
              :row-selection="rowSelection"
            >
            </a-table>
          </a-spin>
          <p class="step-desc" v-if="selectedEntities.length > 0">已选择 {{ selectedEntities.length }} 个实体</p>
        </div>
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { message } from 'ant-design-vue';
import { CodeOutlined } from '@ant-design/icons-vue';
import { GetAreaNames, GetAreaNameEntityItems, BulidCode } from '../api/code_generator';

// 响应式数据
const areas = ref([]);
const selectedArea = ref('');
const entities = ref([]);
const selectedEntities = ref([]);
const entitiesLoading = ref(false);
const generateLoading = ref(false);

// 表格列定义（移除操作列）
const entityColumns = [
  {
    title: '实体名称',
    dataIndex: 'entityName',
    key: 'entityName',
    width: '30%',
  },
  {
    title: '描述',
    dataIndex: 'description',
    key: 'description',
    ellipsis: true,
  },
];

// 行选择配置
const rowSelection = {
  selectedRowKeys: computed(() => selectedEntities.value.map(item => item.entityName)),
  onChange: (selectedRowKeys, selectedRows) => {
    selectedEntities.value = selectedRows;
  },
  type: 'checkbox',
  columnWidth: '40px',
};

// 获取区域列表
const loadAreas = async () => {
  try {
    const response = await GetAreaNames();
    if (response.isSuccess) {
      areas.value = response.data || [];
      if (areas.value.length > 0) {
        message.info('已加载可用区域列表');
        // 默认选择第一个区域
        selectedArea.value = areas.value[0];
        // 加载第一个区域的实体
        await onAreaChange(areas.value[0]);
      } else {
        message.warning('暂无可用区域');
      }
    } else {
      message.error('获取区域列表失败');
    }
  } catch (error) {
    console.error('获取区域列表错误:', error);
    message.error('获取区域列表失败');
  }
};

// 区域选择变化
const onAreaChange = async (areaName) => {
  selectedEntities.value = [];
  entitiesLoading.value = true;
  
  try {
    const response = await GetAreaNameEntityItems(areaName);
    if (response.isSuccess) {
      entities.value = response.data || [];
      message.info(`已加载区域 ${areaName} 下的 ${entities.value.length} 个实体`);
    } else {
      message.error('获取实体列表失败');
      entities.value = [];
    }
  } catch (error) {
    console.error('获取实体列表错误:', error);
    message.error('获取实体列表失败');
    entities.value = [];
  } finally {
    entitiesLoading.value = false;
  }
};

// 清空选择
const clearSelection = () => {
  selectedEntities.value = [];
  message.info('已清空选择');
};

// 生成代码
const generateCode = async () => {
  if (selectedEntities.value.length === 0) {
    message.warning('请至少选择一个实体');
    return;
  }

  generateLoading.value = true;
  
  try { 
      try {
        const response = await BulidCode(selectedEntities.value);
        
        if (response.isSuccess && response.data === true) {
          message.success(`已生成 ${selectedEntities.value.length*4} 个代码文件`);
        } else {
          message.error('代码生成失败');
        }
      } catch (error) {
        console.error(`生成实体   代码错误:`, error); 
      }  
    
    // 生成完成后清空选择
    selectedEntities.value = [];
  } catch (error) {
    console.error('生成代码错误:', error);
    message.error('代码生成失败');
  } finally {
    generateLoading.value = false;
  }
};

// 组件挂载时加载区域列表
onMounted(() => {
  loadAreas();
});
</script>

<style scoped>
@import "../css/management.css";

.code-generator-container {
  padding: 16px;
  min-height: 100%;
}

.code-generator-card {
  background: #fff;
  border-radius: 8px;
  border: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
  overflow: hidden;
  height: 100%;
  display: flex;
  flex-direction: column;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.03);
}

.code-generator-card .ant-card-head{
  border-bottom: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
  padding: 0 20px;
  background: #fafafa;
}

.code-generator-card .ant-card-head-title{
  color: rgba(0, 0, 0, 0.88);
  padding: 16px 0;
}

.generator-content {
  padding: 16px 0;
  color: rgba(0, 0, 0, 0.88);
}

.step-section {
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.88);
}

.step-section:last-child {
  border-bottom: none;
}

.step-section h3 {
  margin-bottom: 16px;
  color: rgba(0, 0, 0, 0.88);
  font-size: 16px;
}

.step-desc {
  margin-top: 12px;
  color: rgba(0, 0, 0, 0.65);
  font-style: italic;
}

.entity-info {
  margin-bottom: 16px;
  padding: 12px;
  background: #fafafa;
  border-radius: 4px;
  color: rgba(0, 0, 0, 0.88);
}

.entity-info p {
  margin: 8px 0;
  color: rgba(0, 0, 0, 0.65);
}

.entity-actions {
  margin-bottom: 16px;
}

:deep(.ant-select-selector) {
  background: #fff !important;
  border: 1px solid #d9d9d9 !important;
  color: rgba(0, 0, 0, 0.88) !important;
}

:deep(.ant-select-selection-item) {
  color: rgba(0, 0, 0, 0.88) !important;
}

:deep(.ant-table) {
  background: #fff;
  color: rgba(0, 0, 0, 0.88);
}

:deep(.ant-table-thead > tr > th) {
  background: #fafafa;
  color: rgba(0, 0, 0, 0.88);
  border-bottom: 1px solid #f0f0f0;
}

:deep(.ant-table-tbody > tr > td) {
  border-bottom: 1px solid #f0f0f0;
  color: rgba(0, 0, 0, 0.65);
}

:deep(.ant-table-tbody > tr:hover > td) {
  background: #fafafa !important;
}

:deep(.ant-table-tbody > tr.ant-table-row-selected > td) {
  background: #e6f4ff !important;
}

:deep(.ant-table-wrapper) {
  text-align: left;
}

:deep(.ant-btn-primary) {
  background: #1677ff;
  border: 1px solid #1677ff;
}

:deep(.ant-btn-primary:hover) {
  background: #0958d9;
  border: 1px solid #0958d9;
}

:deep(.ant-btn) {
  background: #fff;
  border: 1px solid #d9d9d9;
  color: rgba(0, 0, 0, 0.88);
}

:deep(.ant-btn:hover) {
  background: #f5f5f5;
  border-color: #d9d9d9;
  color: rgba(0, 0, 0, 0.88);
}

:deep(.ant-tag) {
  color: rgba(0, 0, 0, 0.88);
  background: #fafafa;
  border: 1px solid #d9d9d9;
}
</style>