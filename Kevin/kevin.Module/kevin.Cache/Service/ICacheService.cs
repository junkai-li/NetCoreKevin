namespace kevin.Cache.Service
{
    public interface ICacheService
    {
        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key);
        /// <summary>
        /// 设置string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetString(string key, string value);

        /// <summary>
        /// 设置object类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetObject(string key, object value);

        /// <summary>
        /// 设置string类型key,包含有效时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public bool SetString(string key, string value, TimeSpan timeOut);

        /// <summary>
        /// 设置object类型key,包含有效时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public bool SetObject(string key, object value, TimeSpan timeOut);


        /// <summary>
        /// 读取string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key);

        /// <summary>
        /// 读取string类型的key（异步：不等 Redis 时不占用线程池线程）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<string> GetStringAsync(string key);

        /// <summary>
        /// 读取 Object 类型的key，取不到时返回 default（key 不存在、值为空、已过期都算“取不到”）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetObjectOrDefault<T>(string key) where T : class;

        /// <summary>
        /// 读取 Object 类型的key（异步：不等 Redis 时不占用线程池线程），取不到时返回 default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<T?> GetObjectOrDefaultAsync<T>(string key) where T : class;

        /// <summary>
        /// 读取 Object 类型的key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetObject<T>(string key);

        /// <summary>
        /// 判断是否存在指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsContainKey(string key);
    }
}
