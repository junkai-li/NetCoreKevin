<template>
  <div class="openai-key-container">
    <a-card class="openai-key-card">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <KeyOutlined class="title-icon" />
            <span>OpenAI 兼容接口 · 我的对接 Key</span>
          </div>
          <div class="header-actions">
            <a-tag v-if="keyInfo && keyInfo.isExists" :color="keyInfo.isEnabled ? 'green' : 'red'">
              {{ keyInfo.isEnabled ? '已启用' : '已禁用' }}
            </a-tag>
          </div>
        </div>
      </template>

      <!-- 未生成时：生成入口 -->
      <div v-if="!loading && (!keyInfo || !keyInfo.isExists)" class="empty-state">
        <div class="empty-icon"><KeyOutlined /></div>
        <div class="empty-text">你还没有生成 OpenAI 兼容接口的 API Key</div>
        <div class="empty-tip">
          生成后，你将获得一个以 <code>sk-kv-</code> 开头的密钥，可用于任何 OpenAI 协议客户端调用本平台智能体。
          <br />Key 默认加密存储，明文仅在生成或你主动"显示"时展示。
        </div>
        <a-button type="primary" size="large" @click="openGenerateModal">
          <template #icon><PlusOutlined /></template>
          立即生成
        </a-button>
      </div>

      <!-- 已生成时：Key 详情 + 操作 -->
      <div v-else-if="keyInfo && keyInfo.isExists" class="key-detail">
        <a-descriptions :column="2" bordered size="small">
          <a-descriptions-item label="备注名">
            <a-space>
              <a-input
                v-if="editingName"
                v-model:value="editName"
                size="small"
                style="width: 220px"
                @press-enter="saveName"
              />
              <span v-else>{{ keyInfo.name || '（未设置）' }}</span>
              <a-button v-if="!editingName" size="small" @click="startEditName">
                <EditOutlined />
              </a-button>
              <template v-else>
                <a-button size="small" type="primary" @click="saveName">保存</a-button>
                <a-button size="small" @click="cancelEditName">取消</a-button>
              </template>
            </a-space>
          </a-descriptions-item>
          <a-descriptions-item label="Key 脱敏">
            <a-space>
              <span class="key-mask">{{ keyInfo.maskKey }}</span>
              <a-tooltip title="复制脱敏片段（仅片段，不含完整明文）">
                <a-button size="small" @click="copyText(keyInfo.maskKey, '已复制脱敏片段')">
                  <CopyOutlined />
                </a-button>
              </a-tooltip>
            </a-space>
          </a-descriptions-item>
          <a-descriptions-item label="创建时间">
            {{ formatDate(keyInfo.createTime) }}
          </a-descriptions-item>
          <a-descriptions-item label="最近使用">
            {{ keyInfo.lastUsedTime ? formatDate(keyInfo.lastUsedTime) : '尚无' }}
          </a-descriptions-item>
        </a-descriptions>

        <div class="action-bar">
          <a-button type="primary" @click="openRevealModal">
            <template #icon><EyeOutlined /></template>
            查看明文
          </a-button>
          <a-button @click="openGenerateModal" :loading="regenerating">
            <template #icon><ReloadOutlined /></template>
            重新生成
          </a-button>
          <a-button v-if="keyInfo.isEnabled" @click="toggleEnabled(false)">
            <template #icon><PauseCircleOutlined /></template>
            禁用
          </a-button>
          <a-button v-else @click="toggleEnabled(true)">
            <template #icon><PlayCircleOutlined /></template>
            启用
          </a-button>
          <a-popconfirm
            title="确定要删除（作废）该 Key 吗？删除后外部调用将立即失败，且无法恢复。"
            ok-text="确定删除"
            cancel-text="取消"
            @confirm="onDelete"
          >
            <a-button danger>
              <template #icon><DeleteOutlined /></template>
              删除
            </a-button>
          </a-popconfirm>
        </div>
      </div>
    </a-card>

    <!-- 对接文档卡片 -->
    <a-card class="openai-key-card integration-card" v-if="integrationDoc">
      <template #title>
        <div class="card-header">
          <div class="header-title">
            <ApiOutlined class="title-icon" />
            <span>对接文档</span>
            <a-tag color="blue">OpenAI 协议兼容</a-tag>
          </div>
        </div>
      </template>

      <a-form layout="vertical" class="integration-form">
        <a-row :gutter="16">
          <a-col :span="14">
            <a-form-item label="Base URL" required>
              <a-input
                v-model:value="baseUrl"
                placeholder="https://your-host/api/v1"
                @change="renderSamples"
              >
                <template #suffix>
                  <a-tooltip title="复制">
                    <CopyOutlined style="cursor: pointer" @click="copyText(baseUrl, 'Base URL 已复制')" />
                  </a-tooltip>
                </template>
              </a-input>
              <div class="form-tip">后端已根据当前请求拼出，可改成你的公网域名（外部调用方需能访问到）</div>
            </a-form-item>
          </a-col>
          <a-col :span="10">
            <a-form-item label="Model（智能体名称）" required>
              <a-select
                v-model:value="selectedModelName"
                placeholder="选择可用的智能体"
                @change="renderSamples"
                show-search
                :filter-option="filterModelOption"
              >
                <a-select-option
                  v-for="m in integrationDoc.models"
                  :key="m.id"
                  :value="m.name"
                >
                  {{ m.name }} · {{ m.id }}
                </a-select-option>
              </a-select>
              <div class="form-tip">客户端 <code>model</code> 字段直接传智能体名称（也兼容智能体 Id）</div>
            </a-form-item>
          </a-col>
        </a-row>

        <a-form-item label="可用模型列表（GET /v1/models）">
          <a-space wrap>
            <a-tag v-for="m in integrationDoc.models" :key="m.id" color="purple">
              {{ m.name }} {{ m.id }}
            </a-tag>
            <a-tag v-if="!integrationDoc.models.length" color="default">暂无可用智能体</a-tag>
          </a-space>
        </a-form-item>

        <a-tabs v-model:activeKey="sampleTab" type="card">
          <a-tab-pane key="curl" tab="cURL">
            <pre class="code-block">{{ integrationDoc.samples.curl }}</pre>
            <a-button size="small" @click="copyText(integrationDoc.samples.curl, 'cURL 已复制')">
              <template #icon><CopyOutlined /></template>复制
            </a-button>
          </a-tab-pane>
          <a-tab-pane key="python" tab="Python (openai SDK)">
            <pre class="code-block">{{ integrationDoc.samples.python }}</pre>
            <a-button size="small" @click="copyText(integrationDoc.samples.python, 'Python 示例已复制')">
              <template #icon><CopyOutlined /></template>复制
            </a-button>
          </a-tab-pane>
          <a-tab-pane key="node" tab="Node.js (openai SDK)">
            <pre class="code-block">{{ integrationDoc.samples.nodeJs }}</pre>
            <a-button size="small" @click="copyText(integrationDoc.samples.nodeJs, 'Node.js 示例已复制')">
              <template #icon><CopyOutlined /></template>复制
            </a-button>
          </a-tab-pane>
        </a-tabs>

        <a-alert
          class="doc-tips"
          type="info"
          show-icon
          message="对接要点"
          description="1) Authorization 头传 Bearer &lt;key&gt;；2) model 默认传智能体名称（也兼容智能体 Id），可从 GET /v1/models 拉取；3) 流式/非流式都按 OpenAI 协议下发；4) 删除/禁用 Key 后网关约 5 分钟内使新缓存失效。"
        />
      </a-form>
    </a-card>

    <!-- 生成/重新生成 Modal：返回明文后强制用户保存 -->
    <a-modal
      v-model:open="generateModal.open"
      :title="generateModal.title"
      :footer="null"
      :mask-closable="false"
      :keyboard="false"
      :closable="!!generateModal.plainKey"
    >
      <div v-if="!generateModal.plainKey">
        <a-form layout="vertical">
          <a-form-item label="备注名（可选）">
            <a-input
              v-model:value="generateModal.name"
              placeholder="例如：我的桌面客户端"
              :maxlength="100"
            />
          </a-form-item>
          <a-alert
            v-if="keyInfo && keyInfo.isExists"
            type="warning"
            show-icon
            style="margin-bottom: 16px"
            message="你已存在一个 Key，重新生成将立即作废旧 Key。正在使用旧 Key 的客户端会立刻失败。"
          />
          <div class="modal-footer">
            <a-button @click="generateModal.open = false">取消</a-button>
            <a-button type="primary" :loading="generateModal.loading" @click="onGenerate">
              {{ keyInfo && keyInfo.isExists ? '确认重新生成' : '生成' }}
            </a-button>
          </div>
        </a-form>
      </div>
      <div v-else class="plain-key-block">
        <a-alert
          type="success"
          show-icon
          message="生成成功！请立即复制并妥善保存，关闭后将无法再次查看明文。"
          style="margin-bottom: 12px"
        />
        <a-input-group compact>
          <a-input
            v-model:value="generateModal.plainKey"
            readonly
            style="width: calc(100% - 90px); font-family: 'Cascadia Code', Consolas, monospace"
          />
          <a-button type="primary" @click="copyText(generateModal.plainKey, 'Key 已复制到剪贴板')">
            <template #icon><CopyOutlined /></template>复制
          </a-button>
        </a-input-group>
        <div class="modal-footer">
          <a-button type="primary" @click="closeGenerateModal">我已保存，关闭</a-button>
        </div>
      </div>
    </a-modal>

    <!-- 查看明文 Modal -->
    <a-modal
      v-model:open="revealModal.open"
      :footer="null"
      :mask-closable="false"
      :keyboard="false"
      :closable="!!revealModal.plainKey"
      title="查看 Key 明文"
    >
      <div v-if="!revealModal.plainKey">
        <a-alert
          type="warning"
          show-icon
          message="明文展示涉及敏感信息，请确认周围无人窥屏后再继续。"
          style="margin-bottom: 16px"
        />
        <div class="modal-footer">
          <a-button @click="revealModal.open = false">取消</a-button>
          <a-button type="primary" :loading="revealModal.loading" @click="onReveal">
            我已知晓，展示明文
          </a-button>
        </div>
      </div>
      <div v-else class="plain-key-block">
        <a-input-group compact>
          <a-input
            v-model:value="revealModal.plainKey"
            readonly
            style="width: calc(100% - 90px); font-family: 'Cascadia Code', Consolas, monospace"
          />
          <a-button type="primary" @click="copyText(revealModal.plainKey, 'Key 已复制到剪贴板')">
            <template #icon><CopyOutlined /></template>复制
          </a-button>
        </a-input-group>
        <div class="modal-footer">
          <a-button type="primary" @click="closeRevealModal">关闭</a-button>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue';
