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

# <span class="zh">NetCoreKevin 系统开发-运维-使用文档</span><span class="en">NetCoreKevin System Development-Operations-Usage Documentation</span>

---

## <span class="zh">一、项目概述</span><span class="en">I. Project Overview</span>

### <span class="zh">1.1 项目简介</span><span class="en">1.1 Project Introduction</span>

<span class="zh">**NetCoreKevin** 是基于 .NET 9 构建的企业级 AI中台智能体 SaaS 前后端分离架构，集成了 AI 知识库智能体、Skill技能管理、本地离线AI模型调用、智能体技能可控加载、AI联网搜索、智能体权限管控、一库多租户、分布式系统、微服务架构等核心能力。</span><span class="en">**NetCoreKevin** is an enterprise-level AI Platform Agent SaaS architecture built on .NET 9 with separation of frontend and backend, integrating core capabilities including AI knowledge base agents, Skill management, local offline AI model calling, controllable skill loading, AI web search, agent permission management, single-database multi-tenancy, distributed systems, and microservices architecture.</span>

### <span class="zh">1.2 核心功能模块</span><span class="en">1.2 Core Feature Modules</span>

| <span class="zh">模块</span><span class="en">Module</span> | <span class="zh">功能描述</span><span class="en">Feature Description</span> | <span class="zh">技术实现</span><span class="en">Technical Implementation</span> |
|------|----------|----------|
| <span class="zh">**AI 智能体**</span><span class="en">**AI Agent**</span> | <span class="zh">基于 AgentFramework 的智能代理系统，支持多步推理与任务自动化</span><span class="en">Intelligent agent system based on AgentFramework, supporting multi-step reasoning and task automation</span> | AgentFramework 1.9, OpenAI API Protocol |
| <span class="zh">**知识库系统**</span><span class="en">**Knowledge Base System**</span> | <span class="zh">使用 Qdrant 向量数据库实现 RAG 检索增强</span><span class="en">RAG retrieval enhancement using Qdrant vector database</span> | Qdrant、Ollama |
| <span class="zh">**任务调度**</span><span class="en">**Task Scheduling**</span> | <span class="zh">基于 Hangfire 的定时任务调度系统</span><span class="en">Scheduled task scheduling system based on Hangfire</span> | Hangfire + Redis |
| <span class="zh">**权限管理**</span><span class="en">**Permission Management**</span> | <span class="zh">基于 IdentityServer4 的用户认证与细粒度授权</span><span class="en">User authentication and fine-grained authorization based on IdentityServer4</span> | IDS4、JWT |
| <span class="zh">**消息队列**</span><span class="en">**Message Queue**</span> | <span class="zh">基于 CAP 的分布式事件通信</span><span class="en">Distributed event communication based on CAP</span> | CAP、RabbitMQ |
| <span class="zh">**文件存储**</span><span class="en">**File Storage**</span> | <span class="zh">支持多种云存储服务的文件上传下载</span><span class="en">File upload/download supporting multiple cloud storage services</span> | <span class="zh">腾讯云 COS、阿里云 OSS、七牛云</span><span class="en">Tencent Cloud COS, Alibaba Cloud OSS, Qiniu Cloud</span> |
| <span class="zh">**代码生成器**</span><span class="en">**Code Generator**</span> | <span class="zh">自动化代码生成工具</span><span class="en">Automated code generation tool</span> | <span class="zh">模板引擎</span><span class="en">Template Engine</span> |

---

## <span class="zh">二、技术架构</span><span class="en">II. Technical Architecture</span>

### <span class="zh">2.1 技术栈</span><span class="en">2.1 Technology Stack</span>

| <span class="zh">分类</span><span class="en">Category</span> | <span class="zh">技术</span><span class="en">Technology</span> | <span class="zh">版本</span><span class="en">Version</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|------|------|
| <span class="zh">语言</span><span class="en">Language</span> | C# | 9.0 | .NET 9 LTS |
| <span class="zh">框架</span><span class="en">Framework</span> | ASP.NET Core | 9.0 | <span class="zh">Web API 框架</span><span class="en">Web API Framework</span> |
| <span class="zh">ORM</span><span class="en">ORM</span> | Entity Framework Core | 9.0 | <span class="zh">数据访问层</span><span class="en">Data Access Layer</span> |
| <span class="zh">数据库</span><span class="en">Database</span> | MySQL | 8.0+ | <span class="zh">主数据库</span><span class="en">Main Database</span> |
| <span class="zh">缓存</span><span class="en">Cache</span> | Redis | 7.0+ | <span class="zh">分布式缓存</span><span class="en">Distributed Cache</span> |
| <span class="zh">向量数据库</span><span class="en">Vector Database</span> | Qdrant | 1.7+ | <span class="zh">AI 知识库</span><span class="en">AI Knowledge Base</span> |
| <span class="zh">任务调度</span><span class="en">Task Scheduling</span> | Hangfire | 1.8+ | <span class="zh">定时任务</span><span class="en">Scheduled Tasks</span> |
| <span class="zh">消息总线</span><span class="en">Message Bus</span> | CAP | 7.0+ | <span class="zh">分布式事件</span><span class="en">Distributed Events</span> |
| <span class="zh">认证授权</span><span class="en">Authentication</span> | IdentityServer4 | 6.3+ | OAuth2.0/OIDC |
| <span class="zh">AI 框架</span><span class="en">AI Framework</span> | AgentFramework 1.9 | 1.9+ | <span class="zh">AI 智能体</span><span class="en">AI Agent</span> |
| <span class="zh">Skill 管理</span><span class="en">Skill Management</span> | SkillManager | 1.0+ | <span class="zh">动态加载和管理 Skill</span><span class="en">Dynamic loading and management of Skills</span> |
| <span class="zh">模型调用</span><span class="en">Model Calling</span> | Ollama | 1.4+ | <span class="zh">本地离线模型调用</span><span class="en">Local offline model calling</span> |

