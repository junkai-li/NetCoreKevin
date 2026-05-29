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
          <div class="skill-file-upload">
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
            />
            <a-button
              v-if="form.skillFile && form.skillFile.url"
              @click="openSkillEditFromForm"
              style="margin-left: 8px"
            >
              <EyeOutlined /> 预览编辑
            </a-button>
          </div>
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

    <a-modal
      v-model:open="skillEditModalVisible"
      title="编辑技能文件"
      width="90%"
      style="top: 20px"
      @cancel="handleSkillEditCancel"
      :footer="null"
      :maskClosable="false"
    >
      <div class="skill-edit-container">
        <div class="skill-edit-header">
          <div class="skill-info">
            <span class="skill-name">{{ currentSkillItem?.name }}</span>
            <a-tag :color="currentSkillItem?.activeStatus === 1 ? 'green' : 'red'">
              {{ currentSkillItem?.activeStatus === 1 ? '启用' : '禁用' }}
            </a-tag>
          </div>
          <div class="skill-edit-actions">
            <a-button
              type="primary"
              :loading="skillEditLoading"
              :disabled="!hasSkillFileChanges"
              @click="handleSaveSkillZip"
            >
              <template #icon><SaveOutlined /></template>
              保存并上传
            </a-button>
            <a-button @click="handleDownloadZip" :disabled="skillEditLoading">
              <template #icon><UploadOutlined /></template>
              下载Zip
            </a-button>
            <a-button @click="handleSkillEditCancel">
              <template #icon><CloseOutlined /></template>
              关闭
            </a-button>
          </div>
        </div>
        <div class="skill-edit-content">
          <div class="skill-file-tree">
            <div class="tree-header">
              <span>文件列表</span>
              <a-badge v-if="hasSkillFileChanges" status="warning" text="已修改" />
            </div>
            <div class="tree-content">
              <a-empty v-if="skillFileTree.length === 0 && !skillEditLoading" description="暂无文件" />
              <a-spin v-if="skillEditLoading" tip="加载中..." />
              <div v-else>
                <div
                  v-for="file in skillFileTree"
                  :key="file.path"
                  class="tree-item"
                  :class="{
                    'file-item-directory': file.isDirectory,
                    'selected': selectedSkillFile?.path === file.path
                  }"
                  :style="{ paddingLeft: (file.level * 16 + 12) + 'px' }"
                  @click="handleFileItemClick(file)"
                >
                  <component
                    :is="file.isDirectory ? (file.expanded ? FolderOpenOutlined : FolderOutlined) : getFileIcon(file.name)"
                    :class="['file-icon', { 'icon-directory': file.isDirectory, 'markdown-icon': file.isMarkdown, 'javascript-icon': file.isJavascript }]"
                  />
                  <span class="file-name" :title="file.path">{{ file.name }}</span>
                  <span v-if="skillZipContent[file.path]?.modified" class="modified-dot"></span>
                </div>
              </div>
            </div>
          </div>
          <div class="skill-editor-area">
            <div class="editor-header" v-if="selectedSkillFile">
              <span class="editor-file-name">{{ selectedSkillFile.path }}</span>
              <a-tag v-if="hasSkillFileChanges && skillZipContent[selectedSkillFile.path]?.modified" color="orange">已修改</a-tag>
            </div>
            <div class="editor-content" ref="skillEditorContainer">
              <a-empty v-if="!selectedSkillFile" description="请选择要编辑的文件" />
            </div>
          </div>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import "../../css/CardTable.css";
import { ref, reactive, computed, watch, onMounted, nextTick } from "vue";
import {
  ToolOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined,
  EyeOutlined,
  FileOutlined,
  FileTextOutlined,
  FolderOutlined,
  FolderOpenOutlined,
  SaveOutlined,
  CloseOutlined,
  UploadOutlined,
} from "@ant-design/icons-vue";
import { message, Modal } from "ant-design-vue";
import { Form } from "ant-design-vue";
import {
  getAISkillToolManagementPageData,
  getAISkillToolManagementById,
  addEditAISkillToolManagement,
  deleteAISkillToolManagement,
} from "@/api/ai/aiskilltoolManagement";
import { GetSnowflakeId } from "@/api/baseapi";
import { uploadFile, deleteFileById } from "@/api/file";
import FileUpload from "@/components/FileUpload.vue";
import JSZip from "jszip";
import { saveAs } from "file-saver";
import { EditorView, basicSetup } from "codemirror";
import { EditorState } from "@codemirror/state";
import { javascript } from "@codemirror/lang-javascript";
import { markdown } from "@codemirror/lang-markdown";
import { oneDark } from "@codemirror/theme-one-dark";

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

