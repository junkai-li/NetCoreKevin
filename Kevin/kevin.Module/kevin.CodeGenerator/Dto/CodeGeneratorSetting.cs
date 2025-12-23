namespace kevin.CodeGenerator.Dto
{
    public class CodeGeneratorSetting
    {
        /// <summary>
        /// 配置文件相关信息
        /// </summary>
        public List<CodeGeneratorItem> CodeGeneratorItems { get; set; } = new();
    }

    public class CodeGeneratorItem
    {
        /// <summary>
        /// 区域
        /// </summary>
        public string AreaName { get; set; } = "";

        /// <summary>
        /// 数据库实体类路径
        /// </summary>
        public string AreaPath { get; set; } = "";

        /// <summary>
        /// 仓储接口生成路径
        /// </summary>
        public string IRpBulidPath { get; set; } = "";
        /// <summary>
        /// 仓储生成路径
        /// </summary>
        public string RpBulidPath { get; set; } = "";

        /// <summary>
        /// 服务接口生成路径
        /// </summary>
        public string IServiceBulidPath { get; set; } = "";

        /// <summary>
        /// 服务生成路径
        /// </summary>
        public string ServiceBulidPath { get; set; } = "";


    }
}
