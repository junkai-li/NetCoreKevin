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
              <a-select-option :value="3">Mcp</a-select-option>
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
      :ok-text="skillScanning ? 'AI安全扫描中，请稍候...' : undefined"
      :closable="!skillScanning"
      :mask-closable="!skillScanning"
      :keyboard="!skillScanning"
      :cancel-button-props="skillScanning ? { style: { display: 'none' } } : {}"
      width="820px"
    >
      <a-alert
        v-if="skillScanning"
        type="warning"
        show-icon
        :closable="false"
        style="margin-bottom: 16px"
      >
        <template #icon>
          <LoadingOutlined spin />
        </template>
        <template #message>
          <span>AI安全扫描进行中，已用时 {{ scanElapsedText }}</span>
        </template>
        <template #description>
          Skill技能包需由AI完成安全性扫描，耗时较长，扫描期间本编辑页不可修改，请勿关闭窗口或刷新页面。
        </template>
      </a-alert>
      <a-form :model="form" :disabled="skillScanning" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="名称" v-bind="validateInfos.name">
          <a-input v-model:value="form.name" placeholder="请输入名称" />
        </a-form-item>
        <a-form-item label="技能工具类型" v-bind="validateInfos.skillToolType">
          <a-select v-model:value="form.skillToolType" placeholder="请选择技能工具类型">
            <a-select-option :value="1">Tool</a-select-option>
            <a-select-option :value="2">Skill</a-select-option>
            <a-select-option :value="3">Mcp</a-select-option>
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
            :disabled="skillScanning"
            :initialFiles="form.skillFile ? [form.skillFile] : []"
            uploadButtonText="上传技能压缩包"
            @upload-success="onFileUploadSuccess"
            @upload-error="onFileUploadError"
            @zip-updated="onZipUpdated"
          />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3" label="Mcp地址" v-bind="validateInfos.mcpUrl">
          <a-input v-model:value="form.mcpUrl" placeholder="请输入Mcp地址" />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3" label="Mcp类型" v-bind="validateInfos.mcpType">
          <a-select v-model:value="form.mcpType" placeholder="请选择Mcp类型">
            <a-select-option value="https">https</a-select-option>
            <a-select-option value="sse">sse</a-select-option>
            <a-select-option value="stdio">stdio</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3" label="McpHeaders" v-bind="validateInfos.mcpHeaders">
          <a-textarea v-model:value="form.mcpHeaders" :rows="3" placeholder="键值对Json格式，例如: {&quot;Authorization&quot;: &quot;Bearer xxx&quot;}" />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3 && form.mcpType === 'stdio'" label="McpCommand" v-bind="validateInfos.mcpCommand">
          <a-input v-model:value="form.mcpCommand" placeholder="请输入McpCommand" />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3 && form.mcpType === 'stdio'" label="McpArguments">
          <a-input v-model:value="form.mcpArguments" placeholder=",分隔，例如: arg1,arg2" />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3 && form.mcpType === 'stdio'" label="McpEnvironment">
          <a-textarea v-model:value="form.mcpEnvironment" :rows="3" placeholder="键值对Json格式" />
        </a-form-item>
        <a-form-item v-if="form.skillToolType === 3" label="Mcp工具" v-bind="validateInfos.mcpSelectedTools">
          <div style="margin-bottom: 8px">
            <a-button type="primary" ghost :loading="mcpTesting" :disabled="skillScanning" @click="handleTestMcp">
              <template #icon>
                <ApiOutlined />
              </template>
              测试连接
            </a-button>
            <a-tag v-if="mcpTested" color="green" style="margin-left: 12px">
              连接成功，共 {{ mcpToolList.length }} 个工具
            </a-tag>
            <span style="margin-left: 12px; color: #999; font-size: 12px">
              测试连接后勾选需要启用的工具，AI 只会加载勾选的工具
            </span>
          </div>
          <a-spin :spinning="mcpTesting">
            <div v-if="mcpToolList.length > 0" class="mcp-tool-list">
              <a-input-search
                v-model:value="mcpToolKeyword"
                placeholder="搜索Mcp工具名称或描述"
                allow-clear
                style="margin-bottom: 8px"
              />
              <div style="margin-bottom: 8px; border-bottom: 1px solid #f0f0f0; padding-bottom: 6px">
                <a-checkbox
                  :checked="allFilteredChecked"
                  :indeterminate="someFilteredChecked && !allFilteredChecked"
                  :disabled="filteredMcpToolList.length === 0"
                  @change="onCheckAllMcpTools"
                >
                  全选（已选 {{ form.mcpSelectedTools.length }}/{{ mcpToolList.length }}）
                </a-checkbox>
              </div>
              <a-checkbox-group v-model:value="form.mcpSelectedTools" style="width: 100%">
                <div v-for="tool in filteredMcpToolList" :key="tool.name" style="margin-bottom: 6px">
                  <a-checkbox :value="tool.name">
                    <span style="font-weight: 500">{{ tool.name }}</span>
                    <span v-if="tool.description" style="color: #999; margin-left: 8px">{{ tool.description }}</span>
                  </a-checkbox>
                </div>
              </a-checkbox-group>
              <div v-if="filteredMcpToolList.length === 0" style="color: #999; padding: 8px 0">
                没有匹配“{{ mcpToolKeyword }}”的工具
              </div>
            </div>
            <div v-else style="color: #999; padding: 8px 0">
              尚未测试连接，请点击“测试连接”加载该 Mcp 服务下的工具列表
            </div>
          </a-spin>

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
        <a-descriptions-item label="Mcp地址" v-if="viewItem?.skillToolType === 3">{{ viewItem?.mcpUrl || '无' }}</a-descriptions-item>
        <a-descriptions-item label="Mcp类型" v-if="viewItem?.skillToolType === 3">{{ viewItem?.mcpType || '无' }}</a-descriptions-item>
        <a-descriptions-item label="McpHeaders" v-if="viewItem?.skillToolType === 3">{{ viewItem?.mcpHeaders || '无' }}</a-descriptions-item>
        <a-descriptions-item label="McpCommand" v-if="viewItem?.skillToolType === 3 && viewItem?.mcpType === 'stdio'">{{ viewItem?.mcpCommand || '无' }}</a-descriptions-item>
        <a-descriptions-item label="McpArguments" v-if="viewItem?.skillToolType === 3 && viewItem?.mcpType === 'stdio'">{{ viewItem?.mcpArguments || '无' }}</a-descriptions-item>
        <a-descriptions-item label="McpEnvironment" v-if="viewItem?.skillToolType === 3 && viewItem?.mcpType === 'stdio'">{{ viewItem?.mcpEnvironment || '无' }}</a-descriptions-item>
        <a-descriptions-item label="启用工具" v-if="viewItem?.skillToolType === 3">
          <template v-if="parseSelectedTools(viewItem?.mcpSelectedTools).length > 0">
            <a-tag v-for="t in parseSelectedTools(viewItem?.mcpSelectedTools)" :key="t" color="blue" style="margin-bottom: 4px">{{ t }}</a-tag>
          </template>
          <span v-else>无</span>
        </a-descriptions-item>
      </a-descriptions>
    </a-modal>
  </div>
