<div align="center">

<img src="InitData/logo.png" width="140" alt="NetCoreKevin" />

# NetCoreKevin

**企业级 AI 智能体中台：.NET 10 做后端，Vue 3 做管理后台，知识库、技能、记忆、额度都在界面上配。**

An enterprise AI-agent platform on .NET 10 — RAG, skills, memory, multi-tenancy and scheduled tasks, with a Vue admin console.

<img src="https://img.shields.io/badge/status-actively--developed-orange" alt="status" />
<img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10" />
<img src="https://img.shields.io/badge/Vue-3-4FC08D?logo=vuedotjs&logoColor=white" alt="Vue 3" />
<img src="https://img.shields.io/badge/MySQL-8.0+-4479A1?logo=mysql&logoColor=white" alt="MySQL" />
<img src="https://img.shields.io/badge/Redis-7.0+-DC382D?logo=redis&logoColor=white" alt="Redis" />
<img src="https://img.shields.io/badge/Qdrant-vector_search-DC244C" alt="Qdrant" />
<img src="https://img.shields.io/badge/license-MIT-blue" alt="MIT" /> 
</div>

> [!NOTE]
> NetCoreKevin 面向「要在企业内网里落一个 AI 助手 / SaaS 智能体中台」的场景：多租户、登录鉴权、权限点、审计日志、定时任务、模型接入这些地基已经铺好，你只需要接自己的业务数据和业务系统。

<p align="center">
  <img src="Doc/Img/list1/3.png" width="881" alt="NetCoreKevin 智能体对话" />
</p>

## NetCoreKevin 是什么

把「企业里的 AI 助手」拆开，需要的东西其实就四样：

- **后端**：`App/WebApi`。.NET 10 + EF Core 10 + MySQL，按 Domain / Application / RepositorieRps / WebApi 四层拆分，API 带 v1、v2 版本。
- **AI 能力**：`Kevin/kevin.Module`。kevin.AI.AgentFramework 基于 Microsoft Agent Framework，kevin.RAG 负责切分、向量化与重排，都在同一个进程里，不用额外起 Python 服务。
- **管理后台**：`vue/kevin.web.vue`。Vue 3 + Ant Design Vue，租户、用户、角色、智能体、知识库、技能、任务、API Key 全在界面上配。
- **底座**：25 个可插拔模块——权限、缓存、消息队列、分布式锁、SignalR、Hangfire、对象存储、短信、邮件、代码生成器。

它不是脚手架 demo：装好依赖、跑完迁移就能登录后台，有菜单、有权限、有定时任务、有审计日志。

## 特点

### 智能体是配置出来的，不是写死的

- **提示词 / 模型 / 参数**：每个智能体单独配置，对话模型与向量模型分开选，温度、上下文裁剪都能调。
- **技能动态加载**：技能是 `SKILL.md` + 脚本（`.py` / `.ps1` / `.sh`），后台能新建、在线编辑、随时启停，改完不用重新编译。
- **知识库（RAG）**：上传文档自动切分写入 Qdrant，检索结果过一遍 `qwen3-rerank` 重排再喂给模型。
- **记忆**：按「租户 + 智能体 + 用户」三维隔离存在 Qdrant，是否使用记忆挂在智能体开关上。
- **多智能体协同**：主智能体可以把子任务派给其他智能体执行。
- **上下文压缩**：按配置阈值压缩历史消息，长期使用不会越聊越慢、越聊越贵。
- **联网搜索**：接入火山引擎豆包搜索，模型需要新信息时自己去查。
- **全模态**：图片理解、语音转写（阿里 ASR）、文生图（生成后自动上传 OSS）。

### 能当企业底座用

