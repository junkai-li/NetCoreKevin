# NetCoreKevin介绍
|  ![输入图片说明](Doc/fe381882-f0d6-457f-8b87-974a6e6a8f21.png) | ![输入图片说明](Doc/4344f17d-26d4-41ea-a235-1b9d67a633ce.png)  |  ![输入图片说明](Doc/3e146090-58a6-44bf-8cff-2b6b6323ef11.png)  |
|---|---|---|
| ![输入图片说明](Doc/bd2a60ce-2f32-46df-8bd6-9ffa5da515b4.png)  |  ![输入图片说明](Doc/3482bece-3f43-4ace-9337-283eb8a42903.png)|  ![输入图片说明](Doc/39c1ee78-3814-4e27-a158-2934317a2666.png) |

# 🌐知识库AI智能体效果图 
![输入图片说明](Doc/aixg1.png)
![输入图片说明](Doc/aixg2.png)![输入图片说明](Doc/aixg3.png)
# 🌐后台管理系统效果图（基于VUE3-AntDesign） 

| ![输入图片说明](Doc/cf218b0549ca43120dca3d1ea0cd11eb.png)  |  ![输入图片说明](Doc/6271341ceebc25c93a2ae361008937f6.png) | ![输入图片说明](Doc/xg3.png)   |  ![输入图片说明](Doc/xg4.png) |
|---|---|---|---|
| ![输入图片说明](Doc/xg5.png)   |  ![输入图片说明](Doc/xg6.png) | ![输入图片说明](Doc/xg7.png)  |  ![输入图片说明](Doc/xg8.png) |
|  ![输入图片说明](Doc/xg9.png) | ![输入图片说明](Doc/xg10.png)  |  ![输入图片说明](Doc/xg11.png) | ![输入图片说明](Doc/xg12.png)  |


# 🌐后台自动任务调度系统（基于Quartz.NET）
![输入图片说明](Doc/task.png)

# NetCoreKevin前言
基于NET搭建-AI智能体-现代化Saas企业级前后端分离架构-开启智能应用的无限可能：前端Vue3、IDS4单点登录、多缓存、自动任务、分布式、一库多租户、日志、授权和鉴权、CAP集成事件、SignalR、领域事件、ESL、MCP协议服务、IOC模块化注入、Cors、Quartz自动任务、多短信集成、AI、AgentFramework智能体、AISemanticKernel集成、RAG检索增强、OCR验证码识别、API多版本兼容、单元集成测试、RabbitMQ
解耦设计都可以单独引用项目启动时注意相关Redis、db链接配置、RabbitMQ、生成数据库可见下面说明文件 命令生成即可 

# DDD思想    
![输入图片说明](Doc/e6daeb16-5b1e-487b-93be-f73f5201964a.png)
# 思维导图
![思维导图](Doc/junkai-li-NetCoreKevin-mindmap.png)

# 教学文档地址 [[教学文档地址](https://blog.csdn.net/weixin_42629287/category_13037923.html)]

# 🌐 项目概述
NetCoreKevin 是一个基于 DDD（Domain-Driven Design） 和 微服务架构 的 .NET 8 Web API 项目，其核心目标是提供一个可复用、模块化、可扩展的架构平台。它集成了以下关键功能：

    身份认证与授权（基于 IdentityServer4）
    多缓存支持（Redis、内存缓存）
    分布式系统支持（CAP 集成事件）
    多租户支持（一库多租户）
    任务调度（Quartz）
    日志系统（log4Net）
    实时通信（SignalR）
    AI 集成（SemanticKernel、OCR 验证码识别）
    短信服务（阿里云、腾讯云）
    文件存储（阿里云、腾讯云）
    自动化爬虫（Selenium）
    模块化依赖注入（IOC）
    多版本 API 兼容
    单元测试支持
    Docker 支持
    RabbitMQ

 ![输入图片说明](Doc/image.png)


# 🧩 项目结构详解
以下是项目的核心模块及其功能说明：

