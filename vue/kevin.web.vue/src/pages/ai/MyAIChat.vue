<template>
  <div class="ai-chat-container">
    <!-- 粒子背景 -->
    <div class="particles-bg">
      <div v-for="i in 30" :key="'p'+i" class="particle" :style="{
        left: (Math.random() * 100) + '%',
        top: (Math.random() * 100) + '%',
        animationDelay: (Math.random() * 5) + 's',
        animationDuration: (3 + Math.random() * 6) + 's',
        width: (2 + Math.random() * 3) + 'px',
        height: (2 + Math.random() * 3) + 'px',
      }"></div>
    </div>

    <div class="chat-layout">
      <!-- 左侧对话列表 -->
      <div class="chat-sidebar">
        <div class="sidebar-header">
          <div class="sidebar-title-group">
            <div class="sidebar-icon">
              <span class="icon-pulse"></span>
              <MessageOutlined />
            </div>
            <h3>对话中心</h3>
          </div>
          <a-button
            type="primary"
            @click="() => showAgentSelectionModal()"
            size="small"
            class="add-button"
            :disabled="isSending"
          >
            <template #icon>
              <PlusOutlined />
            </template>
            新建对话
          </a-button>
        </div>
        <div class="conversation-list">
          <a-list
            :data-source="conversations"
            :loading="loadingConversations"
            :pagination="false"
          >
            <template #renderItem="{ item }">
              <a-list-item
                class="conversation-item"
                :class="{ active: item.id === activeConversationId, disabled: isSending }"
                @click="!isSending && selectConversation(item)"
              >
                <div class="conversation-content">
                  <div class="conversation-title">{{ item.title || "新对话" }}</div>
                  <div class="conversation-preview">
                    {{ item.lastMessage || "开始新的对话..." }}
                  </div>
                  <div class="conversation-time">
                    {{ formatDate(item.updatedAt) }}
                  </div>
                </div>
                <div class="conversation-actions" @click.stop>
                  <a-button
                    type="text"
                    class="delete-btn"
                    @click="(event) => deleteConversation(item.id, event)"
                  >
                    <template #icon>
                      <DeleteOutlined />
                    </template>
                  </a-button>
                </div>
              </a-list-item>
            </template>
            <template #renderEmpty>
              <div class="empty-conversations">
                <a-empty description="暂无对话记录" />
              </div>
            </template>
          </a-list>
        </div>
      </div>

      <!-- 右侧聊天区域 -->
      <div class="chat-main">
        <div class="chat-header" v-if="activeConversation">
          <div class="header-left">
            <div> 
              <div class="agent-info" v-if="activeConversation.appId">
                <div class="ai-robot-mini">
                  <div class="robot-head-mini">
                    <div class="robot-eye-left-mini"></div>
                    <div class="robot-eye-right-mini"></div>
                    <div class="robot-mouth-mini"></div>
                  </div>
                  <div class="robot-antenna-mini">
                    <div class="antenna-dot-mini"></div>
                  </div>
                </div>
                <span>{{ getAiAppName(activeConversation.appId) }}</span>
              </div>
            </div>
          </div>
          <div class="header-status">
            <span class="status-dot"></span>
            <span class="status-text">在线</span>
          </div>
        </div>

        <div class="chat-messages" ref="messagesContainer" v-if="activeConversation">
          <div
            v-for="message in messages"
            :key="message.id"
            class="message-item"
            :class="{
              'user-message': message.isSend === true,
              'ai-message': message.isSend === false,
            }"
          >
            <div class="message-avatar" :class="{ 'avatar-ai': message.isSend === false, 'avatar-user': message.isSend === true }">
              <UserOutlined v-if="message.isSend === true" />
              <div v-else class="ai-robot-mini">
                <div class="robot-head-mini">
                  <div class="robot-eye-left-mini"></div>
                  <div class="robot-eye-right-mini"></div>
                  <div class="robot-mouth-mini"></div>
                </div>
                <div class="robot-antenna-mini">
                  <div class="antenna-dot-mini"></div>
                </div>
              </div>
            </div>
            <div class="message-content">
              <!-- 非语音模式：文字正常显示 -->
              <div class="message-text" v-if="message.isSend === false && !isVoiceMode" v-html="message.content"></div>
              <div class="message-text" v-else-if="message.isSend === true" v-html="message.content"></div>
              <!-- AI 语音条 -->
              <div
                v-if="isVoiceMode && message.isSend === false && message.content"
                class="voice-msg-bar"
              >
                <div class="voice-msg-left" @click.stop="playAIVoice(message)">
                  <SoundOutlined class="voice-msg-icon" />
                  <div :class="['voice-msg-wave', { playing: isSpeaking && currentSpeakingMsgId === message.id }]">
                    <span v-for="i in 8" :key="i" class="voice-wave-bar" :style="{ animationDelay: (i * 0.1) + 's' }"></span>
                  </div>
                  <span v-if="isSpeaking && currentSpeakingMsgId === message.id" class="voice-msg-duration">播放中...</span>
                </div>
              </div>
              <!-- 语音模式 + 转文字：文字显示在语音条下方 -->
              <div v-if="isVoiceMode && showTextInVoiceMode && message.isSend === false && message.content" class="message-text voice-expanded-text" v-html="message.content"></div>
              <a-collapse v-if="message.aiReasoningContent" class="message-collapse" ghost :default-active-key="expandedReasoning ? ['reasoning'] : []">
                <a-collapse-panel key="reasoning" header="思考过程">
                  <div class="collapse-content">
                    <div v-if="message.aiReasoningContent.length > 350">{{ truncateContent(message.aiReasoningContent) }}<a @click="showDetailModal('思考过程详情', message.aiReasoningContent)">点击查看详情</a></div>
                    <div v-else>{{ message.aiReasoningContent }}</div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <a-collapse v-if="message.aiToolsContent" class="message-collapse" ghost :default-active-key="expandedTools ? ['tools'] : []">
                <a-collapse-panel key="tools" header="工具调用">
                  <div class="collapse-content">
                    <div v-if="message.aiToolsContent.length > 350">{{ truncateContent(message.aiToolsContent) }}<a @click="showDetailModal('工具调用详情', message.aiToolsContent)">点击查看详情</a></div>
                    <div v-else>{{ message.aiToolsContent }}</div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <a-collapse v-if="message.fileNames && message.contentFileUrls" class="message-collapse" ghost>
                <a-collapse-panel key="files" header="附件">
                  <div class="collapse-content file-list-content">
                    <div v-for="(fileName, index) in message.fileNames.split(',')" :key="index" class="file-item">
                      <FileTextOutlined />
                      <a :href="message.contentFileUrls.split(',')[index]" target="_blank" class="file-link">
                        {{ fileName }}
                      </a>
                    </div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <a-collapse v-if="message.aIChatHistorysBindLogs && message.aIChatHistorysBindLogs.length > 0" class="message-collapse" ghost>
                <a-collapse-panel key="logs" header="AI相关日志">
                  <div class="collapse-content">
                    <div v-for="(log, index) in message.aIChatHistorysBindLogs" :key="index" class="log-item">
                      <span class="log-type-tag">{{ getLogTypeName(log.logType) }}</span>
                      <template v-if="log.logContent && log.logContent.length > 200">
                        {{ truncateContent(log.logContent) }}<a @click="showDetailModal('AI相关日志详情', log.logContent)">点击查看详情</a>
                      </template>
                      <template v-else>{{ log.logContent }}</template>
                    </div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <div class="message-actions">
                <a-button
                  type="text"
                  size="small"
                  @click="copyMessageContent(message.content)"
                  class="copy-button">
                  <template #icon>
                    <CopyOutlined />
                  </template>
                </a-button>
              </div>
              <div class="message-time">
                {{ formatTime(message.createdAt) }}
                <span v-if="message.totalTokenCount" class="token-count">消耗: {{ formatTokenCount(message.totalTokenCount) }} tokens</span>
              </div>
            </div>
          </div>

          <div v-if="isSending" class="message-item ai-message">
            <div class="message-avatar avatar-ai">
              <div class="ai-robot-mini thinking">
                <div class="robot-head-mini">
                  <div class="robot-eye-left-mini"></div>
                  <div class="robot-eye-right-mini"></div>
                  <div class="robot-mouth-mini"></div>
                </div>
                <div class="robot-antenna-mini">
                  <div class="antenna-dot-mini"></div>
                </div>
              </div>
            </div>
            <div class="message-content">
              <div class="typing-indicator">
                <span></span>
                <span></span>
                <span></span>
              </div>
             <div v-if="isSending && (!isVoiceMode || showTextInVoiceMode)" class="message-text message-text-stream">{{ aimessage2 }}</div>
              <!-- 流式播放中的语音条 -->
              <div v-if="isVoiceMode && isSpeaking && streamingTtsActive" class="voice-msg-bar voice-msg-bar-streaming">
                <div class="voice-msg-left">
                  <SoundOutlined class="voice-msg-icon" />
                  <div class="voice-msg-wave playing">
                    <span v-for="i in 8" :key="i" class="voice-wave-bar" :style="{ animationDelay: (i * 0.1) + 's' }"></span>
                  </div>
                  <span class="voice-msg-duration">播放中...</span>
                </div>
              </div>
              <div class="message-time stream-status">{{ aimessage}}</div>
                <a-collapse v-model:active-key="reasoningActiveKey" class="message-collapse" ghost v-if="aIReasoningContentMsg">
                <a-collapse-panel key="reasoning" header="思考过程">
                  <div class="collapse-content">
                    <div v-if="aIReasoningContentMsg.length > 350">{{ truncateContent(aIReasoningContentMsg) }}<a @click="showDetailModal('思考过程详情', aIReasoningContentMsg)">点击查看详情</a></div>
                    <div v-else>{{ aIReasoningContentMsg }}</div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
              <a-collapse v-model:active-key="toolsActiveKey" class="message-collapse" ghost v-if="aIToolsContentMsg">
                <a-collapse-panel key="tools" header="工具调用">
                  <div class="collapse-content">
                    <div v-if="aIToolsContentMsg.length > 350">{{ truncateContent(aIToolsContentMsg) }}<a @click="showDetailModal('工具调用详情', aIToolsContentMsg)">点击查看详情</a></div>
                    <div v-else>{{ aIToolsContentMsg }}</div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
            </div>
          </div>
        </div>

        <div class="chat-input-area" v-if="activeConversation">
          <div class="input-group">
            <!-- 文字输入框（仅语音模式关闭时显示） -->
            <a-textarea
              v-if="!isVoiceMode"
              v-model:value="newMessage"
              :placeholder="'输入消息...'"
              class="message-input"
              @pressEnter="handlePressEnter"
              :disabled="isSending"
              :auto-size="{ minRows: 3, maxRows: 6 }"
              allow-clear
            />
            <!-- 语音模式: 录音预览 + 发送提示 -->
            <div v-if="isVoiceMode" class="voice-input-area">
              <!-- 录音中显示识别文字预览 -->
              <div v-if="isRecording || isRecognizing" class="voice-preview">
                <span class="voice-preview-label">{{ isRecording ? '录音中...' : '正在识别...' }}</span>
                <span class="voice-preview-text">{{ recognizedPreviewText || '...' }}</span>
              </div>
              <!-- 发送后提示 -->
              <div v-else-if="voiceSentHint" class="voice-sent-hint">
                <CheckCircleFilled v-if="voiceSentHint.includes('已发送')" class="voice-sent-icon success" />
                <InfoCircleFilled v-else class="voice-sent-icon warning" />
                <span>{{ voiceSentHint }}</span>
              </div>
              <div v-else class="voice-hint-text">按住下方按钮开始说话，说完松开自动发送</div>
            </div>
            <!-- 语音录制按钮（按住说话，松开自动发送） -->
            <div v-if="isVoiceMode" class="voice-recorder-area">
              <div class="voice-recorder-wrapper">
                <div
                  :class="['voice-record-btn', { recording: isRecording, recognizing: isRecognizing }]"
                  :style="{ pointerEvents: isSending ? 'none' : 'auto', opacity: isSending ? 0.5 : 1 }"
                  @mousedown="startRecording"
                  @mouseup="stopRecording"
                  @mouseleave="cancelRecording"
                  @touchstart.prevent="startRecording"
                  @touchend.prevent="stopRecording"
                  @touchcancel.prevent="cancelRecording"
                >
                  <AudioOutlined class="voice-record-icon" />
                  <span class="voice-record-text">{{ isRecording ? '松开 结束' : '按住 说话' }}</span>
                </div>
              </div>
              <div v-if="isRecording || isRecognizing" class="recording-indicator">
                <span class="recording-dot" :class="{ recognizing: isRecognizing }"></span>
                <span class="recording-text">{{ isRecording ? '正在录音...' : '正在识别...' }}</span>
              </div>
            </div>
            <div class="input-options">
              <div class="input-options-left">
                <FileUpload
                  ref="fileUploadRef"
                  business="AIChat"
                  :key-value="activeConversationId || ''"
                  sign="chat"
                  :multiple="true"
                  :disabled="isSending"
                  :accept="'.txt,.pdf,.md,.docx,.html,.doc,.xls,.xlsx,.jpg,.jpeg,.png,.gif,.bmp,.webp,.svg'"
                  :show-upload-list="false"
                  upload-button-text="上传文件"
                  @upload-success="handleFileUploadSuccess"
                  @upload-error="handleFileUploadError"
                />
                <a-switch v-model:checked="isOnlineSearch" class="online-search-switch">
                  <template #checkedChildren>联网搜索</template>
                  <template #unCheckedChildren>联网搜索</template>
                </a-switch>
                <a-switch v-model:checked="isVoiceMode" class="voice-mode-switch">
                  <template #checkedChildren>语音模式</template>
                  <template #unCheckedChildren>语音模式</template>
                </a-switch>
                <!-- 倍速选择器（仅语音模式开启时显示） -->
                <div v-if="isVoiceMode" class="voice-speed-selector global-speed-selector">
                  <span
                    v-for="s in speedOptions"
                    :key="s.value"
                    :class="['speed-option', { active: voiceSpeed === s.value }]"
                    @click="setVoiceSpeed(s.value)"
                  >{{ s.label }}</span>
                </div>
                <!-- 转文本开关（语音模式下是否显示文字） -->
                <a-switch v-if="isVoiceMode" v-model:checked="showTextInVoiceMode" class="text-display-switch">
                  <template #checkedChildren>转文字</template>
                  <template #unCheckedChildren>转文字</template>
                </a-switch>
              </div>
              <a-button
                type="primary"
                @click="isSending ? stopMessage() : sendMessage()"
                :disabled="!newMessage.trim() && !isSending && uploadedFileList.length === 0"
                :class="['send-button', { stopping: isSending }]"
              >
                <template #icon>
                  <SendOutlined v-if="!isSending" />
                  <StopOutlined v-else />
                </template>
              </a-button>
            </div>
            <div v-if="uploadedFileList.length > 0" class="uploaded-files-bar">
              <a-tag
                v-for="(file, index) in uploadedFileList"
                :key="index"
                closable
                @close="removeUploadedFile(index)"
              >
                <a :href="file.url" target="_blank" download="{{ file.name }}" class="file-download-link">
                  <FileTextOutlined />
                  {{ file.name }}
                </a>
              </a-tag>
            </div>
          </div>
        </div>

        <div class="chat-placeholder" v-else>
          <!-- AI 机器人动画 -->
          <div class="ai-robot">
            <div class="robot-body">
              <!-- 天线 -->
              <div class="robot-antenna">
                <div class="antenna-line"></div>
                <div class="antenna-dot">
                  <div class="antenna-glow"></div>
                </div>
              </div>
              <!-- 头部 -->
              <div class="robot-head">
                <div class="head-top-bar"></div>
                <div class="robot-face">
                  <div class="robot-eye left-eye">
                    <div class="eye-glow"></div>
                  </div>
                  <div class="robot-eye right-eye">
                    <div class="eye-glow"></div>
                  </div>
                  <div class="robot-mouth">
                    <div class="mouth-wave"></div>
                  </div>
                </div>
                <div class="head-ear left-ear"></div>
                <div class="head-ear right-ear"></div>
              </div>
              <!-- 脖子 -->
              <div class="robot-neck">
                <div class="neck-ring"></div>
              </div>
              <!-- 身体 -->
              <div class="robot-torso">
                <div class="torso-core">
                  <div class="core-ring core-ring-1"></div>
                  <div class="core-ring core-ring-2"></div>
                  <div class="core-center"></div>
                </div>
                <div class="torso-chest-line"></div>
                <div class="torso-chest-line line-2"></div>
              </div>
              <!-- 手臂 -->
              <div class="robot-arm left-arm">
                <div class="arm-upper"></div>
                <div class="arm-lower"></div>
                <div class="arm-hand"></div>
              </div>
              <div class="robot-arm right-arm">
                <div class="arm-upper"></div>
                <div class="arm-lower"></div>
                <div class="arm-hand"></div>
              </div>
              <!-- 底座/悬浮环 -->
              <div class="robot-base">
                <div class="base-ring"></div>
                <div class="base-ring base-ring-2"></div>
              </div>
            </div>
            <!-- 机器人底部的光效 -->
            <div class="robot-hover-glow"></div>
          </div>
          <div class="placeholder-content">
            <p class="placeholder-title">你好，我是 AI 助手</p>
            <p class="placeholder-sub">选择一个对话或创建新对话，开始我们的智能之旅</p>
            <a-button type="primary" @click="() => showAgentSelectionModal()" class="add-button-large" :disabled="isSending">
              <template #icon>
                <PlusOutlined />
              </template>
              新建对话
            </a-button>
          </div>
        </div>

        <a-modal v-model:open="detailModalVisible" :title="detailModalTitle" :footer="null" width="1000px">
          <div style="max-height: 750px; overflow-y: auto; white-space: pre-wrap; word-break: break-all;">
            {{ detailModalContent }}
          </div>
        </a-modal>
      </div>
    </div>

    </div>
