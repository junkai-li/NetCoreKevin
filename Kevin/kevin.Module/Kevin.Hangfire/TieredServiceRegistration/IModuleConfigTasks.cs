using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Hangfire.TieredServiceRegistration
{
    /// <summary>
    /// 任务模块初始化器
    /// </summary>
    public interface IModuleConfigTasks
    {
        /// <summary>
        /// 任务模块初始化器
        /// </summary>
        /// <param name="services"></param>
        public Task<bool> ConfigTasks(IRecurringJobManager recurringJobManager);
    }
}
