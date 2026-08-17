---
kind: external_dependency
name: MySQL 主数据库
slug: mysql
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

项目使用 MySQL 8.0+ 作为主数据库，通过 EF Core 进行数据访问；CAP 消息总线也使用 MySql 存储持久化。连接串在 `App/WebApi/appsettings.json` 的 `ConnectionStrings.dbConnection` 中配置，默认端口 3306、库名 kevin_app。