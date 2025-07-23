namespace Common.AliYun.AntAllianceChain.Models
{

    /// <summary>
    /// 查询块体返回值
    /// </summary>
    public class dtoQueryBlockBodyRet
    {

        public bool success { get; set; }

        public string code { get; set; }

        public string data { get; set; }


        public BlockBodyData blockBodyData
        {
            get
            {
                return Json.JsonHelper.JSONToObject<BlockBodyData>(data);
            }
        }


        public class BlockBodyData
        {
            public Header header { get; set; }
            public Body body { get; set; }
        }

        public class Header
        {
            public int number { get; set; }
            public string transaction_root { get; set; }
            public int gas_used { get; set; }
            public long version { get; set; }
            public string receipt_root { get; set; }
            public string hash { get; set; }
            public string parent_hash { get; set; }
            public string state_root { get; set; }
            public long timestamp { get; set; }
            public string log_bloom { get; set; }
        }

        public class Body
        {
            public Receipt_List[] receipt_list { get; set; }
            public string consensus_proof { get; set; }
            public Transaction_List[] transaction_list { get; set; }
        }

        public class Receipt_List
        {
            public int result { get; set; }
            public string output { get; set; }
            public int gas_used { get; set; }
            public Log[] logs { get; set; }
        }

        public class Log
        {
            public string[] topics { get; set; }
            public string from { get; set; }
            public string log_data { get; set; }
            public string to { get; set; }
        }

        public class Transaction_List
        {
            public int period { get; set; }
            public string data { get; set; }
            public string[] signature { get; set; }
            public int type { get; set; }
            public long nonce { get; set; }
            public int version { get; set; }
            public object[] extensions { get; set; }
            public string group_id { get; set; }
            public int gas { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public int value { get; set; }
            public string hash { get; set; }
            public long timestamp { get; set; }
        }


    }
}
