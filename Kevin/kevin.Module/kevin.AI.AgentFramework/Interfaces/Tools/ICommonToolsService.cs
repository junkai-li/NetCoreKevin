using System.ComponentModel;

namespace kevin.AI.AgentFramework.Interfaces.Tools
{
    /// <summary>
    /// 常用工具类，包含一些常用的静态方法和属性，可以被智能体调用的用于提供一些常用的功能。
    /// </summary>
    public interface ICommonToolsService : IBaseAIToolService
    {
        /// <summary>
        /// 获取当前时间
        /// </summary>
        [Description("获取当前时间，返回系统的当前时间。")]
        Task<string> GetCurrentTime();
        /// <summary>
        /// 获取当前系统。返回运行时平台的友好名称（例如 "Windows"、"Linux"、"macOS"、"Unknown"）。
        /// </summary>
        [Description("获取当前系统,返回运行时平台的友好名称（例如 \"Windows\"、\"Linux\"、\"macOS\"、\"Unknown\"）。")]
        Task<string> GetRuntimePlatform();

        /// <summary>
        /// 获取当前系统桌面路径。 用于获取当前用户的桌面路径
        /// </summary>
        [Description("获取当前系统桌面路径。 用于获取当前用户的桌面路径，若 ensureExists 为 true 则确保目录存在。")]
        Task<string> GetDesktopPath([Description("ensureExists 默认为 true 则确保目录存在。")] bool ensureExists = true);

        /// <summary>
        /// 输出文件到系统桌面。 用于把各种文件输出到桌面
        /// </summary>
        [Description("输出文件到系统桌面。 用于把各种文件输出到桌面，返回完整路径或以 ❌ 开头的错误信息。")]
        Task<string> WriteTextToDesktop([Description("文件名称如（xx.html,xx.txt）支持各种文件类型")] string fileName, [Description("内容")] string content, [Description("文件是否不存在 默认是")] bool overwrite = true);

        /// <summary>
        /// 将字节数组保存到桌面指定文件，返回完整路径或以 "❌" 开头的错误信息。
        /// </summary>
        [Description("将字节数组保存到桌面指定文件，返回完整路径或以 ❌ 开头的错误信息。")]
        Task<string> WriteBytesToDesktop(string fileName, byte[] data, bool overwrite = true);

        /// <summary>
        /// 将流内容保存到桌面指定文件，返回完整路径或以 "❌" 开头的错误信息。
        /// </summary>
        [Description("将流内容保存到桌面指定文件，返回完整路径或以 ❌ 开头的错误信息。")]
        Task<string> WriteStreamToDesktop(string fileName, Stream stream, bool overwrite = true);

        /// <summary>
        /// 从源路径复制文件到桌面（可重命名），返回目标完整路径或以 "❌" 开头的错误信息。
        /// </summary>
        [Description("从源路径复制文件到桌面（可重命名），返回目标完整路径或以 ❌ 开头的错误信息。")]
        Task<string> CopyFileToDesktop(string sourcePath, string fileName = null, bool overwrite = true);
    }
}
