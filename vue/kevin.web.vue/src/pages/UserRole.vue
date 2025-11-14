<template>
  <div class="role-management-container">
    <a-card class="role-management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <SafetyCertificateOutlined class="title-icon" />
            <span>角色管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索角色..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <Button type="primary" class="add-button" @click="showAddRoleModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加角色
            </Button>
          </div>
        </div>
      </template>
      
      <div class="toolbar">
        <div class="toolbar-left">
         <Flex gap="small" wrap>
          <Button @click="handleBatchDelete" type="primary"><template #icon>
              <DeleteOutlined />
            </template>批量删除</Button> 
        </Flex> 
        </div>
      </div>
      
      <div class="table-container">
        <a-table 
          :columns="columns" 
          :data-source="dataSource" 
          :pagination="pagination"
          :row-selection="rowSelection"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'permissions'">
              <a-popover trigger="click" placement="left">
                <template #content>
                  <div class="permissions-popover">
                    <div v-for="perm in record.permissions" :key="perm" class="permission-item">
                      <CheckCircleOutlined class="permission-icon" />
                      {{ perm }}
                    </div>
                  </div>
                </template>
                <a-tag color="processing">{{ record.permissions.length }} 个权限</a-tag>
              </a-popover>
            </template>
            
            <template v-else-if="column.dataIndex === 'status'">
              <a-tag :color="record.status === 1 ? 'success' : 'error'">
                {{ record.status === 1 ? '启用' : '禁用' }}
              </a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'users'">
              <a-tag color="blue">{{ record.users }} 个用户</a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showEditRoleModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-button type="link" size="small" @click="showPermissionModal(record)">
                  <template #icon>
                    <KeyOutlined />
                  </template>
                  权限
                </a-button>
                <a-popconfirm
                  title="确定要删除这个角色吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  @confirm="handleDelete(record.key)"
                >
                  <a-button type="link" size="small" danger>
                    <template #icon>
                      <DeleteOutlined />
                    </template>
                    删除
                  </a-button>
                </a-popconfirm>
              </div>
            </template>
          </template>
        </a-table>
      </div>
    </a-card>
    
    <!-- 添加/编辑角色模态框 -->
    <a-modal v-model:open="roleModalVisible" :title="roleModalTitle" @ok="handleRoleModalOk" @cancel="handleRoleModalCancel">
      <a-form :model="roleForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="角色名称" v-bind="validateInfos.name">
          <a-input v-model:value="roleForm.name" />
        </a-form-item>
        <a-form-item label="角色代码" v-bind="validateInfos.code">
          <a-input v-model:value="roleForm.code" />
        </a-form-item>
        <a-form-item label="描述">
          <a-textarea v-model:value="roleForm.description" :rows="3" />
        </a-form-item>
        <a-form-item label="状态">
          <a-switch v-model:checked="roleForm.status" checked-children="启用" un-checked-children="禁用" />
        </a-form-item>
      </a-form>
    </a-modal>
    
    <!-- 权限配置模态框 -->
    <a-modal v-model:open="permissionModalVisible" title="权限配置" width="600px" @ok="handlePermissionModalOk" @cancel="handlePermissionModalCancel">
      <div class="permission-tree">
        <a-tree
          v-model:checkedKeys="checkedPermissionKeys"
          :tree-data="permissionTreeData"
          checkable
          :expandedKeys="expandedPermissionKeys"
          @expand="onPermissionTreeExpand"
        >
          <template #title="{ title, icon }">
            <span>
              <component :is="icon" v-if="icon" class="permission-node-icon" />
              {{ title }}
            </span>
          </template>
        </a-tree>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { 
  SafetyCertificateOutlined, 
  PlusOutlined, 
  DeleteOutlined,
  EditOutlined,
  KeyOutlined,
  CheckCircleOutlined
} from '@ant-design/icons-vue';
import { message, Modal,Button,Flex  } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import '../css/UserRole.css';

const useForm = Form.useForm;

