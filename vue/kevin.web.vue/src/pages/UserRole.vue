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
      
      <div class="table-container">
        <a-table 
          :columns="columns" 
          :data-source="dataSource" 
          :pagination="pagination" 
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record }">
              
            <template v-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showEditRoleModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-popconfirm
                  title="确定要删除这个角色吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  @confirm="handleDelete(record.id)"
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
        <a-form-item label="备注信息">
          <a-textarea v-model:value="roleForm.remarks" :rows="3" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import '../css/UserRole.css';
import { ref, reactive, onMounted } from 'vue';
import { 
  SafetyCertificateOutlined, 
  PlusOutlined, 
  DeleteOutlined,
  EditOutlined,  
} from '@ant-design/icons-vue';
import { message, Button } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { getRolePage, addEidtRole, deleteRole } from '@/api/roleapi';
import { GetGuId } from "../api/baseapi"; 


const useForm = Form.useForm;

// 角色数据
const dataSource = ref([]);

// 表格列定义
const columns = ref([
  {
    title: '角色名称',
    dataIndex: 'name',
    key: 'name',
    sorter: true
  },
  {
    title: '备注信息',
    dataIndex: 'remarks',
    key: 'remarks'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
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
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`
}); 

// 模态框状态
const roleModalVisible = ref(false); 
const roleModalTitle = ref('添加角色');

// 当前编辑的角色
const currentRole = ref(null);

// 角色表单
const roleForm = reactive({
  id: '',
  name: '',
  remarks: ''
});

// 表单验证规则
const roleRules = reactive({
  name: [
    { required: true, message: '请输入角色名称' },
    { min: 2, message: '角色名称至少2个字符' }
  ]
});

// 表单验证
const { validate: validateRoleForm, validateInfos } = useForm(roleForm, roleRules);

// 搜索关键字
const searchKeyword = ref('');

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.value.current = 1;
  loadRoleData();
};

// 加载角色数据
const loadRoleData = async () => {
  try {
    const params = {
      pageNum: pagination.value.current,
      pageSize: pagination.value.pageSize,
      searchKey: searchKeyword.value
    };
    
    const response = await getRolePage(params); 
    if (response && response.status == 200 && response.data.data.data) {
   dataSource.value = response.data.data.data.map(item => ({
      ...item,
      key: item.id
    }));
        pagination.value.total = response.data.data.total; 
    } 

  } catch (error) {
    console.error('加载角色数据失败:', error);
    message.error('加载角色数据失败: ' + (error.message || '未知错误'));
  }
};

// 显示添加角色模态框
const showAddRoleModal = () => {
  roleModalTitle.value = '添加角色';
  currentRole.value = null;
  // 重置表单
  Object.assign(roleForm, {
    id: '',
    name: '',
    remarks: ''
  });
  roleModalVisible.value = true;
};

// 显示编辑角色模态框
const showEditRoleModal = (record) => {
  roleModalTitle.value = '编辑角色';
  currentRole.value = record;
  // 填充表单数据
  Object.assign(roleForm, {
    id: record.id,
    name: record.name,
    remarks: record.remarks
  });
  roleModalVisible.value = true;
};

 

 

// 删除角色
const handleDelete = async (id) => {
  try {
    await deleteRole(id);
    message.success('角色删除成功');
    loadRoleData(); // 重新加载数据
  } catch (error) {
    console.error('删除角色失败:', error);
    message.error('删除角色失败: ' + (error.message || '未知错误'));
  }
}; 

// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log('表格变化:', pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  loadRoleData();
};

// 角色模态框确认
const handleRoleModalOk = () => {
  validateRoleForm().then(async () => {
    try {
      const params = {
        id: roleForm.id,
        name: roleForm.name,
        remarks: roleForm.remarks
      };
      
      
      
      if (currentRole.value) {
        await addEidtRole(params);
        // 编辑角色
        message.success('角色信息更新成功');
      } else {
        // 添加角色 
          var dataid = await GetGuId();
          if (dataid && dataid.status == 200 && dataid.data.data) {
            var id = dataid.data.data;
            params.id= id;
            await addEidtRole(params);
            message.success("添加成功");
          } else {
            message.error("添加失败"); 
          } 
      }
      
      roleModalVisible.value = false;
      loadRoleData(); // 重新加载数据
    } catch (error) {
      console.error('保存角色失败:', error);
      message.error('保存角色失败: ' + (error.message || '未知错误'));
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 角色模态框取消
const handleRoleModalCancel = () => {
  roleModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('角色管理页面已加载');
  loadRoleData();
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

.toolbar-left,
.toolbar-right {
  display: flex;
  gap: 8px;
}

.toolbar-button {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: rgba(255, 255, 255, 0.85);
  border-radius: 6px;
  transition: all 0.3s ease;
}

.toolbar-button:hover {
  background: rgba(102, 126, 234, 0.2);
  border-color: rgba(102, 126, 234, 0.5);
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.toolbar-button :deep(.anticon) {
  color: rgba(255, 255, 255, 0.85);
}

.toolbar-button:hover :deep(.anticon) {
  color: white;
}

.table-container {
  background: transparent;
  flex: 1;
  overflow: hidden;
}

.role-table {
  color: white;
}

:deep(.role-table .ant-table) {
  background: transparent;
  color: rgba(255, 255, 255, 0.85);
}

:deep(.role-table .ant-table-thead > tr > th) {
  background: rgba(102, 126, 234, 0.2);
  color: white;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  font-weight: 500;
}

:deep(.role-table .ant-table-tbody > tr) {
  background: transparent;
}

:deep(.role-table .ant-table-tbody > tr > td) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  color: rgba(255, 255, 255, 0.85);
  padding: 12px 8px;
  background: transparent;
  transition: background 0.3s ease;
}

:deep(.role-table .ant-table-tbody > tr:hover > td) {
  background: rgba(102, 126, 234, 0.1) !important;
}

:deep(.role-table .ant-table-tbody > tr.ant-table-row-selected > td) {
  background: rgba(102, 126, 234, 0.2) !important;
}

:deep(.role-table .ant-table-tbody > tr.ant-table-row:hover > td) {
  background: rgba(102, 126, 234, 0.1) !important;
}

:deep(.role-table .ant-table-tbody > tr.ant-table-row) {
  background: transparent !important;
}

:deep(.role-table .ant-table-tbody > tr.ant-table-row > td) {
  background: transparent !important;
  transition: background 0.3s ease;
}

:deep(.role-table .ant-pagination) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.role-table .ant-pagination-item) {
  background: transparent;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

:deep(.role-table .ant-pagination-item a) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.role-table .ant-pagination-item-active) {
  background: #667eea;
  border-color: #667eea;
}

:deep(.role-table .ant-pagination-next),
:deep(.role-table .ant-pagination-prev) {
  color: rgba(255, 255, 255, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

:deep(.role-table .ant-btn-link) {
  color: #667eea;
  padding: 0;
}

:deep(.role-table .ant-btn-link:hover) {
  color: #764ba2;
}

.action-buttons {
  display: flex;
  gap: 8px;
  background: transparent;
  margin-top: 0;
}

.action-button {
  color: rgba(255, 255, 255, 0.85) !important;
  padding: 0 4px;
  height: auto;
  line-height: normal;
}

.action-button:hover {
  color: #667eea !important;
  background: transparent;
}

.action-button.danger:hover {
  color: #ff4d4f !important;
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
  border-radius: 6px;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
  transition: all 0.3s ease;
}

.add-button:hover {
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
  transform: translateY(-2px);
}

:deep(.custom-modal .ant-modal-content) {
  background: #1a1a2e;
  color: white;
  border-radius: 12px;
  overflow: hidden;
}

:deep(.custom-modal .ant-modal-header) {
  background: rgba(102, 126, 234, 0.2);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  padding: 16px 24px;
}

:deep(.custom-modal .ant-modal-title) {
  color: white;
}

:deep(.custom-modal .ant-modal-close) {
  color: rgba(255, 255, 255, 0.8);
}

:deep(.custom-modal .ant-modal-close:hover) {
  color: white;
}

:deep(.custom-modal .ant-form-item-label > label) {
  color: rgba(255, 255, 255, 0.85);
}

:deep(.custom-input .ant-input) {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  color: white;
  border-radius: 6px;
}

:deep(.custom-input .ant-input:focus) {
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
  border-color: #667eea;
}

:deep(.custom-select .ant-select-selector) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: white !important;
  border-radius: 6px !important;
}

:deep(.custom-select .ant-select-selection-item) {
  color: white;
}

:deep(.custom-select .ant-select-arrow) {
  color: rgba(255, 255, 255, 0.5);
}

:deep(.ant-switch) {
  background: rgba(255, 255, 255, 0.2);
}

:deep(.ant-switch-checked) {
  background: #667eea;
}

:deep(.avatar-uploader .ant-upload.ant-upload-select-picture-card) {
  width: 100px;
  height: 100px;
  border: 1px dashed rgba(255, 255, 255, 0.3);
  background: rgba(255, 255, 255, 0.05);
  border-radius: 6px;
}

:deep(.avatar-uploader .ant-upload.ant-upload-select-picture-card:hover) {
  border-color: #667eea;
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