- **一库多租户**：共享库按 `TenantId` 隔离，接口层强制过滤，不用每个租户一套库。
- **RBAC 权限**：菜单与 API 两个维度，权限点跟角色走；调试期可以用 `IsOpenPermission` 一把关掉。
- **额度与审计**：每用户每月 Token 额度由定时任务颁发、请求时扣减；审计日志、慢查询告警、分环境文件日志都内置。
- **对外集成**：腾讯云 / 阿里云 / 七牛三种对象存储，腾讯与阿里短信，钉钉消息，RabbitMQ / CAP 消息总线，Consul 服务发现，SignalR 实时推送。

### 给别的客户端留了口子

- **OpenAI 协议兼容接口**：`GET /v1/models`、`POST /v1/chat/completions`（支持 stream）。Cherry Studio、Dify、自研客户端只要配一个 `sk-` 开头的 Key 就能接进来；Key 在后台按用户和租户发放，权限校验、工具调用、聊天记录落库与网页端完全一致。
- **MCP**：`App/WebApiMcp` 提供 MCP 服务端，配置里也支持把现有 HTTP 接口包装成 MCP 工具，或把外部 MCP 服务接成智能体工具。

### 工程化

- **25 个可插拔模块**：每个能力一个 csproj，通过 `IModuleInitializer` / `IModuleConfigTasks` 自行注册，加能力不用改入口文件。
- **代码生成器**：按实体一键生成仓储接口、仓储实现、服务接口、服务实现与控制器。
- **测试**：xUnit 单元测试 + 集成测试，共享测试基础设施。
- **容器化**：`docker compose up -d --build` 一键拉起前端 + 后端 + MySQL + Redis + Qdrant；`App/WebApi/Dockerfile`、`vue/kevin.web.vue/Dockerfile` 也可单独构建镜像。

## 效果图

### AI 智能体技能工具管理

| 动态管理 | 智能体配置 | 对话交互 |
|---|---|---|
| ![动态管理](Doc/Img/list1/1.png) | ![智能体配置](Doc/Img/list1/2.png) | ![对话交互](Doc/Img/list1/3.png) |

### 多智能体协同与聊天记录压缩

| 多智能体动态配置 | 压缩策略配置 | 对话交互 |
|---|---|---|
| ![多智能体动态配置](Doc/Img/list7/1.png) | ![压缩策略配置](Doc/Img/list7/2.png) | ![对话交互](Doc/Img/list7/3.png) |

### Skill 动态管理

| 界面管理 | 在线编辑 | 动态配置 |
|---|---|---|
| ![界面管理](Doc/Img/list6/1.png) | ![在线编辑](Doc/Img/list6/2.png) | ![动态配置](Doc/Img/list6/3.png) |

### AI 知识库系统

| 知识库管理 | 文档上传 | 智能问答 |
|---|---|---|
| ![知识库管理](Doc/Img/list2/1.png) | ![文档上传](Doc/Img/list2/2.png) | ![智能问答](Doc/Img/list2/3.png) |

### AI 智能体技能

| 技能列表 | 技能配置 | 技能执行 |
|---|---|---|
| ![技能列表](Doc/Img/list3/1.png) | ![技能配置](Doc/Img/list3/2.png) | ![技能执行](Doc/Img/list3/3.png) |

### AI 智能体自动任务调度

| 任务列表 | 任务配置 | 执行记录 |
|---|---|---|
| ![任务列表](Doc/Img/list4/1.png) | ![任务配置](Doc/Img/list4/2.png) | ![执行记录](Doc/Img/list4/3.png) |

### 后台管理（Vue 3 + Ant Design Vue）

| 用户管理 | 角色管理 | 权限管理 | 系统关键日志 |
|---|---|---|---|
| ![用户管理](Doc/Img/list5/1.png) | ![角色管理](Doc/Img/list5/2.png) | ![权限管理](Doc/Img/list5/3.png) | ![系统关键日志](Doc/Img/list5/4.png) |

## 架构