const skillEditModalVisible = ref(false);
const skillEditLoading = ref(false);
const currentSkillItem = ref(null);
const skillFileTree = ref([]);
const skillZipContent = ref({});
const selectedSkillFile = ref(null);
const skillEditorContainer = ref(null);
const skillEditorView = ref(null);
const hasSkillFileChanges = ref(false);

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

const showViewModal = (record) => {
  viewItem.value = record;
  if (record.skillToolType === 2 && record.skillFile && record.skillFile.url) {
    showSkillEditModal(record);
  } else {
    viewModalVisible.value = true;
  }
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

const getFileExtension = (filename) => {
  const parts = filename.split(".");
  return parts.length > 1 ? parts[parts.length - 1].toLowerCase() : "";
};

const isMarkdownFile = (filename) => {
  const ext = getFileExtension(filename);
  return ext === "md" || ext === "markdown";
};

const isJavascriptFile = (filename) => {
  const ext = getFileExtension(filename);
  return ext === "js" || ext === "jsx" || ext === "ts" || ext === "tsx";
};

const getFileIcon = (fileName) => {
  if (isMarkdownFile(fileName)) return FileTextOutlined;
  return FileOutlined;
};

const handleFileItemClick = (item) => {
  if (item.isDirectory) {
    const newExpanded = new Set(expandedFolders.value);
    if (newExpanded.has(item.path)) {
      newExpanded.delete(item.path);
    } else {
      newExpanded.add(item.path);
    }
    expandedFolders.value = newExpanded;
    rebuildFileTree();
  } else {
    selectSkillFile(item);
  }
};

const skillFileTreeOriginal = ref([]);
const expandedFolders = ref(new Set());

const rebuildFileTree = () => {
  const result = [];
  const processedPaths = new Set();

  const flatten = (items, level) => {
    items.forEach((item) => {
      if (processedPaths.has(item.path)) return;
      processedPaths.add(item.path);

      const isExpanded = expandedFolders.value.has(item.path);
      result.push({ ...item, level, isExpanded });

      if (item.isDirectory && isExpanded && item.children && item.children.length > 0) {
        flatten(item.children, level + 1);
      }
    });
  };

  flatten(skillFileTreeOriginal.value, 0);

  const rootLevelItems = skillFileTreeOriginal.value.filter(
    (item) => !item.parentPath || item.parentPath === ""
  );
  rootLevelItems.forEach((item) => {
    if (!processedPaths.has(item.path)) {
      processedPaths.add(item.path);
      const isExpanded = expandedFolders.value.has(item.path);
      result.push({ ...item, level: 0, isExpanded });
    }
  });

  skillFileTree.value = result;
};

const showSkillEditModal = async (record) => {
  if (record.skillToolType !== 2 || !record.skillFile || !record.skillFile.url) {
    message.warning("该技能工具没有可编辑的技能文件");
    return;
  }

  currentSkillItem.value = record;
  skillEditModalVisible.value = true;
  skillEditLoading.value = true;
  skillFileTree.value = [];
  skillZipContent.value = {};
  selectedSkillFile.value = null;
  hasSkillFileChanges.value = false;

  try {
    const response = await fetch(record.skillFile.url);
    const blob = await response.blob();
    const zip = await JSZip.loadAsync(blob);

    const filePromises = [];
    const contentMap = {};
    const folderSet = new Set();

    zip.forEach((relativePath, file) => {
      const parts = relativePath.split("/");
      for (let i = 1; i < parts.length - 1; i++) {
        folderSet.add(parts.slice(0, i).join("/"));
      }

      if (!file.dir) {
        const filePromise = file.async("string").then((content) => {
          contentMap[relativePath] = {
            content: content,
            originalContent: content,
            modified: false,
          };
        });
        filePromises.push(filePromise);
      }
    });

    await Promise.all(filePromises);

    const treeData = [];

    folderSet.forEach((folderPath) => {
      const parts = folderPath.split("/");
      const name = parts[parts.length - 1];
      const parentPath = parts.length > 1 ? parts.slice(0, -1).join("/") : "";
      treeData.push({
        name: name,
        path: folderPath,
        isDirectory: true,
        isExpanded: false,
        children: [],
        parentPath: parentPath,
      });
    });

    zip.forEach((relativePath, file) => {
      if (!file.dir) {
        const name = relativePath.split("/").pop();
        const parts = relativePath.split("/");
        const parentPath = parts.length > 1 ? parts.slice(0, -1).join("/") : "";

        treeData.push({
          name: name,
          path: relativePath,
          isDirectory: false,
          isMarkdown: isMarkdownFile(name),
          isJavascript: isJavascriptFile(name),
          modified: false,
          parentPath: parentPath,
          children: null,
        });
      }
    });

    const buildTree = (data) => {
      const pathToItem = new Map();

      data.forEach((item) => {
        pathToItem.set(item.path, { ...item, children: [] });
      });

      const rootItems = [];

      pathToItem.forEach((item) => {
        if (item.parentPath && pathToItem.has(item.parentPath)) {
          pathToItem.get(item.parentPath).children.push(item);
        } else {
          rootItems.push(item);
        }
      });

      return rootItems;
    };

    const flattenTree = (items, level = 0) => {
      const result = [];
      items.forEach((item) => {
        result.push({ ...item, level });
        if (item.isDirectory && item.isExpanded && item.children && item.children.length > 0) {
          result.push(...flattenTree(item.children, level + 1));
        }
      });
      return result;
    };

    const treeItems = buildTree(treeData);
    skillFileTreeOriginal.value = treeItems;
    skillFileTree.value = flattenTree(treeItems);
    skillZipContent.value = contentMap;

    if (skillFileTree.value.length > 0) {
      await nextTick();
      const firstFile = skillFileTree.value.find((item) => !item.isDirectory);
      if (firstFile) {
        selectSkillFile(firstFile);
      }
    }
  } catch (error) {
    console.error("加载技能文件失败:", error);
    message.error("加载技能文件失败: " + (error.message || "未知错误"));
  } finally {
    skillEditLoading.value = false;
  }
};

const selectSkillFile = async (file) => {
  if (selectedSkillFile.value?.path === file.path) return;

  selectedSkillFile.value = file;

  await nextTick();

  if (skillEditorView.value) {
    skillEditorView.value.destroy();
    skillEditorView.value = null;
  }

  if (skillEditorContainer.value) {
    skillEditorContainer.value.innerHTML = "";
  }

  if (!skillEditorContainer.value) return;

  const fileData = skillZipContent.value[file.path];
  if (!fileData) {
    console.error("文件数据不存在:", file.path, skillZipContent.value);
    return;
  }

  const languageExtension = file.isMarkdown ? markdown() : file.isJavascript ? javascript() : [];

  const state = EditorState.create({
    doc: fileData.content,
    extensions: [
      basicSetup,
      languageExtension,
      oneDark,
      EditorView.updateListener.of((update) => {
        if (update.docChanged) {
          const newContent = update.state.doc.toString();
          skillZipContent.value[file.path] = {
            ...skillZipContent.value[file.path],
            content: newContent,
            modified: newContent !== skillZipContent.value[file.path].originalContent,
          };
          checkHasChanges();
        }
      }),
      EditorView.theme({
        "&": {
          height: "100%",
          fontSize: "13px",
        },
        ".cm-scroller": {
          overflow: "auto",
        },
      }),
    ],
  });

  skillEditorView.value = new EditorView({
    state: state,
    parent: skillEditorContainer.value,
  });
};

const checkHasChanges = () => {
  hasSkillFileChanges.value = Object.values(skillZipContent.value).some((f) => f.modified);
};

const handleSaveSkillZip = async () => {
  if (!currentSkillItem.value) return;

  skillEditLoading.value = true;

  try {
    const zip = new JSZip();

    const entries = Object.entries(skillZipContent.value);
    console.log("保存文件数量:", entries.length);

    for (const [path, data] of entries) {
      console.log("添加文件:", path, "内容长度:", data.content ? data.content.length : 0);
      zip.file(path, data.content);
    }

    const blob = await zip.generateAsync({ type: "blob" });
    console.log("生成的zip大小:", blob.size);
    const fileName = currentSkillItem.value.skillFile?.name || "skill.zip";

    if (currentSkillItem.value.skillFile?.id) {
      try {
        await deleteFileById(currentSkillItem.value.skillFile.id);
        console.log("旧文件已删除");
      } catch (deleteError) {
        console.error("删除旧文件失败:", deleteError);
      }
    }

    const result = await uploadFile(
      "AISkillToolManagement",
      currentSkillItem.value.id,
      "SkillZip",
      new File([blob], fileName)
    );

    console.log("上传结果:", result);

    if (result.code === 200 && result.data) {
      const newFileId = typeof result.data === 'string' ? result.data : result.data.fileId || result.data.id;

      if (!newFileId) {
        message.error("上传失败：未获取到文件ID");
        return;
      }

      const originalSkillFile = currentSkillItem.value.skillFile || {};

      const newSkillFile = {
        id: newFileId,
        name: originalSkillFile.name || "skill.zip",
        url: originalSkillFile.url || "",
        size: blob.size
      };

      console.log("newSkillFile:", newSkillFile);

      form.skillFile = newSkillFile;
      currentSkillItem.value.skillFile = newSkillFile;

      await nextTick();
      clearValidate(["skillFile"]);

      for (const path in skillZipContent.value) {
        skillZipContent.value[path] = {
          ...skillZipContent.value[path],
          originalContent: skillZipContent.value[path].content,
          modified: false,
        };
      }

      hasSkillFileChanges.value = false;

      const itemId = currentSkillItem.value.id;

      closeSkillEditModal();

      try {
        const detailRes = await getAISkillToolManagementById(itemId);
        if (detailRes.code === 200 && detailRes.data) {
          const updatedItem = detailRes.data;
          form.skillFile = updatedItem.skillFile || form.skillFile;
          Object.assign(form, {
            name: updatedItem.name,
            classMethod: updatedItem.classMethod,
            description: updatedItem.description,
            activeStatus: updatedItem.activeStatus,
          });
        }
      } catch (detailError) {
        console.error("获取详情失败:", detailError);
      } 
      message.success("技能文件保存成功"); 
    } else {
      message.error(result.msg || result.message || "上传失败");
    }
  } catch (error) {
    console.error("保存技能文件失败:", error);
    message.error("保存技能文件失败: " + (error.message || "未知错误"));
  } finally {
    skillEditLoading.value = false;
  }
};

const handleDownloadZip = async () => {
  if (!currentSkillItem.value) return;

  try {
    const zip = new JSZip();

    for (const [path, data] of Object.entries(skillZipContent.value)) {
      zip.file(path, data.content);
    }

    const blob = await zip.generateAsync({ type: "blob" });
    const fileName = currentSkillItem.value.skillFile?.name || "skill.zip";

    saveAs(blob, fileName);
    message.success("下载成功");
  } catch (error) {
    console.error("下载失败:", error);
    message.error("下载失败: " + (error.message || "未知错误"));
  }
};

const handleSkillEditCancel = () => {
  if (hasSkillFileChanges.value) {
    Modal.confirm({
      title: "确认关闭",
      content: "有未保存的修改，确定要关闭吗？",
      okText: "确定",
      cancelText: "取消",
      onOk: () => {
        closeSkillEditModal();
      },
    });
  } else {
    closeSkillEditModal();
  }
};

const openSkillEditFromForm = async () => {
  if (!form.skillFile || !form.skillFile.url) {
    message.warning("请先上传技能压缩包");
    return;
  }

  currentSkillItem.value = {
    id: form.id,
    name: form.name,
    skillFile: form.skillFile,
    activeStatus: form.activeStatus,
  };

  skillEditModalVisible.value = true;
  skillEditLoading.value = true;
  skillFileTree.value = [];
  skillZipContent.value = {};
  selectedSkillFile.value = null;
  hasSkillFileChanges.value = false;

  try {
    const response = await fetch(form.skillFile.url);
    const blob = await response.blob();
    const zip = await JSZip.loadAsync(blob);

    const filePromises = [];
    const contentMap = {};
    const folderSet = new Set();

    zip.forEach((relativePath, file) => {
      const parts = relativePath.split("/");
      for (let i = 1; i < parts.length - 1; i++) {
        folderSet.add(parts.slice(0, i).join("/"));
      }

      if (!file.dir) {
        const filePromise = file.async("string").then((content) => {
          contentMap[relativePath] = {
            content: content,
            originalContent: content,
            modified: false,
          };
        });
        filePromises.push(filePromise);
      }
    });

    await Promise.all(filePromises);

    const treeData = [];

    folderSet.forEach((folderPath) => {
      const parts = folderPath.split("/");
      const name = parts[parts.length - 1];
      const parentPath = parts.length > 1 ? parts.slice(0, -1).join("/") : "";
      treeData.push({
        name: name,
        path: folderPath,
        isDirectory: true,
        isExpanded: false,
        children: [],
        parentPath: parentPath,
      });
    });

    zip.forEach((relativePath, file) => {
      if (!file.dir) {
        const name = relativePath.split("/").pop();
        const parts = relativePath.split("/");
        const parentPath = parts.length > 1 ? parts.slice(0, -1).join("/") : "";

        treeData.push({
          name: name,
          path: relativePath,
          isDirectory: false,
          isMarkdown: isMarkdownFile(name),
          isJavascript: isJavascriptFile(name),
          modified: false,
          parentPath: parentPath,
          children: null,
        });
      }
    });

    const buildTree = (data) => {
      const pathToItem = new Map();

      data.forEach((item) => {
        pathToItem.set(item.path, { ...item, children: [] });
      });

      const rootItems = [];

      pathToItem.forEach((item) => {
        if (item.parentPath && pathToItem.has(item.parentPath)) {
          pathToItem.get(item.parentPath).children.push(item);
        } else {
          rootItems.push(item);
        }
      });

      return rootItems;
    };

    const flattenTree = (items, level = 0) => {
      const result = [];
      items.forEach((item) => {
        result.push({ ...item, level });
        if (item.isDirectory && item.isExpanded && item.children && item.children.length > 0) {
          result.push(...flattenTree(item.children, level + 1));
        }
      });
      return result;
    };

    const treeItems = buildTree(treeData);
    skillFileTreeOriginal.value = treeItems;
    skillFileTree.value = flattenTree(treeItems);
    skillZipContent.value = contentMap;

    if (skillFileTree.value.length > 0) {
      await nextTick();
      const firstFile = skillFileTree.value.find((item) => !item.isDirectory);
      if (firstFile) {
        selectSkillFile(firstFile);
      }
    }
  } catch (error) {
    console.error("加载技能文件失败:", error);
    message.error("加载技能文件失败: " + (error.message || "未知错误"));
  } finally {
    skillEditLoading.value = false;
  }
};

const closeSkillEditModal = () => {
  if (skillEditorView.value) {
    skillEditorView.value.destroy();
    skillEditorView.value = null;
  }

  skillEditModalVisible.value = false;
  currentSkillItem.value = null;
  skillFileTree.value = [];
  skillFileTreeOriginal.value = [];
  expandedFolders.value = new Set();
  skillZipContent.value = {};
  selectedSkillFile.value = null;
  hasSkillFileChanges.value = false;
};
</script>

<style scoped>
.skill-edit-container {
  display: flex;
  flex-direction: column;
  height: 70vh;
}

.skill-edit-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 16px;
  border-bottom: 1px solid #f0f0f0;
  margin-bottom: 16px;
}

