---
kind: external_dependency
name: Ollama 本地大模型服务
slug: ollama
category: external_dependency
category_hints:
    - vendor_identity
scope:
    - '**'
---

通过 OllamaSharp SDK 集成 Ollama 本地模型服务，用于本地离线推理。默认地址 `http://localhost:11434/api/embeddings`，可在系统配置中切换为智谱 AI 等云端 embedding API。支持 qwen3:4b 等模型运行。