# NetCoreKevin 系统开发-运维-使用文档

---

## 一、项目概述

### 1.1 项目简介

**NetCoreKevin** 是基于 .NET 10 构建的企业级 AI 智能体中台（SaaS 前后端分离），核心能力包括：AI 智能体（Microsoft Agent Framework + 任意 OpenAI 协议兼容模型）、RAG 知识库（Qdrant 向量检索 + 重排）、Skill 技能包（SKILL.md + `.py` / `.ps1` / `.sh` 脚本动态加载）、智能体记忆（Qdrant 三维隔离）、MCP 工具接入、联网搜索（火山引擎豆包）、多模态（图片理解 / 语音转写 / 文生图）、一库多租户、RBAC 权限、分布式系统与定时任务。

对外还提供 **OpenAI 协议兼容网关**（`/api/v1/models`、`/api/v1/chat/completions`），Cherry Studio、Dify、自研客户端只要配置一个用户级 `sk-kv-` Key 即可接入。

### 1.2 核心功能模块

| 模块 | 功能描述 | 技术实现 |
|------|----------|----------|
| **AI 智能体** | 多步推理、工具调用、多智能体协同、上下文压缩 | Microsoft Agent Framework（`Microsoft.Agents.AI.OpenAI` 1.23.0）、OpenAI 协议 |
| **知识库系统** | 文档切分、向量化、RAG 检索与重排 | Qdrant 1.17、OllamaSharp 5.4.25、阿里 `qwen3-rerank` |
| **技能（Skill）** | `SKILL.md` + 脚本动态加载、在线编辑、地址白名单校验 | 文件系统 + Python/Shell 子进程执行器 |
| **MCP** | 外部 MCP 服务接入智能体（http / sse / stdio） | ModelContextProtocol 1.4.1 |
| **任务调度** | 定时 / 一次性任务、Hangfire 面板 | Hangfire 1.8.23（当前内存存储，Redis 存储代码保留） |
| **认证授权** | JWT 登录、租户隔离、菜单与 API 双维度权限 | `Kevin.Authentication.Jwt`（JwtBearer 10.0.14）、`kevin.Permission` |
| **消息通信** | RabbitMQ 消息、SignalR 实时推送（Redis 背板） | RabbitMQ.Client 6.8.1、SignalR + StackExchange.Redis |
| **文件存储** | 本地静态文件（默认）/ 腾讯云 / 阿里云 / 七牛 | `kevin.FileStorage` 多实现，DI 切换 |
| **代码生成器** | 按实体生成仓储接口/实现、服务接口/实现 | `kevin.CodeGenerator` |
| **OpenAI 兼容网关** | 让第三方客户端直接调用智能体，与网页端共享链路 | `OpenAICompatibleController` + 用户级 Key |

### 1.3 可插拔模块清单（Kevin/kevin.Module，共 25 个 + 1 聚合包）

| 模块 | 职责 |
|---|---|
| `kevin.AI.AgentFramework` | 智能体框架、内置工具、Skill 脚本执行、多模态、文生图 |
| `Kevin.RAG` | Qdrant 向量库、Embedding（Ollama/云端）、Rerank |
| `Kevin.Authentication.Jwt` | JWT 签发、校验、刷新 |
| `kevin.Permission` | RBAC 权限策略、`MyArea`/`MyModule`/`SkipAuthority` 特性 |
| `kevin.Cache` | 内存缓存、Redis 缓存服务 |
| `kevin.Cap` | CAP 消息总线（当前未在启动装配中启用） |
| `kevin.RabbitMQ` | RabbitMQ 客户端封装 |
| `Kevin.Hangfire` | Hangfire 注册、面板、`IModuleConfigTasks` 自动任务 |
| `Kevin.SignalR` | Redis 背板 SignalR、`MySignalRHub`、`ChatRoomHub` |
| `Kevin.SMS` / `Kevin.Email` | 阿里云短信 / SMTP 邮件 |
| `kevin.FileStorage` | 本地 / 腾讯 / 阿里 / 七牛对象存储 |
| `kevin.CodeGenerator` | 代码生成器 |
| `kevin.Consul` | 服务发现（默认未启用） |
| `kevin.DistributedLock` | Redis 分布式锁 |
| `Kevin.SnowflakeId` | 雪花 ID |
| `Kevin.log4Net` | log4net 日志集成 |
| `Kevin.Cors` / `Kevin.Asr` / `Kevin.HttpApiClients` | 跨域 / 阿里 ASR / HTTP 客户端工厂 |
| `kevin.Domain.EventBus` | MediatR 12.4.1 领域事件 |
| `kevin.Ioc` | 分层反射注册（`IModuleInitializer`、替换控制器激活器） |
| `Kevin.Common` | 加解密、配置、全局异常、工具扩展 |
| `Kevin.Versioning.Swagger` | API 版本（Asp.Versioning 8.1）+ Swashbuckle 9.0.6 |
| `Kevin.ALL.Module` | 聚合包（对外发布 NuGet 用） |

---

## 二、技术架构

### 2.1 技术栈

| 分类 | 技术 | 版本 | 说明 |
|------|------|------|------|
| 运行时 | .NET | 10.0 | 所有项目 `net10.0` |
| Web 框架 | ASP.NET Core | 10.0 | Web API + SignalR |
| ORM | EF Core | 10.0.14 | MySql.EntityFrameworkCore 10.0.12（Oracle 官方） |
| 数据库 | MySQL | 8.0+ | 主库（连接串 `ConnectionStrings.dbConnection`） |
| 缓存 | Redis | 7.0+ | 分布式锁、SignalR 背板、JWT 缓存；进程内缓存用 MemoryCache |
| 向量数据库 | Qdrant | 1.17 客户端 | 知识库与记忆 |
| 任务调度 | Hangfire | 1.8.23 | 当前 `Hangfire.MemoryStorage`，Redis 存储注册代码保留 |
| 消息总线 | RabbitMQ | Client 7.2.1 | CAP 10.0.2 包已引用，启动装配默认未启用 |
| 认证 | JWT | Microsoft.AspNetCore.Authentication.JwtBearer 10.0.14 | 无 IdentityServer4 |
| API 版本 | Asp.Versioning.Mvc | 10.2.1 | Query/UrlSegment/Header(`X-API-Version`)/MediaType |
| 接口文档 | Swashbuckle.AspNetCore | 10.3.1 | `/swagger`，基于 Microsoft.OpenApi 2.x |
| AI 框架 | Microsoft Agent Framework | Microsoft.Agents.AI.OpenAI 1.23.0 | `AIFunctionFactory` 工具调用 |
| 本地模型 | OllamaSharp | 5.4.25 | 原生 Ollama 接口 |
| MCP | ModelContextProtocol | 1.4.1 | http / sse / stdio |
| 日志 | log4net | 3.3.2 | 按环境加载 `LogConfigs/log4.{Env}.config` |
| 前端 | Vue 3 + Ant Design Vue 4 | Vue CLI 5（webpack） | `vue/kevin.web.vue` |

> 版本以各 `.csproj` 的 `PackageReference` 为准，本表为主要依赖的实际取值。

#### 2.1.1 .NET 10 配套升级核对

所有参与编译的项目 TFM 均为 `net10.0`；`dotnet-ef` 工具为 10.0.14（见 `App/WebApi/.config/dotnet-tools.json`）；容器基础镜像为 `mcr.microsoft.com/dotnet/aspnet:10.0` / `sdk:10.0`。