### <span class="zh">2.2 架构设计</span><span class="en">2.2 Architecture Design</span>

```
┌─────────────────────────────────────────────────────────────────┐
│                      <span class="zh">前端层 (Vue3)</span><span class="en">Frontend Layer (Vue3)</span>                              │
├─────────────────────────────────────────────────────────────────┤
│                      <span class="zh">API 网关层</span><span class="en">API Gateway Layer</span>                                  │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │  App.WebApi │ │  <span class="zh">AI 模块</span><span class="en">AI Module</span>    │ │  <span class="zh">任务调度</span><span class="en">Task Scheduling</span>   │ │  <span class="zh">其他服务</span><span class="en">Other Services</span> │ │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └─────┬─────┘ │
├─────────┼────────────────┼────────────────┼────────────────┼─────┤
│  ┌──────▼──────┐ ┌──────▼──────┐ ┌──────▼──────┐               │
│  │  <span class="zh">应用服务层</span><span class="en">Application Service</span> │ │  <span class="zh">领域服务层</span><span class="en">Domain Service</span> │ │  <span class="zh">仓储层</span><span class="en">Repository</span>     │               │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘               │
├─────────┼────────────────┼────────────────┼─────────────────────┤
│                      <span class="zh">数据访问层</span><span class="en">Data Access Layer</span>                                  │
├─────────┼────────────────┼────────────────┼─────────────────────┤
│  MySQL  │    Redis      │    Qdrant     │    RabbitMQ         │
└─────────────────────────────────────────────────────────────────┘
```

### <span class="zh">2.3 项目结构</span><span class="en">2.3 Project Structure</span>

```
kevin.abp.core/
├── App/                            # <span class="zh">应用模块</span><span class="en">Application Module</span>
│   ├── AppShare/                   # <span class="zh">共享组件</span><span class="en">Shared Components</span>
│   ├── Application/                # <span class="zh">应用服务层</span><span class="en">Application Service Layer</span>
│   ├── Domain/                     # <span class="zh">领域层</span><span class="en">Domain Layer</span>
│   ├── RepositorieRps/             # <span class="zh">仓储实现</span><span class="en">Repository Implementation</span>
│   └── WebApi/                     # <span class="zh">API 入口</span><span class="en">API Entry</span>
├── Kevin/                          # <span class="zh">核心模块</span><span class="en">Core Module</span>
│   ├── Application/                # <span class="zh">核心应用服务</span><span class="en">Core Application Services</span>
│   │   └── Services/
│   │       ├── AI/                 # <span class="zh">AI 服务</span><span class="en">AI Services</span>
│   │       └── ...                 # <span class="zh">其他服务</span><span class="en">Other Services</span>
│   ├── Domain/                     # <span class="zh">核心领域模型</span><span class="en">Core Domain Models</span>
│   │   ├── Entities/               # <span class="zh">实体定义</span><span class="en">Entity Definitions</span>
│   │   ├── Interfaces/             # <span class="zh">接口定义</span><span class="en">Interface Definitions</span>
│   │   └── Bases/                  # <span class="zh">基类</span><span class="en">Base Classes</span>
│   ├── Kevin.EntityFrameworkCore/  # <span class="zh">EF Core 实现</span><span class="en">EF Core Implementation</span>
│   ├── Kevin.Web.Basics/           # <span class="zh">Web 基础组件</span><span class="en">Web Basic Components</span>
│   └── kevin.CodeGenerator/        # <span class="zh">代码生成器</span><span class="en">Code Generator</span>
├── Doc/                            # <span class="zh">文档资源</span><span class="en">Documentation Resources</span>
└── InitData/                       # <span class="zh">初始化数据</span><span class="en">Initialization Data</span>
```

---

## <span class="zh">三、环境要求</span><span class="en">III. Environment Requirements</span>

### <span class="zh">3.1 必备依赖</span><span class="en">3.1 Required Dependencies</span>

| <span class="zh">依赖</span><span class="en">Dependency</span> | <span class="zh">版本要求</span><span class="en">Version Requirement</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|----------|------|
| .NET SDK | 9.0+ | <span class="zh">开发环境</span><span class="en">Development Environment</span> |
| MySQL | 8.0+ | <span class="zh">数据库</span><span class="en">Database</span> |
| Redis | 7.0+ | <span class="zh">缓存、Hangfire</span><span class="en">Cache, Hangfire</span> |
| Qdrant | 1.7+ | <span class="zh">向量数据库（AI 功能）</span><span class="en">Vector Database (AI Features)</span> |
| Node.js | 18+ | <span class="zh">前端开发（可选）</span><span class="en">Frontend Development (Optional)</span> |

### <span class="zh">3.2 可选依赖</span><span class="en">3.2 Optional Dependencies</span>