.skill-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.skill-name {
  font-size: 16px;
  font-weight: 600;
}

.skill-edit-actions {
  display: flex;
  gap: 8px;
}

.skill-edit-content {
  display: flex;
  flex: 1;
  gap: 16px;
  min-height: 0;
}

.skill-file-tree {
  width: 280px;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  display: flex;
  flex-direction: column;
}

.tree-header {
  padding: 12px 16px;
  background: #fafafa;
  border-bottom: 1px solid #f0f0f0;
  font-weight: 500;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tree-content {
  flex: 1;
  overflow-y: auto;
  padding: 8px;
}

.tree-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  cursor: pointer;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.tree-item:hover {
  background-color: #f5f5f5;
}

.tree-item.selected {
  background-color: #e6f7ff;
  color: #1890ff;
}

.tree-item .file-name {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.tree-item .markdown-icon {
  color: #42b983;
}

.tree-item .javascript-icon {
  color: #f7df1e;
}

.skill-editor-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  min-width: 0;
}

.editor-header {
  padding: 8px 16px;
  background: #fafafa;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
  gap: 12px;
}

.editor-file-name {
  font-weight: 500;
  color: #333;
}

.editor-content {
  flex: 1;
  overflow: hidden;
  position: relative;
}

.editor-content .ant-empty {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.editor-content :deep(.cm-editor) {
  height: 100%;
}

.editor-content :deep(.cm-scroller) {
  overflow: auto;
}

.skill-file-upload {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
}
</style>
