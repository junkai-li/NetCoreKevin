---
kind: error_handling
name: 基于 UserFriendlyException 与 FieldValidationException 的统一异常处理体系
category: error_handling
scope:
    - '**'
source_files:
    - Kevin/kevin.Module/Kevin.Common/App/Global/GlobalError.cs
    - Kevin/kevin.Share/FieldValidationException.cs
    - Kevin/Kevin.Web.Basics/Filters/ResultFilter.cs
    - App/WebApi/Program.cs
---

## 1. 采用的系统/方案

该项目在 .NET 9 WebApi 上采用 **自定义业务异常 + ASP.NET Core 全局异常中间件** 的方式实现统一错误处理：
- 业务层通过抛出 `UserFriendlyException`（来自 kevin 框架的 `Common.App` 命名空间）表达“可预期、应返回友好提示”的业务错误。
- 数据/参数校验失败时抛出自定义异常 `FieldValidationException`（位于 `Kevin/kevin.Share/FieldValidationException.cs`），用于字段级校验失败。
- 启动阶段通过 `app.UseExceptionHandler(builder => builder.Run(async context => await GlobalError.ErrorEvent(context)))` 注册全局异常处理器，由 `GlobalError.ErrorEvent` 统一捕获、记录日志并写出 JSON 响应。
- 同时使用 MVC 结果过滤器 `ResultFilter`（`Kevin/Kevin.Web.Basics/Filters/ResultFilter.cs`）对正常返回结果进行统一包装，将不同 HTTP 状态码转换为统一的 `{ code, IsSuccess, msg, errMsg }` JSON 结构。

## 2. 关键文件与包

| 文件 | 作用 |
|---|---|
| `Kevin/kevin.Module/Kevin.Common/App/Global/GlobalError.cs` | 全局异常处理器，根据异常类型决定 HTTP 状态码和响应体 |
| `Kevin/kevin.Share/FieldValidationException.cs` | 字段校验失败专用异常，支持消息模板与 InnerException |
| `Kevin/Kevin.Web.Basics/Filters/ResultFilter.cs` | 全局结果过滤器，统一包装成功/400/401/500 响应格式 |
| `App/WebApi/Program.cs` | 应用入口，注册 `UseExceptionHandler` 并在开发环境启用 `DeveloperExceptionPage` |
| `Kevin/kevin.Module/Kevin.log4Net/LogHelper.cs` | 日志记录，被 `GlobalError` 用于记录未预期异常 |

## 3. 架构与约定

### 异常分类与传播路径
1. **业务异常**：Service/Controller 中遇到“数据不存在或已删除”“智能体名称已存在”“权限不足”等可预期错误时，直接 `throw new UserFriendlyException("...")`。该异常会被 `GlobalError.ErrorEvent` 识别为业务异常，映射为 **HTTP 400 + `{ code: 400, IsSuccess: false, errMsg: 异常消息 }`**。
2. **字段校验异常**：DTO/实体验证失败时抛 `FieldValidationException`，同样被 `GlobalError` 识别为 400 业务错误。
3. **其他异常**：任何未被捕获的异常（如 `ArgumentException`、`NotSupportedException`、数据库异常等）均进入 `else` 分支，记录 path、parameter、authorization 及完整堆栈到日志，并返回 **HTTP 500 + `{ code: 500, IsSuccess: false, errMsg: "Global internal exception of the system" }`**。
4. **正常流程**：控制器返回的 `ObjectResult` 经 `ResultFilter.OnResultExecuting` 拦截，按状态码改写为标准 JSON；400/401/500 会覆盖为统一结构，其余默认转为 `{ code: 200, msg: "success", IsSuccess: true, data }`。

### 设计决策
- **不依赖 ASP.NET Core 内置 `ProblemDetails`**，而是手写 JSON 结构，保持前后端契约一致。
- **区分“业务可恢复错误”与“系统内部异常”**：前者用 `UserFriendlyException` / `FieldValidationException` 明确表达，后者走通用 500 路径。
- **开发/生产差异化**：开发环境启用 `DeveloperExceptionPage` 展示详细堆栈；生产环境仅输出通用错误信息并通过 log4net 记录详情。
- **请求上下文收集**：通过 `IHttpContextAccessor` 获取 URL、请求参数、Authorization 头，便于排查问题。

## 4. 约定与约束

- **业务层禁止吞掉异常**：从代码可见 Service 层大量直接 `throw new UserFriendlyException(...)`，未见 try-catch 包裹业务异常后再转译，说明约定是“业务异常向上冒泡至全局处理器”。
- **字段校验集中抛 `FieldValidationException`**：所有 DTO/实体验证失败点（如 `AIAppsDto`、`dtoUser`、`dtoFormItem`）统一使用该异常，而非返回错误集合。
- **HTTP 状态码约定**：400 表示业务/参数错误，401 表示未授权，500 表示系统内部异常，均由 `ResultFilter` 与 `GlobalError` 共同保证。
- **日志必须记录**：`GlobalError.ErrorEvent` 对所有异常调用 `LogHelper<Exception>.logger.Error`，确保未预期异常可追溯。
- **响应体结构固定**：无论成功还是失败，JSON 响应均包含 `code`、`IsSuccess`、`msg`、`errMsg` 四个字段，前端据此统一处理。
- **未使用 `try-finally` 做资源清理**：当前代码未发现 `using` 外的显式 finally 块，资源释放依赖框架与 GC。
- **未使用 `catch (Exception ex) { throw; }` 包装模式**：仅在 `Program.Main` 顶层 catch 一次用于启动期日志记录后重新抛出，业务层不捕获再重抛。

## 5. 缺失/待完善之处

- `GlobalError.ErrorEvent` 中 500 分支构造了 `path/parameter/authorization/error` 对象但未写入响应体（仅序列化到局部变量），实际只写入了通用 500 消息，调试信息未返回客户端。
- 未看到针对 404、403 等状态的专门处理逻辑，可能依赖 MVC 默认行为。
- `UserFriendlyException` 来自外部 kevin 框架，未在仓库源码中定义，其具体属性（如是否携带错误码）无法从本仓库确认。