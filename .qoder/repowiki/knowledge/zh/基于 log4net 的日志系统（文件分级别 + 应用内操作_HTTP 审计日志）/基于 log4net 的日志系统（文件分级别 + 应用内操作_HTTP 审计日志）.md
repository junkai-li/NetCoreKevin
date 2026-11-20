---
kind: logging_system
name: 基于 log4net 的日志系统（文件分级别 + 应用内操作/HTTP 审计日志）
category: logging_system
scope:
    - '**'
source_files:
    - Kevin/kevin.Module/Kevin.log4Net/LoggingBuilderExtensions.cs
    - Kevin/kevin.Module/Kevin.log4Net/EnvironmentConfigHelper.cs
    - Kevin/kevin.Module/Kevin.log4Net/LogHelper.cs
    - App/WebApi/LogConfigs/log4.config
    - App/WebApi/LogConfigs/log4.Development.config
    - App/WebApi/LogConfigs/log4.Test.config
    - App/WebApi/Program.cs
    - Kevin/Application/Services/HttpLogService.cs
    - Kevin/Application/Services/OSLogService.cs
    - Kevin/Domain/Entities/TLog.cs
    - Kevin/Domain/Entities/TOSLog.cs
---

## 1. 使用的框架与整体方案

- **运行时日志**：使用 `log4net`，通过自定义扩展 `LoggingBuilderExtensions.UseKevinLog4Net()` 接入 ASP.NET Core 的 `Microsoft.Extensions.Logging` 管道。入口在 `App/WebApi/Program.cs` 第 42 行：`builder.Logging.UseKevinLog4Net();`。
- **配置加载**：`LoggingBuilderExtensions` 根据环境变量 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT`（由 `EnvironmentConfigHelper.GetEnvironment()` 读取）动态选择配置文件路径 `LogConfigs/log4.{env}.config`，默认回退到 `LogConfigs/log4.config`。未找到文件时仅 Warn 并跳过初始化。
- **业务审计日志**：除运行期日志外，项目还维护数据库层面的两类审计实体 `TLog`（通用日志，含 Sign/Type/Content，构造时触发 `TLogCreatedEvent`）和 `TOSLog`（操作标记，含 Table/TableId/Sign/Content/ActionUserId/IpAddress/DeviceMark 等），分别对应 `HttpLogService` 与 `OSLogService`，提供分页查询 API。

## 2. 关键文件与包

- `Kevin/kevin.Module/Kevin.log4Net/LoggingBuilderExtensions.cs` — 注册 log4net 的扩展方法，解析配置文件路径并调用 `AddLog4Net`。
- `Kevin/kevin.Module/Kevin.log4Net/EnvironmentConfigHelper.cs` — 从环境变量读取当前环境名。
- `Kevin/kevin.Module/Kevin.log4Net/LogHelper.cs` — 封装 `log4net.ILog` 获取；同时提供按名称动态创建独立 Repository 的 `GetLog(string)`，为每个 `logName+日期` 生成独立的 `RollingFileAppender`，输出到 `Log/{date}/{logName}.log`，单文件最大 5MB、最多保留 100 个备份，格式包含时间、线程ID、级别、位置、消息。
- `App/WebApi/LogConfigs/log4.config`（以及 `log4.Development.config`、`log4.Test.config`）— 定义 Debug/Info/Warn/Error 四个 `RollingFileAppender`，按日期切分（`yyyy-MM-dd/'Level.log'`），单文件大小 1MB，最多 100 个备份，`MinimalLock` 允许多进程写入；root logger 级别设为 `DEBUG`，并对 `NHibernate` 单独降级到 `WARN`。
- `App/WebApi/Program.cs` — 启动时调用 `UseKevinLog4Net()`，并在顶层 `try/catch` 中用 `LogHelper<Program>.logger.Error(ex.Message, ex)` 记录异常。
- `Kevin/Application/Services/HttpLogService.cs`、`Kevin/Application/Services/OSLogService.cs` — 暴露 HTTP 操作日志与 OS 变更日志的分页查询接口。
- `Kevin/Domain/Entities/TLog.cs`、`TOSLog.cs` — 持久化日志实体。
- `Kevin/Domain/Events/TLogCreatedEvent.cs` 及 `TLogCreatedEventHandlers.cs` — 通过领域事件异步落库 TLog。

## 3. 架构与约定

- **双通道日志**：
  - 运行期诊断日志 → log4net 文件输出，按级别拆分为 `Debug.log` / `Info.log` / `Warn.log` / `Error.log`，目录结构 `Log/yyyy-MM-dd/Level.log`。
  - 业务审计日志 → 写入数据库表 `TLog`、`TOSLog`，并通过 Application Service 暴露分页查询 API。
- **环境隔离**：log4net 配置文件按环境拆分（Development/Test/默认），通过 `EnvironmentConfigHelper` 自动选择。
- **动态日志仓库**：`LogHelper.GetLog(logName)` 会为每个业务模块创建独立的 log4net `Repository`，避免不同模块互相覆盖配置，并按天滚动输出到 `Log/{date}/{logName}.log`。
- **领域事件驱动写日志**：`TLog` 构造函数主动抛出 `TLogCreatedEvent`，由领域事件处理器完成持久化，将“产生日志”与“存储日志”解耦。
- **Web 层集成点**：`Program.Main` 中统一注入 log4net，全局异常捕获也走同一 logger。

## 4. 约定与约束

- **日志级别策略**：根 logger 级别为 `DEBUG`，各 appender 通过 `LevelRangeFilter` 严格限定只接收对应级别（Debug/Info/Warn/Error 各自一个 appender），因此实际输出是“每级别一个文件”，而非单一文件内混排。
- **文件轮转规则**：所有 appender 统一采用 `rollingStyle=Date` + `maxSizeRollBackups=100` + `maximumFileSize=1MB`（`LogHelper` 中动态 appender 为 5MB），文件名按 `yyyy-MM-dd/Level.log` 命名。
- **并发安全**：appender 使用 `MinimalLock` 模式以支持多进程写入同一文件。
- **布局格式**：PatternLayout 固定输出 `【日志时间】%d 【线程ID】%thread 【日志对象】%logger 【日志内容】%m`，便于人工阅读。
- **第三方日志降噪**：`NHibernate` logger 被显式设置为 `WARN`，抑制其 DEBUG/INFO 输出。
- **配置缺失容错**：当 `log4.{env}.config` 不存在时，扩展方法仅 Warn 并返回，不抛异常，保证服务仍可启动。
- **结构化字段**：运行期日志为纯文本 PatternLayout，无结构化 JSON 字段；业务审计日志通过 `TLog.Sign`/`TLog.Type`/`TLog.Content` 以及 `TOSLog.Table`/`TableId`/`Sign`/`Content`/`ActionUserId`/`IpAddress`/`DeviceMark` 等字段承载上下文信息。