namespace kevin.AI.AgentFramework.Interfaces
{
    public interface IBaseAIToolService
    {
        /// <summary>
        /// 初始化数据 用于AI前传递数据
        /// </summary>
        /// <param name="data"></param>
        public void InitData(object data);
    }
}
