---
kind: configuration_system
name: 基于 ASP.NET Core 配置系统与环境分层的配置体系
category: configuration_system
scope:
    - '**'
source_files:
    - App/WebApi/Program.cs
    - App/WebApi/appsettings.json
    - App/WebApi/appsettings.Development.json
    - App/WebApi/appsettings.Test.json
    - App/WebApiMcp/App.WebApi.Mcp/appsettings.json
    - Kevin/kevin.Module/Kevin.Common/Helper/EnvironmentConfigHelper.cs
    - Kevin/kevin.Module/Kevin.log4Net/LoggingBuilderExtensions.cs
    - Kevin/kevin.Module/Kevin.log4Net/EnvironmentConfigHelper.cs
    - App/WebApi/LogConfigs/log4.config
    - vue/kevin.web.vue/.env.development
    - vue/kevin.web.vue/.env.pre
---

## 1. 使用的系统与框架

- **ASP.NET Core 内置配置系统**：通过 `WebApplication.CreateBuilder(args)` 构建 `IConfiguration`，并自动加载 `appsettings.json`、`appsettings.{Environment}.json`、环境变量、命令行参数等。
- **自定义环境检测与切换**：在 `Program.Main` 启动时调用 `Kevin.Common.Helper.EnvironmentConfigHelper.SetEnvironment(...)`，优先读取 `NETCORE_ENVIRONMENT`，回退到 `ASPNETCORE_ENVIRONMENT`，并将值写回两个环境变量，从而驱动 ASP.NET Core 选择对应的 `appsettings.*.json`。
- **log4net 独立配置文件**：日志使用 log4net，通过扩展方法 `UseKevinLog4Net()` 动态解析 `LogConfigs/log4.{Environment}.config`（Development/Test/正式），找不到文件时记录警告并跳过初始化。
- **前端 Vue 3 多环境配置**：使用 Vite/Vue CLI 约定的 `.env.development`、`.env.pre` 等文件，通过 `VUE_APP_*` 前缀注入到前端代码。

## 2. 关键文件与位置

| 类别 | 路径 | 作用 |
|---|---|---|
| WebApi 入口 | `App/WebApi/Program.cs` | 设置环境变量、注册服务、启用中间件 |
| 默认配置 | `App/WebApi/appsettings.json` | 数据库连接串、JWT、Hangfire、Consul、RabbitMQ、AI 服务密钥等所有运行时配置 |
| 开发配置 | `App/WebApi/appsettings.Development.json` | 覆盖默认配置中的连接串、日志级别等 |
| 测试配置 | `App/WebApi/appsettings.Test.json` | 覆盖默认配置用于测试环境 |
| MCP 应用配置 | `App/WebApiMcp/App.WebApi.Mcp/appsettings.json` | MCP 转发服务的轻量配置 |
| 环境工具类 | `Kevin/kevin.Module/Kevin.Common/Helper/EnvironmentConfigHelper.cs` | 定义 Development/Test/Formal 常量及读写 `NETCORE_ENVIRONMENT` |
| log4net 扩展 | `Kevin/kevin.Module/Kevin.log4Net/LoggingBuilderExtensions.cs` | 根据环境选择 `log4.{Env}.config` |
| log4net 默认配置 | `App/WebApi/LogConfigs/log4.config` | 按 Debug/Info/Warn/Error 分文件滚动输出 |
| 前端环境变量 | `vue/kevin.web.vue/.env.development`、`.env.pre` | 定义 `VUE_APP_API_BASE_URL` 等前端可访问变量 |

## 3. 架构与约定

### 3.1 环境分层模型
项目将运行环境抽象为三个枚举值：
- `Development` → 对应 `appsettings.Development.json`
- `Test` → 对应 `appsettings.Test.json`
- `Formal`（空字符串）→ 对应根 `appsettings.json`（即生产环境）

启动流程：`Program.Main` 先调用 `EnvironmentConfigHelper.GetEnvironment()` 获取当前环境变量，再调用 `SetEnvironment` 将其标准化后写回 `NETCORE_ENVIRONMENT` 和 `ASPNETCORE_ENVIRONMENT`，之后才创建 `WebApplication`。这样 ASP.NET Core 的配置系统就能按约定加载对应的 `appsettings.*.json`。

