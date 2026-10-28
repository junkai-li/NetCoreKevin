---
kind: design
name: OpenAI 兼容接口的 API Key 从 appsettings.json 迁移至用户级数据库存储
source: session
category: adr
---

# OpenAI 兼容接口的 API Key 从 appsettings.json 迁移至用户级数据库存储

_来源：c2f1620 → c475015 提交周期内记录的编码计划——内容为规划时意图，实现可能滞后或有出入。_

**状态：** accepted

## 背景
OpenAI 兼容接口的鉴权原依赖全局 `appsettings.json` 中的 `ApiKeys` 列表，无法区分租户和用户；需要改为每个用户一条、可独立启用/禁用、支持明文查看与重新生成的密钥管理方案。

## 决策驱动
- 多租户隔离（按 TenantId + UserId）
- 最小改动网关层语义
- 复用现有 AES 加密实现
- 前端自助管理密钥

## 备选方案
- **保留 appsettings.json 全局配置** _（已否决）_ — 优点：无需改 Filter、无需新表；缺点：无法区分用户/租户，无法动态启停，不符合多租户需求
- **引入外部密钥管理服务（如 HashiCorp Vault / Azure Key Vault）** _（已否决）_ — 优点：企业级密钥管理、审计完善；缺点：引入额外依赖和运维成本，超出当前范围
- **将 EncryptedKey 落库 + Filter 解密匹配** — 优点：复用现有 `AESEncryptor`，Filter 改动最小，鉴权流程不变；缺点：每次鉴权需遍历该租户所有记录并解密比较，存在性能瓶颈

## 决策
新建实体 `TAIOpenAICompatibleKey`（UserId 唯一约束），通过 `OpenAICompatibleKeyController` 提供生成/查看/启停等管理接口；`OpenAICompatibleApiKeyFilter` 改为从 `ITAIOpenAICompatibleKeyRp` 查 `IsEnabled=true` 的记录，逐条 `AESEncryptor.DecryptString(EncryptedKey)` 后与请求 Bearer Key 做 `string.Equals` 匹配，命中后用 `IAuthorizeService.GetTokenById(UserId, TenantId)` 签发内部 token。

## 影响
鉴权路径新增一次 DB 查询 + 若干次 AES 解密；后续可按 `EncryptedKey` 建索引或加缓存优化。旧 `OpenAICompatibleSetting.ApiKeys` 不再读取。前端新增 `/home/my/openai-compatible-key` 页面供用户自助管理密钥及查看对接文档。EF Core 通过 `GetTables()` 自动建表 `t_a_io_pen_a_i_compatible_key`，部署时需执行 EF Migration。