</template>

<script setup>
import "../../css/CardTable.css";
import { ref, reactive, computed, watch, onMounted, onUnmounted, nextTick, h } from "vue";
import {
  ToolOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined,
  EyeOutlined,
  LoadingOutlined,
  ApiOutlined,
} from "@ant-design/icons-vue";
import { message, Modal } from "ant-design-vue";
import { Form } from "ant-design-vue";
import {
  getAISkillToolManagementPageData, 
  addEditAISkillToolManagement,
  deleteAISkillToolManagement,
  testMcpConnection,
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

// Skill类型提交后后台会做AI安全扫描，耗时较长，扫描期间锁定编辑弹窗
const skillScanning = ref(false);
const scanSeconds = ref(0);
let scanTimer = null;

const scanElapsedText = computed(() => {
  const minutes = Math.floor(scanSeconds.value / 60);
  const seconds = scanSeconds.value % 60;
  return minutes > 0 ? `${minutes}分${seconds}秒` : `${seconds}秒`;
});

const startScanTimer = () => {
  stopScanTimer();
  scanSeconds.value = 0;
  scanTimer = setInterval(() => {
    scanSeconds.value += 1;
  }, 1000);
};

const stopScanTimer = () => {
  if (scanTimer) {
    clearInterval(scanTimer);
    scanTimer = null;
  }
};

const form = reactive({
  id: "",
  name: "",
  classMethod: "",
  description: "",
  activeStatus: 1,
  skillToolType: undefined,
  skillFile: null,
  mcpUrl: "",
  mcpType: "",
  mcpHeaders: "",
  mcpCommand: "",
  mcpArguments: "",
  mcpEnvironment: "",
  mcpSelectedTools: [],
});

// Mcp测试连接状态：mcpToolList为测试返回的全部工具，mcpTested标记本次是否测试成功
const mcpTesting = ref(false);
const mcpTested = ref(false);
const mcpToolList = ref([]);
// Mcp工具搜索关键字（按名称/描述过滤，仅影响展示，不影响已勾选结果）
const mcpToolKeyword = ref("");
// 编辑回显期间抑制“配置变更即清空测试结果”的watch，避免刚回填就被清掉
const mcpRestoring = ref(false);

// 按关键字过滤后的Mcp工具列表（不区分大小写，匹配名称或描述）
const filteredMcpToolList = computed(() => {
  const kw = (mcpToolKeyword.value || "").trim().toLowerCase();
  if (!kw) return mcpToolList.value;
  return mcpToolList.value.filter(
    (t) =>
      (t.name || "").toLowerCase().includes(kw) ||
      (t.description || "").toLowerCase().includes(kw)
  );
});

// 当前筛选结果是否已全选/部分选中
const allFilteredChecked = computed(
  () =>
    filteredMcpToolList.value.length > 0 &&
    filteredMcpToolList.value.every((t) => form.mcpSelectedTools.includes(t.name))
);
const someFilteredChecked = computed(() =>
  filteredMcpToolList.value.some((t) => form.mcpSelectedTools.includes(t.name))
);


// 解析后端存储的JSON字符串数组（勾选工具/全部工具名），非法或空返回[]
const parseJsonArray = (json) => {
  if (!json) return [];
  try {
    const arr = JSON.parse(json);
    return Array.isArray(arr) ? arr.filter((t) => t) : [];
  } catch (e) {
    return [];
  }
};

// 预览弹窗展示已勾选工具名
const parseSelectedTools = (json) => parseJsonArray(json);

const onCheckAllMcpTools = (e) => {
  // 全选/取消仅作用于当前筛选出的工具，不影响筛选外的已勾选项
  const names = filteredMcpToolList.value.map((t) => t.name);
  if (e.target.checked) {
    form.mcpSelectedTools = Array.from(new Set([...form.mcpSelectedTools, ...names]));
  } else {
    form.mcpSelectedTools = form.mcpSelectedTools.filter((n) => !names.includes(n));
  }
};

const handleTestMcp = async () => {
  if (!form.mcpType) {
    message.warning("请先选择Mcp类型");
    return;
  }
  if (form.mcpType !== "stdio" && !form.mcpUrl) {
    message.warning("请先填写Mcp地址");
    return;
  }
  if (form.mcpType === "stdio" && !form.mcpCommand) {
    message.warning("请先填写McpCommand");
    return;
  }
  mcpTesting.value = true;
  mcpToolKeyword.value = "";
  try {
    const response = await testMcpConnection({
      mcpUrl: form.mcpUrl,
      mcpType: form.mcpType,
      mcpHeaders: form.mcpHeaders,
      mcpCommand: form.mcpCommand,
      mcpArguments: form.mcpArguments,
      mcpEnvironment: form.mcpEnvironment,
    });
    if (response && response.code === 200 && Array.isArray(response.data)) {
      mcpToolList.value = response.data.map((t) => ({
        name: t.name,
        description: t.description || "",
      }));
      // 保留仍然存在的已勾选工具，测试新返回的工具默认不勾选
      const names = mcpToolList.value.map((t) => t.name);
      form.mcpSelectedTools = (form.mcpSelectedTools || []).filter((n) => names.includes(n));
      mcpTested.value = true;
      clearValidate("mcpSelectedTools");
      message.success(`连接成功，返回 ${mcpToolList.value.length} 个工具`);
    } else {
      mcpToolList.value = [];
      mcpTested.value = false;
      message.error(response?.err_msg || "测试连接失败");
    }
  } catch (error) {
    mcpToolList.value = [];
    mcpTested.value = false;
    message.error("测试连接失败: " + (error?.message || "未知错误"));
  } finally {
    mcpTesting.value = false;
  }
};


const validateSkillFile = (rule, value) => {
  if (form.skillToolType !== 2) {
    return Promise.resolve();
  }
  if (!value || !value.id) {
    return Promise.reject("请上传技能压缩包");
  }
  return Promise.resolve();
};

// 验证名称不能包含中文
const validateNoChinese = (rule, value) => {
  if (!value) return Promise.resolve();
  const chineseRegex = /[\u4e00-\u9fa5]/;
  if (chineseRegex.test(value)) {
    return Promise.reject("名称不能包含中文");
  }
  return Promise.resolve();
};

// 验证技能压缩包文件名与名称一致
const validateSkillFileName = (rule, value) => {
  if (form.skillToolType !== 2) return Promise.resolve();
  if (!value) return Promise.resolve();
  if (!form.name) return Promise.resolve();
  const expectedName = form.name + '.zip';
  if (value.name !== expectedName) {
    return Promise.reject(`技能压缩包文件名必须为 "${expectedName}"`);
  }
  return Promise.resolve();
};

// Mcp必须先测试连接成功并至少勾选一个工具，AI运行时只会加载勾选的工具
const validateMcpSelectedTools = (rule, value) => {
  if (form.skillToolType !== 3) return Promise.resolve();
  if (mcpToolList.value.length === 0) {
    return Promise.reject("请先点击“测试连接”并成功返回Mcp工具");
  }
  if (!value || value.length === 0) {
    return Promise.reject("请至少勾选一个Mcp工具");
  }
  return Promise.resolve();
};


const rules = computed(() => ({
  name: [
    { required: true, message: "请输入名称" },
    { validator: validateNoChinese, trigger: "change" },
  ],
  classMethod: form.skillToolType === 1 ? [{ required: true, message: "请输入方法" }] : [],
  skillToolType: [{ required: true, message: "请选择技能工具类型" }],
  activeStatus: [{ required: true, message: "请选择启用状态" }],
  skillFile: form.skillToolType === 2 ? [
    { required: true, validator: validateSkillFile, trigger: "change" },
    { validator: validateSkillFileName, trigger: "change" },
  ] : [],
  mcpUrl: (form.skillToolType === 3 && form.mcpType !== 'stdio') ? [{ required: true, message: "请输入Mcp地址" }] : [],
  mcpType: form.skillToolType === 3 ? [{ required: true, message: "请选择Mcp类型" }] : [],
  mcpCommand: (form.skillToolType === 3 && form.mcpType === 'stdio') ? [{ required: true, message: "请输入McpCommand" }] : [],
  mcpSelectedTools: form.skillToolType === 3 ? [{ required: true, validator: validateMcpSelectedTools, trigger: "change" }] : [],
}));

const { validate: validateForm, validateInfos, clearValidate } = useForm(form, rules);

watch(() => form.skillToolType, () => {
  clearValidate(["classMethod", "skillFile", "mcpUrl", "mcpType", "mcpHeaders", "mcpCommand", "mcpArguments", "mcpEnvironment", "mcpSelectedTools"]);
});

// Mcp连接配置变更后，此前的测试结果与勾选失效，需重新测试连接
watch(
  () => [form.mcpUrl, form.mcpType, form.mcpHeaders, form.mcpCommand, form.mcpArguments, form.mcpEnvironment],
  () => {
    if (mcpRestoring.value) return;
    mcpToolList.value = [];
    form.mcpSelectedTools = [];
    mcpTested.value = false;
    mcpToolKeyword.value = "";
  }
);


// Mcp类型切换时清空stdio相关验证
watch(() => form.mcpType, () => {
  clearValidate(["mcpCommand", "mcpArguments", "mcpEnvironment"]);
});

// 名称变化时重新校验技能文件
watch(() => form.name, () => {
  if (form.skillToolType === 2 && form.skillFile) {
    clearValidate("skillFile");
  }
});

const searchKeyword = ref("");
const filterType = ref(2);

const getSkillToolTypeName = (type) => {
  const types = { 1: "Tool", 2: "Skill", 3: "Mcp" };
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
  mcpRestoring.value = true;
  mcpToolList.value = [];
  mcpTested.value = false;
  mcpToolKeyword.value = "";
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
      mcpUrl: "",
      mcpType: "",
      mcpHeaders: "",
      mcpCommand: "",
      mcpArguments: "",
      mcpEnvironment: "",
      mcpSelectedTools: [],
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
      mcpUrl: "",
      mcpType: "",
      mcpHeaders: "",
      mcpCommand: "",
      mcpArguments: "",
      mcpEnvironment: "",
      mcpSelectedTools: [],
    });
  }
  modalVisible.value = true;
  nextTick(() => {
    mcpRestoring.value = false;
  });
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
  if (skillScanning.value) {
    message.warning("AI安全扫描进行中，请等待扫描完成");
    return;
  }
  modalTitle.value = "编辑技能工具";
  currentRecord.value = record;
  mcpRestoring.value = true;
  form.id = record.id || "";
  form.name = record.name || "";
  form.classMethod = record.classMethod || "";
  form.description = record.description || "";
  form.activeStatus = record.activeStatus !== undefined ? record.activeStatus : 1;
  form.skillToolType = record.skillToolType !== undefined ? record.skillToolType : undefined;
  form.skillFile = record.skillFile || null;
  form.mcpUrl = record.mcpUrl || "";
  form.mcpType = record.mcpType || "";
  form.mcpHeaders = record.mcpHeaders || "";
  form.mcpCommand = record.mcpCommand || "";
  form.mcpArguments = record.mcpArguments || "";
  form.mcpEnvironment = record.mcpEnvironment || "";
  // 回显已保存的Mcp工具列表与勾选状态（描述需重新测试连接才会带出）
  const savedToolNames = parseJsonArray(record.mcpTools);
  mcpToolList.value = savedToolNames.map((n) => ({ name: n, description: "" }));
  form.mcpSelectedTools = parseJsonArray(record.mcpSelectedTools).filter((n) => savedToolNames.includes(n));
  mcpTested.value = mcpToolList.value.length > 0;
  mcpToolKeyword.value = "";
  modalVisible.value = true;
  nextTick(() => {
    mcpRestoring.value = false;
  });
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

