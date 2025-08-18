using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
namespace kevin.Ioc.TieredServiceRegistration
{
    public class ReflectionScheduler
    {
        /// <summary>
        /// 是否微软官方程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        private static bool IsSystemAssembly(Assembly assembly)
        {
            var asmCompanyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            if (asmCompanyAttr is null) return false;
            else
            {
                string companyName = asmCompanyAttr.Company;
                return companyName.Contains("Microsoft");
            }
        }

        /// <summary>
        /// 是否系统程序集
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <returns></returns>
        private static bool IsSystemAssembly(string assemblyPath)
        {
            //var moduleDef = AsmResolver.DotNet.ModuleDefinition.FromFile(assemblyPath);

            //var assembly = moduleDef.Assembly;
            //if (assembly is null) return false;

            //var asmCompanyAttr = assembly.CustomAttributes.FirstOrDefault(c => c.Constructor?.DeclaringType?.FullName == typeof(AssemblyCompanyAttribute).FullName);
            //if (asmCompanyAttr is null) return false;


            //var companyName = ((AsmResolver.Utf8String?)asmCompanyAttr.Signature?.FixedArguments[0]?.Element)?.Value;
            //if (companyName is null) return false;

            return assemblyPath.Contains("Microsoft");
        }

        /// <summary>
        /// 判断文件是否是程序集
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        private static bool IsManagedAssembly(string file)
        {
            using var fs = File.OpenRead(file);
            using PEReader peReader = new PEReader(fs);
            return peReader.HasMetadata && peReader.GetMetadataReader().IsAssembly;
        }

        /// <summary>
        /// 尝试加载程序集
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <returns></returns>
        private static Assembly? TryLoadAssembly(string assemblyPath)
        {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            Assembly? assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (BadImageFormatException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (FileLoadException ex)
            {
                Debug.WriteLine(ex);
            }

            if (assembly is null)
            {
                try
                {
                    assembly = Assembly.LoadFile(assemblyPath);
                }
                catch (BadImageFormatException ex)
                {
                    Debug.WriteLine(ex);
                }
                catch (FileLoadException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return assembly;
        }

        /// <summary>
        /// 循环加载所有程序集
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAllReferencedAssemblies(bool skipSystemAssemblies = true)
        {
            Assembly? rootAssembly = Assembly.GetEntryAssembly();
            if (rootAssembly == null)
            {
                rootAssembly = Assembly.GetCallingAssembly();
            }
            var returnAssemblies = new HashSet<Assembly>(new AssemblyEquality());
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();
            assembliesToCheck.Enqueue(rootAssembly);
            if (skipSystemAssemblies && IsSystemAssembly(rootAssembly) != false)
            {
                if (IsValid(rootAssembly))
                {
                    returnAssemblies.Add(rootAssembly);
                }
            }
            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();
                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName))
                    {
                        var assembly = Assembly.Load(reference);
                        if (skipSystemAssemblies && IsSystemAssembly(assembly))
                        {
                            continue;
                        }
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        if (IsValid(assembly))
                        {
                            returnAssemblies.Add(assembly);
                        }
                    }
                }
            }

            var assemblysInBaseDir = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll", new EnumerationOptions { RecurseSubdirectories = true });

            foreach (var assemblyPath in assemblysInBaseDir)
            {
                if (!IsManagedAssembly(assemblyPath)) continue;

                AssemblyName asmName = AssemblyName.GetAssemblyName(assemblyPath);

                // 如果程序集已经加载过了就不再加载
                if (returnAssemblies.Any(x => AssemblyName.ReferenceMatchesDefinition(x.GetName(), asmName))) continue;

                if (skipSystemAssemblies && IsSystemAssembly(assemblyPath)) continue;

                Assembly? assembly = TryLoadAssembly(assemblyPath);
                if (assembly == null) continue;

                // Assembly asm = Assembly.Load(asmName);
                if (!IsValid(assembly)) continue;

                if (skipSystemAssemblies && IsSystemAssembly(assembly)) continue;

                returnAssemblies.Add(assembly);
            }
            return returnAssemblies.ToArray();
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static bool IsValid(Assembly assembly)
        {
            try
            {
                assembly.GetTypes();
                assembly.DefinedTypes.ToList();
                return true;
            }
            catch (ReflectionTypeLoadException)
            {
                return false;
            }
        }
    }

    class AssemblyEquality : EqualityComparer<Assembly>
    {
        public override bool Equals(Assembly? x, Assembly? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return AssemblyName.ReferenceMatchesDefinition(x.GetName(), y.GetName());
        }

        public override int GetHashCode([DisallowNull] Assembly obj)
        {
            return obj.GetName().FullName.GetHashCode();
        }
    } 
}