// 角色数据
const dataSource = ref([
  {
    key: '1',
    name: '管理员',
    code: 'admin',
    description: '系统管理员，拥有所有权限',
    status: 1,
    permissions: ['用户管理', '角色管理', '权限管理', '系统配置', '日志管理'],
    users: 1
  },
  {
    key: '2',
    name: '普通用户',
    code: 'user',
    description: '普通用户，拥有基本权限',
    status: 1,
    permissions: ['查看信息', '个人设置'],
    users: 12
  },
  {
    key: '3',
    name: '审计员',
    code: 'auditor',
    description: '审计员，拥有查看日志和报表权限',
    status: 1,
    permissions: ['日志管理', '数据分析', '报表查看'],
    users: 3
  },
  {
    key: '4',
    name: '操作员',
    code: 'operator',
    description: '操作员，拥有部分业务操作权限',
    status: 0,
    permissions: ['业务操作', '数据录入'],
    users: 5
  }
]);

// 表格列定义
const columns = ref([
  {
    title: '角色名称',
    dataIndex: 'name',
    key: 'name',
    sorter: true
  },
  {
    title: '角色代码',
    dataIndex: 'code',
    key: 'code'
  },
  {
    title: '描述',
    dataIndex: 'description',
    key: 'description'
  },
  {
    title: '权限',
    dataIndex: 'permissions',
    key: 'permissions'
  },
  {
    title: '用户数',
    dataIndex: 'users',
    key: 'users',
    sorter: true
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
    filters: [
      { text: '启用', value: 1 },
      { text: '禁用', value: 0 }
    ]
  },
  {
    title: '操作',
    dataIndex: 'action',
    key: 'action',
    fixed: 'right',
    width: 200
  }
]);

// 分页配置
const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 4,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
});

// 表格选择配置
const selectedRowKeys = ref([]);
const rowSelection = ref({
  selectedRowKeys: selectedRowKeys.value,
  onChange: (selectedKeys) => {
    selectedRowKeys.value = selectedKeys;
  }
});

// 模态框状态
const roleModalVisible = ref(false);
const permissionModalVisible = ref(false);
const roleModalTitle = ref('添加角色');

// 当前编辑的角色
const currentRole = ref(null);

// 角色表单
const roleForm = reactive({
  name: '',
  code: '',
  description: '',
  status: true
});

// 权限表单
const checkedPermissionKeys = ref(['user:list', 'user:create', 'user:edit']);
const expandedPermissionKeys = ref(['user', 'system']);

// 权限树数据
const permissionTreeData = ref([
  {
    title: '用户管理',
    key: 'user',
    icon: 'UserOutlined',
    children: [
      {
        title: '用户列表',
        key: 'user:list',
        icon: 'UnorderedListOutlined'
      },
      {
        title: '创建用户',
        key: 'user:create',
        icon: 'PlusOutlined'
      },
      {
        title: '编辑用户',
        key: 'user:edit',
        icon: 'EditOutlined'
      },
      {
        title: '删除用户',
        key: 'user:delete',
        icon: 'DeleteOutlined'
      }
    ]
  },
  {
    title: '系统管理',
    key: 'system',
    icon: 'SettingOutlined',
    children: [
      {
        title: '角色管理',
        key: 'role:list',
        icon: 'SafetyCertificateOutlined'
      },
      {
        title: '权限管理',
        key: 'permission:list',
        icon: 'KeyOutlined'
      },
      {
        title: '系统配置',
        key: 'system:config',
        icon: 'ControlOutlined'
      }
    ]
  },
  {
    title: '日志管理',
    key: 'log',
    icon: 'FileTextOutlined',
    children: [
      {
        title: '查看日志',
        key: 'log:view',
        icon: 'FileSearchOutlined'
      },
      {
        title: '导出日志',
        key: 'log:export',
        icon: 'DownloadOutlined'
      }
    ]
  }
]);

