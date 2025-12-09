<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <ApartmentOutlined class="title-icon" />
            <span>租户管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索租户..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <Button type="primary" class="add-button" @click="showAddTenantModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加租户
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
            <template v-if="column.dataIndex === 'code'">
              {{ record.code }}
            </template>
            
            <template v-else-if="column.dataIndex === 'name'">
              {{ record.name }}
            </template>
            
            <template v-else-if="column.dataIndex === 'status'">
              <a-tag :color="record.status === 1 ? 'green' : 'red'">
                {{ record.status === 1 ? '有效' : '无效' }}
              </a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'createTime'">
              {{ formatDate(record.createTime) }}
            </template>
              
            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showEditTenantModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                
                <a-button 
                  v-if="record.status !== 1" 
                  type="link" 
                  size="small" 
                  @click="activateTenantfunc(record.id)"
                >
                  <template #icon>
                    <CheckCircleOutlined />
                  </template>
                  设为有效
                </a-button>
                
                <a-button 
                  v-if="record.status === 1" 
                  type="link" 
                  size="small" 
                  @click="deactivateTenantfunc(record.id)"
                >
                  <template #icon>
                    <StopOutlined />
                  </template>
                  设为无效
                </a-button>
                
                <a-popconfirm
                  title="确定要删除这个租户吗?"
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
    
    <!-- 添加/编辑租户模态框 -->
    <a-modal 
      v-model:open="tenantModalVisible" 
      :title="tenantModalTitle" 
      @ok="handleTenantModalOk" 
      @cancel="handleTenantModalCancel"
    >
      <a-form :model="tenantForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="租户编码" v-bind="validateInfos.code">
          <a-input-number 
            v-model:value="tenantForm.code" 
            :min="1" 
            style="width: 100%" 
            placeholder="请输入租户编码"
          />
        </a-form-item>
        
        <a-form-item label="租户名称" v-bind="validateInfos.name">
          <a-input 
            v-model:value="tenantForm.name" 
            placeholder="请输入租户名称"
          />
        </a-form-item>
      </a-form>
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
  ApartmentOutlined,
  PlusOutlined,
  DeleteOutlined,
  EditOutlined,
  CheckCircleOutlined,
  StopOutlined
} from '@ant-design/icons-vue';
import { message, Button } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { 
  getTenantPage, 
  createTenant, 
  editTenant, 
  deleteTenant, 
  activateTenant, 
  deactivateTenant 
} from '@/api/tenant';

const useForm = Form.useForm;

// 租户数据
const dataSource = ref([]);

// 表格列定义
const columns = ref([
  {
    title: '租户编码',
    dataIndex: 'code',
    key: 'code',
    sorter: true
  },
  {
    title: '租户名称',
    dataIndex: 'name',
    key: 'name',
    sorter: true
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status'
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
    width: 300
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
const tenantModalVisible = ref(false);
const tenantModalTitle = ref('添加租户');

// 当前编辑的租户
const currentTenant = ref(null);

// 租户表单
const tenantForm = reactive({
  id: '',
  code: undefined,
  name: ''
});

// 表单验证规则
const tenantRules = reactive({
  code: [
    { required: true, message: '请输入租户编码' }
  ],
  name: [
    { required: true, message: '请输入租户名称' },
    { min: 2, message: '租户名称至少2个字符' }
  ]
});

// 表单验证
const { validate: validateTenantForm, validateInfos } = useForm(tenantForm, tenantRules);

// 搜索关键字
const searchKeyword = ref('');

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
};

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.value.current = 1;
  loadTenantData();
};

// 加载租户数据
const loadTenantData = async () => {
  try {
    const params = {
      pageNum: pagination.value.current,
      pageSize: pagination.value.pageSize,
      searchKey: searchKeyword.value
    };
    
    const response = await getTenantPage(params);
    if (response && response.code == 200 && response.data.data) {
      dataSource.value = response.data.data.map(item => ({
        ...item,
        key: item.id
      }));
      pagination.value.total = response.data.total;
    }
  } catch (error) {
    console.error('加载租户数据失败:', error);
    message.error('加载租户数据失败: ' + (error.message || '未知错误'));
  }
};

// 显示添加租户模态框
const showAddTenantModal = () => {
  tenantModalTitle.value = '添加租户';
  currentTenant.value = null;
  // 重置表单
  Object.assign(tenantForm, {
    id: '',
    code: undefined,
    name: ''
  });
  tenantModalVisible.value = true;
};

// 显示编辑租户模态框
const showEditTenantModal = (record) => {
  tenantModalTitle.value = '编辑租户';
  currentTenant.value = record;
  // 填充表单数据
  Object.assign(tenantForm, {
    id: record.id,
    code: record.code,
    name: record.name
  });
  tenantModalVisible.value = true;
};

// 删除租户
const handleDelete = async (id) => {
  try {
    await deleteTenant(id);
    message.success('租户删除成功');
    loadTenantData(); // 重新加载数据
  } catch (error) {
    console.error('删除租户失败:', error);
    message.error('删除租户失败: ' + (error.message || '未知错误'));
  }
}; 

// 激活租户
const activateTenantfunc = async (id) => {
  try {
    await activateTenant(id);
    message.success('租户已设为有效');
    loadTenantData(); // 重新加载数据
  } catch (error) {
    console.error('激活租户失败:', error);
    message.error('激活租户失败: ' + (error.message || '未知错误'));
  }
};

// 停用租户
const deactivateTenantfunc = async (id) => {
  try {
    await deactivateTenant(id);
    message.success('租户已设为无效');
    loadTenantData(); // 重新加载数据
  } catch (error) {
    console.error('停用租户失败:', error);
    message.error('停用租户失败: ' + (error.message || '未知错误'));
  }
};

// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log('表格变化:', pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  loadTenantData();
};

// 租户模态框确认
const handleTenantModalOk = () => {
  validateTenantForm().then(async () => {
    try {
      const params = {
        id: tenantForm.id,
        code: tenantForm.code,
        name: tenantForm.name
      };
      
      if (currentTenant.value) {
        // 编辑租户
        await editTenant(params);
        message.success('租户信息更新成功');
      } else {
        // 新增租户
        await createTenant(params);
        message.success('租户信息添加成功');
      }
      
      tenantModalVisible.value = false;
      loadTenantData(); // 重新加载数据
    } catch (error) {
      console.error('保存租户失败:', error);
      message.error('保存租户失败: ' + (error.message || '未知错误'));
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 租户模态框取消
const handleTenantModalCancel = () => {
  tenantModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('租户管理页面已加载');
  loadTenantData();
});
</script>