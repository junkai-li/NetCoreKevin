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
    - Kevin/kevin.Module/Kevin.Common/Helper/EnvironmentConfigHelper.cs
    - Kevin/kevin.Module/Kevin.Common/Helper/ConfigHelper.cs
    - Kevin/kevin.Module/kevin.log4Net/LoggingBuilderExtensions.cs
    - App/WebApi/LogConfigs/log4.config
    - App/WebApi/LogConfigs/log4.Development.config
    - Kevin/kevin.Module/Kevin.Authentication.Jwt/ServiceCollectionExtensions.cs
    - Kevin/kevin.Module/kevin.Permission/ServiceCollectionExtensions.cs
    - Kevin/kevin.Module/kevin.RabbitMQ/RabbitMQOptions.cs
    - Kevin/kevin.Module/Kevin.EntityFrameworkCore/Database/KevinDbContext.cs
    - App/WebApi/web.config
    - App/WebApi/Dockerfile
    - vue/kevin.web.vue/.env
    - vue/kevin.web.vue/.env.development
    - vue/kevin.web.vue/.env.pre
---

## 1. 使用的系统与框架

本仓库采用 .NET 9 / ASP.NET Core 内置的 `Microsoft.Extensions.Configuration` 体系作为统一配置源，配合 kevin 自研模块进行环境切换、日志与功能开关管理。前端 Vue 工程使用 Vite/Vue CLI 约定的 `.env*` 文件进行构建期环境变量注入。

## 2. 关键文件与位置

- **后端入口与启动**：`App/WebApi/Program.cs` —— 调用 `EnvironmentConfigHelper.SetEnvironment(...)` 设置运行环境，再调用 `builder.Services.ConfigServies(builder.Configuration)` 注册服务，最后通过 `app.UseKevin(builder.Configuration)` 挂载 kevin 中间件。
- **应用配置（JSON）**：
  - `App/WebApi/appsettings.json`（默认/正式）
  - `App/WebApi/appsettings.Development.json`（开发环境覆盖）
  - `App/WebApi/appsettings.Test.json`（测试环境覆盖）
  这些文件遵循 ASP.NET Core 约定：同名键在 Development/Test 文件中会覆盖基础文件中的值。
- **连接字符串**：集中在 `ConnectionStrings.dbConnection`、`ConnectionStrings.redisConnection`。
- **环境检测与切换**：`Kevin/kevin.Module/Kevin.Common/Helper/EnvironmentConfigHelper.cs` —— 读取 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`，并提供 `Development`、`Test`、`Formal` 常量及字典映射；`SetEnvironment` 会同时写入两个环境变量名以兼容不同宿主。
- **全局静态配置访问器**：`Kevin/kevin.Module/Kevin.Common/Helper/ConfigHelper.cs` —— 提供 `Initialize(IConfiguration)` 将根配置注入静态字段，并通过 `GetSection<T>(key)`、`GetValue(key)` 供非 DI 场景读取。
- **日志配置（log4net）**：`App/WebApi/LogConfigs/log4.config`、`log4.Development.config`、`log4.Test.config`；由 `Kevin/kevin.Module/kevin.log4Net/LoggingBuilderExtensions.cs` 的 `UseKevinLog4Net()` 根据当前环境自动选择对应配置文件路径（`LogConfigs/log4.{环境}.config`），找不到时记录警告并跳过。
- **各模块配置绑定**：
  - JWT：`Kevin/kevin.Module/Kevin.Authentication.Jwt/ServiceCollectionExtensions.cs` 读取 `Jwt` 节（Key/Issuer/Audience/过期时间）。
  - 权限开关：`Kevin/kevin.Module/kevin.Permission/ServiceCollectionExtensions.cs` 读取 `IsOpenPermission` 决定是否启用授权策略。
  - RabbitMQ：`Kevin/kevin.Module/kevin.RabbitMQ/RabbitMQOptions.cs` 定义 `HostName/Port/UserName/Password/VirtualHost` 等选项类，对应 `RabbitMQ` 配置节。
  - CORS、Consul、Hangfire、SignalR、云存储（TencentCloud/AliCloud/Qiniu）、Email、SnowflakeId、CodeGenerator、Ollama/DingDing/AliRerank/AliyunAsr 等均在 `appsettings*.json` 中以独立节形式声明。
- **EF Core 配置**：`Kevin/kevin.Module/Kevin.EntityFrameworkCore/Database/KevinDbContext.cs` 通过 `IConfiguration.GetSection("DBDefaultHasIndexFields")` 读取数据库默认索引字段列表。
- **IIS 部署**：`App/WebApi/web.config` 移除 WebDAV 并将请求体上限设为 4GB。
- **容器化**：`App/WebApi/Dockerfile` 通过 `ENV ASPNETCORE_URLS=http://+:8080` 暴露端口，镜像内依赖外部 `appsettings*.json` 或环境变量注入。
- **前端配置**：`vue/kevin.web.vue/.env`（默认）、`.env.development`、`.env.pre` 通过 `VUE_APP_*` 前缀变量注入构建产物，如 `VUE_APP_API_BASE_URL`。

