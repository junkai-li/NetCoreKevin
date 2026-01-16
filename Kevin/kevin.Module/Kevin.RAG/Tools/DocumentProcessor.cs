using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kevin.RAG.Tools
{
    /// <summary>
    /// 文档处理服务，负责清理和分块
    /// </summary>
    public class DocumentProcessor
    {
        private readonly int _chunkSize;
        private readonly int _chunkOverlap;

        public DocumentProcessor(int chunkSize = 500, int chunkOverlap = 50)
        {
            _chunkSize = chunkSize;
            _chunkOverlap = chunkOverlap;
        }

        /// <summary>
        /// 清理文档内容
        /// </summary>
        public string CleanDocument(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // 1. 统一换行符
            content = content.Replace("\r\n", "\n").Replace("\r", "\n");

            // 2. 移除多余的空白字符（但保留单个空格和换行）
            content = Regex.Replace(content, @"[ \t]+", " ");
            content = Regex.Replace(content, @"\n{3,}", "\n\n");

            // 3. 移除首尾空白
            content = content.Trim();

            return content;
        }

        /// <summary>
        /// 按段落分块
        /// </summary>
        public List<string> ChunkByParagraph(string content)
        {
            var chunks = new List<string>();

            // 按双换行符分割段落
            var paragraphs = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = new List<string>();
            var currentLength = 0;

            foreach (var paragraph in paragraphs)
            {
                var paragraphLength = paragraph.Length;

                // 如果当前块加上新段落超过限制，保存当前块
                if (currentLength + paragraphLength > _chunkSize && currentChunk.Count > 0)
                {
                    chunks.Add(string.Join("\n\n", currentChunk));

                    // 保留最后一个段落作为重叠
                    if (_chunkOverlap > 0 && currentChunk.Count > 0)
                    {
                        currentChunk = new List<string> { currentChunk[^1] };
                        currentLength = currentChunk[0].Length;
                    }
                    else
                    {
                        currentChunk.Clear();
                        currentLength = 0;
                    }
                }

                currentChunk.Add(paragraph);
                currentLength += paragraphLength;
            }

            // 添加最后一个块
            if (currentChunk.Count > 0)
            {
                chunks.Add(string.Join("\n\n", currentChunk));
            }

            return chunks;
        }

        /// <summary>
        /// 按固定大小分块
        /// </summary>
        public List<string> ChunkBySize(string content)
        {
            var chunks = new List<string>();
            var start = 0;

            while (start < content.Length)
            {
                var length = Math.Min(_chunkSize, content.Length - start);

                // 尝试在句子边界处切分
                if (start + length < content.Length)
                {
                    var lastPeriod = content.LastIndexOfAny(new[] { '。', '！', '？', '.', '!', '?' },
                                                            start + length, length);
                    if (lastPeriod > start)
                    {
                        length = lastPeriod - start + 1;
                    }
                }

                chunks.Add(content.Substring(start, length).Trim());
                start += length - _chunkOverlap;
            }

            return chunks;
        }
    }
}
