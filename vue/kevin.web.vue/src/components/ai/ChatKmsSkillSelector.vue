<template>
  <!-- 内联命令面板（CodeBuddy 风格）：非模态，锚定在输入框上方；点击条目即时生效，无需"确定" -->
  <div v-if="open" class="selector-popover" @mousedown.stop>
    <div class="selector-head">
      <a-input
        ref="searchRef"
        v-model:value="search"
        placeholder="搜索知识库 / 技能…（↑↓ 选择，回车确认）"
        allow-clear
        size="small"
        class="selector-search"
        @keydown="onSearchKeydown"
      >
        <template #prefix><SearchOutlined /></template>
      </a-input>
    </div>
    <div class="selector-body">
      <template v-if="filteredKmsList.length">
        <div class="selector-group-title">知识库</div>
        <div
          v-for="(item, i) in filteredKmsList"
          :key="'kms-' + item.id"
          class="selector-item"
          :class="{ selected: checkedKmsIds.includes(String(item.id)), highlighted: highlightIndex === i }"
          @click="toggleItem('kms', item)"
          @mouseenter="highlightIndex = i"
        >
          <BookOutlined class="selector-item-icon" />
          <span class="selector-item-name">{{ item.name }}</span>
          <CheckOutlined v-if="checkedKmsIds.includes(String(item.id))" class="selector-item-check" />
        </div>
      </template>
      <template v-if="filteredSkillList.length">
        <div class="selector-group-title">技能</div>
        <div
          v-for="(item, i) in filteredSkillList"
          :key="'skill-' + item.id"
          class="selector-item"
          :class="{ selected: checkedSkillIds.includes(String(item.id)), highlighted: highlightIndex === filteredKmsList.length + i }"
          @click="toggleItem('skill', item)"
          @mouseenter="highlightIndex = filteredKmsList.length + i"
        >
          <ThunderboltOutlined class="selector-item-icon" />
          <span class="selector-item-text">
            <span class="selector-item-name">{{ item.name }}</span>
            <span v-if="item.description" class="selector-item-desc">{{ briefDesc(item.description) }}</span>
          </span>
          <CheckOutlined v-if="checkedSkillIds.includes(String(item.id))" class="selector-item-check" />
        </div>
      </template>
      <template v-if="filteredMcpList.length">
        <div class="selector-group-title">MCP</div>
        <div
          v-for="(item, i) in filteredMcpList"
          :key="'mcp-' + item.id"
          class="selector-item"
          :class="{ selected: checkedMcpIds.includes(String(item.id)), highlighted: highlightIndex === filteredKmsList.length + filteredSkillList.length + i }"
          @click="toggleItem('mcp', item)"
          @mouseenter="highlightIndex = filteredKmsList.length + filteredSkillList.length + i"
        >
          <ApiOutlined class="selector-item-icon" />
          <span class="selector-item-text">
            <span class="selector-item-name">{{ item.name }}</span>
            <span v-if="item.description" class="selector-item-desc">{{ briefDesc(item.description) }}</span>
          </span>
          <CheckOutlined v-if="checkedMcpIds.includes(String(item.id))" class="selector-item-check" />
        </div>
      </template>
      <a-empty
        v-if="filteredKmsList.length === 0 && filteredSkillList.length === 0 && filteredMcpList.length === 0"
        :description="loading ? '加载中…' : '暂无可选知识库 / 技能 / MCP'"
        :image-style="{ height: '48px' }"
      />
    </div>
    <div class="selector-foot">
      <span class="selector-switch-item">
        <a-switch
          :checked="localExcludeDefaultKmss"
          size="small"
          @change="(v) => onSwitchChange('kmss', v)"
        />
        <span class="selector-switch-label">忽略默认知识库</span>
      </span>
      <span class="selector-switch-item">
        <a-switch
          :checked="localExcludeDefaultSkills"
          size="small"
          @change="(v) => onSwitchChange('skills', v)"
        />
        <span class="selector-switch-label">忽略默认技能</span>
      </span>
      <a class="selector-clear" @click="clearAll">清空已选</a>
    </div>
  </div>
</template>

<script setup>
/* eslint-disable */
import { ref, computed, watch, onBeforeUnmount, nextTick } from "vue";
import { SearchOutlined, CheckOutlined, BookOutlined, ThunderboltOutlined, ApiOutlined } from "@ant-design/icons-vue";
import { getAIKmssList } from "@/api/ai/aikmss";
import { GetAllSkills, GetAllMcps } from "@/api/ai/aiskilltoolManagement";

// Props：open 控制显隐；kmsIds/skillIds/mcpIds 为当前已选（打开时回显）；
// excludeDefaultKmss/excludeDefaultSkills 为两个"忽略默认"开关的受控值
const props = defineProps({
  open: { type: Boolean, default: false },
  kmsIds: { type: Array, default: () => [] },
  skillIds: { type: Array, default: () => [] },
  mcpIds: { type: Array, default: () => [] },
  excludeDefaultKmss: { type: Boolean, default: false },
  excludeDefaultSkills: { type: Boolean, default: false },
});