已跟随 .NET 10 齐套的依赖：EF Core 全家族 10.0.14、`Microsoft.Extensions.*` 10.0.14、`Microsoft.AspNetCore.Authentication.JwtBearer` / `Microsoft.AspNetCore.Mvc.Testing` / `Microsoft.AspNetCore.SignalR.StackExchangeRedis` 10.0.14、`System.Drawing.Common` 与 `System.Security.Cryptography.Xml` 10.0.14、MySQL 提供程序 `MySql.EntityFrameworkCore` 10.0.12、向量抽象 `Microsoft.Extensions.VectorData.Abstractions` 10.11.0。

原停留在旧版、已随本次升级一并处理：

| 包 | 升级前 | 现状 | 处理方式 |
|---|---|---|---|
| `Microsoft.AspNetCore.*`（22 处，15 个 csproj） | 2.3.0 | 已移除 | 改为 `<FrameworkReference Include="Microsoft.AspNetCore.App" />`；同时清掉框架已自带的冗余 `Microsoft.Extensions.*` / `System.Security.Cryptography.Xml` 引用 |
| `Asp.Versioning.Mvc` / `Asp.Versioning.Mvc.ApiExplorer` | 8.1.0 | 10.2.1 | `ApiDescriptionExtensions.IsDeprecated()` 改为属性 `IsDeprecated` |
| `Swashbuckle.AspNetCore` | 9.0.6 | 10.3.1 | 适配 Microsoft.OpenApi 2.x：`Microsoft.OpenApi.Models` → `Microsoft.OpenApi`，`OpenApiSchema.Type` 用 `JsonSchemaType` 枚举，安全方案改 `OpenApiSecuritySchemeReference`，`AddSecurityRequirement` 改 `Func<OpenApiDocument, …>` 重载，过滤器里 `IOpenApiParameter`/`IOpenApiSchema` 只读属性改用具体实现类型写回 |
| `DotNetCore.CAP`（6 个包） | 8.4.1 | 10.0.2 | 无代码改动；但它传递引入 `RabbitMQ.Client` 7.2.1 |
| `RabbitMQ.Client`（`kevin.RabbitMQ`） | 6.8.1 | 7.2.1 | 必须同步升级，否则与 CAP 的 7.x 冲突导致 `MissingMethodException`；已适配 7.x 异步 API：`CreateConnectionAsync` / `CreateChannelAsync` / `IChannel`、`*Async` 方法族、`BasicProperties`、`AsyncEventingBasicConsumer.ReceivedAsync`、发布确认改由 `CreateChannelOptions` 开启 |
| `Microsoft.NET.Test.Sdk` / `xunit` / `xunit.runner.visualstudio` / `coverlet.collector` | 17.8.0 / 2.5.3 / 2.5.3 / 6.0.0 | 18.11.0 / 2.9.3 / 4.0.1 / 10.1.0 | 修复 xunit 2.9 分析器报错（`Assert.ThrowsAsync` 必须 await） |

自测结论（Debug）：整解决方案编译 0 错误；`App.WebApi` 启动正常，`/swagger/v1/swagger.json`（145 个路径）、`/swagger/v2/swagger.json`、`/swagger/index.html` 均返回 200，JWT 安全方案 `Bearer` 生效；单元测试 134 个用例通过 117、失败 17（其中 1 个为 RabbitMQ 测试所需 vhost `/app_webapi_test` 不存在，其余为 RAG 工具类既有断言与实现不一致、Selenium 缺 `msedgedriver`、外网样例地址 404，与本次依赖升级无关）。

> 另有一批与 .NET 版本无关的包（`MediatR` 12、`AutoMapper` 15、`FluentAssertions` 6、`SixLabors.ImageSharp` 3、`SkiaSharp` 3、`Emgu.CV` 4、`ModelContextProtocol` 1），无需跟随 .NET 升级；其更新的大版本中部分已转为商业许可或含破坏性变更，需单独评估后再动。

### 2.2 架构设计

```
┌──────────────────────────────────────────────────────────────────┐
│                    前端层（Vue 3 + Ant Design Vue）              │
│        用户/角色/权限 · 智能体 · 知识库 · 技能 · 任务 · Key       │
└───────────────────────────────┬──────────────────────────────────┘
                                │ REST / SignalR（/VarApi 代理 → 9901）
┌───────────────────────────────▼──────────────────────────────────┐
│                    App.WebApi（.NET 10，9901）                    │
│  Swagger(/swagger) · Hangfire(/pchangfire) · 全局异常 · 权限过滤  │
│  OpenAI 兼容网关(/api/v1) · SignalR Hub(/api/MySignalRHub)        │
├──────────────────────────────────────────────────────────────────┤
│  App.Application（业务编排） → App.Domain（实体/契约）            │
│                              → App.RepositorieRps（EF Core 仓储） │
├──────────────────────────────────────────────────────────────────┤
│  AI 能力层（同进程，kevin.Module）                                │
│  AgentFramework（对话/工具/记忆/多模态） · RAG（切分/向量/重排）  │
├──────────────────────────────────────────────────────────────────┤
│  MySQL · Redis · Qdrant · RabbitMQ · 本地文件(KevinFlies)/云存储  │
└──────────────────────────────────────────────────────────────────┘
```

- 模型服务不限定厂商：对话走 OpenAI 官方 SDK 客户端，只要服务端实现 OpenAI 协议（OpenAI、智谱、DeepSeek、通义、Kimi、vLLM、Ollama `/v1` 等）填 `EndPoint + ModelName + ModelKey` 即可。
- 各层通过各自的 `ModuleInitializer : IModuleInitializer` 暴露注册逻辑，`kevin.Ioc` 负责反射批量注册并替换控制器激活器。

### 2.3 项目结构

```
kevin.abp.core/
├── NetCoreKevin.sln                # 解决方案入口
├── App/                            # 业务应用（分层参考 Kevin）
│   ├── Application/                # 应用服务层
│   ├── Domain/                     # 领域层（实体、接口契约）
│   ├── RepositorieRps/             # 仓储实现
│   ├── AppShare/                   # 跨层共享 DTO
│   ├── WebApi/                     # API 入口（Program.cs、appsettings、LogConfigs、Dockerfile）
│   └── WebApiMcp/                  # MCP 转发服务端
├── Kevin/                          # 框架核心
│   ├── Application/                # 核心业务服务（AI 服务、AI 工具注册、任务配置）
│   │   ├── Services/AI/            # AIAppsService、AIAgentToolSkillService 等
│   │   └── TaskModuleConfigs/      # IModuleConfigTasks 定时任务注册
│   ├── Domain/                     # 核心实体（Entities/AI）、BaseDatas 种子数据
│   ├── Kevin.EntityFrameworkCore/  # DbContext、慢查询拦截器、Migrations（38+ 迁移）
│   ├── Kevin.Web.Basics/           # 控制器、全局过滤器、ServiceConfiguration 装配
│   ├── RepositorieRps/             # 核心仓储实现
│   ├── kevin.Share/                # 共享 DTO/枚举（kevin.Domain.Share.csproj）
│   └── kevin.Module/               # 25 个可插拔模块（见 1.3）
├── Test/
│   ├── Kevin.Unit.Tests/           # 单元测试（xUnit + FakeItEasy）
│   ├── Kevin.Integration.Tests/    # 集成测试（WebApplicationFactory）
│   └── Kevin.Testing.Shared/       # 共享测试基础设施
├── vue/kevin.web.vue/              # 管理后台（Vue 3 + Ant Design Vue）
├── Doc/ · InitData/                # 文档截图 · logo 与省市区镇初始化数据
└── docker-compose.yml              # 一键部署编排（前端 + 后端 + MySQL + Redis + Qdrant）
```

