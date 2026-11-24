using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kevin.Common.Helper.FileHandleTools
{
    /// <summary>
    /// 文档处理服务，负责清理和分块
    /// </summary>
    public class DocumentProcessor
    {
        private readonly int _chunkSize;
        private readonly int _chunkOverlap;
        /// <summary>
        /// 超长段落内单行的硬切上限（知识库的“行最大数”）。
        /// 只在段落本身已超过块上限、必须再切的场景生效；未配置或大于块上限时等价于块上限。
        /// </summary>
        private readonly int _maxLineSize;

        public DocumentProcessor(int chunkSize = 500, int chunkOverlap = 50, int? maxLineSize = null)
        {
            _chunkSize = chunkSize > 0 ? chunkSize : 500;
            _chunkOverlap = chunkOverlap > 0 ? chunkOverlap : 0;
            var lineSize = maxLineSize.GetValueOrDefault();
            _maxLineSize = lineSize > 0 ? Math.Min(lineSize, _chunkSize) : _chunkSize;
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

        /// <summary>块内段落之间的分隔符（长度计入块长度预算，否则会出现块长略超配置上限）</summary>
        private const string ParagraphSeparator = "\n\n";

        /// <summary>
        /// 按段落分块，保证「每个块的长度不超过 chunkSize」。
        /// <para>
        /// 段落本身超过块上限时必须再切：Excel 转出的 Markdown 表格行间只有单换行、没有空行，
        /// 整张表就是“一个段落”，若整段吞进一个块，块长会等于整表长度（可达几十万字），
        /// 既让向量语义严重偏离，也会把重排模型的单条/单次请求 token 预算直接打穿。
        /// </para>
        /// </summary>
        public List<string> ChunkByParagraph(string content)
        {
            var chunks = new List<string>();

            // 按双换行符分割段落
            var paragraphs = content.Split(new[] { ParagraphSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = new List<string>();
            var currentLength = 0;

            foreach (var rawParagraph in paragraphs)
            {
                foreach (var paragraph in SplitOversizeParagraph(rawParagraph))
                {
                    var separatorLength = currentChunk.Count > 0 ? ParagraphSeparator.Length : 0;

                    if (currentLength + separatorLength + paragraph.Length > _chunkSize && currentChunk.Count > 0)
                    {
                        var lastParagraph = currentChunk[^1];
                        chunks.Add(string.Join(ParagraphSeparator, currentChunk));
                        currentChunk.Clear();
                        currentLength = 0;

                        if (_chunkOverlap > 0)
                        {
                            var overlapLength = lastParagraph.Length;
                            // 预留分隔符与当前段落的长度，保证「重叠片段 + 分隔符 + 当前段落」不超块上限
                            var neededLength = _chunkSize - ParagraphSeparator.Length - paragraph.Length;

                            if (neededLength > 0 && neededLength < overlapLength)
                            {
                                currentChunk.Add(lastParagraph.Substring(overlapLength - neededLength));
                                currentLength = currentChunk[0].Length;
                            }
                        }
                    }

                    currentChunk.Add(paragraph);
                    currentLength += (currentChunk.Count > 1 ? ParagraphSeparator.Length : 0) + paragraph.Length;
                }
            }

            // 添加最后一个块
            if (currentChunk.Count > 0)
            {
                chunks.Add(string.Join(ParagraphSeparator, currentChunk));
            }

            return chunks;
        }

        /// <summary>
        /// 把超过块上限的段落再切成若干不超过块上限的片段；未超限的段落原样返回。
        /// 表格段落按行切并补齐表头，其余段落按行/句子切。
        /// </summary>
        private List<string> SplitOversizeParagraph(string paragraph)
        {
            if (paragraph.Length <= _chunkSize)
            {
                return new List<string> { paragraph };
            }

            var lines = paragraph.Split('\n');
            // Markdown 表格（至少两行以 | 开头）：按行切块，避免行被切断、切碎后丢掉列语义
            if (lines.Length > 1 && lines.Count(l => l.TrimStart().StartsWith("|")) >= 2)
            {
                return SplitTableParagraph(lines);
            }

            return SplitParagraphByLines(lines);
        }

        /// <summary>
        /// 表格段落按行切片：每片都重复表头（首行 + 紧随其后的分隔行），保证每块单独看都带列名。
        /// </summary>
        private List<string> SplitTableParagraph(string[] lines)
        {
            var headerLines = new List<string> { lines[0] };
            var bodyStart = 1;
            if (lines.Length > 1 && IsTableSeparatorRow(lines[1]))
            {
                headerLines.Add(lines[1]);
                bodyStart = 2;
            }

            var header = string.Join("\n", headerLines);
            // 表头本身占满块上限（极宽表）：重复表头反而会让每片超限，退化为普通按行切
            if (header.Length + 2 > _chunkSize)
            {
                return SplitParagraphByLines(lines);
            }

            var budget = Math.Max(1, _chunkSize - header.Length - 1);

            var pieces = new List<string>();
            var buffer = new List<string>();
            var bufferLength = 0;

            void Flush()
            {
                if (buffer.Count == 0)
                    return;
                pieces.Add(header + "\n" + string.Join("\n", buffer));
                buffer.Clear();
                bufferLength = 0;
            }

            foreach (var rawRow in lines.Skip(bodyStart))
            {
                // 单行本身装不进一个块（极端宽表）只能硬切，其余情况下行不被切断
                var rows = rawRow.Length <= budget ? new List<string> { rawRow } : HardCut(rawRow, budget);
                foreach (var row in rows)
                {
                    if (bufferLength > 0 && bufferLength + 1 + row.Length > budget)
                    {
                        Flush();
                    }
                    buffer.Add(row);
                    bufferLength = bufferLength == 0 ? row.Length : bufferLength + 1 + row.Length;
                }
            }

            Flush();
            return pieces;
        }

        /// <summary>普通超长段落按行累加到块上限；单行超过行上限时按句子优先切断</summary>
        private List<string> SplitParagraphByLines(string[] lines)
        {
            var pieces = new List<string>();
            var buffer = new List<string>();
            var bufferLength = 0;

            foreach (var rawLine in lines)
            {
                foreach (var line in SplitOversizeLine(rawLine))
                {
                    if (bufferLength > 0 && bufferLength + 1 + line.Length > _chunkSize)
                    {
                        pieces.Add(string.Join("\n", buffer));
                        buffer.Clear();
                        bufferLength = 0;
                    }
                    buffer.Add(line);
                    bufferLength = bufferLength == 0 ? line.Length : bufferLength + 1 + line.Length;
                }
            }

            if (buffer.Count > 0)
            {
                pieces.Add(string.Join("\n", buffer));
            }

            return pieces.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
        }

        /// <summary>单行超过行上限时切成多段：优先在句末标点处切，找不到就按上限硬切</summary>
        private IEnumerable<string> SplitOversizeLine(string line)
        {
            if (line.Length <= _maxLineSize)
            {
                yield return line;
                yield break;
            }

            var start = 0;
            while (start < line.Length)
            {
                var length = Math.Min(_maxLineSize, line.Length - start);
                if (start + length < line.Length)
                {
                    var lastPeriod = line.LastIndexOfAny(new[] { '。', '！', '？', '；', '.', '!', '?', ';' },
                                                         start + length, length);
                    if (lastPeriod > start)
                    {
                        length = lastPeriod - start + 1;
                    }
                }

                yield return line.Substring(start, length);
                start += length;
            }
        }

        /// <summary>是否为 Markdown 表格的分隔行（形如 | --- | :---: |）</summary>
        private static bool IsTableSeparatorRow(string line)
        {
            var trimmed = line.Trim();
            return trimmed.StartsWith("|") && trimmed.All(c => c is '|' or '-' or ':' or ' ');
        }

        /// <summary>按固定长度硬切</summary>
        private static List<string> HardCut(string text, int maxLength)
        {
            var result = new List<string>();
            for (var start = 0; start < text.Length; start += maxLength)
            {
                result.Add(text.Substring(start, Math.Min(maxLength, text.Length - start)));
            }
            return result;
        }

        /// <summary>
        /// 按固定大小分块
        /// </summary>
        public List<string> ChunkBySize(string content)
        {
            var chunks = new List<string>();
            if (string.IsNullOrEmpty(content))
                return chunks;

            var start = 0;
            // 重叠上限锁在块长的一半：重叠接近块长时步进会退化成 1 个字符，
            // 短文本被切成逐字碎片，长文本也几乎不推进（大重叠配置下直接退化成死循环似的长尾）
            var maxOverlap = Math.Max(0, _chunkSize / 2);

            while (start < content.Length)
            {
                var remaining = content.Length - start;
                var length = Math.Min(_chunkSize, remaining);
                // 本次已能取到文末：不再回退 overlap，否则尾部会被反复切出重复碎片
                var isLastChunk = length >= remaining;

                // 尝试在句子边界处切分
                if (!isLastChunk)
                {
                    var lastPeriod = content.LastIndexOfAny(new[] { '。', '！', '？', '.', '!', '?' },
                                                            start + length, length);
                    if (lastPeriod > start)
                    {
                        length = lastPeriod - start + 1;
                    }
                }

                var effectiveOverlap = Math.Min(_chunkOverlap, Math.Min(maxOverlap, Math.Max(0, length - 1)));
                chunks.Add(content.Substring(start, length).Trim());
                if (isLastChunk)
                    break;
                start += Math.Max(1, length - effectiveOverlap);
            }

            return chunks;
        }
    }
}
