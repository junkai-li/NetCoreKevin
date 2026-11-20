---
kind: configuration_system
name: 基于 ASP.NET Core Configuration + appsettings 分层的环境配置系统
category: configuration_system
scope:
    - '**'
source_files:
    - App/WebApi/Program.cs
    - App/WebApi/appsettings.json
    - App/WebApi/appsettings.Development.json
    - App/WebApi/appsettings.Test.json
    - Kevin/Kevin.Web.Basics/Extensions/ServiceConfiguration.cs
    - App/WebApiMcp/App.WebApi.Mcp/EnvironmentConfigHelper.cs
    - Kevin/kevin.Module/kevin.log4Net/EnvironmentConfigHelper.cs
    - Kevin/kevin.Module/kevin.Email/EmailSetting.cs
    - Kevin/kevin.Module/kevin.RabbitMQ/RabbitMQOptions.cs
    - Kevin/kevin.Module/kevin.AI.AgentFramework/AISetting.cs
---

## 1. 采用的系统与工具

- 框架：ASP.NET Core 内置的 `IConfiguration` / `IOptions<T>` 配置模型。
- 配置文件：JSON（`appsettings.json`、`appsettings.Development.json`、`appsettings.Test.json`）+ 环境变量（`NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`）+ 进程级环境变量（`DB_AUTO_MIGRATE`）。前端 Vue 项目使用 `.env` / `.env.development` / `.env.pre`。
- 日志配置：log4net，通过 `Kevin.log4Net/LoggingBuilderExtensions.cs` 的扩展方法 `UseKevinLog4Net()` 接入，并配合 `LogConfigs/log4.{Development,Test,config}.config` 按环境加载。
- 数据库迁移：EF Core，通过 `MigrationsAssembly` 配置项指定程序集；容器启动时根据环境变量 `DB_AUTO_MIGRATE=true` 自动执行 `Database.Migrate()` 或 `EnsureCreated()`。

## 2. 关键文件与位置

| 角色 | 路径 |
|---|---|
| WebApi 入口（构建 IHost、注册服务、读取配置） | `App/WebApi/Program.cs` |
| 默认应用配置 | `App/WebApi/appsettings.json` |
| 开发/测试环境覆盖 | `App/WebApi/appsettings.Development.json`、`App/WebApi/appsettings.Test.json` |
| 统一服务注册与配置绑定中心 | `Kevin/Kevin.Web.Basics/Extensions/ServiceConfiguration.cs` |
| 环境变量解析与设置（WebApiMcp 侧） | `App/WebApiMcp/App.WebApi.Mcp/EnvironmentConfigHelper.cs` |
| 环境变量解析（log4net 侧） | `Kevin/kevin.Module/kevin.log4Net/EnvironmentConfigHelper.cs` |
| 配置 POCO 示例 | `Kevin/kevin.Module/kevin.Email/EmailSetting.cs`、`Kevin/kevin.Module/kevin.RabbitMQ/RabbitMQOptions.cs`、`Kevin/kevin.Module/kevin.AI.AgentFramework/AISetting.cs` |
| Vue 前端环境变量 | `vue/kevin.web.vue/.env*` |

## 3. 架构与约定

### 3.1 启动流程中的配置加载顺序

