using Kevin.SnowflakeId.Models;
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

        /// <summary>
        /// 进程级共享的生成状态：应用内确实存在多个实例（无 HttpContext 的兜底路径会直接 new，见 BaseService）。
        /// 若每个实例各持一份 lastTimestamp/sequence，同一毫秒内多个实例都从 sequence=0 开始，会生成完全相同的 Id，
        /// 落库即报 Duplicate entry for key 'PRIMARY'。因此时间戳、序列号与锁对象都必须由所有实例共享，
        /// 不能再用 lock(this)（两个实例各自锁自己，互不排斥）。
        /// </summary>
        private static readonly object syncRoot = new object();
        private static long lastTimestamp = -1L; // 上次生成ID的时间戳
        private static long sequence = 0L; // 序列号
        private long workerId; // 节点ID
        private long datacenterId; // 数据中心ID

        /// <summary>
        /// 兜底配置快照：由 AddKevinSnowflakeId 在启动时写入。
        /// 无参构造（无 DI 上下文时直接 new）读不到 IOptions，只能靠它拿到与容器内单例一致的节点ID，
        /// 否则兜底实例会退化成默认的 MachineId/DataCenterId。
        /// </summary>
        private static SnowflakeIdSetting? _fallbackSetting;

        /// <summary>
        /// 写入兜底配置快照（启动期调用一次）
        /// </summary>
        public static void SetFallbackSetting(SnowflakeIdSetting setting)
        {
            if (setting != default)
            {
                _fallbackSetting = setting;
            }
        }

        private SnowflakeIdSetting _config;
        public SnowflakeIdService()
        {
            _config = _fallbackSetting ?? new SnowflakeIdSetting();
            this.workerId = _config.MachineId;
            this.datacenterId = _config.DataCenterId;
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"Worker ID 必须在 0 到 {maxWorkerId} 之间");
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"Datacenter ID 必须在 0 到 {maxDatacenterId} 之间");
            }
        }
        public SnowflakeIdService(IOptionsMonitor<SnowflakeIdSetting> config)
        {
            _config = config.CurrentValue;
            this.workerId = _config.MachineId;
            this.datacenterId = _config.DataCenterId;
            if (_config == default)
            {
                _config = new SnowflakeIdSetting();
            }
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"Worker ID 必须在 0 到 {maxWorkerId} 之间");
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"Datacenter ID 必须在 0 到 {maxDatacenterId} 之间");
            }

        }

        public long GetNextId()
        {
            lock (syncRoot) // 加锁保证多线程安全：必须是所有实例共享的同一把锁
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

        private static long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private static long TimeGen() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 获取当前时间戳（毫秒）

    }
}
