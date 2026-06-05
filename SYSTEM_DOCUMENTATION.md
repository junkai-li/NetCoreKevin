# NetCoreKevin 系统开发-运维-使用文档

---

## 一、项目概述

### 1.1 项目简介

**NetCoreKevin** 是基于 .NET 9 构建的 AI中台智能体 SaaS 企业级前后端分离架构，集成了 AI 知识库智能体、分布式系统、微服务架构等核心能力。

### 1.2 核心功能模块

| 模块 | 功能描述 | 技术实现 |
|------|----------|----------|
| **AI 智能体** | 基于 AgentFramework 的智能代理系统，支持多步推理与任务自动化 | SemanticKernel、MCP 协议 |
| **知识库系统** | 使用 Qdrant 向量数据库实现 RAG 检索增强 | Qdrant、Ollama |
| **任务调度** | 基于 Hangfire 的定时任务调度系统 | Hangfire + Redis |
| **权限管理** | 基于 IdentityServer4 的用户认证与细粒度授权 | IDS4、JWT |
| **消息队列** | 基于 CAP 的分布式事件通信 | CAP、RabbitMQ |
| **文件存储** | 支持多种云存储服务的文件上传下载 | 腾讯云 COS、阿里云 OSS、七牛云 |
| **代码生成器** | 自动化代码生成工具 | 模板引擎 |

---

## 二、技术架构

### 2.1 技术栈

| 分类 | 技术 | 版本 | 说明 |
|------|------|------|------|
| 语言 | C# | 9.0 | .NET 9 LTS |
| 框架 | ASP.NET Core | 9.0 | Web API 框架 |
| ORM | Entity Framework Core | 9.0 | 数据访问层 |
| 数据库 | MySQL | 8.0+ | 主数据库 |
| 缓存 | Redis | 7.0+ | 分布式缓存 |
| 向量数据库 | Qdrant | 1.7+ | AI 知识库 |
| 任务调度 | Hangfire | 1.8+ | 定时任务 |
| 消息总线 | CAP | 7.0+ | 分布式事件 |
| 认证授权 | IdentityServer4 | 6.3+ | OAuth2.0/OIDC |
| AI 框架 | SemanticKernel | 1.0+ | AI 智能体 |

### 2.2 架构设计

```
┌─────────────────────────────────────────────────────────────────┐
│                      前端层 (Vue3)                              │
├─────────────────────────────────────────────────────────────────┤
│                      API 网关层                                  │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────────┐ │
│  │  App.WebApi │ │  AI 模块    │ │  任务调度   │ │  其他服务 │ │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └─────┬─────┘ │
├─────────┼────────────────┼────────────────┼────────────────┼─────┤
│  ┌──────▼──────┐ ┌──────▼──────┐ ┌──────▼──────┐               │
│  │  应用服务层 │ │  领域服务层 │ │  仓储层     │               │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘               │
├─────────┼────────────────┼────────────────┼─────────────────────┤
│                      数据访问层                                  │
├─────────┼────────────────┼────────────────┼─────────────────────┤
│  MySQL  │    Redis      │    Qdrant     │    RabbitMQ         │
└─────────────────────────────────────────────────────────────────┘
```

### 2.3 项目结构

```
kevin.abp.core/
├── App/                            # 应用模块
│   ├── AppShare/                   # 共享组件
│   ├── Application/                # 应用服务层
│   ├── Domain/                     # 领域层
│   ├── RepositorieRps/             # 仓储实现
│   └── WebApi/                     # API 入口
├── Kevin/                          # 核心模块
│   ├── Application/                # 核心应用服务
│   │   └── Services/
│   │       ├── AI/                 # AI 服务
│   │       └── ...                 # 其他服务
│   ├── Domain/                     # 核心领域模型
│   │   ├── Entities/               # 实体定义
│   │   ├── Interfaces/             # 接口定义
│   │   └── Bases/                  # 基类
│   ├── Kevin.EntityFrameworkCore/  # EF Core 实现
│   ├── Kevin.Web.Basics/           # Web 基础组件
│   └── kevin.CodeGenerator/        # 代码生成器
├── Doc/                            # 文档资源
└── InitData/                       # 初始化数据
```

---

## 三、环境要求

### 3.1 必备依赖

