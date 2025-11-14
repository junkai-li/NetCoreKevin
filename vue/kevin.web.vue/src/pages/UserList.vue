<template>
  <div class="user-management-container">
    <a-card class="user-management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <UserOutlined class="title-icon" />
            <span>用户管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索用户..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddUserModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加用户
            </a-button>
          </div>
        </div>
      </template>
      
      <div class="toolbar">
        <div class="toolbar-left">
          <a-button @click="handleBatchDelete">
            <template #icon>
              <DeleteOutlined />
            </template>
            批量删除
          </a-button>
          <a-button @click="handleExport">
            <template #icon>
              <DownloadOutlined />
            </template>
            导出
          </a-button>
        </div>
        <div class="toolbar-right">
          <a-dropdown>
            <a-button>
              <BarsOutlined />
              列设置
            </a-button>
            <template #overlay>
              <a-menu>
                <a-menu-item v-for="column in columns" :key="column.key">
                  <a-checkbox 
                    :checked="!hiddenColumns.includes(column.dataIndex)"
                    @change="(e) => toggleColumn(column.dataIndex, e.target.checked)"
                  >
                    {{ column.title }}
                  </a-checkbox>
                </a-menu-item>
              </a-menu>
            </template>
          </a-dropdown>
        </div>
      </div>
      
      <div class="table-container">
        <a-table 
          :columns="visibleColumns" 
          :data-source="dataSource" 
          :pagination="pagination"
          :row-selection="rowSelection"
          :scroll="{ x: 1200 }"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'avatar'">
              <a-avatar :src="record.avatar" :size="32" />
            </template>
            
            <template v-else-if="column.dataIndex === 'status'">
              <a-tag :color="record.status === 1 ? 'success' : 'error'">
                {{ record.status === 1 ? '启用' : '禁用' }}
              </a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'role'">
              <a-tag v-for="role in record.roles" :key="role" color="processing">
                {{ role }}
              </a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'lastLogin'">
              {{ record.lastLogin ? record.lastLogin : '从未登录' }}
            </template>
            
            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showEditUserModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-button type="link" size="small" @click="showResetPasswordModal(record)">
                  <template #icon>
                    <KeyOutlined />
                  </template>
                  重置密码
                </a-button>
                <a-popconfirm
                  title="确定要删除这个用户吗?"
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
    
    <!-- 添加/编辑用户模态框 -->
    <a-modal v-model:open="userModalVisible" :title="userModalTitle" @ok="handleUserModalOk" @cancel="handleUserModalCancel">
      <a-form :model="userForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="用户名" v-bind="validateInfos.username">
          <a-input v-model:value="userForm.username" />
        </a-form-item>
        <a-form-item label="邮箱" v-bind="validateInfos.email">
          <a-input v-model:value="userForm.email" />
        </a-form-item>
        <a-form-item label="角色">
          <a-select v-model:value="userForm.roles" mode="multiple" placeholder="请选择角色">
            <a-select-option value="admin">管理员</a-select-option>
            <a-select-option value="user">普通用户</a-select-option>
            <a-select-option value="auditor">审计员</a-select-option>
            <a-select-option value="operator">操作员</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="状态">
          <a-switch v-model:checked="userForm.status" checked-children="启用" un-checked-children="禁用" />
        </a-form-item>
      </a-form>
    </a-modal>
    
    <!-- 重置密码模态框 -->
    <a-modal v-model:open="passwordModalVisible" title="重置密码" @ok="handlePasswordModalOk" @cancel="handlePasswordModalCancel">
      <a-form :model="passwordForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="新密码" v-bind="passwordValidateInfos.password">
          <a-input-password v-model:value="passwordForm.password" />
        </a-form-item>
        <a-form-item label="确认密码" v-bind="passwordValidateInfos.confirmPassword">
          <a-input-password v-model:value="passwordForm.confirmPassword" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { 
  UserOutlined, 
  PlusOutlined, 
  DeleteOutlined, 
  DownloadOutlined, 
  BarsOutlined,
  EditOutlined,
  KeyOutlined
} from '@ant-design/icons-vue';
import { message, Modal } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import '../css/UserList.css';
import hedeImage from '../assets/hede.png'; // 导入图片

const useForm = Form.useForm;

// 用户数据
const dataSource = ref([
  {
    key: '1',
    username: 'admin',
    email: 'admin@example.com',
    avatar: hedeImage,
    roles: ['admin'],
    status: 1,
    createTime: '2023-01-01 12:00:00',
    lastLogin: '2023-10-01 09:30:00'
  },
  {
    key: '2',
    username: 'user1',
    email: 'user1@example.com',
    avatar: hedeImage,
    roles: ['user'],
    status: 1,
    createTime: '2023-01-02 14:20:00',
    lastLogin: '2023-10-02 15:45:00'
  },
  {
    key: '3',
    username: 'user2',
    email: 'user2@example.com',
    avatar: hedeImage,
    roles: ['user', 'auditor'],
    status: 0,
    createTime: '2023-01-03 16:10:00',
    lastLogin: null
  },
  {
    key: '4',
    username: 'operator1',
    email: 'op1@example.com',
    avatar: hedeImage,
    roles: ['operator'],
    status: 1,
    createTime: '2023-02-15 10:30:00',
    lastLogin: '2023-10-03 11:20:00'
  },
  {
    key: '5',
    username: 'auditor1',
    email: 'auditor1@example.com',
    avatar: hedeImage,
    roles: ['auditor'],
    status: 1,
    createTime: '2023-03-20 09:15:00',
    lastLogin: '2023-10-04 14:10:00'
  }
]);