---

## 三、环境要求

### 3.1 必备依赖

| 依赖 | 版本要求 | 说明 |
|------|----------|------|
| .NET SDK | 10.0+ | 后端开发/运行 |
| MySQL | 8.0+ | 主数据库（建议 `utf8mb4`） |
| Redis | 7.0+ | 分布式锁、SignalR 背板、缓存 |
| Qdrant | 1.7+ | 知识库与记忆向量检索 |
| 模型服务 | OpenAI 协议兼容 | 云端（OpenAI/智谱/DeepSeek/通义等）或本地（Ollama/vLLM），有一个即可 |

### 3.2 可选依赖

| 依赖 | 说明 |
|------|------|
| Node.js 16+ | 前端管理后台（`vue/kevin.web.vue`，Vue CLI 5） |
| Ollama | 本地对话/Embedding 模型 |
| RabbitMQ | `kevin.RabbitMQ` 消息能力（按需） |
| Consul | 服务发现（`Kevin.Consul` 模块已内置，`Program.cs` 中 `UseKevinConsul` 默认注释未启用） |
| 阿里云 ASR / 豆包搜索 / 云存储 | 对应能力启用的前提 |

---

## 四、安装配置

### 4.1 数据库与 Redis 配置

编辑 `App/WebApi/appsettings.json`（或对应环境文件）：

```json
{
  "ConnectionStrings": {
    "dbConnection": "server=localhost;port=3306;database=kevin_app;user id=root;password=admin123;Convert Zero Datetime=True;TreatTinyAsBoolean=false;AllowLoadLocalInfile=true;Charset=utf8;Command Timeout=120;",
    "redisConnection": "localhost:6379,DefaultDatabase=0,password=123456,connectTimeout=5000,syncTimeout=5000,connectRetry=3"
  }
}
```

### 4.2 环境分层

- 环境仅三种：`Development`（`appsettings.Development.json`）、`Test`（`appsettings.Test.json`）、正式（根 `appsettings.json`）。
- `Program.Main` 先调用 `EnvironmentConfigHelper.SetEnvironment(...)`（优先 `NETCORE_ENVIRONMENT`，回退 `ASPNETCORE_ENVIRONMENT`），再构建 `WebApplication`。
- 覆盖优先级遵循 ASP.NET Core 默认：`appsettings.json` → `appsettings.{Env}.json` → 环境变量（如 `ConnectionStrings__dbConnection`）→ 命令行参数。
- 日志配置按环境加载 `App/WebApi/LogConfigs/log4.{Env}.config`（无后缀为正式环境），缺失则跳过 log4net 初始化。

**Windows PowerShell：**
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
```

**Linux/macOS：**
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

### 4.3 数据库迁移

仓库已包含历次 EF 迁移（`Kevin/Kevin.EntityFrameworkCore/Migrations`），全新库直接更新即可。在 **程序包管理控制台** 中（默认项目 `Kevin.EntityFrameworkCore`，启动项目 `App.WebApi`）：

```powershell
Update-Database
```

CLI 方式：

```bash
dotnet ef database update --project Kevin/Kevin.EntityFrameworkCore --startup-project App/WebApi
```

只有修改实体后才需要先 `Add-Migration "你的改动"` 再更新。迁移程序集由配置项 `MigrationsAssembly`（默认 `Kevin.EntityFrameworkCore`）指定。

### 4.4 关键配置节速查

| 配置节 | 作用 | 关键项 |
|--------|------|--------|
| `ConnectionStrings` | 数据库 / Redis | `dbConnection`、`redisConnection` |
| `Jwt` | JWT 签发 | `Key`、`Issuer`、`Audience`、`AccessTokenExpirationMinutes`(60)、`RefreshTokenExpirationDays`(1) |
| `TenantId` | 默认初始化租户 | 1000 |
| `IsOpenPermission` | 是否启用权限校验 | true |
| `AESInfo` | AES 加密用 Key/IV | Key 必须 16/24/32 字节，IV 必须 16 字节；变更后历史密文不可解密 |
| `HangfireSetting` | Hangfire 面板账号 | `userSetting`（默认 admin/admin） |
| `HangfireRedisSetting` | Hangfire Redis 存储参数 | 当前注册处被注释，默认内存存储 |
| `QdrantClientSetting` | 向量库 | `Url`、`ApiKey`、`MemoryEmbeddingSize`(2048) |
| `OllamaApiSetting` | Embedding 模型（OpenAI 兼容或原生 Ollama） | `Url`、`DefaultModel`、`ApiKey` |
| `AliRerankApiSetting` | 重排模型 | `Url`、`DefaultModel`(qwen3-rerank)、`ApiKey` |
| `AliyunAsr` | 阿里语音转写 | `AccessKeyId`、`AccessKeySecret`、`AsrAppKey` |
| `DoubaoSearchApiSetting` | 豆包联网搜索 | `ApiKey`、`GlobalApiKey`、`CustomApiKey` |
| `SkillToolSecuritySetting` | 技能脚本 URL 白名单 | `IsOpenAllowedUrlPrefixes`、`AllowedUrlPrefixes` |
| `AIChatStorageSetting` | 对话入库体积上限 | 单条提问/回答 40 万字符等 |
| `AITokenQuotaSetting` | 用户月度 Token 额度 | `MonthlyQuota`(200000000) |
| `Modality` | 音视频输入限制 | 音频 20MB/600s、图片 10MB、单消息 10 个附件 |
| `ImageGeneration` | 文生图 | 尺寸、质量、OSS 上传开关 |
| `OpenAICompatibleSetting` | OpenAI 兼容网关 | `SessionExpirationHours`(24)、`ReasoningOutput`、`AIChatIsHidden` |
| `CorsSetting` | 跨域 | `EnableAllIPs`、`IPs` |
| `SnowflakeIdSetting` | 雪花 ID | `MachineId`、`DataCenterId` |
| `CodeGeneratorSetting` | 代码生成器 | `CodeGeneratorItems`（两组命名空间/路径映射） |
| `DBDefaultHasIndexFields` | 默认索引字段 | `tableid,createtime,...` |

> 敏感项（`Jwt:Key`、`AESInfo`、云厂商 SecretKey）上线前务必替换，并建议通过环境变量注入。

---

## 五、启动运行

### 5.1 开发环境启动

**方式一：Visual Studio**
1. 打开解决方案 `NetCoreKevin.sln`
2. 设置 `App.WebApi` 为启动项目
3. F5 启动（`launchSettings.json` 已指定 `ASPNETCORE_ENVIRONMENT=Development` 与 `http://localhost:9901`）

**方式二：命令行**
```bash
dotnet run --project App/WebApi
```

### 5.2 管理后台启动

```bash
cd vue/kevin.web.vue
npm install
npm run serve
```

- 开发服务器默认 `http://localhost:8080`。
- `/VarApi` 反向代理到 `http://localhost:9901`（`vue.config.js` 中 `pathRewrite` 去掉前缀），无需额外跨域配置。
- 前端 API 地址由 `.env*` 的 `VUE_APP_API_BASE_URL` 注入。

