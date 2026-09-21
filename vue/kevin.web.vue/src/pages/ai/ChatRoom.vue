<template>
  <div class="chatroom-page">
    <!-- 未进入房间：选择智能体开始 -->
    <div v-if="!chatId" class="entry">
      <a-card title="进入智能体聊天室" :bordered="false" class="entry-card">
        <a-select
          v-model:value="selectedAppId"
          style="width: 100%"
          placeholder="请选择智能体"
          :options="agentOptions"
          show-search
          option-filter-prop="label"
        />
        <div class="entry-tip">基于 SignalR 的一对一实时聊天：可连续提问，回答异步流式返回，无需等待上一条完成。</div>
        <a-button type="primary" :loading="entering" :disabled="!selectedAppId" @click="startNewRoom">
          新建聊天室
        </a-button>
      </a-card>
    </div>

    <!-- 房间内 -->
    <template v-else>
      <div class="room-header">
        <div class="room-title">{{ roomName }}</div>
        <a-tag :color="connState === 'connected' ? 'green' : connState === 'connecting' ? 'blue' : 'red'">
          {{ connText }}
        </a-tag>
      </div>

      <div ref="msgListRef" class="msg-list">
        <div v-for="m in messages" :key="m.key" class="msg-row" :class="m.isSend ? 'row-user' : 'row-ai'">
          <div class="bubble" :class="m.isSend ? 'bub-user' : 'bub-ai'">
            <!-- 提问 -->
            <template v-if="m.isSend">
              <div class="msg-content pre-wrap">{{ m.content }}</div>
              <div v-if="isAskFailed(m)" class="ask-fail">
                <a-tooltip :title="m.failReason || '发送失败'">
                  <a-button size="small" danger @click="retryAsk(m)">重试</a-button>
                </a-tooltip>
              </div>
            </template>
            <!-- 回答 -->
            <template v-else>
              <div v-if="m.process && m.sending" class="msg-process">{{ m.process }}</div>
              <div class="msg-content pre-wrap">{{ m.content || (m.sending ? '' : m.error || '') }}</div>
              <a-collapse v-if="m.reasoning" v-model:active-key="m.reasoningActiveKey" class="message-collapse" ghost>
                <a-collapse-panel key="reasoning" header="思考过程">
                  <div class="collapse-content">
                    <div v-if="m.reasoning.length > 350">{{ truncateContent(m.reasoning) }}<a @click="showDetailModal('思考过程详情', m.reasoning)">点击查看详情</a></div>
                    <div v-else>{{ m.reasoning }}</div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <a-collapse v-if="m.tools" v-model:active-key="m.toolsActiveKey" class="message-collapse" ghost>
                <a-collapse-panel key="tools" header="工具调用">
                  <div class="collapse-content">
                    <div v-if="m.tools.length > 350">{{ truncateContent(m.tools) }}<a @click="showDetailModal('工具调用详情', m.tools)">点击查看详情</a></div>
                    <div v-else>{{ m.tools }}</div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <div v-if="m.sending" class="typing">● 正在输入…</div>
              <div v-if="m.error && !m.sending" class="ai-error pre-wrap">{{ m.error }}</div>
              <div v-if="!m.sending && m.totalTokenCount" class="msg-meta">tokens: {{ m.totalTokenCount }}</div>
            </template>
          </div>
        </div>
      </div>

      <div class="composer">
        <div class="composer-tools">
          <a-switch v-model:checked="onlineSearch" size="small" />
          <span class="tool-label">联网搜索</span>
        </div>
        <a-textarea
          v-model:value="draft"
          :auto-size="{ minRows: 2, maxRows: 6 }"
          placeholder="输入消息，Enter 发送，Shift+Enter 换行"
          @pressEnter="onPressEnter"
        />
        <div class="composer-actions">
          <a-button type="primary" :disabled="!draft.trim() || connState !== 'connected'" @click="doSend()">
            发送
          </a-button>
        </div>
      </div>

      <a-modal v-model:open="detailModalVisible" :title="detailModalTitle" :footer="null" width="1000px">
        <div class="detail-modal-body">{{ detailModalContent }}</div>
      </a-modal>
    </template>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { message } from 'ant-design-vue';
import * as signalR from '@microsoft/signalr';
import { getMyAIAppsALLList } from '@/api/ai/aiapps';
import { addAIChats } from '@/api/ai/aichats';
import { getAIChatHistorysPageData } from '@/api/ai/aichathistorys';
import { GetSnowflakeId } from '@/api/baseapi';

const route = useRoute();
const API_BASE = process.env.VUE_APP_API_BASE_URL;

const chatId = ref('');
const roomName = ref('聊天室');
const draft = ref('');
const onlineSearch = ref(false);
const messages = reactive([]);
const msgListRef = ref(null);

