using Microsoft.Extensions.AI;

namespace kevin.AI.AgentFramework.ImageGeneration
{
    /// <summary>
    /// 文生图工具工厂：按智能体绑定的模型配置构造 <c>GenerateImage</c> <see cref="AITool"/>。
    /// <para>
    /// 独立于 <see cref="IImageGenerationClient"/> 的动机：AIFunction 需要通过反射拿到方法上的
    /// <c>[Description]</c> 特性生成参数 schema，直接把 <see cref="IImageGenerationClient.GenerateAsync"/> 挂上去会
    /// 把 <see cref="ImageGenModelConfig"/> 也当成模型可见参数暴露出去（模型不可能自己填 endpoint/key）。
    /// 工厂负责把 config 通过闭包捕获，只让模型看到 prompt/size 两个业务参数。
    /// </para>
    /// </summary>
    public interface IImageGenToolFactory
    {
        /// <summary>
        /// 构造一个绑定到指定模型配置的 GenerateImage AIFunction
        /// </summary>
        /// <param name="config">文生图模型配置（endpoint/model/key）</param>
        /// <returns>可直接加入 <c>ChatOptions.Tools</c> 的 AITool</returns>
        AITool BuildGenerateFunction(ImageGenModelConfig config);
    }
}