### 5.3 服务访问

| 服务 | 地址 |
|------|------|
| API 接口 | http://localhost:9901 |
| Swagger 文档 | http://localhost:9901/swagger |
| Hangfire 面板 | http://localhost:9901/pchangfire |
| SignalR Hub | `ws://localhost:9901/api/MySignalRHub`、`/api/ChatRoomHub` |
| 管理后台 | http://localhost:8080 |

Hangfire 面板默认 Basic Auth，账号取 `HangfireSetting.userSetting`（默认 `admin` / `admin`），面板语言固定中文。

### 5.4 默认账户

| 用户名 | 密码 | 租户 | 说明 |
|--------|------|------|------|
| admin | 123456 | 1000（`TenantId` 配置） | 超级管理员（`IsSuperAdmin=true`），种子数据见 `Kevin/Domain/BaseDatas/TUserBaseData.cs` |

---

## 六、AI 智能体配置

### 6.1 Qdrant 配置

```bash
docker run -d --name qdrant -p 6333:6333 -v qdrant_storage:/qdrant/storage qdrant/qdrant
```

```json
{
  "QdrantClientSetting": {
    "Url": "localhost",
    "ApiKey": "",
    "MemoryEmbeddingSize": 2048
  }
}
```

`MemoryEmbeddingSize` 需与所选 Embedding 模型输出维度一致（默认 2048 对应智谱 `embedding-3`），否则集合检索会报维度不匹配。

### 6.2 模型配置

模型接入统一走 OpenAI 协议：只要服务商提供 OpenAI 兼容接口，填好地址、模型名和 Key 就能用——OpenAI、智谱、DeepSeek、通义、Kimi、vLLM、Ollama（`/v1`）都在支持范围内。对话模型与向量模型都在后台「模型管理」里配，也可直接改 `appsettings.json`。

**Embedding（云端，以智谱为例；其它 OpenAI 兼容服务换成自己的地址和 Key）：**

```json
{
  "OllamaApiSetting": {
    "Url": "https://open.bigmodel.cn/api/paas/v4/embeddings",
    "DefaultModel": "embedding-3",
    "ApiKey": "your-api-key"
  }
}
```

**Embedding（Ollama 本地，原生接口不传 ApiKey）：**

```json
{
  "OllamaApiSetting": {
    "Url": "http://localhost:11434/api/embeddings",
    "DefaultModel": "qwen3:4b"
  }
}
```

**重排（可选）：**

```json
{
  "AliRerankApiSetting": {
    "Url": "https://dashscope.aliyuncs.com/compatible-api/v1/reranks",
    "DefaultModel": "qwen3-rerank",
    "ApiKey": "your-api-key"
  }
}
```

不配重排也能用，只是检索结果不做二次排序。

### 6.3 模型管理字段说明（TAIModels）

| 字段 | 说明 |
|------|------|
| `AIType` | 平台类型（枚举默认 `OpenAI`，另有 AzureOpenAI、智谱、Bge、Ollama 等标记） |
| `AIModelType` | 位标记：`Chat`(1)、`Embedding`(2)、`Rerank`(4)、`ImageGeneration`(8)、`AudioUnderstanding`(16)，可与 Chat 组合 |
| `EndPoint` | 模型服务地址（OpenAI 协议 Base URL） |
| `ModelName` / `ModelKey` | 模型名 / 密钥（本地无鉴权可留空，用占位凭据） |
| `ModelDescription` | 部署名（Azure 场景使用） |
| `EmbeddingValueSize` | 向量维度（默认 2048） |
| `MaxAskPromptSize` | 提问最大 Token（上下文窗口预算，默认 131072） |
| `AnswerTokens` | 回答最大 Token（默认 8192） |

智能体（TAIApps）侧还有：`Temperature`、`MaxRetries`(2)、`NetworkTimeout`(30 分钟)、`ChatMessageLimit`(100)、`ContentLengthLimit`(50000)、`Relevance`/`MaxMatchesCount`/`RerankCount`（知识库检索参数）、`IsThinkingLog`/`IsToolLog`（思考与工具日志开关，同时决定流式下发通道）、`IsAIMessageCompaction`（上下文压缩）、`IsMemory`、`IsAITools`/`IsSkill`/`IsMcp`、`IsImageGeneration`、`AuthorizedDomains`（Shell/HTTP 工具域名白名单，默认 `*`）。

### 6.4 AI 智能体使用流程

1. 登录后台（admin / 123456 / 租户 1000）。
2. **模型管理**：配置对话模型（`AIModelType=Chat`，EndPoint 指向 OpenAI 兼容服务）与向量模型（`Embedding`，维度与 Qdrant 集合一致）。
3. **知识库**：新建知识库并选择向量模型，上传文档；向量化由 Hangfire 定时任务「每5分钟检测是否有AI文档知识库需要处理」异步处理。
4. **智能体**：选择对话模型、绑定知识库、按需勾选技能 / 工具 / MCP、开启记忆与上下文压缩。
5. **开始对话**：网页端（`/api/MySignalRHub` 或 SSE `POST /api/AIChatHistorys/AddSSE`）或第三方客户端（OpenAI 兼容接口）。

### 6.5 内置自动任务（Hangfire）

| 任务 ID | 触发规则 | 作用 |
|---------|----------|------|
| 每5分钟检测是否有AI文档知识库需要处理 | `0 0/5 0/1 * * ?` | 扫描待处理知识库文档并向量化 |
| 每月颁发用户Token额度 | `0 0 0 1 * ?` | 每月 1 日 00:00 按 `AITokenQuotaSetting.MonthlyQuota` 给所有用户颁发当月额度（幂等） |
| 每天清理过期的智能体短期记忆 | `0 0 3 * * ?` | 清理过期短期记忆并同步删除 Qdrant 向量 |

任务通过实现 `IModuleConfigTasks.ConfigTasks` 在启动时自动注册（`Kevin/Application/TaskModuleConfigs/`）。

> 注意：当前 `ServiceConfiguration` 注册的是 `AddKevinHangfireMemoryStorage()`，**进程重启后任务与执行记录会丢失**（周期性任务会在下次启动时重新注册）。需要持久化时改用被注释的 `AddKevinHangfireRedis()` 并配置 `HangfireRedisSetting`。

### 6.6 OpenAI 协议兼容接口

- 路由：`GET /api/v1/models`（返回可调用的智能体列表，`model` 默认即智能体名称，同时兼容智能体 Id）、`POST /api/v1/chat/completions`（支持 `stream`）。
- 鉴权：`Authorization: Bearer sk-kv-...`。Key 在后台「我的 OpenAI Key」页面自助生成，加密落库（依赖 `AESInfo`），支持查看明文、启用/禁用、删除；鉴权通过后网关内部换发等价 JWT，聊天记录、权限、租户过滤与网页端完全一致。
- 智能体配置里的 `OpenAICompatibleSetting.SessionExpirationHours` 控制同一会话标识的最大空闲时长；`AIChatIsHidden=true` 时 API 对话在网页端列表隐藏。
<!-- 第二部分续写 -->

---

## 七、API 接口说明

### 7.0 统一响应与鉴权

- 除文件流、SSE、OpenAI 兼容接口外，所有接口经 `ResultFilter` 统一包装：

```json
{ "code": 200, "msg": "success", "isSuccess": true, "data": {} }
```

