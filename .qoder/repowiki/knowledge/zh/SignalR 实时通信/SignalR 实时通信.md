---
kind: external_dependency
name: SignalR 实时通信
slug: signalr-stackexchange-redis
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

使用 SignalR 实现前后端实时通信，并通过 StackExchangeRedis 作为 ScaleOut 后端，使多实例部署下消息能跨进程广播。