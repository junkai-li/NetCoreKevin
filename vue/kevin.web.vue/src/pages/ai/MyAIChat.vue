<template>
  <div class="ai-chat-container">
    <div class="chat-layout">
      <!-- 左侧对话列表 -->
      <div class="chat-sidebar">
        <div class="sidebar-header">
          <h3>我的对话</h3>
          <a-button type="primary" @click="createNewChat" size="small" class="add-button">
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
                :class="{ active: item.id === activeConversationId }"
                @click="selectConversation(item)"
              >
                <div class="conversation-content">
                  <div class="conversation-title">{{ item.title || '新对话' }}</div>
                  <div class="conversation-preview">
                    {{ item.lastMessage || '开始新的对话...' }}
                  </div>
                  <div class="conversation-time">
                    {{ formatDate(item.updatedAt) }}
                  </div>
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
          <h2>{{ activeConversation.title || '新对话' }}</h2>
        </div>

        <div class="chat-messages" ref="messagesContainer" v-if="activeConversation">
          <div
            v-for="message in messages"
            :key="message.id"
            class="message-item"
            :class="{ 'user-message': message.role === 'user', 'ai-message': message.role === 'assistant' }"
          >
            <div class="message-avatar">
              <UserOutlined v-if="message.role === 'user'" />
              <RobotOutlined v-else />
            </div>
            <div class="message-content">
              <div class="message-text">{{ message.content }}</div>
              <div class="message-time">{{ formatTime(message.createdAt) }}</div>
            </div>
          </div>
          
          <div v-if="isSending" class="message-item ai-message">
            <div class="message-avatar">
              <RobotOutlined />
            </div>
            <div class="message-content">
              <div class="typing-indicator">
                <span></span>
                <span></span>
                <span></span>
              </div>
            </div>
          </div>
        </div>

        <div class="chat-input-area" v-if="activeConversation">
          <a-input-group compact class="input-group">
            <a-textarea
              v-model:value="newMessage"
              placeholder="输入消息..."
              class="message-input"
              @pressEnter="sendMessage"
              :disabled="isSending"
              :auto-size="{ minRows: 3, maxRows: 6 }"
              allow-clear
            />
            <a-button
              type="primary"
              @click="sendMessage"
              :loading="isSending"
              :disabled="!newMessage.trim() || isSending"
              class="send-button"
            >
              发送
            </a-button>
          </a-input-group>
        </div>

        <div class="chat-placeholder" v-else>
          <div class="placeholder-content">
            <MessageOutlined class="placeholder-icon" />
            <p>选择一个对话或创建新对话开始聊天</p>
            <a-button type="primary" @click="createNewChat" class="add-button-large">
              <template #icon>
                <PlusOutlined />
              </template>
              新建对话
            </a-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, watch } from 'vue';
import {
  PlusOutlined,
  UserOutlined,
  RobotOutlined,
  MessageOutlined
} from '@ant-design/icons-vue';
import { message } from 'ant-design-vue';

// 模拟数据
const conversations = ref([]);
const activeConversationId = ref(null);
const activeConversation = ref(null);
const messages = ref([]);
const newMessage = ref('');
const loadingConversations = ref(false);
const isSending = ref(false);
const messagesContainer = ref(null);

// 获取对话列表
const loadConversations = async () => {
  loadingConversations.value = true;
  try {
    // 模拟API调用
    setTimeout(() => {
      conversations.value = [
        {
          id: 1,
          title: '如何学习Vue.js?',
          lastMessage: 'Vue.js是一套构建用户界面的渐进式框架...',
          createdAt: '2023-05-15T10:30:00Z',
          updatedAt: '2023-05-15T10:30:00Z'
        },
        {
          id: 2,
          title: 'JavaScript闭包详解',
          lastMessage: '闭包是指有权访问另一个函数作用域中变量的函数...',
          createdAt: '2023-05-14T14:20:00Z',
          updatedAt: '2023-05-14T14:20:00Z'
        },
        {
          id: 3,
          title: 'CSS Grid布局指南',
          lastMessage: 'Grid布局是CSS中最强大的布局系统之一...',
          createdAt: '2023-05-12T09:15:00Z',
          updatedAt: '2023-05-12T09:15:00Z'
        }
      ];
      loadingConversations.value = false;
    }, 500);
  } catch (error) {
    console.error('加载对话列表失败:', error);
    message.error('加载对话列表失败');
    loadingConversations.value = false;
  }
};

