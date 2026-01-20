using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace kevin.Cache.Service
{
    public class CacheService : ICacheService
    {
        private IDistributedCache Cache;
        public CacheService(IDistributedCache distributed)
        {
            Cache = distributed;
        }

        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
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
        public bool SetString(string key, string value)
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
        public bool SetObject(string key, object value)
        {
            try
            {
                var valueStr = JsonConvert.SerializeObject(value).ToString();
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
        public bool SetString(string key, string value, TimeSpan timeOut)
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
        public bool SetObject(string key, object value, TimeSpan timeOut)
        {
            try
            {
                var valueStr = JsonConvert.SerializeObject(value).ToString();
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
        public string GetString(string key)
        {
            return Cache.GetString(key);
        }



        /// <summary>
        /// 读取 Object 类型的key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetObject<T>(string key)
        {
            var valueStr = Cache.GetString(key);

            var value = JsonConvert.DeserializeObject<T>(valueStr.Replace("undefined", "null"));

            return value;
        }



        /// <summary>
        /// 判断是否存在指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsContainKey(string key)
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
