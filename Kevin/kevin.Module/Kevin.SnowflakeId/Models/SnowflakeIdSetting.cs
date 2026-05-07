namespace Kevin.SnowflakeId.Models
{
    public class SnowflakeIdSetting
    {
        /// <summary>
        /// 机器标识Id
        /// </summary>
        public long MachineId { get; set; } = 21;

        /// <summary>
        /// 数据中心标识Id
        /// </summary>
        public long DataCenterId { get; set; } = 11;
    }
}
