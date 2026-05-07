namespace Kevin.RAG.Dto
{
    public class EmbeddingDto
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> data { get; set; } = new List<DataItem>();
        /// <summary>
        /// 
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @object { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Usage usage { get; set; }
    }

    public class DataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<float> embedding { get; set; } = new List<float>();
        /// <summary>
        /// 
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @object { get; set; }
    }

    public class Usage
    {
        /// <summary>
        /// 
        /// </summary>
        public int completion_tokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int prompt_tokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total_tokens { get; set; }
    }
}
