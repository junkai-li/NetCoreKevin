<style>
.lang-switch { position: fixed; top: 20px; right: 20px; z-index: 9999; display: flex; gap: 8px; }
.lang-switch button { padding: 8px 16px; border: 1px solid #ddd; border-radius: 4px; cursor: pointer; font-size: 14px; background: #fff; transition: all 0.3s; }
.lang-switch button:hover { background: #f5f5f5; }
.lang-switch button.active { background: #1890ff; color: #fff; border-color: #1890ff; }
.en { display: none; }
body.lang-en .zh { display: none; }
body.lang-en .en { display: block; }
</style>
<script>
function switchLang(lang) {
    document.body.className = lang === 'en' ? 'lang-en' : '';
    localStorage.setItem('doc-lang', lang);
    document.querySelectorAll('.lang-switch button').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.lang === lang);
    });
}
document.addEventListener('DOMContentLoaded', function() {
    var savedLang = localStorage.getItem('doc-lang') || 'zh';
    switchLang(savedLang);
});
</script>
<div class="lang-switch">
    <button data-lang="zh" class="active" onclick="switchLang('zh')">中文</button>
    <button data-lang="en" onclick="switchLang('en')">English</button>
</div>

# <span class="zh">AI智能体后台管理系统 - 使用文档</span><span class="en">AI Agent Backend Management System - User Documentation</span>

## <span class="zh">一、系统概述</span><span class="en">I. System Overview</span>

### <span class="zh">1.1 系统简介</span><span class="en">1.1 System Introduction</span>

<span class="zh">**AI智能体后台管理系统**是一款基于 Vue 3 + Ant Design Vue 构建的企业级后台管理平台，主要用于管理AI智能体、用户、权限、知识库等核心业务模块。</span><span class="en">**AI Agent Backend Management System** is an enterprise-level backend management platform built on Vue 3 + Ant Design Vue, primarily used for managing core business modules such as AI agents, users, permissions, and knowledge bases.</span>

### <span class="zh">1.2 技术架构</span><span class="en">1.2 Technical Architecture</span>

| <span class="zh">层级</span><span class="en">Layer</span> | <span class="zh">技术栈</span><span class="en">Technology Stack</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|--------|------|
| <span class="zh">前端框架</span><span class="en">Frontend Framework</span> | Vue 3 | <span class="zh">渐进式 JavaScript 框架</span><span class="en">Progressive JavaScript Framework</span> |
| <span class="zh">UI组件库</span><span class="en">UI Component Library</span> | Ant Design Vue 4.x | <span class="zh">企业级UI设计语言</span><span class="en">Enterprise-Grade UI Design Language</span> |
| <span class="zh">路由管理</span><span class="en">Routing Management</span> | Vue Router 4.x | <span class="zh">Vue官方路由管理器</span><span class="en">Vue Official Router Manager</span> |
| <span class="zh">HTTP客户端</span><span class="en">HTTP Client</span> | Axios | <span class="zh">基于Promise的HTTP库</span><span class="en">Promise-Based HTTP Library</span> |
| <span class="zh">实时通信</span><span class="en">Real-time Communication</span> | SignalR | <span class="zh">实时双向通信</span><span class="en">Real-time Bidirectional Communication</span> |
| <span class="zh">代码编辑器</span><span class="en">Code Editor</span> | CodeMirror 6 | <span class="zh">高性能代码编辑器</span><span class="en">High-Performance Code Editor</span> |
| <span class="zh">构建工具</span><span class="en">Build Tool</span> | Vue CLI 5 | <span class="zh">Vue官方构建工具</span><span class="en">Vue Official Build Tool</span> |

### <span class="zh">1.3 系统环境要求</span><span class="en">1.3 System Environment Requirements</span>

- **Node.js**: >= 14.0.0
- **npm**: >= 6.0.0
- <span class="zh">**浏览器支持**</span><span class="en">**Browser Support**</span>: Chrome >= 80, Firefox >= 75, Safari >= 13, Edge >= 80

