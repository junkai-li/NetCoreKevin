<template>
  <a-modal
    v-model:open="modalVisible"
    :title="modalTitle"
    @ok="handleOk"
    @cancel="handleCancel"
    :width="600"
  :confirm-loading="confirmLoading"
  >
    <a-form :model="form" :label-col="{ span: 6 }" :wrapper-col="{ span: 18 }">
      <a-form-item label="用户名" v-bind="validateInfos.username">
        <a-input v-model:value="form.username" class="custom-input" />
      </a-form-item>
      <a-form-item label="昵称" v-bind="validateInfos.nickName">
        <a-input v-model:value="form.nickName" class="custom-input" />
      </a-form-item>
      <a-form-item label="手机号" v-bind="validateInfos.phone">
        <a-input v-model:value="form.phone" class="custom-input" />
      </a-form-item>
      <a-form-item label="邮箱" v-bind="validateInfos.email">
        <a-input v-model:value="form.email" class="custom-input" />
      </a-form-item>
      <a-form-item label="角色" v-bind="validateInfos.roles">
        <a-select
          v-model:value="form.roles"
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
      <a-form-item label="部门" v-bind="validateInfos.departmentId">
        <a-select
          v-model:value="form.dtoUserInfo.departmentId"
         ref="select"
          show-search
          placeholder="请选择部门"
          
          :loading="roleLoading"
        >
          <a-select-option v-for="dept in departmentList" :key="dept.id" :value="dept.id">
            {{ dept.name }}
          </a-select-option>
        </a-select>
      </a-form-item> 
        <a-form-item label="岗位" v-bind="validateInfos.positions">
        <a-select
          v-model:value="form.positions"
          mode="multiple"
          placeholder="请选择岗位"
          class="custom-select"
          :loading="roleLoading"
        >
          <a-select-option v-for="pos in positionList" :key="pos.id" :value="pos.id">
            {{ pos.name }}
          </a-select-option>
        </a-select>
      </a-form-item> 
      <a-form-item label="状态">
        <a-switch
          v-model:checked="form.status"
          checked-children="启用"
          un-checked-children="禁用"
        /> 
      </a-form-item>
      <a-form-item label="头像">
        <a-upload
          v-model:file-list="form.avatar"
          list-type="picture-card"
          class="avatar-uploader"
          :show-upload-list="false"
          :before-upload="beforeUpload"
        >
          <img
            v-if="form.avatarUrl"
            :src="form.avatarUrl"
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
          v-model:value="form.PassWord"
        />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, reactive, watch } from 'vue';
import { PlusOutlined } from "@ant-design/icons-vue";
import { message } from "ant-design-vue";
import { Form } from "ant-design-vue"; 
import { GetSnowflakeId } from "../api/baseapi"; 
import { createUser, updateUser, getUserRoleList } from "../api/userapi";
import { getPositionALLList } from "../api/organizational/position";
import { getDepartmentALLList } from "../api/organizational/department";
const emit = defineEmits(['ok', 'cancel']);

const useForm = Form.useForm;

// 表单数据
const form = reactive({
  username: "",
  id: "",
  nickName: "",
  PassWord: "",
  phone: "",
  email: "",
  roles: [],
  positions:[],
  status: true,
  avatar: [],
  avatarUrl: "",
 dtoUserInfo:{ departmentId: ""}
});

// 表单验证规则
const formRules = reactive({
  username: [
    { required: true, message: "请输入用户名" },
    { min: 3, message: "用户名至少3个字符" },
  ],
  nickName: [{ required: true, message: "请输入昵称" }],
  phone: [
   { required: true, message: "请输入手机号" },  
  {  pattern: /^1[3-9]\d{9}$/, message: "请输入正确的手机号" }],
  email: [ 
    { type: "email", message: "请输入有效的邮箱地址" },
  ], 
  roles: [ 
      { required: true, message: "请选择角色" },   
  ], positions: [ 
      { required: true, message: "请选择岗位" },   
  ],
  departmentId: [ 
      { required: true, message: "请选择岗位" },   
  ],
});

// 表单验证
const { validate: validateForm, validateInfos } = useForm(form, formRules);

// Props
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: "用户管理"
  },
  user: {
    type: Object,
    default: null
  },
  roleList: {
    type: Array,
    default: () => []
  },
  roleLoading: {
    type: Boolean,
    default: false
  }
});

// 内部状态
const modalVisible = ref(false);
const modalTitle = ref("用户管理");
const confirmLoading = ref(false);
const roleLoading = ref(false);
const roleList = ref([]);
const positionList = ref([]);
const departmentList = ref([]);
// 监听props变化
watch(() => props.visible, (newVal) => {
  modalVisible.value = newVal;
  if (newVal) {
    loadRoleList();
    loadPositionList();
    loadDepartmentList();
  }
});

watch(() => props.title, (newVal) => {
  modalTitle.value = newVal;
});

