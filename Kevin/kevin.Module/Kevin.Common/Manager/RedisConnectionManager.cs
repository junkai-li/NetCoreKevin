using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.Manager
{
    /// <summary>
    /// redis 连接管理器，使用单例模式确保全局只有一个 ConnectionMultiplexer 实例。
    /// </summary>
    public sealed class RedisConnectionManager
    {
        // 1. 定义静态锁对象，用于线程安全
        private static readonly object _lock = new object();

        // 2. 定义静态实例变量
        private static ConnectionMultiplexer _instance;
        private static string _connectionString;

        // 3. 私有构造函数，防止外部通过 new 创建实例
        private RedisConnectionManager() { }

        /// <summary>
        /// 初始化连接字符串（建议在程序启动时调用一次，例如 Main 方法或 Startup.cs）
        /// </summary>
        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 获取 ConnectionMultiplexer 单例实例
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                // 第一次检查：如果实例已存在，直接返回，避免不必要的锁等待
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        // 第二次检查：锁住之后再次检查，防止多个线程同时通过第一次检查
                        if (_instance == null)
                        {
                            if (string.IsNullOrWhiteSpace(_connectionString))
                            {
                                throw new InvalidOperationException("Redis 连接字符串未初始化。请先调用 RedisConnectionManager.Initialize()。");
                            }

                            // 配置连接选项
                            var configOptions = ConfigurationOptions.Parse(_connectionString);

                            // 【重要】AbortOnConnectFail 设置为 false
                            // 这样如果 Redis 启动时不可用，ConnectionMultiplexer 不会抛出异常，
                            // 而是会在后台静默重连，提高程序的健壮性。
                            configOptions.AbortOnConnectFail = false;
                            configOptions.ConnectRetry = 3; // 连接失败重试次数
                            configOptions.ConnectTimeout = 5000; // 连接超时时间(毫秒)

                            // 创建连接
                            _instance = ConnectionMultiplexer.Connect(configOptions);

                            // 注册注册事件（可选，用于调试或监控）
                            _instance.ConnectionFailed += (sender, e) =>
                            {
                                Console.WriteLine($"Redis 连接失败: ");
                            };

                            _instance.ConnectionRestored += (sender, e) =>
                            {
                                Console.WriteLine($"Redis 连接恢复: {e.EndPoint}");
                            };
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取默认数据库 (Database ID 0)
        /// </summary>
        public static IDatabase GetDatabase()
        {
            return Instance.GetDatabase();
        }

        /// <summary>
        /// 获取指定数据库
        /// </summary>
        /// <param name="db">数据库 ID (例如 0, 1, 2...)</param>
        /// <param name="asyncState">异步状态对象</param>
        public static IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            return Instance.GetDatabase(db, asyncState);
        }
    }
}