import {
  KeyOutlined,
  PlusOutlined,
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  CopyOutlined,
  ReloadOutlined,
  PauseCircleOutlined,
  PlayCircleOutlined,
  ApiOutlined,
} from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';
import {
  getMyOpenAIKey,
  generateOpenAIKey,
  revealOpenAIKey,
  updateOpenAIKeyName,
  setOpenAIKeyEnabled,
  deleteOpenAIKey,
  getOpenAIIntegrationDoc,
} from '@/api/ai/openaiCompatibleKey';

const loading = ref(false);
const regenerating = ref(false);

const keyInfo = ref(null); // OpenAICompatibleKeyInfoDto
const integrationDoc = ref(null); // OpenAICompatibleIntegrationDocDto
const baseUrl = ref('');
const selectedModelName = ref(null);
const sampleTab = ref('curl');

// ===== 备注名编辑 =====
const editingName = ref(false);
const editName = ref('');
const startEditName = () => {
  editName.value = keyInfo.value?.name || '';
  editingName.value = true;
};
const cancelEditName = () => {
  editingName.value = false;
};
const saveName = async () => {
  try {
    await updateOpenAIKeyName(editName.value);
    keyInfo.value.name = editName.value || null;
    editingName.value = false;
    message.success('备注已更新');
  } catch (e) {
    message.error('保存失败：' + (e.message || '未知错误'));
  }
};

