using kevin.CodeGenerator.Dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;


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
                        // 读取文件内容
                        var code = File.ReadAllText(file);
                        var tree = CSharpSyntaxTree.ParseText(code);
                        var root = (CompilationUnitSyntax)tree.GetRoot();

                        // 查找所有类声明
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDeclaration in classDeclarations)
                        {
                            // 检查类是否有 Table 特性
                            if (classDeclaration.AttributeLists.Any(list =>
                                list.Attributes.Any(attr =>
                                    attr.Name.ToString() == "Table")))
                            {
                                string description = "";
                                // 检查类是否有 Description 特性
                                var descriptionAttr = classDeclaration.AttributeLists
                                    .SelectMany(list => list.Attributes)
                                    .FirstOrDefault(attr => attr.Name.ToString() == "Description");

                                if (descriptionAttr != null)
                                {
                                    // 获取特性参数值
                                    var arg = descriptionAttr.ArgumentList?.Arguments.FirstOrDefault();
                                    if (arg?.Expression is LiteralExpressionSyntax literal)
                                    {
                                        description = literal.Token.ValueText;
                                    }
                                }
                                entityItems.Add(new EntityItemDto
                                {
                                    AreaName = area.AreaName,
                                    EntityName = classDeclaration.Identifier.Text,
                                    Description = $"{file}: {description}"
                                });

                            }
                        }
                    }

                    return entityItems;
                }

            }
            return new List<EntityItemDto>();
        }

        public async Task<bool> BulidCode(List<EntityItemDto> entityItems)
        {
            //获取对应的模板文件
            var iRpTemplate = GetBuildCodeTemplate("IRp");
            var rpTemplate = GetBuildCodeTemplate("Rp");
            var iServiceTemplate = GetBuildCodeTemplate("IService");
            var service = GetBuildCodeTemplate("Service");
            foreach (var item in entityItems)
            {
                var area = _config.CodeGeneratorItems.FirstOrDefault(t => t.AreaName == item.AreaName);
                if (area != default)
                {
                    if (item.EntityName.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                    {
                        item.EntityName = item.EntityName.Substring(1);
                    }
                    WriteCode(new Dictionary<string, string>
                    {
                        {  "%entityName%",item.EntityName},
                        {  "%namespacePath%",area.IRpBulidPath}
                    }, iRpTemplate, $"../../{area.IRpBulidPath.Trim().Replace(".", "\\")}/I{item.EntityName}Rp.cs");
                    WriteCode(new Dictionary<string, string>
                    {
                        {  "%entityName%",item.EntityName},
                        {  "%namespacePath%",area.RpBulidPath}
                    }, rpTemplate, $"../../{area.RpBulidPath.Trim().Replace(".", "\\")}/{item.EntityName}Rp.cs");
                    WriteCode(new Dictionary<string, string>
                    {
                        {  "%entityName%",item.EntityName},
                        {  "%namespacePath%",area.IServiceBulidPath}
                    }, iServiceTemplate, $"../../{area.IServiceBulidPath.Trim().Replace(".", "\\")}/I{item.EntityName}Service.cs");
                    WriteCode(new Dictionary<string, string>
                    {
                        {  "%entityName%",item.EntityName},
                        {  "%namespacePath%",area.ServiceBulidPath}
                    }, service, $"../../{area.ServiceBulidPath.Trim().Replace(".", "\\")}/{item.EntityName}Service.cs");
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