</template>

<script setup>
/* eslint-disable */
import { ref, onMounted, nextTick, watch, h, onUnmounted } from "vue";
import {
  PlusOutlined,
  UserOutlined,
  RobotOutlined,
  MessageOutlined,
  DeleteOutlined,
  CopyOutlined,
  LoadingOutlined,
  StopOutlined,
  FileTextOutlined,
  LinkOutlined,
  UploadOutlined,
  SendOutlined,
  AudioOutlined,
  SoundOutlined,
  AudioMutedOutlined,
  CheckCircleFilled,
  InfoCircleFilled,
} from "@ant-design/icons-vue";
import FileUpload from "../../components/FileUpload.vue";
import { message, Modal, Select } from "ant-design-vue";
import { getMyAIAppsALLList } from "../../api/ai/aiapps.js";
import { getAIChatsMyPageData, addAIChats, deleteAIChats } from "../../api/ai/aichats.js";
import {
  getAIChatHistorysPageData,
  addAIChatHistorys,
} from "../../api/ai/aichathistorys.js";
import * as signalR from '@microsoft/signalr';
import { GetSnowflakeId } from '../../api/baseapi';
// 模拟数据
const conversations = ref([]);
const activeConversationId = ref(null);
const activeConversation = ref(null);
const messages = ref([]);
const newMessage = ref("");
const loadingConversations = ref(false);
const isSending = ref(false);
const messagesContainer = ref(null);
const aimessage=ref("");
const aimessage2=ref("");
const aIToolsContentMsg=ref("");
const aIReasoningContentMsg=ref("");
const reasoningActiveKey = ref([]);
const toolsActiveKey = ref([]);
const currentReceivingMsgId = ref(null);
const expandedReasoning = ref(false);
const expandedTools = ref(false);
const lastSentMessage = ref("");
const lastSentMessageId = ref(null);
let abortController = null;

// 文件上传相关
const uploadedFileList = ref([]);
const pendingFileNames = ref([]);
const pendingFileUrls = ref([]);
const fileBackups = ref([]);
const fileUploadRef = ref(null);

// 自动收起标志位
const reasoningAutoCollapsed = ref(false);
const toolsAutoCollapsed = ref(false);

// 监听思考过程内容长度
watch(aIReasoningContentMsg, (newVal, oldVal) => {
  if (!newVal) return;
  // 内容首次出现且不超过300字，自动展开
  if (!oldVal && newVal.length <= 300) {
    reasoningActiveKey.value = ['reasoning'];
    reasoningAutoCollapsed.value=false;
  }
  // 内容超过300字且之前没超过，自动收起一次
  if (newVal.length > 300 && oldVal && oldVal.length <= 300 && !reasoningAutoCollapsed.value) {
    reasoningActiveKey.value = [];
    reasoningAutoCollapsed.value = true;
  }
});

// 监听工具调用内容长度
watch(aIToolsContentMsg, (newVal, oldVal) => {
  if (!newVal) return;
  // 内容首次出现且不超过350字，自动展开
  if (!oldVal && newVal.length <= 300) {
    toolsActiveKey.value = ['tools'];
    toolsAutoCollapsed.value=false;
  }
  // 内容超过300字且之前没超过，自动收起一次
  if (newVal.length > 300 && oldVal && oldVal.length <= 300 && !toolsAutoCollapsed.value) {
    toolsActiveKey.value = [];
    toolsAutoCollapsed.value = true;
  }
});

const detailModalVisible = ref(false);
const detailModalContent = ref("");
const detailModalTitle = ref("");
// 添加联网搜索开关变量
const isOnlineSearch = ref(false); // 默认为关闭状态

// 语音模式相关变量
const isVoiceMode = ref(false); // 语音模式开关
const showTextInVoiceMode = ref(false); // 语音模式下是否显示文字（转文本模式）
const isRecording = ref(false); // 是否正在录音
const isRecognizing = ref(false); // 是否正在识别（延迟停止中）
const micPermissionGranted = ref(false); // 麦克风权限是否已获取
const recognizedPreviewText = ref(''); // 语音识别预览文字
const voiceSentHint = ref(''); // 语音发送后提示
let recordingStartTime = 0; // 录音开始时间戳
let pendingStopTimer = null; // 延迟停止识别的定时器
const recognition = ref(null); // 语音识别实例
const isSpeaking = ref(false); // 是否正在播放语音
const currentSpeakingMsgId = ref(null); // 当前正在播放的消息ID
const voiceSpeed = ref(1.0); // 语音播放倍速
const speedOptions = [
  { label: '1x', value: 1.0 },
  { label: '1.5x', value: 1.5 },
  { label: '2x', value: 2.0 },
  { label: '2.5x', value: 2.5 }, 
];

// 设置播放倍速
const setVoiceSpeed = (speed) => {
  voiceSpeed.value = speed;
};

// ========== 语音功能 ==========

// 初始化语音识别（每次创建新实例，避免复用导致无法再次 start）
const initSpeechRecognition = () => {
  const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
  if (!SpeechRecognition) {
    message.warning('您的浏览器不支持语音识别，请使用 Chrome 浏览器');
    return null;
  }
  // 安全上下文检查（SpeechRecognition 需要 HTTPS 或 localhost）
  if (!window.isSecureContext) {
    message.error('语音识别需要 HTTPS 环境或 localhost 访问');
    return null;
  }
  const rec = new SpeechRecognition();
  rec.lang = 'zh-CN'; // 普通话（简体）
  rec.interimResults = true;
  rec.continuous = true;
  rec.maxAlternatives = 1;
  return rec;
};

// 检查/申请麦克风权限
const ensureMicrophonePermission = async () => {
  // 优先用 Permissions API 查询当前状态
  if (navigator.permissions) {
    try {
      const status = await navigator.permissions.query({ name: 'microphone' });
      if (status.state === 'granted') {
        micPermissionGranted.value = true;
        return true;
      }
      if (status.state === 'denied') {
        micPermissionGranted.value = false;
        // 权限已被拒绝，浏览器不会再弹窗，需要引导用户手动开启
        Modal.warning({
          title: '麦克风权限被拒绝',
          content: '浏览器已记住您之前的拒绝选择，需要手动重新开启。请点击浏览器地址栏左侧的锁形图标（或设置图标），找到"麦克风"权限并改为"允许"，然后刷新页面重试。',
          okText: '我知道了',
        });
        return false;
      }
      // state === 'prompt': 可以通过 getUserMedia 触发权限弹窗
    } catch (e) {
      // Permissions API 不支持 microphone，降级到 getUserMedia
    }
  }
  // 通过 getUserMedia 触发权限申请弹窗
  try {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    // 申请成功，立即释放资源（只是用来触发权限）
    stream.getTracks().forEach((t) => t.stop());
    micPermissionGranted.value = true;
    return true;
  } catch (err) {
    micPermissionGranted.value = false;
    if (err.name === 'NotAllowedError' || err.name === 'PermissionDeniedError') {
      Modal.warning({
        title: '麦克风权限被拒绝',
        content: '您拒绝了麦克风权限。如需重新开启，请点击浏览器地址栏左侧的锁形图标，找到"麦克风"权限并改为"允许"，然后刷新页面重试。',
        okText: '我知道了',
      });
    } else if (err.name === 'NotFoundError' || err.name === 'DevicesNotFoundError') {
      message.error('未检测到麦克风设备');
    } else {
      message.error('麦克风访问失败: ' + err.message);
    }
    return false;
  }
};

// 开始录音
const startRecording = (e) => {
  if (isSending.value) return;

  // 检查麦克风权限是否已获取
  if (!micPermissionGranted.value) {
    message.warning('正在申请麦克风权限，请稍后再试');
    // 异步触发权限申请
    ensureMicrophonePermission();
    return;
  }

  // 在用户手势中解锁语音合成（为后续AI回复自动播放做准备）
  unlockSpeechSynthesis();

  // 开始录音时，停止正在播放的AI语音
  if (isSpeaking.value) {
    staticPlaybackGen++; // 使旧回调失效
    streamingTtsActive = false;
    staticPlaybackMsgId = null;
    staticSentenceList = [];
    staticSentenceIndex = 0;
    window.speechSynthesis.cancel();
    resetSpeakingState();
  }

  // 每次创建新的识别实例（stop/abort 后旧实例无法复用）
  recognition.value = initSpeechRecognition();
  if (!recognition.value) return;

  isRecording.value = true;
  isRecognizing.value = false;
  recognizedPreviewText.value = '';
  newMessage.value = '';
  recordingStartTime = Date.now();

  // 累积所有已完成的识别结果
  let accumulatedText = '';

  recognition.value.onresult = (event) => {
    // 遍历所有结果（包括已完成 + 临时）
    let interimText = '';
    for (let i = 0; i < event.results.length; i++) {
      const result = event.results[i];
      if (result.isFinal) {
        // 最终结果：直接累加到完整文本
        accumulatedText += result[0].transcript;
      } else {
        // 临时结果：取最后一个临时结果作为预览
        interimText = result[0].transcript;
      }
    }
    // 最终显示 = 已完成文本 + 当前临时文本
    recognizedPreviewText.value = accumulatedText + interimText;
  };

  recognition.value.onerror = (event) => {
    console.error('语音识别错误:', event.error);
    if (event.error === 'aborted' || event.error === 'no-speech') {
      return;
    }
    if (event.error === 'not-allowed' || event.error === 'service-not-allowed') {
      Modal.warning({
        title: '麦克风权限被拒绝',
        content: '请点击浏览器地址栏左侧的锁形图标，找到"麦克风"权限并改为"允许"，然后刷新页面重试。',
        okText: '我知道了',
      });
    } else {
      message.error('语音识别失败: ' + event.error);
    }
    isRecording.value = false;
    isRecognizing.value = false;
  };

  recognition.value.onend = () => {
    // 如果不是我们主动 stop 的，才重置 isRecording
    if (isRecording.value) {
      isRecording.value = false;
    }
  };

  try {
    recognition.value.start();
    console.log('语音识别已启动');
  } catch (err) {
    console.error('启动语音识别失败:', err);
    isRecording.value = false;
    isRecognizing.value = false;
  }
};