// 创建新对话
const createNewChat = () => {
  const newConversation = {
    id: Date.now(),
    title: '',
    lastMessage: '',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  };
  
  conversations.value.unshift(newConversation);
  selectConversation(newConversation);
};

// 选择对话
const selectConversation = (conversation) => {
  activeConversationId.value = conversation.id;
  activeConversation.value = conversation;
  
  // 模拟加载消息
  loadMessages(conversation.id);
};

// 加载消息
const loadMessages = (conversationId) => {
  // 模拟API调用
  setTimeout(() => {
    if (conversationId === 1) {
      messages.value = [
        {
          id: 1,
          conversationId: 1,
          role: 'user',
          content: '如何学习Vue.js?',
          createdAt: '2023-05-15T10:30:00Z'
        },
        {
          id: 2,
          conversationId: 1,
          role: 'assistant',
          content: '学习Vue.js可以从以下几个方面入手：\n\n1. 官方文档：Vue.js官网提供了详细的教程和API文档\n2. 基础概念：掌握响应式数据、指令、组件等核心概念\n3. 实践项目：通过实际项目加深理解\n4. 生态工具：学习Vue Router、Vuex等配套工具',
          createdAt: '2023-05-15T10:31:00Z'
        }
      ];
    } else if (conversationId === 2) {
      messages.value = [
        {
          id: 3,
          conversationId: 2,
          role: 'user',
          content: '什么是JavaScript闭包？',
          createdAt: '2023-05-14T14:20:00Z'
        },
        {
          id: 4,
          conversationId: 2,
          role: 'assistant',
          content: '闭包是指有权访问另一个函数作用域中变量的函数，即使在外部函数返回后仍然可以访问这些变量。它是JavaScript中的一个重要概念，常用于创建私有变量和模块模式。',
          createdAt: '2023-05-14T14:22:00Z'
        }
      ];
    } else if (conversationId === 3) {
      messages.value = [
        {
          id: 5,
          conversationId: 3,
          role: 'user',
          content: 'CSS Grid有哪些优势？',
          createdAt: '2023-05-12T09:15:00Z'
        },
        {
          id: 6,
          conversationId: 3,
          role: 'assistant',
          content: 'CSS Grid的优势包括：\n\n1. 二维布局系统：可以同时控制行和列\n2. 灵活的网格定义：可以轻松创建复杂的网格结构\n3. 强大的对齐能力：提供了多种对齐选项\n4. 响应式友好：可以轻松适应不同屏幕尺寸',
          createdAt: '2023-05-12T09:17:00Z'
        }
      ];
    } else {
      messages.value = [];
    }
    
    scrollToBottom();
  }, 300);
};
/* eslint-disable */ 
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
  newMessage.value = '';
  isSending.value = true;
  
  try {
    // 添加用户消息到列表
    const userMessage = {
      id: Date.now(),
      conversationId: activeConversationId.value,
      role: 'user',
      content: messageToSend,
      createdAt: new Date().toISOString()
    };
    
    messages.value.push(userMessage);
    scrollToBottom();
    
    // 更新对话列表中的预览
    const conversation = conversations.value.find(c => c.id === activeConversationId.value);
    if (conversation) {
      conversation.lastMessage = messageToSend;
      conversation.updatedAt = new Date().toISOString();
      if (!conversation.title) {
        conversation.title = messageToSend.substring(0, 20) + (messageToSend.length > 20 ? '...' : '');
      }
    }
    
    // 模拟AI回复
    setTimeout(() => {
      const aiResponse = {
        id: Date.now() + 1,
        conversationId: activeConversationId.value,
        role: 'assistant',
        content: getMockAIResponse(messageToSend),
        createdAt: new Date().toISOString()
      };
      
      messages.value.push(aiResponse);
      isSending.value = false;
      
      // 更新对话列表中的预览
      if (conversation) {
        conversation.lastMessage = aiResponse.content.substring(0, 30) + (aiResponse.content.length > 30 ? '...' : '');
        conversation.updatedAt = new Date().toISOString();
      }
      
      scrollToBottom();
    }, 1000);
    
  } catch (error) {
    console.error('发送消息失败:', error);
    message.error('发送消息失败');
    isSending.value = false;
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

// 获取模拟AI回复
const getMockAIResponse = (userMessage) => {
  const responses = [
    `关于"${userMessage}"，这是一个很好的问题。我可以为您提供相关信息...`,
    `针对您的问题"${userMessage}"，我的看法是这样的...`,
    `感谢您的提问"${userMessage}"。根据我的知识库，我可以告诉您...`,
    `"${userMessage}"确实是一个值得探讨的话题。让我来详细解释一下...`,
    `对于"${userMessage}"这个问题，我建议您可以从以下几个方面考虑...`
  ];
  
  return responses[Math.floor(Math.random() * responses.length)];
};

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  const now = new Date();
  const diffTime = Math.abs(now - date);
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  
  if (diffDays === 1) {
    return '今天';
  } else if (diffDays === 2) {
    return '昨天';
  } else if (diffDays <= 7) {
    return `${diffDays - 1}天前`;
  } else {
    return date.toLocaleDateString('zh-CN');
  }
};

