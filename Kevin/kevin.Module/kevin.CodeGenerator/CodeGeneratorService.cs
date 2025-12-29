using kevin.CodeGenerator.Dto;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace kevin.CodeGenerator
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private CodeGeneratorSetting _config;

        public CodeGeneratorService(IOptionsMonitor<CodeGeneratorSetting> config)
        {
            _config = config.CurrentValue;
        }
        public async Task<List<string>> GetAreaNames()
        {
            return _config.CodeGeneratorItems.Select(t => t.AreaName).ToList();
        }

        public async Task<List<EntityItemDto>> GetAreaNameEntityItems(string areaName)
        {
            var area = _config.CodeGeneratorItems.FirstOrDefault(t => t.AreaName == areaName);
            if (area != default)
            {
                var entityItems = new List<EntityItemDto>();
                var path = "..\\..\\" + area.AreaPath.Trim().Replace(".", "\\");
                // 遍历路径下的所有 .cs 文件 
                if (!Directory.Exists(path))
                {
                    throw new ArgumentException($"CodeGeneratorSetting配置:{areaName}{area.AreaPath}不存在");
                }
                else
                {
                    var csFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                    foreach (var file in csFiles)
                    { 
                        var cus= GetTableAttributes(file);
                        entityItems.Add(new EntityItemDto
                        {
                            AreaName = area.AreaName,
                            EntityName = cus.FirstOrDefault().TableName,
                            Description = $"{file}"
                        }); 
                    } 
                    return entityItems;
                }

            }
            return new List<EntityItemDto>();
        }
        static List<(string ClassName, string TableName)> GetTableAttributes(string filePath)
        {
            var results = new List<(string ClassName, string TableName)>();
            var sourceCode = File.ReadAllText(filePath); 
            // 正则表达式匹配类声明和Table特性 
            string pattern = @"\[Table\(""([^""]*)""\)\]\s*(?:\[[^\]]*\]\s*)*public.*?class\s+(\w+)";
            var matches = Regex.Matches(sourceCode, pattern, RegexOptions.Singleline); 
            foreach (Match match in matches)
            {
                string className = match.Groups[1].Value;
                string tableName = match.Groups[2].Value;
                results.Add((className, tableName));
            } 
            return results;
        }
        /// <summary>
        /// 处理路径
        /// </summary>
        /// <param name="entityItems"></param>
        /// <returns></returns>
        private void ProcessingPath(ref List<EntityItemDto> entityItems)
        {
            if (entityItems.Count > 0)
            {
                var areaName = entityItems.FirstOrDefault().AreaName;
                var area = _config.CodeGeneratorItems.FirstOrDefault(t => t.AreaName == areaName);
                if (area == default)
                {
                    throw new ArgumentException($"CodeGeneratorSetting配置:{areaName}{area.AreaPath}不存在");
                }
                var path = "..\\..\\" + area.AreaPath.Trim().Replace(".", "\\");
                var csFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                foreach (var item in entityItems)
                {
                    item.RpBulidPath = area.RpBulidPath;
                    item.IRpBulidPath = area.IRpBulidPath;
                    item.ServiceBulidPath = area.ServiceBulidPath;
                    item.IServiceBulidPath = area.IServiceBulidPath;
                    //判断是有子文件夹
                    var childFlie = item.Description.Replace(path, "").Replace(item.EntityName + ".cs", "").Replace("\\", ".");
                    if (!string.IsNullOrEmpty(childFlie))
                    {
                        if (childFlie.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                        {
                            childFlie = childFlie.Substring(0, childFlie.Length - 1);
                        }
                        item.RpBulidPath += childFlie;
                        item.IRpBulidPath += childFlie;
                        item.ServiceBulidPath += childFlie;
                        item.IServiceBulidPath += childFlie;
                    }

                }

            }

        }

        public async Task<bool> BulidCode(List<EntityItemDto> entityItems)
        {
            if (entityItems.Count > 0)
            {
                ProcessingPath(ref entityItems);
                //获取对应的模板文件
                var iRpTemplate = GetBuildCodeTemplate("IRp");
                var rpTemplate = GetBuildCodeTemplate("Rp");
                var iServiceTemplate = GetBuildCodeTemplate("IService");
                var service = GetBuildCodeTemplate("Service");

                foreach (var item in entityItems)
                {
                    var tEntityName = item.EntityName;
                    if (item.EntityName.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                    {
                        item.EntityName = item.EntityName.Substring(1);
                    }
                    var entityNamespace = item.Description.Replace("..\\..\\", "").Replace("\\" + tEntityName + ".cs", "").Replace("\\", ".");
                    WriteCode(new Dictionary<string, string>
                    {
                        {"%entityName%",item.EntityName},
                        {"%entityNamespace%", entityNamespace},
                        {"%namespacePath%",item.IRpBulidPath}
                    }, iRpTemplate, $"../../{item.IRpBulidPath.Trim().Replace(".", "\\")}\\I{item.EntityName}Rp.cs");
                    WriteCode(new Dictionary<string, string>
                    {
                        {"%entityName%",item.EntityName},
                        {"%entityNamespace%", entityNamespace},
                        {"%iRpBulidNamespace%", item.IRpBulidPath},
                        {"%namespacePath%",item.RpBulidPath}
                    }, rpTemplate, $"../../{item.RpBulidPath.Trim().Replace(".", "\\")}\\{item.EntityName}Rp.cs");
                    WriteCode(new Dictionary<string, string>
                    {
                        {"%entityName%",item.EntityName},
                        {"%entityNamespace%", entityNamespace},
                        {"%namespacePath%",item.IServiceBulidPath}
                    }, iServiceTemplate, $"../../{item.IServiceBulidPath.Trim().Replace(".", "\\")}\\I{item.EntityName}Service.cs");
                    WriteCode(new Dictionary<string, string>
                    {
                        {"%entityName%",item.EntityName},
                        {"%entityNamespace%", entityNamespace},
                        {"%iRpBulidNamespace%", item.IRpBulidPath},
                        {"%iServiceBulidNamespace%", item.IServiceBulidPath},
                        {"%namespacePath%",item.ServiceBulidPath}
                    }, service, $"../../{item.ServiceBulidPath.Trim().Replace(".", "\\")}\\{item.EntityName}Service.cs");


                }
            }

            return true;
        }

        /// <summary>
        /// 获取对应模板文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetBuildCodeTemplate(string name)
        {
            return File.ReadAllText("..\\..\\" + "Kevin\\kevin.Module\\kevin.CodeGenerator\\BuildCodeTemplate\\" + name + ".txt", encoding: Encoding.UTF8);
        }
        /// <summary>
        /// 生成文件和代码
        /// </summary>
        /// <param name="paramters"></param>
        /// <param name="content"></param>
        /// <param name="savePath"></param>
        private void WriteCode(Dictionary<string, string> paramters, string content, string savePath)
        {
            foreach (var item in paramters)
            {
                content = content.Replace(item.Key, item.Value);
            }
            var dir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            //判断是否存在相同文件
            var csFiles = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
            bool fileExists = false;
            foreach (var file in csFiles)
            {
                if (Path.GetFileName(file) == Path.GetFileName(savePath))
                {
                    fileExists = true;
                }
            }
            if (fileExists)
            {
                Console.WriteLine($"文件{savePath}已存在，跳过生成！");
            }
            else
            {
                File.WriteAllText(savePath, content, Encoding.UTF8);
            }

        }
    }
}
