---
name: system-ops
description: 系统运维诊断技能。适用于系统健康检查、磁盘空间分析、进程资源监控、故障排查等系统运维场景。包含可执行的诊断脚本。
---

# 系统运维（System Operations）

## 可用诊断脚本

以下脚本位于本技能的 `scripts/` 目录，可通过 `run_shell` 工具执行：

| 脚本 | 用途 | 执行命令 |
|------|------|--------|
| check-system-info.ps1 | 获取系统基本信息（OS、CPU、内存） | 
| check-disk-usage.ps1 | 检查磁盘使用情况和剩余空间 | 
| check-top-processes.ps1 | 查看 CPU/内存占用 Top 进程 |

## 运维检查流程

1. **基础检查**：先执行 `check-system-info.ps1` 获取系统概况
2. **针对性诊断**：根据用户问题，选择性执行磁盘或进程检查脚本
3. **分析报告**：综合脚本输出和故障排查指南，给出诊断结论和建议
4. **故障排查**：如需深入排查，参考 [references/troubleshooting-guide.md](references/troubleshooting-guide.md)

## 告警阈值

| 指标 | 正常 | 警告 | 严重 |
|------|------|------|------|
| CPU 使用率 | <70% | 70-90% | >90% |
| 内存使用率 | <80% | 80-95% | >95% |
| 磁盘使用率 | <70% | 70-90% | >90% |
| 单进程 CPU | <30% | 30-60% | >60% |