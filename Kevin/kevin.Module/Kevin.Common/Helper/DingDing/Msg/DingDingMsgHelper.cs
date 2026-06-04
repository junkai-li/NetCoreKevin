using Kevin.Common.App.Global;
using Kevin.Common.Extension;
using Kevin.HttpApiClients.Helper;
using Kevin.log4Net;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Util.Helper.DingDing.RQ;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.Helper.DingDing.Msg
{
    public class DingDingMsgHelper
    {
        /// <summary>
        /// 钉钉默认解决消息幂等
        /// </summary>
        public const string MESSAGE_API = "https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2?access_token=";
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DingDingMsgHelper));

        string AgentId { get; set; }
        string appKey { get; set; }
        string appSecret { get; set; }

        public DingDingMsgHelper(string appKey = null, string appSecret = null, string AgentId = null)
        {
            this.AgentId = AgentId ?? ConfigHelper.Configuration["DingDingMsgInfo:dingdingAgentId"];
            this.appKey = appKey ?? ConfigHelper.Configuration["DingDingMsgInfo:appKey"];
            this.appSecret = appSecret ?? ConfigHelper.Configuration["DingDingMsgInfo:appSecret"];
        }

        /// <summary>
        /// 机器人发送消息给用户
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public void RobotSendTextMessageToUsers(string robotCode, List<string> userIds, string content)
        {
            var dingTalkRequest = new
            {
                robotCode,
                userIds, // 纯集合类型
                msgKey = "sampleText",
                msgParam = "{\"content\": \"" + content + "\"}"
            };
            var token = GetAccessToken();
            var url = "https://api.dingtalk.com/v1.0/robot/oToMessages/batchSend";
            Dictionary<string, string> dictHead = new Dictionary<string, string>();
            dictHead.Add("x-acs-dingtalk-access-token", token);
            var str_result = HttpClientHelper.CreatePostHttpResponse(url, dictHead, dingTalkRequest.ToJson());
            var Jresult = str_result.ToObject<dynamic>();
            string processQueryKey = Jresult?.processQueryKey?.ToString()?.Trim();
            // 2. 判断是否为空（null/空字符串/全空格都视为失败）
            if (string.IsNullOrEmpty(processQueryKey))
            {
                // 抛出自定义异常，携带明确的错误信息
                throw new Exception($"机器人发送消息给用户失败：robotCode={robotCode},userIds={userIds.ToJson()},content={content}");
            }
        }


        /// <summary>
        /// 工作通知消息（钉钉URL）
        /// </summary>
        /// <returns></returns>
        public DingDingResult SendMessage_ByDingDingURL(string UseridList, string Content, string MessageUrl, List<DingDingKvFrom> forms = null)
        {

            forms = forms ?? new List<DingDingKvFrom>();
            var accessToken = GetAccessToken();
            var bodyParam = new
            {
                agent_id = AgentId,
                userid_list = UseridList,
                to_all_user = false,
                msg = new
                {
                    msgtype = "oa",
                    oa = new
                    {
                        message_url = MessageUrl,
                        head = new
                        {
                            bgcolor = "417DEB",
                            //text = "头部标题"
                        },
                        body = new
                        {
                            content = "",
                            title = Content,
                            form = forms,
                        }
                    },
                }
            }.ToJson();
            var str_result = HttpClientHelper.CreatePostHttpResponse("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2?access_token=" + accessToken, bodyParam);
            var Jresult = str_result.ToObject<DingDingResult>();
            if (!Jresult.isSuccess)
            {
                log.Error($"钉钉消息发送失败  {str_result}");
            }
            ;
            return Jresult;
        }


        /// <summary>
        /// TEXT通知消息（钉钉URL）
        /// </summary>
        /// <returns></returns>
        public DingDingResult SendMessage_ByDingDingURLText(string UseridList, string Content)
        {
            var accessToken = GetAccessToken();
            var bodyParam = new
            {
                agent_id = AgentId,
                userid_list = UseridList,
                to_all_user = false,
                msg = new
                {
                    msgtype = "text",
                    text = new
                    {
                        content = Content
                    }
                }
            }.ToJson();
            var str_result = HttpClientHelper.CreatePostHttpResponse("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2?access_token=" + accessToken, bodyParam);
            var Jresult = str_result.ToObject<DingDingResult>();
            if (!Jresult.isSuccess)
            {
                log.Error($"钉钉消息发送失败，【{Content}】  {str_result}");
            }
            ;
            return Jresult;
        }
        /// <summary>
        /// 发布-可以整体跳转的消息
        /// </summary>
        /// <param name="dingTalk_ID">钉钉的用户ID</param>
        /// <param name="Content">消息内容  (\n 左右两边 空2格 即可换行)</param>
        /// <param name="redirectUrl">点击消息重定向到URL</param>
        /// <param name="forms"></param>
        /// <param name="environment">环境</param>
        public async Task Publish_actionCard(string dingTalk_ID, string Content, string redirectUrl)
        {
            //构建请求模型
            var msg_action_card = new Publish_actionCard_action_card_Msg_action_card
            {
                title = "测试标题",
                single_title = "查看详情",
                single_url = redirectUrl,
                markdown = Content
            };

            var action_card_Msg = new Publish_actionCard_action_card_Msg(msg_action_card) { msgtype = "action_card" };
            var buildRequestModel = new RQ_DingTalk_action_card(action_card_Msg)
            {
                agent_id = AgentId ?? ConfigHelper.Configuration["DingDingMsgInfo:dingdingAgentId"], //拿取配置文件上的钉钉ID
                userid_list = dingTalk_ID
            };
            var token = GetAccessToken();
            var body = buildRequestModel.ToJson();
            var str_result = HttpClientHelper.Post(MESSAGE_API + token, body, "json");
            var Jresult = str_result.ToObject<DingDingResult>();
            if (!Jresult.isSuccess) log.Error($"钉钉消息发送失败  {str_result}");
        }

        #region 上传文件消息

        /// <summary>
        /// 发送文件信息
        /// </summary>
        /// <returns></returns>
        public DingDingResult Publish_file(string token, string user_id, string media_id)
        {
            var body = new
            {
                agent_id = AgentId ?? ConfigHelper.Configuration["DingDingMsgInfo:dingdingAgentId"], //拿取配置文件上的应用ID
                userid_list = user_id,
                msg = new
                {
                    msgtype = "file",
                    file = new { media_id }
                }
            }.ToJson();
            var str_result = HttpClientHelper.Post("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2?access_token=" + token, body, "json");
            return str_result.ToObject<DingDingResult>();
        }
        #endregion
        public string GetAccessToken()
        {
            string cacheKey = "dingding.accessToken" + appKey;
            var cacheser = GlobalServices.ServiceProvider.GetService<ICacheService>();
            var token = cacheser.GetString(cacheKey);
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }
            appKey = appKey ?? ConfigHelper.Configuration["DingDingMsgInfo:appKey"];
            appSecret = appSecret ?? ConfigHelper.Configuration["DingDingMsgInfo:appSecret"];
            var dictParam = new
            {
                appKey,
                appSecret
            }.EntityToJson();
            var str_result = HttpClientHelper.CreatePostHttpResponse("https://api.dingtalk.com/v1.0/oauth2/accessToken", dictParam);
            var jresult = str_result.ToObject<JObject>();
            var accessToken = jresult["accessToken"].ToString();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                cacheser.SetString(cacheKey, accessToken, TimeSpan.FromHours(1));
            }
            return accessToken;
        }
        /// <summary>
        /// 获取钉钉的js鉴权码
        /// </summary>
        /// <returns></returns>
        public (bool success, string result) GetJsapi_ticket()
        {
            const string url = "https://oapi.dingtalk.com/get_jsapi_ticket";
            var token = GetAccessToken();
            var api = url + $"?access_token={token}";
            var response = HttpClientHelper.Get(api);
            var response_model = response.ToObject<DingTalkTicket>();
            if (response_model.errcode != 0) return (false, response_model.errmsg);
            return (true, response_model.ticket);
        }
        /// <summary>
        /// 通过免登码获取钉钉用户信息 (面登码请求后 会标记失效，需要重新获取， 存活时间5分钟)
        /// </summary>
        /// <param name="freeCode"></param>
        /// <returns></returns>
        public (bool success, string result) GetUserInfoByFreeCode(string freeCode)
        {
            var token = GetAccessToken();
            string url = $"https://oapi.dingtalk.com/topapi/v2/user/getuserinfo?access_token={token}";
            var body = (new { code = freeCode }).ToJson();
            var response = HttpClientHelper.Post(url, body, "json");
            var response_dto = response.ToObject<JObject>();
            var code = response_dto["errcode"].ToInt64();
            if (code != 0) return (false, response_dto["errmsg"].ToString());
            return (true, response);
        }
        #region 钉钉小程序签名
        public (bool success, string msg, DingTalkSign result) GetDingTalkSign()
        {
            var ticket = GetJsapi_ticket();
            if (!ticket.success) return (false, ticket.result, null);
            var settings = new DingTalkSign() { signature = ticket.result };
            var queryString = settings.GetQueryString(); 

            var sha = new SHA1CryptoServiceProvider();
            var enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(queryString);
            byte[] dataHashed = sha.ComputeHash(dataToHash);
            var hash = BitConverter.ToString(dataHashed).Replace("-", "");
            hash = hash.ToLower();
            settings.signature = hash;
            return (true, null, settings);
        }
        #endregion  
    }
    public class DingTalkTicket
    {
        public int errcode { get; set; }
        public string ticket { get; set; }
        public string errmsg { get; set; }
        public string expires_in { get; set; }
    }
    public class DingTalk_jsAPI_config
    {

    }
    public sealed class DingTalkSign
    {
        readonly string url = ConfigHelper.Configuration["DingDingMsgInfo:microApplicationHost"];
        public readonly string nonceStr = "123456";
        public readonly string agentId = ConfigHelper.Configuration["DingDingMsgInfo:dingdingAgentId"];
        public readonly string timeStamp = ConfigHelper.Configuration["DingDingMsgInfo:timeStamps"];
        public readonly string corpId = ConfigHelper.Configuration["DingDingMsgInfo:corpId"];
        public string signature = null;
        public string GetQueryString() => $"jsapi_ticket={signature}&noncestr={nonceStr}&timestamp={timeStamp}&url={@$"{url}/"}";
    }
    public class DingDingKvFrom
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class DingDingResult
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string task_id { get; set; }
        public string request_id { get; set; }

        public bool isSuccess { get { return errcode == "0"; } }
    }




}
namespace NetCore.Util.Helper.DingDing.RQ
{
    public abstract class action_card_Msg { }
    public abstract class action_card_Msg_action_card { }
    /// <summary>
    /// 钉钉action_card消息请求类型
    /// </summary>
    public class RQ_DingTalk_action_card
    {
        public RQ_DingTalk_action_card(action_card_Msg msgModel) => msg = msgModel;
        public string agent_id { get; set; }
        public string userid_list { get; set; }
        public action_card_Msg msg { get; set; }
    }

    #region 可跳转【普通】消息模型
    public class Publish_actionCard_action_card_Msg : action_card_Msg
    {
        public Publish_actionCard_action_card_Msg(action_card_Msg_action_card msgModel) => action_card = msgModel;
        public string msgtype { get; set; }
        public action_card_Msg_action_card action_card { get; set; }
    }
    public class Publish_actionCard_action_card_Msg_action_card : action_card_Msg_action_card
    {
        public string title { get; set; }
        public string single_title { get; set; }
        public string markdown { get; set; }
        public string single_url { get; set; }
    }
    #endregion
}