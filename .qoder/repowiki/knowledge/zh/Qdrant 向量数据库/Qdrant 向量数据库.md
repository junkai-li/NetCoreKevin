---
kind: external_dependency
name: Qdrant 向量数据库
slug: qdrant
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

Qdrant 1.7+ 作为 RAG 检索增强生成的向量数据库，由 `Kevin.RAG` 模块通过 Qdrant.Client SDK 接入。默认监听 6333 端口，可通过 Docker 启动；向量模型和对话模型在系统配置中注册后用于知识库问答。