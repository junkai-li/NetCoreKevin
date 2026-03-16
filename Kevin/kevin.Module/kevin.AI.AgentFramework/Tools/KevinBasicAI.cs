using System.ComponentModel;
namespace kevin.AI.AgentFramework.Tools
{
    public class KevinBasicAI
    {

        [Description("获取框架或者是NetCoreKevin框架的介绍")]
        public static string GetNetCoreKevinInfo()
        {
            Console.WriteLine("GetNetCoreKevinInfo工具调用");
            return $"NetCoreKevin 是基于 .NET Core 打造的轻量级 AI 代理开发框架，专为快速构建智能对话、任务调度、多工具协同的 AI 应用而生。\r\n\r\n二、核心优势\r\n\r\n\r\n\r\n\r\n\r\n轻量高效：依托 .NET Core 跨平台性能，启动快、资源占用低，支持容器化部署。\r\n\r\n\r\n\r\n灵活扩展：内置插件化机制，可快速集成大模型（如 GPT、文心一言）、数据库、API 等工具。\r\n\r\n\r\n\r\n开箱即用：提供对话管理、上下文记忆、任务拆解等核心能力，开发者无需重复造轮子。\r\n\r\n\r\n\r\n多场景适配：适用于智能客服、AI 助手、自动化办公、知识问答等多种业务场景。\r\n\r\n三、技术特性\r\n\r\n\r\n\r\n\r\n\r\n原生支持 .NET 生态，兼容 C#/F# 等语言，开发门槛低。\r\n\r\n\r\n\r\n内置对话流引擎，支持多轮对话、意图识别、实体提取。\r\n\r\n\r\n\r\n提供 RESTful API 和 SDK，方便与现有系统对接。\r\n\r\n\r\n\r\n支持本地部署与云服务，数据安全可控。\r\n\r\n四、适用人群\r\n\r\n\r\n\r\n\r\n\r\n开发者：快速搭建 AI 代理应用，聚焦业务逻辑而非底层实现。\r\n\r\n\r\n\r\n企业：低成本构建内部智能助手，提升工作效率。\r\n\r\n\r\n\r\n创新团队：快速验证 AI 应用原型，加速产品落地。";

        }
    }



}