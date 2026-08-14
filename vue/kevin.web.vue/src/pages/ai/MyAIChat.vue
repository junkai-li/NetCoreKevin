<template>
  <div class="ai-chat-container">
    <!-- 粒子背景 -->
    <div class="particles-bg">
      <div v-for="(style, i) in particleStyles" :key="'p'+i" class="particle" :style="style"></div>
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
                  :class="['voice-record-btn', { recording: isRecording, recognizing: isRecognizing, disabled: isSending }]"
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
              <div class="asr-provider-tag">{{ asrProviderLabel }}</div>
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
                <a-button
                  class="phone-mode-btn"
                  @click="enterPhoneMode"
                  size="small"
                >
                  <template #icon>
                    <PhoneOutlined />
                  </template>
                  电话
                </a-button>
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
          <div class="detail-modal-body">{{ detailModalContent }}</div>
        </a-modal>
      </div>
    </div>

    </div>

    <!-- 电话模式全屏覆盖 -->
    <Teleport to="body">
      <div v-if="isPhoneMode && !phoneModeCollapsed" class="phone-mode-overlay">
        <div class="phone-call-container">
          <!-- 收起按钮：隐藏全屏覆盖层，后台保持聆听 -->
          <div class="phone-collapse-btn" @click="phoneModeCollapsed = true">
            <span class="phone-collapse-text">收起</span>
          </div>
          <!-- 字幕按钮 -->
          <div class="phone-subtitle-btn" :class="{ active: phoneSubtitleEnabled }" @click="phoneSubtitleEnabled = !phoneSubtitleEnabled">
            <span class="phone-subtitle-btn-text">字幕</span>
          </div>
          <!-- 顶部：通话计时 -->
          <div class="phone-timer">
            <span class="phone-timer-text">{{ formatPhoneCallDuration(phoneCallDuration) }}</span>
          </div>

          <!-- 中部：AI头像 + 名称 + 状态 + 波形 -->
          <div class="phone-main">
            <div class="phone-avatar">
              <div class="phone-avatar-ring" :class="{ active: isSpeaking }"></div>
              <div class="ai-robot-mini phone-robot">
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
            <div class="phone-name">{{ getAiAppName(activeConversation?.appId) || 'AI 助手' }}</div>
            <div class="phone-status">{{ phoneStatusText }}</div>
            <div class="phone-asr-tag">{{ asrProviderLabel }}</div>
            <!-- 播放波形（AI说话时显示） -->
            <div class="phone-wave" v-if="isSpeaking">
              <span v-for="i in 24" :key="i" class="phone-wave-bar" :style="{ animationDelay: (i * 0.05) + 's' }"></span>
            </div>
          </div>

          <!-- 底部：识别预览 + 麦克风指示器 + 挂断 -->
          <div class="phone-controls">
            <div class="phone-recognize-preview" v-if="isRecording || isRecognizing">
              {{ recognizedPreviewText || '...' }}
            </div>
            <div class="phone-buttons">
              <!-- 麦克风指示器（自动监听中） -->
              <div class="phone-mic-wrapper">
                <div :class="['phone-mic-indicator', { listening: isRecording }]">
                  <AudioOutlined class="phone-mic-icon" />
                </div>
                <span class="phone-mic-label">{{ isRecording ? '聆听中' : (isSending || isSpeaking ? '等待中' : '准备中') }}</span>
              </div>
              <!-- 手动发送按钮 -->
              <div class="phone-send-btn" @click="sendPhoneMessage" v-if="isRecording">
                <SendOutlined class="phone-send-icon" />
              </div>
              <!-- 挂断按钮 -->
              <div class="phone-hangup-btn" @click="hangUpPhone">
                <PhoneOutlined class="phone-hangup-icon" />
              </div>
            </div>
          </div>
          <!-- 字幕面板（提升到 phone-call-container 层级） -->
          <div v-if="phoneSubtitleEnabled" class="phone-subtitle-panel" ref="phoneSubtitlePanel">
            <div v-if="phoneMessages.length === 0" class="phone-subtitle-empty">开始说话后，字幕将显示在这里</div>
            <div v-for="(msg) in phoneMessages" :key="msg.id" :class="['phone-subtitle-item', msg.isSend ? 'user' : 'ai']">
              <span class="phone-subtitle-role">{{ msg.isSend ? '你' : 'AI' }}</span>
              <span class="phone-subtitle-content">{{ msg.content }}</span>
            </div>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- 电话模式收起后的悬浮恢复按钮：点击恢复全屏 -->
    <Teleport to="body">
      <div v-if="isPhoneMode && phoneModeCollapsed" class="phone-restore-btn" @click="phoneModeCollapsed = false">
        <PhoneOutlined class="phone-restore-icon" :class="{ 'restore-pulse': isSpeaking }" />
        <div class="phone-restore-info">
          <span class="phone-restore-name">{{ getAiAppName(activeConversation?.appId) || 'AI 助手' }}</span>
          <span class="phone-restore-text">{{ isSpeaking ? '播放中' : (isRecording ? '聆听中' : '电话中') }}</span>
        </div>
        <span class="phone-restore-dot" :class="{ active: isRecording }"></span>
      </div>
    </Teleport>