---

## <span class="zh">二、项目结构</span><span class="en">II. Project Structure</span>

```
src/
├── api/                    # <span class="zh">接口层</span><span class="en">API Layer</span>
│   ├── ai/                 # <span class="zh">AI相关接口</span><span class="en">AI-related APIs</span>
│   ├── organizational/     # <span class="zh">组织架构接口</span><span class="en">Organizational APIs</span>
│   └── *.js                # <span class="zh">其他业务接口</span><span class="en">Other Business APIs</span>
├── components/             # <span class="zh">公共组件</span><span class="en">Common Components</span>
│   ├── ai/                 # <span class="zh">AI模块组件</span><span class="en">AI Module Components</span>
│   └── *.vue               # <span class="zh">通用组件</span><span class="en">General Components</span>
├── pages/                  # <span class="zh">页面层</span><span class="en">Page Layer</span>
│   ├── ai/                 # <span class="zh">AI管理页面</span><span class="en">AI Management Pages</span>
│   ├── organizational/     # <span class="zh">组织架构页面</span><span class="en">Organizational Pages</span>
│   └── *.vue               # <span class="zh">其他页面</span><span class="en">Other Pages</span>
├── router/                 # <span class="zh">路由配置</span><span class="en">Route Configuration</span>
├── utils/                  # <span class="zh">工具函数</span><span class="en">Utility Functions</span>
├── css/                    # <span class="zh">全局样式</span><span class="en">Global Styles</span>
├── assets/                 # <span class="zh">静态资源</span><span class="en">Static Resources</span>
├── App.vue                 # <span class="zh">根组件</span><span class="en">Root Component</span>
└── main.js                 # <span class="zh">入口文件</span><span class="en">Entry File</span>
```

---

## <span class="zh">三、功能模块</span><span class="en">III. Feature Modules</span>

### <span class="zh">3.1 首页仪表盘</span><span class="en">3.1 Dashboard</span>

<span class="zh">**路径**: `/home`</span><span class="en">**Path**: `/home`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">展示系统概览信息</span><span class="en">Display system overview information</span>
- <span class="zh">用户数量统计</span><span class="en">User count statistics</span>
- <span class="zh">未读消息数量</span><span class="en">Unread message count</span>
- <span class="zh">系统健康状态</span><span class="en">System health status</span>
- <span class="zh">快捷操作入口</span><span class="en">Quick action entries</span>

---

### <span class="zh">3.2 用户管理</span><span class="en">3.2 User Management</span>

#### <span class="zh">3.2.1 用户列表</span><span class="en">3.2.1 User List</span>

<span class="zh">**路径**: `/home/user/list`</span><span class="en">**Path**: `/home/user/list`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">用户数据展示（分页、搜索）</span><span class="en">User data display (pagination, search)</span>
- <span class="zh">用户信息编辑、删除</span><span class="en">User information editing, deletion</span>
- <span class="zh">用户状态管理</span><span class="en">User status management</span>
- <span class="zh">角色分配</span><span class="en">Role assignment</span>

<span class="zh">**主要操作**:</span><span class="en">**Main Operations**:</span>
| <span class="zh">操作</span><span class="en">Operation</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|
| <span class="zh">搜索</span><span class="en">Search</span> | <span class="zh">支持按用户名、账号搜索</span><span class="en">Support search by username, account</span> |
| <span class="zh">分页</span><span class="en">Pagination</span> | <span class="zh">支持自定义每页条数（10/20/30/50/100）</span><span class="en">Support custom page size (10/20/30/50/100)</span> |
| <span class="zh">添加</span><span class="en">Add</span> | <span class="zh">点击添加按钮创建新用户</span><span class="en">Click add button to create new user</span> |
| <span class="zh">编辑</span><span class="en">Edit</span> | <span class="zh">点击操作列编辑图标修改用户信息</span><span class="en">Click edit icon in action column to modify user info</span> |
| <span class="zh">删除</span><span class="en">Delete</span> | <span class="zh">点击操作列删除图标删除用户</span><span class="en">Click delete icon in action column to delete user</span> |

