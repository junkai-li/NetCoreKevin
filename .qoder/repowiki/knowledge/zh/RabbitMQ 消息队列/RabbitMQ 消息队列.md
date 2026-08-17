---
kind: external_dependency
name: RabbitMQ 消息队列
slug: rabbitmq
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

RabbitMQ 作为 CAP 的消息传输介质，用于分布式事件发布订阅。CAP 通过 DotNetCore.CAP.RabbitMQ 扩展接入，需配合 MySQL 持久化保证消息可靠性。