using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class RedisHelper
    {

        private static string ConnectionString = IO.Config.Get().GetConnectionString("redisConnection");

        private static readonly ConnectionMultiplexer ConnectionMultiplexer = ConnectionMultiplexer.Connect(ConnectionString);



        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyDelete(string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.KeyDelete(key);
        }



        /// <summary>
        /// 设置string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StringSet(string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.StringSet(key, value);
        }



        /// <summary>
        /// 设置string类型key,包含有效时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static bool StringSet(string key, string value, TimeSpan timeOut)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.StringSet(key, value, timeOut);
        }



        /// <summary>
        /// 读取string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string StringGet(string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.StringGet(key);
        }



        /// <summary>
        /// 通过key判断是否存在指定Str
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsContainString(string key)
        {
            if (string.IsNullOrEmpty(StringGet(key)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        /// <summary>
        /// 给value追加值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long StringAppend(string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.StringAppend(key, value);
        }



        /// <summary>
        /// 给value加上指定值,适用于value是long类型的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long LongIncrement(string key, long value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.StringIncrement(key, value);
        }



        /// <summary>
        /// 给value减去指定值,适用于value是long类型的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long LongDecrement(string key, long value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.StringDecrement(key, value);
        }



        /// <summary>
        /// 设置List，Value可重复
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ListRightPush(string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.ListRightPush(key, value);
        }



        /// <summary>
        /// 设置List,Value不可重复
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void ListAdd(string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            database.SetAdd(key, value);
        }



        /// <summary>
        /// 通过key获取list中指定row的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string ListGetByIndex(string key, int row)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.ListGetByIndex(key, row);
        }



        /// <summary>
        /// 通过key获取list中的所有值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<RedisValue> ListGetAll(string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.ListRange(key).ToList();
        }



        /// <summary>
        /// 删除List中指定的value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void ListRemove(string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            database.ListRemove(key, value);
        }



        /// <summary>
        /// 返回List的总行数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long ListCount(string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.ListLength(key);
        }



        /// <summary>
        /// 设置Hash，传入Hash的主name,里面的key,value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void HashSet(string name, string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            database.HashSet(name, key, value);
        }



        /// <summary>
        /// 读取Hash中某个key的值,传入Hash的name和key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HashGet(string name, string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.HashGet(name, key);
        }



        /// <summary>
        /// 读取Hash中的所有值,传入Hash的name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<HashEntry> HashGetAll(string name)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.HashGetAll(name).ToList();
        }



        /// <summary>
        /// 删除Hash中指定key,传入Hash的name和要删除的key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        public static void HashDelete(string name, string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            database.HashDelete(name, key);
        }



        /// <summary>
        /// 验证Hash中是否存在指定key,传入Hash的name和查询的key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HashExists(string name, string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.HashExists(name, key);
        }



        /// <summary>
        /// 返回Hash中总行数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long HashCount(string key)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.HashLength(key);
        }



        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="key">锁的名称，不可重复</param>
        /// <param name="value">解锁密钥</param>
        /// <param name="timeOut">失效时长</param>
        /// <returns></returns>
        public static bool Lock(string key, string value, TimeSpan timeOut)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.LockTake(key, value, timeOut);
        }



        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="key">锁的名称</param>
        /// <param name="value">解锁密钥</param>
        /// <returns></returns>
        public static bool UnLock(string key, string value)
        {
            var database = ConnectionMultiplexer.GetDatabase();
            return database.LockRelease(key, value);
        }



    }
}