#### <span class="zh">3.2.2 角色管理</span><span class="en">3.2.2 Role Management</span>

<span class="zh">**路径**: `/home/user/role`</span><span class="en">**Path**: `/home/user/role`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">角色列表展示</span><span class="en">Role list display</span>
- <span class="zh">角色权限配置</span><span class="en">Role permission configuration</span>
- <span class="zh">角色增删改查</span><span class="en">Role CRUD operations</span>

#### <span class="zh">3.2.3 个人资料</span><span class="en">3.2.3 Personal Profile</span>

<span class="zh">**路径**: `/home/user/profile`</span><span class="en">**Path**: `/home/user/profile`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">用户个人信息展示</span><span class="en">User personal information display</span>
- <span class="zh">头像上传</span><span class="en">Avatar upload</span>
- <span class="zh">密码修改</span><span class="en">Password change</span>

#### <span class="zh">3.2.4 权限管理</span><span class="en">3.2.4 Permission Management</span>

<span class="zh">**路径**: `/home/user/permission`</span><span class="en">**Path**: `/home/user/permission`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">权限列表管理</span><span class="en">Permission list management</span>
- <span class="zh">权限分配</span><span class="en">Permission assignment</span>

---

### <span class="zh">3.3 AI管理</span><span class="en">3.3 AI Management</span>

#### <span class="zh">3.3.1 智能体管理</span><span class="en">3.3.1 Agent Management</span>

<span class="zh">**路径**: `/home/aimanagement/aiappsmg`</span><span class="en">**Path**: `/home/aimanagement/aiappsmg`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">AI智能体列表展示</span><span class="en">AI agent list display</span>
- <span class="zh">智能体配置（名称、描述、图标、类型）</span><span class="en">Agent configuration (name, description, icon, type)</span>
- <span class="zh">模型配置（聊天模型、嵌入模型、图像模型）</span><span class="en">Model configuration (chat model, embedding model, image model)</span>
- <span class="zh">参数配置（温度、相关性、Token限制）</span><span class="en">Parameter configuration (temperature, relevance, token limit)</span>
- <span class="zh">工具和技能绑定</span><span class="en">Tool and skill binding</span>
- <span class="zh">用户角色绑定</span><span class="en">User role binding</span>

<span class="zh">**智能体配置项**:</span><span class="en">**Agent Configuration Items**:</span>

| <span class="zh">配置项</span><span class="en">Configuration</span> | <span class="zh">说明</span><span class="en">Description</span> | <span class="zh">默认值</span><span class="en">Default</span> |
|--------|------|--------|
| <span class="zh">名称</span><span class="en">Name</span> | <span class="zh">智能体名称</span><span class="en">Agent name</span> | - |
| <span class="zh">描述</span><span class="en">Description</span> | <span class="zh">智能体描述信息</span><span class="en">Agent description</span> | - |
| <span class="zh">图标</span><span class="en">Icon</span> | <span class="zh">智能体图标标识</span><span class="en">Agent icon</span> | windows |
| <span class="zh">类型</span><span class="en">Type</span> | <span class="zh">智能体类型</span><span class="en">Agent type</span> | - |
| <span class="zh">聊天模型</span><span class="en">Chat Model</span> | <span class="zh">选择AI模型</span><span class="en">Select AI model</span> | - |
| <span class="zh">嵌入模型</span><span class="en">Embedding Model</span> | <span class="zh">选择嵌入模型</span><span class="en">Select embedding model</span> | - |
| <span class="zh">温度</span><span class="en">Temperature</span> | <span class="zh">创意程度 (0-100)</span><span class="en">Creativity level (0-100)</span> | 70 |
| <span class="zh">相关性</span><span class="en">Relevance</span> | <span class="zh">匹配程度 (0-100)</span><span class="en">Matching level (0-100)</span> | 60 |
| <span class="zh">提问Token</span><span class="en">Question Token</span> | <span class="zh">最大提问Token数</span><span class="en">Max question token count</span> | 2048 |
| <span class="zh">回答Token</span><span class="en">Answer Token</span> | <span class="zh">最大回答Token数</span><span class="en">Max answer token count</span> | 2048 |

