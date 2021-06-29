using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.AliYun
{
    public class DnsHelper
    {
        string accessKeyId = "";
        string accessKeySecret = "";


        public DnsHelper()
        {

        }


        public DnsHelper(string in_accessKeyId, string in_accessKeySecret)
        {
            accessKeyId = in_accessKeyId;
            accessKeySecret = in_accessKeySecret;
        }


        /// <summary>
        /// 添加解析记录
        /// </summary>
        /// <param name="host">主机头</param>
        /// <param name="type">解析类型，["A","AAAA"]</param>
        /// <param name="value">值</param>
        /// <param name="domain">域名</param>
        /// <param name="ttl">生存时间</param>
        /// <returns></returns>
        public bool AddDomainRecord(string host, string type, string value, string domain, long? ttl)
        {

            try
            {
                IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
                DefaultAcsClient client = new DefaultAcsClient(profile);

                var request = new AddDomainRecordRequest();
                request._Value = value;
                request.Type = type;
                request.RR = host;
                request.TTL = ttl;
                request.DomainName = domain;

                var response = client.GetAcsResponse(request);

                return true;

            }
            catch
            {
                return false;
            }
        }

    }
}