</template>

<script setup>
/* eslint-disable */
import { ref, onMounted, nextTick, watch, h, onUnmounted, computed } from "vue";
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
  SendOutlined,
  AudioOutlined,
  SoundOutlined,
  CheckCircleFilled,
  InfoCircleFilled,
  PhoneOutlined,
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
import { getAliAsrToken } from '../../api/ai/aliasr.js';
import { AliAsrAdapter } from '../../utils/aliAsrAdapter.js';
// 模拟数据
const conversations = ref([]);
const activeConversationId = ref(null);
const activeConversation = ref(null);
const messages = ref([]);
const newMessage = ref("");
const loadingConversations = ref(false);
const isSending = ref(false);
const messagesContainer = ref(null);

// 粒子背景样式预计算（避免每次渲染 Math.random() 触发 Vue 重算）
const particleStyles = Array.from({ length: 30 }, () => ({
  left: (Math.random() * 100) + '%',
  top: (Math.random() * 100) + '%',
  animationDelay: (Math.random() * 5) + 's',
  animationDuration: (3 + Math.random() * 6) + 's',
  width: (2 + Math.random() * 3) + 'px',
  height: (2 + Math.random() * 3) + 'px',
}));
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

// 阿里云实时 ASR 配置
const aliAsrAppKey = ref(''); // 阿里云语音识别 AppKey（从后端获取）
const aliAsrToken = ref(''); // 阿里云临时 Token
const aliAsrTokenExpiry = ref(0); // Token 过期时间戳
const aliAsrFailed = ref(false); // 阿里云 ASR 连接失败后，不再重试，直接走浏览器兜底

// 当前使用的 ASR 引擎（显示用）
const asrProviderLabel = computed(() => {
  if (aliAsrFailed.value) return '浏览器(阿里云降级)';
  return aliAsrToken.value && aliAsrAppKey.value ? '阿里云 ASR' : '浏览器';
});
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

// ========== 电话模式 ==========
const isPhoneMode = ref(false); // 电话模式开关
const phoneModeCollapsed = ref(false); // 电话模式全屏收起标志（收起后后台保持聆听，仅隐藏全屏覆盖层）
const phoneSubtitleEnabled = ref(false); // 字幕开关
const phoneCallStartMsgId = ref(null); // 进入电话模式时最后一条消息的 ID，用于筛选本次通话的消息
// 电话模式字幕：直接复用 messages，与聊天记录保持一致
const phoneMessages = computed(() => {
  if (!phoneCallStartMsgId.value) return messages.value;
  const startIdx = messages.value.findIndex(m => m.id === phoneCallStartMsgId.value);
  return startIdx >= 0 ? messages.value.slice(startIdx + 1) : messages.value;
});
const phoneSubtitlePanel = ref(null); // 字幕面板 DOM 引用
const previousVoiceMode = ref(false); // 进入电话模式前 isVoiceMode 的原值
const phoneCallDuration = ref(0); // 通话时长（秒）
let phoneCallTimer = null; // 通话计时句柄
const PHONE_SILENCE_THRESHOLD = 3000; // 静默3秒后自动发送
let phoneSilenceTimer = null; // 静默检测定时器
let phoneLastSpeechTime = 0; // 最后一次检测到语音的时间

// 通话状态文字（根据录音/识别/发送/播放状态自动计算）
const phoneStatusText = computed(() => {
  if (isRecording.value) return '正在聆听...';
  if (isRecognizing.value) return '识别中...';
  if (isSending.value && !isSpeaking.value) return 'AI 思考中...';
  if (isSpeaking.value) return '正在回复...';
  return '通话中';
});

// ========== 语音功能 ==========

// onresult 节流机制：Chrome interimResults 每 50-100ms 触发一次 onresult，
// 直接更新 Vue ref 会导致大量 DOM 重渲染 → 用户感知卡顿。
// 使用 requestAnimationFrame 合并多次 onresult 的更新，每帧最多触发一次 DOM 更新。
let previewTextBuffer = '';      // 缓存最新的识别文本
let previewTextRafId = null;     // requestAnimationFrame 句柄
let lastPreviewText = '';        // 上一次已渲染的文本（用于对比变化）

const flushPreviewText = () => {
  previewTextRafId = null;
  if (previewTextBuffer !== lastPreviewText) {
    lastPreviewText = previewTextBuffer;
    recognizedPreviewText.value = previewTextBuffer;
  }
};

