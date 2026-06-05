# NetCoreKevin

> 是基于 .NET 9 构建的 AI中台智能体 SaaS 企业级前后端分离架构，集成了 AI 知识库智能体、分布式系统、微服务架构等核心能力。

---

## 📷 功能效果图

### AI 智能体技能工具管理
| 动态管理 | 智能体配置 | 对话交互 |
|---------|----------|---------|
| ![动态管理](Doc/dtai1.png) | ![智能体配置](Doc/dtai2.png) | ![对话交互](Doc/dtai3.png) |

### AI 知识库系统
| 知识库管理 | 文档上传 | 智能问答 |
|-----------|---------|---------|
| ![知识库管理](Doc/aixg1.png) | ![文档上传](Doc/aixg2.png) | ![智能问答](Doc/aixg3.png) |

### AI 智能体技能
| 技能列表 | 技能配置 | 技能执行 |
|---------|---------|---------|
| ![技能列表](Doc/aizhinnegti01.png) | ![技能配置](Doc/aijineng04.png) | ![技能执行](Doc/zhinnegti03.png) |

### Hangfire 任务调度
| 任务列表 | 任务配置 | 执行记录 |
|---------|---------|---------|
| ![任务列表](Doc/xg_hangfire_01.png) | ![任务配置](Doc/xg_hangfire_02.png) | ![执行记录](Doc/xg_hangfire_03.png) |

### 后台管理系统 (Vue3 + AntDesign)
| 用户管理 | 角色管理 | 权限管理 | 系统配置 |
|---------|---------|---------|---------|
| ![用户管理](Doc/cf218b0549ca43120dca3d1ea0cd11eb.png) | ![角色管理](Doc/xg3.png) | ![权限管理](Doc/xg4.png) | ![系统配置](Doc/xg5.png) |

---

## ✨ 技术亮点

| 技术点 | 说明 |
|--------|------|
| **.NET 9** | 最新 LTS 版本，性能更优，支持更多新特性 |
| **DDD 架构** | 领域驱动设计，模块化结构，便于维护扩展 |
| **微服务架构** | 基于 Consul、CAP、Hangfire 实现服务解耦 |
| **AI 集成** | SemanticKernel、MCP 协议、Ollama 本地模型支持 |
| **RAG 检索增强** | Qdrant 向量数据库实现知识库问答 |
| **多租户支持** | 一库多租户架构，数据隔离 |
| **分布式缓存** | Redis 缓存层，支持多种缓存策略 |
| **日志系统** | log4net 日志框架，支持多级别日志 |

---

## 🚀 快速开始

### 环境要求

- .NET SDK 9.0+
- MySQL 8.0+
- Redis 7.0+
- Qdrant 1.7+（AI 功能）

### 配置步骤

**1. 配置数据库连接**

编辑 `App/WebApi/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "dbConnection": "server=127.0.0.1;port=3306;database=kevin_app;user id=root;password=admin123",
    "redisConnection": "127.0.0.1:6379,DefaultDatabase=0,password=123456"
  }
}
```

**2. 初始化数据库**

在 **程序包管理控制台** 执行：

```powershell
# 选择 Kevin.EntityFrameworkCore 项目
Add-Migration "初始化数据库"
Update-Database
```

**3. 启动应用**

```bash
cd App/WebApi
dotnet run --environment Development
```

**4. 访问地址**

| 服务 | 地址 |
|------|------|
| API | http://localhost:9901 |
| Swagger | http://localhost:9901/swagger |
| Hangfire | http://localhost:9901/hangfire |

### 默认账户

| 用户名 | 密码 | 租户 |
|--------|------|------|
| admin | 123456 | 1000 |

---

## 🧠 AI 智能体配置

### AI 配置效果图
| 模型管理 | 智能体管理 | 对话界面 |
|---------|----------|---------|
| ![模型管理](Doc/xg11.png) | ![智能体管理](Doc/xg12.png) | ![对话界面](Doc/dtai3.png) |

### 1. 安装 Qdrant

```bash
# 使用 Docker 启动 Qdrant
docker run -p 6333:6333 qdrant/qdrant
```

### 2. 配置 AI 模型

**智谱 AI：**
```json
{
  "OllamaApiSetting": {
    "Url": "https://open.bigmodel.cn/api/paas/v4/embeddings",
    "DefaultModel": "embedding-3",
    "ApiKey": "your-api-key"
  }
}
```

**Ollama 本地模型：**
```json
{
  "OllamaApiSetting": {
    "Url": "http://localhost:11434/api/embeddings",
    "DefaultModel": "qwen3:4b"
  }
}
```

### 3. 使用流程

1. 注册 AI 账户获取 API Key
2. 在系统中配置模型
3. 创建知识库并上传文档
4. 配置智能体并绑定技能
5. 开始智能对话

---

## 📁 项目结构

```
kevin.abp.core/
├── App/                    # 业务应用模块
│   ├── Application/        # 应用服务层
│   ├── Domain/             # 领域层
│   ├── RepositorieRps/     # 仓储实现
│   └── WebApi/             # API 入口
├── Kevin/                  # 核心框架模块
│   ├── Application/        # 核心服务
│   │   └── Services/AI/    # AI 相关服务
│   ├── Domain/             # 核心领域模型
│   ├── Kevin.EntityFrameworkCore/  # EF Core 实现
│   └── Kevin.Web.Basics/   # Web 基础组件
├── Doc/                    # 文档资源
└── InitData/               # 初始化数据
```

---

## 🛠️ 功能模块

| 模块 | 功能 | 状态 |
|------|------|------|
| **用户管理** | 用户CRUD、权限绑定 | ✅ |
| **角色管理** | 角色CRUD、权限配置 | ✅ |
| **权限管理** | 菜单权限、API权限 | ✅ |
| **AI 智能体** | 智能对话、工具调用 | ✅ |
| **知识库** | 文档管理、RAG检索 | ✅ |
| **任务调度** | Hangfire定时任务 | ✅ |
| **消息服务** | 钉钉消息推送 | ✅ |
| **文件存储** | 多云存储支持 | ✅ |

---

## 📖 文档资源

- **详细文档**: [SYSTEM_DOCUMENTATION.md](SYSTEM_DOCUMENTATION.md)
- **教学文档**: [CSDN 专栏](https://blog.csdn.net/weixin_42629287/category_13037923.html)
- **新项目教程**: [基于 NetCoreKevin 二次开发](https://gitee.com/netkevin-li/ainet)

---

## 🤝 交流社区

| 微信 | 交流群 |
|------|--------|
| ![微信](Doc/wx.jpeg) | ![交流群](Doc/wx_jiaoliuqun.JPG) |

---

## 📊 Star History

<a href="https://www.star-history.com/?repos=junkai-li/NetCoreKevin&type=timeline">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/chart?repos=junkai-li/NetCoreKevin&type=timeline&theme=dark" />
    <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/chart?repos=junkai-li/NetCoreKevin&type=timeline" />
    <img alt="Star History Chart" src="https://api.star-history.com/chart?repos=junkai-li/NetCoreKevin&type=timeline" />
  </picture>
</a>

---

**版本**: v1.0  
**License**: MIT  
**维护者**: NetCoreKevin 开发团队