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
        /// 为了兼容性，我们将值包装为 { value: "...", expire_at: ticks? }
        /// expire_at 为 null 表示不生效
        /// </summary>
        public bool SetString(string key, string value)
        {
            try
            {
                var payload = new
                {
                    value = value,
                    expire_at = (long?)null
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
                    expire_at = (long?)null
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
                var expire_at = DateTime.UtcNow.Add(timeOut).Ticks;
                var payload = new
                {
                    value = value,
                    expire_at = (long?)expire_at
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
                var expire_at = DateTime.UtcNow.Add(timeOut).Ticks;
                var payload = new
                {
                    value = valueStr,
                    expire_at = (long?)expire_at
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
            var valueStr = ReadPayloadValue(Cache.GetString(key), out var expired);
            if (expired)
            {
                // 已过期：删除并返回空字符串
                try { Cache.Remove(key); } catch { }
                return "";
            }
            return valueStr ?? "";
        }

        /// <summary>
        /// 读取string类型的key（异步版，语义同 <see cref="GetString"/>）
        /// </summary>
        public async Task<string> GetStringAsync(string key)
        {
            var valueStr = ReadPayloadValue(await Cache.GetStringAsync(key), out var expired);
            if (expired)
            {
                try { await Cache.RemoveAsync(key); } catch { }
                return "";
            }
            return valueStr ?? "";
        }

        /// <summary>
        /// 从包装格式 { value, expire_at } 中取出 value 原文；返回 null 表示没有可用值
        /// （key 不存在、value 为空或已过期）。expired 为 true 时由调用方按自己的方式（同步/异步）删键。
        /// <para>与 GetString/GetObject 的兼容分支一致：不是包装格式时按原始字符串返回。</para>
        /// </summary>
        private static string? ReadPayloadValue(string? raw, out bool expired)
        {
            expired = false;
            if (string.IsNullOrEmpty(raw))
            {
                return null;
            }

            var valueStr = raw;
            try
            {
                var jt = JsonConvert.DeserializeObject<JObject>(raw);
                if (jt != null && jt["value"] != null)
                {
                    var expireToken = jt["expire_at"];
                    if (expireToken != null && expireToken.Type != JTokenType.Null
                        && DateTime.UtcNow.Ticks > expireToken.Value<long>())
                    {
                        expired = true;
                        return null;
                    }
                    valueStr = (jt["value"] ?? "").ToString();
                }
            }
            catch
            {
                // 解析失败，视为原始字符串（向后兼容）
            }
            return string.IsNullOrEmpty(valueStr) ? null : valueStr;
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
                    var expireToken = jt["expire_at"];
                    if (expireToken != null && expireToken.Type != JTokenType.Null)
                    {
                        var expireTicks = expireToken.Value<long>();
                        if (DateTime.UtcNow.Ticks > expireTicks)
                        {
                            try { Cache.Remove(key); } catch { }
                            throw new Exception($"缓存key：{key}值为空");
                        }
                    }
                    valueStr = (jt["value"] ?? "").ToString();
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
        /// 读取 Object 类型的key，取不到时返回 default（同 <see cref="GetObject{T}"/> 但语义上是“没有值”而不是错误）
        /// <para>
        /// 适用于“取不到就降级”的场景（例如按连接映射定位推送目标：没有在线连接就不推）。
        /// key 不存在、值为空、已过期都返回 default；Redis 自身的通信异常仍然抛出，
        /// 由调用方决定是否降级，避免把故障伪装成“确实没有数据”。
        /// </para>
        /// </summary>
        public T? GetObjectOrDefault<T>(string key) where T : class
        {
            var valueStr = ReadPayloadValue(Cache.GetString(key), out var expired);
            if (expired)
            {
                try { Cache.Remove(key); } catch { }
            }
            return valueStr == null ? default : JsonConvert.DeserializeObject<T>(valueStr.Replace("undefined", "null"));
        }

        /// <summary>
        /// 读取 Object 类型的key（异步版，语义同 <see cref="GetObjectOrDefault{T}"/>）
        /// </summary>
        public async Task<T?> GetObjectOrDefaultAsync<T>(string key) where T : class
        {
            var valueStr = ReadPayloadValue(await Cache.GetStringAsync(key), out var expired);
            if (expired)
            {
                try { await Cache.RemoveAsync(key); } catch { }
            }
            return valueStr == null ? default : JsonConvert.DeserializeObject<T>(valueStr.Replace("undefined", "null"));
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