// 表单验证规则
const roleRules = reactive({
  name: [
    { required: true, message: '请输入角色名称' },
    { min: 2, message: '角色名称至少2个字符' }
  ],
  code: [
    { required: true, message: '请输入角色代码' },
    { pattern: /^[a-zA-Z0-9_]+$/, message: '角色代码只能包含字母、数字和下划线' }
  ]
});

// 表单验证
const { validate: validateRoleForm, validateInfos } = useForm(roleForm, roleRules);

// 搜索处理
const onSearch = (value) => {
  console.log('搜索:', value);
  message.info(`搜索角色: ${value}`);
};

// 显示添加角色模态框
const showAddRoleModal = () => {
  roleModalTitle.value = '添加角色';
  currentRole.value = null;
  // 重置表单
  Object.assign(roleForm, {
    name: '',
    code: '',
    description: '',
    status: true
  });
  roleModalVisible.value = true;
};

// 显示编辑角色模态框
const showEditRoleModal = (record) => {
  roleModalTitle.value = '编辑角色';
  currentRole.value = record;
  // 填充表单数据
  Object.assign(roleForm, {
    name: record.name,
    code: record.code,
    description: record.description,
    status: record.status === 1
  });
  roleModalVisible.value = true;
};

// 显示权限配置模态框
const showPermissionModal = (record) => {
  currentRole.value = record;
  permissionModalVisible.value = true;
};

// 权限树展开处理
const onPermissionTreeExpand = (expandedKeys) => {
  expandedPermissionKeys.value = expandedKeys;
};