| 依赖 | 版本要求 | 说明 |
|------|----------|------|
| .NET SDK | 9.0+ | 开发环境 |
| MySQL | 8.0+ | 数据库 |
| Redis | 7.0+ | 缓存、Hangfire |
| Qdrant | 1.7+ | 向量数据库（AI 功能） |
| Node.js | 18+ | 前端开发（可选） |

### 3.2 可选依赖

| 依赖 | 说明 |
|------|------|
| RabbitMQ | CAP 消息队列 |
| Consul | 服务发现 |
| Ollama | 本地大语言模型 |

---

## 四、安装配置

### 4.1 数据库配置

**appsettings.json** 中的连接字符串配置：

```json
{
  "ConnectionStrings": {
    "dbConnection": "server=127.0.0.1;port=3306;database=kevin_app;user id=root;password=admin123;Convert Zero Datetime=True;TreatTinyAsBoolean=false;AllowLoadLocalInfile=true;Charset=utf8;Command Timeout=120;",
    "redisConnection": "127.0.0.1:6379,DefaultDatabase=0,password=123456"
  }
}
```

### 4.2 数据库迁移

在 **Package Manager Console** 中执行：

```powershell
# 1. 选择默认项目为 Kevin.EntityFrameworkCore
# 2. 创建迁移
Add-Migration "初始化数据库"

# 3. 应用迁移
Update-Database
```

### 4.3 环境变量配置

**Windows PowerShell：**
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
```

**Linux/macOS：**
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

---

## 五、启动运行

### 5.1 开发环境启动

**方式一：使用 Visual Studio**
1. 打开解决方案 `kevin.abp.core.sln`
2. 设置 `App.WebApi` 为启动项目
3. 按 F5 启动调试

**方式二：使用命令行**
```bash
cd App/WebApi
dotnet run --environment Development
```

### 5.2 服务访问

| 服务 | 地址 |
|------|------|
| API 接口 | http://localhost:9901 |
| Swagger 文档 | http://localhost:9901/swagger |
| Hangfire 面板 | http://localhost:9901/pchangfire |

### 5.3 默认账户

| 账户 | 密码 | 租户 |
|------|------|------|
| admin | 123456 | 1000 |

---

## 六、AI 智能体配置

### 6.1 Qdrant 配置

```json
{
  "QdrantClientSetting": {
    "Url": "localhost"
  }
}
```

### 6.2 模型配置

**智谱 AI 配置：**
```json
{
  "OllamaApiSetting": {
    "Url": "https://open.bigmodel.cn/api/paas/v4/embeddings",
    "DefaultModel": "embedding-3",
    "ApiKey": "your-api-key"
  }
}
```

**Ollama 本地模型配置：**
```json
{
  "OllamaApiSetting": {
    "Url": "http://localhost:11434/api/embeddings",
    "DefaultModel": "qwen3:4b",
    "ApiKey": ""
  }
}
```

### 6.3 AI 智能体使用流程

1. **注册 AI 账户** → 获取 API Key
2. **配置模型** → 设置向量模型和对话模型
3. **新建知识库** → 上传文档，选择向量模型
4. **配置智能体** → 绑定技能工具
5. **开始对话** → 与 AI 智能体交互

---

## 七、API 接口说明

### 7.1 认证接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/Authorize/Login` | POST | 用户登录 |
| `/api/Authorize/Logout` | POST | 用户登出 |
| `/api/Authorize/RefreshToken` | POST | 刷新 Token |

**登录请求示例：**
```json
{
  "userName": "admin",
  "password": "123456",
  "tenantId": 1000
}
```

### 7.2 AI 接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/AIChats/Chat` | POST | 发送聊天消息 |
| `/api/AIKmss/Create` | POST | 创建知识库 |
| `/api/AIKmss/UploadFile` | POST | 上传知识库文档 |
| `/api/AIModels/GetAll` | GET | 获取模型列表 |

### 7.3 任务调度接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/AITasks/AddOrUpdateCronTask` | POST | 创建/更新定时任务 |
| `/api/AITasks/RemoveCronTask` | POST | 删除定时任务 |
| `/api/AITasks/TriggerCronTask` | POST | 立即触发任务 |

### 7.4 代码生成器接口

| 接口 | 方法 | 说明 |
|------|------|------|
| `/api/CodeGenerator/GetAreaNames` | GET | 获取区域列表 |
| `/api/CodeGenerator/GetAreaNameEntityItems` | GET | 获取实体列表 |
| `/api/CodeGenerator/BulidCode` | POST | 生成代码 |

---

