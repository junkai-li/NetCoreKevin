
using kevin.Domain.Kevin;
using System.Reflection;

namespace kevin.Permission.Permisson
{
    /// <summary>
    /// 应用全局缓存
    /// </summary>
    public class GlobalData
    {
        /// <summary>
        /// Controllers
        /// </summary>
        private List<Type>? _allControllers;
        public List<Type> AllControllers
        {
            get
            {
                if (_allControllers == null)
                {
                    _allControllers = FrameworkServiceExtension.GetAllControllers(AllAssembly);
                }
                return _allControllers;
            }
        }
        /// <summary>
        /// 程序集
        /// </summary>
        private List<Assembly>? _allAssembly;
        public List<Assembly> AllAssembly
        {
            get
            {
                if (_allAssembly == null)
                {
                    _allAssembly = Utils.GetAllAssembly();
                }
                return _allAssembly;
            }
        }


        /// <summary>
        /// 可访问的url地址
        /// </summary>

        private List<string>? _allAccessUrls;
        public List<string> AllAccessUrls
        {
            get
            {
                if(_allAccessUrls == null)
                {
                    _allAccessUrls = FrameworkServiceExtension.GetAllAccessUrls(AllControllers);
                }
                return _allAccessUrls;
            }
        }

        /// <summary>
        /// 模块
        /// </summary>
        private List<SysCtrl>? _allModules;
        public List<SysCtrl> AllModule
        {
            get
            {
                if(_allModules == null)
                {
                    _allModules = FrameworkServiceExtension.GetAllModules(AllControllers);
                }
                return _allModules;
            }
        }
        /// <summary>
        /// 菜单
        /// </summary>
        private List<TPermission>? _allModule;
        public List<TPermission> AllModules
        {
            get
            {
                if(_allModule == null)
                {
                    _allModule = FrameworkServiceExtension.GetAllMenus(AllModule);
                }
                return _allModule;
            }
        }
    }
}
