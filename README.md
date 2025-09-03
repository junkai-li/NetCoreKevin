# NetCoreKevin
基于NET8搭建DDD-微服务-WebApi架构支持：IDS4单点登录、多缓存、自动任务、分布式、一库多租户、日志、授权和鉴权、CAP集成事件、SignalR、领域事件、ESL、MCP协议服务、IOC模块化注入、Cors、Quartz自动任务、多短信集成、AI智能体、AISemanticKernel集成、OCR验证码识别、API多版本兼容、单元测试
解耦设计都可以单独引用 项目启动时注意相关Redis、db链接配置  
生成数据库可见下面说明文件 命令生成即可 
项目结构    
![image](https://github.com/user-attachments/assets/e0fe3e9f-18b0-4345-b9ac-3b728d3e780c)
思维导图
![输入图片说明](Doc/junkai-li-NetCoreKevin-mindmap.png)
[教学文档地址](https://opendeep.wiki/junkai-li/NetCoreKevin/mindmap)


<img width="563" height="832" alt="image" src="https://github.com/user-attachments/assets/79d71802-8e07-459d-b5e1-a98cb953b6f4" />
![输入图片说明](Doc/image.png)


 
 # NetCoreKevin
基于NET8搭建DDD-微服务-WebApi架构支持：IDS4单点登录、多缓存、自动任务、分布式、一库多租户、日志、授权和鉴权、CAP集成事件、SignalR、领域事件、ESL、MCP协议服务、IOC模块化注入、Cors、Quartz自动任务、多短信集成、AI智能体、AISemanticKernel集成、OCR验证码识别、API多版本兼容、单元测试
解耦设计都可以单独引用 项目启动时注意相关Redis、db链接配置  
生成数据库可见下面说明文件 命令生成即可 
项目结构    
![image](https://github.com/user-attachments/assets/e0fe3e9f-18b0-4345-b9ac-3b728d3e780c)
思维导图
![输入图片说明](Doc/junkai-li-NetCoreKevin-mindmap.png)
[教学文档地址](https://opendeep.wiki/junkai-li/NetCoreKevin/mindmap)


<img width="563" height="832" alt="image" src="https://github.com/user-attachments/assets/79d71802-8e07-459d-b5e1-a98cb953b6f4" />
![输入图片说明](Doc/image.png)


 
 1.目录 
 
    1. App.AuthorizationService: 颁发授权服务中心 基于Identity框架搭建支持单点登录

    2. Kevin.Common: 工具类库包含了、常用帮助类、扩展方法、文件流操作、 图片操作、等常见代码封装

    3. 文档: 基础使用文档

    4. InitData: 数据初始化Sql脚本

    5. Share: 用于存放Dto、相关结构类型

    6. App.TaskQuartz: 基于Quartz搭建的自动任务调度系统

    7. App.RepositorieRps: 数据库仓储

    8. App.Application: 应用服务层

    9. Kevin.Web: Web相关的工具层包含 过滤器、中间件、全局类、基础类、服务初始化

    10. App.WebApi: API层

    11. kevin.CrawlingService ：基于Selenium.WebDriver.MSEdgeDriver用于爬虫数据||自动化测试

    12. Kevin.EntityFrameworkCore：基于EFCore搭建的工作单元

    13. kevin.Domain：领域服务层

    14. kevin.Cache：缓存基于微软IDistributedCache基础开发 支持多缓存模式

    15. kevin.Cap：基于Cap搭建分布式事件

    16. kevin.Consul：微服务中基于Consul实现的服务注册与发现

    17. Kevin.Cors：跨域-可自定义跨域配置

    18. kevin.DistributedLock：分布式锁

    19. kevin.Domain.EventBus：基于MediatR实现领域事件

    20. kevin.FileStorage：文件服务支持阿里云、腾讯云

    21. Kevin.HttpApiClients：IHttpClientFactory工厂实现（HTTP2.0）

    22. kevin.Ioc：IOC容器

    23. Kevin.log4Net：Log4日志

    24. kevin.Permission：权限服务可根据Api初始化配置相关权限

    25. Kevin.SignalR：SignalR实时通信

    26. Kevin.SMS：短信服务支持阿里云、腾讯云

    27.Kevin.AI Kevin.AI.MCP.Server  Kevin.AI.SemanticKernel AI集成基于SemanticKernel简单示例 MCP服务简单示例 AI使用简单示例

 AI效果图
 ![203d2bb1bb39cfc0d42010fba0dae0a](https://github.com/user-attachments/assets/dc7c1e76-1714-47d0-b252-3c130546cf4b)

 2.docker配置  
 
![image](https://user-images.githubusercontent.com/57887866/233823710-e8ad6fe8-5eb8-4fda-b3e1-09c36e508ed0.png)  

 3.json配置  
 
![image](https://user-images.githubusercontent.com/57887866/233823842-2263ff1b-91ec-4f77-8ff3-dca129e01bd7.png)    

 4.部分说明  
 
![image](https://user-images.githubusercontent.com/57887866/233824730-55549181-057c-4298-8601-e9734bf99d0a.png)  
  
 5.基础API
 <img width="1920" height="911" alt="image" src="https://github.com/user-attachments/assets/9ac73e9a-1e3d-4d0c-add9-7e4b938e231e" />



 AI效果图
 ![203d2bb1bb39cfc0d42010fba0dae0a](https://github.com/user-attachments/assets/dc7c1e76-1714-47d0-b252-3c130546cf4b)

 2.docker配置  
 
![image](https://user-images.githubusercontent.com/57887866/233823710-e8ad6fe8-5eb8-4fda-b3e1-09c36e508ed0.png)  

 3.json配置  
 
![image](https://user-images.githubusercontent.com/57887866/233823842-2263ff1b-91ec-4f77-8ff3-dca129e01bd7.png)    

 4.部分说明  
 
![image](https://user-images.githubusercontent.com/57887866/233824730-55549181-057c-4298-8601-e9734bf99d0a.png)  
  
 5.基础API
 <img width="1920" height="911" alt="image" src="https://github.com/user-attachments/assets/9ac73e9a-1e3d-4d0c-add9-7e4b938e231e" />