// Skill类型提交前提示：需AI安全扫描、耗时较久且扫描期间不可编辑
const confirmSkillScanSubmit = () => {
  return new Promise((resolve) => {
    Modal.confirm({
      title: "提交AI安全扫描提示",
      content:
        "Skill类型提交后需由AI对技能包进行安全性扫描，扫描耗时较久，提交期间该技能不可编辑，请勿关闭窗口或刷新页面。是否确认提交？",
      okText: "确认提交",
      cancelText: "取消",
      onOk: () => resolve(true),
      onCancel: () => resolve(false),
    });
  });
};

// Skill提交失败（含AI安全扫描不通过、后端400/500）时用弹窗完整展示返回文案，
// message气泡会被截断长度且不支持换行，看不清扫描报告全文
const showSkillErrorModal = (msg) => {
  Modal.error({
    title: "Skill提交失败",
    okText: "知道了",
    width: 640,
    zIndex: 1100,
    content: h(
      "div",
      {
        style: {
          whiteSpace: "pre-wrap",
          wordBreak: "break-all",
          maxHeight: "50vh",
          overflowY: "auto",
          lineHeight: "1.6",
        },
      },
      msg
    ),
  });
};

const submitForm = async () => {
  // 只有Skill类型会触发AI安全扫描，扫描期间锁定表单不允许编辑
  const needScan = form.skillToolType === 2;
  confirmLoading.value = true;
  if (needScan) {
    skillScanning.value = true;
    startScanTimer();
  }
  try {
    const mcpToolsPayload = form.skillToolType === 3 ? JSON.stringify(mcpToolList.value.map((t) => t.name)) : "";
    const mcpSelectedPayload = form.skillToolType === 3 ? JSON.stringify(form.mcpSelectedTools || []) : "";
    await addEditAISkillToolManagement(currentRecord.value ? {
      id: form.id,
      name: form.name,
      classMethod: form.classMethod,
      description: form.description,
      activeStatus: form.activeStatus,
      skillToolType: form.skillToolType,
      mcpUrl: form.mcpUrl,
      mcpType: form.mcpType,
      mcpHeaders: form.mcpHeaders,
      mcpCommand: form.mcpCommand,
      mcpArguments: form.mcpArguments,
      mcpEnvironment: form.mcpEnvironment,
      mcpTools: mcpToolsPayload,
      mcpSelectedTools: mcpSelectedPayload,
    } : {
      id: form.id,
      name: form.name,
      classMethod: form.classMethod,
      description: form.description,
      activeStatus: form.activeStatus,
      skillToolType: form.skillToolType,
      mcpUrl: form.mcpUrl,
      mcpType: form.mcpType,
      mcpHeaders: form.mcpHeaders,
      mcpCommand: form.mcpCommand,
      mcpArguments: form.mcpArguments,
      mcpEnvironment: form.mcpEnvironment,
      mcpTools: mcpToolsPayload,
      mcpSelectedTools: mcpSelectedPayload,
    });


    message.success(currentRecord.value ? "更新成功" : "添加成功");
    modalVisible.value = false;
    loadData();
  } catch (error) {
    console.error("保存失败:", error);
    // error.message 即后端返回的 err_msg 原文（已由http拦截器透传）
    const errMsg = error?.message || "未知错误";
    if (needScan) {
      showSkillErrorModal(errMsg);
    } else {
      message.error("保存失败: " + errMsg);
    }
  } finally {
    if (needScan) {
      stopScanTimer();
      skillScanning.value = false;
    }
    confirmLoading.value = false;
  }
};

const handleModalOk = () => {
  validateForm()
    .then(async () => {
      if (form.skillToolType === 2) {
        const confirmed = await confirmSkillScanSubmit();
        if (!confirmed) return;
      }
      await submitForm();
    })
    .catch((err) => {
      console.log("表单验证失败:", err);
    });
};

const handleModalCancel = () => {
  if (skillScanning.value) {
    message.warning("AI安全扫描进行中，请等待扫描完成后再关闭");
    return;
  }
  modalVisible.value = false;
};

onMounted(() => {
  loadData();
});

onUnmounted(() => {
  stopScanTimer();
});
</script>

<style scoped>
.mcp-tool-list {
  max-height: 260px;
  overflow-y: auto;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  padding: 8px 12px;
}
</style>
