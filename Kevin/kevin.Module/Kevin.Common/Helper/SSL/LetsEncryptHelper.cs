using Certes;
using Certes.Acme;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.SSL
{
    public static class LetsEncryptHelper
    {




        /// <summary>
        /// 创建SSL证书
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="sslPath"></param>
        /// <remarks>Common.SSL.LetsEncryptHelper.CreateSSL("*.domain.com").Wait();</remarks>
        public async static Task CreateSSL(string domainName, string sslPath)
        {
            var basePath = "D:/SSL/";

            if (!Directory.Exists(basePath + "IIS/"))
            {
                Directory.CreateDirectory(basePath + "IIS/");
            }
            if (!Directory.Exists(basePath + "Nginx/"))
            {
                Directory.CreateDirectory(basePath + "Nginx/");
            }

            var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);
            var account = await acme.NewAccount("xxx@qq.com", true);

            var pemKey = acme.AccountKey.ToPem();

            var order = await acme.NewOrder(new[] { domainName });

            var authz = (await order.Authorizations()).First();
            var dnsChallenge = await authz.Dns();
            var dnsTxt = acme.AccountKey.DnsTxt(dnsChallenge.Token);


            SetDomainTxt(domainName, dnsTxt);
            Thread.Sleep(5000);


            await dnsChallenge.Validate();

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.RS256);
            var cert = await order.Generate(new CsrInfo { }, privateKey);



            var fileName = domainName.Replace("*", "_");


            //CreateIIS
            var keystorePass = GuidHelper.Reduce(Guid.NewGuid()).Replace("-", "");
            var pfxBuilder = cert.ToPfx(privateKey);
            var pfx = pfxBuilder.Build(domainName, keystorePass);
            File.WriteAllBytes(basePath + "/IIS/" + fileName + ".pfx", pfx);
            File.WriteAllText(basePath + "/IIS/keystorePass_" + fileName + ".txt", keystorePass);


            //CreateNginx
            var certPem = cert.ToPem();
            var privateKeyPem = privateKey.ToPem();
            File.WriteAllText(basePath + "/Nginx/" + fileName + ".crt", certPem);
            File.WriteAllText(basePath + "/Nginx/" + fileName + ".key", privateKeyPem);

        }





        /// <summary>
        /// 调用 DNS 解析方法，添加TXT 验证记录
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <param name="txtValue">TXT值</param>
        private static void SetDomainTxt(string domainName, string txtValue)
        {

            var fulldomainArray = domainName.Split(".");

            var onedomain = fulldomainArray[fulldomainArray.Length - 2] + "." + fulldomainArray[fulldomainArray.Length - 1];

            var host = "_acme-challenge." + domainName.Replace("." + onedomain, "");

            if (host.Contains("*"))
            {
                host = host.Replace(".*", "");
            }


            var dnsHelper = new AliYun.DnsHelper("", "");

            dnsHelper.AddDomainRecord(host, "TXT", txtValue, onedomain, 600);

        }


    }
}
