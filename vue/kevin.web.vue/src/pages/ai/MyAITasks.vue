<template>
  <div class="ai-tasks-container">
    <div class="page-header">
      <h2 class="page-title">我的AI自动任务</h2>
      <p class="page-description">管理和监控您的AI自动任务</p>
    </div>

    <a-card :bordered="false" class="task-card">
      <div class="card-header">
        <a-button type="primary" @click="refreshTasks" :loading="loading">
          <ReloadOutlined />
          刷新任务
        </a-button>
      </div>

      <a-table
        :columns="columns"
        :data-source="taskList"
        :loading="loading"
        :pagination="false"
        row-key="name"
        :scroll="{ x: 'max-content' }"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'actions'">
            <a-space :size="8">
              <a-button
                type="primary"
                size="small"
                @click="handleExecute(record.name)"
                :disabled="record.executing"
              >
                <PlayCircleOutlined />
                {{ record.executing ? '执行中' : '执行' }}
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
import { ref, onMounted } from "vue";
import { message } from "ant-design-vue";
import {
  ReloadOutlined,
  PlayCircleOutlined,
  DeleteOutlined,
} from "@ant-design/icons-vue";
import { getTaskList, removeCronTask, triggerCronTask } from "../../api/ai/aitask";

const loading = ref(false);
const taskList = ref([]);

const columns = [
  {
    title: "任务名称",
    dataIndex: "name",
    key: "name",
    width: 180,
    ellipsis: true,
  },
  {
    title: "Cron表达式",
    dataIndex: "cron",
    key: "cron",
    width: 200,
    ellipsis: true,
  },
  {
    title: "下次执行时间",
    dataIndex: "next",
    key: "next",
    width: 200,
    ellipsis: true,
  },
  {
    title: "上次执行时间",
    dataIndex: "last",
    key: "last",
    width: 200,
    ellipsis: true,
  },
  {
    title: "时区",
    dataIndex: "timeZone",
    key: "timeZone",
    width: 120,
    ellipsis: true,
  },
  {
    title: "操作",
    key: "actions",
    width: 160,
    fixed: "right",
  },
];

const parseTaskData = (data) => {
  if (!data || data.length === 0) return [];
  
  return data?.map((item) => {
    const parts = item.split(" | ");
    const task = {
      name: "",
      cron: "",
      next: "",
      last: "",
      timeZone: "",
      executing: false,
    };

    parts.forEach((part) => {
      if (part.startsWith("name:")) {
        task.name = part.replace("name:", "");
      } else if (part.startsWith("Cron:")) {
        task.cron = part.replace("Cron:", "");
      } else if (part.startsWith("Next:")) {
        task.next = part.replace("Next:", "");
      } else if (part.startsWith("Last:")) {
        task.last = part.replace("Last:", "");
      } else if (part.startsWith("TimeZone:")) {
        task.timeZone = part.replace("TimeZone:", "");
      }
    });

    return task;
  });
};

const fetchTasks = async () => {
  loading.value = true;
  try {
    const response = await getTaskList();
    if (response.code === 200 && response.isSuccess) { 
      taskList.value = parseTaskData(response.data.list || []);
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

const handleExecute = async (name) => {
  const task = taskList.value.find((t) => t.name === name);
  if (!task || task.executing) return;

  task.executing = true;
  try {
    const response = await triggerCronTask(name);
    if (response.code === 200 && response.isSuccess) {
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

const handleDelete = async (name) => {
  try {
    const response = await removeCronTask(name);
    if (response.code === 200 && response.isSuccess) {
      message.success("任务删除成功");
      taskList.value = taskList.value.filter((t) => t.name !== name);
    } else {
      message.error(response.message || "删除任务失败");
    }
  } catch (error) {
    console.error("Failed to delete task:", error);
    message.error("删除任务失败，请稍后重试");
  }
};

const refreshTasks = () => {
  fetchTasks();
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

:deep(.ant-table-thead > tr > th) {
  background: #fafafa;
  color: rgba(0, 0, 0, 0.88);
  border-bottom: 1px solid #f0f0f0;
}

:deep(.ant-table-tbody > tr:hover > td) {
  background: rgba(22, 119, 255, 0.04);
}

:deep(.ant-btn-primary) {
  background: var(--accent);
  border-color: var(--accent);
}

:deep(.ant-btn-primary:hover) {
  background: var(--accent-hover);
  border-color: var(--accent-hover);
}
</style>