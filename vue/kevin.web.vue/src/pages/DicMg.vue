<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <BookOutlined class="title-icon" />
            <span>字典管理</span>
          </div>
          <div class="header-actions">
            <a-select
              v-model:value="selectedType"
              :options="typeOptions"
              placeholder="请选择字典类型"
              style="width: 200px; margin-right: 16px"
              allow-clear
              @change="onTypeChange"
            />
            <a-input-search
              class="search-input"
              placeholder="搜索字典..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <Button type="primary" class="add-button" @click="showAddDicModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加字典
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
            <template v-if="column.dataIndex === 'key'">
              {{ record.key }}
            </template>
            
            <template v-else-if="column.dataIndex === 'value'">
              {{ record.value }}
            </template>
            
            <template v-else-if="column.dataIndex === 'type'">
              {{ record.type }}
            </template>
            
            <template v-else-if="column.dataIndex === 'sort'">
              {{ record.sort }}
            </template>
            
            <template v-else-if="column.dataIndex === 'remarks'">
              {{ record.remarks }}
            </template>
            
            <template v-else-if="column.dataIndex === 'createTime'">
              {{ formatDate(record.createTime) }}
            </template>
              
            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button 
                  type="link" 
                  size="small" 
                  :disabled="record.isSystem"
                  @click="showEditDicModal(record)"
                >
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-popconfirm
                  title="确定要删除这个字典项吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  :disabled="record.isSystem"
                  @confirm="handleDelete(record.id)"
                >
                  <a-button 
                    type="link" 
                    size="small" 
                    :disabled="record.isSystem"
                    danger
                  >
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
    
    <!-- 添加/编辑字典模态框 -->
    <a-modal v-model:open="dicModalVisible" :title="dicModalTitle" @ok="handleDicModalOk" @cancel="handleDicModalCancel">
      <a-form :model="dicForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="字典类型" v-bind="validateInfos.type">
          <a-auto-complete
            v-model:value="dicForm.type"
            :options="typeOptions"
            placeholder="请输入或选择字典类型"
            allow-clear
          />
        </a-form-item>
        <a-form-item label="字典键" v-bind="validateInfos.key">
          <a-input v-model:value="dicForm.key" />
        </a-form-item>
        <a-form-item label="字典值" v-bind="validateInfos.value">
          <a-input v-model:value="dicForm.value" />
        </a-form-item>
        <a-form-item label="排序" v-bind="validateInfos.sort">
          <a-input-number v-model:value="dicForm.sort" :min="0" />
        </a-form-item>
        <a-form-item label="备注信息">
          <a-textarea v-model:value="dicForm.remarks" :rows="3" />
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
  BookOutlined, 
  PlusOutlined, 
  DeleteOutlined,
  EditOutlined  
} from '@ant-design/icons-vue';
import { message, Button } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { GetPageData, GetTypeList, addEditMessage, deleteMessage } from '@/api/dic'; 

const useForm = Form.useForm;

// 字典数据
const dataSource = ref([]);

// 表格列定义
const columns = ref([
  {
    title: '字典类型',
    dataIndex: 'type',
    key: 'type',
    sorter: true
  },
  {
    title: '字典键',
    dataIndex: 'key',
    key: 'key'
  },
  {
    title: '字典值',
    dataIndex: 'value',
    key: 'value'
  },
   {
    title: '是否系统创建',
    dataIndex: 'isSystem',
    key: 'isSystem'
  },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort'
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
    width: 150
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
const dicModalVisible = ref(false);
const dicModalTitle = ref('添加字典');

// 当前编辑的字典项
const currentDic = ref(null);

// 字典类型筛选相关
const selectedType = ref(undefined);
const typeOptions = ref([]);

// 字典表单
const dicForm = reactive({
  id: '',
  key: '',
  value: '',
  type: '',
  sort: 0,
  remarks: ''
});

// 表单验证规则
const dicRules = reactive({
  type: [
    { required: true, message: '请输入字典类型' }
  ],
  key: [
    { required: true, message: '请输入字典键' }
  ],
  value: [
    { required: true, message: '请输入字典值' }
  ],
  sort: [
    { required: true, message: '请输入排序值' }
  ]
});

// 表单验证
const { validate: validateDicForm, validateInfos } = useForm(dicForm, dicRules);

// 搜索关键字
const searchKeyword = ref('');

// 日期格式化函数
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
};

// 获取字典类型列表
const loadTypeList = async () => {
  try {
    const response = await GetTypeList();
    if (response && response.status === 200 && response.data) {
      // 将返回的数据转换为Select选项格式
      typeOptions.value = response.data.data.map(item => ({
        label: item,
        value: item
      }));
    }
  } catch (error) {
    console.error('加载字典类型失败:', error);
    message.error('加载字典类型失败: ' + (error.message || '未知错误'));
  }
};

// 类型筛选处理
const onTypeChange = (value) => {
  selectedType.value = value;
  pagination.value.current = 1;
  loadDicData();
};

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.value.current = 1;
  loadDicData();
};

