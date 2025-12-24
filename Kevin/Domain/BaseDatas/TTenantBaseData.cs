using Kevin.Common.App;

namespace kevin.Domain.BaseDatas
{
    public class TTenantBaseData
    {
        public static List<TTenant> TTenants { get; set; } = new List<TTenant>()
        {
           new TTenant(TenantHelper.GetSettingsTenantId().ToTryInt32(),"admin",DateTime.Parse("2020-01-01 00:00:01")) { Id = 4514140354251222771}
        };
    }
}