```mermaid
flowchart LR
  subgraph FE["Vue 3 管理后台"]
    UI["用户 / 租户 / 权限<br/>智能体 · 知识库 · 技能 · 任务"]
  end

  subgraph API["App.WebApi（.NET 10）"]
    C["Controller v1 / v2<br/>JWT · 权限 · 全局异常"]
    A["Application<br/>业务编排"]
    D["Domain<br/>实体 / 接口契约"]
    R["RepositorieRps<br/>EF Core 仓储"]
  end

  subgraph AI["AI 能力层（同进程）"]
    AG["AgentFramework<br/>对话 · 技能 · 记忆"]
    RG["RAG<br/>切分 · 向量化 · 重排"]
    SK["Skill 脚本<br/>py · ps1 · sh"]
  end

  subgraph INFRA["基础设施"]
    M[("MySQL")]
    RD[("Redis")]
    Q[("Qdrant")]
    MQ["RabbitMQ / CAP"]
    HF["Hangfire"]
    CS["Consul"]
  end

  LLM["模型服务：任意 OpenAI 协议兼容<br/>（OpenAI · 智谱 · DeepSeek · Ollama）<br/>豆包搜索 · 阿里 ASR / Rerank"]

  UI <-->|"REST / SignalR"| C
  C --> A
  A --> D
  A --> R
  R --> M
  A --- AI
  AG --> LLM
  RG --> Q
  A --- RD
  A --- MQ
  A --- HF
  A --- CS
```

各层通过自己的 `ModuleInitializer` 暴露注册逻辑，由上层统一调用完成 DI 装配：`WebApi` 只负责启动与管道，业务契约留在 `Domain`，实现落在 `Application` 与 `RepositorieRps`。

## 目录

| 路径 | 内容 |
|---|---|
| [`App/`](App/) | 业务应用：Domain、Application、RepositorieRps、WebApi、WebApiMcp |
| [`Kevin/`](Kevin/) | 框架核心：EF Core（含慢查询拦截器）、Web 基础组件、25 个可插拔模块 |
| [`vue/kevin.web.vue/`](vue/kevin.web.vue/) | Vue 3 + Ant Design Vue 管理后台 |
| [`Test/`](Test/) | xUnit 单元测试、集成测试、共享测试基础设施 |
| [`Doc/`](Doc/) | 截图与文档资源 |
| [`InitData/`](InitData/) | logo（svg + png）与省市区镇等初始化数据 |

## 一键部署（Docker Compose）

不想在本机装 .NET SDK、Node、MySQL、Redis？一条命令把 **前端 + 后端 + MySQL + Redis + Qdrant** 全套拉起来。

### 1. 准备

| 项 | 要求 |
|---|---|
| Docker | 20.10+，需带 `docker compose` v2（Docker Desktop 自带） |
| 内存 | 建议 8GB 以上（前端 webpack 构建 + MySQL 比较吃内存） |
| 磁盘 | 约 6GB（.NET SDK 基础镜像比较大） |

### 2. 一条命令启动

```bash
git clone https://gitee.com/netkevin-li/NetCoreKevin.git
cd NetCoreKevin
docker compose up -d --build
```

这就是全部步骤。首次执行会构建前后端镜像并拉取依赖，视网络情况 10~30 分钟；之后再启动只需几秒。

镜像已构建过、只想起服务时：

```bash
# 启动全部服务
docker compose up -d

# 停止（数据卷保留，不会丢数据）
docker compose down
```

### 3. 访问地址

| 服务 | 地址 |
|---|---|
| 管理后台（前端） | <http://localhost:8080> |
| API / Swagger | <http://localhost:19901/swagger> |
| Hangfire 面板 | <http://localhost:19901/pchangfire> |
| Qdrant 控制台 | <http://localhost:16333/dashboard> |
| MySQL | `127.0.0.1:13306`，库 `kevin_app` |
| Redis | `127.0.0.1:16379` |

默认账号 `admin` / `123456`，租户 `1000`。

> [!NOTE]
> 宿主机端口默认避开了本地开发常用端口（3306 / 6379 / 9901），可以和本机已装的 MySQL、Redis、VS 调试中的后端同时运行。

### 4. 首次启动自动做了什么