watch(() => props.user, (newVal) => {
   // 重置表单 
  if (newVal.id) {
    console.log('编辑'+newVal);
    // 填充表单数据
    Object.assign(form, {
      id: newVal.id,
      username: newVal.name,
      nickName: newVal.nickName,
      phone: newVal.phone,
      email: newVal.email,
      roles: newVal.roles.map((role) => role.id),
       positions: newVal.positions.map((role) => role.id),
      status: newVal.status == 1,
      avatar: [],
      avatarUrl: newVal.avatar || "",
      dtoUserInfo:newVal.dtoUserInfo
    });
  } else {
     console.log('新增'+newVal);
    Object.assign(form, {
      username: "",
      id: "",
      nickName: "",
      phone: "",
      email: "",
      roles: [],
      positions:newVal.positions?.map((role) => role.id),
      status: true,
      avatar: [],
      avatarUrl: "",
      dtoUserInfo:newVal.dtoUserInfo??{}
    });
  }
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
      form.avatarUrl = e.target.result;
    };
    reader.readAsDataURL(file);
  }
  return false; // 不自动上传
};

// 加载角色列表
const loadRoleList = async () => {
  roleLoading.value = true;
  try {
    const response = await getUserRoleList();
    if (response && response.status === 200 && response.data) {
      roleList.value = response.data.data.map((role) => ({
        id: role.key,
        name: role.value,
        value: role.key,
      }));
    }
  } catch (error) {
    console.error("获取角色列表失败:", error);
    message.error("获取角色列表失败: " + error.message);
  } finally {
    roleLoading.value = false;
  }
};
const loadPositionList = async () => {
  roleLoading.value = true;
  try {
    const response = await getPositionALLList();
    if (response && response.status === 200 && response.data&&response.data.code==200) {
      positionList.value = response.data.data.map((role) => ({
        id: role.id,
        name: role.name+'-'+role.code,
        value: role.id,
      }));
    }
  } catch (error) {
    console.error("获取岗位列表失败:", error);
    message.error("获取岗位列表失败: " + error.message);
  } finally {
    roleLoading.value = false;
  }
};
const loadDepartmentList = async () => {
  roleLoading.value = true;
  try {
    const response = await getDepartmentALLList();
    if (response && response.status === 200 && response.data&&response.data.code==200) {
      departmentList.value = response.data.data.map((role) => ({
        id: role.id,
        name: role.name+'-'+role.code,
        value: role.id,
      }));
    }
  } catch (error) {
    console.error("获取部门列表失败:", error);
    message.error("获取部门列表失败: " + error.message);
  } finally {
    roleLoading.value = false;
  }
}; 

// 处理确认
const handleOk = () => {
  validateForm()
    .then(async () => {
      if(!form.dtoUserInfo.departmentId){
           message.error("请选择部门！");  
            return;
      }
      try {
        confirmLoading.value = true;
        var roles = roleList.value.filter((role) => form.roles.includes(role.id));
          var positions = positionList.value.filter((role) => form.positions.includes(role.id));
        const userData = {
          id: form.id,
          name: form.username,
          nickName: form.nickName,
          PassWord: form.PassWord,
          phone: form.phone,
          email: form.email,
          dtoUserInfo:form.dtoUserInfo,
          roles: roles.map((role) => ({ id: role.id, name: role.name })),
          positions: positions.map((role) => ({ id: role.id, name: role.name })),
          status: form.status,
          headImgs: form.avatarUrl
            ? [{ key: "avatar", value: form.avatarUrl }]
            : [],
        };

        let result;
        // 如果是新增用户，获取新ID
        if (!props.user?.id) {
          var dataid = await GetSnowflakeId();
          if (dataid && dataid.status == 200 && dataid.data.data) {
            var id = dataid.data.data;
            userData.id = id;
            result = await createUser(userData);
          } else {
            message.error("获取用户ID失败"); 
            confirmLoading.value = false;
            return;
          }
        } else {
          // 编辑用户
          result = await updateUser(userData);
        }

        if (result && result.status == 200) {
          message.success(props.user ? "用户信息更新成功" : "用户添加成功");
          emit('ok', userData);
        } else {
          message.error(props.user ? "用户信息更新失败" : "用户添加失败");
        }
      } catch (error) {
        console.error("操作失败:", error);
        message.error(props.user ? "用户信息更新失败: " + error.message : "用户添加失败: " + error.message);
      } finally {
        confirmLoading.value = false;
      }
    })
    .catch((err) => {
      console.log("表单验证失败:", err);
    });
};

// 处理取消
const handleCancel = () => {
    Object.assign(form, {
      username: "",
      id: "",
      nickName: "",
      phone: "",
      email: "",
      roles: [],
      positions:[],
      status: true,
      avatar: [],
      avatarUrl: "",
      dtoUserInfo:{}
    });
  emit('cancel');
};
</script>