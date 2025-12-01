<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <SafetyCertificateOutlined class="title-icon" />
            <span>关键数据变动日志管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="关键数据变动日志管理..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            /> 
          </div>
        </div>
      </template>

      <div class="toolbar"> 
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
          :pagination="pagination"
          :scroll="{ x: 1200 }"
          :loading="loading"
          @change="handleTableChange"
          class="my-table"
        > 
        </a-table>
      </div>
    </a-card> 
  </div>
</template>

<script setup>
import "../css/MyTable.css";
import "../css/buttons.css";
import "../css/management.css";
import { ref, onMounted, computed } from "vue";
import {
  SafetyCertificateOutlined, 
  BarsOutlined,
} from "@ant-design/icons-vue";
import { message } from "ant-design-vue"; 
import { GetOSLogPageData } from "@/api/log"; 
// 隐藏的列
const hiddenColumns = ref([]); 
// 计算可见列
const visibleColumns = computed(() => {
  return columns.value.filter(
    (column) => !hiddenColumns.value.includes(column.dataIndex)
  );
});
// 切换列显示/隐藏
const toggleColumn = (dataIndex, visible) => {
  if (visible) {
    hiddenColumns.value = hiddenColumns.value.filter((col) => col !== dataIndex);
  } else {
    hiddenColumns.value.push(dataIndex);
  }
};
// 数据变动日志数据
const dataSource = ref([]);
// 表格列定义
const columns = ref([
  // {
  //   title: "id",
  //   dataIndex: "id",
  //   key: "id"
  // },
  {
    title: "操作时间",
    dataIndex: "createTime",
    key: "createTime", 
    width: 200
  },
   {
    title: "外链表名",
    dataIndex: "table",
    key: "table",
    width: 100
  },
  {
    title: "外链表ID",
    dataIndex: "tableId",
    key: "tableId",
    width: 300
  }, 
  {
    title: "标记",
    dataIndex: "sign",
    key: "sign",
    width: 100, ellipsis: true,
  },  {
    title: "变动内容",
    dataIndex: "content",
    key: "content", 
     ellipsis: true,
     width: 450
  },
  {
    title: "操作人信息",
    dataIndex: "actionUserName",
    key: "actionUserName",
     ellipsis: true,
          width: 100
  }, {
    title: "Ip地址",
    dataIndex: "ipAddress",
    key: "ipAddress", ellipsis: true,width: 100
  },
  {
    title: "设备标记",
    dataIndex: "deviceMark", 
    key: "deviceMark", ellipsis: true,width: 100
  } 
]);
// 数据加载状态
const loading = ref(false); 
// 分页配置
const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`,
});  
// 搜索关键字
const searchKeyword = ref(""); 
// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.value.current = 1;
  loadPermData();
};

// 加载数据变动日志数据
const loadPermData = async () => {
  try {
    const params = {
      pageNum: pagination.value.current,
      pageSize: pagination.value.pageSize,
      searchKey: searchKeyword.value,
    };
    loading.value = true;
    const response = await GetOSLogPageData(params);
    loading.value = false;
    if (response && response.data) {
      dataSource.value = response.data.data.map((item) => ({
        ...item,
        key: item.id,
      }));
      pagination.value.total = response.data.total;
    }
  } catch (error) {
    loading.value = false;
    console.error("加载数据变动日志数据失败:", error);
    message.error("加载数据变动日志数据失败: " + (error.message || "未知错误"));
  } finally {
    loading.value = false;
  }
}; 
// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log("表格变化:", pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  loadPermData();
};  

// 组件挂载时的初始化
onMounted(() => {
  console.log("数据变动日志管理页面已加载");
  loadPermData();
});
</script>
