using kevin.Domain.Kevin;
using kevin.Permission.Permisson.Attributes;
using kevin.Permission.Permisson.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System.Reflection;
using System.Runtime.Loader;

namespace kevin.Permission.Permisson
{
    public static class FrameworkServiceExtension
    {

        public static List<TPermission> GetAllMenus(List<SysCtrl> ctrls)
        {
            var mdls = new List<TPermission>();
            foreach (var ctrl in ctrls)
            {
                if (!ctrl.IgnorePrivillege)
                {
                    ctrl?.Actions?.ForEach(a =>
                    {
                        var url = a.Url;
                        if (!a.IgnorePrivillege)
                            mdls.Add(new TPermission
                            {
                                ModuleName = ctrl.ModuleName,
                                ActionName = a.ActionName,
                                HttpMethod = a.HttpMethod,
                                AreaName = ctrl.Area?.AreaName ?? "",
                                Area = ctrl.Area?.Prefix ?? "",
                                Module = ctrl.Module,
                                Action = a.Action,
                                FullName = ctrl.FullName,
                                IsManual = false
                            });
                    });
                }

            }
            return mdls;
        }

        /// <summary>
        /// 获取所有继承 BaseController 控制器
        /// </summary>
        /// <param name="allAssemblies"></param>
        /// <returns></returns>
        public static List<Type> GetAllControllers(List<Assembly> allAssemblies)
        {
            var controllers = new List<Type>();
            foreach (var ass in allAssemblies)
            {
                var types = new List<Type>();
                try
                {
                    types.AddRange(ass.GetExportedTypes());
                }
                catch { }

                controllers.AddRange(types.Where(x => typeof(ControllerBase).IsAssignableFrom(x) && !x.IsAbstract).ToList());
            }
            return controllers;
        }