失败：`{ "code": 400/401/500, "msg": "...", "isSuccess": false, "errMsg": "具体错误信息" }`。业务异常（`UserFriendlyException` / `FieldValidationException`）统一返回 HTTP 400；认证失败 401、无权限 403 由 `ApiResponseHandler` 专用通道返回。

- 鉴权：请求头 `Authorization: Bearer <JWT>`。Token 剩余有效期不足 20 分钟时，权限中间件会自动续签，并在响应头 `NewToken` 返回新 Token（同时暴露 `Access-Control-Expose-Headers`）。
- 多租户：请求上下文自动携带 `TenantId`，仓储层强制租户过滤，前端无需传租户参数。
- JSON：后端统一小驼峰、`long` 序列化为字符串（`LongConverter`），前端按小驼峰取字段。

### 7.1 认证接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/Authorize/GetToken` | POST | 账号密码登录，返回 JWT 字符串 |
| `/api/Authorize/GetTokenBySms` | POST | 短信验证码登录 |
| `/api/Authorize/SendSmsVerifyPhone` | POST | 发送短信验证码 |

**登录请求体（`dtoLogin`）：**

```json
{
  "name": "admin",
  "passWord": "123456",
  "tenantId": 1000
}
```

### 7.2 AI 接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/AIApps/GetPageData` | POST | 智能体分页 |
| `/api/AIApps/GetALLList` / `GetMyALLList` | GET | 智能体全量 / 我的智能体 |
| `/api/AIApps/AddEdit` | POST | 新增或编辑智能体 |
| `/api/AIApps/GetDetails` | GET | 智能体详情 |
| `/api/AIChats/GetMyPageData` | POST | 我的对话分页 |
| `/api/AIChats/Add` | POST | 新建对话（返回首条历史 DTO） |
| `/api/AIChatHistorys/Add` | POST | 发送消息（非流式） |
| `/api/AIChatHistorys/AddSSE` | POST | 发送消息（SSE 流式） |
| `/api/AIChatHistorys/GetPageData` | POST | 历史消息分页 |
| `/api/AIKmss/GetList` / `GetMyPageData` | POST | 知识库列表 / 分页 |
| `/api/AIKmss/AddEdit` / `GetDetails` / `Delete` | POST/GET/DELETE | 知识库维护 |
| `/api/AIModels/GetPageData` / `GetALLList` | POST/GET | 模型分页 / 按类型全量（缓存 60s） |
| `/api/AIModels/AddEdit` / `Delete` | POST/DELETE | 模型维护 |
| `/api/AIPrompts/GetPageData` / `GetALLList` | POST/GET | 提示词分页 / 全量 |
| `/api/AIPrompts/AddEdit` / `Delete` | POST/DELETE | 提示词维护 |
| `/api/AISkillToolManagement/GetPageData` / `GetById` | POST/GET | 技能工具分页 / 详情 |
| `/api/AISkillToolManagement/AddEdit` / `Delete` | POST/DELETE | 技能工具维护 |
| `/api/AISkillToolManagement/GetAllTools` / `GetAllSkills` / `GetAllMcps` | GET | 工具 / 技能 / MCP 列表 |
| `/api/AISkillToolManagement/TestMcpConnection` | POST | 测试 MCP 连接并列出工具 |
| `/api/AIUserTokenQuota/GetMyQuota` | GET | 我的当月 Token 额度与用量 |
| `/api/OpenAICompatibleKey/GetMyKey` | GET | 查询我的 Key 状态 |
| `/api/OpenAICompatibleKey/Generate` / `Reveal` | POST | 生成 Key / 查看明文（AES 解密） |
| `/api/OpenAICompatibleKey/UpdateName` / `SetEnabled` | POST | 改名 / 启停 |
| `/api/OpenAICompatibleKey/GetIntegrationDoc` | GET | 获取对接文档（BaseUrl、示例代码） |
| `GET /api/v1/models` | GET | OpenAI 协议：可用智能体列表 |
| `POST /api/v1/chat/completions` | POST | OpenAI 协议：对话（支持 stream） |

### 7.3 文件接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/File/UploadFile` | POST | 上传文件（Query：`business`、`key`、`sign`；Form：`file`），返回 fileId |
| `/api/File/BatchUploadFile` | POST | 批量上传 |
| `/api/File/RemoteUploadFile` | POST | 远程文件转存 |
| `/api/File/GetFile` / `GetImage` / `GetFilePath` / `GetById` | GET | 下载 / 缩略图 / 路径 / 元信息 |
| `/api/File/DeleteFile` | DELETE | 删除文件 |

上传签名参数 `key`/`sign` 由业务侧约定生成，用于避免匿名上传。知识库文档即通过该接口上传后再挂到知识库记录上。

### 7.4 任务调度接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/AITasks/GetTaskList` | POST | 我的任务列表（周期 + 未执行的一次性任务） |
| `/api/AITasks/RemoveCronTask?name=...` | DELETE | 移除任务（兼容两类任务） |
| `/api/AITasks/TriggerCronTask?name=...` | GET | 立即触发（一次性任务走 Requeue） |

智能体在对话中还可通过内置工具 `AddOrUpdateCronTask` / `AddOnceTask` 创建任务（服务实现 `Kevin/Application/Services/AI/KevinAITasksService.cs`）。

### 7.5 代码生成器接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/CodeGenerator/GetAreaNames` | GET | 获取配置中的区域（AreaName）列表 |
| `/api/CodeGenerator/GetAreaNameEntityItems?name=...` | GET | 获取该区域下的实体列表 |
| `/api/CodeGenerator/BulidCode` | POST | 按实体生成代码 |

### 7.6 API 版本控制

- 使用 Asp.Versioning：`AssumeDefaultVersionWhenUnspecified = true`，未指定版本时按当前最高实现版本处理。
- 版本传递方式可组合：Query `?version=`、URL 段、请求头 `X-API-Version`、MediaType 参数。
- 响应头 `api-supported-versions` / `api-deprecated-versions` 由 `ReportApiVersions` 输出；Swagger 按版本分组（`/swagger`）。

---

## 八、开发规范

### 8.1 代码风格

- 类、接口、方法：**PascalCase**；参数、局部变量：**camelCase**。
- 文件编码 UTF-8；编辑以 `.editorconfig` 为准。
- 全局 using 集中在各项目 `GlobalUsings.cs`。

### 8.2 分层与命名规范

```
App/
├── Domain/Entities/                # 实体，继承 CUD_User（租户/软删/审计字段）
├── Domain/Interfaces/Repositorie/v1/   # I[Entity]Rp : IRepository<T, long>
├── Domain/Interfaces/Services/v1/      # I[Service]Service
├── Application/Services/v1/            # [Service]Service : BaseService
├── RepositorieRps/Repositories/v1/     # [Entity]Rp : Repository<T, long>
└── WebApi/Controllers/v1/              # 控制器
```

- 仓储实现继承 `Repository<T, TId>`，由 `ModuleInitializer` 中 `IocHelper.BatchAddScopeds<IBaseRepository>` 批量注册。
- 服务通过构造函数注入 `I*Rp` 仓储接口；跨层 DTO 放 `AppShare` / `kevin.Share`。
- 配置类（`XxxSetting`）放在对应能力模块（`kevin.Module/*/Models` 或 `Dto`），不放服务层；业务默认值优先用 `ConfigHelper` 静态读取（支持热重载）。

### 8.3 异常处理