// 停止录音并发送
const stopRecording = () => {
  if (!isRecording.value) return;
  
  // 检查录音时长（最短 300ms，防止误触）
  const duration = Date.now() - recordingStartTime;
  if (duration < 300) {
    // 录音时间太短，直接取消
    isRecording.value = false;
    if (recognition.value) {
      try { recognition.value.abort(); } catch (e) { /* ignore */ }
    }
    recognizedPreviewText.value = '';
    voiceSentHint.value = '说话时间太短';
    setTimeout(() => { voiceSentHint.value = ''; }, 1500);
    return;
  }
  
  // 正常流程：进入识别状态，延迟 500ms 让识别引擎处理最后的语音
  isRecording.value = false;
  isRecognizing.value = true;
  
  pendingStopTimer = setTimeout(() => {
    isRecognizing.value = false;
    pendingStopTimer = null;
    
    // 停止识别引擎
    if (recognition.value) {
      try { recognition.value.stop(); } catch (e) { /* ignore */ }
    }
    
    // 等 onend 事件处理完最后一批结果后，发送消息
    // 再等一个小延迟确保最后的 final result 已到达
    setTimeout(() => {
      if (recognizedPreviewText.value.trim()) {
        newMessage.value = recognizedPreviewText.value;
        recognizedPreviewText.value = '';
        voiceSentHint.value = '已发送 ✓';
        setTimeout(() => { voiceSentHint.value = ''; }, 1500);
        sendMessage();
      } else {
        recognizedPreviewText.value = '';
        voiceSentHint.value = '未识别到内容，已取消';
        setTimeout(() => { voiceSentHint.value = ''; }, 1500);
      }
    }, 300);
  }, 500);
};

// 取消录音
const cancelRecording = () => {
  if (!isRecording.value && !isRecognizing.value) return;
  isRecording.value = false;
  isRecognizing.value = false;
  // 取消待执行的停止定时器
  if (pendingStopTimer) {
    clearTimeout(pendingStopTimer);
    pendingStopTimer = null;
  }
  newMessage.value = '';
  recognizedPreviewText.value = '';
  voiceSentHint.value = '已取消';
  setTimeout(() => { voiceSentHint.value = ''; }, 1000);
  if (recognition.value) {
    try {
      recognition.value.abort();
    } catch (e) {
      // ignore
    }
  }
};

// 播放AI语音（TTS）
// 清理文本用于语音播放：去除表情、特殊符号、标点、Markdown语法等
const sanitizeTextForTTS = (text) => {
  if (!text) return '';
  let t = text;
  // 去除表情符号（emoji）
  t = t.replace(/[\u{1F300}-\u{1F9FF}\u{2600}-\u{26FF}\u{2700}-\u{27BF}\u{1F600}-\u{1F64F}\u{1F680}-\u{1F6FF}\u{1F1E0}-\u{1F1FF}\u{1F900}-\u{1F9FF}\u{1FA00}-\u{1FA6F}\u{1FA70}-\u{1FAFF}\u{200D}\u{FE00}-\u{FE0F}]/gu, ' ');
  // 去除Markdown代码块 ```...```
  t = t.replace(/```[\s\S]*?```/g, ' ');
  // 去除行内代码 `...`
  t = t.replace(/`[^`]*`/g, ' ');
  // 去除Markdown链接 [text](url) → text
  t = t.replace(/\[([^\]]+)\]\([^)]*\)/g, '$1');
  // 去除Markdown图片 ![alt](url)
  t = t.replace(/!\[[^\]]*\]\([^)]*\)/g, ' ');
  // 去除Markdown加粗/斜体标记 **text** *text*
  t = t.replace(/\*\*([^*]+)\*\*/g, '$1');
  t = t.replace(/\*([^*]+)\*/g, '$1');
  t = t.replace(/__([^_]+)__/g, '$1');
  t = t.replace(/_([^_]+)_/g, '$1');
  // 去除Markdown标题 # 
  t = t.replace(/^#{1,6}\s+/gm, '');
  // 去除URL
  t = t.replace(/https?:\/\/[^\s]+/g, ' ');
  // 去除中文标点符号
  t = t.replace(/[，。！？、；：""''（）【】《》〈〉…—·！]/g, ' ');
  // 去除英文标点符号
  t = t.replace(/[,.!?;:"'()\[\]{}<>]/g, ' ');
  // 去除多余空白
  t = t.replace(/\s+/g, ' ').trim();
  return t;
};

// 获取最自然的中文语音（普通话，排除粤语/台湾音）
let cachedBestVoice = null;
let speechUnlocked = false; // 语音合成是否已解锁（浏览器自动播放策略）

// 在用户手势中解锁语音合成（解决首次自动播放被阻止的问题）
const unlockSpeechSynthesis = () => {
  if (speechUnlocked) return;
  try {
    const unlocker = new SpeechSynthesisUtterance('');
    unlocker.volume = 0;
    unlocker.lang = 'cmn-Hans-CN';
    window.speechSynthesis.speak(unlocker);
    window.speechSynthesis.cancel();
    speechUnlocked = true;
    console.log('语音合成已解锁');
  } catch (e) {
    // ignore
  }
};
const getBestChineseVoice = () => {
  if (cachedBestVoice) return cachedBestVoice;
  const voices = window.speechSynthesis.getVoices();
  // 只选择普通话语音：zh-CN, cmn-Hans-CN, zh-MO（排除 zh-HK, zh-TW, yue-* 等粤语/台湾音）
  const zhVoices = voices.filter((v) => {
    if (!v.lang) return false;
    const lang = v.lang.toLowerCase();
    // 排除粤语和台湾
    if (lang.includes('hk') || lang.includes('tw') || lang.includes('yue') || lang.includes('hant')) return false;
    // 选择普通话
    return lang.startsWith('zh') || lang.startsWith('cmn');
  });

  if (zhVoices.length === 0) {
    cachedBestVoice = voices.find((v) => v.lang && (v.lang.startsWith('zh') || v.lang.startsWith('cmn'))) || null;
    return cachedBestVoice;
  }

  // 排除 Preview/Experimental 等实验性语音
  const stableVoices = zhVoices.filter(
    (v) => !v.name.toLowerCase().includes('preview') && !v.name.toLowerCase().includes('experimental')
  );

  // 优先选择本地服务的语音（响应更快）
  const localVoices = stableVoices.filter((v) => v.localService);

  // 普通话女声优先级关键词
  const preferredKeywords = [
    'yaoyao', 'yaoyao-neural', 'huihui', 'xiaoxiao', 'xiaoyi',
    'xiaomeng', 'xiaorou', 'yunxi', 'yunjian', 'yunyang',
    'yunkang', 'yunxia', 'tingting'
  ];

  // 优先匹配含关键词的本地语音
  let best = localVoices.find((v) =>
    preferredKeywords.some((k) => v.name.toLowerCase().includes(k))
  );

  // 退而求其次：stable 语音中匹配关键词
  if (!best) {
    best = stableVoices.find((v) =>
      preferredKeywords.some((k) => v.name.toLowerCase().includes(k))
    );
  }

  // 再退而求其次：第一个非 Default 非 Preview 的
  if (!best && stableVoices.length > 0) {
    best = stableVoices.find((v) => !v.name.toLowerCase().includes('default')) || stableVoices[0];
  }

  // 最后兜底
  if (!best) best = zhVoices[0];

  cachedBestVoice = best;
  return best;
};

// 初始化时加载语音列表（某些浏览器需要异步加载）
if ('speechSynthesis' in window) {
  window.speechSynthesis.onvoiceschanged = () => {
    cachedBestVoice = null;
    getBestChineseVoice();
  };
}

// ========== 非流式 TTS：逐句播放（支持实时倍速调整）==========

let staticSentenceList = []; // 句子列表
let staticSentenceIndex = 0; // 下一句的索引
let staticPlaybackMsgId = null; // 当前播放的消息ID
let staticPlaybackGen = 0; // 播换代次（递增使旧回调失效，防止竞争）

// 将文本分割成句子
const splitTextIntoSentences = (text) => {
  if (!text || !text.trim()) return [];
  const sentences = [];
  let start = 0;
  for (let i = 0; i < text.length; i++) {
    if (/[。！？.!?\n]/.test(text[i])) {
      const s = text.substring(start, i + 1).trim();
      if (s) sentences.push(s);
      start = i + 1;
    }
  }
  if (start < text.length) {
    const r = text.substring(start).trim();
    if (r) sentences.push(r);
  }
  return sentences.length > 0 ? sentences : [text.trim()];
};

// 播放下一句（非流式，每句读取最新倍速）
const playNextStaticSentence = (gen) => {
  if (gen !== staticPlaybackGen) return; // 被新播放取代
  if (!staticPlaybackMsgId || staticSentenceIndex >= staticSentenceList.length) {
    resetSpeakingState();
    staticPlaybackMsgId = null;
    staticSentenceList = [];
    staticSentenceIndex = 0;
    return;
  }

  const sentence = staticSentenceList[staticSentenceIndex];
  staticSentenceIndex++;

  const utterance = new SpeechSynthesisUtterance(sentence);
  utterance.lang = 'zh-CN';
  const bestVoice = getBestChineseVoice();
  if (bestVoice) utterance.voice = bestVoice;
  utterance.pitch = 1.0;
  utterance.rate = voiceSpeed.value; // 每句读取最新倍速
  utterance.volume = 1.0;

  utterance.onstart = () => {
    if (gen !== staticPlaybackGen) return;
    isSpeaking.value = true;
  };
  utterance.onend = () => {
    if (gen !== staticPlaybackGen) return;
    playNextStaticSentence(gen);
  };
  utterance.onerror = () => {
    if (gen !== staticPlaybackGen) return;
    playNextStaticSentence(gen);
  };

  window.speechSynthesis.speak(utterance);
};

const playAIVoice = (message) => {
  if (!message || !message.content) return;

  // 如果正在播放同一条消息，则停止
  if (isSpeaking.value && currentSpeakingMsgId.value === message.id) {
    staticPlaybackGen++; // 递增代次，使旧回调失效
    window.speechSynthesis.cancel();
    streamingTtsActive = false;
    resetSpeakingState();
    staticPlaybackMsgId = null;
    staticSentenceList = [];
    staticSentenceIndex = 0;
    return;
  }

  // 停止之前的播放（包括流式播放）
  staticPlaybackGen++; // 递增代次，使旧回调失效
  window.speechSynthesis.cancel();
  streamingTtsActive = false;
  resetSpeakingState();

  // 去除HTML标签获取纯文本
  const tempDiv = document.createElement('div');
  tempDiv.innerHTML = message.content;
  const plainText = (tempDiv.textContent || tempDiv.innerText || '').trim();
  if (!plainText) return;

  // 清理文本：去除表情、标点、特殊符号等，只保留纯文字
  const cleanText = sanitizeTextForTTS(plainText);
  if (!cleanText) return;

  // 分割成句子，逐句播放（支持实时倍速调整）
  staticSentenceList = splitTextIntoSentences(cleanText);
  staticSentenceIndex = 0;
  staticPlaybackMsgId = message.id;

  const gen = staticPlaybackGen;
  currentSpeakingMsgId.value = message.id;
  isSpeaking.value = true;

  playNextStaticSentence(gen);
  startSpeakingCheck();
};

// 倍速变化时实时调整：取消当前句子，从当前句重新开始（新倍速）
watch(voiceSpeed, () => {
  // 非流式播放中：立即从当前句子重新播放（新倍速）
  if (isSpeaking.value && staticPlaybackMsgId && !streamingTtsActive) {
    staticPlaybackGen++; // 递增代次，使旧 onend 失效
    staticSentenceIndex = Math.max(0, staticSentenceIndex - 1); // 回退到当前句
    window.speechSynthesis.cancel();
    const gen = staticPlaybackGen;
    setTimeout(() => {
      if (gen === staticPlaybackGen && staticPlaybackMsgId) {
        playNextStaticSentence(gen);
      }
    }, 50);
  }
  // 流式播放中：当前句子播完后，下一句自动使用新倍速（speakSentence 每次读取 voiceSpeed.value）
});

// ========== 流式 TTS：边接收边播放 ==========

let streamingTtsPlayedLength = 0; // 已送入 TTS 的文本长度
let streamingTtsActive = false; // 流式 TTS 是否激活
let ttsUtteranceCount = 0; // 当前排队的 utterance 数量
let speakingCheckTimer = null; // 轮询定时器：检测 TTS 是否已停止
let userStoppedSending = false; // 用户主动停止发送，阻止后续自动播放

// 重置播放状态（统一出口）
const resetSpeakingState = () => {
  isSpeaking.value = false;
  currentSpeakingMsgId.value = null;
  ttsUtteranceCount = 0;
  stopSpeakingCheck();
};

// 启动轮询：检测 speechSynthesis 是否已实际停止（onend 不可靠的兜底方案）
const startSpeakingCheck = () => {
  stopSpeakingCheck();
  speakingCheckTimer = setInterval(() => {
    // speechSynthesis 既不在播放也没有排队，但 isSpeaking 仍为 true → 状态卡住了
    if (isSpeaking.value && !window.speechSynthesis.speaking && !window.speechSynthesis.pending) {
      console.log('检测到 TTS 已停止但状态未重置，自动修复');
      resetSpeakingState();
      return;
    }
    // Chrome 15秒暂停 bug：长文本会自动暂停，定期 resume 保持播放
    if (window.speechSynthesis.speaking && window.speechSynthesis.paused) {
      window.speechSynthesis.resume();
    }
  }, 500);
};

const stopSpeakingCheck = () => {
  if (speakingCheckTimer) {
    clearInterval(speakingCheckTimer);
    speakingCheckTimer = null;
  }
};

// 开始流式 TTS
const startStreamingTTS = () => {
  if (!isVoiceMode.value) return;
  window.speechSynthesis.cancel();
  streamingTtsPlayedLength = 0;
  streamingTtsActive = true;
  ttsUtteranceCount = 0;
  isSpeaking.value = false;
  currentSpeakingMsgId.value = null;
  stopSpeakingCheck();
};

// 播放单个句子（加入 TTS 队列）
const speakSentence = (text) => {
  if (!text || !text.trim()) return;
  const cleanText = sanitizeTextForTTS(text);
  if (!cleanText) return;

  const utterance = new SpeechSynthesisUtterance(cleanText);
  utterance.lang = 'zh-CN';
  const bestVoice = getBestChineseVoice();
  if (bestVoice) utterance.voice = bestVoice;
  utterance.pitch = 1.0;
  utterance.rate = voiceSpeed.value;
  utterance.volume = 1.0;

  ttsUtteranceCount++;
  utterance.onstart = () => {
    isSpeaking.value = true;
    startSpeakingCheck();
  };
  utterance.onend = () => {
    ttsUtteranceCount = Math.max(0, ttsUtteranceCount - 1);
    if (ttsUtteranceCount === 0) {
      resetSpeakingState();
    }
  };
  utterance.onerror = () => {
    ttsUtteranceCount = Math.max(0, ttsUtteranceCount - 1);
    if (ttsUtteranceCount === 0) {
      resetSpeakingState();
    }
  };

  window.speechSynthesis.speak(utterance);
};

// 流式播放：从累积文本中提取新的完整句子并播放
const streamTTS = (fullText) => {
  if (!isVoiceMode.value || !streamingTtsActive || !fullText) return;

  // 在已播放位置之后，查找待播放的文本
  const remaining = fullText.substring(streamingTtsPlayedLength);
  if (!remaining || remaining.length < 2) return;

  // 1. 优先找句子结束符（强分割点：。！？.!?\n）
  const sentenceEndRegex = /[。！？.!?\n]/g;
  let lastSentenceEnd = -1;
  let match;
  while ((match = sentenceEndRegex.exec(remaining)) !== null) {
    lastSentenceEnd = match.index + 1;
  }

  if (lastSentenceEnd > 0) {
    // 有完整句子，立即播放
    const newSentences = remaining.substring(0, lastSentenceEnd);
    speakSentence(newSentences);
    streamingTtsPlayedLength += lastSentenceEnd;
    return;
  }

  // 2. 没有句子结束符时，按累积长度决定是否提前播放（避免第一句话太长时等太久）
  const MAX_WAIT_LENGTH = 30; // 超过30字强制播放（无标点也切）
  const MIN_WEAK_BREAK_LENGTH = 8; // 超过8字时，逗号/分号也作为分割点

  if (remaining.length >= MAX_WAIT_LENGTH) {
    // 累积文本已超过最大等待长度，强制播放
    speakSentence(remaining);
    streamingTtsPlayedLength += remaining.length;
    return;
  }

  if (remaining.length >= MIN_WEAK_BREAK_LENGTH) {
    // 检查是否有逗号/分号等弱分割点
    const weakBreakRegex = /[，,；;、：:]/g;
    let lastWeakBreak = -1;
    while ((match = weakBreakRegex.exec(remaining)) !== null) {
      lastWeakBreak = match.index + 1;
    }
    if (lastWeakBreak > 0) {
      const partial = remaining.substring(0, lastWeakBreak);
      speakSentence(partial);
      streamingTtsPlayedLength += lastWeakBreak;
    }
  }
};

// 完成流式播放：播放剩余的未完成文本
const finishStreamingTTS = (fullText, msgId) => {
  if (!isVoiceMode.value) return;
  // 流式未激活（例如中途被停止/切换），由 watch 自动播放处理
  if (!streamingTtsActive) return;

  // 播放剩余文本
  const remaining = fullText.substring(streamingTtsPlayedLength);
  if (remaining.trim()) {
    speakSentence(remaining);
  }
  streamingTtsPlayedLength = fullText.length;
  streamingTtsActive = false;

  // 只有实际有 utterance 在播放时才设置状态（用于语音条动画）
  // 非流式场景（没有收到任何流式文本）ttsUtteranceCount === 0，不设置状态，
  // 由下方 watch 检测到 !isSpeaking 后调用 autoPlayLatestAIVoice 常规播放
  if (ttsUtteranceCount > 0) {
    currentSpeakingMsgId.value = msgId;
    isSpeaking.value = true;
    startSpeakingCheck(); // 确保轮询兜底检测已启动
  }
};

// 自动播放最新AI消息
const autoPlayLatestAIVoice = () => {
  if (!isVoiceMode.value) return;
  if (isSending.value) return;
  const lastMsg = messages.value[messages.value.length - 1];
  if (lastMsg && !lastMsg.isSend && lastMsg.content && lastMsg.content.trim()) {
    console.log('自动播放AI语音:', lastMsg.content.substring(0, 30));
    nextTick(() => {
      playAIVoice(lastMsg);
    });
  }
};

// 监听消息变化，自动播放AI语音
watch(
  () => messages.value.length,
  (newLen) => {
    // 流式 TTS 激活时跳过（由 finishStreamingTTS 处理播放）
    // isSpeaking 已为 true 时跳过（finishStreamingTTS 已开始播放，避免 playAIVoice 误判为停止）
    // userStoppedSending 时跳过（用户主动停止发送，阻止 SignalR 延迟完成事件触发自动播放）
    if (newLen > 0 && isVoiceMode.value && !isSending.value && !streamingTtsActive && !isSpeaking.value && !userStoppedSending) {
      autoPlayLatestAIVoice();
    }
  }
);

// 关闭语音模式时停止所有语音
watch(isVoiceMode, async (newVal) => {
  if (newVal) {
    // 开启语音模式时：解锁语音合成 + 提前申请麦克风权限
    unlockSpeechSynthesis();
    await ensureMicrophonePermission();
  } else {
    window.speechSynthesis.cancel();
    streamingTtsActive = false;
    streamingTtsPlayedLength = 0;
    staticPlaybackGen++; // 使旧回调失效
    staticPlaybackMsgId = null;
    staticSentenceList = [];
    staticSentenceIndex = 0;
    resetSpeakingState();
    micPermissionGranted.value = false;
    if (pendingStopTimer) {
      clearTimeout(pendingStopTimer);
      pendingStopTimer = null;
    }
    if (recognition.value && (isRecording.value || isRecognizing.value)) {
      try { recognition.value.abort(); } catch (e) { /* ignore */ }
    }
    isRecording.value = false;
    isRecognizing.value = false;
    recognizedPreviewText.value = '';
    voiceSentHint.value = '';
  }
});

// 转文字开关变化时，保持滚动到最底部（避免内容高度变化导致跳动）
watch(showTextInVoiceMode, () => {
  nextTick(() => {
    nextTick(() => {
      if (messagesContainer.value) {
        messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
      }
    });
  });
});

// 分页相关变量
const currentPage = ref(1);
const hasMoreMessages = ref(true);
const loadingMessages = ref(false);
const pageSize = ref(20); // 每页消息数量

// 智能体相关
const aiApps = ref([]); // 存储智能体列表
const selectedAiApp = ref(null); // 存储选中的智能体

// 复制功能
const copyMessageContent = async (content) => {
  if (!content) {
    message.warning('没有可复制的内容');
    return;
  }

  try {
    // 优先使用 clipboard API
    if (navigator.clipboard) {
      await navigator.clipboard.writeText(content);
      message.success('内容已复制到剪贴板');
    } else {
      // 降级方案：使用 textarea
      const textarea = document.createElement('textarea');
      textarea.value = content;
      textarea.style.position = 'fixed';
      textarea.style.left = '-9999px';
      document.body.appendChild(textarea);
      textarea.select();
      document.execCommand('copy');
      document.body.removeChild(textarea);
      message.success('内容已复制到剪贴板');
    }
  } catch (err) {
    console.error('复制失败:', err);
    message.error('复制失败');
  }
};

// 将消息内容中的 URL 转换为可点击的链接
const formatMessageContent = (content) => {
  if (!content) return '';

  const urlRegex = /https?:\/\/[^\s<>"']+/gi;

  const escapeHtml = (str) =>
    str.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

  return content.replace(urlRegex, (url) => {
    let cleanUrl = url;

    // 1. 去掉末尾的 .,;:!? 和 Markdown 符号 * _ ~ `
    cleanUrl = cleanUrl.replace(/[.,;:!?*_~`]+$/, '');
    // 2. 循环去掉不成对的 ) ] } ' "
    while (/[)}\]'"`]$/.test(cleanUrl)) {
      const last = cleanUrl.slice(-1);
      const openMap = { ')': '(', ']': '[', '}': '{', "'": "'", '"': '"', '`': '`' };
      if (cleanUrl.includes(openMap[last])) break;
      cleanUrl = cleanUrl.slice(0, -1);
    }

    // 必须仍是 http(s) 开头
    if (!/^https?:\/\//i.test(cleanUrl)) return url;

    const safeUrl = escapeHtml(cleanUrl);

    const docContent = `<!DOCTYPE html>
<html>
<head><meta charset="UTF-8"><style>body{margin:0;font:14px/1.5 sans-serif;}a{color:#1a73e8;text-decoration:none;word-break:break-all;}a:hover{text-decoration:underline;}</style></head>
<body><a href="${safeUrl}" target="_blank" rel="noopener noreferrer">${safeUrl}</a></body>
</html>`;

    const safeSrcdoc = escapeHtml(docContent);

    // 高度设为 auto，并设最小高度，避免截断
   return `<iframe srcdoc="${safeSrcdoc}" sandbox="allow-same-origin allow-popups" style="border:none;width:100%;height:1.6em;display:inline-block;vertical-align:middle;" title="URL 预览"></iframe>`;
  });
};