- **自动建表**：`api` 容器启动时执行 EF Core 迁移（`DB_AUTO_MIGRATE=true`），自动创建全部业务表并写入种子数据，不用手动 `Update-Database`；后续升级代码也会自动增量迁移。
- **数据持久化**：MySQL、Redis、Qdrant 数据与上传文件（`/KevinFlies`）、log4net 日志（`Log`）都落在 named volume 里，`docker compose down` 不会丢。
- **前后端同源**：前端容器内 Nginx 直接把 `/api`、`/KevinFlies`、`/VarApi` 反代到 `api:8080`（含 SSE / SignalR 所需的长连接配置），所以不存在跨域问题。

### 5. 改端口 / 改密码

所有可调项都在 `.env` 里，仓库提供了一份示例：

```bash
# Windows PowerShell
Copy-Item .env.example .env

# Linux / macOS
cp .env.example .env
```

编辑 `.env`，常改的几个：

```ini
WEB_PORT=8080               # 前端入口
API_PORT=19901              # 后端 / Swagger
MYSQL_PORT=13306
REDIS_PORT=16379
MYSQL_ROOT_PASSWORD=admin123
REDIS_PASSWORD=123456
DB_AUTO_MIGRATE=true        # 设为 false 则启动时不自动迁移
TAG=latest                  # 镜像 tag，便于升级回滚
```

改完重新执行 `docker compose up -d` 生效。

> [!WARNING]
> MySQL 密码只在**数据卷首次初始化**时生效。已经启动过再改 `MYSQL_ROOT_PASSWORD`，容器会用旧密码，需要 `docker compose down -v` 清空数据重来（会丢库），或进容器手动改账号。

### 6. 可选：一起启动 RabbitMQ

RabbitMQ 是懒加载的，不启动也不影响主流程；需要消息队列时加 `--profile mq`：

```bash
docker compose --profile mq up -d
```

管理台 <http://localhost:15672>，默认 `admin` / `admin`，vhost `/app_webapi`。

### 7. 常用命令

```bash
docker compose ps                      # 查看各服务状态
docker compose logs -f api             # 跟踪后端日志
docker compose logs -f web             # 跟踪 Nginx 日志
docker compose up -d --build api       # 只重建并重启后端
docker compose up -d --build web       # 只重建并重启前端
docker compose restart api             # 重启后端
docker compose down                    # 停止全部（保留数据）
docker compose down -v                 # 停止并删除数据卷（清库，慎用）
```

### 8. 常见问题

| 现象 | 处理 |
|---|---|
| `port is already allocated` | 宿主机端口被占用，改 `.env` 里对应端口后 `docker compose up -d` |
| 基础镜像拉不动 / 超时 | 配置 Docker 镜像加速，或在 `.env` 里用 `MYSQL_IMAGE` / `REDIS_IMAGE` / `QDRANT_IMAGE` 指定可用镜像站地址 |
| 前端 `npm install` 很慢 | 默认走 npmmirror，可 `NPM_REGISTRY=https://registry.npmjs.org` 切换 |
| `api` 一直重启 | `docker compose logs api` 看原因；多为连不上 MySQL 或迁移失败 |
| 想重新初始化数据库 | `docker compose down -v` 再 `docker compose up -d` |

> [!IMPORTANT]
> 不要给 `api` 容器设置 `ASPNETCORE_ENVIRONMENT=Production`：框架的 `EnvironmentConfigHelper` 只识别 `Development` / `Test` / 空值，空值即读取 `appsettings.json` 作为正式环境配置。

## 快速开始

以上是容器化部署。如果要在本机做二次开发、调试后端代码，按下面步骤本地运行。

### 你需要准备

| 项 | 说明 |
|---|---|
| .NET SDK | 10.0+ |
| Node.js | 16+，只跑管理后台时需要 |
| MySQL | 8.0+，建议 `utf8mb4` |
| Redis | 7.0+，缓存、Hangfire、SignalR 都用它 |
| Qdrant | 1.7+，知识库与记忆功能需要 |
| 模型服务 | 任意 OpenAI 协议兼容服务——OpenAI / 智谱 / DeepSeek / 通义等云端 API，或 Ollama 等本地部署，有一个即可 |

