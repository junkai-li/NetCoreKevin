<template>
  <div class="management-container">
    <a-card class="management-card">
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
           class="my-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'name'">
              {{ record.name }}
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
    
    <!-- 权限配置模态框 -->
    <a-modal 
      v-model:open="permissionModalVisible" 
      title="权限配置" 
      width="900px" 
      :footer="null"
      @cancel="handlePermissionModalCancel"
    >
      <PermissionManager 
        v-if="currentRole" 
        :role-id="currentRole.id" 
        @save-success="handlePermissionSaveSuccess"
        @cancel="handlePermissionModalCancel"
      />
    </a-modal>
  </div>
</template>

<script setup>
import '../css/UserRole.css';
import '../css/MyTable.css';
import '../css/buttons.css';
import '../css/management.css';
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
import PermissionManager from '@/components/PermissionManager.vue'
import { KeyOutlined } from '@ant-design/icons-vue'

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
const roleModalVisible = ref(false)
const permissionModalVisible = ref(false)
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

// 显示权限配置模态框
const showPermissionModal = (record) => {
  currentRole.value = record
  permissionModalVisible.value = true
}
 

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
          const addparams = { 
        name: roleForm.name,
        remarks: roleForm.remarks
      }; 
         await addEidtRole(addparams);
          message.success("角色信息添加成功");
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

// handlePermissionModalOk 函数已不再需要，因为 modal 的 footer 已被移除

// 权限保存成功回调
const handlePermissionSaveSuccess = () => {
  permissionModalVisible.value = false
}

// 权限模态框取消
const handlePermissionModalCancel = () => {
  permissionModalVisible.value = false
}

// 组件挂载时的初始化
onMounted(() => {
  console.log('角色管理页面已加载');
  loadRoleData();
});
</script> 