        /// <summary>
        /// 获取所有模块
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        public static List<SysCtrl> GetAllModules(List<Type> controllers)
        {
            var modules = new List<SysCtrl>();

            foreach (var ctrl in controllers)
            {
                var pubattr12 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                var skip12 = ctrl.GetCustomAttributes(typeof(SkipAuthorityAttribute), false);


                var debugattr = ctrl.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                var areaattr = ctrl.GetCustomAttributes(typeof(MyAreaAttribute), false);
                var model = new SysCtrl
                {
                    ClassName = ctrl.Name.Replace("Controller", string.Empty)
                };
                if (areaattr.Length == 0 && model.ClassName == "Home")
                {
                    continue;
                }
                if (pubattr12.Length > 0 || debugattr.Length > 0 || skip12.Length > 0)
                {
                    model.IgnorePrivillege = true;
                }
                if (typeof(ControllerBase).IsAssignableFrom(ctrl))
                {
                    model.IsApi = true;
                }
                model.FullName = ctrl.FullName;
                //获取controller上标记的ActionDescription属性的值
                var attrs = ctrl.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    var ada = attrs[0] as ActionDescriptionAttribute;
                    var nameKey = ada?.GetDescription(ctrl);
                    model.ModuleName = nameKey ?? "";
                    model.Module = model.ClassName;
                }
                else
                {
                    model.ModuleName = model.ClassName;
                    model.Module = model.ClassName;
                }
                //获取该controller下所有的方法
                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                //过滤掉/Login/Login方法和特殊方法
                if (model.ClassName.ToLower() == "login")
                {
                    methods = methods.Where(x => x.IsSpecialName == false && x.Name.ToLower() != "login").ToArray();
                }
                else
                {
                    methods = methods.Where(x => x.IsSpecialName == false).ToArray();
                }
                model.Actions = new List<SysAction>();
                //循环所有方法
                foreach (var method in methods)
                {
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                    var skip22 = method.GetCustomAttributes(typeof(SkipAuthorityAttribute), false);
                    var dbgOnly = method.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                    var logon = method.GetCustomAttributes(typeof(AuthorizeAttribute), false);
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        var action = new SysAction
                        {
                            Controller = model,
                            MethodName = method.Name,
                            HttpMethod = "GET"
                        };
                        if (pubattr22.Length > 0 || logon.Length > 0 || dbgOnly.Length > 0 || skip22.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }

                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            action.ActionName = ada?.Description ?? "";
                            action.Action = action.MethodName;
                        }
                        else
                        {
                            action.ActionName = action.MethodName;
                            action.Action = action.MethodName;
                        }
                        var pars = method.GetParameters();
                        if (pars != null && pars.Length > 0)
                        {
                            action.ParasToRunTest = new List<string>();
                            foreach (var par in pars)
                            {
                                action.ParasToRunTest.Add(par?.Name ?? "");
                            }
                        }
                        model.Actions.Add(action);
                    }
                }
                //再次循环所有方法
                foreach (var method in methods)
                {
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                    var skip22 = method.GetCustomAttributes(typeof(SkipAuthorityAttribute), false);


                    var debugattr2 = method.GetCustomAttributes(typeof(AuthorizeAttribute), false);

                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //找到post的方法且没有同名的非post的方法，添加到controller的action列表里
                    if (postAttr.Length > 0 && model.Actions.Where(x => x.MethodName?.ToLower() == method.Name.ToLower()).FirstOrDefault() == null)
                    {
                        if (method.Name.ToLower().StartsWith("dobatch"))
                        {
                            if (model.Actions.Where(x => "do" + x.MethodName?.ToLower() == method.Name.ToLower()).FirstOrDefault() != null)
                            {
                                continue;
                            }
                        }
                        var action = new SysAction
                        {
                            Controller = model,
                            MethodName = method.Name,
                            HttpMethod = "POST"
                        };
                        if (pubattr22.Length > 0 || debugattr2.Length > 0 || skip22.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }
                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            action.ActionName = ada?.Description ?? "";
                            action.Action = action.MethodName;
                        }
                        else
                        {
                            action.ActionName = action.MethodName;
                            action.Action = action.MethodName;
                        }
                        model.Actions.Add(action);
                    }
                }
                if (model.Actions != null && model.Actions.Count() > 0)
                {
                    if (areaattr.Length > 0)
                    {
                        string areaName = (areaattr[0] as MyAreaAttribute)?.AreaName ?? "";
                        var existArea = modules.Where(x => x.Area?.AreaName == areaName && x.ClassName == model.ClassName).Select(x => x.Area).FirstOrDefault();
                        if (existArea == null)
                        {
                            model.Area = new SysArea
                            {
                                AreaName = (areaattr[0] as MyAreaAttribute)?.AreaName ?? "",
                                Prefix = (areaattr[0] as MyAreaAttribute)?.Area ?? "",
                            };
                        }
                        else
                        {
                            model.Area = existArea;
                        }
                    }
                    modules.Add(model);
                }
            }

            return modules;
        }

        /// <summary>
        /// 获取所有无需权限验证的链接
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        public static List<string> GetAllAccessUrls(List<Type> controllers)
        {
            var rv = new List<string>();
            foreach (var ctrl in controllers)
            {
                if (typeof(ControllerBase).IsAssignableFrom(ctrl))
                {
                    continue;
                }
                var area = string.Empty;
                var ControllerName = ctrl.Name.Replace("Controller", string.Empty);
                var includeAll = false;
                //获取controller上标记的ActionDescription属性的值
                var attrs22 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                var areaAttr = ctrl.GetCustomAttribute(typeof(MyAreaAttribute), false);
                if (areaAttr != null)
                {
                    area = (areaAttr as MyAreaAttribute)?.Area;
                }
                if (attrs22.Length > 0)
                {
                    includeAll = true;
                }

                //获取该controller下所有的方法
                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                //过滤掉特殊方法
                methods = methods.Where(x => x.IsSpecialName == false).ToArray();
                var ActionName = string.Empty;
                //循环所有方法
                foreach (var method in methods)
                {
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        ActionName = method.Name;
                        var url = ControllerName + "/" + ActionName;
                        if (!string.IsNullOrEmpty(area))
                        {
                            url = area + "/" + url;
                        }
                        if (includeAll == true)
                        {
                            rv.Add(url);
                        }
                        else
                        {
                            var attrs42 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs42.Length > 0)
                            {
                                rv.Add(url);
                            }
                        }
                    }
                }
                //再次循环所有方法
                foreach (var method in methods)
                {
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //找到post的方法且没有同名的非post的方法，添加到controller的action列表里
                    if (postAttr.Length > 0 && !rv.Contains(ControllerName + "/" + method.Name))
                    {
                        ActionName = method.Name;
                        var url = ControllerName + "/" + ActionName;
                        if (!string.IsNullOrEmpty(area))
                        {
                            url = area + "/" + url;
                        }
                        if (includeAll == true)
                        {
                            rv.Add(url);
                        }
                        else
                        {
                            var attrs62 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs62.Length > 0)
                            {
                                rv.Add(url);
                            }
                        }
                    }
                }
            }
            return rv;
        }

        public static Assembly? GetRuntimeAssembly(string name)
        {
            var path = Assembly.GetEntryAssembly()?.Location;
            var library = DependencyContext.Default?.RuntimeLibraries.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (library == null)
            {
                return null;
            }
            var r = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
        {
            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)??""),
            new ReferenceAssemblyPathResolver(),
            new PackageCompilationAssemblyResolver()
        });

            var wrapper = new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable);

            var assemblies = new List<string>();
            r.TryResolveAssemblyPaths(wrapper, assemblies);
            if (assemblies.Count > 0)
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblies[0]);
            }
            else
            {
                return null;
            }
        }
    }


    /// <summary>
    /// 解决IIS InProgress下CurrentDirectory获取错误的问题
    /// </summary>
    internal class CurrentDirectoryHelpers

    {

        internal const string AspNetCoreModuleDll = "aspnetcorev2_inprocess.dll";



        [System.Runtime.InteropServices.DllImport("kernel32.dll")]

        private static extern IntPtr GetModuleHandle(string lpModuleName);



        [System.Runtime.InteropServices.DllImport(AspNetCoreModuleDll)]

        private static extern int http_get_application_properties(ref IISConfigurationData iiConfigData);



        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]

        private struct IISConfigurationData

        {

            public IntPtr pNativeApplication;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzFullApplicationPath;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzVirtualApplicationPath;

            public bool fWindowsAuthEnabled;

            public bool fBasicAuthEnabled;

            public bool fAnonymousAuthEnable;

        }



        public static void SetCurrentDirectory()

        {

            try

            {

                // Check if physical path was provided by ANCM

                var sitePhysicalPath = Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH");

                if (string.IsNullOrEmpty(sitePhysicalPath))

                {

                    // Skip if not running ANCM InProcess

                    if (GetModuleHandle(AspNetCoreModuleDll) == IntPtr.Zero)

                    {

                        return;

                    }



                    IISConfigurationData configurationData = default(IISConfigurationData);

                    if (http_get_application_properties(ref configurationData) != 0)

                    {

                        return;

                    }



                    sitePhysicalPath = configurationData.pwzFullApplicationPath;

                }



                Environment.CurrentDirectory = sitePhysicalPath;

            }

            catch

            {

                // ignore

            }

        }

    }
}
