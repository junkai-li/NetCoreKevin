using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Kevin.Common.Helper
{
    /// <summary>
    /// Python安全校验器
    /// 用于检测危险的Python代码，防止恶意代码执行
    /// </summary>
    public class PythonSecurityValidator
    {
        /// <summary>
        /// 危险导入列表 (仅保留真正高危且少用的模块)
        /// </summary>
        private static readonly string[] DangerousImports = new[]
        {
        "import subprocess", // 需结合上下文，但作为初步筛选可保留
        "from pickle import",
        "from marshal import",
        "from imp import" // imp 已废弃，通常意味着老旧或不安全代码
    };

        /// <summary>
        /// 危险正则模式列表 (聚焦于直接执行代码和反序列化)
        /// </summary>
        private static readonly string[] DangerousPatterns = new[]
        {
        // 1. 任意代码执行 (最高危)
        @"exec\s*\(",
        @"eval\s*\(",
        @"compile\s*\(",
        @"__import__\s*\(",
        
        // 2. 系统命令执行 (需警惕 shell=True)
        @"os\.system\s*\(",
        @"os\.popen\s*\(",
        @"os\.execl\s*\(",
        @"os\.execv\s*\(",
        @"os\.spawnl\s*\(",
        @"os\.spawnv\s*\(",
        @"os\.fork\s*\(",
        
        // 3. 不安全的反序列化 (远程代码执行常见入口)
        @"pickle\.load\s*\(",
        @"pickle\.loads\s*\(",
        @"marshal\.load\s*\(",
        @"marshal\.loads\s*\(",
        
        // 4. 动态导入 (视业务情况而定，若允许插件机制则需移除)
        // @"importlib\.\w+", 
        
        // 5. 典型的注入模式: eval(input(...))
        @"eval\s*\(\s*input\s*\("
    };

        /// <summary>
        /// 危险命令/字符串列表 (仅保留明确的 Shell 注入特征或敏感文件访问)
        /// </summary>
        private static readonly string[] DangerousCommands = new[]
        {
        // Shell 注入特殊字符 (仅在拼接字符串时危险，静态检测难免误报，但比单字符好)
        "&&", "||", ";", "$(", "`", 
        
        // 交互式 Shell 启动 (典型后门特征)
        "bash -i", "sh -i",
        
        // 敏感文件路径 (直接读取/etc/passwd通常是恶意的)
        "/etc/passwd",
        "/etc/shadow",
        
        // 提权命令 (在服务器端脚本中出现通常异常)
        "sudo", "su ", "chmod", "chown", "useradd", "adduser", "passwd"
    };

        public static ValidationResult ValidatePythonCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ValidationResult { IsValid = true, Message = "代码为空，校验通过" };
            }

            var result = new ValidationResult();

            // 1. 检查危险导入
            foreach (var import in DangerousImports)
            {
                if (ContainsIgnoreCase(code, import))
                {
                    result.IsValid = false;
                    result.BlockedItems.Add($"危险导入: {import}");
                }
            }

            // 2. 检查危险正则模式
            foreach (var pattern in DangerousPatterns)
            {
                try
                {
                    var matches = Regex.Matches(code, pattern, RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        result.IsValid = false;
                        result.BlockedItems.Add($"危险模式: {match.Value}");
                    }
                }
                catch (ArgumentException) { /* 忽略无效正则 */ }
            }

            // 3. 检查危险命令字符串
            foreach (var command in DangerousCommands)
            {
                if (ContainsIgnoreCase(code, command))
                {
                    result.IsValid = false;
                    result.BlockedItems.Add($"危险命令: {command}");
                }
            }

            result.Message = result.IsValid
                ? "Python代码安全校验通过"
                : $"Python代码安全校验失败: 检测到{result.BlockedItems.Count}个危险项";

            return result;
        }
        /// <summary>
        /// 根据Python脚本路径读取并校验代码
        /// </summary>
        /// <param name="scriptPath">Python脚本路径</param>
        /// <returns>校验结果</returns>
        public static ValidationResult ValidatePythonFile(string scriptPath)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                return new ValidationResult { IsValid = false, Message = "脚本路径不能为空" };
            }

            if (!File.Exists(scriptPath))
            {
                return new ValidationResult { IsValid = false, Message = $"脚本文件不存在: {scriptPath}" };
            }

            var extension = Path.GetExtension(scriptPath).ToLowerInvariant();
            if (extension != ".py")
            {
                return new ValidationResult { IsValid = false, Message = $"只支持.py文件，当前文件类型: {extension}" };
            }

            try
            {
                var code = File.ReadAllText(scriptPath);
                return ValidatePythonCode(code);
            }
            catch (Exception ex)
            {
                return new ValidationResult { IsValid = false, Message = $"读取文件失败: {ex.Message}" };
            }
        }
        private static bool ContainsIgnoreCase(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }


    /// <summary>
    /// 安全校验结果
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 校验是否通过
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 结果消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 被拦截的危险项列表
        /// </summary>
        public List<string> BlockedItems { get; set; } = new();
    }
}