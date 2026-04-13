using Microsoft.Agents.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace kevin.AI.AgentFramework.SkillClass
{
#pragma warning disable MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
    public sealed class UnitConverterSkill : AgentClassSkill<UnitConverterSkill>
    {
        public override AgentSkillFrontmatter Frontmatter { get; } = new(
            "unit-converter",
            "使用倍率因子在常用单位之间进行转换。当被要求转换英里、公里、磅或公斤时，请使用此方法。");

        protected override string Instructions => """
        当用户要求进行单位转换时，使用此技能。
        1. 查看转换表资源以找到正确的系数。
        2. 使用转换脚本，并从表中传递数值和系数。
        3. 清晰地展示结果，同时提供两种单位。
        """;

        [AgentSkillResource("conversion-table")]
        [Description("常见单位换算乘数查找表。")]
        public string ConversionTable => """
        # Conversion Tables
        Formula: **result = value × factor**
        | From       | To         | Factor   |
        |------------|------------|----------|
        | miles      | kilometers | 1.60934  |
        | kilometers | miles      | 0.621371 |
        | pounds     | kilograms  | 0.453592 |
        | kilograms  | pounds     | 2.20462  |
        """;

        [AgentSkillScript("convert")]
        [Description("转换器 将一个值乘以一个转换因子，并将结果以JSON格式返回。")]
        public static string ConvertUnits(double value, double factor)
        {
            double result = Math.Round(value * factor, 4);
            return JsonSerializer.Serialize(new { value, factor, result });
        }
    }

#pragma warning restore MAAI001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
}
