---
kind: external_dependency
name: Redis 缓存与分布式基础设施
slug: redis
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

Redis 7.0+ 在本项目中承担多重角色：分布式缓存（Microsoft.Extensions.Caching.StackExchangeRedis）、Hangfire 任务存储（Hangfire.Redis.StackExchange）、SignalR 后端（StackExchangeRedis）以及 CAP 的 Redis Streams 存储。连接串位于 `ConnectionStrings.redisConnection`，格式为 `host:port,DefaultDatabase=0,password=...`。