## 3. 架构与约定

### 3.1 配置加载顺序（ASP.NET Core 默认行为）
1. `appsettings.json`（基础配置）
2. `appsettings.{Environment}.json`（按 `ASPNETCORE_ENVIRONMENT` 覆盖）
3. 环境变量（键名用 `:` 分隔，如 `Jwt:Key`）
4. 命令行参数
5. 其他已注册的 `IConfigurationProvider`

本项目在 `Program.Main` 开头显式调用 `EnvironmentConfigHelper.SetEnvironment(EnvironmentConfigHelper.GetEnvironment())`，确保后续所有组件（包括 log4net）都能读到一致的环境标识。

### 3.2 模块化配置注册模式
每个 kevin 子模块（JWT、CORS、Hangfire、RabbitMQ、SignalR、Consul、FileStorage、SMS、RAG 等）都提供 `ServiceCollectionExtensions` 扩展方法，接收 `IServiceCollection` 与 `IConfiguration`，从 `Configuration.GetSection("XxxSetting")` 或 `Configuration["Xxx"]` 读取配置后完成服务注册。业务应用只需在 `Program.cs` 中调用对应的 `AddKevinXxx(Configuration)` 即可。

### 3.3 日志与环境联动
`UseKevinLog4Net()` 内部根据 `EnvironmentConfigHelper.GetEnvironment()` 动态拼接 log4net 配置文件名：`LogConfigs/log4.{环境}.config`。若文件不存在则输出警告并跳过初始化，保证容器/生产环境中即使缺少该文件也不会崩溃。

### 3.4 功能开关
`IsOpenPermission`（布尔）控制是否启用权限校验；`MigrationsAssembly` 指定 EF 迁移程序集，可在不同环境的 appsettings 中覆盖。

## 4. 约定与约束

- **环境命名**：仅支持 `Development`、`Test`、空串（代表正式/Formal）三种环境，见 `EnvironmentConfigHelper.EnvironmentDictionary`。新增环境需同时添加常量与字典项。
- **配置文件命名**：应用层必须提供 `appsettings.json` 以及对应环境的 `appsettings.{环境}.json` 副本，键结构保持一致，仅覆盖差异值。
- **连接字符串**：统一放在 `ConnectionStrings` 节下，通过 `builder.Configuration.GetConnectionString("dbConnection")` / `redisConnection` 获取。
- **敏感信息**：JWT Key、云厂商 SecretId/SecretKey、短信密钥、邮箱密码等均以明文存放在 `appsettings*.json` 中；仓库未集成 Azure Key Vault / AWS Secrets Manager 等外部机密管理，部署时应通过环境变量或运行时替换机制覆盖。
- **日志配置**：log4net 配置文件位于 `App/WebApi/LogConfigs/`，文件名遵循 `log4.{环境}.config` 约定；生产环境应提供 `log4.config`（无后缀）。
- **前端环境变量**：必须以 `VUE_APP_` 为前缀才能被打包进浏览器产物；API 地址通过 `VUE_APP_API_BASE_URL` 注入。
- **容器环境变量**：Dockerfile 固定 `ASPNETCORE_URLS=http://+:8080`，实际监听端口由容器编排决定；可通过 `ASPNETCORE_ENVIRONMENT` 切换 JSON 配置。
- **静态配置访问限制**：`ConfigHelper` 是静态单例，必须在应用启动早期调用 `ConfigHelper.Initialize(builder.Configuration)` 后才能使用，否则抛出 `InvalidOperationException`。
- **权限开关**：`IsOpenPermission` 为 false 时将完全跳过 `AddAuthorization` 注册，相当于关闭鉴权，适合本地调试。

## 5. 总结

该项目采用“ASP.NET Core 原生 Configuration 体系 + 多份 appsettings.*.json 按环境覆盖 + kevin 模块化的 ServiceCollection 扩展”的组合方式组织配置。核心思想是把每个基础设施（JWT、缓存、消息队列、对象存储、AI 模型、邮件、短信、分布式锁等）拆成独立模块，各自从 `Configuration` 读取自己的配置节并完成服务注册；应用层 `Program.cs` 只做编排。日志子系统通过自定义扩展根据环境变量选择 log4net 配置文件，实现与运行时环境解耦。