## 八、开发规范

### 8.1 代码风格

- 使用 **PascalCase** 命名类、接口、方法
- 使用 **camelCase** 命名参数、局部变量
- 文件编码使用 **UTF-8 无 BOM**
- 每行代码不超过 120 个字符

### 8.2 目录结构规范

```
Application/Services/
├── [ModuleName]/
│   ├── [ServiceName]Service.cs      # 服务实现
│   └── Dto/
│       ├── [EntityName]Dto.cs       # 数据传输对象
│       └── [EntityName]Input.cs     # 输入参数

Domain/
├── Entities/
│   └── [EntityName].cs              # 实体定义
├── Interfaces/
│   ├── IRepositories/
│   │   └── I[EntityName]Rp.cs       # 仓储接口
│   └── IServices/
│       └── I[ServiceName]Service.cs # 服务接口
```

### 8.3 异常处理

- 使用统一的异常处理中间件
- 捕获异常时记录日志（使用 log4net）
- API 返回统一格式的错误响应

### 8.4 日志规范

```csharp
// 使用 log4net 记录日志
LogHelper<MyService>.logger.Info("信息级别日志");
LogHelper<MyService>.logger.Warn("警告级别日志");
LogHelper<MyService>.logger.Error("错误级别日志", exception);
```

### 8.5 代码生成器使用

#### 8.5.1 功能概述

代码生成器可以根据数据库实体自动生成：
- 仓储接口（IRepository）
- 仓储实现（Repository）
- 服务接口（IService）
- 服务实现（Service）

#### 8.5.2 配置说明

编辑 `appsettings.json`：
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

#### 8.5.3 使用步骤

1. **获取区域列表**
```bash
GET /api/CodeGenerator/GetAreaNames
```

2. **获取实体列表**
```bash
GET /api/CodeGenerator/GetAreaNameEntityItems?name=App.WebApi.v1
```

3. **生成代码**
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

#### 8.5.4 注意事项

- 仅超级管理员有权限使用代码生成器
- 生成的代码会覆盖现有文件，请谨慎操作
- 建议在生成前备份相关文件

### 8.6 自定义AI工具开发

#### 8.6.1 开发步骤

1. **创建工具接口**

```csharp
public interface IMyCustomToolService
{
    [Description("自定义工具描述")]
    Task<string> MyToolMethod(
        [Description("参数1说明")] string param1,
        [Description("参数2说明")] int param2
    );
}
```

2. **实现工具服务**

```csharp
public class MyCustomToolService : IMyCustomToolService
{
    public async Task<string> MyToolMethod(string param1, int param2)
    {
        // 实现逻辑
        return "执行结果";
    }
}
```

3. **注册工具到容器**

在模块初始化中注册服务：
```csharp
services.AddTransient<IMyCustomToolService, MyCustomToolService>();
```

4. **配置工具到智能体**

在 `AIAgentToolSkillService.cs` 中添加工具注册：
```csharp
aiTools.Add(
    AIFunctionFactory.Create(_myCustomToolService.MyToolMethod,
    new AIFunctionFactoryOptions
    {
        Name = "MyToolMethod",
        Description = "自定义工具描述"
    }
));
```

#### 8.6.2 工具开发规范

- 使用 `[Description]` 属性添加参数说明
- 返回类型推荐使用 `Task<string>`
- 错误返回以 `❌` 开头的字符串
- 支持的参数类型：string, int, bool, List<string>

---

## 九、运维部署

### 9.1 Docker 部署

**Dockerfile 示例：**
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

**Docker Compose 示例：**
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

### 9.2 Linux 系统服务部署

**创建 systemd 服务文件 `/etc/systemd/system/kevin-webapi.service`：**
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

**启动服务：**
```bash
sudo systemctl daemon-reload
sudo systemctl start kevin-webapi
sudo systemctl enable kevin-webapi
```

---

## 十、监控运维

### 10.1 日志管理

日志文件位于 `App/WebApi/Logs/` 目录：

| 日志文件 | 说明 |
|----------|------|
| `log_*.txt` | 应用日志 |
| `error_*.txt` | 错误日志 |
| `http_*.txt` | HTTP 请求日志 |

### 10.2 性能监控

- **Hangfire Dashboard**: http://localhost:9901/hangfire
- **Redis 监控**: 使用 Redis Insight 或类似工具
- **MySQL 监控**: 使用 Prometheus + Grafana

### 10.3 常见问题