| <span class="zh">依赖</span><span class="en">Dependency</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|
| RabbitMQ | <span class="zh">CAP 消息队列</span><span class="en">CAP Message Queue</span> |
| Consul | <span class="zh">服务发现</span><span class="en">Service Discovery</span> |
| Ollama | <span class="zh">本地大语言模型</span><span class="en">Local Large Language Model</span> |

---

## <span class="zh">四、安装配置</span><span class="en">IV. Installation and Configuration</span>

### <span class="zh">4.1 数据库配置</span><span class="en">4.1 Database Configuration</span>

<span class="zh">**appsettings.json** 中的连接字符串配置：</span><span class="en">**appsettings.json** connection string configuration:</span>

```json
{
  "ConnectionStrings": {
    "dbConnection": "server=127.0.0.1;port=3306;database=kevin_app;user id=root;password=admin123;Convert Zero Datetime=True;TreatTinyAsBoolean=false;AllowLoadLocalInfile=true;Charset=utf8;Command Timeout=120;",
    "redisConnection": "127.0.0.1:6379,DefaultDatabase=0,password=123456"
  }
}
```

### <span class="zh">4.2 数据库迁移</span><span class="en">4.2 Database Migration</span>

<span class="zh">在 **Package Manager Console** 中执行：</span><span class="en">Execute in **Package Manager Console**:</span>

```powershell
# <span class="zh">1. 选择默认项目为 Kevin.EntityFrameworkCore</span><span class="en">1. Select Kevin.EntityFrameworkCore as default project</span>
# <span class="zh">2. 创建迁移</span><span class="en">2. Create migration</span>
Add-Migration "<span class="zh">初始化数据库</span><span class="en">Initialize Database</span>"

# <span class="zh">3. 应用迁移</span><span class="en">3. Apply migration</span>
Update-Database
```

### <span class="zh">4.3 环境变量配置</span><span class="en">4.3 Environment Variable Configuration</span>

