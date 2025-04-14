using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin_ESL.Dto
{
    public static class DpuParser
    {
        public static string GetLineValueFromKey(string dpu, string key)
        {
            try
            {
                string[] lines = dpu.Split('\n');
                foreach (string line in lines)
                {
                    if (line.StartsWith($"{key}:"))
                    {
                        string value = line.Substring($"{key}:".Length).Trim();
                        return value;
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            } 
    }
}
}
