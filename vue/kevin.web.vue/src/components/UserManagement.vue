<template>
  <div class="user-management-container">
    <a-card class="user-management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <UserOutlined class="title-icon" />
            <span>{{ title }}</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              :placeholder="searchPlaceholder"
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button v-if="showAddButton" type="primary" class="add-button" @click="showAddUserModal">
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
          <a-button v-if="showExportButton" class="toolbar-button" @click="handleExport">
            <template #icon>
              <DownloadOutlined />
            </template>
            导出
          </a-button>
        </div>
        <div class="toolbar-right">
          <a-dropdown>
            <a-button class="toolbar-button">
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
          :pagination="props.paginationConfig" 
          :scroll="{ x: 1200 }"
          :loading="loading"
          @change="handleTableChange"
          class="my-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'avatar'">
              <a-avatar :src="record.avatar" :size="32" />
            </template>

            <template v-else-if="column.dataIndex === 'status'">
              <a-tag :color="record.status==true? 'success' : 'error'">
                {{ record.status==true? "启用" : "禁用" }}
              </a-tag>
            </template>

            <template v-else-if="column.dataIndex === 'role'">
              <a-tag v-for="role in record.roles" :key="role.id" color="processing">
                {{ role.name }}
              </a-tag>
            </template>
          <template v-else-if="column.dataIndex === 'position'">
              <a-tag v-for="role in record.positions" :key="role.id" color="processing">
                {{ role.name }}
              </a-tag>
            </template>
            <template v-else-if="column.dataIndex === 'lastLogin'">
              {{ record.lastLogin ? record.lastLogin : "从未登录" }}
            </template>

            <template v-else-if="column.dataIndex === 'createTime'">
              {{ formatDate(record.createTime) }}
            </template>
            
            <template v-else-if="column.dataIndex === 'recentLoginTime'">
              {{ formatDate(record.recentLoginTime) }}
            </template>

            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button
                  v-if="showEditButton"
                  type="link"
                  size="small"
                  class="action-button"
                  @click="showEditUserModal(record)"
                >
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button> 
                <a-popconfirm
                  v-if="showDeleteButton"
                  title="确定要删除这个用户吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  @confirm="handleDelete(record.id)"
                >
                  <a-button type="link" size="small" class="action-button danger">
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

    <!-- 用户管理模态框 -->
    <UserAddEdit
      :visible="userModalVisible"
      :title="userModalTitle"
      :user="currentUser"
      @ok="handleUserModalOk"
      @cancel="handleUserModalCancel"
    />
  </div>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, computed, onMounted, watch } from "vue";
import {
  UserOutlined,
  PlusOutlined,
  DeleteOutlined,
  DownloadOutlined,
  BarsOutlined,
  EditOutlined,
} from "@ant-design/icons-vue";
import { message } from "ant-design-vue";
import { getUserList, DeleteUser, ExportGetSysUserList } from "../api/userapi";
import hedeImage from "../assets/hede.png"; // 导入图片
import UserAddEdit from "./UserAddEdit.vue";

// Props
const props = defineProps({
  // 组件标题
  title: {
    type: String,
    default: "用户管理"
  },
  // 搜索框占位符
  searchPlaceholder: {
    type: String,
    default: "搜索用户..."
  },
  // 是否显示添加按钮
  showAddButton: {
    type: Boolean,
    default: true
  },
  // 是否显示导出按钮
  showExportButton: {
    type: Boolean,
    default: true
  },
  // 是否显示编辑按钮
  showEditButton: {
    type: Boolean,
    default: true
  },
  // 是否显示删除按钮
  showDeleteButton: {
    type: Boolean,
    default: true
  },
  // 外部传入的查询参数
  queryParams: {
    type: Object,
    default: () => ({})
  },
  // 是否自动加载数据
  autoLoad: {
    type: Boolean,
    default: true
  }
});

// Emit事件
const emit = defineEmits(['user-added', 'user-updated', 'user-deleted', 'user-exported', 'load-success', 'load-error']);

// 数据加载状态
const loading = ref(false);

// 用户数据
const dataSource = ref([]);

// 表格列定义
const columns = ref([
  {
    title: "用户",
    dataIndex: "avatar",
    key: "avatar",
    width: 80,
  },
  {
    title: "手机号",
    dataIndex: "phone",
    key: "phone", 
    width: 120,
  },
  {
    title: "用户名",
    dataIndex: "name",
    key: "name", 
    width: 120,
  },
  {
    title: "昵称",
    dataIndex: "nickName",
    key: "nickName",
    width: 120,
  },
  {
    title: "邮箱",
    dataIndex: "email",
    key: "email",
    width: 150,
  },
  {
    title: "角色",
    dataIndex: "role",
    key: "role",
    width: 200,
  },
    {
    title: "岗位",
    dataIndex: "position",
    key: "position",
    width: 200,
  },
  {
    title: "状态",
    dataIndex: "status",
    key: "status",
    filters: [
      { text: "启用", value: true },
      { text: "禁用", value: false },
    ],
    width: 100,
  },
  {
    title: "创建时间",
    dataIndex: "createTime",
    key: "createTime",
    sorter: true,
    width: 180,
  },
  {
    title: "最近登录时间",
    dataIndex: "recentLoginTime",
    key: "recentLoginTime",
    sorter: true,
    width: 180,
  },
  {
    title: "操作",
    dataIndex: "action",
    key: "action",
    fixed: "right",
    width: 200,
  },
]);