<span class="zh">**Windows PowerShell：**</span><span class="en">**Windows PowerShell:**</span>
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
```

<span class="zh">**Linux/macOS：**</span><span class="en">**Linux/macOS:**</span>
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

---

## <span class="zh">五、启动运行</span><span class="en">V. Startup and Running</span>

### <span class="zh">5.1 开发环境启动</span><span class="en">5.1 Development Environment Startup</span>

<span class="zh">**方式一：使用 Visual Studio**</span><span class="en">**Method 1: Using Visual Studio**</span>
1. <span class="zh">打开解决方案 `kevin.abp.core.sln`</span><span class="en">Open solution `kevin.abp.core.sln`</span>
2. <span class="zh">设置 `App.WebApi` 为启动项目</span><span class="en">Set `App.WebApi` as startup project</span>
3. <span class="zh">按 F5 启动调试</span><span class="en">Press F5 to start debugging</span>

<span class="zh">**方式二：使用命令行**</span><span class="en">**Method 2: Using Command Line**</span>
```bash
cd App/WebApi
dotnet run --environment Development
```

### <span class="zh">5.2 服务访问</span><span class="en">5.2 Service Access</span>

| <span class="zh">服务</span><span class="en">Service</span> | <span class="zh">地址</span><span class="en">Address</span> |
|------|------|
| <span class="zh">API 接口</span><span class="en">API Interface</span> | http://localhost:9901 |
| <span class="zh">Swagger 文档</span><span class="en">Swagger Documentation</span> | http://localhost:9901/swagger |
| <span class="zh">Hangfire 面板</span><span class="en">Hangfire Dashboard</span> | http://localhost:9901/pchangfire |

### <span class="zh">5.3 默认账户</span><span class="en">5.3 Default Account</span>

| <span class="zh">账户</span><span class="en">Account</span> | <span class="zh">密码</span><span class="en">Password</span> | <span class="zh">租户</span><span class="en">Tenant</span> |
|------|------|------|
| admin | 123456 | 1000 |

---

## <span class="zh">六、AI 智能体配置</span><span class="en">VI. AI Agent Configuration</span>

### <span class="zh">6.1 Qdrant 配置</span><span class="en">6.1 Qdrant Configuration</span>

```json
{
  "QdrantClientSetting": {
    "Url": "localhost"
  }
}
```

### <span class="zh">6.2 模型配置</span><span class="en">6.2 Model Configuration</span>

<span class="zh">**智谱 AI 配置：**</span><span class="en">**Zhipu AI Configuration:**</span>
```json
{
  "OllamaApiSetting": {
    "Url": "https://open.bigmodel.cn/api/paas/v4/embeddings",
    "DefaultModel": "embedding-3",
    "ApiKey": "your-api-key"
  }
}
```

<span class="zh">**Ollama 本地模型配置：**</span><span class="en">**Ollama Local Model Configuration:**</span>
```json
{
  "OllamaApiSetting": {
    "Url": "http://localhost:11434/api/embeddings",
    "DefaultModel": "qwen3:4b",
    "ApiKey": ""
  }
}
```

### <span class="zh">6.3 AI 智能体使用流程</span><span class="en">6.3 AI Agent Usage Process</span>

1. <span class="zh">**注册 AI 账户**</span><span class="en">**Register AI Account**</span> → <span class="zh">获取 API Key</span><span class="en">Get API Key</span>
2. <span class="zh">**配置模型**</span><span class="en">**Configure Models**</span> → <span class="zh">设置向量模型和对话模型</span><span class="en">Set vector model and chat model</span>
3. <span class="zh">**新建知识库**</span><span class="en">**Create Knowledge Base**</span> → <span class="zh">上传文档，选择向量模型</span><span class="en">Upload documents, select vector model</span>
4. <span class="zh">**配置智能体**</span><span class="en">**Configure Agent**</span> → <span class="zh">绑定技能工具</span><span class="en">Bind skill tools</span>
5. <span class="zh">**开始对话**</span><span class="en">**Start Conversation**</span> → <span class="zh">与 AI 智能体交互</span><span class="en">Interact with AI agent</span>

---

## <span class="zh">七、API 接口说明</span><span class="en">VII. API Interface Documentation</span>

### <span class="zh">7.1 认证接口</span><span class="en">7.1 Authentication Interfaces</span>

| <span class="zh">接口</span><span class="en">Interface</span> | <span class="zh">方法</span><span class="en">Method</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|------|
| `/api/Authorize/Login` | POST | <span class="zh">用户登录</span><span class="en">User Login</span> |
| `/api/Authorize/Logout` | POST | <span class="zh">用户登出</span><span class="en">User Logout</span> |
| `/api/Authorize/RefreshToken` | POST | <span class="zh">刷新 Token</span><span class="en">Refresh Token</span> |

<span class="zh">**登录请求示例：**</span><span class="en">**Login Request Example:**</span>
```json
{
  "userName": "admin",
  "password": "123456",
  "tenantId": 1000
}
```

### <span class="zh">7.2 AI 接口</span><span class="en">7.2 AI Interfaces</span>

| <span class="zh">接口</span><span class="en">Interface</span> | <span class="zh">方法</span><span class="en">Method</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|------|
| `/api/AIChats/Chat` | POST | <span class="zh">发送聊天消息</span><span class="en">Send Chat Message</span> |
| `/api/AIKmss/Create` | POST | <span class="zh">创建知识库</span><span class="en">Create Knowledge Base</span> |
| `/api/AIKmss/UploadFile` | POST | <span class="zh">上传知识库文档</span><span class="en">Upload Knowledge Base Document</span> |
| `/api/AIModels/GetAll` | GET | <span class="zh">获取模型列表</span><span class="en">Get Model List</span> |

### <span class="zh">7.3 任务调度接口</span><span class="en">7.3 Task Scheduling Interfaces</span>

| <span class="zh">接口</span><span class="en">Interface</span> | <span class="zh">方法</span><span class="en">Method</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|------|
| `/api/AITasks/AddOrUpdateCronTask` | POST | <span class="zh">创建/更新定时任务</span><span class="en">Create/Update Scheduled Task</span> |
| `/api/AITasks/RemoveCronTask` | POST | <span class="zh">删除定时任务</span><span class="en">Delete Scheduled Task</span> |
| `/api/AITasks/TriggerCronTask` | POST | <span class="zh">立即触发任务</span><span class="en">Trigger Task Immediately</span> |

### <span class="zh">7.4 代码生成器接口</span><span class="en">7.4 Code Generator Interfaces</span>

| <span class="zh">接口</span><span class="en">Interface</span> | <span class="zh">方法</span><span class="en">Method</span> | <span class="zh">说明</span><span class="en">Description</span> |
|------|------|------|
| `/api/CodeGenerator/GetAreaNames` | GET | <span class="zh">获取区域列表</span><span class="en">Get Area List</span> |
| `/api/CodeGenerator/GetAreaNameEntityItems` | GET | <span class="zh">获取实体列表</span><span class="en">Get Entity List</span> |
| `/api/CodeGenerator/BulidCode` | POST | <span class="zh">生成代码</span><span class="en">Generate Code</span> |

---

## <span class="zh">八、开发规范</span><span class="en">VIII. Development Standards</span>

### <span class="zh">8.1 代码风格</span><span class="en">8.1 Code Style</span>

- <span class="zh">使用 **PascalCase** 命名类、接口、方法</span><span class="en">Use **PascalCase** for classes, interfaces, methods</span>
- <span class="zh">使用 **camelCase** 命名参数、局部变量</span><span class="en">Use **camelCase** for parameters, local variables</span>
- <span class="zh">文件编码使用 **UTF-8 无 BOM**</span><span class="en">File encoding uses **UTF-8 without BOM**</span>
- <span class="zh">每行代码不超过 120 个字符</span><span class="en">Each line does not exceed 120 characters</span>

### <span class="zh">8.2 目录结构规范</span><span class="en">8.2 Directory Structure Standards</span>

```
Application/Services/
├── [ModuleName]/
│   ├── [ServiceName]Service.cs      # <span class="zh">服务实现</span><span class="en">Service Implementation</span>
│   └── Dto/
│       ├── [EntityName]Dto.cs       # <span class="zh">数据传输对象</span><span class="en">Data Transfer Object</span>
│       └── [EntityName]Input.cs     # <span class="zh">输入参数</span><span class="en">Input Parameters</span>