- 业务可预期错误直接 `throw new UserFriendlyException("提示文案")`，全局 `GlobalError` 映射为 HTTP 400；字段校验失败抛 `FieldValidationException`。
- 未处理异常统一 500，并由 log4net 记录请求上下文与堆栈（大请求体截断）。
- 不要手动拼装响应体，交给 `ResultFilter` 统一包装。

### 8.4 日志规范

```csharp
using Kevin.log4Net;

LogHelper<MyService>.logger.Info("信息级别日志");
LogHelper<MyService>.logger.Warn("警告级别日志");
LogHelper<MyService>.logger.Error("错误级别日志", exception);
// 无泛型场景：LogHelper.logger.Error(...)
```

- 配置：`App/WebApi/LogConfigs/log4.config`、`log4.Development.config`、`log4.Test.config`，按 Debug/Info/Warn/Error 分文件滚动，文件落在运行目录 `Log/` 下。
- 新增日志文件类型需同时改三份环境配置，保持键一致。

### 8.5 缓存规范

- 进程内缓存：`services.AddKevinMemoryCache()`（当前装配即为内存缓存）；分布式锁走 Redis（`AddKevinDistributedLockRedis`）。
- 控制器/方法缓存特性（基于缓存服务 + 可选 Token 维度）：

```csharp
[CacheDataFilter<List<AIModelsDto>>(TTL = 60, UseToken = true)] // 缓存 60 秒，按用户 Token 隔离
public async Task<List<AIModelsDto>> GetALLList([FromQuery] int Type = 1)
```

`TTL` 单位秒；`UseToken=false` 时所有用户共享同一缓存（如角色字典）。

### 8.6 权限与多租户规范

- 权限开关：`IsOpenPermission`（`appsettings.json`），关闭后仅保留登录认证。
- 权限键规则：`{area}/{module}/{action}`。控制器标注：

```csharp
[MyArea("agent.manage", "智能体管理")]
[MyModule("agent", "智能体")]
public class AIAppsController : ApiControllerBase
{
    [ActionDescription("新增或编辑智能体")] // 出现在权限/操作日志中
    public async Task<bool> AddEdit([FromBody] AIAppsDto par) { ... }
}
```

- 放行：`[AllowAnonymous]`（匿名）或 `[SkipAuthority]`（免权限但需登录）。
- 多租户：实体继承基类自动带 `TenantId`，查询自动过滤，不要在业务代码手动拼接租户条件。

### 8.7 代码生成器使用

配置（`CodeGeneratorSetting.CodeGeneratorItems`）需与命名空间/路径一一对应，当前仓库配置两组：`App.WebApi.v1` → `App` 各层，`kevin.Application` → `kevin` 各层。

1. `GET /api/CodeGenerator/GetAreaNames` 取区域。
2. `GET /api/CodeGenerator/GetAreaNameEntityItems?name=App.WebApi.v1` 取实体。
3. `POST /api/CodeGenerator/BulidCode`，Body：

```json
[
  { "entityName": "MyEntity", "areaName": "App.WebApi.v1" }
]
```

注意：生成会覆盖同名文件，操作前先备份；仅超级管理员可用。

### 8.8 自定义 AI 工具开发

工具注册统一在 `Kevin/Application/Services/AI/AIAgentToolSkillService.cs`（`GetAITools` / `GetMemoryToolsAsync` / `GetMcpTools`）。新增工具步骤：

1. **实现服务方法**（建议接口 + 实现分离，便于 DI）：

```csharp
public interface IMyCustomToolService
{
    /// <summary>查询订单状态</summary>
    Task<string> GetOrderStatus(
        [Description("订单号")] string orderNo);
}
```

2. **注册服务**：在对应模块的 `ServiceCollectionExtensions` 或 `ModuleInitializer` 中注册（`kevin.Ioc` 也会按约定扫描批量注册）。

3. **挂到工具清单**：在 `AIAgentToolSkillService.GetAITools` 中按开关分支添加：

```csharp
case "MyCustomTools.GetOrderStatus":
    aiTools.Add(AIFunctionFactory.Create(_myCustomToolService.GetOrderStatus,
        new AIFunctionFactoryOptions
        {
            Name = "GetOrderStatus",
            Description = "查询订单状态，参数为订单号"
        }));
    break;
```

4. **后台登记工具**：在「技能工具管理」新增一条 `ClassMethod = MyCustomTools.GetOrderStatus` 的工具记录，再给智能体勾选。

规范要点：
- 参数与返回值优先 `string`；错误信息以 `❌` 开头返回，便于模型识别。
- 可选参数必须有默认值（`AIFunctionFactory` 按 `HasDefaultValue` 判定必填，而不是可空类型）。
- 内置基础工具（6 个，默认注入）：`AddJson`、`GetUrlAuthorizedCodeAsync`、`GetCurrentTime`、`GetCurrentUserInfo`、`SaveFileContent`、`SendDDToMyMsg`。
- 可选工具（后台勾选后才注入）：HTTP（`GetAsync`/`PostAsync`/`PutAsync`/`DeleteAsync`）、Shell（`RunShell`）、Python（`RunPythonCode`）、系统（`GetRuntimePlatform`/`GetDesktopPath`/`WriteTextToDesktop`）、任务（`AddOrUpdateCronTask`/`AddOnceTask`/`RemoveCronTask`/`TriggerCronTask`/`GetTaskList`）、钉钉（`SendDDToUserMsg`）、联网搜索（`DoubaoSearchGlobalAsync`/`DoubaoSearchCustomAsync`）。
- 记忆工具（`SaveMemory`/`SearchMemory`/`UpdateMemory`/`DeleteMemory`）只在智能体开启 `IsMemory` 时注入，不参与手动绑定。
- MCP 工具按 `McpType`（http/sse/stdio）动态创建客户端并加载勾选工具。

---

## 九、运维部署

### 9.1 Docker 部署

#### 一键部署（推荐）

根目录 `docker-compose.yml` 已编排 **前端（Nginx）+ 后端（.NET WebApi）+ MySQL + Redis + Qdrant**，RabbitMQ 可选：

```bash
docker compose up -d --build        # 首次：构建前后端镜像并启动全部服务
docker compose up -d                # 镜像已存在时只启动
docker compose down                 # 停止（数据卷保留）
docker compose --profile mq up -d   # 额外启动 RabbitMQ
```

| 服务 | 宿主机地址 |
|---|---|
| 管理后台（前端） | <http://localhost:8080> |
| API / Swagger | <http://localhost:19901/swagger> |
| Hangfire 面板 | <http://localhost:19901/pchangfire> |
| Qdrant 控制台 | <http://localhost:16333/dashboard> |
| MySQL | `127.0.0.1:13306`，库 `kevin_app` |
| Redis | `127.0.0.1:16379` |

编排要点：

- **自动迁移**：`api` 容器通过 `DB_AUTO_MIGRATE=true` 在启动阶段执行 EF Core 迁移（带重试等待 MySQL 就绪），首次部署自动建表并写入种子数据；设为 `false` 可关闭。
- **数据持久化**：`kevin_mysql_data` / `kevin_redis_data` / `kevin_qdrant_data` / `kevin_files`（上传文件）/ `kevin_logs`（log4net 日志）均为 named volume。
- **同源反向代理**：`web` 容器内 Nginx 将 `/api`、`/KevinFlies`、`/VarApi` 转发到 `api:8080`；关闭 `proxy_buffering`、超时放到 3600s 以支持 SSE / SignalR；`Host` 用 `$http_host` 保留端口，保证后端生成的文件 URL 正确。
- **配置覆盖**：端口、密码、镜像源集中在根目录 `.env`（模板 `.env.example`），键名沿用 `ConnectionStrings__dbConnection`、`SignalrRdisSetting__hostname`、`QdrantClientSetting__Url` 这类双下划线扁平形式。
- **依赖顺序**：`api` 通过 `depends_on` + `healthcheck` 等 MySQL、Redis 就绪后再启动；Qdrant 只需 `service_started`；RabbitMQ 用 `required: false`，未启用 profile 时不阻塞启动。