#### <span class="zh">3.3.2 提示词管理</span><span class="en">3.3.2 Prompt Management</span>

<span class="zh">**路径**: `/home/aimanagement/aipromptsmg`</span><span class="en">**Path**: `/home/aimanagement/aipromptsmg`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">提示词模板管理</span><span class="en">Prompt template management</span>
- <span class="zh">提示词增删改查</span><span class="en">Prompt CRUD operations</span>

#### <span class="zh">3.3.3 知识库管理</span><span class="en">3.3.3 Knowledge Base Management</span>

<span class="zh">**路径**: `/home/aimanagement/aikmssmg`</span><span class="en">**Path**: `/home/aimanagement/aikmssmg`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">知识库列表管理</span><span class="en">Knowledge base list management</span>
- <span class="zh">知识库文件上传</span><span class="en">Knowledge base file upload</span>
- <span class="zh">知识库配置</span><span class="en">Knowledge base configuration</span>

#### <span class="zh">3.3.4 模型管理</span><span class="en">3.3.4 Model Management</span>

<span class="zh">**路径**: `/home/aimanagement/aimodelmg`</span><span class="en">**Path**: `/home/aimanagement/aimodelmg`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">AI模型列表管理</span><span class="en">AI model list management</span>
- <span class="zh">模型配置参数设置</span><span class="en">Model configuration parameter settings</span>

#### <span class="zh">3.3.5 技能工具管理</span><span class="en">3.3.5 Skill Tool Management</span>

<span class="zh">**路径**: `/home/aimanagement/aiskilltoolmg`</span><span class="en">**Path**: `/home/aimanagement/aiskilltoolmg`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">技能工具列表管理</span><span class="en">Skill tool list management</span>
- <span class="zh">工具配置</span><span class="en">Tool configuration</span>

---

### <span class="zh">3.4 个人中心</span><span class="en">3.4 Personal Center</span>

#### <span class="zh">3.4.1 AI聊天</span><span class="en">3.4.1 AI Chat</span>

<span class="zh">**路径**: `/home/my/ai-chat`</span><span class="en">**Path**: `/home/my/ai-chat`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">与AI智能体对话</span><span class="en">Chat with AI agent</span>
- <span class="zh">文件上传功能（支持多文件）</span><span class="en">File upload (multi-file support)</span>
- <span class="zh">消息历史记录</span><span class="en">Message history</span>
- <span class="zh">链接高亮与点击跳转</span><span class="en">Link highlighting and click navigation</span>
- <span class="zh">消息内容复制</span><span class="en">Message content copy</span>
- <span class="zh">Token消耗展示（以万为单位）</span><span class="en">Token consumption display (in ten thousands)</span>

<span class="zh">**聊天操作**:</span><span class="en">**Chat Operations**:</span>
| <span class="zh">操作</span><span class="en">Operation</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|
| <span class="zh">发送消息</span><span class="en">Send Message</span> | <span class="zh">输入内容后按Enter或点击发送</span><span class="en">Press Enter or click send after input</span> |
| <span class="zh">上传文件</span><span class="en">Upload File</span> | <span class="zh">点击上传按钮选择文件，发送时一同提交</span><span class="en">Click upload button to select files, submit together with message</span> |
| <span class="zh">复制消息</span><span class="en">Copy Message</span> | <span class="zh">点击消息旁复制按钮</span><span class="en">Click copy button beside message</span> |
| <span class="zh">联网搜索</span><span class="en">Web Search</span> | <span class="zh">开启后支持联网搜索功能</span><span class="en">Enable web search when turned on</span> |