Domain/
├── Entities/
│   └── [EntityName].cs              # <span class="zh">实体定义</span><span class="en">Entity Definition</span>
├── Interfaces/
│   ├── IRepositories/
│   │   └── I[EntityName]Rp.cs       # <span class="zh">仓储接口</span><span class="en">Repository Interface</span>
│   └── IServices/
│       └── I[ServiceName]Service.cs # <span class="zh">服务接口</span><span class="en">Service Interface</span>
```

### <span class="zh">8.3 异常处理</span><span class="en">8.3 Exception Handling</span>

- <span class="zh">使用统一的异常处理中间件</span><span class="en">Use unified exception handling middleware</span>
- <span class="zh">捕获异常时记录日志（使用 log4net）</span><span class="en">Record logs when catching exceptions (using log4net)</span>
- <span class="zh">API 返回统一格式的错误响应</span><span class="en">API returns unified format error response</span>

### <span class="zh">8.4 日志规范</span><span class="en">8.4 Log Standards</span>

```csharp
// <span class="zh">使用 log4net 记录日志</span><span class="en">Use log4net to record logs</span>
LogHelper<MyService>.logger.Info("<span class="zh">信息级别日志</span><span class="en">Info level log</span>");
LogHelper<MyService>.logger.Warn("<span class="zh">警告级别日志</span><span class="en">Warning level log</span>");
LogHelper<MyService>.logger.Error("<span class="zh">错误级别日志</span><span class="en">Error level log</span>", exception);
```

### <span class="zh">8.5 代码生成器使用</span><span class="en">8.5 Code Generator Usage</span>

#### <span class="zh">8.5.1 功能概述</span><span class="en">8.5.1 Feature Overview</span>

<span class="zh">代码生成器可以根据数据库实体自动生成：</span><span class="en">Code generator can automatically generate based on database entities:</span>
- <span class="zh">仓储接口（IRepository）</span><span class="en">Repository Interface (IRepository)</span>
- <span class="zh">仓储实现（Repository）</span><span class="en">Repository Implementation (Repository)</span>
- <span class="zh">服务接口（IService）</span><span class="en">Service Interface (IService)</span>
- <span class="zh">服务实现（Service）</span><span class="en">Service Implementation (Service)</span>

#### <span class="zh">8.5.2 配置说明</span><span class="en">8.5.2 Configuration Instructions</span>

<span class="zh">编辑 `appsettings.json`：</span><span class="en">Edit `appsettings.json`:</span>
```json
{
  "CodeGeneratorSetting": {
    "CodeGeneratorItems": [
      {
        "AreaName": "App.WebApi.v1",
        "AreaPath": "App.Domain.Entities",
        "IRpBulidPath": "App.Domain.Interfaces.Repositorie.v1",
        "RpBulidPath": "App.RepositorieRps.Repositories.v1",
        "IServiceBulidPath": "App.Domain.Interfaces.Services.v1",
        "ServiceBulidPath": "App.Application.Services.v1"
      }
    ]
  }
}
```

#### <span class="zh">8.5.3 使用步骤</span><span class="en">8.5.3 Usage Steps</span>

1. <span class="zh">**获取区域列表**</span><span class="en">**Get Area List**</span>
```bash
GET /api/CodeGenerator/GetAreaNames
```

2. <span class="zh">**获取实体列表**</span><span class="en">**Get Entity List**</span>
```bash
GET /api/CodeGenerator/GetAreaNameEntityItems?name=App.WebApi.v1
```

3. <span class="zh">**生成代码**</span><span class="en">**Generate Code**</span>
```bash
POST /api/CodeGenerator/BulidCode
Content-Type: application/json

[
  {
    "EntityName": "MyEntity",
    "AreaName": "App.WebApi.v1"
  }
]
```

#### <span class="zh">8.5.4 注意事项</span><span class="en">8.5.4 Notes</span>

- <span class="zh">仅超级管理员有权限使用代码生成器</span><span class="en">Only super administrators have permission to use the code generator</span>
- <span class="zh">生成的代码会覆盖现有文件，请谨慎操作</span><span class="en">Generated code will overwrite existing files, please proceed with caution</span>
- <span class="zh">建议在生成前备份相关文件</span><span class="en">It is recommended to backup related files before generation</span>

### <span class="zh">8.6 自定义AI工具开发</span><span class="en">8.6 Custom AI Tool Development</span>

#### <span class="zh">8.6.1 开发步骤</span><span class="en">8.6.1 Development Steps</span>

1. <span class="zh">**创建工具接口**</span><span class="en">**Create Tool Interface**</span>

```csharp
public interface IMyCustomToolService
{
    [Description("<span class="zh">自定义工具描述</span><span class="en">Custom tool description</span>")]
    Task<string> MyToolMethod(
        [Description("<span class="zh">参数1说明</span><span class="en">Parameter 1 description</span>")] string param1,
        [Description("<span class="zh">参数2说明</span><span class="en">Parameter 2 description</span>")] int param2
    );
}
```

2. <span class="zh">**实现工具服务**</span><span class="en">**Implement Tool Service**</span>

```csharp
public class MyCustomToolService : IMyCustomToolService
{
    public async Task<string> MyToolMethod(string param1, int param2)
    {
        // <span class="zh">实现逻辑</span><span class="en">Implementation logic</span>
        return "<span class="zh">执行结果</span><span class="en">Execution result</span>";
    }
}
```

3. <span class="zh">**注册工具到容器**</span><span class="en">**Register Tool to Container**</span>

<span class="zh">在模块初始化中注册服务：</span><span class="en">Register service in module initialization:</span>
```csharp
services.AddTransient<IMyCustomToolService, MyCustomToolService>();
```

4. <span class="zh">**配置工具到智能体**</span><span class="en">**Configure Tool to Agent**</span>

<span class="zh">在 `AIAgentToolSkillService.cs` 中添加工具注册：</span><span class="en">Add tool registration in `AIAgentToolSkillService.cs`:</span>
```csharp
aiTools.Add(
    AIFunctionFactory.Create(_myCustomToolService.MyToolMethod,
    new AIFunctionFactoryOptions
    {
        Name = "MyToolMethod",
        Description = "<span class="zh">自定义工具描述</span><span class="en">Custom tool description</span>"
    }
));
```

#### <span class="zh">8.6.2 工具开发规范</span><span class="en">8.6.2 Tool Development Standards</span>

- <span class="zh">使用 `[Description]` 属性添加参数说明</span><span class="en">Use `[Description]` attribute to add parameter descriptions</span>
- <span class="zh">返回类型推荐使用 `Task<string>`</span><span class="en">Recommended return type is `Task<string>`</span>
- <span class="zh">错误返回以 `❌` 开头的字符串</span><span class="en">Error returns string starting with `❌`</span>
- <span class="zh">支持的参数类型：string, int, bool, List<string></span><span class="en">Supported parameter types: string, int, bool, List<string></span>

---

## <span class="zh">九、运维部署</span><span class="en">IX. Operations and Deployment</span>

### <span class="zh">9.1 Docker 部署</span><span class="en">9.1 Docker Deployment</span>

<span class="zh">**Dockerfile 示例：**</span><span class="en">**Dockerfile Example:**</span>
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["App/WebApi/App.WebApi.csproj", "App/WebApi/"]
RUN dotnet restore "App/WebApi/App.WebApi.csproj"
COPY . .
WORKDIR "/src/App/WebApi"
RUN dotnet build "App.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.WebApi.dll"]
```

