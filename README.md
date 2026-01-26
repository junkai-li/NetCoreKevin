# NetCoreKevin介绍
|  ![输入图片说明](Doc/fe381882-f0d6-457f-8b87-974a6e6a8f21.png) | ![输入图片说明](Doc/4344f17d-26d4-41ea-a235-1b9d67a633ce.png)  |  ![输入图片说明](Doc/3e146090-58a6-44bf-8cff-2b6b6323ef11.png)  |
|---|---|---|
| ![输入图片说明](Doc/bd2a60ce-2f32-46df-8bd6-9ffa5da515b4.png)  |  ![输入图片说明](Doc/3482bece-3f43-4ace-9337-283eb8a42903.png)|  ![输入图片说明](Doc/39c1ee78-3814-4e27-a158-2934317a2666.png) |

# 🌐知识库AI智能体效果图 
|  ![输入图片说明](Doc/aixg1.png) | ![输入图片说明](Doc/aixg2.png)  | ![输入图片说明](Doc/aixg3.png)  |
|---|---|---|



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



# 教学文档地址 [[教学文档地址](https://blog.csdn.net/weixin_42629287/category_13037923.html)]






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

# 🧰AI智能体教程

-   第一步
-   `请先完成上手教程在进行AI智能体教程`
-   第二步
-   `下载安装Qdrant--官网有教程 安装后配置json文件QdrantClientSetting 默认是localhost不需要动的`
-   第三步
-   `注册AI账户 教程以智谱AI为例 去[官网](https://open.bigmodel.cn)注册登录后获取APIKey`
-   第四步
-  `配置向量模型和对话模型默认如下`

|![向量模型](Doc/%E9%A1%B9%E7%9B%AE%E7%9B%B8%E5%85%B3/7f97a2cb-3707-46f6-a8f7-bc48196ed941.png)|![对话模型](Doc/%E9%A1%B9%E7%9B%AE%E7%9B%B8%E5%85%B3/e0b7574a-0c47-45e7-b462-2d0e0707fe4d.png)|
|--|--| 

-   第五步
-   `新建知识库选择向量模型(如果不选择请在json配置中配置)2048（默认）：最高精度，适合对准确性要求极高的场景===》配置智能体==》新建对话就OK了`

|![输入图片说明](Doc/%E9%A1%B9%E7%9B%AE%E7%9B%B8%E5%85%B3/image.png)|![输入图片说明](Doc/%E9%A1%B9%E7%9B%AE%E7%9B%B8%E5%85%B3/a4f1e8c1-7380-4b3f-acca-b01c849730bb.png)| ![输入图片说明](Doc/%E9%A1%B9%E7%9B%AE%E7%9B%B8%E5%85%B3/image3423123.png) |
|--|--|--|

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

一、 核心安全与认证

授权服务：基于IdentityServer4，提供用户登录认证和API访问授权。

权限服务：实现基于API接口的细粒度访问控制。

二、 业务架构层

领域层：存放核心业务逻辑与规则（DDD核心）。

应用服务层：协调领域逻辑，处理具体业务用例。

仓储层：封装所有数据库的增删改查操作。

API层：提供对外RESTful接口。

三、 通用技术支持

工具库：包含常用辅助类、扩展方法等。

工作单元：基于EF Core，管理数据库事务。

缓存服务：支持多种分布式缓存。

日志系统：基于log4Net记录运行日志。

文件存储：支持云存储的文件上传下载。

短信服务：集成主流云平台发送短信。

四、 分布式与集成

服务发现：使用Consul进行微服务注册与发现。

事件总线：基于CAP处理分布式事件通信。

领域事件：基于MediatR处理进程内事件。

HTTP客户端：封装HTTP/2.0调用，用于外部API访问。

分布式锁：控制分布式环境下的资源并发访问。

五、 AI与智能模块

AI基础模块：集成语义理解、OCR识别等AI能力。

智能体服务：基于AgentFramework的AI智能体系统。

知识库系统：使用Qdrant向量数据库实现RAG检索增强。

联网搜索：集成搜索引擎API，支持实时信息获取。

智能体管理：智能体生命周期管理、工具调用和工作流编排。

六、 特定功能模块

任务调度：基于Quartz执行定时任务。

实时通信：基于SignalR实现服务端消息推送。

爬虫服务：基于Selenium进行网页数据采集。

七、 基础设施与配置

IOC容器：提供模块化依赖注入。

跨域配置：管理Web跨域访问策略。

共享模型：存放系统通用的DTO、枚举等。

数据初始化：提供数据库结构和初始数据的SQL脚本。

Web工具层：包含全局过滤器、中间件等Web相关基础组件。


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

        
    
# 🌐交流群

 |  ![输入图片说明](Doc/wx.jpeg) |     ![输入图片说明](Doc/wx_jiaoliuqun.JPG)  |
|---|---|

# DDD思想    
![输入图片说明](Doc/e6daeb16-5b1e-487b-93be-f73f5201964a.png)
# 思维导图
![思维导图](Doc/junkai-li-NetCoreKevin-mindmap.png)

