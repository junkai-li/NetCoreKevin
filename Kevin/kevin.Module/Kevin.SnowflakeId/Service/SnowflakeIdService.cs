using Kevin.SnowflakeId.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Kevin.SnowflakeId.Service
{
    public class SnowflakeIdService : ISnowflakeIdService
    {
        private const long twepoch = 687888001000L; // 起始时间戳（毫秒）
        private static readonly long workerIdBits = 5L; // 节点ID所占的位数
        private static readonly long datacenterIdBits = 5L; // 数据中心ID所占的位数
        private static readonly long maxWorkerId = -1L ^ (-1L << (int)workerIdBits); // 节点ID最大值
        private static readonly long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits); // 数据中心ID最大值
        private static readonly long sequenceBits = 12L; // 序列号占用的位数
        private static readonly long workerIdShift = sequenceBits; // 节点ID左移位数
        private static readonly long datacenterIdShift = sequenceBits + workerIdBits; // 数据中心ID左移位数
        private static readonly long timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits; // 时间戳左移位数
        private static readonly long sequenceMask = -1L ^ (-1L << (int)sequenceBits); // 用于掩码序列号

        private long lastTimestamp = -1L; // 上次生成ID的时间戳
        private long workerId; // 节点ID
        private long datacenterId; // 数据中心ID
        private long sequence = 0L; // 序列号

        public SnowflakeIdService(IOptionsMonitor<SnowflakeIdSetting> config)
        {
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {maxWorkerId} or less than 0");
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"datacenter Id can't be greater than {maxDatacenterId} or less than 0");
            }
            this.workerId = config.CurrentValue.MachineId;
            this.datacenterId = config.CurrentValue.DataCenterId; ;
        }

        public long GetNextId()
        {
            lock (this) // 加锁保证多线程安全
            {
                long timestamp = TimeGen();
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("Clock moved backwards. Refusing to generate id for " + (lastTimestamp - timestamp) + " milliseconds");
                }
                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0;
                }
                lastTimestamp = timestamp;
                return ((timestamp - twepoch) << (int)timestampLeftShift) | (datacenterId << (int)datacenterIdShift) | (workerId << (int)workerIdShift) | sequence;
            }
        }

        private long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private long TimeGen() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 获取当前时间戳（毫秒）

    }
}