// 格式化时间
const formatTime = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' });
};

// 监听消息变化，自动滚动到底部
watch(messages, () => {
  scrollToBottom();
});

// 组件挂载时加载对话列表
onMounted(() => {
  loadConversations();
});
</script>

<style scoped>
.ai-chat-container {
  height: calc(100vh - 220px);
  border-radius: 15px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  color: white;
}

.chat-layout {
  display: flex;
  height: 100%;
}

.chat-sidebar {
  width: 280px;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
  display: flex;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.05);
  height: 100%; /* 设置固定高度 */
}

.sidebar-header {
  padding: 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0; /* 防止头部被压缩 */
}

.sidebar-header h3 {
  margin: 0;
  color: white;
  font-size: 16px;
  font-weight: 500;
}

.add-button {
  background: linear-gradient(45deg, #667eea, #764ba2);
  border: none;
  border-radius: 20px;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
  transition: all 0.3s ease;
}

.add-button:hover {
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
  transform: translateY(-2px);
}

.conversation-list {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden; /* 隐藏水平滚动条 */
  /* 添加自定义滚动条样式 */
  scrollbar-width: thin;
  scrollbar-color: rgba(102, 126, 234, 0.5) rgba(255, 255, 255, 0.1);
  height: 0; /* 允许flex-grow填充剩余空间 */
}

.conversation-list::-webkit-scrollbar {
  width: 6px;
}

.conversation-list::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 3px;
}

.conversation-list::-webkit-scrollbar-thumb {
  background: rgba(102, 126, 234, 0.5);
  border-radius: 3px;
}

.conversation-list::-webkit-scrollbar-thumb:hover {
  background: rgba(102, 126, 234, 0.7);
}

.conversation-item {
  padding: 15px 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  cursor: pointer;
  transition: all 0.3s ease;
  background: transparent;
  white-space: nowrap; /* 防止换行 */
  overflow: hidden; /* 隐藏溢出内容 */
}

.conversation-content {
  display: flex;
  flex-direction: column;
  text-align: left;
  overflow: hidden; /* 防止内容溢出 */
}

.conversation-title {
  font-weight: 500;
  color: white;
  margin-bottom: 5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-align: left; /* 标题靠左对齐 */
}

.conversation-preview {
  font-size: 13px;
  color: rgba(255, 255, 255, 0.75);
  margin-bottom: 5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-align: left; /* 预览内容靠左对齐 */
}

.conversation-time {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.6);
  text-align: left;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.empty-conversations {
  padding: 40px 20px;
  text-align: center;
}

:deep(.empty-conversations .ant-empty-description) {
  color: rgba(255, 255, 255, 0.5);
}