#### <span class="zh">3.4.2 AI任务</span><span class="en">3.4.2 AI Tasks</span>

<span class="zh">**路径**: `/home/my/ai-tasks`</span><span class="en">**Path**: `/home/my/ai-tasks`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">任务列表展示</span><span class="en">Task list display</span>
- <span class="zh">任务状态管理</span><span class="en">Task status management</span>

#### <span class="zh">3.4.3 我的智能体</span><span class="en">3.4.3 My Agents</span>

<span class="zh">**路径**: `/home/my/ai-agents`</span><span class="en">**Path**: `/home/my/ai-agents`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">用户绑定的智能体列表</span><span class="en">User-bound agent list</span>
- <span class="zh">快速访问入口</span><span class="en">Quick access entry</span>

---

### <span class="zh">3.5 系统管理</span><span class="en">3.5 System Management</span>

#### <span class="zh">3.5.1 字典配置</span><span class="en">3.5.1 Dictionary Configuration</span>

<span class="zh">**路径**: `/home/system/dic`</span><span class="en">**Path**: `/home/system/dic`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">系统字典管理</span><span class="en">System dictionary management</span>
- <span class="zh">数据字典增删改查</span><span class="en">Data dictionary CRUD operations</span>

#### <span class="zh">3.5.2 日志管理</span><span class="en">3.5.2 Log Management</span>

<span class="zh">**路径**: `/home/system/log`</span><span class="en">**Path**: `/home/system/log`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">HTTP请求日志查看</span><span class="en">HTTP request log viewing</span>
- <span class="zh">操作日志记录</span><span class="en">Operation log recording</span>

#### <span class="zh">3.5.3 系统公告</span><span class="en">3.5.3 System Announcement</span>

<span class="zh">**路径**: `/home/system/announcement`</span><span class="en">**Path**: `/home/system/announcement`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">公告发布管理</span><span class="en">Announcement publishing management</span>
- <span class="zh">公告列表展示</span><span class="en">Announcement list display</span>

#### <span class="zh">3.5.4 租户管理</span><span class="en">3.5.4 Tenant Management</span>

<span class="zh">**路径**: `/home/system/tenant`</span><span class="en">**Path**: `/home/system/tenant`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">租户信息管理</span><span class="en">Tenant information management</span>
- <span class="zh">租户配置</span><span class="en">Tenant configuration</span>

#### <span class="zh">3.5.5 代码生成器</span><span class="en">3.5.5 Code Generator</span>

<span class="zh">**路径**: `/home/system/code-generator`</span><span class="en">**Path**: `/home/system/code-generator`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">自动化代码生成</span><span class="en">Automated code generation</span>
- <span class="zh">模板配置</span><span class="en">Template configuration</span>

---

### <span class="zh">3.6 组织架构</span><span class="en">3.6 Organizational Structure</span>

#### <span class="zh">3.6.1 部门管理</span><span class="en">3.6.1 Department Management</span>

<span class="zh">**路径**: `/home/department/management`</span><span class="en">**Path**: `/home/department/management`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">部门树结构展示</span><span class="en">Department tree structure display</span>
- <span class="zh">部门信息管理</span><span class="en">Department information management</span>

#### <span class="zh">3.6.2 岗位管理</span><span class="en">3.6.2 Position Management</span>

<span class="zh">**路径**: `/home/position/management`</span><span class="en">**Path**: `/home/position/management`</span>

<span class="zh">**功能说明**:</span><span class="en">**Feature Description**:</span>
- <span class="zh">岗位列表管理</span><span class="en">Position list management</span>
- <span class="zh">岗位信息配置</span><span class="en">Position information configuration</span>

---

## <span class="zh">四、核心功能详解</span><span class="en">IV. Core Feature Details</span>

