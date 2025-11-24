<template>
  <div class="user-management-container">
    <a-card class="user-management-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <UserOutlined class="title-icon" />
            <span>用户管理</span>
          </div>
          <div class="header-actions">
            <a-input-search
              class="search-input"
              placeholder="搜索用户..."
              style="width: 250px; margin-right: 16px"
              @search="onSearch"
            />
            <a-button type="primary" class="add-button" @click="showAddUserModal">
              <template #icon>
                <PlusOutlined />
              </template>
              添加用户
            </a-button>
          </div>
        </div>
      </template>

      <div class="toolbar">
        <div class="toolbar-left">
          <a-button class="toolbar-button" @click="handleExport">
            <template #icon>
              <DownloadOutlined />
            </template>
            导出
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
            <template v-if="column.dataIndex === 'avatar'">
              <a-avatar :src="record.avatar" :size="32" />
            </template>

            <template v-else-if="column.dataIndex === 'status'">
              <a-tag :color="record.status==true? 'success' : 'error'">
                {{ record.status==true? "启用" : "禁用" }}
              </a-tag>
            </template>

            <template v-else-if="column.dataIndex === 'role'">
              <a-tag v-for="role in record.roles" :key="role.id" color="processing">
                {{ role.name }}
              </a-tag>
            </template>

            <template v-else-if="column.dataIndex === 'lastLogin'">
              {{ record.lastLogin ? record.lastLogin : "从未登录" }}
            </template>

            <template v-else-if="column.dataIndex === 'createTime'">
              {{ formatDate(record.createTime) }}
            </template>
                <template v-else-if="column.dataIndex === 'recentLoginTime'">
              {{ formatDate(record.recentLoginTime) }}
            </template>

            <template v-else-if="column.dataIndex === 'action'">
              <div class="action-buttons">
                <a-button
                  type="link"
                  size="small"
                  class="action-button"
                  @click="showEditUserModal(record)"
                >
                  <template #icon>
                    <EditOutlined />
                  </template>
                  编辑
                </a-button> 
                <a-popconfirm
                  title="确定要删除这个用户吗?"
                  ok-text="确定"
                  cancel-text="取消"
                  @confirm="handleDelete(record.id)"
                >
                  <a-button type="link" size="small" class="action-button danger">
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

    <!-- 添加/编辑用户模态框 -->
    <a-modal
      v-model:open="userModalVisible"
      :title="userModalTitle"
      @ok="handleUserModalOk"
      @cancel="handleUserModalCancel" 
      :width="600"
    >
      <a-form :model="userForm" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
        <a-form-item label="用户名" v-bind="validateInfos.username">
          <a-input v-model:value="userForm.username" class="custom-input" />
        </a-form-item>
        <a-form-item label="昵称" v-bind="validateInfos.nickName">
          <a-input v-model:value="userForm.nickName" class="custom-input" />
        </a-form-item>
        <a-form-item label="手机号" v-bind="validateInfos.phone">
          <a-input v-model:value="userForm.phone" class="custom-input" />
        </a-form-item>
        <a-form-item label="邮箱" v-bind="validateInfos.email">
          <a-input v-model:value="userForm.email" class="custom-input" />
        </a-form-item>
        <a-form-item label="角色">
          <a-select
            v-model:value="userForm.roles"
            mode="multiple"
            placeholder="请选择角色"
            class="custom-select"
            :loading="roleLoading"
          >
            <a-select-option v-for="role in roleList" :key="role.id" :value="role.id">
              {{ role.name }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="状态">
          <a-switch
            v-model:checked="userForm.status"
            checked-children="启用"
            un-checked-children="禁用"
          />
        </a-form-item>
        <a-form-item label="头像">
          <a-upload
            v-model:file-list="userForm.avatar"
            list-type="picture-card"
            class="avatar-uploader"
            :show-upload-list="false"
            :before-upload="beforeUpload"
          >
            <img
              v-if="userForm.avatarUrl"
              :src="userForm.avatarUrl"
              alt="avatar"
              style="width: 100%"
            />
            <div v-else>
              <PlusOutlined />
              <div style="margin-top: 8px">上传头像</div>
            </div>
          </a-upload>
        </a-form-item>
        <a-form-item label="密码" v-bind="validateInfos.PassWord">
          <a-input-password
            size="middle"
            placeholder="密码"
            v-model:value="userForm.PassWord"
          />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>

import { ref, reactive, computed, onMounted, onUnmounted } from "vue";
import {
  UserOutlined,
  PlusOutlined,
  DeleteOutlined,
  DownloadOutlined,
  BarsOutlined,
  EditOutlined,
} from "@ant-design/icons-vue";
import { message } from "ant-design-vue";
import { Form } from "ant-design-vue";
import { getUserList, createUser, updateUser, getUserRoleList,DeleteUser,ExportGetSysUserList } from "../api/userapi";
import { GetGuId } from "../api/baseapi"; 
import hedeImage from "../assets/hede.png"; // 导入图片
const useForm = Form.useForm;

// 数据加载状态
const loading = ref(false);
const roleLoading = ref(false);

// 用户数据
const dataSource = ref([]);

// 角色列表
const roleList = ref([]);

// 表格列定义
const columns = ref([
  {
    title: "用户",
    dataIndex: "avatar",
    key: "avatar",
    width: 80,
  },
  {
    title: "用户名",
    dataIndex: "name",
    key: "name",
    sorter: true,
    width: 120,
  },
  {
    title: "昵称",
    dataIndex: "nickName",
    key: "nickName",
    width: 120,
  },
  {
    title: "邮箱",
    dataIndex: "email",
    key: "email",
    width: 200,
  },
  {
    title: "角色",
    dataIndex: "role",
    key: "role",
    width: 200,
  },
  {
    title: "状态",
    dataIndex: "status",
    key: "status",
    filters: [
      { text: "启用", value: true },
      { text: "禁用", value: false },
    ],
    width: 100,
  },
  {
    title: "创建时间",
    dataIndex: "createTime",
    key: "createTime",
    sorter: true,
    width: 180,
  },
    {
    title: "最近登录时间",
    dataIndex: "recentLoginTime",
    key: "recentLoginTime",
    sorter: true,
    width: 180,
  },
  {
    title: "操作",
    dataIndex: "action",
    key: "action",
    fixed: "right",
    width: 200,
  },
]);

// 隐藏的列
const hiddenColumns = ref([]);

// 计算可见列
const visibleColumns = computed(() => {
  return columns.value.filter(
    (column) => !hiddenColumns.value.includes(column.dataIndex)
  );
});

// 分页配置
const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  showQuickJumper: true,
  showTotal: (total) => `共 ${total} 条记录`,
});
 