> [!IMPORTANT]
> 任何部署方式都 **不要** 把 `ASPNETCORE_ENVIRONMENT` 设为 `Production`：`EnvironmentConfigHelper` 只识别 `Development` / `Test` / 空值，空值即读取 `appsettings.json` 作为正式环境配置，设为 `Production` 会在启动阶段抛 KeyNotFoundException。compose 已刻意不设置该变量，9.2 的 systemd 示例同理不设置。

#### 单镜像构建（不使用 compose）

`App/WebApi/Dockerfile` 为多阶段构建，关键事实：

- 基础镜像 `mcr.microsoft.com/dotnet/aspnet:10.0` / `sdk:10.0`。
- 容器内固定监听 **8080**（`ENV ASPNETCORE_URLS=http://+:8080`、`EXPOSE 8080`）。
- 构建上下文必须是 **仓库根**，先 COPY 各 csproj 再 `dotnet restore` 以利用层缓存。
- 以非 root 用户（`$APP_UID`）运行，镜像内已预建 `/app/Log`、`/app/KevinFlies` 并 `chown`。

```bash
docker build -f App/WebApi/Dockerfile -t netcore-kevin .
docker run -d --name kevin-api -p 19901:8080 \
  -e ConnectionStrings__dbConnection="server=mysql;port=3306;database=kevin_app;user id=root;password=admin123" \
  -e ConnectionStrings__redisConnection="redis:6379,DefaultDatabase=0,password=123456" \
  -e QdrantClientSetting__Url=http://qdrant:6334 \
  -v kevin_files:/app/KevinFlies \
  netcore-kevin
```

数据库表需事先用 `Update-Database` 初始化，或额外传入 `-e DB_AUTO_MIGRATE=true` 交由启动阶段迁移；前端镜像为 `vue/kevin.web.vue/Dockerfile`（Node 构建 + Nginx 托管）。

### 9.2 Linux systemd 部署

**`/etc/systemd/system/kevin-webapi.service`：**

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
# 注意：不要设置 ASPNETCORE_ENVIRONMENT=Production，
# 不设置即按正式环境读取 appsettings.json（详见 9.1）

[Install]
WantedBy=multi-user.target
```

`ASPNETCORE_ENVIRONMENT` 保持不设置；确需区分环境时只能取 `Development` 或 `Test`，写成 `Production` 会让框架启动时抛 KeyNotFoundException。

```bash
sudo systemctl daemon-reload
sudo systemctl start kevin-webapi
sudo systemctl enable kevin-webapi
```

### 9.3 IIS 部署

`App/WebApi/web.config` 已内置两项关键设置：

- 移除 `WebDAVModule`，避免 PUT/DELETE 请求被拦截。
- `maxAllowedContentLength=4294967295`（4GB），配合应用内 `MultipartBodyLengthLimit=long.MaxValue` 支持大文件上传。

发布时使用文件夹发布（`FolderProfile`）后指向站点物理路径即可。

---

## 十、监控运维

### 10.1 日志管理

| 日志 | 位置 | 说明 |
|------|------|------|
| 文件日志 | 运行目录 `Log/`（相对 `Directory.GetCurrentDirectory()`） | 按 Debug/Info/Warn/Error 分文件，1MB 滚动 |
| 配置 | `App/WebApi/LogConfigs/log4.{Env}.config` | 按环境加载，缺失则跳过 log4net |
| 审计/HTTP 日志 | 数据库表（`HttpLogFilter` 落库） | 后台「Http 日志」「系统关键日志」页面可查 |
| 慢查询 | EF Core `SlowQueryInterceptor` | 超过阈值的 SQL 走日志 |

### 10.2 性能与状态监控

- **Hangfire 面板**：http://localhost:9901/pchangfire（任务、重试、失败记录）。
- **Redis**：Redis Insight 查看连接、内存、慢日志。
- **MySQL**：Prometheus + Grafana 或云监控。
- **进程侧**：`Program.cs` 将线程池最小线程数抬到 200，GC 硬限制 2GB（AI 场景），排障时关注线程池饥饿与内存。

### 10.3 常见问题

| 问题 | 排查/解决 |
|------|-----------|
| **数据库连接失败** | 检查 MySQL 启动、连接串；`Database=kevin_app` 与库名一致 |
| **Redis 连接失败** | 检查服务与密码；SignalR、分布式锁、Token 续签都依赖 Redis |
| **Qdrant 连接失败** | 检查 6333 端口；`QdrantClientSetting.Url` 不带协议前缀（如 `localhost`） |
| **模型鉴权失败（401）** | 核对模型 `ModelKey` 与服务商 Base URL；错误文案提示"模型鉴权失败" |
| **提问超上下文窗口（400）** | 调大模型 `MaxAskPromptSize`，或开启智能体上下文压缩、新建对话 |
| **向量维度不匹配** | `MemoryEmbeddingSize` / 模型 `EmbeddingValueSize` 与 Qdrant 集合维度保持一致，换模型要重建集合 |
| **AI 回复不符合预期但无报错** | 检查智能体提示词、`Temperature`、工具开关（`IsAITools`/`IsSkill`/`IsMcp`）与日志开关 |
| **Hangfire 任务重启后消失** | 当前为内存存储（`AddKevinHangfireMemoryStorage`），属预期；需持久化切 Redis 存储 |
| **对话入库失败 / 请求过大** | MySQL `max_allowed_packet`（默认 4MB）限制；按 `AIChatStorageSetting` 调整或调大服务端包限制 |
| **技能脚本保存被拦截** | 命中 `SkillToolSecuritySetting` URL 前缀白名单，或脚本包含受限文件/危险命令 |

### 10.4 故障排查指南

#### 10.4.1 启动失败

1. 端口占用：`netstat -ano | findstr :9901`
2. MySQL 连通：`mysql -h 127.0.0.1 -P 3306 -u root -p`
3. Redis 连通：`redis-cli -h 127.0.0.1 -p 6379 -a 123456 ping`
4. 日志：查看运行目录 `Log/` 最新文件与 `GlobalError` 输出。

#### 10.4.2 AI 工具调用失败

1. 确认后台工具记录已勾选给智能体，且 `ClassMethod` 与 `GetAITools` 的 `case` 完全一致。
2. 确认依赖的配置节已填（如豆包 Key、ASR Key、OSS）。
3. 查看日志中工具异常堆栈；工具返回以 `❌` 开头的文案说明是业务拦截（如域名白名单、危险命令）。
4. MCP 工具检查 `McpType` 与 `McpUrl`/`McpCommand`，可用「测试 MCP 连接」验证。

#### 10.4.3 知识库检索异常

1. `curl http://localhost:6333/health` 检查 Qdrant。
2. 确认知识库文档已上传且被定时任务处理（Hangfire 面板可见）。
3. 核对 Embedding 模型维度与集合配置；重排服务不可用只影响排序，不影响召回。