// ===== 启用/禁用 =====
const toggleEnabled = async (target) => {
  try {
    await setOpenAIKeyEnabled(target);
    keyInfo.value.isEnabled = target;
    message.success(target ? '已启用' : '已禁用');
  } catch (e) {
    message.error('操作失败：' + (e.message || '未知错误'));
  }
};

// ===== 删除 =====
const onDelete = async () => {
  try {
    await deleteOpenAIKey();
    message.success('Key 已删除');
    keyInfo.value = { isExists: false };
  } catch (e) {
    message.error('删除失败：' + (e.message || '未知错误'));
  }
};

// ===== 生成/重新生成 Modal =====
const generateModal = reactive({
  open: false,
  loading: false,
  title: '生成 Key',
  name: '',
  plainKey: '',
});
const openGenerateModal = () => {
  generateModal.title = keyInfo.value?.isExists ? '重新生成 Key' : '生成 Key';
  generateModal.name = '';
  generateModal.plainKey = '';
  generateModal.open = true;
};
const onGenerate = async () => {
  generateModal.loading = true;
  try {
    const res = await generateOpenAIKey(generateModal.name);
    // http 拦截器返回的是整个响应包 {code, msg, isSuccess, data}，明文在 data 里
    if (res && res.code === 200 && typeof res.data === 'string') {
      generateModal.plainKey = res.data;
    } else {
      throw new Error(res?.msg || '生成返回数据格式异常');
    }
    // 重新拉一次元数据（更新 maskKey/创建时间等），并刷新对接文档里的 key 示例
    await loadKeyInfo();
    renderSamples();
  } catch (e) {
    message.error('生成失败：' + (e.message || '未知错误'));
  } finally {
    generateModal.loading = false;
  }
};
const closeGenerateModal = () => {
  generateModal.open = false;
  generateModal.plainKey = '';
};