### 1. 克隆代码

```bash
git clone https://gitee.com/netkevin-li/NetCoreKevin.git
cd NetCoreKevin
dotnet restore
```

GitHub 镜像：<https://github.com/junkai-li/NetCoreKevin>

### 2. 配置连接串

编辑 `App/WebApi/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "dbConnection": "server=127.0.0.1;port=3306;database=kevin_app;user id=root;password=admin123",
    "redisConnection": "127.0.0.1:6379,DefaultDatabase=0,password=123456"
  }
}
```

同一个文件里还有 Hangfire、Consul、对象存储、模型服务等配置，按需覆盖；`appsettings.Development.json` 会覆盖同名项。

### 3. 初始化数据库

Visual Studio 的**程序包管理控制台**，默认项目选 `Kevin.EntityFrameworkCore`，启动项目选 `App.WebApi`：

```powershell
Update-Database
```

或者用 CLI：

```bash
dotnet ef database update --project Kevin/Kevin.EntityFrameworkCore --startup-project App/WebApi
```

> [!TIP]
> 仓库里已经带了历次迁移记录，全新库直接 `Update-Database` 即可。只有改了实体才需要先 `Add-Migration "你的改动"` 再更新。

### 4. 启动后端

```bash
dotnet run --project App/WebApi
```

`launchSettings.json` 里已经指定 `ASPNETCORE_ENVIRONMENT=Development` 与 `http://localhost:9901`。

### 5. 启动管理后台

```bash
cd vue/kevin.web.vue
npm install
npm run serve
```

开发服务器已把 `/VarApi` 反向代理到 `http://localhost:9901`，不需要额外配跨域。

### 访问地址

| 服务 | 地址 |
|---|---|
| API | <http://localhost:9901> |
| Swagger | <http://localhost:9901/swagger> |
| Hangfire 面板 | <http://localhost:9901/pchangfire> |
| 管理后台 | <http://localhost:8080> |

Hangfire 面板默认走 Basic Auth，账号密码取 `appsettings.json` 的 `HangfireSetting.userSetting`（默认 `admin` / `admin`）。

### 默认账户

| 用户名 | 密码 | 租户 |
|---|---|---|
| admin | 123456 | 1000 |

> [!WARNING]
> 以上都是本地开发默认值。上线前请替换数据库密码、`Jwt.Key`、Hangfire 账号，以及 `AESInfo` 的 Key / IV——Key 与 IV 必须是 16/24/32 与 16 字节，且一旦变更，历史密文将无法解密。

## AI 能力配置

### 1. 启动 Qdrant

```bash
docker run -d --name qdrant -p 6333:6333 -v qdrant_storage:/qdrant/storage qdrant/qdrant
```

`QdrantClientSetting` 默认指向 `localhost`，本机跑的话不用改。

> [!TIP]
> 用 Docker Compose 一键部署时 Qdrant 已经包含在内，后端通过 `QdrantClientSetting__Url=http://qdrant:6334` 走 gRPC 直连，不需要本节操作。

### 2. 配置模型

向量模型与对话模型在后台「模型管理」里配，也可以直接改 `appsettings.json`。模型接入走 OpenAI 协议：只要服务商提供 OpenAI 兼容接口，填好地址、模型名和 Key 就能用——OpenAI、智谱、DeepSeek、通义、Kimi、vLLM、Ollama 都在支持范围内。下面以智谱和 Ollama 为例：

