using Common.BaiduAi.Models.ImageSearch;
using System;
using System.Collections.Generic;
using System.IO;

namespace Common.BaiduAi
{
    public class ImageSearch
    {


        /// <summary>
        /// 带参数调用商品检索—入库
        /// </summary>
        /// <param name="imgpath">图片路径</param>
        /// <param name="brief">摘要</param>
        /// <param name="class_id1">一级分类ID</param>
        /// <param name="class_id2">二级分类ID</param>
        public static Newtonsoft.Json.Linq.JObject ProductAdd(string path, string brief, int class_id1, int class_id2, bool isurl = false)
        {

            string type = "ProductAdd_" + DateTime.Now.ToLongDateString();

            if (DBHelper.RunCountGet(type) <= 10000)
            {
                DBHelper.RunCountSet(type);

                var result = new Newtonsoft.Json.Linq.JObject();

                var options = new Dictionary<string, object>{
                    {"brief", brief},
                    {"class_id1", class_id1},
                    {"class_id2", class_id2}
                };


                if (isurl)
                {
                    result = Client.ImageSearch().ProductAddUrl(path, options);
                }
                else
                {
                    var image = File.ReadAllBytes(path);

                    result = Client.ImageSearch().ProductAdd(image, options);
                };


                Console.WriteLine(result);

                return result;


            }
            else
            {
                return null;
            }
        }



        public static Newtonsoft.Json.Linq.JObject ProductUpdate(string path, string brief, int class_id1, int class_id2, bool isurl = false)
        {
            string type = "ProductUpdate" + DateTime.Now.ToLongDateString();

            if (DBHelper.RunCountGet(type) <= 10000)
            {
                DBHelper.RunCountSet(type);

                var result = new Newtonsoft.Json.Linq.JObject();

                var options = new Dictionary<string, object>{
                    {"brief", brief},
                    {"class_id1", class_id1},
                    {"class_id2", class_id2}
                };


                if (isurl)
                {
                    result = Client.ImageSearch().ProductUpdateUrl(path, options);
                }
                else
                {
                    var image = File.ReadAllBytes(path);

                    result = Client.ImageSearch().ProductUpdate(image, options);
                };


                Console.WriteLine(result);

                return result;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 带参数调用商品检索—检索
        /// </summary>
        public static List<ProductSearch.Result> ProductSearch(string imgpath, int class_id1 = 0, int class_id2 = 0, int pn = 0, int rn = 0)
        {
            var list = new List<ProductSearch.Result>();

            string type = "ProductSearch_" + DateTime.Now.ToLongDateString();

            if (DBHelper.RunCountGet(type) <= 500)
            {
                DBHelper.RunCountSet(type);
                try
                {
                    var image = File.ReadAllBytes(imgpath);

                    var options = new Dictionary<string, object>();

                    if (class_id1 != 0)
                    {
                        options.Add("class_id1", class_id1);
                    }

                    if (class_id2 != 0)
                    {
                        options.Add("class_id2", class_id2);
                    }

                    if (rn != 0)
                    {
                        options.Add("pn", pn.ToString());
                        options.Add("rn", rn.ToString());
                    }



                    var result = Client.ImageSearch().ProductSearch(image, options);
                    Console.WriteLine(result);


                    var x = result.GetValue("result").ToString();

                    list = Common.Json.JsonHelper.JSONToList<ProductSearch.Result>(x);
                }
                catch
                {

                }
            }

            return list;
        }
    }





}

