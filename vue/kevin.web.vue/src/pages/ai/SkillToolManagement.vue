<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <ToolOutlined class="title-icon" />
            <span>AI技能工具管理</span>
          </div>
          <div class="header-actions">
            <a-select
              v-model:value="filterType"
              placeholder="筛选类型"
              allow-clear
              style="width: 120px; margin-right: 16px"
              @change="onFilterChange"
            >
              <a-select-option :value="1">Tool</a-select-option>
              <a-select-option :value="2">Skill</a-select-option>
            </a-select>
            <a-input-search
              class="search-input"
              placeholder="搜索技能工具..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加技能工具
            </a-button>
          </div>
        </div>
      </template>

      <div class="cards-container">
        <a-row :gutter="[24, 24]">
          <a-col :xs="24" :sm="12" :md="12" :lg="8" :xl="6" v-for="item in dataList" :key="item.id">
            <a-card class="model-card" hoverable>
              <template #title>
                <div class="card-title">
                  <span class="model-name">{{ item.name }}</span>
                  <a-tag :color="item.activeStatus === 1 ? 'green' : 'red'" style="margin-left: 8px">
                    {{ item.activeStatus === 1 ? '启用' : '禁用' }}
                  </a-tag>
                  <a-tag v-if="item.isSystem" color="blue" style="margin-left: 4px">系统内置</a-tag>
                </div>
              </template>
              <template #extra>
                <a-dropdown>
                  <a class="ant-dropdown-link" @click.prevent>
                    <EllipsisOutlined />
                  </a>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item @click="showViewModal(item)">
                        <eye-outlined /> 预览
                      </a-menu-item>
                      <a-menu-item @click="showEditModal(item)" :disabled="item.isSystem">
                        <edit-outlined /> 编辑
                      </a-menu-item>
                      <a-menu-item @click="showDeleteConfirm(item)" :disabled="item.isSystem">
                        <delete-outlined /> 删除
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </template>
              <div @click="showEditModal(item)" class="card-content">
                <div class="model-info horizontal-layout">
                  <div class="info-item horizontal">
                    <span class="info-label">类型:</span>
                    <span class="info-value">{{ getSkillToolTypeName(item.skillToolType) }}</span>
                  </div>
                  <div class="info-item horizontal">
                    <span class="info-label">方法:</span>
                    <span class="info-value">{{ item.classMethod }}</span>
                  </div>
                </div>
                <div class="model-info horizontal-layout">
                  <div class="info-item horizontal">
                    <span class="info-label">描述:</span>
                    <span class="info-value">{{ item.description }}</span>
                  </div>
                </div>
                <div class="model-info horizontal-layout" v-if="item.createUser || item.updateUser">
                  <div class="info-item horizontal">
                    <span class="info-label" v-if="item.createUser">创建人:</span>
                    <span class="info-value" v-if="item.createUser">{{ item.createUser }}</span>
                    <span class="info-label" v-if="item.updateUser" style="margin-left: 16px;">更新人:</span>
                    <span class="info-value" v-if="item.updateUser">{{ item.updateUser }}</span>
                  </div>
                </div>
              </div>
            </a-card>
          </a-col>
        </a-row>

        <a-empty v-if="dataList.length === 0" description="暂无技能工具数据" />

        <div class="pagination-container" v-if="dataList.length > 0">
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

    <a-modal
      v-model:open="modalVisible"
      :title="modalTitle"
      @ok="handleModalOk"
      @cancel="handleModalCancel"
      :confirm-loading="confirmLoading"
      width="600px"
    >
      <a-form :model="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="名称" v-bind="validateInfos.name">
          <a-input v-model:value="form.name" placeholder="请输入名称" />
        </a-form-item>
        <a-form-item label="技能工具类型" v-bind="validateInfos.skillToolType">
          <a-select v-model:value="form.skillToolType" placeholder="请选择技能工具类型">
            <a-select-option :value="1">Tool</a-select-option>
            <a-select-option :value="2">Skill</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="方法" v-bind="validateInfos.classMethod">
          <a-input v-model:value="form.classMethod" placeholder="请输入方法" />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 2" label="技能文件" v-bind="validateInfos.skillFile">
          <FileUpload
            business="AISkillToolManagement"
            :keyValue="form.id"
            sign="SkillZip"
            accept=".zip"
            :multiple="false"
            :maxCount="1"
            :initialFiles="form.skillFile ? [form.skillFile] : []"
            uploadButtonText="上传技能压缩包"
            @upload-success="onFileUploadSuccess"
            @upload-error="onFileUploadError"
            @zip-updated="onZipUpdated"
          />
        </a-form-item>
        <a-form-item label="描述">
          <a-textarea v-model:value="form.description" :rows="4" placeholder="请输入描述" :maxlength="500" show-count />
        </a-form-item>
        <a-form-item label="启用状态" v-bind="validateInfos.activeStatus">
          <a-select v-model:value="form.activeStatus" placeholder="请选择启用状态">
            <a-select-option :value="1">启用</a-select-option>
            <a-select-option :value="0">禁用</a-select-option>
          </a-select>
        </a-form-item>
      </a-form>
    </a-modal>

    <a-modal
      v-model:open="viewModalVisible"
      title="预览技能工具"
      :footer="null"
      width="600px"
    >
      <a-descriptions :column="1" bordered>
        <a-descriptions-item label="名称">{{ viewItem?.name }}</a-descriptions-item>
        <a-descriptions-item label="类型">{{ getSkillToolTypeName(viewItem?.skillToolType) }}</a-descriptions-item>
        <a-descriptions-item label="方法">{{ viewItem?.classMethod }}</a-descriptions-item>
        <a-descriptions-item label="描述">{{ viewItem?.description }}</a-descriptions-item>
        <a-descriptions-item label="启用状态">
          <a-tag :color="viewItem?.activeStatus === 1 ? 'green' : 'red'">
            {{ viewItem?.activeStatus === 1 ? '启用' : '禁用' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="系统内置">
          <a-tag :color="viewItem?.isSystem ? 'blue' : 'default'">
            {{ viewItem?.isSystem ? '是' : '否' }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="技能文件">
          <a v-if="viewItem?.skillFile" :href="viewItem?.skillFile.url" target="_blank">
            {{ viewItem?.skillFile.name }}
          </a>
          <span v-else>无</span>
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>
  </div>
</template>

<script setup>
import "../../css/CardTable.css";
import { ref, reactive, computed, watch, onMounted } from "vue";
import {
  ToolOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined,
  EyeOutlined,
} from "@ant-design/icons-vue";
import { message, Modal } from "ant-design-vue";
import { Form } from "ant-design-vue";
import {
  getAISkillToolManagementPageData, 
  addEditAISkillToolManagement,
  deleteAISkillToolManagement,
} from "@/api/ai/aiskilltoolManagement";
import { GetSnowflakeId } from "@/api/baseapi";
import FileUpload from "@/components/FileUpload.vue";

const useForm = Form.useForm;

const dataList = ref([]);

const pagination = reactive({
  current: 1,
  pageSize: 8,
  total: 0,
});

const modalVisible = ref(false);
const confirmLoading = ref(false);
const modalTitle = ref("添加技能工具");

const currentRecord = ref(null);

const viewModalVisible = ref(false);
const viewItem = ref(null);

const form = reactive({
  id: "",
  name: "",
  classMethod: "",
  description: "",
  activeStatus: 1,
  skillToolType: undefined,
  skillFile: null,
});

const validateSkillFile = (rule, value) => {
  if (form.skillToolType !== 2) {
    return Promise.resolve();
  }
  if (!value || !value.id) {
    return Promise.reject("请上传技能压缩包");
  }
  return Promise.resolve();
};

const rules = computed(() => ({
  name: [{ required: true, message: "请输入名称" }],
  classMethod: form.skillToolType === 1 ? [{ required: true, message: "请输入方法" }] : [],
  skillToolType: [{ required: true, message: "请选择技能工具类型" }],
  activeStatus: [{ required: true, message: "请选择启用状态" }],
  skillFile: form.skillToolType === 2 ? [{ required: true, validator: validateSkillFile, trigger: "change" }] : [],
}));

const { validate: validateForm, validateInfos, clearValidate } = useForm(form, rules);

watch(() => form.skillToolType, () => {
  clearValidate(["classMethod", "skillFile"]);
});

const searchKeyword = ref("");
const filterType = ref(2);

const getSkillToolTypeName = (type) => {
  const types = { 1: "Tool", 2: "Skill" };
  return types[type] || "未知";
};

const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.current = 1;
  loadData();
};

const onFilterChange = () => {
  pagination.current = 1;
  loadData();
};

const loadData = async () => {
  try {
    const params = {
      pageNum: pagination.current,
      pageSize: pagination.pageSize,
      searchKey: searchKeyword.value,
      parameter: filterType.value,
    };

    const response = await getAISkillToolManagementPageData(params);
    if (response && response.code === 200 && response.data.data) {
      dataList.value = response.data.data.map((item) => ({
        ...item,
        key: item.id,
      }));
      pagination.total = response.data.total;
      if (dataList.value.length === 0 && pagination.current > 1) {
        pagination.current -= 1;
        loadData();
      }
    }
  } catch (error) {
    console.error("加载数据失败:", error);
    message.error("加载数据失败: " + (error.message || "未知错误"));
  }
};

const showAddModal = async () => {
  modalTitle.value = "添加技能工具";
  currentRecord.value = null;
  try {
    const snowflakeId = await GetSnowflakeId();
    Object.assign(form, {
      id: snowflakeId.data,
      name: "",
      classMethod: "",
      description: "",
      activeStatus: 1,
      skillToolType: undefined,
      skillFile: null,
    });
  } catch (error) {
    console.error("获取ID失败:", error);
    Object.assign(form, {
      id: "",
      name: "",
      classMethod: "",
      description: "",
      activeStatus: 1,
      skillToolType: undefined,
      skillFile: null,
    });
  }
  modalVisible.value = true;
};

const onFileUploadSuccess = (data) => {
  form.skillFile = {
    id: data.fileId,
    name: data.fileName,
    path: data.path || "",
    url: data.url || "",
    size: data.fileSize || 0
  };
  message.success("技能文件上传成功");
};

const onFileUploadError = (data) => {
  console.error("文件上传失败:", data.error);
  message.error("文件上传失败: " + (data.error?.message || "未知错误"));
};

const onZipUpdated = (data) => {
  form.skillFile = {
    id: data.file.id,
    name: data.file.name,
    path: data.file.path || "",
    url: data.file.url || "",
    size: data.file.size || 0
  };
  message.success("技能文件更新成功");
};

const showViewModal = (record) => {
  viewItem.value = record;
  viewModalVisible.value = true;
};

const showEditModal = (record) => {
  if (record.isSystem) return;
  modalTitle.value = "编辑技能工具";
  currentRecord.value = record;
  form.id = record.id || "";
  form.name = record.name || "";
  form.classMethod = record.classMethod || "";
  form.description = record.description || "";
  form.activeStatus = record.activeStatus !== undefined ? record.activeStatus : 1;
  form.skillToolType = record.skillToolType !== undefined ? record.skillToolType : undefined;
  form.skillFile = record.skillFile || null;
  modalVisible.value = true;
};

const showDeleteConfirm = (record) => {
  if (record.isSystem) {
    message.warning("系统内置工具不允许删除");
    return;
  }
  Modal.confirm({
    title: "确认删除",
    content: `确定要删除技能工具"${record.name}"吗？`,
    okText: "确认",
    cancelText: "取消",
    onOk: () => handleDelete(record.id, record.name),
  });
};

const handleDelete = async (id, name) => {
  try {
    await deleteAISkillToolManagement(id);
    message.success(`技能工具"${name}"删除成功`);
    loadData();
  } catch (error) {
    console.error("删除失败:", error);
    message.error("删除失败: " + (error.message || "未知错误"));
  }
};

const handlePageChange = (page, pageSize) => {
  pagination.current = page;
  pagination.pageSize = pageSize;
  loadData();
};

const handleModalOk = () => {
  validateForm()
    .then(async () => {
      confirmLoading.value = true;
      try {
        await addEditAISkillToolManagement(currentRecord.value ? {
          id: form.id,
          name: form.name,
          classMethod: form.classMethod,
          description: form.description,
          activeStatus: form.activeStatus,
          skillToolType: form.skillToolType,
        } : {
          id: form.id,
          name: form.name,
          classMethod: form.classMethod,
          description: form.description,
          activeStatus: form.activeStatus,
          skillToolType: form.skillToolType,
        });

        message.success(currentRecord.value ? "更新成功" : "添加成功");
        modalVisible.value = false;
        loadData();
      } catch (error) {
        console.error("保存失败:", error);
        message.error("保存失败: " + (error.message || "未知错误"));
      } finally {
        confirmLoading.value = false;
      }
    })
    .catch((err) => {
      console.log("表单验证失败:", err);
    });
};

const handleModalCancel = () => {
  modalVisible.value = false;
};

onMounted(() => {
  loadData();
});
</script>