**云端服务（以智谱为例，[注册地址](https://open.bigmodel.cn)；换成其它 OpenAI 兼容服务的地址和 Key 即可）：**

```json
{
  "OllamaApiSetting": {
    "Url": "https://open.bigmodel.cn/api/paas/v4/embeddings",
    "DefaultModel": "embedding-3",
    "ApiKey": "your-api-key"
  }
}
```

**Ollama（本地，原生接口不传 ApiKey）：**

```json
{
  "OllamaApiSetting": {
    "Url": "http://localhost:11434/api/embeddings",
    "DefaultModel": "qwen3:4b"
  }
}
```

重排模型走 `AliRerankApiSetting`（默认 `qwen3-rerank`），不配也能用，只是检索结果不做二次排序。

### 3. 跑通第一个智能体

1. 登录后台，进入模型管理，配置对话模型与向量模型。
2. 新建知识库，选择向量模型（维度需与 `QdrantClientSetting.MemoryEmbeddingSize` 一致，默认 2048），上传文档等待向量化完成。
3. 新建智能体：选对话模型，绑知识库，按需勾选技能与记忆。
4. 新建对话，开始聊。

<p align="center">
  <img src="Doc/%E9%A1%B9%E7%9B%AE%E7%9B%B8%E5%85%B3/image.png" width="440" alt="知识库模型配置" />
  <img src="Doc/Img/list1/2.png" width="440" alt="智能体配置" />
</p>

### 4. 用 Ollama 跑本地模型（可选）

- 支持 macOS、Windows、Linux 与 Docker 部署，官方下载：<https://ollama.com/download>
- 硬件建议：4 核以上 CPU、16GB 以上内存、足够的磁盘空间；想跑大模型或微调再上 NVIDIA GPU。
- 拉一个能跑得动的小模型先试：

```bash
ollama run qwen3:4b
```

然后在后台把对话模型的地址指向 `http://localhost:11434`。

![本地模型配置](Doc/localve.png)

### 5. 自动任务（Hangfire）

Hangfire 存储当前默认使用内存（`AddKevinHangfireMemoryStorage`），Redis 存储的注册代码已保留，可在 `ServiceConfiguration` 中切换。

继承 `IModuleConfigTasks` 实现 `ConfigTasks`，任务会在启动时自动注册，并且可以直接调用应用服务：

```csharp
/// <summary>
/// AIKmssTasks 配置任务设置
/// </summary>
public class AIKmssModuleConfigTasks : IModuleConfigTasks
{
    /// <summary>
    /// 配置任务
    /// </summary>
    public Task<bool> ConfigTasks(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<IAIKmssService>(
            recurringJobId: "每5分钟检测是否有AI文档知识库需要处理",   // 唯一 ID，后续修改或删除都用它
            (s) => s.ProcessKmssVectorData(default),
            "0 0/5 0/1 * * ? ",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,   // 指定时区，默认 UTC
            }
        );
        return Task.FromResult(true);
    }
}
```

## 项目结构

```
kevin.abp.core/
├── App/                    # 业务应用
│   ├── Application/        # 应用服务层
│   ├── Domain/             # 领域层
│   ├── RepositorieRps/     # 仓储实现
│   ├── AppShare/           # 跨层共享 DTO
│   ├── WebApi/             # API 入口
│   └── WebApiMcp/          # MCP 服务端
├── Kevin/                  # 框架核心
│   ├── Application/        # 核心服务（含 AI）
│   ├── Domain/             # 核心领域模型
│   ├── Kevin.EntityFrameworkCore/   # EF Core 实现（含慢查询拦截器、迁移）
│   ├── Kevin.Web.Basics/   # Web 基础组件（控制器、全局过滤器）
│   ├── RepositorieRps/     # 核心仓储实现
│   ├── kevin.Share/        # 共享 DTO、枚举、常量
│   └── kevin.Module/       # 可插拔模块（25 个）
│       ├── kevin.AI.AgentFramework/  # 智能体框架
│       ├── Kevin.RAG/                # 检索增强（Qdrant / Ollama / Rerank）
│       ├── Kevin.Authentication.Jwt/ # JWT 认证
│       ├── kevin.Permission/         # RBAC 权限
│       ├── kevin.Cache/              # 缓存
│       ├── Kevin.Hangfire/           # 任务调度
│       ├── Kevin.SignalR/            # 实时通信
│       ├── kevin.Cap/ · kevin.RabbitMQ/  # 消息总线
│       ├── Kevin.SMS/ · Kevin.Email/ · kevin.FileStorage/
│       ├── kevin.DistributedLock/ · Kevin.SnowflakeId/
│       └── ...                       # ASR、Consul、代码生成器等
├── Test/                   # 测试套件
│   ├── Kevin.Web.Test/     # 单元测试（xUnit）
│   ├── Kvin.Integration.Tests/  # 集成测试
│   └── Testing.Shared/     # 共享测试基础设施
├── vue/kevin.web.vue/      # 管理后台（Vue 3 + Ant Design Vue）
├── Doc/                    # 文档与截图
└── InitData/               # 初始化数据
```

## 功能模块

| 模块 | 功能 | 状态 |
|---|---|---|
| **用户 / 角色 / 权限** | 用户 CRUD、角色 CRUD、菜单与 API 权限点 | ✅ |
| **多租户** | 一库多租户、租户级数据隔离 | ✅ |
| **AI 智能体** | 智能对话、工具调用、多智能体协同、上下文压缩 | ✅ |
| **知识库** | 文档上传、切分向量化、RAG 检索与重排 | ✅ |
| **技能（Skill）** | 在线编辑、动态启停、脚本执行、地址白名单校验 | ✅ |
| **记忆** | 租户 + 智能体 + 用户三维隔离 | ✅ |
| **多模态** | 图片理解、语音转写、文生图 | ✅ |
| **OpenAI 兼容** | `/v1/models`、`/v1/chat/completions`、Key 管理 | ✅ |
| **任务调度** | Hangfire 定时任务与面板 | ✅ |
| **Token 额度** | 月度额度颁发、请求扣减 | ✅ |
| **文件存储** | 腾讯云 / 阿里云 / 七牛 | ✅ |
| **消息服务** | 钉钉消息、站内 SignalR 推送 | ✅ |

## 文档资源

- **项目 Wiki**：[快速开始](.qoder/repowiki/zh/content/快速开始.md)——`.qoder/repowiki/zh/content/` 下覆盖架构设计、核心模块、开发指南、API 接口、数据库设计、部署运维
- **系统文档**：[SYSTEM_DOCUMENTATION.md](SYSTEM_DOCUMENTATION.md)
- **教学专栏**：[CSDN](https://blog.csdn.net/weixin_42629287/category_13037923.html)
- **二次开发教程**：[基于 NetCoreKevin 新建项目](https://gitee.com/netkevin-li/ainet)

## 交流社区

| 微信 | 交流群 |
|---|---|
| ![微信](Doc/wx.jpeg) | ![交流群](Doc/wx_jiaoliuqun.JPG) |

## Star History

<a href="https://star-history.dera.page/#junkai-li/NetCoreKevin&type=timeline">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://star-history.dera.page/svg?repos=junkai-li/NetCoreKevin&type=timeline&theme=dark" />
    <source media="(prefers-color-scheme: light)" srcset="https://star-history.dera.page/svg?repos=junkai-li/NetCoreKevin&type=timeline" />
    <img alt="Star History Chart" src="https://star-history.dera.page/svg?repos=junkai-li/NetCoreKevin&type=timeline" />
  </picture>
</a>

## License

MIT License，详见 [LICENSE](LICENSE)。

欢迎提 Issue 和 PR。仓库还在持续迭代，改动会优先进入 `master` 分支。

**版本**：v1.1（代码审查治理版）　**维护者**：NetCoreKevin 开发团队

## 鸣谢
<p>
  <a href="https://atomgit.com/NetCoreKevin_li/NetCoreKevin/star/new_badge.svg">
    <img alt="AtomGit G-Star" width="60"  src="https://atomgit.com/NetCoreKevin_li/NetCoreKevin/star/new_badge.svg"></a>
</p>
