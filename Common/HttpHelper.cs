using Models.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace Common
{

    /// <summary>
    /// 常用Http操作类集合
    /// </summary>
    public static class HttpHelper
    {


        /// <summary>
        /// Get方式获取远程资源
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string Get(string url, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (isSkipSslVerification)
            {
                request.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            }

            request.Method = "GET";
            request.Accept = "*/*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            var obj = new
            {
                Result = retString,
                IsSuccess = true
            };
            return retString;
        }




        /// <summary>
        /// Model对象转换为Uri网址参数形式
        /// </summary>
        /// <param name="obj">Model对象</param>
        /// <param name="url">前部分网址</param>
        /// <returns></returns>
        public static string ModelToUriParam(object obj, string url = "")
        {
            PropertyInfo[] propertis = obj.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append(url);
            sb.Append("?");
            foreach (var p in propertis)
            {
                var v = p.GetValue(obj, null);
                if (v == null)
                    continue;

                sb.Append(p.Name);
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(v.ToString()));
                sb.Append("&");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }




        /// <summary>
        /// Post数据到指定url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="type">form,data,json,xml</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string Post(string url, string data, string type, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }

            if (isSkipSslVerification)
            {
                req.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            }

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);
            req.Method = "POST";
            if (type == "form")
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            else if (type == "data")
            {
                req.ContentType = "multipart/form-data";
            }
            else if (type == "json")
            {
                req.ContentType = "application/json";
            }
            else if (type == "xml")
            {
                req.ContentType = "text/xml";
            }

            req.ContentLength = requestBytes.Length;
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
            string PostJie = sr.ReadToEnd();
            sr.Close();
            res.Close();

            return PostJie;
        }




        /// <summary>
        /// Post数据到指定url,异步执行
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="data">数据</param>
        /// <param name="type">form,data,json,xml</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public async static void PostAsync(string url, string data, string type, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }

            if (isSkipSslVerification)
            {
                req.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            }

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(data);

            req.Method = "POST";
            if (type == "form")
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            else if (type == "data")
            {
                req.ContentType = "multipart/form-data";
            }
            else if (type == "json")
            {
                req.ContentType = "application/json";
            }
            else if (type == "xml")
            {
                req.ContentType = "text/xml";
            }

            req.ContentLength = requestBytes.Length;
            Stream requestStream = await req.GetRequestStreamAsync();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
            string PostJie = sr.ReadToEnd();
            sr.Close();
            res.Close();

        }




        /// <summary>
        /// Post文件和数据到指定url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formItems">Post表单内容</param>
        /// <param name="headers">自定义Header集合</param>
        /// <param name="isSkipSslVerification">是否跳过SSL验证</param>
        /// <returns></returns>
        public static string PostForm(string url, List<dtoFormItem> formItems, Dictionary<string, string> headers = default, bool isSkipSslVerification = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (headers != default)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (isSkipSslVerification)
            {
                request.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            }

            request.Method = "POST";


            string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

            //请求流
            var postStream = new MemoryStream();

            //是否用Form上传文件
            var formUploadFile = formItems != null && formItems.Count > 0;
            if (formUploadFile)
            {
                //文件数据模板
                string fileFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                    "\r\nContent-Type: application/octet-stream" +
                    "\r\n\r\n";
                //文本数据模板
                string dataFormdataTemplate =
                    "\r\n--" + boundary +
                    "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                    "\r\n\r\n{1}";
                foreach (var item in formItems)
                {
                    string formdata = null;
                    if (item.IsFile)
                    {
                        //上传文件
                        formdata = string.Format(
                            fileFormdataTemplate,
                            item.Key, //表单键
                            item.FileName);
                    }
                    else
                    {
                        //上传文本
                        formdata = string.Format(
                            dataFormdataTemplate,
                            item.Key,
                            item.Value);
                    }

                    //统一处理
                    byte[] formdataBytes = null;
                    //第一行不需要换行
                    if (postStream.Length == 0)
                        formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                    else
                        formdataBytes = Encoding.UTF8.GetBytes(formdata);
                    postStream.Write(formdataBytes, 0, formdataBytes.Length);

                    //写入文件内容
                    if (item.FileContent != null && item.FileContent.Length > 0)
                    {
                        using (var stream = item.FileContent)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = 0;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                postStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                //结尾
                var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                postStream.Write(footer, 0, footer.Length);

            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }

            request.ContentLength = postStream.Length;

            if (postStream != null)
            {
                postStream.Position = 0;
                //直接写入流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                ////debug
                //postStream.Seek(0, SeekOrigin.Begin);
                //StreamReader sr = new StreamReader(postStream);
                //var postStr = sr.ReadToEnd();

                postStream.Close();//关闭文件访问
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string retString = myStreamReader.ReadToEnd();
                    return retString;
                }
            }
        }


    }
}