| 问题 | 解决方案 |
|------|----------|
| **数据库连接失败** | 检查 MySQL 服务是否启动，连接字符串配置是否正确 |
| **Redis 连接失败** | 检查 Redis 服务是否启动，密码配置是否正确 |
| **Qdrant 连接失败** | 检查 Qdrant 服务是否启动，配置的 URL 是否正确 |
| **AI 工具调用失败** | 检查工具注册配置，确保 `InitData` 正确传递参数 |
| **Hangfire 任务不执行** | 检查 Redis 连接，确保服务已启动 |

### 10.4 故障排查指南

#### 10.4.1 启动失败

**症状：** 应用无法启动

**排查步骤：**

1. 检查端口是否被占用
```bash
netstat -ano | findstr :9901
```

2. 检查数据库连接
```bash
# 测试 MySQL 连接
mysql -h 127.0.0.1 -P 3306 -u root -p
```

3. 检查 Redis 连接
```bash
redis-cli -h 127.0.0.1 -p 6379 -a 123456 ping
```

#### 10.4.2 AI工具调用失败

**症状：** Error: Function failed

**排查步骤：**

1. 检查工具注册配置
2. 检查 `InitData` 参数传递
3. 查看日志文件 `Logs/log_*.txt`
4. 检查工具方法签名是否正确

#### 10.4.3 知识库检索失败

**症状：** 问答返回空或错误

**排查步骤：**

1. 检查 Qdrant 服务状态
```bash
curl http://localhost:6333/health
```

2. 检查知识库文档是否已上传
3. 检查向量模型配置

---

## 十一、安全规范

### 11.1 输入验证

- 所有用户输入必须进行验证
- 使用数据注解或 FluentValidation
- 防止 SQL 注入、XSS 攻击

### 11.2 权限控制

- 使用基于角色的访问控制（RBAC）
- 敏感接口需要权限验证
- 日志记录所有权限检查失败的请求

### 11.3 数据加密

- 数据库密码使用 AES 加密存储
- API 密钥等敏感配置使用环境变量
- 传输层使用 HTTPS

### 11.4 安全防护

#### 11.4.1 危险命令拦截

系统会拦截以下危险命令：
- `rm -rf`
- `del /s /q`
- 格式化磁盘命令
- 其他破坏性操作

#### 11.4.2 域名白名单

HTTP/HTTPS 请求需要配置授权域名白名单：

```json
{
  "AuthorizedDomains": [
    "example.com",
    "api.example.com"
  ]
}
```

---

## 十二、性能优化

### 12.1 数据库优化

| 优化项 | 说明 |
|--------|------|
| 索引优化 | 为常用查询字段创建索引 |
| 分页查询 | 使用 Skip/Take 避免全表扫描 |
| 批量操作 | 使用 EF Core 的批量操作 API |
| 读写分离 | 配置数据库读写分离 |

### 12.2 缓存策略

```csharp
// 使用 Redis 缓存
[CacheDataFilter(Duration = 60)] // 缓存60秒
public async Task<MyDto> GetData(int id)
{
    // 查询逻辑
}
```

### 12.3 异步编程

- 使用 `async/await` 模式
- 避免同步阻塞调用
- 使用 `ConfigureAwait(false)` 优化性能

### 12.4 日志优化

- 生产环境关闭 Debug 级别日志
- 使用异步日志写入
- 定期清理日志文件

---

## 十三、附录

### 13.1 配置项说明

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| `IsOpenPermission` | 是否开启权限验证 | true |
| `TenantId` | 默认租户 ID | 1000 |
| `Jwt:AccessTokenExpirationMinutes` | Token 过期时间（分钟） | 60 |
| `HangfireSetting` | Hangfire 配置 | - |
| `CorsSetting` | CORS 跨域配置 | - |
| `CodeGeneratorSetting` | 代码生成器配置 | - |

### 13.2 数据库字段索引

```json
{
  "DBDefaultHasIndexFields": "tableid,createtime,updatetime,deletetime,tenantid,createuserid,updateuserid,deleteuserid"
}
```

### 13.3 联系方式

- 项目地址：https://gitee.com/netkevin-li/ainet
- 教学文档：https://blog.csdn.net/weixin_42629287/category_13037923.html
- 交流群：文档中包含微信群二维码

---

**版本**: v1.0  
**最后更新**: 2026年6月  
**维护团队**: NetCoreKevin 开发团队