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
        /// 危险命令列表
        /// 包含Shell特殊字符、文件操作命令等
        /// </summary>
        private static readonly string[] DangerousCommands = new[]
        {
            ">:", "|", "&&", "||", ";", "$(", "`",
            "bash -i", "sh -i",
            "python.*-c", "python.*eval", "exec", "eval(", "system(", "popen",
            "subprocess.*shell=true", "os.execl", "os.spawn", "os.system", "fork",
            "pickle", "marshal",
            "__import__", "compile(", "exec(", "open(.*,.*w",
            "chmod", "chown", "useradd", "adduser", "passwd", "sudo", "su ",
            "git clone",
            "/etc/passwd", "/etc/shadow",
            "C:\\Windows\\System32", "C:\\Windows\\SysWOW64",
            "/bin/bash", "/usr/bin/", "/usr/sbin/",
            "2>&1", "0>&1", "1>&1"
        };

        /// <summary>
        /// 危险导入列表
        /// </summary>
        private static readonly string[] DangerousImports = new[]
        {
            "import subprocess",
            "from pickle import", "from marshal import", "from imp import",
            "from importlib import", "__import__(", "importlib."
        };

        /// <summary>
        /// 危险正则模式列表
        /// </summary>
        private static readonly string[] DangerousPatterns = new[]
        {
            @"subprocess\.run\(", @"subprocess\.Popen\(", @"subprocess\.call\(", @"subprocess\.spawn\(",
            @"os\.system\(", @"os\.popen\(", @"os\.execl\(", @"os\.execv\(", @"os\.spawnl\(", @"os\.spawnv\(",
            @"os\.fork\(",
            @"pickle\.load\(", @"pickle\.loads\(", @"marshal\.load\(", @"marshal\.loads\(",
            @"exec\s*\(", @"eval\s*\(", @"compile\s*\(", @"__import__\s*\(",
            @"importlib\.\w+",
            @"eval\s*\(.*input", @"input\s*\(.*\)",
            @"base64\.b64decode\(", @"codecs\.decode\(", @"str\.decode\(",
            @"bytes\(.*\.encode\(\)", @"\.encode\([^)]*\)",
            @"0x[0-9a-fA-F]+", @"\\x[0-9a-fA-F]{2}",
            @"reverse.?shell", @"pentest", @"exploit", @"privilege.?escalat",
            @"CVE-\d{4}-\d{4,}", @"backdoor"
        };

        /// <summary>
        /// 校验Python代码的安全性
        /// </summary>
        /// <param name="code">需要校验的Python代码</param>
        /// <returns>校验结果</returns>
        public static ValidationResult ValidatePythonCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ValidationResult { IsValid = true, Message = "代码为空，校验通过" };
            }

            var result = new ValidationResult();

            foreach (var import in DangerousImports)
            {
                if (ContainsIgnoreCase(code, import))
                {
                    result.IsValid = false;
                    result.BlockedItems.Add($"危险导入: {import}");
                }
            }

            foreach (var pattern in DangerousPatterns)
            {
                var matches = Regex.Matches(code, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    result.IsValid = false;
                    result.BlockedItems.Add($"危险模式: {match.Value}");
                }
            }

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
            return source.Contains(value, StringComparison.OrdinalIgnoreCase);
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