// 文件上传成功处理
const handleFileUploadSuccess = (result) => {
  uploadedFileList.value.push({
    id: result.fileId,
    name: result.fileName,
    url: result.url,
    path: result.path
  });
  pendingFileNames.value.push(result.fileName);
  pendingFileUrls.value.push(result.url);
  nextTick(() => {
    if (fileUploadRef.value) {
      fileUploadRef.value.clearUploadedFiles();
    }
  });
  message.success('文件上传成功');
};

// 文件上传失败处理
const handleFileUploadError = (error) => {
  console.error('文件上传失败:', error);
  message.error('文件上传失败');
};

// 移除已上传文件
const removeUploadedFile = (index) => {
  uploadedFileList.value.splice(index, 1);
  pendingFileNames.value.splice(index, 1);
  pendingFileUrls.value.splice(index, 1);
};

// 清空文件列表
const clearUploadedFiles = () => {
  fileBackups.value = [...uploadedFileList.value];
  uploadedFileList.value = [];
  pendingFileNames.value = [];
  pendingFileUrls.value = [];
};

// 恢复文件列表（中止时）
const restoreUploadedFiles = () => {
  if (fileBackups.value.length > 0) {
    uploadedFileList.value = [...fileBackups.value];
    pendingFileNames.value = uploadedFileList.value.map(f => f.name);
    pendingFileUrls.value = uploadedFileList.value.map(f => f.url);
    fileBackups.value = [];
  }
};

// 获取对话列表
const loadConversations = async () => {
  loadingConversations.value = true;
  try {
    // 调用真实API获取对话列表
    const response = await getAIChatsMyPageData({
      pageNum: 1,
      pageSize: 200, // 获取足够多的对话记录
    });

    if (response && response.code === 200 && response.data) {
      if (response.data.data.length > 0) {
        conversations.value = response.data.data.map((item) => ({
          id: item.id,
          title: item.name,
          lastMessage: item.lastMessage,
          createdAt: item.createTime,
          updatedAt: item.updateTime,
          appId: item.appId,
        }));
        selectConversation(conversations.value[0]);
      } else {
        conversations.value = [];
      }
    } else {
      throw new Error(response?.msg || "获取对话列表失败");
    }
    loadingConversations.value = false;
  } catch (error) {
    console.error("加载对话列表失败:", error);
    message.error("加载对话列表失败: " + (error.message || error));
    loadingConversations.value = false;
  }
};

// 加载智能体列表
const loadAIApps = async () => {
  try {
    const response = await getMyAIAppsALLList();
    if (response && response.code === 200 && response.data) {
      // 格式化数据以适应Select组件
      aiApps.value = response.data.map((app) => ({
        label: app.name,
        value: app.id,
      }));
    }
  } catch (error) {
    console.error("加载智能体列表失败:", error);
    message.error("加载智能体列表失败");
  }
};

