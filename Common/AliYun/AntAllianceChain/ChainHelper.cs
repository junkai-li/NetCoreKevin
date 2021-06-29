using Common.AliYun.AntAllianceChain.Models;
using Common.Json;
using System;

namespace Common.AliYun.AntAllianceChain
{


    /// <summary>
    /// 蚂蚁开放联盟链帮助类
    /// </summary>
    public class ChainHelper
    {

        string accessId = "";
        string accessKeyPath = "";
        string bizId = "";
        string tenantId = "RDASAIQS";



        public ChainHelper()
        {

        }



        public ChainHelper(string in_AccessId, string in_AccessKeyPath, string in_BizId, string in_TenantId)
        {
            accessId = in_AccessId;
            accessKeyPath = in_AccessKeyPath;
            bizId = in_BizId;
            tenantId = in_TenantId;
        }



        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {

            var key = accessId + "_token";

            var token = Common.RedisHelper.StringGet(key);

            if (string.IsNullOrEmpty(token))
            {
                var sendData = new dtoGetToken();

                sendData.accessId = accessId;
                sendData.time = DateTimeHelper.TimeToJs(DateTime.Now).ToString();

                string msg = sendData.accessId + sendData.time;

                var accessKey = System.IO.File.ReadAllText(accessKeyPath);

                sendData.secret = CryptoHelper.HexRSASha256Sign(msg, accessKey);

                var sendDataStr = JsonHelper.ObjectToJSON(sendData);

                var ret = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/shakeHand", sendDataStr, "json");

                token = JsonHelper.GetValueByKey(ret, "data");

                RedisHelper.StringSet(key, token, TimeSpan.FromMilliseconds(28));
            }

            return token;
        }




        /// <summary>
        /// 存证
        /// </summary>
        /// <param name="orderId">业务方请求唯一标识，用于重试去重</param>
        /// <param name="account">链上账户名</param>
        /// <param name="content">存证内容</param>
        /// <param name="mykmsKeyId">创建链上账户时使用的 mykmsKeyId</param>
        /// <returns></returns>
        public dtoBaseRet Deposit(string orderId, string account, string content, string mykmsKeyId)
        {
            var deposit = new dtoDeposit();

            deposit.orderId = orderId;
            deposit.account = account;
            deposit.mykmsKeyId = mykmsKeyId;
            deposit.content = content;


            deposit.tenantid = tenantId;
            deposit.bizid = bizId;
            deposit.accessId = accessId;
            deposit.token = GetToken();
            deposit.gas = 100000;

            var depositStr = JsonHelper.ObjectToJSON(deposit);

            var depositRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCallForBiz", depositStr, "json");

            var baseRet = JsonHelper.JSONToObject<dtoBaseRet>(depositRetStr);

            return baseRet;
        }



        /// <summary>
        /// 调用合约
        /// </summary>
        /// <param name="orderId">业务方请求唯一标识，用于重试去重。</param>
        /// <param name="account">链上账户名</param>
        /// <param name="contractName">合约名</param>
        /// <param name="methodSignature">方法签名,例如：sayName(string)</param>
        /// <param name="inputParamListStr">实参列表，例如：[\"zhangx11iaodong\"]</param>
        /// <param name="outTypes">返回参数列表，例如：[\"string\"]</param>
        /// <param name="mykmsKeyId">链上账户对应的 mykmsKeyId</param>
        public dtoBaseRet CallSolidity(string orderId, string account, string contractName, string methodSignature, string inputParamListStr, string outTypes, string mykmsKeyId)
        {
            var callSolidity = new dtoCallSolidity();

            callSolidity.orderId = orderId;
            callSolidity.account = account;
            callSolidity.contractName = contractName;
            callSolidity.methodSignature = methodSignature;
            callSolidity.inputParamListStr = inputParamListStr;
            callSolidity.outTypes = outTypes;
            callSolidity.mykmsKeyId = mykmsKeyId;

            callSolidity.bizid = bizId;
            callSolidity.accessId = accessId;
            callSolidity.token = GetToken();
            callSolidity.gas = 100000;
            callSolidity.tenantid = tenantId;

            var callSolidityStr = JsonHelper.ObjectToJSON(callSolidity);

            var callSolidityRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCallForBiz", callSolidityStr, "json");

            var callSolidityRet = JsonHelper.JSONToObject<dtoBaseRet>(callSolidityRetStr);

            return callSolidityRet;
        }



        /// <summary>
        /// 查询交易
        /// </summary>
        public dtoQueryTransActionRet QueryTransAction(string hash)
        {
            var queryTransAction = new dtoQueryTransAction();

            queryTransAction.method = "QUERYTRANSACTION";
            queryTransAction.bizid = bizId;
            queryTransAction.hash = hash;
            queryTransAction.accessId = accessId;
            queryTransAction.token = GetToken();

            var queryTransActionStr = JsonHelper.ObjectToJSON(queryTransAction);

            var queryTransActionRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCall", queryTransActionStr, "json");

            var queryTransActionRet = JsonHelper.JSONToObject<dtoQueryTransActionRet>(queryTransActionRetStr);

            return queryTransActionRet;
        }



