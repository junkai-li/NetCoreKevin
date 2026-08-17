---
kind: logging_system
name: 基于 log4net 的分环境文件滚动日志系统
category: logging_system
scope:
    - '**'
source_files:
    - Kevin/Kevin.Module/kevin.log4Net/LoggingBuilderExtensions.cs
    - Kevin/Kevin.Module/kevin.log4Net/LogHelper.cs
    - Kevin/Kevin.Module/kevin.log4Net/EnvironmentConfigHelper.cs
    - App/WebApi/LogConfigs/log4.config
    - App/WebApi/LogConfigs/log4.Development.config
    - App/WebApi/LogConfigs/log4.Test.config
    - App/WebApi/Program.cs
---

## 1. 使用的系统与框架

项目采用 **log4net** 作为核心日志框架，并通过自定义扩展 `Kevin.log4Net` 模块将其集成到 .NET 9 WebApi 的 `Microsoft.Extensions.Logging` 管道中。启动时在 `Program.Main` 中调用 `builder.Logging.UseKevinLog4Net()` 完成初始化。

## 2. 关键文件与包

- `Kevin/Kevin.Module/kevin.log4Net/LoggingBuilderExtensions.cs`：提供 `UseKevinLog4Net()` 扩展方法，自动根据 `NETCORE_ENVIRONMENT` / `ASPNETCORE_ENVIRONMENT` 环境变量选择配置文件（如 `log4.Development.config`、`log4.Test.config`、`log4.config`）。
- `Kevin/Kevin.Module/kevin.log4Net/EnvironmentConfigHelper.cs`：读取运行环境名称，用于动态定位配置。
- `Kevin/Kevin.Module/kevin.log4Net/LogHelper.cs`：提供两种使用方式——
  - `LogHelper.GetLog(Type)` / `LogHelper.logger`：通过 `LogManager.GetLogger` 获取全局 ILog。
  - `LogHelper<T>.logger`：泛型静态类，按类型名创建独立 logger，被各业务层广泛使用。
  - `LogHelper.GetLog(string logName)`：运行时按日期+名称创建独立 repository + RollingFileAppender，输出到 `Log/{date}/{logName}.log`，用于按业务维度隔离日志。
- `App/WebApi/LogConfigs/log4.config`、`log4.Development.config`、`log4.Test.config`：log4net XML 配置，定义四个 appender：`RollingFileDebug`、`RollingFileInfo`、`RollingFileWarn`、`RollingFileError`，分别写入 `Log/Debug.log`、`Log/Info.log`、`Log/Warn.log`、`Log/Error.log`。
- `App/WebApi/Program.cs`：入口调用 `UseKevinLog4Net()`，并在 `Main` 的 catch 块中通过 `LogHelper<Program>.logger.Error` 记录启动异常。

## 3. 架构与约定

- **分环境配置**：`UseKevinLog4Net` 默认加载 `LogConfigs/log4.{环境}.config`，若不存在则回退到 `LogConfigs/log4.config`；当找不到配置文件时仅输出 Console 警告并跳过注册，不会抛错。
- **按级别拆分文件**：每个日志级别对应一个独立的 `RollingFileAppender`，通过 `LevelRangeFilter` 严格限定只接收该级别消息（例如 Debug appender 的 LevelMin=Debug、LevelMax=Debug），最终在根 `<root>` 中同时引用四个 appender。
- **滚动策略**：每个 appender 同时启用 Size 和 Date 滚动（`rollingStyle value="Size"` 与 `datePattern` 并存），最大保留 100 个备份文件，单文件上限 1MB（通用 appender）或 5MB（LogHelper 动态创建的 appender）。
- **多进程安全**：所有文件 appender 均设置 `lockingModel type="log4net.Appender.FileAppender+MinimalLock"`，允许多进程并发写入同一目录。
- **结构化字段**：日志格式由 `PatternLayout` 的 `conversionPattern` 控制，统一包含时间、线程ID、日志级别、logger 名称、消息内容，并以分隔线结尾。
- **第三方库降噪**：根配置中对 `NHibernate` 单独设置 level 为 WARN，抑制其 DEBUG/INFO 噪音。
- **双通道使用**：业务代码普遍通过 `Kevin.log4Net.LogHelper<T>.logger`（如 `AIChatsService`、`CacheDataFilter`、`GlobalError`、`RetryTools` 等）直接调用 `Error(...)`；同时 `Program.cs` 也展示了在异常捕获中直接使用的方式。

## 4. 约定与约束

- **日志输出位置**：默认相对路径 `Log/`，实际落盘于应用运行目录下的 `Log/` 文件夹；按天/级别生成 `yyyy-MM-dd/Debug.log` 等文件。
- **环境切换方式**：通过设置 `NETCORE_ENVIRONMENT` 或 `ASPNETCORE_ENVIRONMENT` 环境变量切换 `log4.{env}.config`，无需改代码。
- **新增日志级别需同步修改配置**：当前仅支持 Debug/Info/Warn/Error 四级，新增级别需在 XML 中新增对应的 appender 并在 root 中引用。
- **ElasticSearch 输出已预留但未启用**：配置文件中注释了 `ElasticSearchAppender` 片段，表明未来可扩展至 ES 收集，但当前未接入。
- **运行时动态日志**：`LogHelper.GetLog(logName)` 会为每次调用创建独立 repository，适合临时诊断场景，但不建议在生产高频调用，因为会重复创建 Repository 和 Appender。
- **异常兜底**：`Program.Main` 的顶层 try/catch 以及 `GlobalError` 统一通过 `LogHelper` 记录异常，确保启动失败也能落地日志。