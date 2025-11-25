<template>
  <div class="permission-manager">
    <a-spin :spinning="loading">
      <a-alert 
        message="权限配置" 
        description="请选择需要分配给该角色的权限，然后点击保存。" 
        type="info" 
        show-icon 
        style="margin-bottom: 16px;"
      />
      
      <div class="permission-tree-container">
        <a-tree
          v-model:checkedKeys="checkedKeys"
          :tree-data="treeData"
          checkable
          :expandedKeys="expandedKeys"
          :autoExpandParent="autoExpandParent"
          @expand="onExpand"
          @check="onCheck"
        >
          <template #title="{ title, permissionType }">
            <span>
              <span v-if="permissionType === 1">📁 </span>
              <span v-else-if="permissionType === 2">⚙️ </span>
              <span v-else-if="permissionType === 3">📊 </span>
              <span v-else-if="permissionType === 4">🔗 </span>
              {{ title }}
            </span>
          </template>
        </a-tree>
      </div>
      
      <div class="permission-actions" style="margin-top: 16px; text-align: right;">
        <a-button @click="cancel">取消</a-button>
        <a-button type="primary" @click="save" style="margin-left: 8px;">保存</a-button>
      </div>
    </a-spin>
  </div>
</template>

<script setup>
/* eslint-disable no-undef */
import { ref, onMounted, watch } from 'vue'
import { message } from 'ant-design-vue'
import { getAllAreaPermissions, editAllAreaPermissions } from '@/api/permission'

// 定义组件属性
const props = defineProps({
  roleId: {
    type: String,
    required: true
  }
})

// 定义组件事件
const emit = defineEmits(['save-success', 'cancel'])

// 加载状态
const loading = ref(false)

// 树形控件相关数据
const treeData = ref([])
const checkedKeys = ref([])
const expandedKeys = ref([])
const autoExpandParent = ref(true)

// 权限类型映射
const permissionTypeMap = {
  '菜单权限': 1,
  '功能权限': 2,
  '数据权限': 3,
  '接口权限': 4
}

// 将API返回的数据转换为树形结构
const convertToTreeData = (data) => {
  const result = []
  
  // 用于存储已选中的权限ID
  const checkedIds = []
  
  // 遍历权限类型
  data.dtos.forEach(permissionTypeGroup => {
    const permissionType = permissionTypeMap[permissionTypeGroup.permissionType] || 0
    
    // 为每个权限类型创建根节点
    const typeNode = {
      title: permissionTypeGroup.permissionType,
      key: `type-${permissionTypeGroup.permissionType}`,
      permissionType: permissionType,
      children: []
    }
    
    // 遍历区域
    permissionTypeGroup.dtos.forEach(area => {
      const areaNode = {
        title: area.areaName,
        key: `area-${area.area}`,
        permissionType: permissionType,
        children: []
      }
      
      // 遍历模块
      area.modules.forEach(module => {
        const moduleNode = {
          title: module.moduleName,
          key: `module-${area.area}-${module.module}`,
          permissionType: permissionType,
          children: []
        }
        
        // 遍历操作
        module.actions.forEach(action => {
          const actionNode = {
            title: action.actionName,
            key: action.id,
            permissionType: permissionType
          }
          
          // 如果该权限已被选中，则添加到checkedIds数组中
          if (action.isPermission) {
            checkedIds.push(action.id)
          }
          
          moduleNode.children.push(actionNode)
        })
        
        areaNode.children.push(moduleNode)
      })
      
      typeNode.children.push(areaNode)
    })
    
    result.push(typeNode)
  })
  
  return { tree: result, checkedIds: checkedIds }
}

// 展开节点
const onExpand = (expandedKeysValue) => {
  expandedKeys.value = expandedKeysValue
  autoExpandParent.value = false
}

// 选中节点
const onCheck = (checkedKeysValue) => {
  checkedKeys.value = checkedKeysValue
}

// 取消操作
const cancel = () => {
  emit('cancel')
}

// 保存权限配置
const save = async () => {
  try {
    // 过滤掉非叶子节点（只保留具体的操作权限ID）
    const permissionIds = checkedKeys.value.filter(key => 
      !key.startsWith('type-') && 
      !key.startsWith('area-') && 
      !key.startsWith('module-')
    )
    
    const params = {
      roleId: props.roleId,
      permissionIds: permissionIds
    }
    
    const response = await editAllAreaPermissions(params)
    if (response.data.code === 200) {
      message.success('权限配置保存成功')
      emit('save-success')
    } else {
      message.error('权限配置保存失败: ' + response.data.msg)
    }
  } catch (error) {
    console.error('保存权限配置失败:', error)
    message.error('保存权限配置失败: ' + (error.message || '未知错误'))
  }
}

// 加载权限数据
const loadPermissions = async () => {
  try {
    loading.value = true
    const response = await getAllAreaPermissions(props.roleId)
    if (response.data.code === 200) {
      // 转换数据结构为树形结构
      const convertedData = convertToTreeData(response.data.data)
      treeData.value = convertedData.tree
      
      // 获取已选中的权限ID
      checkedKeys.value = convertedData.checkedIds || []
      
      // 默认展开所有根节点
      expandedKeys.value = treeData.value.map(item => item.key)
    } else {
      message.error('获取权限数据失败: ' + response.data.msg)
    }
  } catch (error) {
    console.error('获取权限数据失败:', error)
    message.error('获取权限数据失败: ' + (error.message || '未知错误'))
  } finally {
    loading.value = false
  }
}

// 监听角色ID变化
watch(() => props.roleId, (newRoleId) => {
  if (newRoleId) {
    loadPermissions()
  }
}, { immediate: true })

// 组件挂载时加载数据
onMounted(() => {
  if (props.roleId) {
    loadPermissions()
  }
})
</script>

<style scoped>
.permission-manager {
  min-height: 300px;
}

.permission-tree-container {
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  padding: 12px;
  max-height: 400px;
  overflow-y: auto;
}

.permission-actions {
  padding-top: 16px;
  border-top: 1px solid #f0f0f0;
  margin-top: 16px;
}
</style>