// emit：update:open（关闭面板）+ confirm（任一选择变化即时透出，面板保持打开便于连续多选）
const emit = defineEmits(["update:open", "confirm"]);

const searchRef = ref(null);
const kmsList = ref([]);
const skillList = ref([]);
const mcpList = ref([]);
const search = ref("");
const checkedKmsIds = ref([]);
const checkedSkillIds = ref([]);
const checkedMcpIds = ref([]);
const localExcludeDefaultKmss = ref(false);
const localExcludeDefaultSkills = ref(false);
const loading = ref(false);

// 前端过滤（搜索框关键字同时匹配知识库与技能）
const matchKeyword = (item) => {
  const kw = search.value.trim().toLowerCase();
  if (!kw) return true;
  return (item.name || "").toLowerCase().includes(kw);
};
// 描述截断：超过 20 字用 ... 结尾（避免长描述撑爆面板行高）
const briefDesc = (text) => {
  const t = String(text || "").trim();
  return t.length > 20 ? t.slice(0, 20) + "..." : t;
};

const filteredKmsList = computed(() => kmsList.value.filter(matchKeyword));
const filteredSkillList = computed(() => skillList.value.filter(matchKeyword));
const filteredMcpList = computed(() => mcpList.value.filter(matchKeyword));

// 键盘导航：↑↓ 在"知识库+技能"扁平列表上移动高亮，回车选中高亮项
const highlightIndex = ref(0);
const flatCount = computed(() => filteredKmsList.value.length + filteredSkillList.value.length + filteredMcpList.value.length);
watch(search, () => {
  highlightIndex.value = 0;
});
const onSearchKeydown = (e) => {
  if (e.key === "ArrowDown") {
    e.preventDefault();
    highlightIndex.value = Math.min(highlightIndex.value + 1, Math.max(flatCount.value - 1, 0));
  } else if (e.key === "ArrowUp") {
    e.preventDefault();
    highlightIndex.value = Math.max(highlightIndex.value - 1, 0);
  } else if (e.key === "Enter") {
    e.preventDefault();
    const flat = [
      ...filteredKmsList.value.map((t) => ({ type: "kms", item: t })),
      ...filteredSkillList.value.map((t) => ({ type: "skill", item: t })),
      ...filteredMcpList.value.map((t) => ({ type: "mcp", item: t })),
    ];
    const current = flat[highlightIndex.value];
    if (current) toggleItem(current.type, current.item);
  }
};

const loadMcpsList = async () => {
  try {
    const response = await GetAllMcps();
    if (response && response.code === 200 && response.data) {
      mcpList.value = response.data;
    } else {
      console.warn("Unexpected response format for GetAllMcps:", response);
      mcpList.value = [];
    }
  } catch (error) {
    console.error("加载MCP列表失败:", error);
    mcpList.value = [];
  }
};

// 打开时拉取两个列表、回显当前已选，并聚焦搜索框
watch(
  () => props.open,
  (open) => {
    if (open) {
      checkedKmsIds.value = (props.kmsIds || []).map((t) => String(t));
      checkedSkillIds.value = (props.skillIds || []).map((t) => String(t));
      checkedMcpIds.value = (props.mcpIds || []).map((t) => String(t));
      localExcludeDefaultKmss.value = props.excludeDefaultKmss;
      localExcludeDefaultSkills.value = props.excludeDefaultSkills;
      search.value = "";
      loadKmsList();
      loadSkillsList();
      loadMcpsList();
      nextTick(() => searchRef.value && searchRef.value.focus && searchRef.value.focus());
      document.addEventListener("mousedown", onOutsideMousedown, true);
      window.addEventListener("keydown", onEscKeydown, true);
    } else {
      document.removeEventListener("mousedown", onOutsideMousedown, true);
      window.removeEventListener("keydown", onEscKeydown, true);
    }
  }
);

onBeforeUnmount(() => {
  document.removeEventListener("mousedown", onOutsideMousedown, true);
  window.removeEventListener("keydown", onEscKeydown, true);
});

// 点击面板外关闭；面板内点击与触发按钮（.kms-skill-trigger-btn）除外（前者是正常选择操作，后者由父组件 toggle 处理）。
// 注意：监听挂在 capture 阶段，必须在监听内自行排除面板内部目标，面板自身的 @mousedown.stop 拦不住捕获阶段
const onOutsideMousedown = (e) => {
  const target = e.target;
  if (target && target.closest && target.closest(".selector-popover")) return;
  if (target && target.closest && target.closest(".kms-skill-trigger-btn")) return;
  emit("update:open", false);
};

const onEscKeydown = (e) => {
  if (e.key === "Escape") emit("update:open", false);
};

