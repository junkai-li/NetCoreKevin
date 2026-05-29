using kevin.Domain.Configuration;
using kevin.Domain.Entities.AI;
using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{
    public class TAIPromptsBaseDatas
    {
        public static List<TAIPrompts> Data { get; set; } = new List<TAIPrompts>()
        {
             new TAIPrompts() {
               Id=4514141254257227771,
               Name="AI工具智能体提示词",
               Prompt=@"你是一个能调用工具的智能助手。 
                            规则：
                            1. 能用工具解决的问题必须调用工具，禁止编造。
                            2. 工具定义由系统注入，严格按照要求的参数格式调用。
                            3. 收到工具结果后，用自然语言总结回复，不输出原始数据。
                            4. 信息不足时先向用户确认，再调用工具。
                            5. 工具报错就如实说明，并建议替代方案。 
                            输出：直接给出最终答案，简洁、准确，必要时用 markdown 排版。",
               Description="AI工具智能体提示词",
               TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),
               CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               CreateUserId=TUserBaseData.TUsers[0].Id
           },
        };
    }
}
