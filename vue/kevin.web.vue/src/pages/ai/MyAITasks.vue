<template>
  <div class="ai-tasks-container">
    <div class="page-header">
      <h2 class="page-title">我的AI自动任务</h2>
      <p class="page-description">管理和监控您的AI自动任务：支持暂停（暂停后不再执行，可随时恢复）</p>
    </div>

    <a-card :bordered="false" class="task-card">
      <div class="card-header">
        <a-space :size="12">
          <a-button type="primary" @click="refreshTasks" :loading="loading">
            <ReloadOutlined />
            刷新任务
          </a-button>
          <a-input-search
            v-model:value="searchKey"
            placeholder="按任务名称搜索"
            style="width: 260px"
            allow-clear
            @search="handleSearch"
          />
          <a-select v-model:value="stateFilter" style="width: 130px" @change="handleStateChange">
            <a-select-option value="">全部状态</a-select-option>
            <a-select-option value="running">运行中</a-select-option>
            <a-select-option value="paused">已暂停</a-select-option>
          </a-select>
        </a-space>
      </div>

      <a-table
        :columns="columns"
        :data-source="taskList"
        :loading="loading"
        :pagination="pagination"
        row-key="name"
        :scroll="{ x: 'max-content' }"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'type'">
            <a-tag :color="record.taskType === 'cron' ? 'blue' : 'purple'">
              {{ record.taskType === 'cron' ? '周期性' : '一次性' }}
            </a-tag>
          </template>
          <template v-else-if="column.key === 'state'">
            <a-tag :color="record.isPaused ? 'orange' : 'green'">
              {{ record.isPaused ? '已暂停' : '运行中' }}
            </a-tag>
          </template>
          <template v-else-if="column.key === 'next'">
            {{ record.isPaused ? '—（已暂停）' : (record.next || '—') }}
          </template>
          <template v-else-if="column.key === 'last'">
            {{ record.last || '—' }}
          </template>
          <template v-else-if="column.key === 'actions'">
            <a-space :size="8">
              <a-button
                type="primary"
                size="small"
                @click="handleExecute(record.name)"
                :disabled="record.executing || record.isPaused"
              >
                <PlayCircleOutlined />
                {{ record.executing ? '执行中' : '执行' }}
              </a-button>
              <a-button
                size="small"
                @click="record.isPaused ? handleResume(record) : handlePause(record)"
                :loading="record.toggling"
              >
                <template v-if="!record.isPaused"><PauseCircleOutlined /></template>
                <template v-else><CaretRightOutlined /></template>
                {{ record.isPaused ? '恢复' : '暂停' }}
              </a-button>
              <a-button
                size="small"
                danger
                @click="handleDelete(record.name)"
              >
                <DeleteOutlined />
                删除
              </a-button>
            </a-space>
          </template>
        </template>
      </a-table>

      <div v-if="taskList.length === 0 && !loading" class="empty-state">
        <a-empty description="暂无任务" />
      </div>
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from "vue";
import { message, Modal } from "ant-design-vue";
import {
  ReloadOutlined,
  PlayCircleOutlined,
  PauseCircleOutlined,
  CaretRightOutlined,
  DeleteOutlined,
} from "@ant-design/icons-vue";
import {
  getMyTaskPage,
  removeCronTask,
  triggerCronTask,
  pauseTask,
  resumeTask,
} from "../../api/ai/aitask";