// 隐藏的列
const hiddenColumns = ref([]);

// 计算可见列
const visibleColumns = computed(() => {
  return columns.value.filter(
    (column) => !hiddenColumns.value.includes(column.dataIndex)
  );
});

// 分页配置
const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`,
});
 
// 搜索关键词
const searchKey = ref("");

// 模态框状态
const userModalVisible = ref(false);
const userModalTitle = ref("添加用户");

// 当前编辑的用户
const currentUser = ref(null);

// 组件挂载时加载数据
onMounted(() => {
  if (props.autoLoad) {
    fetchUserList();
  }
});

// 监听查询参数变化
watch(() => props.queryParams, () => {
  // 重置到第一页
  pagination.value.current = 1;
  fetchUserList();
}, { deep: true });

// 切换列显示/隐藏
const toggleColumn = (dataIndex, visible) => {
  if (visible) {
    hiddenColumns.value = hiddenColumns.value.filter((col) => col !== dataIndex);
  } else {
    hiddenColumns.value.push(dataIndex);
  }
};

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
};

// 搜索处理
const onSearch = (value) => {
  searchKey.value = value;
  pagination.value.current = 1;
  fetchUserList();
};

// 获取用户列表
const fetchUserList = async () => {
  loading.value = true;
  try {
    // 构造查询参数，优先使用外部传入的参数，然后是搜索关键字和分页信息
    const params = {
      searchKey: searchKey.value,
      pageSize: pagination.value.pageSize,
      pageNum: pagination.value.current,
      ...props.queryParams
    };
    
    const response = await getUserList(params);
    if (response && response.status == 200 && response.data && response.data.data) {
      // 处理返回的数据
      dataSource.value = response.data.data.data.map((user) => ({
        key: user.id,
        id: user.id,
        name: user.name,
        nickName: user.nickName,
        phone: user.phone,
        email: user.email,
        roles: user.roles,
        positions: user.positions,
        status: user.status, 
        createTime: user.createTime,
        recentLoginTime: user.recentLoginTime,
        avatar: hedeImage,
      }));

      pagination.value.total = response.data.data.total;
      
      // 触发加载成功事件
      emit('load-success', response.data.data);
    } else {
      throw new Error("数据格式不正确");
    }
  } catch (error) {
    console.error("获取用户列表失败:", error);
    message.error("获取用户列表失败: " + error.message);
    // 触发加载失败事件
    emit('load-error', error);
  } finally {
    loading.value = false;
  }
};

// 显示添加用户模态框
const showAddUserModal = () => {
  userModalTitle.value = "添加用户";
  currentUser.value = null;
  userModalVisible.value = true;
};

// 显示编辑用户模态框
const showEditUserModal = (record) => {
  userModalTitle.value = "编辑用户";
  currentUser.value = record;
  userModalVisible.value = true;
};

// 用户模态框确认
const handleUserModalOk = async (userData) => {
  userModalVisible.value = false;
  if (props.currentUser) {
    emit('user-updated', userData);
  } else {
    emit('user-added', userData);
  }
  fetchUserList(); // 重新加载数据
};

// 用户模态框取消
const handleUserModalCancel = () => {
  userModalVisible.value = false;
};

// 删除用户
const handleDelete = (key) => { 
  DeleteUser(key).then(() => {
    message.success("用户删除成功");
    emit('user-deleted', key);
    fetchUserList(); // 重新加载数据
  }).catch(error => {
    message.error("删除用户失败: " + error.message);
  });
};

// 导出数据
const handleExport = async () => {
  loading.value = true;
  try {
    // 构造导出参数
    const params = {
      searchKey: searchKey.value,
      pageSize: pagination.value.pageSize,
      pageNum: pagination.value.current,
      ...props.queryParams
    };
    
    const response = await ExportGetSysUserList(params);
    if (response) {
      message.success("导出用户成功");
      emit('user-exported', response);
    }
  } catch (error) { 
    message.error("导出用户失败: " + error.message);
  } finally {
    loading.value = false;
  }
};

// 表格变化处理（分页、排序、筛选）
const handleTableChange = (pager, filters, sorter) => {
  // eslint-disable-next-line no-unused-vars
  const unusedFilters = filters;
  // eslint-disable-next-line no-unused-vars
  const unusedSorter = sorter;
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  fetchUserList(); // 重新加载数据
};

// 公开方法：重新加载数据
defineExpose({
  reload: fetchUserList
});
</script>

<style scoped>
@import "../css/MyTable.css";
@import "../css/UserList.css";  
@import "../css/management.css";  
</style>