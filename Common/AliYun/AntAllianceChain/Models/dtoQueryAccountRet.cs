namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 查询账户-返回值
    /// </summary>
    public class dtoQueryAccountRet : dtoBaseRet
    {
        public AccountData accountData
        {
            get
            {
                return Json.JsonHelper.JSONToObject<AccountData>(data);
            }
        }



        public class AccountData
        {
            public string recovery_key { get; set; }
            public int balance { get; set; }
            public Auth_Map[] auth_map { get; set; }
            public string encryption_key { get; set; }
            public string id { get; set; }
            public int recovery_time { get; set; }
            public int version { get; set; }
            public int status { get; set; }
        }

        public class Auth_Map
        {
            public int value { get; set; }
            public string key { get; set; }
        }

    }
}
