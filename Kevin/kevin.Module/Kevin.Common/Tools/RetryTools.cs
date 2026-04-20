using Kevin.log4Net;
using System;
using System.Threading.Tasks;
/// <summary>
/// 任务进行多次的重试  return Common.RetryTools.Retry(() => _spSystemSettingsDapperRep.GetSendMailList(mailSearchModel));
/// </summary>

namespace Common
{
    /// <summary>
    /// 任务重试工具类，提供两种重试方式：1. 若发生 Exception (数据库查询超时)，重复执行相同动作 2. 传入多个动作，遇到 Exception依序执行 (数据库查询超时，改用不同条件查询)
    /// </summary>
    public static class RetryTools
    {
        /// <summary>
        /// 若发生 Exception (数据库查询超时)，重复执行相同动作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="retryTimes">默认重试 3次，传入 0直接 return default(T)</param>
        /// <param name="sleepMillisecondsTimeout">每次重试的间隔时间，默认为 1000 毫秒</param>
        /// <returns></returns>
        public static T Retry<T>(Func<T> handler, int retryTimes = 3, int sleepMillisecondsTimeout = 1000)
        {
            if (retryTimes <= 0)
            {
                return default;
            }
            try
            {
                return handler();
            }
            catch (Exception e)
            {
                retryTimes--;
                LogHelper.logger.Error($"剩余重试次数: {retryTimes}, retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                return Retry(handler, retryTimes);
            }
        }

        /// <summary>
        /// 若发生 Exception (数据库查询超时)，重复执行相同动作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="retryTimes">默认重试 3次，传入 0直接 return default(T)</param>
        /// <param name="sleepMillisecondsTimeout">每次重试的间隔时间，默认为 1000 毫秒</param>
        /// <returns></returns>
        public static async Task<T> RetryAsync<T>(Func<Task<T>> handler, int retryTimes = 3, int sleepMillisecondsTimeout = 1000)
        {
            if (retryTimes <= 0)
            {
                return default;
            }
            try
            {
                return await handler();
            }
            catch (Exception e)
            {
                retryTimes--;
                LogHelper.logger.Error($"剩余重试次数: {retryTimes}, retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                return await Retry(handler, retryTimes);
            }
        }
        /// <summary>
        /// 传入多个动作，遇到 Exception依序执行 (数据库查询超时，改用不同条件查询)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="sleepMillisecondsTimeout">每次重试的间隔时间，默认为 1000 毫秒</param>
        /// <returns></returns>
        public static T Retry<T>(int sleepMillisecondsTimeout = 1000, params Func<T>[] handlers)
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                try
                {
                    return handler();
                }
                catch (Exception e)
                {
                    LogHelper.logger.Error($"第 {i}次执行错误(start from 0): retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                    System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                }
            }
            return default;
        }
        /// <summary>
        /// 若发生 Exception (数据库查询超时)，重复执行相同动作
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="retryTimes">默认重试 3次，传入 0直接 return</param>
        public static void RetryVoid<T>(Func<T> handler, int retryTimes = 3, int sleepMillisecondsTimeout = 1000)
        {
            if (retryTimes <= 0)
            {
                return;
            }
            try
            {
                handler();
            }
            catch (Exception e)
            {
                retryTimes--;
                LogHelper.logger.Error($"剩余重试次数: {retryTimes}, retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                RetryVoid(handler, retryTimes);
            }
        }


    }

}