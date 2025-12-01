<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <MessageOutlined class="title-icon" />
            <span>提示词管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索提示词..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddPromptModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加提示词
            </a-button>
          </div>
        </div>
      </template>

      <div class="cards-container">
        <a-row :gutter="[24, 24]">
          <a-col
            :xs="24"
            :sm="12"
            :md="12"
            :lg="8"
            :xl="6"
            v-for="prompt in promptList"
            :key="prompt.id"
          >
            <a-card class="prompt-card" hoverable>
              <template #title>
                <div class="card-title">
                  <span class="prompt-name">{{ prompt.name }}</span>
                </div>
              </template>
              <template #extra>
                <a-dropdown>
                  <a class="ant-dropdown-link" @click.prevent>
                    <EllipsisOutlined />
                  </a>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item @click="showEditPromptModal(prompt)">
                        <edit-outlined /> 编辑
                      </a-menu-item>
                      <a-menu-item @click="showDeleteConfirm(prompt)">
                        <delete-outlined /> 删除
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </template>
              <div class="card-content">
                <div class="prompt-section">
                  <h4>入参提示词:</h4>
                  <p class="prompt-text">{{ prompt.inputPrompt }}</p>
                </div>
                <div class="prompt-section">
                  <h4>返回提示词:</h4>
                  <p class="prompt-text">{{ prompt.outputPrompt }}</p>
                </div>
              </div>
            </a-card>
          </a-col>
        </a-row>

        <a-empty v-if="promptList.length === 0" description="暂无提示词数据" />

        <div class="pagination-container" v-if="promptList.length > 0">
          <a-pagination
            v-model:current="pagination.current"
            v-model:page-size="pagination.pageSize"
            :total="pagination.total"
            show-size-changer
            show-quick-jumper
            :show-total="(total) => `共 ${total} 条记录`"
            @change="handlePageChange"
          />
        </div>
      </div>
    </a-card>

    <!-- 添加/编辑提示词模态框 -->
    <a-modal
      v-model:open="promptModalVisible"
      :title="promptModalTitle"
      @ok="handlePromptModalOk"
      @cancel="handlePromptModalCancel"
      :confirm-loading="confirmLoading"
      width="600px"
    >
      <a-form :model="promptForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="名称" v-bind="validateInfos.name">
          <a-input v-model:value="promptForm.name" placeholder="请输入提示词名称" />
        </a-form-item>
        <a-form-item label="入参提示词" v-bind="validateInfos.inputPrompt">
          <a-textarea
            v-model:value="promptForm.inputPrompt"
            :rows="4"
            placeholder="请输入入参提示词"
            :maxlength="1500"
            show-count
          />
        </a-form-item>
        <a-form-item label="返回提示词" v-bind="validateInfos.outputPrompt">
          <a-textarea
            v-model:value="promptForm.outputPrompt"
            :rows="4"
            placeholder="请输入返回提示词"
            :maxlength="1500"
            show-count
          />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import "../../css/PromptManagement.css";
import { ref, reactive, onMounted } from "vue";
import {
  MessageOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined,
} from "@ant-design/icons-vue";
import { message, Modal } from "ant-design-vue";
import { Form } from "ant-design-vue";
import {
  getAIPromptsPageData,
  addEditAIPrompts,
  deleteAIPrompts,
} from "@/api/ai/aiPrompts";

const useForm = Form.useForm;

// 提示词数据
const promptList = ref([]);

// 分页配置
const pagination = reactive({
  current: 1,
  pageSize: 8,
  total: 0,
});

// 模态框状态
const promptModalVisible = ref(false);
const confirmLoading = ref(false);
const promptModalTitle = ref("添加提示词");

// 当前编辑的提示词
const currentPrompt = ref(null);

// 提示词表单
const promptForm = reactive({
  id: "",
  name: "",
  inputPrompt: "",
  outputPrompt: "",
});

// 表单验证规则
const promptRules = reactive({
  name: [{ required: true, message: "请输入提示词名称" }],
  inputPrompt: [
    { required: true, message: "请输入入参提示词" },
    { max: 1500, message: "入参提示词不能超过1500个字符" },
  ],
  outputPrompt: [
    { required: true, message: "请输入返回提示词" },
    { max: 1500, message: "返回提示词不能超过1500个字符" },
  ],
});

// 表单验证
const { validate: validatePromptForm, validateInfos, resetFields } = useForm(
  promptForm,
  promptRules
);

// 搜索关键字
const searchKeyword = ref("");

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.current = 1;
  loadPromptData();
};

// 加载提示词数据
const loadPromptData = async () => {
  try {
    const params = {
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
      searchKey: searchKeyword.value,
    };

    const response = await getAIPromptsPageData(params);
    if (response && response.code === 200 && response.data.data) {
      promptList.value = response.data.data.map((item) => ({
        ...item,
        key: item.id,
      }));
      pagination.total = response.data.total;
    }
  } catch (error) {
    console.error("加载提示词数据失败:", error);
    message.error("加载提示词数据失败: " + (error.message || "未知错误"));
  }
};

// 显示添加提示词模态框
const showAddPromptModal = () => {
  promptModalTitle.value = "添加提示词";
  currentPrompt.value = null;
  // 重置表单
  Object.assign(promptForm, {
    id: "",
    name: "",
    inputPrompt: "",
    outputPrompt: "",
  });
  resetFields();
  promptModalVisible.value = true;
};

// 显示编辑提示词模态框
const showEditPromptModal = (record) => {
  promptModalTitle.value = "编辑提示词";
  currentPrompt.value = record;
  // 填充表单数据
  Object.assign(promptForm, {
    id: record.id,
    name: record.name,
    inputPrompt: record.inputPrompt,
    outputPrompt: record.outputPrompt,
  });
  resetFields();
  promptModalVisible.value = true;
};

// 删除提示词确认
const showDeleteConfirm = (record) => {
  Modal.confirm({
    title: "确认删除",
    content: `确定要删除提示词"${record.name}"吗？`,
    okText: "确认",
    cancelText: "取消",
    onOk: () => handleDelete(record.id, record.name),
  });
};

// 删除提示词
const handleDelete = async (id, name) => {
  try {
    await deleteAIPrompts(id);
    message.success(`提示词"${name}"删除成功`);
    loadPromptData(); // 重新加载数据
  } catch (error) {
    console.error("删除提示词失败:", error);
    message.error("删除提示词失败: " + (error.message || "未知错误"));
  }
};

// 分页变化处理
const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadPromptData();
};

// 提示词模态框确认
const handlePromptModalOk = () => {
  validatePromptForm()
    .then(async () => {
      confirmLoading.value = true;
      try {  
        await addEditAIPrompts(currentPrompt.value? {
              id: promptForm.id,
              name: promptForm.name,
              inputPrompt: promptForm.inputPrompt,
              outputPrompt: promptForm.outputPrompt,
            }:{
              name: promptForm.name,
              inputPrompt: promptForm.inputPrompt,
              outputPrompt: promptForm.outputPrompt,
            }); 
        message.success(
          currentPrompt.value ? "提示词信息更新成功" : "提示词信息添加成功"
        );

        promptModalVisible.value = false;
        loadPromptData(); // 重新加载数据
      } catch (error) {
        console.error("保存提示词失败:", error);
        message.error("保存提示词失败: " + (error.message || "未知错误"));
      } finally {
        confirmLoading.value = false;
      }
    })
    .catch((err) => {
      console.log("表单验证失败:", err);
    });
};

// 提示词模态框取消
const handlePromptModalCancel = () => {
  promptModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log("提示词管理页面已加载");
  loadPromptData();
});
</script>