// 加载字典数据
const loadDicData = async () => {
  try {
    const params = {
      pageNum: pagination.value.current,
      pageSize: pagination.value.pageSize,
      searchKey: searchKeyword.value,
      type: selectedType.value
    };
    
    const response = await GetPageData(params); 
    if (response && response.status == 200 && response.data.data.data) {
      dataSource.value = response.data.data.data.map(item => ({
        ...item,
        isSystem: item.isSystem ? '是' : '否'
      }));
      pagination.value.total = response.data.data.total; 
    } 
  } catch (error) {
    console.error('加载字典数据失败:', error);
    message.error('加载字典数据失败: ' + (error.message || '未知错误'));
  }
};

// 显示添加字典模态框
const showAddDicModal = () => {
  dicModalTitle.value = '添加字典';
  currentDic.value = null;
  // 重置表单
  Object.assign(dicForm, {
    id: '',
    key: '',
    value: '',
    type: '',
    sort: 0,
    remarks: ''
  });
  dicModalVisible.value = true;
};

// 显示编辑字典模态框
const showEditDicModal = (record) => {
  // 系统内置字典项不能编辑
  if (record.isSystem) {
    message.warning('系统内置字典项不能编辑');
    return;
  }
  
  dicModalTitle.value = '编辑字典';
  currentDic.value = record;
  // 填充表单数据
  Object.assign(dicForm, {
    id: record.id,
    key: record.key,
    value: record.value,
    type: record.type,
    sort: record.sort,
    remarks: record.remarks
  });
  dicModalVisible.value = true;
};

// 删除字典项
const handleDelete = async (id) => {
  // 查找要删除的字典项
  const record = dataSource.value.find(item => item.id === id);
  // 系统内置字典项不能删除
  if (record && record.isSystem) {
    message.warning('系统内置字典项不能删除');
    return;
  }
  
  try {
    await deleteMessage(id);
    message.success('字典项删除成功');
    loadDicData(); // 重新加载数据
  } catch (error) {
    console.error('删除字典项失败:', error);
    message.error('删除字典项失败: ' + (error.message || '未知错误'));
  }
}; 

// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log('表格变化:', pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  loadDicData();
};

// 字典模态框确认
const handleDicModalOk = () => {
  // 检查是否是系统内置字典项的编辑
  if (currentDic.value && currentDic.value.isSystem) {
    message.warning('系统内置字典项不能编辑');
    return;
  }
  
  validateDicForm().then(async () => {
    try {
      const params = {
        id: dicForm.id,
        key: dicForm.key,
        value: dicForm.value,
        type: dicForm.type,
        sort: dicForm.sort,
        remarks: dicForm.remarks
      };
      
      await addEditMessage(params);
      
      if (currentDic.value) {
        message.success('字典信息更新成功');
      } else {
        message.success("字典信息添加成功");
      }
      
      dicModalVisible.value = false;
      loadDicData(); // 重新加载数据
    } catch (error) {
      console.error('保存字典失败:', error);
      message.error('保存字典失败: ' + (error.message || '未知错误'));
    }
  }).catch(err => {
    console.log('表单验证失败:', err);
  });
};

// 字典模态框取消
const handleDicModalCancel = () => {
  dicModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log('字典管理页面已加载');
  loadDicData();
  loadTypeList();
});
</script>