const loadKmsList = async () => {
  try {
    const response = await getAIKmssList({});
    if (response && response.code === 200) {
      if (response.data && Array.isArray(response.data)) {
        kmsList.value = response.data;
      } else if (response.data && response.data.list && Array.isArray(response.data.list)) {
        kmsList.value = response.data.list;
      } else if (Array.isArray(response)) {
        kmsList.value = response;
      } else {
        console.warn("Unexpected response format for getAIKmssList:", response);
        kmsList.value = [];
      }
    } else {
      console.error("Failed to load knowledge base list:", response?.message || "Unknown error");
      kmsList.value = [];
    }
  } catch (error) {
    console.error("加载知识库列表失败:", error);
    kmsList.value = [];
  }
};

const loadSkillsList = async () => {
  try {
    const response = await GetAllSkills();
    if (response && response.code === 200 && response.data) {
      skillList.value = response.data;
    } else {
      console.warn("Unexpected response format for GetAllSkills:", response);
      skillList.value = [];
    }
  } catch (error) {
    console.error("加载技能列表失败:", error);
    skillList.value = [];
  }
};

// 点击条目：即时切换选中并透出结果，面板保持打开（连续多选）
const toggleItem = (type, item) => {
  const id = String(item.id);
  const target = type === "kms" ? checkedKmsIds : type === "skill" ? checkedSkillIds : checkedMcpIds;
  const idx = target.value.indexOf(id);
  if (idx >= 0) target.value.splice(idx, 1);
  else target.value.push(id);
  emitSelection();
};

const onSwitchChange = (type, value) => {
  if (type === "kmss") localExcludeDefaultKmss.value = value;
  else localExcludeDefaultSkills.value = value;
  emitSelection();
};

const clearAll = () => {
  checkedKmsIds.value = [];
  checkedSkillIds.value = [];
  checkedMcpIds.value = [];
  emitSelection();
};

// 透出当前选择（仅随本轮请求透传，不落库；面板不关闭）
const emitSelection = () => {
  const kmsItems = kmsList.value
    .filter((t) => checkedKmsIds.value.includes(String(t.id)))
    .map((t) => ({ id: String(t.id), name: t.name }));
  const skillItems = skillList.value
    .filter((t) => checkedSkillIds.value.includes(String(t.id)))
    .map((t) => ({ id: String(t.id), name: t.name }));
  const mcpItems = mcpList.value
    .filter((t) => checkedMcpIds.value.includes(String(t.id)))
    .map((t) => ({ id: String(t.id), name: t.name }));
  emit("confirm", {
    kmsIds: checkedKmsIds.value,
    skillIds: checkedSkillIds.value,
    mcpIds: checkedMcpIds.value,
    excludeDefaultKmss: localExcludeDefaultKmss.value,
    excludeDefaultSkills: localExcludeDefaultSkills.value,
    // 扩展字段：供父组件 chips 直接展示名称（契约字段 kmsIds/skillIds 等保持不变）
    kmsItems,
    skillItems,
    mcpItems,
  });
};
</script>

<style scoped>
/* 锚定在输入框上方：父容器 .input-wrapper 需 position:relative（父组件已加） */
.selector-popover {
  position: absolute;
  left: 8px;
  right: 8px;
  bottom: calc(100% - 4px);
  z-index: 1000;
  background: #fff;
  border: 1px solid rgba(0, 0, 0, 0.08);
  border-radius: 10px;
  box-shadow: 0 6px 24px rgba(0, 0, 0, 0.14);
  overflow: hidden;
}
.selector-head {
  padding: 8px 8px 4px;
  border-bottom: 1px solid #f5f5f5;
}
.selector-body {
  max-height: 300px;
  overflow-y: auto;
  padding: 4px 0;
}
.selector-group-title {
  font-size: 12px;
  color: #999;
  padding: 6px 12px 2px;
  user-select: none;
}
.selector-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px;
  cursor: pointer;
  font-size: 13px;
  transition: background 0.15s;
}
.selector-item:hover,
.selector-item.highlighted {
  background: #f5f7fa;
}
.selector-item.selected {
  background: #f0f7ff;
}
.selector-item-icon {
  color: #bfbfbf;
  font-size: 13px;
}
.selector-item.selected .selector-item-icon {
  color: #1677ff;
}
.selector-item-text {
  flex: 1;
  display: flex;
  align-items: baseline;
  gap: 6px;
  min-width: 0;
}
.selector-item-name {
  flex-shrink: 0;
  max-width: 50%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.selector-item-desc {
  flex: 1;
  font-size: 11px;
  color: #999;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.selector-item-check {
  color: #1677ff;
  font-size: 13px;
}
.selector-foot {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 8px 12px;
  border-top: 1px solid #f5f5f5;
  background: #fafafa;
}
.selector-switch-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #666;
}
.selector-switch-label {
  user-select: none;
}
.selector-clear {
  margin-left: auto;
  font-size: 12px;
  color: #999;
}
.selector-clear:hover {
  color: #1677ff;
}
</style>
