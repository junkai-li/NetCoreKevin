<template>
  <div class="ai-chat-container">
    <div class="chat-layout">
      <!-- 左侧对话列表 -->
      <div class="chat-sidebar">
        <div class="sidebar-header">
          <h3>我的对话</h3>
          <a-button
            type="primary"
            @click="showAgentSelectionModal"
            size="small"
            class="add-button"
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
                :class="{ active: item.id === activeConversationId }"
                @click="selectConversation(item)"
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
          <h2>{{ activeConversation.title || "新对话" }}</h2>
          <div class="agent-info" v-if="activeConversation.appId">
            <RobotOutlined />
            <span>{{ getAiAppName(activeConversation.appId) }}</span>
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
            <div class="message-avatar">
              <UserOutlined v-if="message.isSend === true" />
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
             <div class="message-text">{{ aimessage2 }}</div>
              <div class="message-time">  {{ aimessage}}</div> 
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
            <a-button type="primary"  @click="showAgentSelectionModal" class="add-button-large">
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
/* eslint-disable */
import { ref, onMounted, nextTick, watch, h, onUnmounted } from "vue";
import {
  PlusOutlined,
  UserOutlined,
  RobotOutlined,
  MessageOutlined,
  DeleteOutlined,
} from "@ant-design/icons-vue";
import { message, Modal, Select } from "ant-design-vue";
import { getAIAppsALLList } from "../../api/ai/aiapps.js";
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
const aimessage=ref("");//提示消息  
const aimessage2=ref("");//提示消息  
// 分页相关变量
const currentPage = ref(1);
const hasMoreMessages = ref(true);
const loadingMessages = ref(false);
const pageSize = ref(20); // 每页消息数量

// 智能体相关
const aiApps = ref([]); // 存储智能体列表
const selectedAiApp = ref(null); // 存储选中的智能体

// 获取对话列表
const loadConversations = async () => {
  loadingConversations.value = true;
  try {
    // 调用真实API获取对话列表
    const response = await getAIChatsMyPageData({
      pageNum: 1,
      pageSize: 10000, // 获取足够多的对话记录
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
    const response = await getAIAppsALLList();
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
const showAgentSelectionModal = () => {
  // 重置选中的智能体
  selectedAiApp.value = null;

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
        createdAt: item.creationTime || new Date().toISOString(),
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
  newMessage.value = "";
  isSending.value = true; 
  aimessage.value='正在思考....'
  aimessage2.value='';
 const snowflakeId = (await GetSnowflakeId()).data;
  try {
    // 添加用户消息到列表
    const userMessage = {
      id: Date.now(),
      conversationId: activeConversationId.value,
      isSend: true,
      content: messageToSend,
      createdAt: new Date().toISOString(),
    };
    messages.value.push(userMessage); 
     // 滚动到底部以显示最新内容
        scrollToBottom();
         //监听流式输出
    getAiMySignalRHubMsg(snowflakeId); 
    var reulst = await addAIChatHistorys({
      aIChatsId: activeConversationId.value,
      id:snowflakeId,
      content: messageToSend,
    });
    if (reulst && reulst.code != 200) {
      message.error("发送失败");
      stopAiMySignalRHubMsg(snowflakeId);
    }
    scrollToBottom();
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
     messages.value.push({
      id: reulst.data.id,
      conversationId: activeConversationId.value,
      isSend: reulst.data.isSend,
      content: reulst.data.content,
      createdAt: reulst.data.createTime,
    }); 
      stopAiMySignalRHubMsg(snowflakeId);
        isSending.value = false;  
    scrollToBottom();
  } catch (error) {
    stopAiMySignalRHubMsg(snowflakeId);
    console.error("发送消息失败:", error);
    message.error("发送消息失败");
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
var connectionServer=null;
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
  });
}) 
// 接收AI消息
connectionServer.on('processmsg', (msg) => {    
    aimessage.value=msg 
}) 
}

const stopAiMySignalRHubMsg=(id)=>{
  aimessage.value='';
  aimessage2.value='';
connectionServer?.stop();
}
   

// 格式化日期
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  const now = new Date();
  const diffTime = Math.abs(now - date);
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

  if (diffDays === 1) {
    return "今天";
  } else if (diffDays === 2) {
    return "昨天";
  } else if (diffDays <= 7) {
    return `${diffDays - 1}天前`;
  } else {
    return date.toLocaleDateString("zh-CN");
  }
};

// 格式化时间
const formatTime = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleTimeString("zh-CN", { hour: "2-digit", minute: "2-digit" });
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

// 组件卸载时移除滚动事件监听器
onUnmounted(() => {
  if (messagesContainer.value) {
    messagesContainer.value.removeEventListener("scroll", handleScroll);
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
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  margin-bottom: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 15px;
  color: rgba(255, 255, 255, 0.8);
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.conversation-item:hover {
  background: rgba(102, 126, 234, 0.2);
  border: 1px solid rgba(102, 126, 234, 0.3);
}

.conversation-item.active {
  background: rgba(102, 126, 234, 0.3);
  border: 1px solid rgba(102, 126, 234, 0.5);
}

.conversation-content {
  display: flex;
  flex-direction: column;
  flex: 1;
  text-align: left;
  overflow: hidden; /* 防止内容溢出 */
}

.conversation-title {
  font-weight: bold;
  margin-bottom: 5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.conversation-preview {
  font-size: 12px;
  margin-bottom: 5px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.conversation-time {
  font-size: 10px;
  opacity: 0.7;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.conversation-actions {
  opacity: 0;
  transition: opacity 0.3s ease;
  margin-left: 10px;
}

.conversation-item:hover .conversation-actions {
  opacity: 1;
}

.delete-btn {
  color: rgba(255, 255, 255, 0.5);
}

.delete-btn:hover {
  color: #ff4d4f;
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

.agent-info {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-top: 8px;
  font-size: 14px;
  opacity: 0.8;
}

/* 智能体选择模态框样式 */
.agent-selection-modal {
  padding: 20px 0;
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
  0%,
  100% {
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
