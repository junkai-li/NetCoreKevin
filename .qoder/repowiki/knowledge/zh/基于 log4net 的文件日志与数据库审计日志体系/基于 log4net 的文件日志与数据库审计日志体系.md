---
kind: logging_system
name: 基于 log4net 的文件日志与数据库审计日志体系
category: logging_system
scope:
    - '**'
source_files:
    - Kevin/kevin.Module/Kevin.log4Net/LoggingBuilderExtensions.cs
    - Kevin/kevin.Module/Kevin.log4Net/LogHelper.cs
    - Kevin/kevin.Module/Kevin.log4Net/EnvironmentConfigHelper.cs
    - App/WebApi/Program.cs
    - App/WebApi/LogConfigs/log4.config
    - App/WebApi/LogConfigs/log4.Development.config
    - App/WebApi/LogConfigs/log4.Test.config
    - Kevin/Domain/Entities/TLog.cs
    - Kevin/Domain/Entities/THttpLog.cs
    - Kevin/Domain/Entities/TOSLog.cs
    - Kevin/Domain/Events/TLogCreatedEvent.cs
    - Kevin/Domain/EventHandlers/TLogCreatedEventHandlers.cs
    - Kevin/RepositorieRps/Repositories/HttpLogRp.cs
    - Kevin/Application/Services/HttpLogService.cs
    - Kevin/Application/Services/OSLogService.cs
    - Kevin/kevin.Share/Attributes/HttpLogAttribute.cs
    - Kevin/kevin.Module/Kevin.Common/Tools/RetryTools.cs
---

## 1. 使用的框架与整体思路

本仓库采用 **log4net** 作为应用运行期日志框架，并通过 `Kevin.log4Net` 模块以扩展方法形式集成到 ASP.NET Core 的 `ILoggingBuilder` 管线中；同时通过领域实体 + 仓储/服务的方式将 **HTTP 请求日志、操作变更日志（OSLog）、通用业务日志（TLog）** 持久化到数据库中，形成“文件日志 + 数据库审计日志”双轨体系。

- 应用启动时调用 `builder.Logging.UseKevinLog4Net()`（见 `App/WebApi/Program.cs`），由 `LoggingBuilderExtensions.UseKevinLog4Net` 根据环境变量自动选择 `LogConfigs/log4.{Environment}.config`，再调用第三方包提供的 `AddLog4Net(configPath)` 完成注册。
- 代码中直接通过 `Kevin.log4Net.LogHelper.logger` 或 `LogHelper.GetLog(name)` 获取 `log4net.ILog` 实例进行 Debug/Info/Warn/Error 输出。
- 业务侧通过 `HttpLogAttribute` + `HttpLogFilter` + `HttpLogRp` + `THttpLog` 实现 HTTP 请求入参/出参/设备/IP 等结构化字段落库；通过 `TLog` 领域事件 + `TLogCreatedEventHandlers` 记录通用业务日志；通过 `TOSLog` 记录数据表级操作变更。

## 2. 核心文件与职责

