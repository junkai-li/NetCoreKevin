using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin_ESL.Dto
{
    public class ChannelsDto
    {
        public string Uuid { get; set; }
        public string Direction { get; set; }
        public string Created { get; set; }
        public string State { get; set; }
        public string CallerNumber { get; set; }
        public string CalleeNumber { get; set; }
        public string Callstate { get; set; }

    }

    public class ChannelsDtoParser
    {
        public static List<ChannelsDto> ParseCsv(string csvData)
        {
            var records = new List<ChannelsDto>();

            // 分割行
            var lines = csvData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) return records;

            // 获取标题
            var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();

            // 处理数据行
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split(',').Select(f => f.Trim()).ToArray();

                var record = new ChannelsDto
                {
                    Uuid = GetFieldValue("uuid", headers, fields),
                    Direction = GetFieldValue("direction", headers, fields),
                    Created = GetFieldValue("created", headers, fields),
                    State = GetFieldValue("state", headers, fields),
                    CallerNumber = GetFieldValue("cid_num", headers, fields),
                    CalleeNumber = GetFieldValue("callee_num", headers, fields),
                    Callstate = GetFieldValue("callstate", headers, fields)
                };

                records.Add(record);
            }
            return records;
        }

        private static string GetFieldValue(string fieldName, string[] headers, string[] fields)
        {
            int index = Array.IndexOf(headers, fieldName);
            return index >= 0 && index < fields.Length ? fields[index] : null;
        }
    }
}
