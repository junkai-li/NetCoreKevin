namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 交易查询返回值
    /// </summary>
    public class dtoQueryTransActionRet : dtoBaseRet
    {


        public TransData transData
        {
            get
            {
                return Json.JsonHelper.JSONToObject<TransData>(data);
            }
        }





        public class TransData
        {
            public int blockNumber { get; set; }
            public Transactiondo transactionDO { get; set; }
        }

        public class Transactiondo
        {
            public string data { get; set; }


            public string dataDecode
            {
                get
                {
                    return CryptoHelper.Base64Decode(data);
                }
            }

            public long timestamp { get; set; }
        }

    }





}
