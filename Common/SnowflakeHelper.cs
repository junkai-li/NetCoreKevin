using System;

namespace Common
{

    /// <summary>
    /// 利用雪花算法动态生成有序的ID
    /// </summary>
    public class SnowflakeHelper
    {


        //1位，不用。二进制中最高位为1的都是负数，但是我们生成的id一般都使用整数，所以这个最高位固定是0
        //41位，用来记录时间戳（毫秒）。
        //41位可以表示$2^{41}-1$个数字，
        //如果只用来表示正整数（计算机中正数包含0），可以表示的数值范围是：0 至 $2^{41}-1$，减1是因为可表示的数值范围是从0开始算的，而不是1。
        //也就是说41位可以表示$2^{41}-1$个毫秒的值，转化成单位年则是$(2^{41}-1) / (1000 * 60 * 60 * 24 * 365) = 69$年
        //10位，用来记录工作机器id。
        //可以部署在$2^{10} = 1024$个节点，包括5位datacenterId和5位workerId
        //5位（bit）可以表示的最大正整数是$2^{5}-1 = 31$，即可以用0、1、2、3、....31这32个数字，来表示不同的datecenterId或workerId
        //12位，序列号，用来记录同毫秒内产生的不同id。
        //12位（bit）可以表示的最大正整数是$2^{12}-1 = 4095$，即可以用0、1、2、3、....4094这4095个数字，来表示同一机器同一时间截（毫秒)内产生的4095个ID序号


        private static long machineId;//机器ID
        private static long datacenterId = 0L;//数据ID
        private static long sequence = 0L;//计数从零开始

        private static long twepoch = 1609459200000L; //唯一时间随机量，这是一个避免重复的随机量，自行设定不要大于当前时间戳

        private static long machineIdBits = 5L; //机器码字节数
        private static long datacenterIdBits = 5L;//数据字节数
        public static long maxMachineId = -1L ^ -1L << (int)machineIdBits; //最大机器ID
        private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);//最大数据ID

        private static long sequenceBits = 11L; //计数器字节数，11个字节用来保存计数码，每毫秒可以生成2047个ID
        private static long machineIdShift = sequenceBits; //机器码数据左移位数，就是后面计数器占用的位数
        private static long datacenterIdShift = sequenceBits + machineIdBits;
        private static long timestampLeftShift = sequenceBits + machineIdBits + datacenterIdBits; //时间戳左移动位数就是机器码+计数器总字节数+数据字节数
        public static long sequenceMask = -1L ^ -1L << (int)sequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private static long lastTimestamp = -1L;//最后时间戳

        private static object syncRoot = new object();//加锁对象
        static SnowflakeHelper snowflake;



        public static SnowflakeHelper Instance()
        {
            if (snowflake == null)
                snowflake = new SnowflakeHelper();
            return snowflake;
        }



        public SnowflakeHelper()
        {
            Snowflakes(0L, -1);
        }


        /// <summary>
        /// </summary>
        /// <param name="machineId">机器ID,最大为31</param>
        public SnowflakeHelper(long machineId)
        {
            Snowflakes(machineId, -1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="machineId">机器ID,最大为31</param>
        /// <param name="datacenterId">数据ID,最大为31</param>
        public SnowflakeHelper(long machineId, long datacenterId)
        {
            Snowflakes(machineId, datacenterId);
        }



        private void Snowflakes(long machineId, long datacenterId)
        {
            if (machineId >= 0)
            {
                if (machineId > maxMachineId)
                {
                    throw new Exception("机器码ID非法");
                }
                SnowflakeHelper.machineId = machineId;
            }
            if (datacenterId >= 0)
            {
                if (datacenterId > maxDatacenterId)
                {
                    throw new Exception("数据中心ID非法");
                }
                SnowflakeHelper.datacenterId = datacenterId;
            }
        }




        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        private static long GetTimestamp()
        {

            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }




        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            if (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }
            return timestamp;
        }




        /// <summary>
        /// 获取长整形的ID
        /// </summary>
        /// <returns></returns>
        public long GetId()
        {
            lock (syncRoot)
            {
                long timestamp = GetTimestamp();
                if (SnowflakeHelper.lastTimestamp == timestamp)
                { //同一微妙中生成ID
                    sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (sequence == 0)
                    {
                        //一微妙内产生的ID计数已达上限，等待下一微妙
                        timestamp = GetNextTimestamp(SnowflakeHelper.lastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID
                    sequence = 0L;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                SnowflakeHelper.lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long Id = ((timestamp - twepoch) << (int)timestampLeftShift)
                    | (datacenterId << (int)datacenterIdShift)
                    | (machineId << (int)machineIdShift)
                    | sequence;
                return Id;
            }
        }



        /// <summary>
        /// 通过ID获取其中的时间戳
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DateTime GetTimeById(long id)
        {
            var idStr2 = Convert.ToString(id, 2);

            if (idStr2.Length < 63)
            {
                do
                {
                    idStr2 = "0" + idStr2;
                } while (idStr2.Length != 63);
            }

            var timeBits = 64 - machineIdBits - datacenterIdBits - sequenceBits - 1;

            var timeStr2 = idStr2.Substring(0, Convert.ToInt32(timeBits));

            var timeJsStamp = Convert.ToInt64(timeStr2, 2);

            var twepochTime = Common.DateTimeHelper.JsToTime(twepoch);

            var time = DateTimeHelper.JsToTime(timeJsStamp, twepochTime.Year);

            return time;
        }


    }
}