`Program.Main` 中显式调用 `EnvironmentConfigHelper.SetEnvironment(EnvironmentConfigHelper.GetEnvironment())`，先读取 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`，再写回两个环境变量，确保后续 ASP.NET Core 配置系统能识别当前环境。随后创建 `WebApplication.CreateBuilder(args)`，调用 `builder.Logging.UseKevinLog4Net()` 和 `builder.Services.ConfigureServices(builder.Configuration)`，最后 `app.UseKevin(builder.Configuration)`。

### 3.2 环境划分与覆盖机制

- 三个环境常量定义在 `App/WebApiMcp/App.WebApi.Mcp/EnvironmentConfigHelper.cs`：`Development`、`Test`、`Formal`（空字符串代表正式环境）。
- `SetEnvironment` 会把传入值同时写入 `NETCORE_ENVIRONMENT` 和 `ASPNETCORE_ENVIRONMENT`，并在控制台打印 `EnvironmentDictionary` 中的说明。
- 对应三份 JSON 文件：`appsettings.json`（正式）、`appsettings.Development.json`、`appsettings.Test.json`，三者结构基本一致，仅连接串、密钥等环境差异字段不同。

### 3.3 配置分块与强类型绑定

每个外部依赖模块都有对应的 POCO 类，并通过 `IConfiguration.GetRequiredSection("X").Get<T>()` 绑定：

- `EmailSetting` ← `EmailSetting` 节
- `RabbitMQOptions` ← `RabbitMQ` 节
- `AISetting` ← `OllamaApiSetting` 节（命名不完全一致，由 `ServiceConfiguration.cs` 映射）
- `HangfireSetting`、`SignalrRdisSetting`、`CorsSetting`、`CodeGeneratorSetting`、`QdrantClientSetting`、`AliRerankApiSetting`、`SnowflakeIdSetting` 等同理

`ServiceConfiguration.ConfigureServices` 是统一的装配点：所有 `services.AddXxx(...)` 都在此集中完成，从 `Configuration` 取值后注入到各模块的 `ServiceCollectionExtensions`。

### 3.4 连接字符串与全局静态配置

除了 `IOptions<T>` 模式，还有两处直接通过 `Configuration` 访问的全局静态配置：

- `Repository.Database.KevinDbContext.ConnectionString = Configuration.GetConnectionString("dbConnection")`
- `Repository.Database.KevinDbContext.DBDefaultHasIndexFields = Configuration.GetRequiredSection("DBDefaultHasIndexFields").Get<string>().Split(",").ToList()`

这两行位于 `RegisterAppServices` 中，在 `AddDbContextPool` 之前执行，使 EF Core 上下文能拿到连接串。

### 3.5 运行时环境变量（Feature Flags / 开关）

- `DB_AUTO_MIGRATE`：容器启动时若为 true，则自动执行 EF 迁移（最多重试 10 次，每次间隔 5 秒），用于首次部署建表。
- `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`：决定加载哪个 `appsettings.*.json`。
- `GCHeapHardLimit`：硬编码为 2GB，注释说明是为 AI 应用调大堆限制。
- ThreadPool 最小线程数：若小于 200 则提升到 200，避免 Redis 同步调用导致积压。

### 3.6 前端配置

Vue 项目在 `vue/kevin.web.vue/` 下使用 `.env`、`.env.development`、`.env.pre` 管理前端 API 地址等变量，与后端 .NET 配置体系相互独立。

## 4. 约定与约束

- **环境文件命名约定**：通过 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT` 选择 `appsettings.{Environment}.json`，支持 Development、Test、Formal（空串）三种。
- **配置来源优先级**：进程环境变量 → `appsettings.{Environment}.json` → `appsettings.json`，这是 ASP.NET Core 默认行为，代码未自定义 `ConfigurationBuilder`。
- **敏感配置存放位置**：JWT Key、Redis 密码、云厂商 SecretKey、AI ApiKey、AES Key/IV 等全部写在 `appsettings*.json` 中，没有使用 Azure Key Vault、User Secrets 或加密配置源；代码注释明确提醒 AES Key/IV 变更会导致历史密文不可解密。
- **新增外部依赖的配置应遵循的模式**：在 `appsettings.json` 增加节 → 新建 POCO 类 → 在 `ServiceConfiguration.ConfigureServices` 中通过 `Configuration.GetRequiredSection("X").Get<T>()` 绑定并注入。
- **数据库初始化策略**：生产环境不设置 `DB_AUTO_MIGRATE` 即关闭自动迁移，需手工初始化；容器场景可开启以自动建表。
- **日志配置与环境联动**：log4net 的 `EnvironmentConfigHelper.GetEnvironment()` 同样读取 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`，配合 `LogConfigs/log4.{Development,Test,config}.config` 实现日志级别按环境切换。
- **前端环境变量与后端解耦**：Vue 的 `.env*` 文件与 .NET 的 `appsettings*` 文件各自维护，不存在跨语言共享配置文件的机制。