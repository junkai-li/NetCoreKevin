<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <ApartmentOutlined class="title-icon" />
            <span>岗位管理</span>
          </div>
          <div class="header-actions">
            <Button type="primary" class="add-button" @click="showAddPositionModal(0)">
              <template #icon>
                <PlusOutlined />
              </template>
              添加根岗位
            </Button>
          </div>
        </div>
      </template> 
      
      <div class="table-container">
        <a-table 
          :columns="columns" 
          :data-source="dataSource" 
          :pagination="false"
          :tree-style="{ marginLeft: '16px' }"
          childrenColumnName="children"
          class="my-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'name'">
              {{ record.name }}
            </template>
            
            <template v-else-if="column.dataIndex === 'code'">
              {{ record.code }}
            </template>
            
            <template v-else-if="column.dataIndex === 'sort'">
              {{ record.sort }}
            </template>
            
            <template v-else-if="column.dataIndex === 'status'">
              <a-tag :color="record.status === 1 ? 'green' : 'red'">
                {{ record.status === 1 ? '启用' : '禁用' }}
              </a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'createTime'">
              {{ formatDate(record.createTime) }}
            </template>
              
            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showAddPositionModal(record.id)">
                  <template #icon>
                    <PlusOutlined />
                  </template>
                  添加下级
                </a-button>
                <a-button type="link" size="small" @click="showEditPositionModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-popconfirm
                  title="确定要删除这个岗位吗?"
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
    
    <!-- 添加/编辑岗位模态框 -->
    <a-modal v-model:open="positionModalVisible" :title="positionModalTitle" @ok="handlePositionModalOk" @cancel="handlePositionModalCancel">
      <a-form :model="positionForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="岗位名称" v-bind="validateInfos.name">
          <a-input v-model:value="positionForm.name" />
        </a-form-item>
        <a-form-item label="岗位编码" v-bind="validateInfos.code">
          <a-input v-model:value="positionForm.code" />
        </a-form-item>
        <a-form-item label="上级岗位">
          <a-tree-select
            v-model:value="positionForm.parentId"
            show-search
            placeholder="请选择上级岗位"
            :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }"
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
          <a-textarea v-model:value="positionForm.description" :rows="3" />
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
import { ref, reactive, onMounted } from 'vue';
import { 
  ApartmentOutlined,
  PlusOutlined, 
  DeleteOutlined,
  EditOutlined
} from '@ant-design/icons-vue';
import { message, Button } from 'ant-design-vue';
import { Form } from 'ant-design-vue';
import { getPositionTree, addEditPosition, deletePosition } from '@/api/organizational/position';

const useForm = Form.useForm;

// 岗位数据
const dataSource = ref([]);

// 表格列定义
const columns = ref([
  {
    title: '岗位名称',
    dataIndex: 'name',
    key: 'name',
    sorter: true
  },
  {
    title: '岗位编码',
    dataIndex: 'code',
    key: 'code'
  },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort'
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
    width: 250
  }
]);

// 模态框状态
const positionModalVisible = ref(false);
const positionModalTitle = ref('添加岗位');

// 当前编辑的岗位
const currentPosition = ref(null);

// 上级岗位选项
const parentPositions = ref([{ value: 0, title: '无上级岗位', key: 0 }]);

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

// 日期格式化函数
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
};

// 加载上级岗位数据
const loadParentPositions = async () => {
  try {
    const response = await getPositionTree(); 
    if (response && response.data.code == 200 && response.data) { 
      // 转换数据格式以适应TreeSelect组件
      const convertToTreeData = (data) => {
        if (!data) return [];
        
        console.log(data);
        // 递归转换节点
        const traverse = (node) => {
          if (!node) return null;
          
          const convertedNode = {
            value: node.id,
            title: node.name,
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

// 加载岗位数据
const loadPositionData = async () => {
  try {
    const response = await getPositionTree();
    if (response && response.data.code == 200 && response.data) {
      // 转换数据格式以适应Table组件
      const convertToTableData = (data) => {
        if (!data) return null;
        
        const result = {
          ...data,
          key: data.id,
          children: data.children && data.children.length > 0 
            ? data.children.map(child => convertToTableData(child)).filter(child => child !== null)
            : []
        };
        
        return result;
      };
      
      const convertedData = convertToTableData(response.data.data);
      dataSource.value = convertedData ? [convertedData] : [];
    }
  } catch (error) {
    console.error('加载岗位数据失败:', error);
    message.error('加载岗位数据失败: ' + (error.message || '未知错误'));
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