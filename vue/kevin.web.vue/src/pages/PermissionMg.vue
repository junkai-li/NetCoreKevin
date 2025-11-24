<template>
  <div class="management-container">
    <a-card class="management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <SafetyCertificateOutlined class="title-icon" />
            <span>权限管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索权限..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <Button type="primary" class="add-button" @click="showAddPermModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加权限
            </Button>
          </div>
        </div>
      </template>

      <div class="toolbar">
        <div class="toolbar-left">
          <a-button :loading="loadingReload" class="toolbar-button" @click="handleReload">
            <template #icon>
              <DownloadOutlined />
            </template>
            初始化权限
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
          :pagination="pagination"
          :scroll="{ x: 1200 }"
          :loading="loading"
          @change="handleTableChange"
          class="my-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'isManual'">
              <a-tag :color="record.isManual == true ? 'success' : 'error'">
                {{ record.isManual == true ? "是" : "否" }}
              </a-tag>
            </template>
            <template v-if="column.dataIndex === 'permissionType'">
              <a-tag :color="'success'">
                {{ permissionTypes[record.permissionType] }}
              </a-tag>
            </template>
            <template v-if="column.dataIndex === 'webaction'">
              <div class="action-buttons">
                <a-button type="link" size="small" @click="showEditPermModal(record)">
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button>
                <a-popconfirm
                  title="确定要删除这个权限吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  @confirm="handleDelete(record.id)"
                >
                  <a-button type="link" size="small" danger>
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

    <!-- 添加/编辑权限模态框 -->
    <a-modal
      v-model:open="permModalVisible"
      :title="permModalTitle"
      @ok="handlePermModalOk"
      @cancel="handlePermModalCancel"
      width="600px"
    >
      <a-form :model="permForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="权限名称" v-bind="validateInfos.name">
          <a-input v-model:value="permForm.name" />
        </a-form-item>
        <a-form-item label="区域" v-bind="validateInfos.areaName" >
          <a-input v-model:value="permForm.areaName" />
        </a-form-item>
         <a-form-item label="区域(Code)" v-bind="validateInfos.area" >
          <a-input v-model:value="permForm.area" />
        </a-form-item>
        <a-form-item label="模块" v-bind="validateInfos.module">
          <a-input v-model:value="permForm.moduleName"  />
        </a-form-item>
         <a-form-item label="模块(Code)" v-bind="validateInfos.module">
          <a-input v-model:value="permForm.module"  />
        </a-form-item>
        <a-form-item label="功能"  v-bind="validateInfos.actionName" >
          <a-input v-model:value="permForm.actionName" />
        </a-form-item>
          <a-form-item label="功能(Code)"  v-bind="validateInfos.action" >
          <a-input v-model:value="permForm.action" />
        </a-form-item>
        <a-form-item label="请求类型">
          <a-select v-model:value="permForm.httpMethod" placeholder="请选择请求类型">
            <a-select-option value="GET">GET</a-select-option>
            <a-select-option value="POST">POST</a-select-option>
            <a-select-option value="PUT">PUT</a-select-option>
            <a-select-option value="DELETE">DELETE</a-select-option>
            <a-select-option value="PATCH">PATCH</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="权限类型">
          <a-select v-model:value="permForm.permissionType" placeholder="请选择权限类型">
            <a-select-option :value="1">菜单权限</a-select-option>
            <a-select-option :value="2">功能权限</a-select-option>
            <a-select-option :value="3">数据权限</a-select-option>
            <a-select-option :value="4">接口权限</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="是否手动添加">
          <a-tag :color="permForm.isManual == true ? 'success' : 'error'">
            {{ permForm.isManual == true ? "是" : "否" }}
          </a-tag>
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import "../css/MyTable.css";
import "../css/buttons.css";
import "../css/management.css";
import { ref, reactive, onMounted, computed } from "vue";
import {
  SafetyCertificateOutlined,
  PlusOutlined,
  DeleteOutlined,
  EditOutlined,
  DownloadOutlined,
  BarsOutlined,
} from "@ant-design/icons-vue";
import { message, Button } from "ant-design-vue";
import { Form } from "ant-design-vue";
import { GetPageData, addEidt, Delete, reload, getDetails } from "@/api/permission";