const loading = ref(false);
const taskList = ref([]);
const searchKey = ref("");
const stateFilter = ref("");
const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条`,
});

const columns = [
  {
    title: "任务名称",
    dataIndex: "name",
    key: "name",
    width: 180,
    ellipsis: true,
  },
  {
    title: "类型",
    key: "type",
    width: 90,
  },
  {
    title: "状态",
    key: "state",
    width: 90,
  },
  {
    title: "Cron表达式",
    dataIndex: "cron",
    key: "cron",
    width: 180,
    ellipsis: true,
  },
  {
    title: "下次执行时间",
    key: "next",
    width: 180,
    ellipsis: true,
  },
  {
    title: "上次执行时间",
    key: "last",
    width: 180,
    ellipsis: true,
  },
  {
    title: "任务内容",
    dataIndex: "content",
    key: "content",
    width: 220,
    ellipsis: true,
  },
  {
    title: "操作",
    key: "actions",
    width: 240,
    fixed: "right",
  },
];

const fetchTasks = async () => {
  loading.value = true;
  try {
    const response = await getMyTaskPage({
      searchKey: searchKey.value || "",
      state: stateFilter.value || "",
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
    });
    if (response.code === 200 && response.isSuccess) {
      const data = response.data || {};
      taskList.value = (data.list || []).map((item) => ({
        ...item,
        executing: false,
        toggling: false,
      }));
      pagination.total = data.total || 0;
    } else {
      message.error(response.message || "获取任务列表失败");
    }
  } catch (error) {
    console.error("Failed to fetch tasks:", error);
    message.error("获取任务列表失败，请稍后重试");
  } finally {
    loading.value = false;
  }
};

const handleTableChange = (pag) => {
  pagination.current = pag.current;
  pagination.pageSize = pag.pageSize;
  fetchTasks();
};

const handleStateChange = () => {
  pagination.current = 1;
  fetchTasks();
};

const handleSearch = () => {
  pagination.current = 1;
  fetchTasks();
};

const refreshTasks = () => {
  fetchTasks();
};

const handleExecute = async (name) => {
  const task = taskList.value.find((t) => t.name === name);
  if (!task || task.executing) return;

  task.executing = true;
  try {
    const response = await triggerCronTask(name);
    // 包装器对 HTTP 200 恒返回 IsSuccess=true，实际结果在 data 里，必须校验 data === true
    if (response.code === 200 && response.isSuccess && response.data === true) {
      message.success("任务执行成功");
      await fetchTasks();
    } else {
      message.error(response.message || "执行任务失败");
    }
  } catch (error) {
    console.error("Failed to execute task:", error);
    message.error("执行任务失败，请稍后重试");
  } finally {
    task.executing = false;
  }
};

const handlePause = async (task) => {
  if (task.toggling) return;
  Modal.confirm({
    title: "确认暂停该任务？",
    content: `暂停后「${task.name}」将不再自动执行，任务配置保留，可随时在列表中恢复。`,
    okText: "暂停",
    cancelText: "取消",
    onOk: async () => {
      task.toggling = true;
      try {
        const response = await pauseTask(task.name);
        if (response.code === 200 && response.isSuccess && response.data === true) {
          message.success("任务已暂停");
          await fetchTasks();
        } else {
          message.error(response.message || "暂停任务失败");
        }
      } catch (error) {
        console.error("Failed to pause task:", error);
        message.error("暂停任务失败，请稍后重试");
      } finally {
        task.toggling = false;
      }
    },
  });
};

const handleResume = async (task) => {
  if (task.toggling) return;
  task.toggling = true;
  try {
    const response = await resumeTask(task.name);
    if (response.code === 200 && response.isSuccess && response.data === true) {
      message.success("任务已恢复执行");
      await fetchTasks();
    } else {
      message.error(response.message || "恢复任务失败");
    }
  } catch (error) {
    console.error("Failed to resume task:", error);
    message.error("恢复任务失败，请稍后重试");
  } finally {
    task.toggling = false;
  }
};

const handleDelete = async (name) => {
  try {
    const response = await removeCronTask(name);
    if (response.code === 200 && response.isSuccess && response.data === true) {
      message.success("任务删除成功");
      await fetchTasks();
    } else {
      message.error(response.message || "删除任务失败");
    }
  } catch (error) {
    console.error("Failed to delete task:", error);
    message.error("删除任务失败，请稍后重试");
  }
};

onMounted(() => {
  fetchTasks();
});
</script>

<style scoped>
.ai-tasks-container {
  padding: 24px;
}

.page-header {
  margin-bottom: 24px;
}

.page-title {
  font-size: 20px;
  font-weight: 600;
  color: rgba(0, 0, 0, 0.88);
  margin: 0 0 8px;
}

.page-description {
  font-size: 14px;
  color: rgba(0, 0, 0, 0.45);
  margin: 0;
}

.task-card {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.03);
}

.card-header {
  display: flex;
  justify-content: flex-start;
  margin-bottom: 16px;
  padding-bottom: 16px;
  border-bottom: 1px solid #f0f0f0;
}

.empty-state {
  padding: 40px 0;
  text-align: center;
}

::deep(.ant-table-thead > tr > th) {
  background: #fafafa;
  color: rgba(0, 0, 0, 0.88);
  border-bottom: 1px solid #f0f0f0;
}

::deep(.ant-table-tbody > tr:hover > td) {
  background: rgba(22, 119, 255, 0.04);
}

::deep(.ant-btn-primary) {
  background: var(--accent);
  border-color: var(--accent);
}

::deep(.ant-btn-primary:hover) {
  background: var(--accent-hover);
  border-color: var(--accent-hover);
}
</style>
