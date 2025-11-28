<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <ApartmentOutlined class="title-icon" />
            <span>岗位管理</span>
          </div>
        </div>
      </template>
      
      <div class="layout-container">
        <!-- 左侧岗位树 -->
        <div class="left-panel">
          <!-- <div class="panel-header">
            <h3>组织架构</h3>
            <div class="header-actions">
              <a-button type="primary" size="small" @click="showAddPositionModal(0)">
                <template #icon>
                  <PlusOutlined />
                </template>
                添加根岗位
              </a-button>
            </div>
          </div> -->
          
          <div class="org-chart-container">
            <a-spin :spinning="loading">
              <a-directory-tree
                :tree-data="orgTreeData"
                :expandedKeys="expandedKeys"
                :selectedKeys="selectedKeys"
                @expand="onExpand"
                @select="onSelect"
              >
                <template #title="{ title, code, status, id }">
                  <div class="org-node-content">
                    <div class="org-node-info">
                      <span class="org-node-name">{{ title }}</span>
                      <span class="org-node-code">{{ code }}</span>
                    </div>
                    <div class="org-node-status">
                      <a-tag :color="status === 1 ? 'green' : 'red'" class="org-status-tag">
                        {{ status === 1 ? '启用' : '禁用' }}
                      </a-tag>
                    </div>
                    <div class="org-node-actions">
                      <a-button type="link" size="small" @click.stop="showAddPositionModal(id)" title="添加下级岗位">
                        <template #icon>
                          <PlusOutlined />
                        </template>
                      </a-button>
                      <a-button type="link" size="small" @click.stop="showEditPositionModalById(id)" title="编辑岗位">
                        <template #icon>
                          <EditOutlined />
                        </template>
                      </a-button>
                      <a-popconfirm
                        title="确定要删除这个岗位吗?"
                        ok-text="确定"
                        cancel-text="取消"
                        @confirm.stop="handleDelete(id)"
                      >
                        <a-button type="link" size="small" danger title="删除岗位">
                          <template #icon>
                            <DeleteOutlined />
                          </template>
                        </a-button>
                      </a-popconfirm>
                    </div>
                  </div>
                </template>
              </a-directory-tree>
            </a-spin>
          </div>
        </div>
        
        <!-- 右侧用户管理 -->
        <div class="right-panel">
          <UserManagement 
            ref="userManagementRef"
            title="用户管理"
            :query-params="userQueryParams"
            :auto-load="!!selectedPositionId"
          />
        </div>
      </div>
    </a-card>
    
    <!-- 添加/编辑岗位模态框 -->
    <a-modal v-model:open="positionModalVisible" :title="positionModalTitle" @ok="handlePositionModalOk" @cancel="handlePositionModalCancel" width="500px">
      <a-form :model="positionForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="岗位名称" v-bind="validateInfos.name">
          <a-input v-model:value="positionForm.name" placeholder="请输入岗位名称" />
        </a-form-item>
        <a-form-item label="岗位编码" v-bind="validateInfos.code">
          <a-input v-model:value="positionForm.code" placeholder="请输入岗位编码" />
        </a-form-item>
        <a-form-item label="上级岗位">
          <a-tree-select
            v-model:value="positionForm.parentId"
            show-search
            placeholder="请选择上级岗位"
            :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }
"
            :tree-data="parentPositions"
            :filterTreeNode="filterTreeNode"
            style="width: 100%"
          />
        </a-form-item>
        <a-form-item label="排序">
          <a-input-number v-model:value="positionForm.sort" :min="0" :precision="0" style="width: 100%" />
        </a-form-item>
        <a-form-item label="状态">
          <a-radio-group v-model:value="positionForm.status">
            <a-radio :value="1">启用</a-radio>
            <a-radio :value="-1">禁用</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="岗位描述">
          <a-textarea v-model:value="positionForm.description" :rows="3" placeholder="请输入岗位描述" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import '../../css/UserRole.css';
import '../../css/MyTable.css';
import '../../css/buttons.css';
import '../../css/management.css';
import { ref, reactive, onMounted, computed } from 'vue';
import { 
  ApartmentOutlined,
  PlusOutlined, 
  DeleteOutlined,
  EditOutlined
} from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { getPositionTree, addEditPosition, deletePosition } from '@/api/organizational/position';
import UserManagement from '@/components/UserManagement.vue';

const useForm = Form.useForm;

// 岗位数据
const dataSource = ref([]);
// 组织架构树数据
const orgTreeData = ref([]);
// 树节点展开的key
const expandedKeys = ref([]);
// 树节点选中的key
const selectedKeys = ref([]);
// 加载状态
const loading = ref(false);

// 模态框状态
const positionModalVisible = ref(false);
const positionModalTitle = ref('添加岗位');

// 当前编辑的岗位
const currentPosition = ref(null);

// 上级岗位选项
const parentPositions = ref([{ value: 0, title: '无上级岗位', key: 0 }]);

// 用户管理引用
const userManagementRef = ref(null);

// 选中的岗位ID
const selectedPositionId = ref(null);