const updatePreviewText = (text) => {
  if (text === previewTextBuffer) return; // 文本未变化，跳过
  previewTextBuffer = text;
  if (!previewTextRafId) {
    previewTextRafId = requestAnimationFrame(flushPreviewText);
  }
};

const resetPreviewTextBuffer = () => {
  previewTextBuffer = '';
  lastPreviewText = '';
  if (previewTextRafId) {
    cancelAnimationFrame(previewTextRafId);
    previewTextRafId = null;
  }
  recognizedPreviewText.value = '';
};

// 获取阿里云 ASR Token（过期自动刷新）
const fetchAliAsrToken = async () => {
  const now = Date.now();
  if (aliAsrToken.value && now < aliAsrTokenExpiry.value - 60000) {
    return; // Token 还有效（提前 1 分钟刷新）
  }
  try {
    const res = await getAliAsrToken();
    if (res.code === 200 && res.data) {
      aliAsrAppKey.value = res.data.appKey || '';
      aliAsrToken.value = res.data.token || '';
      // 使用后端返回的过期时间，兜底 24 小时
      aliAsrTokenExpiry.value = res.data.expireTime
        ? Number(res.data.expireTime) * 1000
        : now + 23 * 60 * 60 * 1000;
    }
  } catch (err) {
    console.error('获取阿里云 ASR Token 失败:', err);
  }
};

