using kevin.Domain.Configuration;
using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{
    public class TDictionaryBaseDatas
    {
        public static List<TDictionary> Data { get; set; } = new List<TDictionary>()
        {
           new TDictionary() {
               Id=4514140354257227771,Key="上传文件限制50MB",Value="50",
               Type="UploadFileLimit",TenantId=TenantHelper.GetSettingsTenantId().ToTryInt32(),CreateTime = DateTime.Parse("2020-01-01 00:00:01"),
               IsSystem=true,CreateUserId=TUserBaseData.TUsers.FirstOrDefault().Id
           },

        };
    }
}