| 文件 | 作用 |
|---|---|
| `Kevin/kevin.Module/Kevin.log4Net/LoggingBuilderExtensions.cs` | 提供 `UseKevinLog4Net` 扩展，按 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT` 解析 `LogConfigs/log4.{env}.config` 并注入 log4net |
| `Kevin/kevin.Module/Kevin.log4Net/EnvironmentConfigHelper.cs` | 读取环境变量名（优先 `NETCORE_ENVIRONMENT`，回退 `ASPNETCORE_ENVIRONMENT`） |
| `Kevin/kevin.Module/Kevin.log4Net/LogHelper.cs` | 封装 `ILog`：静态 `logger` 用于全局日志；`GetLog(logName)` 动态创建独立 repository 并按 `Log/{date}/{name}.log` 滚动输出 |
| `App/WebApi/LogConfigs/log4.config`、`log4.Development.config`、`log4.Test.config` | log4net 配置：每个级别一个 `RollingFileAppender`，按日期切分，最大 1MB，最多 100 份，使用 `MinimalLock` |
| `App/WebApi/Program.cs` | 在构建阶段调用 `builder.Logging.UseKevinLog4Net()`，并在异常捕获中使用 `LogHelper<Program>.logger.Error` |
| `Kevin/Domain/Entities/TLog.cs` | 通用日志实体（Sign/Type/Content），构造时发布 `TLogCreatedEvent` |
| `Kevin/Domain/Events/TLogCreatedEvent.cs` | MediatR 通知 `TLogCreatedEvent` |
| `Kevin/Domain/EventHandlers/TLogCreatedEventHandlers.cs` | 监听 `TLogCreatedEvent`，当前实现为控制台输出（可扩展为写入 DB） |
| `Kevin/Domain/Entities/THttpLog.cs` | HTTP 请求日志实体：IP、Device、HttpUrl/Action/Method/Body、OperateType/Remark 等 |
| `Kevin/Domain/Entities/TOSLog.cs` | 操作变更日志实体：Table/TableId、Sign、Content、ActionUserId、IpAddress、DeviceMark 等 |
| `Kevin/RepositorieRps/Repositories/HttpLogRp.cs` | 从 `HttpContextAccessor` 抓取 IP、Body、URL、Device、Method 并写入 `THttpLog` |
| `Kevin/Application/Services/HttpLogService.cs`、`OSLogService.cs` | 分页查询 HTTP 日志与 OS 日志，供管理后台展示 |
| `Kevin/kevin.Share/Attributes/HttpLogAttribute.cs` | 标记需要记录 HTTP 日志的 Action |
| `Kevin/kevin.Module/Kevin.Common/Tools/RetryTools.cs`、`Kevin.HttpApiClients/Tools/RetryTools.cs` | 重试逻辑统一通过 `LogHelper.logger.Error` 记录重试次数与异常详情 |

## 3. 架构与约定

### 3.1 运行时文件日志（log4net）
- **初始化位置**：`App/WebApi/Program.cs` → `builder.Logging.UseKevinLog4Net()`。
- **配置文件选择规则**：`LogConfigs/log4.{环境}.config`，环境来自 `EnvironmentConfigHelper.GetEnvironment()`，支持 Development/Test/Release 等多套配置。
- **输出结构**：每个级别独立 Appender（Debug/Info/Warn/Error），文件名形如 `yyyy-MM-dd/Debug.log`、`Info.log`、`Warn.log`、`Error.log`，存放于应用根目录 `Log/`。
- **滚动策略**：`RollingStyle=Date` 且 `MaximumFileSize=1MB`、`MaxSizeRollBackups=100`，使用 `MinimalLock` 允许多进程并发写入。
- **PatternLayout**：包含时间、线程ID、级别、Logger 名称、消息体，并以分隔线收尾。
- **额外能力**：`LogHelper.GetLog(name)` 会为每个 `logName+日期` 动态创建独立 `Repository`，输出到 `Log/{date}/{name}.log`，便于按业务维度拆分文件。

### 3.2 数据库审计日志
- **HTTP 请求日志**：通过 `HttpLogAttribute` 标注 Controller Action，由 `HttpLogFilter`（实现 `IResultFilter`）拦截后调用 `HttpLogRp.Add(operateType, operateRemark)`，从 `HttpContextAccessor` 提取 IP、设备、URL、Method、Body 等写入 `THttpLog`。
- **通用业务日志**：在领域实体构造时附加 `TLogCreatedEvent`，由 MediatR 分发到 `TLogCreatedEventHandlers`，当前实现打印到控制台，可替换为写库。
- **操作变更日志**：`TOSLog` 记录对某张表的某条记录的变动内容、操作人、IP、设备标记等，配合 `OSLogService` 提供分页查询。

### 3.3 日志级别策略
- 文件日志：log4net 配置中 root level 设为 `DEBUG`，各 Appender 通过 `LevelRangeFilter` 严格限定只接收对应级别（Debug 仅 Debug，Info 仅 Info 等）。
- 特定组件：`NHibernate` logger 被单独限制为 `WARN`，降低底层 ORM 噪音。
- 代码层：业务异常统一使用 `LogHelper.logger.Error(...)` 输出，重试工具类在每次失败时记录剩余重试次数和异常 JSON。

## 4. 约定与约束

- **必须通过 `UseKevinLog4Net()` 启用**：`Program.cs` 中显式调用该扩展，若未找到配置文件会 Warn 并跳过注册（见 `LoggingBuilderExtensions` 中的 `File.Exists` 检查）。
- **配置文件命名与环境绑定**：`LogConfigs/log4.{环境}.config`，环境取自 `NETCORE_ENVIRONMENT` 或 `ASPNETCORE_ENVIRONMENT`，部署时需确保对应环境配置文件随应用输出。
- **日志路径约定**：默认输出到应用运行目录下的 `Log/`，子目录按日期划分；`LogHelper.GetLog(name)` 进一步按 `Log/{date}/{name}.log` 拆分。
- **HTTP 日志采集范围**：仅对标注了 `HttpLogAttribute` 的 Action 生效，由 `HttpLogFilter` 在结果阶段写入 `THttpLog`，避免全量请求开销。
- **租户隔离**：`HttpLogRp.Add` 中强制写入 `CurrentUser.TenantId`，保证多租户环境下日志可隔离查询。
- **领域事件解耦**：`TLog` 通过 `TLogCreatedEvent` + `TLogCreatedEventHandlers` 将“记录日志”这一横切行为与领域模型解耦，便于后续替换为异步/写库等实现。
- **ElasticSearch 预留**：`log4.config` 中包含注释掉的 `ElasticSearchAppender` 片段，表明未来可平滑切换到集中式日志平台。