// 初始化语音识别（阿里云实时 ASR，兜底浏览器 SpeechRecognition）
const initSpeechRecognition = () => {
  // 优先使用阿里云实时 ASR，如果已失败则跳过
  if (!aliAsrFailed.value && aliAsrToken.value && aliAsrAppKey.value) {
    return new AliAsrAdapter({
      appKey: aliAsrAppKey.value,
      token: aliAsrToken.value,
    });
  }
  // 兜底：浏览器原生 SpeechRecognition
  const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
  if (!SpeechRecognition) {
    message.warning('您的浏览器不支持语音识别，请使用 Chrome 浏览器');
    return null;
  }
  if (!window.isSecureContext) {
    message.error('语音识别需要 HTTPS 环境或 localhost 访问');
    return null;
  }
  const rec = new SpeechRecognition();
  rec.lang = 'zh-CN';
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
  // 先清理上一轮残留实例，避免旧实例占用语音服务/回调误触发导致识别失效
  if (recognition.value) {
    const old = recognition.value;
    try { old.onend = null; old.onerror = null; old.onresult = null; old.abort(); } catch (e) { /* ignore */ }
  }
  recognition.value = initSpeechRecognition();
  if (!recognition.value) return;

  isRecording.value = true;
  isRecognizing.value = false;
  resetPreviewTextBuffer();
  newMessage.value = '';
  recordingStartTime = Date.now();

  recognition.value.onresult = (event) => {
    let finalText = '';
    let interimText = '';
    for (let i = 0; i < event.results.length; i++) {
      const result = event.results[i];
      if (result.isFinal) {
        finalText += result[0].transcript;
      } else {
        interimText = result[0].transcript;
      }
    }
    updatePreviewText(finalText + interimText);
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
    resetPreviewTextBuffer();
    voiceSentHint.value = '说话时间太短';
    setTimeout(() => { voiceSentHint.value = ''; }, PHONE_SILENCE_THRESHOLD);
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
      // 立即刷新 buffer，确保读取到最新的识别文本
      if (previewTextRafId) {
        cancelAnimationFrame(previewTextRafId);
        previewTextRafId = null;
      }
      flushPreviewText();
      const text = recognizedPreviewText.value.trim();
      if (text) {
        newMessage.value = text;
        resetPreviewTextBuffer();
        voiceSentHint.value = '已发送 ✓';
        setTimeout(() => { voiceSentHint.value = ''; }, PHONE_SILENCE_THRESHOLD);
        sendMessage();
      } else {
        resetPreviewTextBuffer();
        voiceSentHint.value = '未识别到内容，已取消';
        setTimeout(() => { voiceSentHint.value = ''; }, PHONE_SILENCE_THRESHOLD);
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
  resetPreviewTextBuffer();
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

// ========== 电话模式功能 ==========

// 重置电话模式的播放/识别/计时状态（不切换 isPhoneMode / isVoiceMode）
// 供 hangUpPhone 退出时调用，以及 enterPhoneMode 在"已在电话模式中再点电话"时复用
const resetPhoneCallState = () => {
  stopPhoneAutoListen();
  if (isSending.value) {
    stopMessage();
  }
  // 停止 TTS 播放
  staticPlaybackGen++;
  window.speechSynthesis.cancel();
  streamingTtsActive = false;
  streamingTtsPlayedLength = 0;
  staticPlaybackMsgId = null;
  staticSentenceList = [];
  staticSentenceIndex = 0;
  resetSpeakingState();
  userStoppedSending = false;
  stopPhoneCallTimer();
};

// 进入电话模式
const enterPhoneMode = async () => {
  if (!activeConversation.value) {
    message.warning('请先选择一个对话');
    return;
  }
  if (isPhoneMode.value) {
    // 已在电话模式中（例如收起后切换了对话再点电话）：先彻底重置识别/TTS/计时，再为当前对话重新发起
    resetPhoneCallState();
  } else {
    // 首次进入：记住之前的语音模式状态
    previousVoiceMode.value = isVoiceMode.value;
    // 启用语音模式，复用全部 TTS/录音/自动播放逻辑
    // watch(isVoiceMode) 会自动解锁语音合成 + 申请麦克风权限
    isVoiceMode.value = true;
    isPhoneMode.value = true;
  }
  phoneModeCollapsed.value = false;
  phoneCallDuration.value = 0;
  phoneCallStartMsgId.value = messages.value.length > 0 ? messages.value[messages.value.length - 1].id : null;
  phoneSubtitleEnabled.value = false;
  startPhoneCallTimer();
  setTimeout(() => {
    if (isPhoneMode.value) {
      startPhoneAutoListen();
    }
  }, 1000);
};

// 通话计时开始
const startPhoneCallTimer = () => {
  stopPhoneCallTimer();
  phoneCallTimer = setInterval(() => {
    phoneCallDuration.value++;
  }, 1000);
};

// 通话计时停止
const stopPhoneCallTimer = () => {
  if (phoneCallTimer) {
    clearInterval(phoneCallTimer);
    phoneCallTimer = null;
  }
};

// 格式化通话时长
const formatPhoneCallDuration = (seconds) => {
  const h = Math.floor(seconds / 3600);
  const m = Math.floor((seconds % 3600) / 60);
  const s = seconds % 60;
  const pad = (n) => String(n).padStart(2, '0');
  return h > 0 ? `${pad(h)}:${pad(m)}:${pad(s)}` : `${pad(m)}:${pad(s)}`;
};

// 电话模式：启动自动监听（实时对话，无需按住）
const startPhoneAutoListen = () => {
  // 播放中不监听，播放结束后由 watch(isSpeaking) 重新启动
  if (!isPhoneMode.value) return;
  if (isRecording.value) return; // 已在监听中，防止重复启动
  if (!micPermissionGranted.value) {
    // 等待权限获取后重试
    setTimeout(() => {
      if (isPhoneMode.value) startPhoneAutoListen();
    }, 1000);
    return;
  }

  unlockSpeechSynthesis();

  // 清理上一轮残留的识别实例：解除回调并 abort，避免旧实例 onend 误重启新实例、
  // 或旧实例占用语音服务导致几轮后识别失效（Chrome Web Speech 多实例互相干扰）
  if (recognition.value) {
    const old = recognition.value;
    try { old.onend = null; old.onerror = null; old.onresult = null; old.abort(); } catch (e) { /* ignore */ }
  }

  const rec = initSpeechRecognition();
  if (!rec) return;
  recognition.value = rec;

  isRecording.value = true;
  isRecognizing.value = false;
  resetPreviewTextBuffer();
  phoneLastSpeechTime = Date.now();

  rec.onresult = (event) => {
    phoneLastSpeechTime = Date.now();

    // 从完整的 results 数组重建文本（兼容原生 SpeechRecognition 累积和 AliAsrAdapter 替换两种模式）
    let finalText = '';
    let interimText = '';
    for (let i = 0; i < event.results.length; i++) {
      const result = event.results[i];
      if (result.isFinal) {
        finalText += result[0].transcript;
      } else {
        interimText = result[0].transcript;
      }
    }
    const fullText = finalText + interimText;
    updatePreviewText(fullText);

    // 检测到句末标点（。！？.!?）的 final 结果时立即发送，不等静默超时
    // 使用 fullText（累积全部文本）而非 lastResult 单句，因为长句可能被 VAD 切分，
    // 最后一句不带标点但整体文本末尾有标点
    const lastResult = event.results[event.results.length - 1];
    if (lastResult && lastResult.isFinal && /[。！？.!?]\s*$/.test(fullText)) {
      // 立即刷新 buffer，确保 recognizedPreviewText 是最新的
      if (previewTextRafId) {
        cancelAnimationFrame(previewTextRafId);
        previewTextRafId = null;
      }
      flushPreviewText();
      // 防重复：避免在通话中、TTS 播放中状态下重复触发
      if (!isPhoneMode.value || isSpeaking.value || window.speechSynthesis.speaking) return;
      const text = recognizedPreviewText.value.trim();
      if (text) {
        sendPhoneMessage();
      }
    }
  };

  rec.onerror = (event) => {
    console.error('语音识别错误:', event.error);
    if (event.error === 'aborted' || event.error === 'no-speech') return;
    if (event.error === 'not-allowed' || event.error === 'service-not-allowed') {
      message.error('麦克风权限被拒绝');
    } else {
      message.error('语音识别失败: ' + (event.error || '未知错误'));
    }
    isRecording.value = false;
    // 电话模式下识别意外失败，自动重建实例恢复监听
    if (isPhoneMode.value) {
      setTimeout(() => {
        if (isPhoneMode.value) startPhoneAutoListen();
      }, 300);
    }
  };

  rec.onend = () => {
    // 旧实例（已被新实例取代）的 onend：直接返回
    if (recognition.value !== rec) return;
    // 电话模式下自动重启识别（浏览器可能自动停止）
    if (isPhoneMode.value && isRecording.value) {
      // 不调用 rec.start() —— AliAsrAdapter._fireEnd() 已把 this.onend 置 null，
      // 复用旧实例会导致新 ASR 的错误/结束事件无法上报，识别静默死亡。
      isRecording.value = false;
      startPhoneAutoListen();
    } else {
      isRecording.value = false;
    }
  };

  try {
    rec.start();
  } catch (err) {
    console.error('启动语音识别失败:', err);
    isRecording.value = false;
    setTimeout(() => {
      if (isPhoneMode.value && !isSpeaking.value) {
        startPhoneAutoListen();
      }
    }, 500);
  }

  // 启动静默检测：用户停说话1.8秒后自动发送（TTS 播放中不发送）
  stopPhoneSilenceTimer();
  phoneSilenceTimer = setInterval(() => {
    if (!isPhoneMode.value) {
      stopPhoneSilenceTimer();
      return;
    }
    // TTS 播放中不发送消息（播放时录音已关闭，此检查作为安全兜底）
    if (isSpeaking.value || window.speechSynthesis.speaking || window.speechSynthesis.pending) {
      return;
    }
    const silenceDuration = Date.now() - phoneLastSpeechTime;
    // 立即刷新 buffer，确保检测到最新的识别文本
    if (previewTextRafId) {
      cancelAnimationFrame(previewTextRafId);
      previewTextRafId = null;
    }
    flushPreviewText();
    if (silenceDuration > PHONE_SILENCE_THRESHOLD && recognizedPreviewText.value.trim()) {
      sendPhoneMessage();
    }
  }, 300);
};

// 停止静默检测定时器
const stopPhoneSilenceTimer = () => {
  if (phoneSilenceTimer) {
    clearInterval(phoneSilenceTimer);
    phoneSilenceTimer = null;
  }
};

// 停止电话模式自动监听
const stopPhoneAutoListen = () => {
  stopPhoneSilenceTimer();
  isRecording.value = false;
  isRecognizing.value = false;
  if (recognition.value) {
    try { recognition.value.abort(); } catch (e) { /* ignore */ }
  }
  resetPreviewTextBuffer();
};

// 电话模式：静默触发后自动发送消息
const sendPhoneMessage = () => {
  if (previewTextRafId) {
    cancelAnimationFrame(previewTextRafId);
    previewTextRafId = null;
  }
  flushPreviewText();
  let text = recognizedPreviewText.value.trim();

  // 如果正在发送中，中断上一次并把新旧文本合并一起发送
  if (isSending.value) {
    const prevText = lastSentMessage.value || '';
    stopMessage();
    text = (prevText + text).trim();
  }

  if (phoneSubtitleEnabled.value) {
    // 字幕模式：不停止识别，重置缓冲区让新的识别结果重新累积，实现连续聆听
    if (recognition.value && recognition.value.resetBuffer) {
      recognition.value.resetBuffer();
    }
    resetPreviewTextBuffer();
    phoneLastSpeechTime = Date.now(); // 重置静默计时，避免立即触发再次发送
    // 重新启动静默检测，监听下一段语音
    stopPhoneSilenceTimer();
    phoneSilenceTimer = setInterval(() => {
      if (!isPhoneMode.value) {
        stopPhoneSilenceTimer();
        return;
      }
      if (isSpeaking.value || window.speechSynthesis.speaking || window.speechSynthesis.pending) {
        return;
      }
      const silenceDuration = Date.now() - phoneLastSpeechTime;
      if (previewTextRafId) {
        cancelAnimationFrame(previewTextRafId);
        previewTextRafId = null;
      }
      flushPreviewText();
      if (silenceDuration > PHONE_SILENCE_THRESHOLD && recognizedPreviewText.value.trim()) {
        sendPhoneMessage();
      }
    }, 300);
  } else {
    // 非字幕模式：停止识别，等 AI 回复播报完后再恢复
    stopPhoneSilenceTimer();
    isRecording.value = false;
    if (recognition.value) {
      try { recognition.value.stop(); } catch (e) { /* ignore */ }
    }
    resetPreviewTextBuffer();
  }

  if (!text) {
    if (!phoneSubtitleEnabled.value) {
      setTimeout(() => {
        if (isPhoneMode.value) startPhoneAutoListen();
      }, 300);
    }
    return;
  }

  // 正常发送
  newMessage.value = text;
  sendMessage();
  // 字幕模式下识别已在运行，无需恢复；非字幕模式等 watch(isSpeaking) 播完恢复
};

// 挂断电话
const hangUpPhone = () => {
  resetPhoneCallState();
  // 退出电话模式
  isPhoneMode.value = false;
  phoneModeCollapsed.value = false;
  // 恢复之前的语音模式状态
  isVoiceMode.value = previousVoiceMode.value;
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

  // 打印可用语音用于调试
  console.log('[TTS] 可用语音列表:');
  voices.forEach((v) => {
    console.log(`  - "${v.name}" lang=${v.lang} local=${v.localService}`);
  });

  // 只选择普通话语音（严格排除粤语/台湾/繁体）
  const mandarinVoices = voices.filter((v) => {
    if (!v.lang) return false;
    const lang = v.lang.toLowerCase();
    // 排除所有粤语和台湾音变体
    if (lang.includes('hk') || lang.includes('tw') || lang.includes('yue') || lang.includes('hant')) return false;
    // 必须是普通话：zh-CN 或 cmn-Hans-CN
    if (lang === 'zh-cn' || lang === 'cmn-hans-cn') return true;
    // 其他 zh-* 只在明确排除了粤语的情况下才接受
    return lang.startsWith('zh') && !lang.includes('hk') && !lang.includes('tw');
  });

  console.log('[TTS] 筛选后普通话语音:', mandarinVoices.map(v => `${v.name} (${v.lang})`));

  if (mandarinVoices.length === 0) {
    console.warn('[TTS] 未找到任何普通话语音！将使用 utterance.lang 让浏览器自动选择');
    cachedBestVoice = null;
    return null;
  }

  // 排除 Preview/Experimental 等实验性语音
  const stableVoices = mandarinVoices.filter(
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

  // 最后兜底：第一个普通话语音
  if (!best) best = mandarinVoices[0];

  console.log('[TTS] 选中语音:', best ? `${best.name} (${best.lang})` : '无');
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
  if (bestVoice) {
    utterance.voice = bestVoice; // 有普通话语音时才设置
  }
  // 没有普通话语音时不设置 voice，浏览器会根据 lang='zh-CN' 自动选择
  utterance.pitch = 1.0;
  utterance.rate = voiceSpeed.value; // 每句读取最新倍速
  // 电话模式降低音量，减弱扬声器回声对麦克风识别的干扰（与 speakSentence 保持一致）
  utterance.volume = isPhoneMode.value ? 0.6 : 1.0;

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
let streamingSentenceQueue = []; // 流式待播放句子队列：上一句播完后才播下一句，避免 Chrome speechSynthesis 队列堆积导致卡顿
let streamingIsPlaying = false; // 流式是否有句子正在播放
let lastStreamSpeakTime = 0; // 上次调用 speechSynthesis.speak() 的时间戳，用于轮询兜底的宽限期
let speakingCheckTimer = null; // 轮询定时器：检测 TTS 是否已停止
let userStoppedSending = false; // 用户主动停止发送，阻止后续自动播放

// 重置播放状态（统一出口）
const resetSpeakingState = () => {
  isSpeaking.value = false;
  currentSpeakingMsgId.value = null;
  ttsUtteranceCount = 0;
  streamingSentenceQueue = [];
  streamingIsPlaying = false;
  lastStreamSpeakTime = 0;
  stopSpeakingCheck();
};

// 启动轮询：检测 speechSynthesis 是否已实际停止（onend 不可靠的兜底方案）
const startSpeakingCheck = () => {
  stopSpeakingCheck();
  speakingCheckTimer = setInterval(() => {
    // 顺序播放队列兜底：Chrome utterance.onend 在移动端经常不触发，
    // 导致队列卡住（播完一句就停）。检测到当前句子已播完但 onend 没触发时，手动推进。
    // 加 500ms 宽限期避免与 speak() 调用竞态（speak 后 pending 可能未立即置 true）
    // 注意：不检查 streamingTtsActive，因为 finishStreamingTTS 后 streamingTtsActive=false 但队列可能还有剩余句子
    if (streamingIsPlaying && !window.speechSynthesis.speaking && !window.speechSynthesis.pending && (Date.now() - lastStreamSpeakTime > 500)) {
      streamingIsPlaying = false;
      playNextStreamingSentence();
      return;
    }
    // 安全网：队列有积压但没在播放（speakSentence 入队后 playNextStreamingSentence 未触发）
    if (!streamingIsPlaying && streamingSentenceQueue.length > 0) {
      playNextStreamingSentence();
      return;
    }
    // speechSynthesis 既不在播放也没有排队，但 isSpeaking 仍为 true → 状态卡住了
    if (isSpeaking.value && !window.speechSynthesis.speaking && !window.speechSynthesis.pending && !streamingTtsActive) {
      resetSpeakingState();
      return;
    }
    // 兜底：speechSynthesis 正在播放但 isSpeaking 未置 true（移动端 utterance.onstart 偶发不触发）
    if (!isSpeaking.value && (window.speechSynthesis.speaking || window.speechSynthesis.pending)) {
      isSpeaking.value = true;
    }
    // Chrome 15秒暂停 bug：长文本会自动暂停，定期 resume 保持播放
    if (window.speechSynthesis.speaking && window.speechSynthesis.paused) {
      window.speechSynthesis.resume();
    }
  }, 300);
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
  // 字幕模式下不播报语音，保持持续监听
  if (isPhoneMode.value && phoneSubtitleEnabled.value) return;
  // 播放时关闭录音，避免麦克风干扰播放
  if (isPhoneMode.value) {
    stopPhoneAutoListen();
  }
  window.speechSynthesis.cancel();
  streamingTtsPlayedLength = 0;
  streamingTtsActive = true;
  ttsUtteranceCount = 0;
  streamingSentenceQueue = [];
  streamingIsPlaying = false;
  lastStreamSpeakTime = 0;
  isSpeaking.value = false;
  currentSpeakingMsgId.value = null;
  phoneLastSpeechTime = 0;
  resetPreviewTextBuffer();
  startSpeakingCheck();
};

// 播放队列中的下一句（流式顺序播放：上一句 onend 后才 speak 下一句，避免 utterance 堆积卡顿）
const playNextStreamingSentence = () => {
  // 队列空：暂无可播放的句子
  if (streamingSentenceQueue.length === 0) {
    streamingIsPlaying = false;
    // 流式已结束且队列空 → 整段播放完成，重置状态
    if (!streamingTtsActive) {
      resetSpeakingState();
    }
    return;
  }
  streamingIsPlaying = true;
  const cleanText = streamingSentenceQueue.shift();

  const utterance = new SpeechSynthesisUtterance(cleanText);
  utterance.lang = 'zh-CN';
  const bestVoice = getBestChineseVoice();
  if (bestVoice) {
    utterance.voice = bestVoice; // 有普通话语音时才设置
  }
  // 没有普通话语音时不设置 voice，浏览器会根据 lang='zh-CN' 自动选择
  utterance.pitch = 1.0;
  utterance.rate = voiceSpeed.value; // 每句读取最新倍速
  utterance.volume = isPhoneMode.value ? 0.6 : 1.0;

  utterance.onstart = () => {
    isSpeaking.value = true;
    startSpeakingCheck();
  };
  // 上一句播完才播下一句（顺序播放，不依赖 Chrome 队列）
  utterance.onend = () => {
    playNextStreamingSentence();
  };
  utterance.onerror = () => {
    playNextStreamingSentence();
  };

  window.speechSynthesis.speak(utterance);
  lastStreamSpeakTime = Date.now();
};

// 播放单个句子（入队，等上一句播完再播）
const speakSentence = (text) => {
  if (!text || !text.trim()) return;
  const cleanText = sanitizeTextForTTS(text);
  if (!cleanText) return;

  // 入队，若当前无句子在播则立即开始播放下一句
  streamingSentenceQueue.push(cleanText);
  if (!streamingIsPlaying) {
    playNextStreamingSentence();
  }
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

  // 只有实际有句子在播放或排队时才设置状态（用于语音条动画）
  // 非流式场景（没有收到任何流式文本）队列空且未播放，不设置状态，
  // 由下方 watch 检测到 !isSpeaking 后调用 autoPlayLatestAIVoice 常规播放
  if (streamingIsPlaying || streamingSentenceQueue.length > 0) {
    currentSpeakingMsgId.value = msgId;
    isSpeaking.value = true;
    startSpeakingCheck(); // 确保轮询兜底检测已启动
  }
};

// 自动播放最新AI消息
const autoPlayLatestAIVoice = () => {
  if (!isVoiceMode.value) return;
  if (isSending.value) return;
  // 字幕模式下不播报语音
  if (isPhoneMode.value && phoneSubtitleEnabled.value) return;
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
    // 开启语音模式时：解锁语音合成 + 提前获取阿里云 ASR Token + 申请麦克风权限
    unlockSpeechSynthesis();
    await fetchAliAsrToken();
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
    resetPreviewTextBuffer();
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

// 电话模式：AI回复播放结束后重置录音，重新开始监听
watch(isSpeaking, (newVal, oldVal) => {
  if (!isPhoneMode.value) return;
  // 字幕模式下识别一直运行，不需要重启
  if (phoneSubtitleEnabled.value) return;
  // TTS 播放结束 → 停止当前识别，重置后重新开始监听
  if (oldVal && !newVal) {
    setTimeout(() => {
      if (isPhoneMode.value && !isSpeaking.value) {
        // 用户正在说话中，不中断识别
        if (Date.now() - phoneLastSpeechTime < PHONE_SILENCE_THRESHOLD) return;
        if (recognition.value) {
          try { recognition.value.abort(); } catch (e) { /* ignore */ }
        }
        isRecording.value = false;
        resetPreviewTextBuffer();
        startPhoneAutoListen();
      }
    }, 300);
  }
});

// 电话模式：非流式场景（isSpeaking从未true），发送结束后恢复监听
watch(isSending, (newVal, oldVal) => {
  if (!isPhoneMode.value) return;
  // 字幕模式下识别一直运行，不需要重启
  if (phoneSubtitleEnabled.value) return;
  // 发送结束且未进入播放状态 → 恢复监听
  if (oldVal && !newVal && !isSpeaking.value) {
    setTimeout(() => {
      if (isPhoneMode.value && !isSending.value && !isSpeaking.value) {
        // 用户正在说话中，不中断识别、不重置 buffer
        if (Date.now() - phoneLastSpeechTime < PHONE_SILENCE_THRESHOLD) return;
        if (recognition.value) {
          try { recognition.value.abort(); } catch (e) { /* ignore */ }
        }
        isRecording.value = false;
        resetPreviewTextBuffer();
        startPhoneAutoListen();
      }
    }, 300);
  }
});

// 字幕开启时立即停止播放中的语音，恢复监听
watch(phoneSubtitleEnabled, (newVal) => {
  if (!isPhoneMode.value || !newVal) return;
  if (isSpeaking.value || window.speechSynthesis.speaking) {
    window.speechSynthesis.cancel();
    streamingTtsActive = false;
    streamingTtsPlayedLength = 0;
    staticPlaybackMsgId = null;
    staticSentenceList = [];
    staticSentenceIndex = 0;
    resetSpeakingState();
    // 恢复监听
    if (recognition.value) {
      try { recognition.value.abort(); } catch (e) { /* ignore */ }
    }
    isRecording.value = false;
    resetPreviewTextBuffer();
    setTimeout(() => {
      if (isPhoneMode.value) startPhoneAutoListen();
    }, 300);
  }
});

// 字幕面板自动滚动到底部
let phoneSubtitleScrollRaf = null;
watch([() => phoneMessages.value.length, () => {
  const msgs = phoneMessages.value;
  return msgs.length ? msgs[msgs.length - 1].content : '';
}], () => {
  if (!phoneSubtitleEnabled.value) return;
  if (phoneSubtitleScrollRaf) return;
  phoneSubtitleScrollRaf = requestAnimationFrame(() => {
    phoneSubtitleScrollRaf = null;
    if (phoneSubtitlePanel.value) {
      phoneSubtitlePanel.value.scrollTop = phoneSubtitlePanel.value.scrollHeight;
    }
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

// 滚动到底部（RAF 节流：每帧最多滚动一次，避免流式文本逐字触发大量重排）
let _scrollPending = false;
const scrollToBottom = () => {
  if (_scrollPending) return;
  _scrollPending = true;
  requestAnimationFrame(() => {
    _scrollPending = false;
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
  // 保存已接收的流式AI回复内容到消息列表（中断撤回时也需要展示）
  const streamedText = aimessage2.value;
  const streamedReasoning = aIReasoningContentMsg.value;
  const streamedTools = aIToolsContentMsg.value;
  if (streamedText || streamedReasoning || streamedTools) {
    const aiMessage = {
      id: Date.now(),
      conversationId: activeConversationId.value,
      isSend: false,
      content: streamedText || '',
      aiReasoningContent: streamedReasoning || '',
      aiToolsContent: streamedTools || '',
      createdAt: new Date().toISOString(),
      totalTokenCount: 0,
    };
    if (currentReceivingMsgId.value) {
      aiMessage.id = currentReceivingMsgId.value;
    }
    messages.value.push(aiMessage);
  }
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
  if (phoneSubtitleScrollRaf) {
    cancelAnimationFrame(phoneSubtitleScrollRaf);
    phoneSubtitleScrollRaf = null;
  }
  stopPhoneCallTimer(); // 清理通话计时器
  stopPhoneSilenceTimer(); // 清理静默检测定时器
  stopSpeakingCheck(); // 清理轮询定时器
  staticPlaybackGen++; // 使旧回调失效
  staticPlaybackMsgId = null;
  window.speechSynthesis?.cancel();
  if (recognition.value) {
    const rec = recognition.value;
    try { rec.onend = null; rec.onerror = null; rec.onresult = null; rec.abort(); } catch (e) { /* ignore */ }
  }
  // 停止 SignalR 连接
  if (connectionTimeout) {
    clearTimeout(connectionTimeout);
    connectionTimeout = null;
  }
  connectionServer?.stop();
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

<style scoped src="../../css/MyAIChat.css"></style>