### <span class="zh">4.1 文件上传</span><span class="en">4.1 File Upload</span>

<span class="zh">在AI聊天页面支持文件上传功能：</span><span class="en">File upload is supported in AI chat page:</span>

1. <span class="zh">**上传操作**:</span><span class="en">**Upload Operation**:</span>
   - <span class="zh">点击上传按钮</span><span class="en">Click upload button</span>
   - <span class="zh">选择一个或多个文件</span><span class="en">Select one or more files</span>
   - <span class="zh">文件将显示在上传列表中</span><span class="en">Files will be displayed in upload list</span>

2. <span class="zh">**发送逻辑**:</span><span class="en">**Send Logic**:</span>
   - <span class="zh">发送消息时自动携带文件信息</span><span class="en">Automatically carry file info when sending message</span>
   - <span class="zh">文件名为 `FileNames` 字段（逗号分隔）</span><span class="en">File name is `FileNames` field (comma separated)</span>
   - <span class="zh">文件URL为 `ContentFileUrls` 字段（逗号分隔）</span><span class="en">File URL is `ContentFileUrls` field (comma separated)</span>
   - <span class="zh">发送成功后自动清空上传列表</span><span class="en">Automatically clear upload list after successful send</span>

3. <span class="zh">**文件展示**:</span><span class="en">**File Display**:</span>
   - <span class="zh">消息下方展示文件链接</span><span class="en">File links displayed below messages</span>
   - <span class="zh">支持点击下载</span><span class="en">Support click to download</span>

### <span class="zh">4.2 链接高亮</span><span class="en">4.2 Link Highlighting</span>

<span class="zh">消息内容中的URL会自动高亮并可点击：</span><span class="en">URLs in message content will be automatically highlighted and clickable:</span>

- <span class="zh">支持 `http://` 和 `https://` 协议</span><span class="en">Supports `http://` and `https://` protocols</span>
- <span class="zh">自动识别Markdown格式中的链接</span><span class="en">Automatically recognizes links in Markdown format</span>
- <span class="zh">点击后在新窗口打开</span><span class="en">Opens in new window when clicked</span>

### <span class="zh">4.3 消息复制</span><span class="en">4.3 Message Copy</span>

<span class="zh">所有消息内容（包括用户发送和AI回复）都支持复制：</span><span class="en">All message content (including user-sent and AI replies) supports copying:</span>

- <span class="zh">点击消息旁的复制按钮</span><span class="en">Click copy button beside message</span>
- <span class="zh">支持现代浏览器剪贴板API</span><span class="en">Supports modern browser clipboard API</span>
- <span class="zh">旧浏览器自动降级处理</span><span class="en">Automatic fallback for older browsers</span>

### <span class="zh">4.4 智能体安全拦截</span><span class="en">4.4 Agent Security Interception</span>

<span class="zh">智能体支持安全拦截功能：</span><span class="en">Agent supports security interception:</span>

- <span class="zh">**开启状态**: 对Python脚本和Shell命令内容进行安全拦截</span><span class="en">**Enabled**: Security interception for Python scripts and Shell commands</span>
- <span class="zh">**默认状态**: 开启</span><span class="en">**Default**: Enabled</span>
- <span class="zh">**配置位置**: 智能体编辑页面的 `IsSecurityIntercept` 开关</span><span class="en">**Configuration**: `IsSecurityIntercept` switch in agent edit page</span>

---

## <span class="zh">五、部署说明</span><span class="en">V. Deployment Instructions</span>

### <span class="zh">5.1 环境变量配置</span><span class="en">5.1 Environment Variable Configuration</span>

<span class="zh">项目包含以下环境配置文件：</span><span class="en">The project includes the following environment configuration files:</span>

