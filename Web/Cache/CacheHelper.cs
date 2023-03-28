using Microsoft.Extensions.Caching.Distributed;
using System; 
using Web.Global; 
using System.Text;
using Microsoft.Extensions.DependencyInjection;
namespace Web.Cache
{
    public class CacheHelper
    {


        private static bool IsInit;
        private static IDistributedCache InitCache;


        private static IDistributedCache Cache
        {
            get
            {
                if (!IsInit)
                {
                    InitCache = GlobalServices.ServiceProvider.GetService<IDistributedCache>();
                    IsInit = true;
                }

                return InitCache;
            }
        }


        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            try
            {
                Cache.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 设置string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetString(string key, string value)
        {
            try
            {
                Cache.SetString(key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 设置object类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetObject(string key, object value)
        {
            try
            {
                var valueStr = Common.Json.JsonHelper.ObjectToJSON(value);
                Cache.SetString(key, valueStr);
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 设置string类型key,包含有效时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static bool SetString(string key, string value, TimeSpan timeOut)
        {
            try
            {
                Cache.SetString(key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeOut });
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 设置object类型key,包含有效时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static bool SetObject(string key, object value, TimeSpan timeOut)
        {
            try
            {
                var valueStr = Common.Json.JsonHelper.ObjectToJSON(value);
                Cache.SetString(key, valueStr, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeOut });
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 读取string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            return Cache.GetString(key);
        }



        /// <summary>
        /// 读取 Object 类型的key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetObject<T>(string key)
        {
            var valueStr = Cache.GetString(key);

            var value = Common.Json.JsonHelper.JSONToObject<T>(valueStr);

            return value;
        }



        /// <summary>
        /// 判断是否存在指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsContainKey(string key)
        {
            if (string.IsNullOrEmpty(GetString(key)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }



    }
}