// 用户查询参数
const userQueryParams = computed(() => {
  return selectedPositionId.value ? { Parameter:{positionId: selectedPositionId.value }} : {};
});

// 岗位表单
const positionForm = reactive({
  id: '',
  name: '',
  code: '',
  parentId: 0,
  sort: 0,
  status: 1,
  description: ''
});

// 表单验证规则
const positionRules = reactive({
  name: [
    { required: true, message: '请输入岗位名称' },
    { min: 2, message: '岗位名称至少2个字符' }
  ],
  code: [
    { required: true, message: '请输入岗位编码' }
  ]
});

// 表单验证
const { validate: validatePositionForm, validateInfos } = useForm(positionForm, positionRules);
 

// 树节点展开事件
const onExpand = (keys) => {
  expandedKeys.value = keys;
};

// 树节点选择事件
const onSelect = (keys) => {
  selectedKeys.value = keys;
  if (keys && keys.length > 0) {
    selectedPositionId.value = keys[0];
  } else {
    selectedPositionId.value = null;
  }
};

// 通过ID查找岗位数据
const findPositionById = (id, data) => {
  if (!data || !Array.isArray(data)) return null;
  
  for (let item of data) {
    if (item && item.id === id) {
      return item;
    }
    if (item && item.children && item.children.length > 0) {
      const found = findPositionById(id, item.children);
      if (found) return found;
    }
  }
  return null;
};

// 通过ID显示编辑岗位模态框
const showEditPositionModalById = (id) => {
  const position = findPositionById(id, dataSource.value);
  if (position) {
    showEditPositionModal(position);
  }
};

// 加载上级岗位数据
const loadParentPositions = async () => {
  try {
    const response = await getPositionTree();
    if (response && response.data.code == 200 && response.data.data) {
      // 转换数据格式以适应TreeSelect组件
      const convertToTreeData = (data) => {
        if (!data) return [];
        
        // 递归转换节点
        const traverse = (node) => {
          if (!node) return null;
          
          const convertedNode = {
            value: node.id,
            title: `${node.name} - ${node.code}`,
            key: node.id
          };
          
          // 处理子节点
          if (node.children && node.children.length > 0) {
            convertedNode.children = node.children.map(child => traverse(child)).filter(child => child !== null);
          }
          
          return convertedNode;
        };
        
        // 根节点
        const rootNode = traverse(data);
        if (!rootNode) return [];
        
        // 构造结果数组
        const result = [{
          value: 0,
          title: '无上级岗位',
          key: 0
        }, rootNode];
        
        return result;
      };
      
      parentPositions.value = convertToTreeData(response.data.data);
    }
  } catch (error) {
    console.error('加载上级岗位数据失败:', error);
    message.error('加载上级岗位数据失败: ' + (error.message || '未知错误'));
  }
};

// 转换数据为组织架构树
const convertToOrgTreeData = (data) => {
  if (!data) return [];
  
  const traverse = (node) => {
    if (!node) return null;
    
    const treeNode = {
      title: node.name,
      key: node.id,
      code: node.code,
      status: node.status,
      id: node.id,
      children: []
    };
    
    if (node.children && node.children.length > 0) {
      treeNode.children = node.children.map(child => traverse(child)).filter(child => child !== null);
    }
    
    return treeNode;
  };
  
  const root = traverse(data);
  return root ? [root] : [];
};

// 加载岗位数据
const loadPositionData = async () => {
  try {
    loading.value = true;
    const response = await getPositionTree();
    if (response && response.data.code == 200 && response.data) {   
      dataSource.value=[response.data.data];
      // 转换为组织架构树数据
      orgTreeData.value = convertToOrgTreeData(response.data.data);
      
      // 默认展开根节点
      if (response.data && response.data.data.id) {
        expandedKeys.value = [response.data.data.id];
      }
    }
  } catch (error) {
    console.error('加载岗位数据失败:', error);
    message.error('加载岗位数据失败: ' + (error.message || '未知错误'));
  } finally {
    loading.value = false;
  }
};

// 显示添加岗位模态框
const showAddPositionModal = (parentId) => {
  positionModalTitle.value = parentId === 0 ? '添加根岗位' : '添加下级岗位';
  currentPosition.value = null;
  // 重置表单
  Object.assign(positionForm, {
    id: '',
    name: '',
    code: '',
    parentId: parentId,
    sort: 0,
    status: 1,
    description: ''
  });
  positionModalVisible.value = true;
};

// 显示编辑岗位模态框
const showEditPositionModal = (record) => {
  positionModalTitle.value = '编辑岗位';
  currentPosition.value = record;
  // 填充表单数据
  Object.assign(positionForm, {
    id: record.id,
    name: record.name,
    code: record.code,
    parentId: record.parentId || 0,
    sort: record.sort,
    status: record.status,
    description: record.description
  });
  positionModalVisible.value = true;
};
 
// 删除岗位
const handleDelete = async (id) => {
  try {
    await deletePosition(id);
    message.success('岗位删除成功');
    loadPositionData(); // 重新加载数据
    loadParentPositions(); // 重新加载上级岗位数据
  } catch (error) {
    console.error('删除岗位失败:', error);
    message.error('删除岗位失败: ' + (error.message || '未知错误'));
  }
};