// 表格列定义
const columns = ref([
  {
    title: '用户',
    dataIndex: 'avatar',
    key: 'avatar',
    width: 80
  },
  {
    title: '用户名',
    dataIndex: 'username',
    key: 'username',
    sorter: true,
    width: 120
  },
  {
    title: '邮箱',
    dataIndex: 'email',
    key: 'email',
    width: 200
  },
  {
    title: '角色',
    dataIndex: 'role',
    key: 'role',
    width: 200
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
    filters: [
      { text: '启用', value: 1 },
      { text: '禁用', value: 0 }
    ],
    width: 100
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
    sorter: true,
    width: 180
  },
  {
    title: '最后登录',
    dataIndex: 'lastLogin',
    key: 'lastLogin',
    width: 180
  },
  {
    title: '操作',
    dataIndex: 'action',
    key: 'action',
    fixed: 'right',
    width: 200
  }
]);

// 隐藏的列
const hiddenColumns = ref([]);

// 计算可见列
const visibleColumns = computed(() => {
  return columns.value.filter(column => !hiddenColumns.value.includes(column.dataIndex));
});

// 分页配置
const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 5,
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
const userModalVisible = ref(false);
const passwordModalVisible = ref(false);
const userModalTitle = ref('添加用户');

// 当前编辑的用户
const currentUser = ref(null);

// 用户表单
const userForm = reactive({
  username: '',
  email: '',
  roles: [],
  status: true
});

// 密码表单
const passwordForm = reactive({
  password: '',
  confirmPassword: ''
});

// 表单验证规则
const userRules = reactive({
  username: [
    { required: true, message: '请输入用户名' },
    { min: 3, message: '用户名至少3个字符' }
  ],
  email: [
    { required: true, message: '请输入邮箱' },
    { type: 'email', message: '请输入有效的邮箱地址' }
  ]
});

const passwordRules = reactive({
  password: [
    { required: true, message: '请输入新密码' },
    { min: 6, message: '密码至少6个字符' }
  ],
  confirmPassword: [
    { required: true, message: '请确认密码' },
    { validator: validateConfirmPassword }
  ]
});

// 表单验证
const { validate: validateUserForm, validateInfos } = useForm(userForm, userRules);
const { validate: validatePasswordForm, validateInfos: passwordValidateInfos } = useForm(passwordForm, passwordRules);

// 自定义密码确认验证
function validateConfirmPassword(rule, value) {
  if (value && value !== passwordForm.password) {
    return Promise.reject('两次输入的密码不一致');
  }
  return Promise.resolve();
}

// 切换列显示/隐藏
const toggleColumn = (dataIndex, visible) => {
  if (visible) {
    hiddenColumns.value = hiddenColumns.value.filter(col => col !== dataIndex);
  } else {
    hiddenColumns.value.push(dataIndex);
  }
};

// 搜索处理
const onSearch = (value) => {
  console.log('搜索:', value);
  message.info(`搜索用户: ${value}`);
};

// 显示添加用户模态框
const showAddUserModal = () => {
  userModalTitle.value = '添加用户';
  currentUser.value = null;
  // 重置表单
  Object.assign(userForm, {
    username: '',
    email: '',
    roles: [],
    status: true
  });
  userModalVisible.value = true;
};

// 显示编辑用户模态框
const showEditUserModal = (record) => {
  userModalTitle.value = '编辑用户';
  currentUser.value = record;
  // 填充表单数据
  Object.assign(userForm, {
    username: record.username,
    email: record.email,
    roles: [...record.roles],
    status: record.status === 1
  });
  userModalVisible.value = true;
};

// 显示重置密码模态框
const showResetPasswordModal = (record) => {
  currentUser.value = record;
  // 重置表单
  Object.assign(passwordForm, {
    password: '',
    confirmPassword: ''
  });
  passwordModalVisible.value = true;
};

// 用户模态框确认
const handleUserModalOk = () => {
  validateUserForm().then(() => {
    if (currentUser.value) {
      // 编辑用户
      message.success('用户信息更新成功');
    } else {
      // 添加用户
      message.success('用户添加成功');
    }
    userModalVisible.value = false;
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 用户模态框取消
const handleUserModalCancel = () => {
  userModalVisible.value = false;
};

// 密码模态框确认
const handlePasswordModalOk = () => {
  validatePasswordForm().then(() => {
    message.success(`用户 ${currentUser.value.username} 密码重置成功`);
    passwordModalVisible.value = false;
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 密码模态框取消
const handlePasswordModalCancel = () => {
  passwordModalVisible.value = false;
};

// 删除用户
const handleDelete = (key) => {
  message.success('用户删除成功');
  console.log('删除用户:', key);
};

// 批量删除
const handleBatchDelete = () => {
  if (selectedRowKeys.value.length === 0) {
    message.warning('请先选择要删除的用户');
    return;
  }
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除选中的 ${selectedRowKeys.value.length} 个用户吗?`,
    okText: '确定',
    cancelText: '取消',
    onOk() {
      message.success('用户批量删除成功');
      selectedRowKeys.value = [];
    }
  });
};

// 导出数据
const handleExport = () => {
  message.success('数据导出成功');
};

// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log('表格变化:', pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('用户管理页面已加载');
});
</script>

<style scoped>
.user-management-container {
  color: white;
  height: 100%;
}

.user-management-card {
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

:deep(.user-management-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.user-management-card .ant-card-head-title) {
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

.toolbar-left,
.toolbar-right {
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

:deep(.ant-select-selector) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: white !important;
}

:deep(.ant-select-selection-item) {
  color: white;
}

:deep(.ant-select-arrow) {
  color: rgba(255, 255, 255, 0.5);
}

:deep(.ant-switch) {
  background: rgba(255, 255, 255, 0.2);
}

:deep(.ant-switch-checked) {
  background: #667eea;
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
  
  .toolbar-left,
  .toolbar-right {
    width: 100%;
  }
}
</style>