const useForm = Form.useForm;
// 隐藏的列
const hiddenColumns = ref([]);
//权限类型1.菜单权限2.功能权限3.数据权限 4.接口权限
const permissionTypes = ["", "菜单权限", "功能权限", "数据权限", "接口权限"];
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
// 权限数据
const dataSource = ref([]);
// 表格列定义
const columns = ref([
  // {
  //   title: "id",
  //   dataIndex: "id",
  //   key: "id"
  // },
  {
    title: "权限名称",
    dataIndex: "fullName",
    key: "fullName", 
  },
  {
    title: "区域",
    dataIndex: "areaName",
    key: "areaName",
  },  {
    title: "区域",
    dataIndex: "area",
    key: "area",
  },
  {
    title: "模块",
    dataIndex: "moduleName",
    key: "moduleName",
  }, {
    title: "模块",
    dataIndex: "module",
    key: "module",
  },
  {
    title: "功能",
    dataIndex: "actionName",
    key: "actionName",
  }, {
    title: "功能",
    dataIndex: "action",
    key: "action",
  },
  {
    title: "请求类型",
    dataIndex: "httpMethod",
    key: "httpMethod",
  },
  {
    title: "是否手动添加",
    dataIndex: "isManual",
    key: "isManual",
  },
  {
    title: "权限类型",
    dataIndex: "permissionType",
    key: "permissionType",
  },
  {
    title: "操作",
    dataIndex: "webaction",
    key: "webaction",
    fixed: "right",
    width: 200,
  },
]);
// 数据加载状态
const loading = ref(false);
// 数据加载状态
const loadingReload = ref(false);
// 分页配置
const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`,
});
const handleReload = async () => {
  loadingReload.value = true;
  try {
    const response = await reload();
    if (response) {
      message.success("初始化成功");
    }
  } catch (error) {
    message.error("初始化失败: " + error.message);
  } finally {
    loadingReload.value = false;
    pagination.value.current = 1;
    loadPermData();
  }
};
// 模态框状态
const permModalVisible = ref(false);
const permModalTitle = ref("添加权限");

// 当前编辑的权限
const currentPerm = ref(null);

// 权限表单
const permForm = reactive({
  id: "",
  name: "",
  areaName: "",
  moduleName: "",
  actionName: "",
   area: "",
  module: "",
  action: "",
  httpMethod: undefined,
  permissionType: undefined,
  isManual: true,
  remarks: "",
});

// 表单验证规则
const permRules = reactive({
  name: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  areaName: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  moduleName: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  actionName: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  area: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  module: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  action: [
    { required: true, message: "请输入权限名称" },
    { min: 2, message: "权限名称至少2个字符" },
  ],
  permissionType: [{ required: true, message: "请选择权限类型" }],
});

// 表单验证
const { validate: validatePermForm, validateInfos } = useForm(permForm, permRules);

// 搜索关键字
const searchKeyword = ref("");

// 搜索处理
const onSearch = (value) => {
  searchKeyword.value = value;
  pagination.value.current = 1;
  loadPermData();
};

// 加载权限数据
const loadPermData = async () => {
  try {
    const params = {
      pageNum: pagination.value.current,
      pageSize: pagination.value.pageSize,
      searchKey: searchKeyword.value,
    };
    loading.value = true;
    const response = await GetPageData(params);
    loading.value = false;
    if (response && response.data.data) {
      dataSource.value = response.data.data.data.map((item) => ({
        ...item,
        key: item.id,
      }));
      pagination.value.total = response.data.data.total;
    }
  } catch (error) {
    loading.value = false;
    console.error("加载权限数据失败:", error);
    message.error("加载权限数据失败: " + (error.message || "未知错误"));
  } finally {
    loading.value = false;
  }
};

// 显示添加权限模态框
const showAddPermModal = () => {
  permModalTitle.value = "添加权限";
  currentPerm.value = null;
  // 重置表单
  Object.assign(permForm, {
    id: "",
    name: "",
    areaName: "",
    moduleName: "",
    actionName: "",
     area: "",
    module: "",
    action: "",
    httpMethod: undefined,
    permissionType: undefined,
    isManual: true,
    remarks: "",
  });
  permModalVisible.value = true;
};

// 显示编辑权限模态框
const showEditPermModal = async (record) => {
  try {
    permModalTitle.value = "编辑权限";
    // 获取详细信息
    const response = await getDetails(record.id);
    console.log(response);
    if (response && response.data && response.data?.code == 200) {
      currentPerm.value = response.data.data;
      // 填充表单数据
      Object.assign(permForm, {
        id: response.data.data.id,
        name: response.data.data.fullName,
        areaName: response.data.data.areaName,
        moduleName: response.data.data.moduleName,
        actionName: response.data.data.actionName,
         area: response.data.data.area,
        module: response.data.data.module,
        action: response.data.data.action,
        httpMethod: response.data.data.httpMethod,
        permissionType: response.data.data.permissionType,
        isManual: response.data.data.isManual,
        remarks: response.data.data.remarks,
      });
      permModalVisible.value = true;
    } else {
      message.error("获取权限详情失败: " + (response?.data?.errMsg || "未知错误"));
    }
  } catch (error) {
    console.error("获取权限详情失败:", error);
    message.error("获取权限详情失败: " + (error.message || "未知错误"));
  }
};
// 删除权限
const handleDelete = async (id) => {
  try {
    const response = await Delete(id);
    if (response && response.data && response.data.code === 200) {
      message.success("权限删除成功");
      loadPermData(); // 重新加载数据
    } else {
      message.error("权限删除失败: " + (response?.data?.errMsg || "未知错误"));
    }
  } catch (error) {
    console.error("删除权限失败:", error);
    message.error("删除权限失败: " + (error.message || "未知错误"));
  }
};
// 表格变化处理
const handleTableChange = (pager, filters, sorter) => {
  console.log("表格变化:", pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  loadPermData();
};

// 权限模态框确认
const handlePermModalOk = () => {
  validatePermForm()
    .then(async () => {
      try {
        const params = {
          id: permForm.id,
          fullName: permForm.name,
          areaName: permForm.areaName,
          moduleName: permForm.moduleName,
          actionName: permForm.actionName,
           area: permForm.area,
          module: permForm.module,
          action: permForm.action,
          httpMethod: permForm.httpMethod,
          permissionType: permForm.permissionType,
          isManual: permForm.isManual,
          remarks: permForm.remarks,
        };

        let response;
        if (currentPerm.value) {
          // 编辑权限
          response = await addEidt(params);
          console.log(response);
          if (response && response.data && response.data.code === 200) {
            message.success("权限信息更新成功");
          } else {
            message.error("权限信息更新失败: " + (response?.data?.data?.errMsg || "未知错误"));
            return;
          }
        } else {
          // 新增权限
          response = await addEidt(params);
          if (response && response.data && response.data?.code === 200) {
            message.success("权限信息添加成功");
          } else {
            message.error("权限信息添加失败: " + (response?.data?.data?.errMsg || "未知错误"));
            return;
          }
        }

        permModalVisible.value = false;
        loadPermData(); // 重新加载数据
      } catch (error) {
        console.error("保存权限失败:", error);
        message.error("保存权限失败: " + (error.message || "未知错误"));
      }
    })
    .catch((err) => {
      console.log("表单验证失败:", err);
    });
};

// 权限模态框取消
const handlePermModalCancel = () => {
  permModalVisible.value = false;
};

// 组件挂载时的初始化
onMounted(() => {
  console.log("权限管理页面已加载");
  loadPermData();
});
</script>
