using System; 
/// <summary>
/// 任务进行多次的重试  return Common.CommonTools.Retry(() => _spSystemSettingsDapperRep.GetSendMailList(mailSearchModel));
/// </summary>

namespace Common
{
    public static class CommonTools
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static int sleepMillisecondsTimeout = 1000;


        /// <summary>
        /// 若發生 Exception (資料庫查詢逾時)，重複執行相同動作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="retryTimes">預設重試 3次，傳入 0直接 return default(T)</param>
        /// <returns></returns>
        public static T Retry<T>(Func<T> handler, int retryTimes = 3)
        {
            if (retryTimes <= 0)
            {
                return default(T);
            }


            try
            {
                return handler();
            }
            catch (Exception e)
            {
                retryTimes--;
                logger.Error($"剩餘重試次數: {retryTimes}, retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                return Retry(handler, retryTimes);
            }
        }


        /// <summary>
        /// 傳入多個動作，遇到 Exception依序執行 (資料庫查詢逾時，改用不同條件查詢)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public static T Retry<T>(params Func<T>[] handlers)
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
                    logger.Error($"第 {i}次執行錯誤(start from 0): retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                    System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                }
            }
            return default(T);
        }


        /// <summary>
        /// 若發生 Exception (資料庫查詢逾時)，重複執行相同動作
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="retryTimes">預設重試 3次，傳入 0直接 return</param>
        public static void RetryVoid<T>(Func<T> handler, int retryTimes = 3)
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
                logger.Error($"剩餘重試次數: {retryTimes}, retry error: {e.Message}, Exception detail: {Json.JsonHelper.ObjectToJSON(e)}");
                System.Threading.Thread.Sleep(sleepMillisecondsTimeout);
                RetryVoid(handler, retryTimes);
            }
        }


    }

}
