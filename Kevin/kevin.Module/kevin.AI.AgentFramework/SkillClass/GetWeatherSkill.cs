using Microsoft.Agents.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.SkillClass
{
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
    public class GetWeatherSkill : AgentClassSkill<GetWeatherSkill>
    {
        public override AgentSkillFrontmatter Frontmatter => new(
            "get-weather",
            "获取查询地点天气信息。当被要求提供天气信息时，请使用此方法。");

        protected override string Instructions => """
        当用户要求获取天气信息时，使用此技能。
        1. 获取查询地点的天气信息。
        2. 清晰地展示结果，包括温度、湿度、风速等信息。
        3. 提供必要的天气预报信息。
        """;

        [AgentSkillScript("GetWeather")]
        [Description("获取查询地点的天气信息。")]
        public static string GetWeather([Required][Description("需要查询天气信息的地点。")] string location)
        {
            Console.WriteLine($"获取天气信息：{location}");
            return $"当前天气信息：{location} 气温 21°C、湿度 60%、风速 3 m/s";
        }
    }
#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
}