<span class="zh">**Docker Compose 示例：**</span><span class="en">**Docker Compose Example:**</span>
```yaml
version: '3.8'
services:
  webapi:
    build: .
    ports:
      - "9901:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__dbConnection=server=mysql;port=3306;database=kevin_app;user id=root;password=admin123
    depends_on:
      - mysql
      - redis

  mysql:
    image: mysql:8.0
    ports:
      - "3306:3306"
    environment:
      - MYSQL_ROOT_PASSWORD=admin123
      - MYSQL_DATABASE=kevin_app

  redis:
    image: redis:7.0
    ports:
      - "6379:6379"
```

### <span class="zh">9.2 Linux 系统服务部署</span><span class="en">9.2 Linux System Service Deployment</span>

<span class="zh">**创建 systemd 服务文件 `/etc/systemd/system/kevin-webapi.service`：**</span><span class="en">**Create systemd service file `/etc/systemd/system/kevin-webapi.service`:**</span>
```ini
[Unit]
Description=NetCoreKevin Web API
After=network.target

[Service]
Type=simple
User=www-data
WorkingDirectory=/var/www/kevin
ExecStart=/usr/bin/dotnet /var/www/kevin/App.WebApi.dll
Restart=always
RestartSec=5
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

<span class="zh">**启动服务：**</span><span class="en">**Start Service:**</span>
```bash
sudo systemctl daemon-reload
sudo systemctl start kevin-webapi
sudo systemctl enable kevin-webapi
```

---

## <span class="zh">十、监控运维</span><span class="en">X. Monitoring and Operations</span>

### <span class="zh">10.1 日志管理</span><span class="en">10.1 Log Management</span>

<span class="zh">日志文件位于 `App/WebApi/Logs/` 目录：</span><span class="en">Log files are located in `App/WebApi/Logs/` directory:</span>

| <span class="zh">日志文件</span><span class="en">Log File</span> | <span class="zh">说明</span><span class="en">Description</span> |
|----------|------|
| `log_*.txt` | <span class="zh">应用日志</span><span class="en">Application Logs</span> |
| `error_*.txt` | <span class="zh">错误日志</span><span class="en">Error Logs</span> |
| `http_*.txt` | <span class="zh">HTTP 请求日志</span><span class="en">HTTP Request Logs</span> |

### <span class="zh">10.2 性能监控</span><span class="en">10.2 Performance Monitoring</span>

- <span class="zh">**Hangfire Dashboard**: http://localhost:9901/pchangfire</span><span class="en">**Hangfire Dashboard**: http://localhost:9901/pchangfire</span>
- <span class="zh">**Redis 监控**: 使用 Redis Insight 或类似工具</span><span class="en">**Redis Monitoring**: Use Redis Insight or similar tools</span>
- <span class="zh">**MySQL 监控**: 使用 Prometheus + Grafana</span><span class="en">**MySQL Monitoring**: Use Prometheus + Grafana</span>

### <span class="zh">10.3 常见问题</span><span class="en">10.3 Common Issues</span>

| <span class="zh">问题</span><span class="en">Issue</span> | <span class="zh">解决方案</span><span class="en">Solution</span> |
|------|----------|
| <span class="zh">**数据库连接失败**</span><span class="en">**Database connection failed**</span> | <span class="zh">检查 MySQL 服务是否启动，连接字符串配置是否正确</span><span class="en">Check if MySQL service is running, verify connection string configuration</span> |
| <span class="zh">**Redis 连接失败**</span><span class="en">**Redis connection failed**</span> | <span class="zh">检查 Redis 服务是否启动，密码配置是否正确</span><span class="en">Check if Redis service is running, verify password configuration</span> |
| <span class="zh">**Qdrant 连接失败**</span><span class="en">**Qdrant connection failed**</span> | <span class="zh">检查 Qdrant 服务是否启动，配置的 URL 是否正确</span><span class="en">Check if Qdrant service is running, verify configured URL</span> |
| <span class="zh">**AI 工具调用失败**</span><span class="en">**AI tool calling failed**</span> | <span class="zh">检查工具注册配置，确保 `InitData` 正确传递参数</span><span class="en">Check tool registration configuration, ensure `InitData` passes parameters correctly</span> |
| <span class="zh">**Hangfire 任务不执行**</span><span class="en">**Hangfire tasks not executing**</span> | <span class="zh">检查 Redis 连接，确保服务已启动</span><span class="en">Check Redis connection, ensure service is running</span> |

### <span class="zh">10.4 故障排查指南</span><span class="en">10.4 Troubleshooting Guide</span>

#### <span class="zh">10.4.1 启动失败</span><span class="en">10.4.1 Startup Failure</span>

<span class="zh">**症状：** 应用无法启动</span><span class="en">**Symptom**: Application cannot start</span>

<span class="zh">**排查步骤：**</span><span class="en">**Troubleshooting Steps:**</span>

1. <span class="zh">检查端口是否被占用</span><span class="en">Check if port is occupied</span>
```bash
netstat -ano | findstr :9901
```

2. <span class="zh">检查数据库连接</span><span class="en">Check database connection</span>
```bash
# <span class="zh">测试 MySQL 连接</span><span class="en">Test MySQL connection</span>
mysql -h 127.0.0.1 -P 3306 -u root -p
```

3. <span class="zh">检查 Redis 连接</span><span class="en">Check Redis connection</span>
```bash
redis-cli -h 127.0.0.1 -p 6379 -a 123456 ping
```

#### <span class="zh">10.4.2 AI工具调用失败</span><span class="en">10.4.2 AI Tool Calling Failed</span>

<span class="zh">**症状：** Error: Function failed</span><span class="en">**Symptom**: Error: Function failed</span>

<span class="zh">**排查步骤：**</span><span class="en">**Troubleshooting Steps:**</span>

1. <span class="zh">检查工具注册配置</span><span class="en">Check tool registration configuration</span>
2. <span class="zh">检查 `InitData` 参数传递</span><span class="en">Check `InitData` parameter passing</span>
3. <span class="zh">查看日志文件 `Logs/log_*.txt`</span><span class="en">Check log files `Logs/log_*.txt`</span>
4. <span class="zh">检查工具方法签名是否正确</span><span class="en">Check tool method signature</span>

#### <span class="zh">10.4.3 知识库检索失败</span><span class="en">10.4.3 Knowledge Base Retrieval Failed</span>

<span class="zh">**症状：** 问答返回空或错误</span><span class="en">**Symptom**: Q&A returns empty or error</span>

<span class="zh">**排查步骤：**</span><span class="en">**Troubleshooting Steps:**</span>

1. <span class="zh">检查 Qdrant 服务状态</span><span class="en">Check Qdrant service status</span>
```bash
curl http://localhost:6333/health
```

2. <span class="zh">检查知识库文档是否已上传</span><span class="en">Check if knowledge base documents are uploaded</span>
3. <span class="zh">检查向量模型配置</span><span class="en">Check vector model configuration</span>

---

## <span class="zh">十一、安全规范</span><span class="en">XI. Security Standards</span>

### <span class="zh">11.1 输入验证</span><span class="en">11.1 Input Validation</span>

- <span class="zh">所有用户输入必须进行验证</span><span class="en">All user inputs must be validated</span>
- <span class="zh">使用数据注解或 FluentValidation</span><span class="en">Use data annotations or FluentValidation</span>
- <span class="zh">防止 SQL 注入、XSS 攻击</span><span class="en">Prevent SQL injection, XSS attacks</span>

### <span class="zh">11.2 权限控制</span><span class="en">11.2 Permission Control</span>

- <span class="zh">使用基于角色的访问控制（RBAC）</span><span class="en">Use role-based access control (RBAC)</span>
- <span class="zh">敏感接口需要权限验证</span><span class="en">Sensitive interfaces require permission verification</span>
- <span class="zh">日志记录所有权限检查失败的请求</span><span class="en">Log all permission check failed requests</span>

### <span class="zh">11.3 数据加密</span><span class="en">11.3 Data Encryption</span>

- <span class="zh">数据库密码使用 AES 加密存储</span><span class="en">Database passwords stored using AES encryption</span>
- <span class="zh">API 密钥等敏感配置使用环境变量</span><span class="en">API keys and other sensitive configurations use environment variables</span>
- <span class="zh">传输层使用 HTTPS</span><span class="en">Use HTTPS for transport layer</span>

### <span class="zh">11.4 安全防护</span><span class="en">11.4 Security Protection</span>

#### <span class="zh">11.4.1 危险命令拦截</span><span class="en">11.4.1 Dangerous Command Interception</span>

<span class="zh">系统会拦截以下危险命令：</span><span class="en">The system intercepts the following dangerous commands:</span>
- `rm -rf`
- `del /s /q`
- <span class="zh">格式化磁盘命令</span><span class="en">Disk formatting commands</span>
- <span class="zh">其他破坏性操作</span><span class="en">Other destructive operations</span>

#### <span class="zh">11.4.2 域名白名单</span><span class="en">11.4.2 Domain Whitelist</span>

<span class="zh">HTTP/HTTPS 请求需要配置授权域名白名单：</span><span class="en">HTTP/HTTPS requests require authorized domain whitelist configuration:</span>

```json
{
  "AuthorizedDomains": [
    "example.com",
    "api.example.com"
  ]
}
```

---

## <span class="zh">十二、性能优化</span><span class="en">XII. Performance Optimization</span>

### <span class="zh">12.1 数据库优化</span><span class="en">12.1 Database Optimization</span>

| <span class="zh">优化项</span><span class="en">Optimization Item</span> | <span class="zh">说明</span><span class="en">Description</span> |
|--------|------|
| <span class="zh">索引优化</span><span class="en">Index Optimization</span> | <span class="zh">为常用查询字段创建索引</span><span class="en">Create indexes for frequently queried fields</span> |
| <span class="zh">分页查询</span><span class="en">Pagination Query</span> | <span class="zh">使用 Skip/Take 避免全表扫描</span><span class="en">Use Skip/Take to avoid full table scan</span> |
| <span class="zh">批量操作</span><span class="en">Batch Operations</span> | <span class="zh">使用 EF Core 的批量操作 API</span><span class="en">Use EF Core batch operation API</span> |
| <span class="zh">读写分离</span><span class="en">Read-Write Separation</span> | <span class="zh">配置数据库读写分离</span><span class="en">Configure database read-write separation</span> |

### <span class="zh">12.2 缓存策略</span><span class="en">12.2 Cache Strategy</span>

```csharp
// <span class="zh">使用 Redis 缓存</span><span class="en">Use Redis cache</span>
[CacheDataFilter(Duration = 60)] // <span class="zh">缓存60秒</span><span class="en">Cache for 60 seconds</span>
public async Task<MyDto> GetData(int id)
{
    // <span class="zh">查询逻辑</span><span class="en">Query logic</span>
}
```

### <span class="zh">12.3 异步编程</span><span class="en">12.3 Asynchronous Programming</span>

- <span class="zh">使用 `async/await` 模式</span><span class="en">Use `async/await` pattern</span>
- <span class="zh">避免同步阻塞调用</span><span class="en">Avoid synchronous blocking calls</span>
- <span class="zh">使用 `ConfigureAwait(false)` 优化性能</span><span class="en">Use `ConfigureAwait(false)` to optimize performance</span>

### <span class="zh">12.4 日志优化</span><span class="en">12.4 Log Optimization</span>

- <span class="zh">生产环境关闭 Debug 级别日志</span><span class="en">Turn off Debug level logs in production</span>
- <span class="zh">使用异步日志写入</span><span class="en">Use asynchronous log writing</span>
- <span class="zh">定期清理日志文件</span><span class="en">Regularly clean log files</span>

---

## <span class="zh">十三、附录</span><span class="en">XIII. Appendix</span>

### <span class="zh">13.1 配置项说明</span><span class="en">13.1 Configuration Item Description</span>

| <span class="zh">配置项</span><span class="en">Configuration Item</span> | <span class="zh">说明</span><span class="en">Description</span> | <span class="zh">默认值</span><span class="en">Default</span> |
|--------|------|--------|
| `IsOpenPermission` | <span class="zh">是否开启权限验证</span><span class="en">Whether to enable permission verification</span> | true |
| `TenantId` | <span class="zh">默认租户 ID</span><span class="en">Default tenant ID</span> | 1000 |
| `Jwt:AccessTokenExpirationMinutes` | <span class="zh">Token 过期时间（分钟）</span><span class="en">Token expiration time (minutes)</span> | 60 |
| `HangfireSetting` | <span class="zh">Hangfire 配置</span><span class="en">Hangfire Configuration</span> | - |
| `CorsSetting` | <span class="zh">CORS 跨域配置</span><span class="en">CORS Configuration</span> | - |
| `CodeGeneratorSetting` | <span class="zh">代码生成器配置</span><span class="en">Code Generator Configuration</span> | - |

### <span class="zh">13.2 数据库字段索引</span><span class="en">13.2 Database Field Indexes</span>

```json
{
  "DBDefaultHasIndexFields": "tableid,createtime,updatetime,deletetime,tenantid,createuserid,updateuserid,deleteuserid"
}
```

### <span class="zh">13.3 联系方式</span><span class="en">13.3 Contact</span>

- <span class="zh">项目地址</span><span class="en">Project Address</span>: https://gitee.com/netkevin-li/NetCoreKevin
- <span class="zh">教学文档</span><span class="en">Tutorial Documentation</span>: https://blog.csdn.net/weixin_42629287/category_13037923.html
- <span class="zh">交流群</span><span class="en">Community Group</span>: <span class="zh">文档中包含微信群二维码</span><span class="en">WeChat group QR code is included in the documentation</span>

---

**<span class="zh">版本</span><span class="en">Version</span>**: v1.0  
**<span class="zh">最后更新</span><span class="en">Last Updated</span>**: <span class="zh">2026年6月</span><span class="en">June 2026</span>  
**<span class="zh">维护团队</span><span class="en">Maintenance Team</span>**: <span class="zh">NetCoreKevin 开发团队</span><span class="en">NetCoreKevin Development Team</span>