// 搜索关键词
const searchKey = ref("");

// 模态框状态
const userModalVisible = ref(false);
const userModalTitle = ref("添加用户");

// 当前编辑的用户
const currentUser = ref(null);

// 用户表单
const userForm = reactive({
  username: "",
  id: "",
  nickName: "",
  PassWord: "",
  phone: "",
  email: "",
  roles: [],
  status: true,
  avatar: [],
  avatarUrl: "",
});

// 表单验证规则
const userRules = reactive({
  username: [
    { required: true, message: "请输入用户名" },
    { min: 3, message: "用户名至少3个字符" },
  ],
  nickName: [{ required: true, message: "请输入昵称" }],
  phone: [{ pattern: /^1[3-9]\d{9}$/, message: "请输入正确的手机号" }],
  email: [
    { required: true, message: "请输入邮箱" },
    { type: "email", message: "请输入有效的邮箱地址" },
  ],
});

// 表单验证
const { validate: validateUserForm, validateInfos } = useForm(userForm, userRules);

onMounted(() => {
  fetchUserList();
  fetchRoleList();
});

onUnmounted(() => {
  // 不需要在这里做任何事情，因为错误处理现在在全局进行
});

// 头像上传前处理
const beforeUpload = (file) => {
  const isJpgOrPng = file.type === "image/jpeg" || file.type === "image/png";
  if (!isJpgOrPng) {
    message.error("只能上传JPG/PNG格式的图片!");
  }
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isLt2M) {
    message.error("图片大小不能超过2MB!");
  }
  if (isJpgOrPng && isLt2M) {
    // 读取文件并预览
    const reader = new FileReader();
    reader.onload = (e) => {
      userForm.avatarUrl = e.target.result;
    };
    reader.readAsDataURL(file);
  }
  return false; // 不自动上传
};

// 切换列显示/隐藏
const toggleColumn = (dataIndex, visible) => {
  if (visible) {
    hiddenColumns.value = hiddenColumns.value.filter((col) => col !== dataIndex);
  } else {
    hiddenColumns.value.push(dataIndex);
  }
};

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleString("zh-CN");
};

// 搜索处理
const onSearch = (value) => {
  searchKey.value = value;
  pagination.value.current = 1;
  fetchUserList();
};