        /// <summary>
        /// 查询交易回执
        /// </summary>
        public dtoQueryReceiptRet QueryReceipt(string hash)
        {
            var queryTransAction = new dtoQueryTransAction();

            queryTransAction.method = "QUERYRECEIPT";
            queryTransAction.bizid = bizId;
            queryTransAction.hash = hash;
            queryTransAction.accessId = accessId;
            queryTransAction.token = GetToken();


            var queryTransActionStr = JsonHelper.ObjectToJSON(queryTransAction);

            var queryTransActionRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCall", queryTransActionStr, "json");

            var queryReceiptRet = JsonHelper.JSONToObject<dtoQueryReceiptRet>(queryTransActionRetStr);

            return queryReceiptRet;
        }



        /// <summary>
        /// 查询块头
        /// </summary>
        public dtoQueryBlockRet QueryBlock(int blockNumber)
        {

            var queryBlock = new dtoQueryBlock();

            queryBlock.bizid = bizId;
            queryBlock.method = "QUERYBLOCK";
            queryBlock.requestStr = blockNumber;
            queryBlock.accessId = accessId;
            queryBlock.token = GetToken();

            var queryBlockStr = JsonHelper.ObjectToJSON(queryBlock);

            var queryBlockRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCall", queryBlockStr, "json");

            var queryBlockRet = JsonHelper.JSONToObject<dtoQueryBlockRet>(queryBlockRetStr);

            return queryBlockRet;

        }



        /// <summary>
        /// 查询块体
        /// </summary>
        public dtoQueryBlockBodyRet QueryBlockBody(int blockNumber)
        {
            var queryBlock = new dtoQueryBlock();

            queryBlock.bizid = bizId;
            queryBlock.method = "QUERYBLOCKBODY";
            queryBlock.requestStr = blockNumber;
            queryBlock.accessId = accessId;
            queryBlock.token = GetToken();


            var queryBlockStr = JsonHelper.ObjectToJSON(queryBlock);

            var queryBlockRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCall", queryBlockStr, "json");

            var queryBlockRet = JsonHelper.JSONToObject<dtoQueryBlockBodyRet>(queryBlockRetStr);

            return queryBlockRet;
        }



        /// <summary>
        /// 查询最新块高
        /// </summary>
        public dtoQueryBlockRet QueryLastBlock()
        {
            var queryLastBlock = new dtoQueryLastBlock();

            queryLastBlock.bizid = bizId;
            queryLastBlock.method = "QUERYLASTBLOCK";
            queryLastBlock.accessId = accessId;
            queryLastBlock.token = GetToken();

            var queryLastBlockStr = JsonHelper.ObjectToJSON(queryLastBlock);

            var queryLastBlockRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCall", queryLastBlockStr, "json");

            var queryLastBlockRet = JsonHelper.JSONToObject<dtoQueryBlockRet>(queryLastBlockRetStr);

            return queryLastBlockRet;
        }



        /// <summary>
        /// 查询账户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public dtoQueryAccountRet QueryAccount(string account)
        {
            var queryAccount = new dtoQueryAccount();

            queryAccount.requestStr = "{\"queryAccount\":\"" + account + "\"}";

            queryAccount.bizid = bizId;
            queryAccount.accessId = accessId;
            queryAccount.token = GetToken();

            var queryAccountStr = JsonHelper.ObjectToJSON(queryAccount);

            var queryAccountRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCall", queryAccountStr, "json");

            var queryAccountRet = JsonHelper.JSONToObject<dtoQueryAccountRet>(queryAccountRetStr);

            return queryAccountRet;
        }



        /// <summary>
        /// 解析合约参数
        /// </summary>
        /// <param name="orderId">业务方请求唯一标识，用于重试去重。</param>
        /// <param name="content">receipt的output字段进行hex编码，合约执行结果的二进制表示</param>
        /// <param name="abi">解析格式，json表示，比如 [\”bool\”,\”int\”,\”int[]\”]</param>
        /// <param name="account">链上账户名</param>
        /// <param name="mykmsKeyId">链上账户对应的 mykmsKeyId</param>
        /// <returns></returns>
        public dtoBaseRet ParseOutPut(string orderId, string content, string abi, string account, string mykmsKeyId)
        {

            byte[] contentBytes = Convert.FromBase64String(content);
            content = CryptoHelper.BytesToHex(contentBytes);

            var parseOutPut = new dtoParseOutPut();

            parseOutPut.bizid = bizId;
            parseOutPut.tenantid = tenantId;
            parseOutPut.orderId = orderId;
            parseOutPut.vmTypeEnum = "EVM";
            parseOutPut.content = content;
            parseOutPut.abi = abi;
            parseOutPut.mykmsKeyId = mykmsKeyId;
            parseOutPut.token = GetToken();
            parseOutPut.accessId = accessId;
            parseOutPut.account = account;

            var parseOutPutStr = JsonHelper.ObjectToJSON(parseOutPut);

            var parseOutPutRetStr = HttpHelper.Post("https://rest.baas.alipay.com/api/contract/chainCallForBiz", parseOutPutStr, "json");

            var parseOutPutRet = JsonHelper.JSONToObject<dtoBaseRet>(parseOutPutRetStr);

            return parseOutPutRet;
        }
    }
}