// ===== 查看明文 Modal =====
const revealModal = reactive({
  open: false,
  loading: false,
  plainKey: '',
});
const openRevealModal = () => {
  revealModal.plainKey = '';
  revealModal.open = true;
};
const onReveal = async () => {
  revealModal.loading = true;
  try {
    const res = await revealOpenAIKey();
    // http 拦截器返回的是整个响应包 {code, msg, isSuccess, data}，明文在 data 里
    if (res && res.code === 200 && typeof res.data === 'string') {
      revealModal.plainKey = res.data;
    } else {
      throw new Error(res?.msg || '查看明文返回数据格式异常');
    }
  } catch (e) {
    message.error('获取明文失败：' + (e.message || '未知错误'));
  } finally {
    revealModal.loading = false;
  }
};
const closeRevealModal = () => {
  revealModal.open = false;
  revealModal.plainKey = '';
};

// ===== 工具 =====
const formatDate = (d) => {
  if (!d) return '';
  return new Date(d).toLocaleString('zh-CN', { hour12: false });
};
const copyText = async (text, okText) => {
  if (!text) {
    message.warning('内容为空');
    return;
  }
  try {
    if (navigator.clipboard && window.isSecureContext) {
      await navigator.clipboard.writeText(text);
    } else {
      const ta = document.createElement('textarea');
      ta.value = text;
      ta.style.position = 'fixed';
      ta.style.opacity = '0';
      document.body.appendChild(ta);
      ta.select();
      document.execCommand('copy');
      document.body.removeChild(ta);
    }
    message.success(okText || '已复制');
  } catch (e) {
    message.error('复制失败：' + (e.message || '未知错误'));
  }
};
const filterModelOption = (input, option) => {
  const text = option.children.join ? option.children.join('') : String(option.children);
  return text.toLowerCase().includes(input.toLowerCase());
};

// ===== 渲染示例代码（key 用明文时只展示脱敏；model 用选中值）=====
const renderSamples = () => {
  if (!integrationDoc.value) return;
  const base = (baseUrl.value || '').replace(/\/+$/, '');
  const model = selectedModelName.value != null ? String(selectedModelName.value) : '<model name>';
  // 明文 key 不直接进 store（无 plaintext 时用占位）
  const key = keyInfo.value?.maskKey || '<your-api-key>';
  integrationDoc.value.samples = {
    curl: buildCurl(base, key, model),
    python: buildPython(base, key, model),
    nodeJs: buildNode(base, key, model),
  };
};