- 1. App.AuthorizationService
- 功能：授权服务，基于 IdentityServer4 搭建，支持单点登录（SSO）。
- 说明：统一管理用户认证和授权，是整个系统的安全中心。
- 2. Kevin.Common
- 功能：工具类库，包含常用帮助类、扩展方法、文件流操作、图片处理等。
- 说明：提供基础的通用功能，避免重复开发。
- 3. 文档
- 功能：基础使用文档。
- 说明：包含项目启动说明、配置步骤、数据库生成方法等。
- 4. InitData
- 功能：数据初始化的 SQL 脚本。
- 说明：用于初始化数据库结构和测试数据。
- 5. Share
- 功能：存放 DTO（数据传输对象）、结构类型等共享模型。
- 说明：用于跨模块的数据交换。
- 6. App.TaskQuartz
- 功能：基于 Quartz 的自动任务调度系统。
- 说明：用于定时执行后台任务，如数据同步、清理、推送等。
- 7. App.RepositorieRps
- 功能：数据库仓储层。
- 说明：提供对数据库的读写操作，通常使用 EFCore 实现。
- 8. App.Application
- 功能：应用服务层。
- 说明：处理业务逻辑，调用仓储层或领域层，是业务操作的中转站。
- 9. Kevin.Web
- 功能：Web 相关工具层，包含过滤器、中间件、全局类、服务初始化等。
- 说明：提供 Web 层的基础配置和扩展。
- 10. App.WebApi
- 功能：API 层。
- 说明：对外暴露的 RESTful API 接口，接收请求并调用应用服务。
- 11. kevin.CrawlingService
- 功能：基于 Selenium 的爬虫服务或自动化测试。
- 说明：用于模拟浏览器行为进行数据采集或测试。
- 12. Kevin.EntityFrameworkCore
- 功能：基于 EFCore 的工作单元（UnitOfWork）实现。
- 说明：提供统一的数据库事务管理。
- 13. kevin.Domain
- 功能：领域服务层。
- 说明：实现核心业务逻辑，是 DDD 的核心部分。
- 14. kevin.Cache
- 功能：基于 IDistributedCache 的缓存服务，支持多种缓存方式。
- 说明：提供缓存策略和操作接口。
- 15. kevin.Cap
- 功能：基于 CAP 的分布式事件总线。
- 说明：用于微服务之间的消息通信和事件处理。
- 16. kevin.Consul
- 功能：微服务注册与发现，使用 Consul 实现。
- 说明：支持服务的动态发现和负载均衡。
- 17. Kevin.Cors
- 功能：跨域配置模块。
- 说明：提供自定义的跨域策略支持。
- 18. kevin.DistributedLock
- 功能：分布式锁。
- 说明：用于在分布式环境中控制资源访问。
- 19. kevin.Domain.EventBus
- 功能：基于 MediatR 的领域事件实现。
- 说明：用于处理业务逻辑中的事件发布与订阅。
- 20. kevin.FileStorage
- 功能：文件存储服务，支持阿里云、腾讯云。
- 说明：提供文件上传、下载、管理等功能。
- 21. Kevin.HttpApiClients
- 功能：基于 IHttpClientFactory 的 HTTP 客户端。
- 说明：支持 HTTP/2.0，用于调用外部 API。
- 22. kevin.Ioc
- 功能：IOC 容器模块。
- 说明：提供模块化的依赖注入配置。
- 23. Kevin.log4Net
- 功能：日志系统，基于 log4Net。
- 说明：用于记录系统运行日志，便于调试和分析。
- 24. kevin.Permission
- 功能：权限服务，可根据 API 配置权限。
- 说明：支持基于 API 的细粒度权限控制。
- 25. Kevin.SignalR
- 功能：SignalR 实时通信服务。
- 说明：用于实时消息推送、在线状态同步等。
- 26. Kevin.SMS
- 功能：短信服务，支持阿里云、腾讯云。
- 说明：用于发送验证码、通知短信等。
- 27. Kevin.AI 系列
- 功能：AI 集成模块，包括 SemanticKernel、MCP 服务、OCR 验证码识别等。
- 说明：集成 AI 功能，如语义理解、验证码识别等。

# 🧰 技术亮点
技术点	说明

- .NET 8	最新的 .NET 框架，性能更优，支持更多新特性
- DDD	领域驱动设计，将复杂业务逻辑抽象为模块化结构
- 微服务架构	通过 Consul、CAP、Quartz 等实现服务解耦和分布式管理
- CAP	消息总线，用于跨服务事件通信
- MediatR	领域事件处理
- IdentityServer4	安全认证中心，支持 OAuth2 和 OpenID Connect
- Quartz	定时任务调度
- EF Core	ORM 工具，用于数据库操作
- Docker	容器化部署，便于环境管理
- AI 集成	SemanticKernel、MCP 服务、OCR 验证码识别等 AI 技术

# 🧠 AI 集成说明
AI 模块包括：

- SemanticKernel：语义理解框架，支持自然语言处理。
- OCR 验证码识别：通过 AI 技术识别验证码图像。
- MCP 服务：用于 AI 服务通信或协议扩展。

# 📝 总结
NetCoreKevin 是一个非常完整的微服务架构项目，结合了 DDD、CAP、Quartz、EFCore 等多种技术，并集成了 AI、OCR、短信、文件存储等实用功能。其模块化设计使得每个功能都可以独立引用，非常适合大型企业级应用的开发。

# 基础API
![输入图片说明](Doc/478957534-9ac73e9a-1e3d-4d0c-add9-7e4b938e231e.png) 

# 📦 项目启动与配置

- 1. 配置 Redis 和数据库
- 项目启动前需配置 Redis 和数据库连接字符串。
- 数据库结构可通过脚本生成。
- 2. Docker 配置
- 项目支持 Docker 部署，Dockerfile 和配置文件已提供。
- 3. JSON 配置
- 项目使用 JSON 文件进行配置，包括缓存、短信、日志、AI 等模块。

# 🧰上手教程

-   第一步
-   `json配置文件请配置好数据库链接默认使用MySQL（可自行更改）EFCore框架 不熟悉的可以看看微软官网文档`
-   第二步
-   `执行初始化数据库命令：在Kevin.EntityFrameworkCore执行NuGet命令  Add-Migration "初始化数据库"  在执行=》 Update-Database  执行完成后开始第三步`
-   第三步
-   `启动App.WebApi第一次启动会初始化种子数据 种子数据配置可以在kevin.Domain.BaseDatas下面查看和更改 启动后通过http://localhost:9901/swagger/index.html路由查看接口页面和如下页面`
-   第四步
-  `启动App.AuthorizationService项目统一授权中心`
-   第五步
-   `在vue文件夹下面启动前端 前端运行步骤：1、安装依赖npm install 2、运行npm run dev 3、打包npm run build`


 ![输入图片说明](Doc/server.png)
        
    
# 🌐交流群

![输入图片说明](Doc/wx.jpeg)     ![输入图片说明](Doc/wx_jiaoliuqun.JPG)