// 获取用户列表
const fetchUserList = async () => {
  loading.value = true;
  try {
    const params = {
      searchKey: searchKey.value,
      state: "",
      sign: "",
      pageSize: pagination.value.pageSize,
      pageNum: pagination.value.current,
      total: 0,
      // startTime: new Date(new Date().setDate(new Date().getDate() - 30)).toISOString(), // 最近30天
      // endTime: new Date().toISOString()
    };
    const response = await getUserList(params);
    if (response && response.status == 200 && response.data.data.data) {
      // 处理返回的数据
      dataSource.value = response.data.data.data.map((user) => ({
        key: user.id,
        id: user.id,
        name: user.name,
        nickName: user.nickName,
        phone: user.phone,
        email: user.email,
        roles: user.roles,
        status: user.status, 
        createTime: user.createTime,
        recentLoginTime: user.recentLoginTime,
        avatar: hedeImage,
      }));

      pagination.value.total = response.data.total;
      // pagination.value.current = response.data.pageNum;
      // pagination.value.pageSize = response.data.pageSize;
    }
  } catch (error) {
    console.error("获取用户列表失败:", error);
    message.error("获取用户列表失败: " + error.message);
  } finally {
    loading.value = false;
  }
};

// 获取角色列表
const fetchRoleList = async () => {
  roleLoading.value = true;
  try {
    const response = await getUserRoleList();
    if (response && response.status === 200 && response.data) {
      roleList.value = response.data.data.map((role) => ({
        id: role.key,
        name: role.value,
        value: role.key,
      }));
      console.log(roleList.value);
    }
  } catch (error) {
    console.error("获取角色列表失败:", error);
    message.error("获取角色列表失败: " + error.message);
  } finally {
    roleLoading.value = false;
  }
};

// 显示添加用户模态框
const showAddUserModal = () => {
  userModalTitle.value = "添加用户";
  currentUser.value = null;
  // 重置表单
  Object.assign(userForm, {
    username: "",
    id: "",
    nickName: "",
    phone: "",
    email: "",
    roles: [],
    status: true,
    avatar: [],
    avatarUrl: "",
  });
  userModalVisible.value = true;
};

// 显示编辑用户模态框
const showEditUserModal = (record) => {
  userModalTitle.value = "编辑用户";
  currentUser.value = record;
  // 填充表单数据
  Object.assign(userForm, {
    id: record.id,
    username: record.name,
    nickName: record.nickName,
    phone: record.phone,
    email: record.email,
    roles: record.roles.map((role) => role.id),
    status: record.status === 1,
    avatar: [],
    avatarUrl: record.avatar || "",
  });
  userModalVisible.value = true;
};

// 用户模态框确认
const handleUserModalOk = () => {
  validateUserForm()
    .then(async () => {
      try {
        var roles = roleList.value.filter((role) => userForm.roles.includes(role.id));
        console.log(roles);
        const userData = {
          id: userForm.id,
          name: userForm.username,
          nickName: userForm.nickName,
          PassWord: userForm.PassWord,
          phone: userForm.phone,
          email: userForm.email,
          roles: roles.map((role) => ({ id: role.id, name: role.name })),
          status: userForm.status,
          headImgs: userForm.avatarUrl
            ? [{ key: "avatar", value: userForm.avatarUrl }]
            : [],
        };

        if (currentUser.value) {
          // 编辑用户
          await updateUser(userData);
          message.success("用户信息更新成功");
        } else {
          // 添加用户
          var dataid = await GetGuId();
          if (dataid && dataid.status == 200 && dataid.data.data) {
            var id = dataid.data.data;
            userData.id= id;
            await createUser(userData);
            message.success("用户添加成功");
          } else {
            message.error("获取用户ID失败"); 
          }
        }
        userModalVisible.value = false;
        fetchUserList(); // 重新加载数据
      } catch (error) {
        console.error("操作失败:", error);
      }
    })
    .catch((err) => {
      console.log("表单验证失败:", err);
    });
};

// 用户模态框取消
const handleUserModalCancel = () => {
  userModalVisible.value = false;
};

// 删除用户
const handleDelete = (key) => { 
    DeleteUser(key);
   message.success("用户删除成功");
  console.log("删除用户:", key);
  fetchUserList(); // 重新加载数据
};

// 导出数据
const handleExport = async ()  => {
 loading.value = true;
  try {
    const params = {
      searchKey: searchKey.value,
      state: "",
      sign: "",
      pageSize: pagination.value.pageSize,
      pageNum: pagination.value.current,
      total: 0
    };
    const response = await ExportGetSysUserList(params);
    if (response) {
      message.success("导出用户成功");
    }
  } catch (error) { 
    message.error("导出用户: " + error.message);
  } finally {
    loading.value = false;
  }
};

// 表格变化处理（分页、排序、筛选）
const handleTableChange = (pager, filters, sorter) => {
  console.log("表格变化:", pager, filters, sorter);
  pagination.value.current = pager.current;
  pagination.value.pageSize = pager.pageSize;
  fetchUserList(); // 重新加载数据
};
</script>

<style scoped>
@import "../css/MyTable.css";
@import "../css/UserList.css";  
@import "../css/management.css";  
</style>

