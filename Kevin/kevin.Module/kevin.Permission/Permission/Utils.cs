using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace kevin.Permission.Permisson
{
    public class Utils
    {
        /// <summary>
        /// 获取反射集
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAllAssembly()
        {
            var rv = new List<Assembly>();
            var path = Assembly.GetEntryAssembly()?.Location;
            var dir = new DirectoryInfo(Path.GetDirectoryName(path) ?? "");

            string[] systemdll = new string[]
            {
                "Microsoft.",
                "System.",
                "Swashbuckle.",
                "ICSharpCode",
                "Newtonsoft.",
                "Oracle.",
                "Pomelo.",
                "SQLitePCLRaw."
            };

            var asses = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var dll in asses)
            {
                try
                {
                    if (systemdll.Any(x => dll.FullName != default && dll.FullName.StartsWith(x)) == false && !rv.Any(r => r.FullName == dll.FullName))
                    {
                        rv.Add(dll);
                    }
                }
                catch { }
            }
            return rv;
        }
    }
}