| <span class="zh">文件</span><span class="en">File</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|
| `.env` | <span class="zh">默认配置</span><span class="en">Default configuration</span> |
| `.env.development` | <span class="zh">开发环境</span><span class="en">Development environment</span> |
| `.env.pre` | <span class="zh">预发布环境</span><span class="en">Pre-release environment</span> |

### <span class="zh">5.2 依赖安装</span><span class="en">5.2 Dependency Installation</span>

```bash
npm install
```

### <span class="zh">5.3 开发模式</span><span class="en">5.3 Development Mode</span>

```bash
npm run serve           # <span class="zh">默认模式</span><span class="en">Default mode</span>
npm run serve:dev       # <span class="zh">开发环境</span><span class="en">Development environment</span>
npm run serve:pre       # <span class="zh">预发布环境</span><span class="en">Pre-release environment</span>
```

### <span class="zh">5.4 生产构建</span><span class="en">5.4 Production Build</span>

```bash
npm run build           # <span class="zh">默认构建</span><span class="en">Default build</span>
npm run build:dev       # <span class="zh">开发环境构建</span><span class="en">Development build</span>
npm run build:pre       # <span class="zh">预发布环境构建</span><span class="en">Pre-release build</span>
```

### <span class="zh">5.5 代码检查</span><span class="en">5.5 Code Linting</span>

```bash
npm run lint
```

---

## <span class="zh">六、登录与权限</span><span class="en">VI. Login and Permissions</span>

### <span class="zh">6.1 登录流程</span><span class="en">6.1 Login Process</span>

1. <span class="zh">访问系统首页自动跳转至登录页</span><span class="en">Access system homepage, automatically redirect to login page</span>
2. <span class="zh">输入账号密码进行登录</span><span class="en">Enter account password to login</span>
3. <span class="zh">登录成功后跳转至仪表盘</span><span class="en">Redirect to dashboard after successful login</span>

### <span class="zh">6.2 权限验证</span><span class="en">6.2 Permission Verification</span>

- <span class="zh">系统采用路由守卫进行权限验证</span><span class="en">System uses route guards for permission verification</span>
- <span class="zh">未登录用户自动重定向至登录页</span><span class="en">Unauthenticated users automatically redirected to login page</span>
- <span class="zh">用户信息缓存有效期为10分钟</span><span class="en">User info cache valid for 10 minutes</span>

---

## <span class="zh">七、主题切换</span><span class="en">VII. Theme Switching</span>

<span class="zh">系统支持多种主题风格：</span><span class="en">System supports multiple theme styles:</span>

| <span class="zh">主题</span><span class="en">Theme</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|
| <span class="zh">企业蓝</span><span class="en">Enterprise Blue</span> | <span class="zh">蓝色系主题</span><span class="en">Blue theme</span> |
| <span class="zh">墨黑</span><span class="en">Ink Black</span> | <span class="zh">深色主题</span><span class="en">Dark theme</span> |
| <span class="zh">灰蓝</span><span class="en">Gray Blue</span> | <span class="zh">默认主题</span><span class="en">Default theme</span> |
| <span class="zh">绿色</span><span class="en">Green</span> | <span class="zh">绿色系主题</span><span class="en">Green theme</span> |

<span class="zh">**切换方式**: 点击顶部导航栏的"主题"按钮选择主题</span><span class="en">**Switch**: Click "Theme" button in top navigation bar</span>

---

## <span class="zh">八、常见问题</span><span class="en">VIII. FAQ</span>

### <span class="zh">8.1 页面加载异常</span><span class="en">8.1 Page Loading Issues</span>

<span class="zh">**问题**: 页面空白或加载缓慢</span><span class="en">**Problem**: Blank page or slow loading</span>

<span class="zh">**解决方案**:</span><span class="en">**Solutions**:</span>
1. <span class="zh">检查网络连接</span><span class="en">Check network connection</span>
2. <span class="zh">清除浏览器缓存</span><span class="en">Clear browser cache</span>
3. <span class="zh">检查控制台错误信息</span><span class="en">Check console error messages</span>
4. <span class="zh">联系系统管理员</span><span class="en">Contact system administrator</span>