const buildCurl = (base, key, model) => `curl "${base}/chat/completions" \\
  -H "Content-Type: application/json" \\
  -H "Authorization: Bearer ${key}" \\
  -d '{
    "model": "${model}",
    "messages": [
      { "role": "system", "content": "You are a helpful assistant." },
      { "role": "user", "content": "你好" }
    ]
  }'`;

const buildPython = (base, key, model) => `# pip install openai
from openai import OpenAI

client = OpenAI(
    api_key="${key}",
    base_url="${base}",
)

resp = client.chat.completions.create(
    model="${model}",
    messages=[
        { "role": "system", "content": "You are a helpful assistant." },
        { "role": "user", "content": "你好" },
    ],
)
print(resp.choices[0].message.content)`;

const buildNode = (base, key, model) => `// npm install openai
import OpenAI from "openai";

const client = new OpenAI({
  apiKey: "${key}",
  baseURL: "${base}",
});

const completion = await client.chat.completions.create({
  model: "${model}",
  messages: [
    { role: "system", content: "You are a helpful assistant." },
    { role: "user", content: "你好" },
  ],
});
console.log(completion.choices[0].message.content);`;

// ===== 加载数据 =====
const loadKeyInfo = async () => {
  loading.value = true;
  try {
    const res = await getMyOpenAIKey();
    if (res && res.code === 200) {
      keyInfo.value = res.data;
    }
  } catch (e) {
    message.error('加载 Key 信息失败：' + (e.message || '未知错误'));
  } finally {
    loading.value = false;
  }
};

const loadIntegrationDoc = async () => {
  try {
    const res = await getOpenAIIntegrationDoc();
    if (res && res.code === 200) {
      integrationDoc.value = res.data;
      baseUrl.value = res.data.baseUrl || '';
      if (res.data.models && res.data.models.length > 0) {
        selectedModelName.value = res.data.models[0].name;
      }
      renderSamples();
    }
  } catch (e) {
    message.error('加载对接文档失败：' + (e.message || '未知错误'));
  }
};

watch(selectedModelName, () => renderSamples());
watch(baseUrl, () => renderSamples());

onMounted(async () => {
  await loadKeyInfo();
  await loadIntegrationDoc();
});
</script>

<style scoped>
.openai-key-container {
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  max-width: 1200px;
  margin: 0 auto;
}

.openai-key-card {
  border-radius: 8px;
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.header-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
}

.title-icon {
  color: var(--accent, #1677ff);
}

.empty-state {
  text-align: center;
  padding: 48px 16px;
}

.empty-icon {
  font-size: 64px;
  color: #d9d9d9;
  margin-bottom: 16px;
}

.empty-text {
  font-size: 16px;
  margin-bottom: 8px;
}

.empty-tip {
  color: rgba(0, 0, 0, 0.55);
  margin-bottom: 24px;
  line-height: 1.8;
}

.key-detail {
  padding: 8px 0;
}

.key-mask {
  font-family: 'Cascadia Code', Consolas, monospace;
  background: rgba(0, 0, 0, 0.04);
  padding: 2px 8px;
  border-radius: 4px;
  letter-spacing: 0.5px;
}

.action-bar {
  margin-top: 16px;
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.integration-card {
  margin-top: 4px;
}

.integration-form {
  padding-top: 8px;
}

.form-tip {
  color: rgba(0, 0, 0, 0.45);
  font-size: 12px;
  margin-top: 4px;
}

.code-block {
  background: #1e1e1e;
  color: #d4d4d4;
  padding: 16px;
  border-radius: 6px;
  font-family: 'Cascadia Code', Consolas, monospace;
  font-size: 13px;
  line-height: 1.6;
  overflow-x: auto;
  white-space: pre;
  margin-bottom: 8px;
}

.doc-tips {
  margin-top: 16px;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}

.plain-key-block {
  padding: 8px 0;
}
</style>
