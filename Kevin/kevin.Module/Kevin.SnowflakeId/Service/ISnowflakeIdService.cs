namespace Kevin.SnowflakeId.Service
{
    public interface ISnowflakeIdService
    {
        /// <summary>
        /// 获取id
        /// </summary>
        /// <returns></returns>
        public long GetNextId();
    }
}
