using kevin.CodeGenerator.Dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    return new List<EntityItemDto>();
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

        public async Task<bool> BulidCode(EntityItemDto entityItemDto)
        {
            return true;
        }
    }
}
