<template>
  <div class="user-list-container">
    <a-card class="user-list-card">
      <template #title>
        <div class="card-header">
          <span>用户列表</span>
          <a-button type="primary" class="add-button">添加用户</a-button>
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
            <template v-if="column.dataIndex === 'status'">
              <a-tag :color="record.status === 1 ? 'green' : 'red'">
                {{ record.status === 1 ? '启用' : '禁用' }}
              </a-tag>
            </template>
            
            <template v-else-if="column.dataIndex === 'action'">
              <a-button type="link" size="small">编辑</a-button>
              <a-button type="link" size="small" danger>删除</a-button>
            </template>
          </template>
        </a-table>
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const columns = [
  {
    title: '用户名',
    dataIndex: 'username',
    key: 'username',
  },
  {
    title: '邮箱',
    dataIndex: 'email',
    key: 'email',
  },
  {
    title: '角色',
    dataIndex: 'role',
    key: 'role',
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
  },
  {
    title: '操作',
    dataIndex: 'action',
    key: 'action',
  },
];

const dataSource = ref([
  {
    key: '1',
    username: 'admin',
    email: 'admin@example.com',
    role: '管理员',
    status: 1,
    createTime: '2023-01-01',
  },
  {
    key: '2',
    username: 'user1',
    email: 'user1@example.com',
    role: '普通用户',
    status: 1,
    createTime: '2023-01-02',
  },
  {
    key: '3',
    username: 'user2',
    email: 'user2@example.com',
    role: '普通用户',
    status: 0,
    createTime: '2023-01-03',
  },
]);

const pagination = {
  current: 1,
  pageSize: 10,
  total: 3,
};

const handleTableChange = (pager) => {
  console.log('表格分页变化:', pager);
};
</script>

<style scoped>
.user-list-container {
  color: white;
}

.user-list-card {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  overflow: hidden;
}

:deep(.user-list-card .ant-card-head) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0 20px;
  background: transparent;
}

:deep(.user-list-card .ant-card-head-title) {
  color: white;
  padding: 16px 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.add-button {
  background: #667eea;
  border-color: #667eea;
}

.table-container {
  background: transparent;
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
}

:deep(.ant-table-tbody > tr:hover > td) {
  background: rgba(102, 126, 234, 0.1);
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
</style>