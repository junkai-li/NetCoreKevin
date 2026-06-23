using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// 设置string类型的key（无过期）
        /// 为了兼容性，我们将值包装为 { value: "...", expireAt: ticks? }
        /// expireAt 为 null 表示不生效
        /// </summary>
        public bool SetString(string key, string value)
        {
            try
            {
                var payload = new
                {
                    value = value,
                    expireAt = (long?)null
                };
                Cache.SetString(key, JsonConvert.SerializeObject(payload));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置object类型的key（无过期）
        /// </summary>
        public bool SetObject(string key, object value)
        {
            try
            {
                var valueStr = JsonConvert.SerializeObject(value);
                var payload = new
                {
                    value = valueStr,
                    expireAt = (long?)null
                };
                Cache.SetString(key, JsonConvert.SerializeObject(payload));
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
        public bool SetString(string key, string value, TimeSpan timeOut)
        {
            try
            {
                var expireAt = DateTime.UtcNow.Add(timeOut).Ticks;
                var payload = new
                {
                    value = value,
                    expireAt = (long?)expireAt
                };
                Cache.SetString(key, JsonConvert.SerializeObject(payload), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeOut });
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
        public bool SetObject(string key, object value, TimeSpan timeOut)
        {
            try
            {
                var valueStr = JsonConvert.SerializeObject(value);
                var expireAt = DateTime.UtcNow.Add(timeOut).Ticks;
                var payload = new
                {
                    value = valueStr,
                    expireAt = (long?)expireAt
                };
                Cache.SetString(key, JsonConvert.SerializeObject(payload), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeOut });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取string类型的key（会校验包装中的过期时间；兼容未包装的原始字符串）
        /// </summary>
        public string GetString(string key)
        {
            var raw = Cache.GetString(key);
            if (string.IsNullOrEmpty(raw))
            {
                return "";
            }

            // 尝试解析为包装格式
            try
            {
                var jt = JsonConvert.DeserializeObject<JObject>(raw);
                if (jt != null && jt["value"] != null)
                {
                    var expireToken = jt["expireAt"];
                    if (expireToken != null && expireToken.Type != JTokenType.Null)
                    {
                        var expireTicks = expireToken.Value<long>();
                        if (DateTime.UtcNow.Ticks > expireTicks)
                        {
                            // 已过期：删除并返回空字符串
                            try { Cache.Remove(key); } catch { }
                            return "";
                        }
                    }
                    return (jt["value"] ?? "").ToString();
                }
            }
            catch
            {
                // 解析失败，视为原始字符串（向后兼容）
            }

            return raw;
        }

        /// <summary>
        /// 读取 Object 类型的key（兼容包装与原始字符串）
        /// </summary>
        public T GetObject<T>(string key)
        {
            var raw = Cache.GetString(key);
            if (string.IsNullOrEmpty(raw))
            {
                throw new Exception($"缓存key：{key}值为空");
            }

            string valueStr = raw;

            // 如果是包装格式，提取内部 value 并校验过期
            try
            {
                var jt = JsonConvert.DeserializeObject<JObject>(raw);
                if (jt != null && jt["value"] != null)
                {
                    var expireToken = jt["expireAt"];
                    if (expireToken != null && expireToken.Type != JTokenType.Null)
                    {
                        var expireTicks = expireToken.Value<long>();
                        if (DateTime.UtcNow.Ticks > expireTicks)
                        {
                            try { Cache.Remove(key); } catch { }
                            throw new Exception($"缓存key：{key}值为空");
                        }
                    }
                    valueStr = (jt["value"]??"").ToString();
                }
            }
            catch
            {
                // ignore, treat raw as serialized object
            }

            var value = JsonConvert.DeserializeObject<T>(valueStr.Replace("undefined", "null"));
            if (value != null)
            {
                return value;
            }
            throw new Exception($"{valueStr}GetObject为null");
        }

        /// <summary>
        /// 判断是否存在指定key（会被包装和空值逻辑影响）
        /// </summary>
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