### 3.2 配置来源优先级
遵循 ASP.NET Core 标准顺序（由低到高覆盖）：
1. `appsettings.json`（基础配置）
2. `appsettings.{Environment}.json`（环境覆盖）
3. 环境变量（例如 `ConnectionStrings__dbConnection` 会覆盖 JSON 中的同名项）
4. 命令行参数

### 3.3 模块级配置组织
每个 kevin 子模块（如 `Kevin.Email`、`Kevin.RabbitMQ`、`Kevin.Cache`、`Kevin.SignalR`、`Kevin.Consul` 等）都提供独立的 `ServiceCollectionExtensions.cs`，通过 `AddSingleton/AddScoped` 等方式从 `IConfiguration` 或 `IOptions<T>` 读取自身配置，实现“配置即依赖”的模块化装配。

### 3.4 外部服务配置分组
`appsettings.json` 中按功能域分组存放第三方服务配置：
- 基础设施：`ConnectionStrings`、`HangfireSetting`、`HangfireRedisSetting`、`SignalrRdisSetting`
- 认证：`Jwt`
- 服务发现：`ConsulSetting`、`CorsSetting`
- 消息队列：`RabbitMQ`
- 云存储：`TencentCloudFileStorage`、`AliCloudFileStorage`、`QiniuCloudFileStorage`
- AI/RAG：`OllamaApiSetting`、`AliRerankApiSetting`、`DoubaoSearchApiSetting`、`QdrantClientSetting`
- 通知：`EmailSetting`、`DingDingMsgInfo`、`DingDingOAApiInfo`、`TencentCloudSMS`、`AliCloudSMS`
- 其他：`SnowflakeIdSetting`、`CodeGeneratorSetting`、`HttpInterToMcpService`、`MCPSseClient`

### 3.5 前端配置
Vue 工程使用 `.env.*` 文件管理前端环境变量，通过 `VUE_APP_` 前缀暴露给 `process.env`。`development` 指向线上 API 地址，`pre` 指向本地 `localhost:9901`，便于前后端联调。

## 4. 约定与约束

- **环境变量命名**：必须设置 `NETCORE_ENVIRONMENT` 或 `ASPNETCORE_ENVIRONMENT` 为 `Development`、`Test` 或空字符串（代表 Formal），否则默认回退到正式环境。
- **配置文件命名规范**：每个环境必须存在对应的 `appsettings.{Environment}.json`；日志配置必须放在 `LogConfigs/log4.{Environment}.config`，否则 log4net 初始化会被跳过。
- **敏感信息处理**：`appsettings.json` 中包含 JWT Key、各云厂商 SecretKey、API Key 等敏感配置，实际部署时应通过环境变量或外部配置源（如 Azure Key Vault、Consul KV）进行覆盖，避免硬编码。
- **EF 迁移程序集**：通过 `MigrationsAssembly` 配置项指定，不同环境可通过对应 `appsettings.*.json` 覆盖以指向不同的迁移程序集。
- **GC 堆限制**：在 `Program.cs` 中硬编码 `GCHeapHardLimit = 2GB`，注释说明是为 AI 应用调整，属于进程级配置而非配置文件项。
- **前端 API 地址**：通过 `VUE_APP_API_BASE_URL` 区分开发/预发布环境，构建期注入，不可在运行时修改。

## 5. 总结

该仓库采用标准的 ASP.NET Core 配置体系，结合自研的 `EnvironmentConfigHelper` 统一环境标识，并通过 `appsettings.json` + `appsettings.{Environment}.json` 的多层覆盖机制管理运行时配置；日志子系统独立使用 log4net 并按环境加载 XML 配置；前端通过 `.env.*` 管理构建期环境变量。整体结构清晰，但大量敏感配置仍直接写在 `appsettings.json` 中，建议在生产环境中改用环境变量或密钥管理服务覆盖。