const agentOptions = ref([]);
const selectedAppId = ref(undefined);
const entering = ref(false);

// 连接状态：connecting / connected / disconnected
const connState = ref('disconnected');
const connText = computed(() => ({ connecting: '连接中', connected: '已连接', disconnected: '已断开' }[connState.value] || '未连接'));

let connection = null;
let uidSeq = 1;
const nextKey = () => `m_${Date.now()}_${uidSeq++}`;

const isAskFailed = (m) => m.isSend && (m.sendStatus === 1 || m.sendStatus === 2);

// ==== 思考过程/工具调用展示：与 MyAIChat 同一套逻辑（折叠面板 + 超 350 字截断 + 详情弹窗）====
const detailModalVisible = ref(false);
const detailModalTitle = ref('');
const detailModalContent = ref('');
const showDetailModal = (title, content) => {
  detailModalTitle.value = title;
  detailModalContent.value = content;
  detailModalVisible.value = true;
};
const truncateContent = (content, maxLength = 350) => {
  if (!content || content.length <= maxLength) return content;
  return content.substring(0, maxLength) + '...';
};
// 流式追加并按 MyAIChat 的规则自动展开/收起：首次出现且不超过 300 字自动展开，超过 300 字自动收起一次；
// 聊天室多问并行，展开状态挂在每条气泡自己对象上，面板 key 与内容字段同名（reasoning/tools）
const appendStreamField = (m, field, keyField, flagField, add) => {
  const before = m[field].length;
  m[field] += add;
  const after = m[field].length;
  if (before === 0 && after <= 300) m[keyField] = [field];
  if (before <= 300 && after > 300 && !m[flagField]) {
    m[keyField] = [];
    m[flagField] = true;
  }
};

const scrollToBottom = () => {
  nextTick(() => {
    const el = msgListRef.value;
    if (el) el.scrollTop = el.scrollHeight;
  });
};

// ==== 加载可用智能体 ====
const loadAgents = async () => {
  try {
    const res = await getMyAIAppsALLList();
    if (res && res.code === 200 && Array.isArray(res.data)) {
      agentOptions.value = res.data.map((a) => ({ value: a.id, label: a.name }));
    }
  } catch (e) {
    console.error('加载智能体失败', e);
  }
};

