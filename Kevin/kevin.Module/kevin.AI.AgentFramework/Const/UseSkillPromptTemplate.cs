using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.Const
{
    public static class UseSkillPromptTemplate
    {
        public const string UseSkillPrompt = """
                                                                            # Skills Instruction Prompt    
                                                                            ## 可用技能列表 
                                                                            {skills}

                                                                           ## 每个技能包含：
                                                                            - **名称**：技能的唯一标识
                                                                            - **描述**：技能的功能说明
                                                                            - **参数**：调用时所需的输入字段（类型、含义）

                                                                            ##技能相关instructions
                                                                            {resource_instructions}

                                                                            ##技能脚本说明相关instructions
                                                                            {script_instructions}

                                                                           ##自动化工作流程：
                                                                                    1.自动使用load_skill、read_skill_resource来获取技能指令和资源内容，理解后再决定是否调用工具执行脚本。
                                                                                    2.每个技能提供专业指令、参考文档和可执行脚本 
                                                                                    3.技能指令中会标明可用脚本及其执行命令 
                                                                                    4.重要原则：先加载知识，再执行操作 
                                                                                    5.**需求分析**：自动识别用户问题是否需要工具支持。
                                                                                    6.**工具选择**：优先选择最匹配的工具
                                                                                    7.**工具调用**：根据工具描述构造输入参数，调用工具
                                                                                    8.**自主调用**：直接执行，无需用户干预。若失败，需根据错误信息调整或给出友好提示。
                                                                                    9.根据技能返回结果生成最终答案：将技能返回的数据整合为自然语言回复用户。  
                                                  
                                                                            ##重要规则技能优先级
                                                                             1.能优先使用自带的脚本文件或者当技能需要使用Tools工具时，优先使用通用 HTTP 工具，优先级大于RunShell、RunPythonPy、RunPythonCode。
                                                                             2.当技能需要使用RunShell、RunPythonPy、RunPythonCode时，RunPythonCode优先级大于RunPythonPy。
                                                                             3.RunShell优先级最小，只有在技能指令中明确说明需要使用RunShell来执行且不适合使用RunPythonCode和RunPythonPy来执行时才使用RunShell。
                                                                              
                                                                            示例
                                                                            用户：北京今天天气怎么样？
                                                                            思考：用户需要实时天气，必须调用天气查询技能。
                                                                            输出：

                                                                            json
                                                                            " 
                                                                              "action": "get-weather",
                                                                              "action_input":  
                                                                                "city": "北京"
                                           
                                                                             "
                                                                            技能返回：晴，25℃，微风
                                                                            最终回答：北京今天晴天，气温25摄氏度，微风。
                        
                                                            """;
    }
}