.chat-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.02);
  height: 100%; /* 设置固定高度 */
}

.chat-header {
  padding: 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  flex-shrink: 0; /* 防止头部被压缩 */
}

.chat-header h2 {
  margin: 0;
  color: white;
  font-size: 18px;
  font-weight: 500;
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  /* 添加自定义滚动条样式 */
  scrollbar-width: thin;
  scrollbar-color: rgba(102, 126, 234, 0.5) rgba(255, 255, 255, 0.1);
  height: 0; /* 允许flex-grow填充剩余空间 */
}

.chat-messages::-webkit-scrollbar {
  width: 6px;
}

.chat-messages::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 3px;
}

.chat-messages::-webkit-scrollbar-thumb {
  background: rgba(102, 126, 234, 0.5);
  border-radius: 3px;
}

.chat-messages::-webkit-scrollbar-thumb:hover {
  background: rgba(102, 126, 234, 0.7);
}

.message-item {
  display: flex;
  gap: 15px;
  max-width: 80%;
}

.message-item.user-message {
  align-self: flex-end;
}

.message-item.ai-message {
  align-self: flex-start;
}

.message-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: linear-gradient(145deg, rgba(102, 126, 234, 0.3), rgba(102, 126, 234, 0.2));
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  flex-shrink: 0;
  border: 1px solid rgba(102, 126, 234, 0.3);
}

.message-content {
  display: flex;
  flex-direction: column;
  text-align: left; /* 消息内容靠左对齐 */
}

.message-text {
  padding: 12px 16px;
  border-radius: 18px;
  color: white;
  line-height: 1.5;
  white-space: pre-wrap;
  text-align: left; /* 文本内容靠左对齐 */
}

.message-item.user-message .message-text {
  background: linear-gradient(145deg, rgba(102, 126, 234, 0.8), rgba(118, 75, 162, 0.8));
  border-bottom-right-radius: 4px;
}

.message-item.ai-message .message-text {
  background: rgba(255, 255, 255, 0.1);
  border-bottom-left-radius: 4px;
}

.message-time {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.6);
  margin-top: 5px;
}

.message-item.user-message .message-time {
  text-align: right;
}

.typing-indicator {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 18px;
  border-bottom-left-radius: 4px;
}

.typing-indicator span {
  height: 8px;
  width: 8px;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  margin: 0 2px;
  animation: typing 1s infinite;
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
  }
  50% {
    transform: translateY(-5px);
  }
}

.chat-input-area {
  padding: 20px;
  border-top: 1px solid rgba(255, 255, 255, 0.2);
  flex-shrink: 0; /* 防止输入区域被压缩 */
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.message-input {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  color: white;
  border-radius: 15px;
  padding: 12px 16px;
  resize: none;
  text-align: left; /* 文字靠左对齐 */
}

:deep(.message-input .ant-input) {
  background: transparent;
  color: white;
  resize: none;
  text-align: left; /* 确保输入框内文字靠左 */
}

:deep(.message-input .ant-input:focus) {
  box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.2);
}

.send-button {
  align-self: flex-end;
  border-radius: 20px;
  background: linear-gradient(45deg, #667eea, #764ba2);
  border: none;
  padding: 6px 20px;
  width: auto;
}

.chat-placeholder {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.02);
}

.placeholder-content {
  text-align: center;
  color: rgba(255, 255, 255, 0.5);
}

.placeholder-icon {
  font-size: 48px;
  margin-bottom: 20px;
  color: rgba(255, 255, 255, 0.3);
}

.add-button-large {
  background: linear-gradient(45deg, #667eea, #764ba2);
  border: none;
  border-radius: 20px;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
  transition: all 0.3s ease;
}

.add-button-large:hover {
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
  transform: translateY(-2px);
}

.placeholder-content p {
  margin-bottom: 20px;
}

@media (max-width: 768px) {
  .chat-layout {
    flex-direction: column;
  }
  
  .chat-sidebar {
    width: 100%;
    border-right: none;
    border-bottom: 1px solid rgba(255, 255, 255, 0.2);
    max-height: 200px;
  }
}
</style>