// ==== SignalR 连接：一条常驻双向连接，房间分组名 = chatId ====
const openConnection = async () => {
  if (connection) {
    try { await connection.stop(); } catch (e) { /* ignore */ }
    connection = null;
  }
  const token = localStorage.getItem('token');
  connState.value = 'connecting';
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE}/api/ChatRoomHub?IdentityId=${encodeURIComponent(chatId.value)}&Authorization=${encodeURIComponent(token || '')}`, {
      headers: { Authorization: `Bearer ${token || ''}` },
    })
    .withAutomaticReconnect()
    .build();

  connection.onreconnecting(() => { connState.value = 'connecting'; });
  connection.onreconnected(() => { connState.value = 'connected'; });
  connection.onclose(() => { connState.value = 'disconnected'; });

  connection.keepAliveIntervalInMilliseconds = 12e4;
  connection.serverTimeoutInMilliseconds = 24e4;

  // 所有监听器注册完再 start，避免抢在注册前到达的分片被丢弃
  connection.on('chatmsg', onChatMsg);
  connection.on('chatdone', onChatDone);
  connection.on('chaterror', onChatError);

  try {
    await connection.start();
    connState.value = 'connected';
  } catch (err) {
    connState.value = 'disconnected';
    console.warn('SignalR 连接失败', err);
    message.error('聊天室连接失败，请刷新重试');
  }
};

// 按 askId 找到对应“回答”气泡
const findAnswer = (askId) => messages.find((m) => !m.isSend && String(m.askId) === String(askId));

// 流式分片：{ askId, channel, msg }
const onChatMsg = (payloadStr) => {
  let p;
  try { p = JSON.parse(payloadStr); } catch (e) { return; }
  const m = findAnswer(p.askId);
  if (!m) return;
  switch (p.channel) {
    case 'aimsg': m.content += p.msg; break;
    case 'processmsg': m.process = p.msg; break;
    case 'aIToolsContentMsg': appendStreamField(m, 'tools', 'toolsActiveKey', 'toolsAutoCollapsed', p.msg); break;
    case 'aIReasoningContentMsg': appendStreamField(m, 'reasoning', 'reasoningActiveKey', 'reasoningAutoCollapsed', p.msg); break;
  }
  scrollToBottom();
};

// 本轮定稿：{ askId, result }（result 与 HTTP Add 的 data 同构，雪花 Id 为字符串）
const onChatDone = (jsonStr) => {
  let p;
  try { p = JSON.parse(jsonStr); } catch (e) { return; }
  const m = findAnswer(p.askId);
  if (!m) return;
  const result = p.result || {};
  m.sending = false;
  m.process = '';
  m.error = '';
  m.dbId = result.id || m.askId;
  m.totalTokenCount = result.totalTokenCount || 0;
  const sendFail = result.sendStatus === 1 || result.sendStatus === 2;
  // 定稿正文：优先用流式已收文本，回退用后端返回正文
  if (!m.content && result.content) m.content = result.content;
  if (result.aiReasoningContent && !m.reasoning) {
    m.reasoning = result.aiReasoningContent;
    m.reasoningActiveKey = ['reasoning'];
  }
  if (result.aiToolsContent && !m.tools) {
    m.tools = result.aiToolsContent;
    m.toolsActiveKey = ['tools'];
  }
  // 把失败状态与报错同步到配对的提问气泡上，给出重试入口
  const userMsg = messages.find((x) => x.isSend && String(x.askId) === String(p.askId));
  if (userMsg) {
    userMsg.sendStatus = sendFail ? 1 : 0;
    userMsg.failReason = result.failReason || '';
  }
  scrollToBottom();
};

// 业务错误：{ askId, message }
const onChatError = (jsonStr) => {
  let p;
  try { p = JSON.parse(jsonStr); } catch (e) { return; }
  const m = findAnswer(p.askId);
  if (m) { m.sending = false; m.process = ''; m.error = p.message || '发送失败'; }
  const userMsg = messages.find((x) => x.isSend && String(x.askId) === String(p.askId));
  if (userMsg) { userMsg.sendStatus = 1; userMsg.failReason = p.message || '发送失败'; }
  scrollToBottom();
};

// ==== 发送：不 await 完成，立即允许下一次发送（多问并行） ====
const sendOne = async ({ content, retryOfId = '' }) => {
  if (connState.value !== 'connected' || !connection) {
    message.warning('聊天室尚未连接');
    return;
  }
  const text = (content || '').trim();
  if (!text) return;

  let askId = '';
  try {
    askId = String((await GetSnowflakeId()).data);
  } catch (e) {
    message.error('获取消息 Id 失败');
    return;
  }

  const userMsg = reactive({
    key: nextKey(), askId, isSend: true, content: text,
    sendStatus: 0, failReason: '', retryOfId,
  });
  const aiMsg = reactive({
    key: nextKey(), askId, isSend: false, content: '', reasoning: '', tools: '',
    reasoningActiveKey: [], toolsActiveKey: [], reasoningAutoCollapsed: false, toolsAutoCollapsed: false,
    process: '排队中…', sending: true, error: '', dbId: '', totalTokenCount: 0,
  });
  messages.push(userMsg, aiMsg);
  scrollToBottom();

  // fire-and-forget：不等待服务端处理完成，回答通过 chatmsg/chatdone 异步回推
  connection.invoke('sendMessage', {
    aiChatsId: String(chatId.value),
    askId,
    content: text,
    isOnlineSearch: onlineSearch.value,
    fileNames: '',
    contentFileUrls: '',
    retryOfId: String(retryOfId || ''),
  }).catch((err) => {
    aiMsg.sending = false;
    aiMsg.process = '';
    aiMsg.error = '发送失败：' + (err?.message || err);
    userMsg.sendStatus = 1;
    userMsg.failReason = err?.message || String(err);
  });
};

const doSend = () => {
  const text = draft.value.trim();
  if (!text) return;
  draft.value = '';
  sendOne({ content: text });
};

const onPressEnter = (e) => {
  if (e.shiftKey) return; // 换行
  e.preventDefault();
  doSend();
};

// 重试：以旧提问 askId 作为 retryOfId，服务端废弃旧失败问答并累加重试次数
const retryAsk = (userMsg) => {
  const oldAskId = userMsg.askId;
  const idx = messages.findIndex((m) => m.key === userMsg.key);
  // 移除旧提问 + 其回答占位
  if (idx !== -1) {
    const following = messages[idx + 1] && !messages[idx + 1].isSend && messages[idx + 1].askId === oldAskId;
    messages.splice(idx, following ? 2 : 1);
  }
  sendOne({ content: userMsg.content, retryOfId: oldAskId });
};

// ==== 进入房间 ====
const loadHistory = async () => {
  try {
    const res = await getAIChatHistorysPageData({ whereId: chatId.value, pageNum: 1, pageSize: 50 });
    if (res && res.code === 200 && res.data && Array.isArray(res.data.data)) {
      const list = res.data.data.map((item) => reactive({
        key: nextKey(),
        askId: item.id,
        dbId: item.id,
        isSend: item.isSend,
        content: item.content || '',
        reasoning: item.aiReasoningContent || '',
        tools: item.aiToolsContent || '',
        // 历史消息默认展开一次（与 MyAIChat 载入历史时置 expandedReasoning/expandedTools 的行为一致），超长由截断+弹窗兑现可读性
        reasoningActiveKey: item.aiReasoningContent ? ['reasoning'] : [],
        toolsActiveKey: item.aiToolsContent ? ['tools'] : [],
        reasoningAutoCollapsed: false,
        toolsAutoCollapsed: false,
        process: '',
        sending: false,
        error: '',
        sendStatus: item.sendStatus ?? 0,
        failReason: item.failReason || '',
        totalTokenCount: item.totalTokenCount || 0,
      }));
      messages.splice(0, messages.length, ...list.reverse());
      scrollToBottom();
    }
  } catch (e) {
    console.error('加载聊天记录失败', e);
  }
};

const enterRoom = async (id, name) => {
  chatId.value = String(id);
  roomName.value = name || '聊天室';
  await loadHistory();
  await openConnection();
};

const startNewRoom = async () => {
  if (!selectedAppId.value) return;
  entering.value = true;
  try {
    const res = await addAIChats({ appId: selectedAppId.value, name: '新对话', lastMessage: '' });
     console.log(res)
    if (res && res.code === 200 && res.data) {
      const newChatId = res.data.aiChatsId;
      if (!newChatId) throw new Error('未获取到会话 Id');
      await enterRoom(newChatId, '聊天室');
    } else {
      throw new Error(res?.msg || '创建聊天室失败');
    }
  } catch (e) {
    message.error('创建聊天室失败：' + (e.message || e));
  } finally {
    entering.value = false;
  }
};

onMounted(async () => {
  await loadAgents();
  const q = route.query;
  if (q.chatId) {
    await enterRoom(q.chatId, q.name || '聊天室');
  } else if (q.appId) {
    selectedAppId.value = q.appId;
    await startNewRoom();
  }
});

onBeforeUnmount(() => {
  if (connection) {
    try { connection.stop(); } catch (e) { /* ignore */ }
    connection = null;
  }
});
</script>

<style scoped>
.chatroom-page {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 130px);
  padding: 12px;
  box-sizing: border-box;
}
.entry {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}
.entry-card {
  width: 420px;
}
.entry-tip {
  margin: 12px 0;
  color: #888;
  font-size: 12px;
  line-height: 1.6;
}
.room-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 4px;
  border-bottom: 1px solid #eee;
}
.room-title {
  font-weight: 600;
  font-size: 16px;
}
.msg-list {
  flex: 1;
  overflow-y: auto;
  padding: 12px 4px;
}
.msg-row {
  display: flex;
  margin-bottom: 12px;
}
.row-user { justify-content: flex-end; }
.row-ai { justify-content: flex-start; }
.bubble {
  max-width: 72%;
  padding: 10px 12px;
  border-radius: 10px;
  word-break: break-word;
}
.bub-user {
  background: #1677ff;
  color: #fff;
}
.bub-ai {
  background: #f4f5f7;
  color: #222;
}
.pre-wrap { white-space: pre-wrap; }
.msg-process { color: #999; font-size: 12px; margin-bottom: 6px; }
/* 折叠面板：与 MyAIChat.css「折叠面板」一节逐条同值（MyAIChat 是 scoped 引入，对本页不生效，故在此复刻） */
.message-collapse {
  margin-top: 4px;
  font-size: 12px;
  position: relative;
  width: fit-content;
  max-width: 100%;
  z-index: 10;
}
.message-collapse :deep(.ant-collapse-header) {
  padding: 3px 10px !important;
  border-radius: 4px;
  font-size: 10px;
  color: rgba(0, 0, 0, 0.5) !important;
  background: rgba(0, 0, 0, 0.04) !important;
  border: none !important;
  display: flex !important;
  align-items: center !important;
}
.message-collapse :deep(.ant-collapse-content-box) {
  padding: 4px 10px !important;
  background: rgba(255, 255, 255, 0.08) !important;
  border: none !important;
  border-radius: 0 0 4px 4px !important;
}
.collapse-content {
  color: rgba(0, 0, 0, 0.55);
  font-size: 12px;
  line-height: 1.6;
  white-space: pre-wrap;
  word-break: break-all;
}
.collapse-content a {
  color: #1677ff;
}
/* 详情弹窗：与 MyAIChat.css .detail-modal-body 同值 */
.detail-modal-body {
  max-height: 750px;
  overflow-y: auto;
  white-space: pre-wrap;
  word-break: break-all;
}
.msg-content { line-height: 1.6; }
.typing { color: #1677ff; font-size: 12px; margin-top: 4px; }
.ai-error { color: #d4380d; margin-top: 4px; }
.msg-meta { color: #aaa; font-size: 11px; margin-top: 6px; }
.ask-fail { margin-top: 6px; }
.composer {
  border-top: 1px solid #eee;
  padding-top: 8px;
}
.composer-tools {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 6px;
}
.tool-label { font-size: 12px; color: #666; }
.composer-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 8px;
}
</style>