---

## 十一、安全规范

### 11.1 输入验证

- 使用数据注解（`[Required]`、`[StringLength]`）与模型校验；校验失败由 `InvalidModelStateResponseFactory` 统一返回 `errMsg`。
- 文件上传校验扩展名、大小（`Modality` 中音频/图片上限）与业务签名（`business`/`key`/`sign`）。

### 11.2 权限控制

- RBAC：菜单权限 + API 权限键 `{area}/{module}/{action}`，角色绑定权限点，`IdentityVerification.Authorization` 在授权策略中统一裁决。
- 超级管理员（`IsSuperAdmin`）默认拥有全部权限；调试期可用 `IsOpenPermission=false` 临时关闭校验（仅限本地）。
- OpenAI 兼容网关同样走租户与权限体系：Key 命中后换发对应用户 JWT，不绕过任何业务校验。

### 11.3 数据加密

- 密码：SHA256 哈希存储（种子 `admin/123456` 即此方式）。
- 敏感配置：`AESInfo`（Key 16/24/32 字节、IV 16 字节）用于 `AESEncryptor` 加解密，OpenAI 兼容 Key 即密文落库。**Key/IV 一旦更换，历史密文全部无法解密**，必须同步迁移或让用户重新生成 Key。
- 传输：生产环境启用 HTTPS（`app.UseHsts()` + `UseHttpsRedirection()`，证书可在 `Program.cs` Kestrel 段配置）。

### 11.4 安全防护

#### 11.4.1 危险命令拦截（AI 工具护栏）

`CommandGuardrails` 对 Shell/Python/HTTP 类工具统一生效：

- 危险命令黑名单（`rm -rf`、`del /s /q`、格式化磁盘等）；
- 受限文件保护（禁止读取 `appsettings.json` 等配置文件）；
- 工具输出截断与超时控制；
- 命中即返回 `❌ 安全拦截：...`，不会真正执行。

#### 11.4.2 域名白名单

- 智能体级：`TAIApps.AuthorizedDomains`（逗号分隔，默认 `*` 表示不限制）。Shell/HTTP 工具发起请求前用 `CommandGuardrails.IsUrlAuthorized` 校验，防止「`example.com.evil.com`」类绕过。
- 技能脚本级：`SkillToolSecuritySetting` 控制脚本中 http(s) 地址必须命中 `AllowedUrlPrefixes`，否则阻止保存。

#### 11.4.3 Skill 脚本安全

- 脚本执行使用独立进程（`PySubprocessScriptRunner` / `ScriptProcessRunner`），任务收尾必须清理临时产物。
- Windows 上优先使用 `pwsh` 执行 PowerShell 脚本。

---

## 十二、性能优化

### 12.1 数据库优化

| 优化项 | 说明 |
|--------|------|
| 默认索引 | `DBDefaultHasIndexFields` 统一为主键/时间/租户/操作人字段建索引 |
| 分页 | 统一 `dtoPagePar` + `Skip/Take`，避免全表加载 |
| 批量写入 | AI 消息按 `AIChatStorageSetting.MaxCharsPerSave` 拆分批次，避免单条 SQL 超过 `max_allowed_packet` |
| 慢查询 | `SlowQueryInterceptor` 输出慢 SQL，优先补索引 |

### 12.2 缓存策略

- 低频变更的字典/模型/智能体列表加 `[CacheDataFilter<T>(TTL = 60, UseToken = true)]`。
- 进程内缓存用于单实例热点数据；多实例共享数据放 Redis。
- OpenAI 兼容网关的 Key 匹配按租户缓存 5 分钟，减少 DB + 解密开销。

### 12.3 异步与并发

- 全程 `async/await`，避免 sync-over-async 占用工作线程。
- 线程池下限 200 已兜底，但热路径仍应保持异步。
- 长耗时任务（知识库向量化、一次性提醒）交给 Hangfire/后台任务，不在请求线程内做。

### 12.4 AI 调用优化

- 开启智能体上下文压缩（`IsAIMessageCompaction`），控制历史消息 Token。
- 合理设置 `MaxAskPromptSize` / `AnswerTokens`，避免无效超长请求。
- 需要高可用时配置 `FallbackModels`（主模型失败自动切换备选）与 `MaxRetries`。
- 重排（`RerankCount`）与召回（`MaxMatchesCount`）按知识库规模调参，避免无谓开销。

---

## 十三、附录

### 13.1 常用配置项速查

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| `IsOpenPermission` | 是否开启权限验证 | true |
| `TenantId` | 默认初始化租户 | 1000 |
| `Jwt:AccessTokenExpirationMinutes` | Token 有效期（分钟） | 60 |
| `Jwt:RefreshTokenExpirationDays` | 刷新 Token 有效期（天） | 1 |
| `MigrationsAssembly` | EF 迁移程序集 | Kevin.EntityFrameworkCore |
| `HangfireSetting.userSetting` | Hangfire 面板账号 | admin / admin |
| `QdrantClientSetting.MemoryEmbeddingSize` | 向量维度 | 2048 |
| `AITokenQuotaSetting.MonthlyQuota` | 每用户月度 Token 额度 | 200000000 |
| `OpenAICompatibleSetting.SessionExpirationHours` | 兼容接口会话空闲上限（小时） | 24 |
| `SkillToolSecuritySetting.IsOpenAllowedUrlPrefixes` | 是否启用脚本 URL 白名单 | false |

### 13.2 数据库字段索引

```json
{
  "DBDefaultHasIndexFields": "tableid,createtime,updatetime,deletetime,tenantid,createuserid,updateuserid,deleteuserid"
}
```

新增实体的通用审计/租户字段会按该列表自动建索引；特殊查询字段请在实体或 Fluent 配置中补充。

### 13.3 关键代码位置

| 能力 | 位置 |
|------|------|
| 服务装配入口 | `Kevin/Kevin.Web.Basics/Extensions/ServiceConfiguration.cs`（`ConfigureServices` / `UseKevin`） |
| 全局异常 | `Kevin/kevin.Module/Kevin.Common/App/Global/GlobalError.cs` |
| 统一响应 | `Kevin/Kevin.Web.Basics/Filters/ResultFilter.cs` |
| 权限裁决 | `Kevin/kevin.Module/kevin.Permission/Permission/Action/IdentityVerification.cs` |
| AI 对话 | `Kevin/kevin.Module/kevin.AI.AgentFramework/AIAgentService.cs` |
| AI 工具注册 | `Kevin/Application/Services/AI/AIAgentToolSkillService.cs` |
| 智能体编排 | `Kevin/Application/Services/AI/AIAppsService.cs` |
| 定时任务注册 | `Kevin/Application/TaskModuleConfigs/*.cs` |
| OpenAI 兼容网关 | `Kevin/Kevin.Web.Basics/Controllers/AI/OpenAI/` |
| 种子数据 | `Kevin/Domain/BaseDatas/` |

### 13.4 联系方式

- 项目地址：<https://gitee.com/netkevin-li/NetCoreKevin>（GitHub 镜像：<https://github.com/junkai-li/NetCoreKevin>）
- 教学文档：<https://blog.csdn.net/weixin_42629287/category_13037923.html>
- 交流群：仓库 `Doc/` 下含微信群二维码

---

**版本**: v1.1（按当前框架代码校准）
**最后更新**: 2026年10月
**维护团队**: NetCoreKevin 开发团队
