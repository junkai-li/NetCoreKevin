<template>
  <a-table 
    :columns="filteredColumns" 
    :data-source="dataSource" 
    :pagination="pagination" 
    :loading="loading"
    @change="handleTableChange"
    class="my-table"
  >
    <template #bodyCell="{ column, record }">
      <template v-if="column.dataIndex === 'title'">
        <span :class="{ 'unread': !record.isRead }">{{ record.title }}</span>
      </template>
      
      <template v-else-if="column.dataIndex === 'sendUserName'">
        {{ record.sendUserName }}
      </template>
      
      <template v-else-if="column.dataIndex === 'createUser'">
        {{ record.createUser }}
      </template>
      
      <template v-else-if="column.dataIndex === 'createTime'">
        {{ record.createTime ? formatDate(record.createTime) : '' }}
      </template>
      
      <template v-else-if="column.dataIndex === 'action'">
        <div class="action-buttons">
          <a-button type="link" size="small" @click="showDetail(record)">
            <template #icon>
              <EyeOutlined />
            </template>
            查看
          </a-button>
        </div>
      </template>
    </template>
  </a-table>
</template>

<script setup>
import { defineProps, defineEmits, computed } from 'vue'
import { EyeOutlined } from '@ant-design/icons-vue'

// 定义组件属性
const props = defineProps({
  dataSource: {
    type: Array,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  },
  pagination: {
    type: Object,
    required: true
  },
  messageType: {
    type: String,
    default: 'private-system' // private-system, private-user, announcement, system
  }
})

// 定义事件
const emit = defineEmits(['change', 'show-detail'])

// 所有可能的列定义
const allColumns = [
  {
    title: '标题',
    dataIndex: 'title',
    key: 'title'
  },
  {
    title: '发送人',
    dataIndex: 'sendUserName',
    key: 'sendUserName'
  },
  {
    title: '发布人',
    dataIndex: 'createUser',
    key: 'createUser'
  },
  {
    title: '时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '操作',
    dataIndex: 'action',
    key: 'action',
    width: '100px'
  }
]

// 根据消息类型过滤列
const filteredColumns = computed(() => {
  switch (props.messageType) {
    case 'private-system':
    case 'private-user':
      // 私信类型显示发送人
      return allColumns.filter(col => col.dataIndex !== 'createUser')
    case 'announcement':
    case 'system':
      // 公告和系统消息显示发布人
      return allColumns.filter(col => col.dataIndex !== 'sendUserName')
    default:
      return allColumns
  }
})

// 处理表格分页变化
const handleTableChange = (pager) => {
  emit('change', pager)
}

// 显示详情
const showDetail = (record) => {
  emit('show-detail', record)
}

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
}
</script>

<style scoped>
.action-buttons {
  display: flex;
  gap: 8px;
}

.unread {
  font-weight: bold;
}
</style>