// 显示智能体选择模态框
const showAgentSelectionModal = (defaultAppId = null) => {
  // 设置默认选中的智能体：优先使用传入的appId，其次使用当前对话的appId，最后使用第一个智能体
  selectedAiApp.value = defaultAppId || activeConversation.value?.appId || (aiApps.value.length > 0 ? aiApps.value[0].value : null);

  // 创建模态框
  const modal = Modal.confirm({
    title: "选择智能体",
    content: () =>
      h("div", { class: "agent-selection-modal" }, [
        h(Select, {
          placeholder: "请选择智能体",
          options: aiApps.value,
          value: selectedAiApp.value,
          "onUpdate:value": (value) => {
            selectedAiApp.value = value;
          },
          style: { width: "100%" },
        }),
      ]),
    okText: "确认",
    cancelText: "取消",
    onOk: async () => {
      if (!selectedAiApp.value) {
        message.warning("请选择智能体");
        return Promise.reject();
      }

      // 创建新对话
      try {
        // 禁用取消按钮和确认按钮，防止重复点击
        modal.update({
          cancelButtonProps: { disabled: true },
          okButtonProps: { disabled: true }
        });

        // 显示加载提示
        const loadingMsg = message.loading('正在创建对话...', 0);

        // 执行创建对话操作
        await createNewConversation(selectedAiApp.value);

        // 关闭加载提示
        loadingMsg();

        // 成功后关闭模态框
        return Promise.resolve();
      } catch (error) {
        // 关闭加载提示
        message.destroy();

        // 重新启用按钮
        modal.update({
          cancelButtonProps: { disabled: false },
          okButtonProps: { disabled: false }
        });

        message.error("创建对话失败: " + (error.message || error));
        return Promise.reject();
      }
    },
  });
};

// 创建新对话
const createNewConversation = async (appId) => {
  try {
    const response = await addAIChats({
      appId: appId,
      name: "新对话",
      lastMessage: "",
    });

    if (response && response.code === 200 && response.data) {
      await loadConversations();
    } else {
      throw new Error(response?.msg || "创建对话失败");
    }
  } catch (error) {
    console.error("创建对话失败:", error);
    message.error("创建对话失败: " + (error.message || error));
  }
};

// 获取智能体名称
const getAiAppName = (appId) => {
  if (!appId) return "";
  const aiApp = aiApps.value.find((app) => app.value === appId);
  return aiApp ? aiApp.label : "";
};

// 选择对话
const selectConversation = async (conversation) => {
  activeConversationId.value = conversation.id;
  activeConversation.value = conversation;

  // 重置分页参数
  currentPage.value = 1;
  hasMoreMessages.value = true;
  messages.value = [];

  // 加载聊天记录
  await loadChatHistory(conversation.id, 1);

  // 等待DOM更新后滚动到底部
  nextTick(() => {
    scrollToBottom();
  });
};

// 加载聊天记录
const loadChatHistory = async (chatId, page) => {
  if (loadingMessages.value || !hasMoreMessages.value) return;

  loadingMessages.value = true;
  try {
    const response = await getAIChatHistorysPageData({
      whereId: chatId,
      pageNum: page,
      pageSize: pageSize.value,
    });

    if (response && response.code === 200 && response.data) {
      const historyData = response.data.data || [];

      // 处理消息格式
      const historyMessages = historyData.map((item) => ({
        id: item.id,
        conversationId: item.aIChatsId,
        isSend: item.isSend,
        content: item.content,
        aiReasoningContent: item.aiReasoningContent,
        aiToolsContent: item.aiToolsContent,
        fileNames: item.fileNames || '',
        contentFileUrls: item.contentFileUrls || '',
        aIChatHistorysBindLogs: item.aIChatHistorysBindLogs || [],
        createdAt: item.createTime || new Date().toISOString(),
        cachedInputTokenCount: item.cachedInputTokenCount || 0,
         InputTokenCount: item.InputTokenCount || 0,
         OutputTokenCount: item.OutputTokenCount || 0,
         cachedOutputTokenCount: item.cachedOutputTokenCount || 0,
         totalTokenCount: item.totalTokenCount || 0,
         reasoningTokenCount: item.reasoningTokenCount || 0,
      }));

      // 如果是第一页，直接替换消息列表，否则添加到列表开头（历史消息在前）
      if (page === 1) {
        messages.value = historyMessages.reverse();
      } else {
        // 保存当前滚动位置
        const container = messagesContainer.value;
        const beforeScrollHeight = container.scrollHeight;
        const beforeScrollTop = container.scrollTop;

        // 将新消息添加到列表开头
        messages.value = [...historyMessages.reverse(), ...messages.value];

        // 保持滚动位置不变
        nextTick(() => {
          container.scrollTop =
            beforeScrollTop + (container.scrollHeight - beforeScrollHeight);
        });
      }

      // 判断是否还有更多数据
      if (historyData.length < pageSize.value) {
        hasMoreMessages.value = false;
      }

      // 更新当前页
      if (page === 1) {
        currentPage.value = 1;
      } else {
        currentPage.value = page;
      }
    } else {
      throw new Error(response?.msg || "获取聊天记录失败");
    }
  } catch (error) {
    console.error("加载聊天记录失败:", error);
    message.error("加载聊天记录失败: " + (error.message || error));
  } finally {
    loadingMessages.value = false;
  }
};

// 处理滚动事件，实现无限滚动加载
const handleScroll = () => {
  if (!messagesContainer.value || loadingMessages.value || !hasMoreMessages.value) return;

  const { scrollTop } = messagesContainer.value;
  // 当滚动到顶部附近时加载更多
  if (scrollTop < 50) {
    loadMoreMessages();
  }
};

// 加载更多消息
const loadMoreMessages = () => {
  if (hasMoreMessages.value && activeConversationId.value) {
    loadChatHistory(activeConversationId.value, currentPage.value + 1);
  }
};

// 处理回车键按下
const handlePressEnter = (e) => {
  // 如果按下了 Shift+Enter，则允许换行
  if (e.shiftKey) {
    return; // 继续执行默认的换行行为
  }

  // 阻止默认行为（换行）
  e.preventDefault();
  // 调用发送消息方法
  sendMessage();
};

// 发送消息
const sendMessage = async () => {
  if (!newMessage.value.trim() || isSending.value) return;
  const messageToSend = newMessage.value.trim();
  isSending.value = true;
  userStoppedSending = false; // 重置停止标志，允许自动播放
  expandedReasoning.value = false;
  expandedTools.value = false;
  currentReceivingMsgId.value = null;
  aimessage.value='正在思考....'
  aimessage2.value='';
  aIToolsContentMsg.value='';
  aIReasoningContentMsg.value='';
  lastSentMessage.value = messageToSend;
  abortController = new AbortController();

  const currentFileNames = [...pendingFileNames.value];
  const currentFileUrls = [...pendingFileUrls.value];
  clearUploadedFiles();

    // 使用nextTick确保DOM更新
  nextTick(() => {
     newMessage.value = "";
     // 滚动到底部以显示最新内容
        scrollToBottom();
  });
 const snowflakeId = (await GetSnowflakeId()).data;
  try {
    // 添加用户消息到列表
    const userMessage = {
      id: Date.now(),
      conversationId: activeConversationId.value,
      isSend: true,
      content: messageToSend,
      fileNames: currentFileNames.join(','),
      contentFileUrls: currentFileUrls.join(','),
      totalTokenCount: 0,
      createdAt: new Date().toISOString(),
    };
    lastSentMessageId.value = userMessage.id;
    messages.value.push(userMessage);
     // 滚动到底部以显示最新内容
        scrollToBottom();
         //监听流式输出
    // 语音模式下：启动流式 TTS（边接收边播放）
    if (isVoiceMode.value) {
      startStreamingTTS();
    }
    getAiMySignalRHubMsg(snowflakeId);
    var reulst;
    try {
      reulst = await addAIChatHistorys({
        aIChatsId: activeConversationId.value,
        id:snowflakeId,
        content: messageToSend,
        isOnlineSearch: isOnlineSearch.value,
        fileNames: currentFileNames.join(','),
        contentFileUrls: currentFileUrls.join(',')
      }, abortController.signal);
    } catch (error) {
      if (error.name === 'AbortError' || error.name === 'CanceledError' || error.message?.includes('cancel')) {
        message.info('已中止发送');
        stopAiMySignalRHubMsg(snowflakeId);
        isSending.value = false;
        restoreUploadedFiles();
        return;
      }
      const errorMsg = error.message || '';
      if (errorMsg.includes('聊天记录已达上限')) {
        Modal.confirm({
          title: '提示',
          content: '聊天记录已达上限，为了更好的体验，请新建对话？',
          okText: '新建对话',
          cancelText: '取消',
          onOk: () => {
            showAgentSelectionModal(activeConversation.value?.appId);
          }
        });
      } else {
        message.error(errorMsg || "发送失败");
      }
      stopAiMySignalRHubMsg(snowflakeId);
      isSending.value = false;
      restoreUploadedFiles();
      return;
    }
    scrollToBottom();
    lastSentMessage.value = "";
    lastSentMessageId.value = null;
    // 更新对话列表中的预览
    const conversation = conversations.value.find(
      (c) => c.id === activeConversationId.value
    );
    if (conversation) {
      conversation.lastMessage = messageToSend;
      conversation.updatedAt = new Date().toISOString();
      if (!conversation.title) {
        conversation.title =
          messageToSend.substring(0, 20) + (messageToSend.length > 20 ? "..." : "");
      }
    }
    const aiMessage = {
      id: reulst.data.id,
      conversationId: activeConversationId.value,
      isSend: reulst.data.isSend,
      content: reulst.data.content,
      aiToolsContent: reulst.data.aiToolsContent,
      aiReasoningContent: reulst.data.aiReasoningContent,
      createdAt: reulst.data.createTime,
      aIChatHistorysBindLogs: reulst.data.aIChatHistorysBindLogs || [],
      cachedInputTokenCount: reulst.data.cachedInputTokenCount || 0,
       InputTokenCount: reulst.data.InputTokenCount || 0,
       OutputTokenCount: reulst.data.OutputTokenCount || 0,
       cachedOutputTokenCount: reulst.data.cachedOutputTokenCount || 0,
       totalTokenCount: reulst.data.totalTokenCount || 0,
       reasoningTokenCount: reulst.data.reasoningTokenCount || 0,
    };
    if (aiMessage.aiReasoningContent) {
      expandedReasoning.value = true;
    }
    if (aiMessage.aiToolsContent) {
      expandedTools.value = true;
    }
    messages.value.push(aiMessage);
    currentReceivingMsgId.value = aiMessage.id;
    isSending.value = false;
    
    // 先保存流式文本（stopAiMySignalRHubMsg 会清空 aimessage2）
    const streamedText = aimessage2.value;
    
    stopAiMySignalRHubMsg(snowflakeId);
    
    // 将流式接收的内容保存到AI消息对象中
    const lastAiMsg = messages.value[messages.value.length - 1];
    if (lastAiMsg && streamedText) {
      lastAiMsg.content = streamedText;
    }
    
    scrollToBottom();
    // 语音模式下：完成流式播放剩余内容，或常规播放
    if (isVoiceMode.value) {
      finishStreamingTTS(streamedText, lastAiMsg?.id);
    }
  } catch (error) {
    if (error.name === 'AbortError' || error.name === 'CanceledError' || error.message?.includes('cancel')) {
      message.info('已中止发送');
      stopAiMySignalRHubMsg(snowflakeId);
      isSending.value = false;
      restoreUploadedFiles();
      return;
    }
    stopAiMySignalRHubMsg(snowflakeId);
    console.error("发送消息失败:", error);
    message.error("发送消息失败");
    isSending.value = false;
    restoreUploadedFiles();
  }
};

// 滚动到底部
const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  });
};

const showDetailModal = (title, content) => {
  detailModalTitle.value = title;
  detailModalContent.value = content;
  detailModalVisible.value = true;
};

const truncateContent = (content, maxLength = 350) => {
  if (!content || content.length <= maxLength) return content;
  return content.substring(0, maxLength) + '...';
};

// 获取日志类型名称
const getLogTypeName = (logType) => {
  const types = {
    1: '知识库',
    2: '网络搜索',
    3: '系统提示词',
    4: '文件内容'
  };
  return types[logType] || '未知';
};

var connectionServer=null;
let connectionTimeout = null;

const getAiMySignalRHubMsg=(id)=>{
   connectionServer=new signalR.HubConnectionBuilder().withUrl(`${process.env.VUE_APP_API_BASE_URL}/api/MySignalRHub?IdentityId=${id}&Authorization=${localStorage.getItem('token')}`,{

    headers: {
      "Authorization": `Bearer ${localStorage.getItem('token')}`
    }
  })
  .withAutomaticReconnect()
  .build()
connectionServer.start().then(() => {})
connectionServer.onreconnecting(() => {})
connectionServer.onreconnected(() => { })
connectionServer.onclose(() => {})
// 客户端保持连接请求到服务端时间间隔
connectionServer.keepAliveIntervalInMilliseconds = 12e4
// 服务端保持连接请求到客户端时间间隔
connectionServer.serverTimeoutInMilliseconds = 24e4
// 接收AI消息
connectionServer.on('aimsg', (msg) => {
  // 使用nextTick确保DOM更新
  nextTick(() => {
    // 实现逐字显示效果
    aimessage2.value+=msg;
     // 滚动到底部以显示最新内容
        scrollToBottom();
    // 流式播放语音：检测到完整句子就立即播放
    if (isVoiceMode.value && streamingTtsActive) {
      streamTTS(aimessage2.value);
    }
  });
})
// 接收AI消息
connectionServer.on('processmsg', (msg) => {
    aimessage.value=msg
})
// 接收AI工具调用内容（叠加）
connectionServer.on('aIToolsContentMsg', (msg) => {
 // 使用nextTick确保DOM更新
  nextTick(() => {
    // 实现逐字显示效果
    aIToolsContentMsg.value+=msg;
     // 滚动到底部以显示最新内容
     if (aIToolsContentMsg.length<350) {
     scrollToBottom();
    }
  });
})
// 接收AI思考过程内容（叠加）
connectionServer.on('aIReasoningContentMsg', (msg) => {
 // 使用nextTick确保DOM更新
  nextTick(() => {
    // 实现逐字显示效果
    aIReasoningContentMsg.value+=msg;
       // 滚动到底部以显示最新内容
     if (aIReasoningContentMsg.length<350) {
     scrollToBottom();
    }
  });
})
}

