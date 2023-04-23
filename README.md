# NetCoreKevin
基于NET6搭建跨平台DDD思想WebApi架构、IDS4单点登录、多缓存、自动任务、分布式、多租户、日志、授权和鉴权、CAP、SignalR

项目结构  
![1681886164784](https://user-images.githubusercontent.com/57887866/232987165-2521adaa-6e69-4feb-906f-17e1ffb36939.png)  
docker配置  
![image](https://user-images.githubusercontent.com/57887866/233823710-e8ad6fe8-5eb8-4fda-b3e1-09c36e508ed0.png)  
json配置  
![image](https://user-images.githubusercontent.com/57887866/233823842-2263ff1b-91ec-4f77-8ff3-dca129e01bd7.png)    
部分说明  
![image](https://user-images.githubusercontent.com/57887866/233824730-55549181-057c-4298-8601-e9734bf99d0a.png)

 1.目录 
 
    1.Kevin.AuthorizationService: 颁发授权服务中心 基于Identity框架搭建

    2.Common: 工具库包含一些扩展类、帮助类、静态方法、微信、支付宝、阿里云、腾讯、Json相关、SSL证书

    3.文档: 基础使用文档

    4.InitData: 数据初始化Sql脚本

    5.型号: 用于存放Dto、相关结构类型

    6.Quartz: 基于Quartz搭建的自动任务调度系统

    7.Kevin.Repository: 数据库工作单元仓储

    8.AppServices: 应用服务层

    9.Web: Web相关的工具层包含 鉴权验权、过滤器、中间件、全局类、权限控制、基础类、缓存、服务初始化、单点登录

    10.WebApi: API层
    
    11.CrawlingService ：基于Selenium.WebDriver.MSEdgeDriver用于爬虫数据||自动化测试
    
    12.AppRepositorieRps 数据仓储层
    
    13.AppDomainServices 领域服务层

