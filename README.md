# NetCoreKevin
基于NET8搭建DDD-微服务-WebApi架构支持：IDS4单点登录、多缓存、自动任务、分布式、一库多租户、日志、授权和鉴权、CAP集成事件、SignalR、领域事件、ESL、MCP协议服务、IOC模块化注入、Cors、Quartz自动任务、多短信集成、AI智能体、AISemanticKernel集成、OCR验证码识别、API多版本兼容、单元测试
解耦设计都可以单独引用 项目启动时注意相关Redis、db链接配置  
生成数据库可见下面说明文件 命令生成即可 
项目结构    
![image](https://github.com/user-attachments/assets/e0fe3e9f-18b0-4345-b9ac-3b728d3e780c)
思维导图
![输入图片说明](Doc/junkai-li-NetCoreKevin-mindmap.png)
[教学文档地址](https://opendeep.wiki/junkai-li/NetCoreKevin/mindmap)
# 项目概述
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

<img width="563" height="832" alt="image" src="https://github.com/user-attachments/assets/79d71802-8e07-459d-b5e1-a98cb953b6f4" />
![输入图片说明](Doc/image.png)


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