const stopAiMySignalRHubMsg=(id)=>{
  aimessage.value='';
  aimessage2.value='';
  // 不重置 streamingTtsActive，由 finishStreamingTTS 控制
  expandedReasoning.value = false;
  expandedTools.value = false;
  currentReceivingMsgId.value = null;
  if (connectionTimeout) {
    clearTimeout(connectionTimeout);
    connectionTimeout = null;
  }
  connectionServer?.stop();
}

const stopMessage = () => {
  if (!isSending.value) return;
  if (abortController) {
    abortController.abort();
    abortController = null;
  }
  // 标记用户主动停止，阻止 SignalR 延迟完成事件触发自动播放
  userStoppedSending = true;
  // 停止流式 TTS
  streamingTtsActive = false;
  streamingTtsPlayedLength = 0;
  staticPlaybackGen++; // 使旧回调失效
  staticPlaybackMsgId = null;
  staticSentenceList = [];
  staticSentenceIndex = 0;
  window.speechSynthesis.cancel();
  resetSpeakingState();
  stopAiMySignalRHubMsg();
  if (lastSentMessageId.value !== null) {
    const index = messages.value.findIndex(m => m.id === lastSentMessageId.value);
    if (index !== -1) {
      messages.value.splice(index, 1);
    }
    lastSentMessageId.value = null;
  }
  if (lastSentMessage.value) {
    newMessage.value = lastSentMessage.value;
    lastSentMessage.value = "";
  }
  isSending.value = false;
  aimessage.value='';
  aimessage2.value='';
  aIToolsContentMsg.value='';
  aIReasoningContentMsg.value='';
}


// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  const now = new Date();

  const targetDate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const currentDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const diffTime = currentDate - targetDate;
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));

  if (diffDays === 0) {
    return "今天";
  } else if (diffDays === 1) {
    return "昨天";
  } else if (diffDays <= 7) {
    return `${diffDays}天前`;
  } else {
    return date.toLocaleDateString("zh-CN");
  }
};

// 格式化时间
const getCollapseHeader = (message) => {
  const parts = [];
  if (message.aiReasoningContent) parts.push('思考过程');
  if (message.aiToolsContent) parts.push('工具调用');
  return parts.join(' + ') || 'AI详情';
};

const formatTime = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleTimeString("zh-CN", { hour: "2-digit", minute: "2-digit" });
};

const formatTokenCount = (count) => {
  if (!count || count === 0) return "";
  if (count >= 10000) {
    return (count / 10000).toFixed(2) + "万";
  }
  return count.toString();
};

// 监听消息变化，自动滚动到底部
watch(messages, () => {
  scrollToBottom();
});

// 组件挂载时加载对话列表
onMounted(async () => {
  await loadConversations();
  await loadAIApps(); // 加载智能体列表

  // 添加滚动事件监听器
  if (messagesContainer.value) {
    messagesContainer.value.addEventListener("scroll", handleScroll);
  }
});

// 组件卸载时移除事件监听器
onUnmounted(() => {
  if (messagesContainer.value) {
    messagesContainer.value.removeEventListener("scroll", handleScroll);
  }
  if (pendingStopTimer) {
    clearTimeout(pendingStopTimer);
    pendingStopTimer = null;
  }
  stopSpeakingCheck(); // 清理轮询定时器
  staticPlaybackGen++; // 使旧回调失效
  staticPlaybackMsgId = null;
  window.speechSynthesis?.cancel();
  if (recognition.value) {
    try { recognition.value.abort(); } catch (e) { /* ignore */ }
  }
});

// 删除对话
const deleteConversation = async (conversationId, event) => {
  event.stopPropagation(); // 阻止事件冒泡，避免触发选择对话

  try {
    await Modal.confirm({
      title: "确认删除",
      content: "确定要删除这个对话吗？此操作不可恢复。",
      okText: "确认",
      cancelText: "取消",
      onOk: async function () {
        const response = await deleteAIChats(conversationId);
        if (response.code === 200) {
          loadConversations(); // 重新加载对话列表
          message.success("删除成功");
        }
      },
    });
  } catch (error) {
    if (error?.message !== "取消") {
      console.error("删除对话失败:", error);
      message.error("删除对话失败: " + (error.message || error));
    }
  }
};
</script>

<style scoped>
/* ============ 全局科技风格 ============ */
.ai-chat-container {
  height: calc(100vh - 220px);
  border-radius: 12px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, #0a0e27 0%, #0d1b3e 30%, #0f1a3a 60%, #0a0e27 100%);
  border: 1px solid rgba(0, 200, 255, 0.15);
  color: #e0e8ff;
  position: relative;
  box-shadow:
    0 0 30px rgba(0, 150, 255, 0.1),
    inset 0 0 30px rgba(0, 100, 200, 0.03);
}

/* ============ 粒子背景 ============ */
.particles-bg {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.particle {
  position: absolute;
  background: rgba(0, 200, 255, 0.4);
  border-radius: 50%;
  animation: floatUp linear infinite;
}

@keyframes floatUp {
  0% {
    transform: translateY(0) scale(1);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100vh) scale(0.3);
    opacity: 0;
  }
}

/* ============ 布局 ============ */
.chat-layout {
  display: flex;
  height: 100%;
  position: relative;
  z-index: 1;
}

/* ============ 左侧栏 ============ */
.chat-sidebar {
  width: 280px;
  border-right: 1px solid rgba(0, 200, 255, 0.12);
  display: flex;
  flex-direction: column;
  background: rgba(10, 15, 40, 0.85);
  backdrop-filter: blur(20px);
  height: 100%;
}

.sidebar-header {
  padding: 20px 16px;
  border-bottom: 1px solid rgba(0, 200, 255, 0.12);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
}

.sidebar-title-group {
  display: flex;
  align-items: center;
  gap: 10px;
}

.sidebar-icon {
  position: relative;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #00c8ff;
  font-size: 18px;
}

.icon-pulse {
  position: absolute;
  inset: -4px;
  border-radius: 50%;
  border: 1.5px solid rgba(0, 200, 255, 0.5);
  animation: iconPulse 2s ease-in-out infinite;
}

@keyframes iconPulse {
  0%, 100% {
    transform: scale(1);
    opacity: 0.5;
  }
  50% {
    transform: scale(1.3);
    opacity: 0;
  }
}

.sidebar-header h3 {
  margin: 0;
  color: #c8d6ff;
  font-size: 15px;
  font-weight: 500;
  letter-spacing: 0.5px;
}

.add-button {
  background: linear-gradient(135deg, #0066ff, #00aaff) !important;
  border: none !important;
  border-radius: 8px !important;
  box-shadow: 0 0 15px rgba(0, 100, 255, 0.3);
  transition: all 0.3s ease;
}

.add-button:hover {
  background: linear-gradient(135deg, #0080ff, #00ccff) !important;
  box-shadow: 0 0 25px rgba(0, 150, 255, 0.5);
  transform: translateY(-1px);
}

.conversation-list {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  scrollbar-width: thin;
  scrollbar-color: rgba(0, 180, 255, 0.3) transparent;
  height: 0;
  padding: 10px;
}

.conversation-list::-webkit-scrollbar {
  width: 4px;
}

.conversation-list::-webkit-scrollbar-track {
  background: transparent;
}

.conversation-list::-webkit-scrollbar-thumb {
  background: rgba(0, 180, 255, 0.3);
  border-radius: 4px;
}

.conversation-list::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 200, 255, 0.6);
}

.conversation-item {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(0, 200, 255, 0.08);
  margin-bottom: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 14px 12px;
  color: #a0b4d0;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  border-radius: 10px;
  position: relative;
  overflow: hidden;
}

.conversation-item::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(180deg, #0066ff, #00ccff);
  border-radius: 0 3px 3px 0;
  opacity: 0;
  transition: opacity 0.3s;
}

.conversation-item:hover {
  background: rgba(0, 150, 255, 0.08);
  border-color: rgba(0, 200, 255, 0.25);
  transform: translateX(2px);
}

.conversation-item:hover::before {
  opacity: 1;
}

.conversation-item.active {
  background: rgba(0, 120, 255, 0.12);
  border: 1px solid rgba(0, 200, 255, 0.35);
  box-shadow: 0 0 15px rgba(0, 150, 255, 0.1);
}

.conversation-item.active::before {
  opacity: 1;
}

.conversation-item.disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.conversation-content {
  display: flex;
  flex-direction: column;
  flex: 1;
  text-align: left;
  overflow: hidden;
}

.conversation-title {
  font-weight: 600;
  margin-bottom: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  color: #d0dcff;
  font-size: 13px;
}

.conversation-preview {
  font-size: 11px;
  margin-bottom: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  color: #6b7fa0;
}

.conversation-time {
  font-size: 10px;
  color: #4a5f80;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.conversation-actions {
  opacity: 0;
  transition: opacity 0.3s ease;
  margin-left: 8px;
}

.conversation-item:hover .conversation-actions {
  opacity: 1;
}

.delete-btn {
  color: rgba(200, 200, 220, 0.4) !important;
}

.delete-btn:hover {
  color: #ff4d6a !important;
}

.empty-conversations {
  padding: 40px 20px;
  text-align: center;
}

:deep(.empty-conversations .ant-empty-description) {
  color: rgba(180, 200, 240, 0.5) !important;
}

/* ============ 右侧主区域 ============ */
.chat-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: linear-gradient(180deg, rgba(8, 16, 45, 0.6) 0%, rgba(10, 20, 50, 0.4) 100%);
  height: 100%;
}

/* ============ 聊天头部 ============ */
.chat-header {
  padding: 16px 24px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.06);
  flex-shrink: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
}

.header-left {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 14px;
  width: 85%;
  text-align: center;
}

.header-left > div:last-child {
  text-align: center;
}

/* 头部迷你机器人 */
.ai-robot-mini {
  position: relative;
  width: 40px;
  height: 40px;
  filter: drop-shadow(0 0 8px rgba(0, 100, 255, 0.3));
}

