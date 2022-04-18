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

        private static ConnectionMultiplexer ConnectionMultiplexer = ConnectionMultiplexer.Connect(ConnectionString);

        private static string SystemSign = "QingQu";

        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveKey(string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.KeyDelete(key + SystemSign);
        }



        /// <summary>
        /// 设置string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void StrSet(string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.StringSet(key + SystemSign, value);
        }



        /// <summary>
        /// 设置string类型key,包含有效时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        public static void StrSet(string key, string value, TimeSpan timeout)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.StringSet(key + SystemSign, value, timeout);
        }




        /// <summary>
        /// 读取string类型的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string StrGet(string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.StringGet(key + SystemSign);
        }



        /// <summary>
        /// 通过key判断是否存在指定Str
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsContainStr(string key)
        {
            var info = StrGet(key + SystemSign);

            if (string.IsNullOrEmpty(info))
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
        public static long StrAppend(string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.StringAppend(key + SystemSign, value);
        }




        /// <summary>
        /// 给value加上指定值,适用于value是long类型的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long LongIncrement(string key, long value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.StringIncrement(key + SystemSign, value);
        }


        /// <summary>
        /// 给value减去指定值,适用于value是long类型的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long LongDecrement(string key, long value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.StringDecrement(key + SystemSign, value);
        }




        /// <summary>
        /// 设置List，Value可重复
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ListSet(string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.ListRightPush(key + SystemSign, value);
        }




        /// <summary>
        /// 设置List,Value不可重复
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetAdd(string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.SetAdd(key + SystemSign, value);
        }



        /// <summary>
        /// 通过key获取list中指定row的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string ListGetRV(string key, int row)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.ListGetByIndex(key + SystemSign, row);
        }




        /// <summary>
        /// 通过key获取list中的所有值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<RedisValue> ListGetKV(string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.ListRange(key + SystemSign).ToList();
        }




        /// <summary>
        /// 删除List中指定的value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void ListDelV(string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.ListRemove(key + SystemSign, value);
        }




        /// <summary>
        /// 返回List的总行数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long ListCount(string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.ListLength(key + SystemSign);
        }




        /// <summary>
        /// 设置Hash，传入Hash的主name,里面的key,value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void HashSet(string name, string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.HashSet(name + SystemSign, key, value);
        }




        /// <summary>
        /// 读取Hash中某个key的值,传入Hash的name和key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HashGet(string name, string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.HashGet(name + SystemSign, key);
        }




        /// <summary>
        /// 读取Hash中的所有值,传入Hash的name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<HashEntry> HashGet(string name)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.HashGetAll(name + SystemSign).ToList();
        }




        /// <summary>
        /// 删除Hash中指定key,传入Hash的name和要删除的key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        public static void HashDelK(string name, string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            Database.HashDelete(name + SystemSign, key);
        }




        /// <summary>
        /// 验证Hash中是否存在指定key,传入Hash的name和查询的key
        /// </summary>
        /// <param name="name"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HashLike(string name, string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.HashExists(name + SystemSign, key);
        }





        /// <summary>
        /// 返回Hash中总行数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long HashCount(string key)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.HashLength(key + SystemSign);
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
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.LockTake(key + SystemSign, value, timeOut);
        }




        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="key">锁的名称</param>
        /// <param name="value">解锁密钥</param>
        /// <returns></returns>
        public static bool UnLock(string key, string value)
        {
            var Database = ConnectionMultiplexer.GetDatabase();
            return Database.LockRelease(key + SystemSign, value);
        }

    }
}