### <span class="zh">8.2 登录失败</span><span class="en">8.2 Login Failure</span>

<span class="zh">**问题**: 账号密码正确但无法登录</span><span class="en">**Problem**: Correct credentials but cannot login</span>

<span class="zh">**解决方案**:</span><span class="en">**Solutions**:</span>
1. <span class="zh">检查账号密码是否正确</span><span class="en">Check if credentials are correct</span>
2. <span class="zh">检查账号是否被禁用</span><span class="en">Check if account is disabled</span>
3. <span class="zh">联系管理员重置密码</span><span class="en">Contact admin to reset password</span>

### <span class="zh">8.3 文件上传失败</span><span class="en">8.3 File Upload Failure</span>

<span class="zh">**问题**: 文件上传提示失败</span><span class="en">**Problem**: File upload failed</span>

<span class="zh">**解决方案**:</span><span class="en">**Solutions**:</span>
1. <span class="zh">检查文件大小（建议不超过10MB）</span><span class="en">Check file size (recommended ≤ 10MB)</span>
2. <span class="zh">检查文件格式是否支持</span><span class="en">Check if file format is supported</span>
3. <span class="zh">检查网络连接</span><span class="en">Check network connection</span>

### <span class="zh">8.4 分页功能异常</span><span class="en">8.4 Pagination Issues</span>

<span class="zh">**问题**: 分页显示异常或数据重复</span><span class="en">**Problem**: Abnormal pagination display or duplicate data</span>

<span class="zh">**解决方案**:</span><span class="en">**Solutions**:</span>
1. <span class="zh">刷新页面重新加载</span><span class="en">Refresh page to reload</span>
2. <span class="zh">检查分页参数配置</span><span class="en">Check pagination parameter configuration</span>
3. <span class="zh">联系开发人员排查接口问题</span><span class="en">Contact developers to troubleshoot API issues</span>

---

## <span class="zh">九、更新日志</span><span class="en">IX. Update Log</span>

### <span class="zh">v1.0.0</span><span class="en">v1.0.0</span>
- <span class="zh">系统基础框架搭建</span><span class="en">System basic framework setup</span>
- <span class="zh">用户管理模块</span><span class="en">User management module</span>
- <span class="zh">AI智能体管理模块</span><span class="en">AI agent management module</span>
- <span class="zh">知识库管理模块</span><span class="en">Knowledge base management module</span>
- <span class="zh">消息通知模块</span><span class="en">Message notification module</span>

### <span class="zh">v1.0.1</span><span class="en">v1.0.1</span>
- <span class="zh">修复消息时间显示问题</span><span class="en">Fixed message time display issue</span>
- <span class="zh">添加Token消耗展示</span><span class="en">Added token consumption display</span>
- <span class="zh">优化URL链接高亮功能</span><span class="en">Optimized URL link highlighting</span>
- <span class="zh">修复文件上传后列表不清空问题</span><span class="en">Fixed upload list not clearing after upload</span>

### <span class="zh">v1.0.2</span><span class="en">v1.0.2</span>
- <span class="zh">添加智能体安全拦截功能</span><span class="en">Added agent security interception</span>
- <span class="zh">修复权限验证问题</span><span class="en">Fixed permission verification issue</span>
- <span class="zh">优化分页功能</span><span class="en">Optimized pagination</span>
- <span class="zh">添加主题切换功能</span><span class="en">Added theme switching</span>

---

## <span class="zh">十、联系方式</span><span class="en">X. Contact</span>

<span class="zh">如有问题或建议，请联系系统管理员。</span><span class="en">For questions or suggestions, please contact system administrator.</span>

---

*<span class="zh">文档版本</span><span class="en">Document Version</span>: v1.0.2*  
*<span class="zh">生成日期</span><span class="en">Generated Date</span>: June 2026