.robot-head-mini {
  position: absolute;
  top: 6px;
  left: 50%;
  transform: translateX(-50%);
  width: 24px;
  height: 20px;
  background: linear-gradient(180deg, #2a3a6a, #1a2a50);
  border-radius: 6px 6px 4px 4px;
  border: 1px solid rgba(0, 200, 255, 0.4);
  box-shadow: 0 0 8px rgba(0, 150, 255, 0.2);
}

.robot-eye-left-mini,
.robot-eye-right-mini {
  position: absolute;
  top: 5px;
  width: 5px;
  height: 5px;
  background: #00e5ff;
  border-radius: 50%;
  box-shadow: 0 0 6px #00e5ff;
  animation: eyeGlow 2s ease-in-out infinite;
}

.robot-eye-left-mini { left: 5px; }
.robot-eye-right-mini { right: 5px; }

.robot-mouth-mini {
  position: absolute;
  bottom: 4px;
  left: 50%;
  transform: translateX(-50%);
  width: 8px;
  height: 2px;
  background: rgba(0, 200, 255, 0.5);
  border-radius: 1px;
}

.robot-antenna-mini {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 2px;
  height: 8px;
  background: rgba(0, 200, 255, 0.5);
}

.antenna-dot-mini {
  position: absolute;
  top: -4px;
  left: 50%;
  transform: translateX(-50%);
  width: 6px;
  height: 6px;
  background: #00e5ff;
  border-radius: 50%;
  box-shadow: 0 0 8px #00e5ff;
  animation: antennaPulse 1.5s ease-in-out infinite;
}

@keyframes antennaPulse {
  0%, 100% { box-shadow: 0 0 6px #00e5ff; }
  50% { box-shadow: 0 0 14px #00e5ff, 0 0 20px rgba(0, 229, 255, 0.4); }
}

.chat-header h2 {
  margin: 0;
  color: #1a1a2e;
  font-size: 16px;
  font-weight: 600;
  letter-spacing: 0.3px;
}

.agent-info {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  margin-top: 4px;
  font-size: 12px;
  color: #1677ff;
}

.header-status {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: #4ade80;
}

.status-dot {
  width: 8px;
  height: 8px;
  background: #4ade80;
  border-radius: 50%;
  box-shadow: 0 0 8px #4ade80;
  animation: statusPulse 2s ease-in-out infinite;
}

@keyframes statusPulse {
  0%, 100% { box-shadow: 0 0 6px #4ade80; }
  50% { box-shadow: 0 0 14px #4ade80, 0 0 20px rgba(74, 222, 128, 0.3); }
}

/* ============ 消息区域 ============ */
.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  scrollbar-width: thin;
  scrollbar-color: rgba(0, 180, 255, 0.3) transparent;
  height: 0;
}

.chat-messages::-webkit-scrollbar {
  width: 5px;
}

.chat-messages::-webkit-scrollbar-track {
  background: transparent;
}

.chat-messages::-webkit-scrollbar-thumb {
  background: rgba(0, 180, 255, 0.3);
  border-radius: 4px;
}

.chat-messages::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 200, 255, 0.5);
}

.message-item {
  display: flex;
  gap: 14px;
  max-width: 80%;
}

.message-item.user-message {
  align-self: flex-end;
}

.message-item.ai-message {
  align-self: flex-start;
}

/* ============ 头像 ============ */
.message-avatar {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 16px;
}

.avatar-user {
  background: linear-gradient(135deg, #0066ff, #1677ff);
  color: #fff;
  border: 1.5px solid rgba(22, 119, 255, 0.5);
  box-shadow: 0 0 12px rgba(0, 100, 255, 0.3);
}

.avatar-ai {
  background: transparent !important;
  border: none !important;
  border-color: transparent !important;
  box-shadow: none !important;
  overflow: visible !important;
  width: 48px;
  height: 48px;
  border-radius: 0 !important;
}

/* AI 头像小脸 */
.ai-avatar-face {
  position: relative;
  width: 20px;
  height: 16px;
}

.ai-eye-l, .ai-eye-r {
  position: absolute;
  top: 3px;
  width: 4px;
  height: 4px;
  background: #00e5ff;
  border-radius: 50%;
  box-shadow: 0 0 4px #00e5ff;
}

.ai-eye-l { left: 3px; }
.ai-eye-r { right: 3px; }

.ai-mouth {
  position: absolute;
  bottom: 2px;
  left: 50%;
  transform: translateX(-50%);
  width: 8px;
  height: 2px;
  background: rgba(0, 200, 255, 0.6);
  border-radius: 1px;
}

.ai-avatar-face.thinking .ai-eye-l,
.ai-avatar-face.thinking .ai-eye-r {
  animation: blink 1.5s ease-in-out infinite;
}

@keyframes blink {
  0%, 90%, 100% { transform: scaleY(1); }
  95% { transform: scaleY(0.1); }
}

/* ============ 消息内容 ============ */
.message-content {
  position: relative;
  display: inline-flex;
  flex-direction: column;
  width: fit-content;
  max-width: 100%;
}

.message-actions {
  position: absolute;
  top: 8px;
  right: 8px;
  opacity: 0;
  transition: opacity 0.3s;
  z-index: 1;
}

.message-content:hover .message-actions {
  opacity: 1;
}

.copy-button {
  color: rgba(200, 220, 255, 0.5) !important;
}

.copy-button:hover {
  color: #00c8ff !important;
}

.message-text {
  padding: 12px 16px;
  border-radius: 16px;
  color: #e0e8ff;
  line-height: 1.6;
  white-space: pre-wrap;
  text-align: left;
  display: inline-block;
  max-width: 100%;
  overflow-wrap: break-word;
  word-break: break-word;
  font-size: 14px;
}

.message-item.user-message .message-text {
  background: linear-gradient(135deg, #0066ff, #0088ff);
  border-bottom-right-radius: 4px;
  box-shadow: 0 4px 15px rgba(0, 100, 255, 0.25);
}

.message-item.ai-message .message-text {
  background: none;
  border: none;
  border-bottom-left-radius: 4px;
  color: rgba(255, 255, 255, 0.9);
  box-shadow: none;
}

.message-text-stream {
  background: none !important;
  border: none !important;
  border-bottom-left-radius: 4px !important;
  color: rgba(255, 255, 255, 0.9) !important;
  animation: streamFadeIn 0.3s ease-out;
}

@keyframes streamFadeIn {
  from { opacity: 0.6; }
  to { opacity: 1; }
}

.message-text .url-link {
  color: #1677ff;
  text-decoration: underline;
  cursor: pointer;
  word-break: break-all;
}

.message-time {
  font-size: 11px;
  color: rgba(160, 190, 220, 0.5);
  margin-top: 6px;
}

.message-item.user-message .message-time {
  text-align: right;
  color: rgba(160, 200, 255, 0.5);
}

.stream-status {
  color: #1677ff !important;
  font-size: 12px;
}

.token-count {
  margin-left: 8px;
  font-size: 10px;
  color: rgba(0, 0, 0, 0.45);
}

/* ============ 打字指示器 ============ */
.typing-indicator {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  background: rgba(20, 35, 70, 0.7);
  border-radius: 16px;
  border-bottom-left-radius: 4px;
  border: 1px solid rgba(0, 200, 255, 0.1);
}

.typing-indicator span {
  height: 7px;
  width: 7px;
  background: #00c8ff;
  border-radius: 50%;
  margin: 0 2px;
  animation: typing 1.2s infinite;
  box-shadow: 0 0 6px rgba(0, 200, 255, 0.5);
}

.typing-indicator span:nth-child(2) {
  animation-delay: 0.2s;
}

.typing-indicator span:nth-child(3) {
  animation-delay: 0.4s;
}

@keyframes typing {
  0%, 100% {
    transform: translateY(0);
    opacity: 0.4;
  }
  50% {
    transform: translateY(-6px);
    opacity: 1;
  }
}

/* ============ 折叠面板 ============ */
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

.file-list-content {  
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.file-item {
  display: flex;
  align-items: center;
  gap: 6px;
}

.file-link {
  color: #1677ff;
  text-decoration: none;
}

.file-link:hover {
  text-decoration: underline;
}

.log-item {
  margin-bottom: 10px;
}

.log-item:last-child {
  margin-bottom: 0;
}

.log-type-tag {
  display: inline-block;
  background: rgba(0, 150, 255, 0.15);
  color: #1677ff;
  border: 1px solid rgba(0, 200, 255, 0.2);
  border-radius: 4px;
  padding: 1px 8px;
  margin-right: 6px;
  font-size: 11px;
  line-height: 1.5;
}

/* ============ 输入区域 ============ */
.chat-input-area {
  padding: 16px 24px;
  border-top: 1px solid rgba(0, 200, 255, 0.1);
  flex-shrink: 0;
  background: rgba(10, 15, 40, 0.5);
  backdrop-filter: blur(10px);
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
  background: rgba(20, 30, 60, 0.5);
  border: 1px solid rgba(0, 200, 255, 0.12);
  border-radius: 14px;
  padding: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  transition: border-color 0.3s;
}

.input-group:focus-within {
  border-color: rgba(0, 200, 255, 0.3);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2), 0 0 15px rgba(0, 150, 255, 0.1);
}

.message-input {
  background: rgba(10, 20, 40, 0.6) !important;
  border: 1px solid rgba(0, 200, 255, 0.1) !important;
  color: #d0dcff !important;
  border-radius: 10px !important;
  padding: 12px 14px !important;
  resize: none !important;
  text-align: left !important;
  min-height: 80px !important;
  max-height: 180px !important;
  transition: all 0.3s ease !important;
  font-size: 14px !important;
  line-height: 1.5 !important;
}

.message-input:hover {
  border-color: rgba(0, 200, 255, 0.2) !important;
}

.message-input:focus {
  border-color: rgba(0, 200, 255, 0.35) !important;
  box-shadow: 0 0 12px rgba(0, 150, 255, 0.08) !important;
}

:deep(.message-input .ant-input) {
  background: transparent !important;
  color: #d0dcff !important;
  resize: none !important;
  text-align: left !important;
  border: none !important;
  box-shadow: none !important;
  outline: none !important;
}

:deep(.message-input .ant-input::placeholder) {
  color: rgba(120, 150, 200, 0.4) !important;
}

:deep(.message-input .ant-input-clear-icon) {
  color: rgba(120, 150, 200, 0.4) !important;
}

:deep(.message-input .ant-input-clear-icon:hover) {
  color: rgba(160, 200, 240, 0.7) !important;
}

.input-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  padding-top: 8px;
  border-top: 1px solid rgba(0, 200, 255, 0.08);
}

.input-options-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

/* 联网搜索开关 */
:deep(.online-search-switch) {
  min-width: 90px;
  height: 28px;
}

:deep(.online-search-switch.ant-switch-checked) {
  background: #0066ff !important;
}

:deep(.online-search-switch.ant-switch-unchecked) {
  background: rgba(100, 120, 150, 0.3) !important;
}

/* 发送按钮 */
.send-button {
  border-radius: 8px !important;
  background: linear-gradient(135deg, #0066ff, #00aaff) !important;
  border: none !important;
  padding: 8px 24px !important;
  font-size: 14px !important;
  font-weight: 600 !important;
  color: white !important;
  box-shadow: 0 2px 12px rgba(0, 100, 255, 0.3) !important;
  transition: all 0.3s ease !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  gap: 6px !important;
  min-width: 90px !important;
}

.send-button:hover:not(:disabled) {
  background: linear-gradient(135deg, #0080ff, #00ccff) !important;
  box-shadow: 0 4px 20px rgba(0, 150, 255, 0.45) !important;
  transform: translateY(-1px) !important;
}

.send-button:disabled {
  background: rgba(0, 100, 255, 0.2) !important;
  box-shadow: none !important;
  cursor: not-allowed !important;
  opacity: 0.5 !important;
}

.send-button.stopping {
  background: linear-gradient(135deg, #f59e0b, #f97316) !important;
  box-shadow: 0 2px 12px rgba(245, 158, 11, 0.35) !important;
}

.send-button.stopping:hover:not(:disabled) {
  background: linear-gradient(135deg, #fbbf24, #fb923c) !important;
  box-shadow: 0 4px 20px rgba(245, 158, 11, 0.5) !important;
}

.uploaded-files-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.uploaded-files-bar .ant-tag {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background: rgba(0, 120, 255, 0.1);
  border: 1px solid rgba(0, 200, 255, 0.15);
  color: #1677ff;
}

.file-download-link {
  color: #1677ff;
  text-decoration: none;
}

.file-download-link:hover {
  text-decoration: underline;
}

/* ============ 占位区域（含AI机器人） ============ */
.chat-placeholder {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: radial-gradient(ellipse at center, rgba(0, 120, 255, 0.08) 0%, transparent 60%);
  gap: 30px;
  position: relative;
}

/* ============ AI 机器人 ============ */
.ai-robot {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  animation: robotFloat 3s ease-in-out infinite;
  filter: drop-shadow(0 0 30px rgba(0, 150, 255, 0.15));
}

@keyframes robotFloat {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-12px); }
}

.robot-body {
  position: relative;
  width: 140px;
  height: 220px;
}

/* 天线 */
.robot-antenna {
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 3px;
  height: 30px;
  background: linear-gradient(180deg, transparent, rgba(0, 200, 255, 0.3));
}

.antenna-line {
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 30px;
  background: linear-gradient(180deg, rgba(0, 200, 255, 0.4), transparent);
}

.antenna-dot {
  position: absolute;
  top: -5px;
  left: 50%;
  transform: translateX(-50%);
  width: 14px;
  height: 14px;
  background: #00e5ff;
  border-radius: 50%;
  box-shadow: 0 0 10px #00e5ff, 0 0 25px rgba(0, 229, 255, 0.5);
  animation: antennaGlow 1.5s ease-in-out infinite;
}

.antenna-glow {
  position: absolute;
  inset: -6px;
  border-radius: 50%;
  border: 2px solid rgba(0, 229, 255, 0.4);
  animation: antennaGlowRing 1.5s ease-in-out infinite;
}

@keyframes antennaGlow {
  0%, 100% { box-shadow: 0 0 10px #00e5ff, 0 0 25px rgba(0, 229, 255, 0.5); }
  50% { box-shadow: 0 0 18px #00e5ff, 0 0 40px rgba(0, 229, 255, 0.7), 0 0 60px rgba(0, 229, 255, 0.3); }
}

@keyframes antennaGlowRing {
  0%, 100% { transform: scale(1); opacity: 0.4; }
  50% { transform: scale(1.6); opacity: 0; }
}

/* 头部 */
.robot-head {
  position: absolute;
  top: 24px;
  left: 50%;
  transform: translateX(-50%);
  width: 70px;
  height: 60px;
  background: linear-gradient(180deg, #1e3050, #14223a);
  border-radius: 16px 16px 10px 10px;
  border: 2px solid rgba(0, 200, 255, 0.35);
  box-shadow: 0 0 20px rgba(0, 150, 255, 0.15), inset 0 0 15px rgba(0, 100, 200, 0.1);
  z-index: 2;
}

.head-top-bar {
  position: absolute;
  top: 6px;
  left: 50%;
  transform: translateX(-50%);
  width: 30px;
  height: 3px;
  background: rgba(0, 200, 255, 0.4);
  border-radius: 2px;
}

.robot-face {
  position: absolute;
  top: 14px;
  left: 0;
  right: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.robot-eye {
  position: absolute;
  top: 0;
  width: 14px;
  height: 14px;
  background: #0a1a30;
  border-radius: 50%;
  border: 2px solid rgba(0, 200, 255, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.left-eye { left: 12px; }
.right-eye { right: 12px; }

.eye-glow {
  width: 7px;
  height: 7px;
  background: #00e5ff;
  border-radius: 50%;
  box-shadow: 0 0 8px #00e5ff, 0 0 16px rgba(0, 229, 255, 0.5);
  animation: eyeShine 2s ease-in-out infinite;
}

@keyframes eyeShine {
  0%, 100% { opacity: 0.8; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.15); }
}

.robot-mouth {
  position: absolute;
  bottom: -22px;
  left: 50%;
  transform: translateX(-50%);
  width: 20px;
  height: 8px;
  overflow: hidden;
}

.mouth-wave {
  width: 20px;
  height: 4px;
  background: rgba(0, 200, 255, 0.5);
  border-radius: 0 0 10px 10px;
  animation: mouthTalk 2s ease-in-out infinite;
}

@keyframes mouthTalk {
  0%, 100% { height: 3px; border-radius: 0 0 10px 10px; }
  50% { height: 6px; border-radius: 0 0 6px 6px; }
}

/* 耳朵 */
.head-ear {
  position: absolute;
  top: 16px;
  width: 10px;
  height: 20px;
  background: linear-gradient(180deg, rgba(0, 200, 255, 0.2), rgba(0, 100, 200, 0.1));
  border-radius: 4px;
  border: 1px solid rgba(0, 200, 255, 0.2);
}

.left-ear { left: -8px; }
.right-ear { right: -8px; }

/* 脖子 */
.robot-neck {
  position: absolute;
  top: 86px;
  left: 50%;
  transform: translateX(-50%);
  width: 20px;
  height: 14px;
  background: linear-gradient(180deg, #14223a, #1a2a4a);
  border-left: 1px solid rgba(0, 200, 255, 0.2);
  border-right: 1px solid rgba(0, 200, 255, 0.2);
  z-index: 1;
}

.neck-ring {
  position: absolute;
  top: 4px;
  left: 50%;
  transform: translateX(-50%);
  width: 24px;
  height: 3px;
  background: rgba(0, 200, 255, 0.3);
  border-radius: 2px;
}

/* 身体 */
.robot-torso {
  position: absolute;
  top: 100px;
  left: 50%;
  transform: translateX(-50%);
  width: 80px;
  height: 70px;
  background: linear-gradient(180deg, #1a2a4a, #14223a);
  border-radius: 12px 12px 16px 16px;
  border: 2px solid rgba(0, 200, 255, 0.3);
  box-shadow: 0 0 20px rgba(0, 150, 255, 0.1);
  z-index: 1;
}

.torso-core {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 36px;
  height: 36px;
}

.core-ring {
  position: absolute;
  inset: 0;
  border-radius: 50%;
  border: 2px solid rgba(0, 200, 255, 0.3);
}

.core-ring-1 {
  animation: coreSpin 3s linear infinite;
}

.core-ring-2 {
  inset: 4px;
  border-color: rgba(0, 200, 255, 0.5);
  animation: coreSpin 2s linear infinite reverse;
}

.core-center {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 10px;
  height: 10px;
  background: #00e5ff;
  border-radius: 50%;
  box-shadow: 0 0 15px #00e5ff, 0 0 30px rgba(0, 229, 255, 0.5);
  animation: corePulse 1.5s ease-in-out infinite;
}

@keyframes coreSpin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes corePulse {
  0%, 100% { box-shadow: 0 0 12px #00e5ff, 0 0 24px rgba(0, 229, 255, 0.4); }
  50% { box-shadow: 0 0 20px #00e5ff, 0 0 40px rgba(0, 229, 255, 0.6); }
}

.torso-chest-line {
  position: absolute;
  left: 8px;
  right: 8px;
  height: 1px;
  background: rgba(0, 200, 255, 0.15);
}

.torso-chest-line:first-of-type { top: 14px; }
.torso-chest-line.line-2 { bottom: 14px; }

/* 手臂 */
.robot-arm {
  position: absolute;
  top: 108px;
  width: 14px;
  z-index: 0;
}

.left-arm { left: 20px; }
.right-arm { right: 20px; }

.arm-upper {
  width: 14px;
  height: 26px;
  background: linear-gradient(90deg, #1a2a4a, #14223a);
  border-radius: 7px;
  border: 1px solid rgba(0, 200, 255, 0.2);
}

.arm-lower {
  width: 12px;
  height: 22px;
  background: linear-gradient(90deg, #14223a, #0f1a30);
  border-radius: 6px;
  border: 1px solid rgba(0, 200, 255, 0.15);
  margin: 2px auto 0;
}

.arm-hand {
  width: 16px;
  height: 16px;
  background: #1a2a4a;
  border-radius: 50%;
  border: 1.5px solid rgba(0, 200, 255, 0.3);
  margin: 2px auto 0;
  box-shadow: 0 0 8px rgba(0, 150, 255, 0.15);
}

/* 悬浮底座 */
.robot-base {
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 100px;
  height: 10px;
  z-index: 0;
}

.base-ring {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: rgba(0, 200, 255, 0.3);
  border-radius: 50%;
  box-shadow: 0 0 15px rgba(0, 200, 255, 0.3);
  animation: baseGlow 2s ease-in-out infinite;
}

.base-ring-2 {
  bottom: 6px;
  height: 2px;
  opacity: 0.5;
  animation: baseGlow 2s ease-in-out infinite 0.5s;
}

@keyframes baseGlow {
  0%, 100% { box-shadow: 0 0 10px rgba(0, 200, 255, 0.3); }
  50% { box-shadow: 0 0 20px rgba(0, 200, 255, 0.5); }
}

.robot-hover-glow {
  position: absolute;
  bottom: -20px;
  left: 50%;
  transform: translateX(-50%);
  width: 120px;
  height: 30px;
  background: radial-gradient(ellipse, rgba(0, 150, 255, 0.15) 0%, transparent 70%);
  animation: hoverGlow 2s ease-in-out infinite;
}

@keyframes hoverGlow {
  0%, 100% { opacity: 0.4; transform: translateX(-50%) scale(1); }
  50% { opacity: 0.7; transform: translateX(-50%) scale(1.2); }
}

/* ============ 占位文字 ============ */
.placeholder-content {
  text-align: center;
}

.placeholder-title {
  font-size: 22px;
  font-weight: 600;
  background: linear-gradient(135deg, #1677ff, #00c8ff);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0 0 8px 0;
  letter-spacing: 1px;
}

.placeholder-sub {
  font-size: 14px;
  color: rgba(130, 170, 220, 0.7);
  margin: 0 0 24px 0;
}

.add-button-large {
  background: linear-gradient(135deg, #0066ff, #00aaff) !important;
  border: none !important;
  border-radius: 10px !important;
  padding: 6px 28px !important;
  font-size: 15px !important;
  box-shadow: 0 0 20px rgba(0, 100, 255, 0.3) !important;
  transition: all 0.3s ease !important;
}

.add-button-large:hover {
  background: linear-gradient(135deg, #0080ff, #00ccff) !important;
  box-shadow: 0 0 30px rgba(0, 150, 255, 0.5) !important;
  transform: translateY(-2px) !important;
}

/* ============ 语音模式样式 ============ */

/* 语音模式开关 */
:deep(.voice-mode-switch) {
  min-width: 90px;
  height: 28px;
}

:deep(.voice-mode-switch.ant-switch-checked) {
  background: #10b981 !important;
}

:deep(.voice-mode-switch.ant-switch-unchecked) {
  background: rgba(100, 120, 150, 0.3) !important;
}

:deep(.text-display-switch) {
  min-width: 75px;
  height: 28px;
}

:deep(.text-display-switch.ant-switch-checked) {
  background: #3b82f6 !important;
}

:deep(.text-display-switch.ant-switch-unchecked) {
  background: rgba(100, 120, 150, 0.3) !important;
}

/* 语音录制区域 */
/* 语音模式输入区域 */
.voice-input-area {
  flex: 1;
  min-height: 80px;
  padding: 12px 16px;
  background: rgba(0, 200, 255, 0.05);
  border: 1px dashed rgba(0, 200, 255, 0.2);
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  margin-bottom: 10px;
}

.voice-hint-text {
  color: rgba(160, 200, 240, 0.5);
  font-size: 13px;
  text-align: center;
}

.voice-preview {
  display: flex;
  flex-direction: column;
  gap: 4px;
  width: 100%;
}

.voice-preview-label {
  font-size: 12px;
  color: rgba(0, 200, 255, 0.6);
}

.voice-preview-text {
  font-size: 14px;
  color: #c8d6ff;
  line-height: 1.5;
  max-height: 80px;
  overflow-y: auto;
  word-break: break-all;
}

.voice-sent-hint {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: rgba(0, 200, 255, 0.8);
  animation: fadeInUp 0.3s ease;
}

.voice-sent-icon.success {
  color: #52c41a;
  font-size: 20px;
}

.voice-sent-icon.warning {
  color: #faad14;
  font-size: 20px;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.voice-recorder-area {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 4px 0;
}

.voice-recorder-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
}

.voice-record-btn {
  height: 44px;
  width: 100%;
  max-width: 280px;
  border-radius: 22px;
  background: rgba(0, 200, 255, 0.1);
  border: 1.5px solid rgba(0, 200, 255, 0.3);
  color: #c8d6ff;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  cursor: pointer;
  user-select: none;
  -webkit-user-select: none;
  -webkit-touch-callout: none;
}

.voice-record-btn:hover {
  background: rgba(0, 200, 255, 0.18);
  border-color: rgba(0, 200, 255, 0.5);
  box-shadow: 0 0 15px rgba(0, 200, 255, 0.2);
}

.voice-record-btn:active {
  transform: scale(0.98);
}

.voice-record-icon {
  font-size: 18px;
}

.voice-record-text {
  letter-spacing: 1px;
}

.voice-record-btn.recording {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.2), rgba(239, 68, 68, 0.3));
  border-color: rgba(239, 68, 68, 0.5);
  color: #fca5a5;
  animation: voicePulse 1.2s ease-in-out infinite;
  box-shadow: 0 0 20px rgba(239, 68, 68, 0.3);
}

@keyframes voicePulse {
  0%, 100% {
    box-shadow: 0 0 10px rgba(239, 68, 68, 0.3);
  }
  50% {
    box-shadow: 0 0 25px rgba(239, 68, 68, 0.5), 0 0 40px rgba(239, 68, 68, 0.2);
  }
}

/* 录音指示器 */
.recording-indicator {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 16px;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 20px;
}

.recording-dot {
  width: 8px;
  height: 8px;
  background: #ef4444;
  border-radius: 50%;
  animation: recordingBlink 0.8s ease-in-out infinite;
}

.recording-dot.recognizing {
  background: #3b82f6;
  animation: recognizingSpin 1s linear infinite;
}

@keyframes recognizingSpin {
  0% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.3); opacity: 0.6; }
  100% { transform: scale(1); opacity: 1; }
}

@keyframes recordingBlink {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.3; }
}

.recording-text {
  color: #fca5a5;
  font-size: 12px;
  font-weight: 500;
}

.recording-dot.recognizing ~ .recording-text,
.recording-indicator:has(.recognizing) .recording-text {
  color: #93c5fd;
}

/* 识别中按钮样式 */
.voice-record-btn.recognizing {
  background: rgba(59, 130, 246, 0.15);
  border-color: rgba(59, 130, 246, 0.5);
  cursor: default;
}

.voice-record-btn.recognizing:hover {
  background: rgba(59, 130, 246, 0.15);
  box-shadow: none;
}

/* AI 语音条 */
.voice-msg-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 8px;
  padding: 10px 16px;
  background: rgba(0, 200, 255, 0.08);
  border: 1px solid rgba(0, 200, 255, 0.2);
  border-radius: 12px;
  transition: all 0.3s ease;
  width: fit-content;
  user-select: none;
  touch-action: none; /* 禁止浏览器滚动/缩放手势, 确保长按稳定触发 */
  -webkit-touch-callout: none; /* 禁止iOS长按弹出菜单 */
}

/* 流式播放中的语音条 */
.voice-msg-bar-streaming {
  animation: voiceBarPulse 1.5s ease-in-out infinite;
}

@keyframes voiceBarPulse {
  0%, 100% { box-shadow: 0 0 8px rgba(0, 200, 255, 0.15); }
  50% { box-shadow: 0 0 16px rgba(0, 200, 255, 0.35); }
}

.voice-msg-bar:hover {
  background: rgba(0, 200, 255, 0.15);
  border-color: rgba(0, 200, 255, 0.4);
  box-shadow: 0 0 12px rgba(0, 200, 255, 0.15);
}

.voice-msg-left {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  flex-shrink: 0;
}

.voice-msg-icon {
  font-size: 18px;
  color: #00c8ff;
  flex-shrink: 0;
}

.voice-msg-wave {
  display: flex;
  align-items: center;
  gap: 2px;
  height: 20px;
}

.voice-wave-bar {
  display: inline-block;
  width: 3px;
  height: 8px;
  background: #00c8ff;
  border-radius: 2px;
  /* 默认静止状态, 无动画 */
}

.voice-wave-bar:nth-child(odd) {
  height: 14px;
}

/* 仅在播放时启动动画 */
.voice-msg-wave.playing .voice-wave-bar {
  animation: waveAnim 0.8s ease-in-out infinite;
}

@keyframes waveAnim {
  0%, 100% {
    height: 6px;
  }
  50% {
    height: 18px;
  }
}

.voice-msg-duration {
  font-size: 11px;
  color: rgba(160, 200, 240, 0.7);
  flex-shrink: 0;
}

/* 倍速选择器 */
.voice-speed-selector {
  display: flex;
  align-items: center;
  gap: 2px;
  padding: 2px;
  background: rgba(0, 150, 255, 0.1);
  border-radius: 8px;
  flex-shrink: 0;
}

/* 全局倍速选择器（工具栏内） */
.global-speed-selector {
  padding: 3px;
  background: rgba(0, 150, 255, 0.12);
  border: 1px solid rgba(0, 200, 255, 0.15);
  border-radius: 10px;
}

.global-speed-selector .speed-option {
  padding: 3px 10px;
  font-size: 12px;
  line-height: 1.4;
}

.speed-option {
  padding: 2px 7px;
  font-size: 11px;
  color: rgba(160, 200, 240, 0.6);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
  font-weight: 500;
  line-height: 1.6;
}

.speed-option:hover {
  color: #c8d6ff;
  background: rgba(0, 200, 255, 0.15);
}

.speed-option.active {
  color: #00e5ff;
  background: rgba(0, 200, 255, 0.25);
  font-weight: 600;
}

/* 语音模式下展开的文字 */
.voice-expanded-text {
  margin-top: 8px;
  padding: 12px 16px;
  background: rgba(0, 200, 255, 0.05);
  border: 1px solid rgba(0, 200, 255, 0.15);
  border-radius: 12px;
  animation: slideDown 0.2s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-6px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* ============ 响应式 ============ */
@media (max-width: 768px) {
  .chat-layout {
    flex-direction: column;
  }

  .chat-sidebar {
    width: 100%;
    border-right: none;
    border-bottom: 1px solid rgba(0, 200, 255, 0.1);
    max-height: 200px;
  }

  .chat-input-area {
    padding: 12px;
  }

  .input-group {
    padding: 12px;
  }

  .message-input {
    min-height: 60px !important;
  }

  .send-button {
    padding: 6px 18px !important;
    min-width: 70px !important;
  }

  .input-options {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }

  .ai-robot {
    transform: scale(0.7);
  }
}

/* ============ 全局发光动画 ============ */
@keyframes eyeGlow {
  0%, 100% { box-shadow: 0 0 6px #00e5ff; }
  50% { box-shadow: 0 0 12px #00e5ff, 0 0 20px rgba(0, 229, 255, 0.4); }
}
</style>