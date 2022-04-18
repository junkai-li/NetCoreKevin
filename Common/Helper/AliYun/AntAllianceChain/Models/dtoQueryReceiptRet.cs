namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 查询交易回执返回值
    /// </summary>
    public class dtoQueryReceiptRet : dtoBaseRet
    {
       

        public ReceiptData receiptData
        {
            get
            {
                return Json.JsonHelper.JSONToObject<ReceiptData>(data);
            }
        }


        public class ReceiptData
        {
            public int blockNumber { get; set; }
            public int gasUsed { get; set; }
            public Log[] logs { get; set; }
            public string output { get; set; }
            public int result { get; set; }
        }

        public class Log
        {
            public From from { get; set; }
            public string logData { get; set; }
            public To to { get; set; }
            public string[] topics { get; set; }
        }

        public class From
        {
            public string data { get; set; }
            public bool empty { get; set; }
            public string value { get; set; }
        }

        public class To
        {
            public string data { get; set; }
            public bool empty { get; set; }
            public string value { get; set; }
        }


    }


}
