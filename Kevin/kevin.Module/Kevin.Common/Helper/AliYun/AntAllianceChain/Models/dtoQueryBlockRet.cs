namespace Common.AliYun.AntAllianceChain.Models
{


    /// <summary>
    /// 查询块头返回值
    /// </summary>
    public class dtoQueryBlockRet:dtoBaseRet
    {

        public BlockData blockData
        {
            get
            {
                return Json.JsonHelper.JSONToObject<BlockData>(data);
            }
        }


        public class BlockData
        {
            public Block block { get; set; }
        }

        public class Block
        {
            public Blockheader blockHeader { get; set; }
        }

        public class Blockheader
        {
            public int gasUsed { get; set; }
            public string hash { get; set; }
            public string logBloom { get; set; }
            public int number { get; set; }
            public string parentHash { get; set; }
            public string receiptRoot { get; set; }
            public string stateRoot { get; set; }
            public long timestamp { get; set; }
            public string transactionRoot { get; set; }
            public int version { get; set; }
        }

    }

}
