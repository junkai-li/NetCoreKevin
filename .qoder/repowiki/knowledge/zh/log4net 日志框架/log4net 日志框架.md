---
kind: external_dependency
name: log4net 日志框架
slug: log4net
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

项目采用 log4net 作为日志框架，配置文件位于 `App/WebApi/LogConfigs/`，按 Development/Test/Release 环境区分。`Kevin.log4Net` 模块提供封装集成。