// 角色模态框确认
const handleRoleModalOk = () => {
  validateRoleForm().then(() => {
    if (currentRole.value) {
      // 编辑角色
      message.success('角色信息更新成功');
    } else {
      // 添加角色
      message.success('角色添加成功');
    }
    roleModalVisible.value = false;
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 角色模态框取消
const handleRoleModalCancel = () => {
  roleModalVisible.value = false;
};

// 权限模态框确认
const handlePermissionModalOk = () => {
  message.success(`角色 ${currentRole.value.name} 权限配置成功`);
  permissionModalVisible.value = false;
};

// 权限模态框取消
const handlePermissionModalCancel = () => {
  permissionModalVisible.value = false;
};

// 删除角色
const handleDelete = (key) => {
  message.success('角色删除成功');
  console.log('删除角色:', key);
};

// 批量删除
const handleBatchDelete = () => {
  if (selectedRowKeys.value.length === 0) {
    message.warning('请先选择要删除的角色');
    return;
  }
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除选中的 ${selectedRowKeys.value.length} 个角色吗?`,
    okText: '确定',
    cancelText: '取消',
    onOk() {
      message.success('角色批量删除成功');
      selectedRowKeys.value = [];
    }
  });
};

// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log('表格变化:', pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('角色管理页面已加载');
});
</script>

<style scoped>
.role-management-container {
  color: white;
  height: 100%;
}

.role-management-card {
  background: rgba(255, 255, 255, 0.08);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  overflow: hidden;
  backdrop-filter: blur(10px);
  height: 100%;
  display: flex;
  flex-direction: column;
}

:deep(.role-management-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.role-management-card .ant-card-head-title) {
  color: white;
  padding: 16px 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.header-title {
  display: flex;
  align-items: center;
  font-size: 18px;
  font-weight: 500;
}

.title-icon {
  margin-right: 8px;
  color: #667eea;
}

.header-actions {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding: 12px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.toolbar-left {
  display: flex;
  gap: 8px;
}

.table-container {
  background: transparent;
  flex: 1;
  overflow: hidden;
}

:deep(.ant-table) {
  background: transparent;
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-table-thead > tr > th) {
  background: rgba(102, 126, 234, 0.2);
  color: white;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

:deep(.ant-table-tbody > tr) {
  background: transparent;
}

:deep(.ant-table-tbody > tr > td) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  color: rgba(255, 255, 255, 0.85);
  padding: 12px 8px;
  background: transparent;
  transition: background 0.3s ease; /* 添加过渡效果 */
}

:deep(.ant-table-tbody > tr:hover > td) {
  background: rgba(102, 126, 234, 0.1) !important;
}

:deep(.ant-table-tbody > tr.ant-table-row-selected > td) {
  background: rgba(102, 126, 234, 0.2) !important;
}

/* 防止悬停时的闪烁问题 */
:deep(.ant-table-tbody > tr.ant-table-row:hover > td) {
  background: rgba(102, 126, 234, 0.1) !important;
}

/* 防止鼠标移出时的闪烁问题 */
:deep(.ant-table-tbody > tr.ant-table-row) {
  background: transparent !important;
}

:deep(.ant-table-tbody > tr.ant-table-row > td) {
  background: transparent !important;
  transition: background 0.3s ease; /* 添加过渡效果 */
}

:deep(.ant-pagination) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-pagination-item) {
  background: transparent;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

:deep(.ant-pagination-item a) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-pagination-item-active) {
  background: #667eea;
  border-color: #667eea;
}

:deep(.ant-pagination-next),
:deep(.ant-pagination-prev) {
  color: rgba(255, 255, 255, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

:deep(.ant-btn-link) {
  color: #667eea;
}

:deep(.ant-btn-link:hover) {
  color: #764ba2;
}

.action-buttons {
  display: flex;
  gap: 8px;
  background: transparent;
}

.permissions-popover {
  max-height: 300px;
  overflow-y: auto;
}

.permission-item {
  padding: 4px 0;
  display: flex;
  align-items: center;
}

.permission-icon {
  color: #52c41a;
  margin-right: 8px;
}

.permission-tree {
  max-height: 400px;
  overflow-y: auto;
}

.permission-node-icon {
  margin-right: 8px;
  color: #667eea;
}

.search-input {
  border-radius: 20px;
  overflow: hidden;
}

:deep(.search-input .ant-input) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  color: white;
}

:deep(.search-input .ant-input::placeholder) {
  color: rgba(255, 255, 255, 0.5);
}

:deep(.search-input .ant-input:focus) {
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
}

:deep(.search-input .ant-input-search-button) {
  background: rgba(102, 126, 234, 0.8);
  border-color: rgba(102, 126, 234, 0.8);
}

.add-button {
  background: linear-gradient(45deg, #667eea, #764ba2);
  border: none;
  border-radius: 20px;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
  transition: all 0.3s ease;
}

.add-button:hover {
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
  transform: translateY(-2px);
}

:deep(.ant-modal-content) {
  background: #1a1a2e;
  color: white;
  border-radius: 12px;
  overflow: hidden;
}

:deep(.ant-modal-header) {
  background: rgba(102, 126, 234, 0.2);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  padding: 16px 24px;
}

:deep(.ant-modal-title) {
  color: white;
}

:deep(.ant-modal-close) {
  color: rgba(255, 255, 255, 0.8);
}

:deep(.ant-modal-close:hover) {
  color: white;
}

:deep(.ant-form-item-label > label) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-input) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  color: white;
}

:deep(.ant-input:focus) {
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
  border-color: #667eea;
}

:deep(.ant-textarea) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  color: white;
  resize: none;
}

:deep(.ant-textarea:focus) {
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
  border-color: #667eea;
}

:deep(.ant-switch) {
  background: rgba(255, 255, 255, 0.2);
}

:deep(.ant-switch-checked) {
  background: #667eea;
}

:deep(.ant-tree) {
  background: transparent;
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-tree-treenode) {
  padding: 2px 0;
}

:deep(.ant-tree-switcher) {
  color: rgba(255, 255, 255, 0.6);
}

:deep(.ant-tree-node-content-wrapper) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.ant-tree-node-content-wrapper:hover) {
  background: rgba(102, 126, 234, 0.1);
}

:deep(.ant-tree-node-selected) {
  background: rgba(102, 126, 234, 0.2);
}

@media (max-width: 768px) {
  .card-header {
    flex-direction: column;
    align-items: stretch;
  }
  
  .header-actions {
    justify-content: space-between;
  }
  
  .toolbar {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
  
  .toolbar-left {
    width: 100%;
  }
}
</style>