// 岗位模态框确认
const handlePositionModalOk = () => {
  validatePositionForm().then(async () => {
    try {
     
      
      if (currentPosition.value) {
         const params = {
        id: positionForm.id,
        name: positionForm.name,
        code: positionForm.code,
        parentId: positionForm.parentId === 0 ? null : positionForm.parentId,
        sort: positionForm.sort,
        status: positionForm.status,
        description: positionForm.description
      };
      
      await addEditPosition(params);
        message.success('岗位信息更新成功');
      } else {
         const params = { 
        name: positionForm.name,
        code: positionForm.code,
        parentId: positionForm.parentId === 0 ? null : positionForm.parentId,
        sort: positionForm.sort,
        status: positionForm.status,
        description: positionForm.description
      };
      
      await addEditPosition(params);
        message.success('岗位信息添加成功');
      }
      
      positionModalVisible.value = false;
      loadPositionData(); // 重新加载数据
      loadParentPositions(); // 重新加载上级岗位数据
    } catch (error) {
      console.error('保存岗位失败:', error);
      message.error('保存岗位失败: ' + (error.message || '未知错误'));
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 岗位模态框取消
const handlePositionModalCancel = () => {
  positionModalVisible.value = false;
};

// TreeSelect过滤函数
const filterTreeNode = (inputValue, treeNode) => {
  return treeNode.title.toLowerCase().indexOf(inputValue.toLowerCase()) >= 0;
};

// 组件挂载时的初始化
onMounted(() => {
  loadPositionData();
  loadParentPositions();
});
</script>

<style scoped>
.layout-container {
  display: flex;
  height: calc(100vh - 200px);
  gap: 20px;
}

.left-panel {
  flex: 1;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
  padding-right: 20px;
  display: flex;
  flex-direction: column;
  background: transparent;
  color: rgba(255, 255, 255, 0.85);
}

.right-panel {
  flex: 2;
  overflow-y: auto;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.panel-header h3 {
  margin: 0;
  color: #7fdbff;
  font-size: 18px;
  font-weight: 500;
}

.org-chart-container {
  flex: 1;
  padding: 10px 0;
  background: transparent;
  border-radius: 8px;
  min-height: 500px;
  display: flex;
  flex-direction: column;
}

.org-chart-container :deep(.ant-tree) {
  flex: 1;
  overflow-y: auto;
  background: transparent;
  color: rgba(255, 255, 255, 0.85);
}

.org-node-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 8px 0;
}

.org-node-info {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 12px;
}

.org-node-name {
  font-weight: 500;
  color: #7fdbff;
}

.org-node-code {
  color: rgba(127, 219, 255, 0.85);
  font-size: 12px;
  background: rgba(22, 119, 255, 0.2);
  padding: 2px 6px;
  border-radius: 4px;
}

.org-node-status {
  margin-right: 16px;
}

.org-status-tag {
  font-size: 12px;
  border-radius: 8px;
}

.org-node-actions {
  display: flex;
  gap: 4px;
  opacity: 0;
  transition: opacity 0.3s;
}

.org-node-content:hover .org-node-actions {
  opacity: 1;
}

:deep(.ant-tree-treenode) {
  padding: 2px 0;
}

:deep(.ant-tree-switcher) {
  align-self: center;
  color: rgba(127, 219, 255, 0.5);
}

:deep(.ant-tree-node-content-wrapper) {
  display: flex;
  align-items: center;
  padding: 0 8px !important;
  border-radius: 6px;
  transition: all 0.3s;
  color: rgba(255, 255, 255, 0.85);
  background: transparent;
}

:deep(.ant-tree-node-content-wrapper:hover) {
  background: rgba(22, 119, 255, 0.1) !important;
}

:deep(.ant-tree-node-selected) {
  background: rgba(22, 119, 255, 0.2) !important;
}

:deep(.ant-tree-node-selected:hover) {
  background: rgba(22, 119, 255, 0.3) !important;
}

/* 添加按钮样式 */
:deep(.ant-btn-primary) {
  background: linear-gradient(45deg, #1677ff, #001529);
  border: none;
  border-radius: 6px;
  box-shadow: 0 4px 15px rgba(22, 119, 255, 0.3);
  transition: all 0.3s ease;
}

:deep(.ant-btn-primary:hover) {
  box-shadow: 0 6px 20px rgba(22, 119, 255, 0.5);
  transform: translateY(-2px);
}

:deep(.ant-btn-link) {
  color: #7fdbff;
}

:deep(.ant-btn-link:hover) {
  color: #4da6ff;
}

:deep(.ant-btn-dangerous) {
  color: #ff4d4f;
}

:deep(.ant-btn-sm) {
  font-size: 12px;
}

:deep(.ant-tag) {
  border-radius: 4px;
  font-size: 12px;
}

:deep(.ant-spin-nested-loading), 
:deep(.ant-spin-container) {
  height: 100%;
}
</style>
